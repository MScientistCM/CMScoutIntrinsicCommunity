using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace CMScoutIntrinsic {

    static class Helpers {

        public static String GetAppProductId() {
            return "9MZ2B5KP0396";
        }

        public static String GetAppMajorVersion() {
            return Package.Current.Id.Version.Major.ToString();
        }

        public static String LocalDirPath = ApplicationData.Current.LocalFolder.Path;

        private static List< Tuple<StorageFolder, String, String> > _baseFolders = new List< Tuple<StorageFolder, String, String> > {
            Tuple.Create(ApplicationData.Current.LocalFolder,     ApplicationData.Current.LocalFolder.Path,     "ms-appdata:///local"),
            Tuple.Create(ApplicationData.Current.RoamingFolder,   ApplicationData.Current.RoamingFolder.Path,   "ms-appdata:///roaming"),
            Tuple.Create(ApplicationData.Current.TemporaryFolder, ApplicationData.Current.TemporaryFolder.Path, "ms-appdata:///temp"),
            Tuple.Create(Package.Current.InstalledLocation,       Package.Current.InstalledLocation.Path,       "ms-appx://"),
        };

        public static StorageFolder GetBaseFolder(String path) {
            foreach(Tuple<StorageFolder, String, String> t in _baseFolders) {
                if(path.StartsWith(t.Item2)) {
                    return t.Item1;
                }
            }

            throw new Exception(String.Format("Can't find base forder for path \"{0}\"", path));
        }

        public static String GetRelativePath(String basePath, String absolutePath) {
            if(basePath.Length == absolutePath.Length) {
                return String.Empty;
            }
            else {
                return absolutePath.Substring(basePath.Length + 1);
            }
        }

        public static String GetProtocolPath(String path) {
            foreach(Tuple<StorageFolder, String, String> t in _baseFolders) {
                if(path.StartsWith(t.Item2)) {
                    StorageFolder baseFolder = t.Item1;
                    String protocolPrefix = t.Item3;
                    String relativePath = GetRelativePath(baseFolder.Path, path);

                    return String.Format("{0}/{1}", protocolPrefix, relativePath.Replace('\\', '/'));
                }
            }

            throw new Exception(String.Format("Can't find base forder for path \"{0}\"", path));
        }

        public static String NormalizeUriSeparator(String path) {
            return path.Replace('\\', '/');
        }

        public static String NormalizePathSeparator(String path) {
            return path.Replace('/', '\\');
        }

        public static async Task<Boolean> IsFolderExistsAsync(String path) {
            return await IsItemExistsAsync(path);
        }

        public static async Task<Boolean> IsFileExistsAsync(String path) {
            return await IsItemExistsAsync(path);
        }

        private static async Task<Boolean> IsItemExistsAsync(String path) {
            StorageFolder baseFolder   = GetBaseFolder(path);
            String        relativePath = GetRelativePath(baseFolder.Path, path);

            return (await baseFolder.TryGetItemAsync(relativePath) != null);
        }

        public static async Task<StorageFolder> CreateFolderAsync(String path) {
            StorageFolder baseFolder   = GetBaseFolder(path);
            String        relativePath = GetRelativePath(baseFolder.Path, path);

            if(String.IsNullOrEmpty(relativePath)) {
                return baseFolder;
            }
            else {
                return await baseFolder.CreateFolderAsync(relativePath, CreationCollisionOption.OpenIfExists);
            }
        }

        public static async Task<StorageFile> CreateFileAsync(String path) {
            return await (await CreateFolderAsync(Path.GetDirectoryName(path))).CreateFileAsync(Path.GetFileName(path), CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<String> ReadTextAsync(String path) {
            return await FileIO.ReadTextAsync(await StorageFile.GetFileFromPathAsync(path), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }

        public static async Task WriteTextAsync(String path, String text) {
            await FileIO.WriteTextAsync(await CreateFileAsync(path), text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }

        public static async Task<IRandomAccessStreamWithContentType> OpenReadAsync(String path) {
            return await (await StorageFile.GetFileFromPathAsync(path)).OpenReadAsync();
        }

        public static async Task<IRandomAccessStream> OpenWriteAsync(String path) {
            return await (await CreateFileAsync(path)).OpenAsync(FileAccessMode.ReadWrite);
        }

        public static async Task<Stream> OpenStreamForReadAsync(String path) {
            return await (await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(path))).OpenStreamForReadAsync(Path.GetFileName(path));
        }

        public static async Task<Stream> OpenStreamForWriteAsync(String path) {
            return await OpenStreamForWriteAsync(path, CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<Stream> OpenStreamForWriteAsync(String path, CreationCollisionOption creationCollisionOption) {
            return await (await CreateFolderAsync(Path.GetDirectoryName(path))).OpenStreamForWriteAsync(Path.GetFileName(path), creationCollisionOption);
        }

        public static async Task UnZipAsync(String srcPath, String dstPath, Action<Int32, Int32> progressCallback, CancellationToken cancellationToken) {
            using(Stream srcStream = await OpenStreamForReadAsync(srcPath)) {
                await UnZipAsync(srcStream, dstPath, progressCallback, cancellationToken);
            }
        }

        public static async Task UnZipAsync(Stream srcStream, String dstPath, Action<Int32, Int32> progressCallback, CancellationToken cancellationToken) {
            using(ZipArchive zipArchive = new ZipArchive(srcStream)) {
                Int32 totalEntriesCount     = zipArchive.Entries.Count;
                Int32 extractedEntriesCount = 0;

                StorageFolder dstFolder = await CreateFolderAsync(dstPath);

                foreach(ZipArchiveEntry entry in zipArchive.Entries) {
                    String entryFullName = NormalizePathSeparator(entry.FullName);

                    if(String.IsNullOrEmpty(Path.GetFileName(entryFullName))) {
                        await dstFolder.CreateFolderAsync(Path.GetDirectoryName(entryFullName), CreationCollisionOption.OpenIfExists);
                    }
                    else {
                        using(Stream entryStream = entry.Open()) {
                            using(Stream fileStream = await dstFolder.OpenStreamForWriteAsync(entryFullName, CreationCollisionOption.ReplaceExisting)) {
                                await entryStream.CopyToAsync(fileStream);
                            }
                        }
                    }

                    ++extractedEntriesCount;

                    progressCallback?.Invoke(extractedEntriesCount, totalEntriesCount);

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public static String ReadText(String path) {
            return Encoding.UTF8.GetString(File.ReadAllBytes(path));
        }

        public static void WriteText(String path, String text) {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }



        public static Boolean HasThreadAccess() {
            return CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess;
        }

        public static async Task InvokeInUiThreadSync(Action action) {
            if(HasThreadAccess()) {
                action();
            }
            else {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => action());
            }
        }

        public static async void InvokeInUiThreadAsync(Action action) {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => action());
        }

        public static async void InvokeInBackgroundThreadAsync(Action action) {
            await Task.Run(action);
        }



        public static async Task<BitmapSource> GetBitmapSourceAsync(String path) {
            BitmapImage bitmapImage = new BitmapImage();

            if(GetBaseFolder(path).Path == Package.Current.InstalledLocation.Path) {
                using(IRandomAccessStream stream = await (await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetProtocolPath(path)))).OpenReadAsync()) {
                    await bitmapImage.SetSourceAsync(stream);
                }
            }
            else {
                using(IRandomAccessStream stream = await OpenReadAsync(path)) {
                    await bitmapImage.SetSourceAsync(stream);
                }
            }

            return bitmapImage;
        }



        private class BindingEvaluator : DependencyObject {
            public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof (Object), typeof (BindingEvaluator), new PropertyMetadata(null, null));

            public Object Target { get => GetValue(TargetProperty); set => SetValue(TargetProperty, value); }
        }

        public static Object GetValue(Object source, String propertyPath) {
            Binding binding = new Binding();

            binding.Source = source;
            binding.Path   = new PropertyPath(propertyPath);

            BindingEvaluator evaluator = new BindingEvaluator();

            BindingOperations.SetBinding(evaluator, BindingEvaluator.TargetProperty, binding);

            return evaluator.Target;
        }

        public static Object GetEnumValue(Object value, Type enumType) {
            if(value.GetType() == enumType) {
                return value;
            }
            else if(value is Int32) {
                return Enum.ToObject(enumType, value);
            }
            else if(value is String) {
                return Enum.Parse(enumType, (String)value);
            }
            else {
                throw new Exception(String.Format("GetEnumValue failed. Value: \"{0}\". Value type: \"{1}\" Enum type: \"{2}\"", value, value.GetType(), enumType));
            }
        }

        public static void ApplyStyle(DependencyObject target, Style style) {
            if(style != null) {
                if(style.BasedOn != null) {
                    foreach(Setter setter in style.BasedOn.Setters.OfType<Setter>()) {
                        target.SetValue(setter.Property, setter.Value);
                    }
                }

                foreach(Setter setter in style.Setters.OfType<Setter>()) {
                    target.SetValue(setter.Property, setter.Value);
                }
            }
        }



        public static Int32 CompareNullable<T>(T? x, T? y) where T : struct, IComparable {
            if(x == null) {
                return (y == null ? 0 : 1);
            }
            else if(y == null) {
                return -1;
            }
            else {
                return x.Value.CompareTo(y.Value);
            }
        }

        public static Int32 CompareNullable(String x, String y) {
            if(String.IsNullOrEmpty(x)) {
                return (String.IsNullOrEmpty(y) ? 0 : 1);
            }
            else if(String.IsNullOrEmpty(y)) {
                return -1;
            }
            else {
                return String.CompareOrdinal(x, y);
            }
        }



        public static String Substring(String s, Int32 startIndex, Int32 length) {
            return (startIndex < s.Length ? s.Substring(startIndex, Math.Min(length, s.Length - startIndex)) : String.Empty);
        }

        public static Boolean Contains(String str, String subStr, StringComparison comp) {
            return (str.IndexOf(subStr, comp) >= 0);
        }



        public static Color ColorFromString(String s) {
            if(s.StartsWith("#")) {
                if(s.Length == 7) {
                    Byte r = (Byte)(Convert.ToUInt32(s.Substring(1, 2), 16));
                    Byte g = (Byte)(Convert.ToUInt32(s.Substring(3, 2), 16));
                    Byte b = (Byte)(Convert.ToUInt32(s.Substring(5, 2), 16));

                    return Color.FromArgb(0xFF, r, g, b);
                }
                else {
                    Byte a = (Byte)(Convert.ToUInt32(s.Substring(1, 2), 16));
                    Byte r = (Byte)(Convert.ToUInt32(s.Substring(3, 2), 16));
                    Byte g = (Byte)(Convert.ToUInt32(s.Substring(5, 2), 16));
                    Byte b = (Byte)(Convert.ToUInt32(s.Substring(7, 2), 16));

                    return Color.FromArgb(a, r, g, b);
                }
            }

            return (Color)(typeof(Colors).GetRuntimeProperty(s)).GetValue(null);
        }

        public static String ColorToString(Color c) {
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        public static Int32 Count(this IEnumerable source) {
            if(source is ICollection) {
                return ((ICollection)source).Count;
            }

            Int32 c = 0;

            IEnumerator e = source.GetEnumerator();

            while(e.MoveNext()) {
                ++c;
            }

            if(e is IDisposable) {
                ((IDisposable)e).Dispose();
            }

            return c;
        }

        public static Object ElementAt(this IEnumerable source, Int32 index) {
            if(source is IList) {
                return ((IList)source)[index];
            }

            Object  o = null;
            Boolean f = false;

            Int32 i = 0;

            IEnumerator e = source.GetEnumerator();

            while(e.MoveNext()) {
                if(i++ == index) {
                    o = e.Current;
                    f = true;

                    break;
                }
            }

            if(e is IDisposable) {
                ((IDisposable)e).Dispose();
            }

            if(!f) {
                throw new IndexOutOfRangeException();
            }

            return o;
        }

        public static T Find<T>(this IEnumerable<T> source, Predicate<T> predicate) {
            var enumerator = source.GetEnumerator();

            while(enumerator.MoveNext()) {
                T obj = enumerator.Current;

                if(predicate(obj)) {
                    return obj;
                }
            }

            return default(T);
        }

        public static Int32 FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate) {
            var enumerator = source.GetEnumerator();

            Int32 index = 0;

            while(enumerator.MoveNext()) {
                T obj = enumerator.Current;

                if(predicate(obj)) {
                    return index;
                }

                ++index;
            }

            return -1;
        }

        public static Boolean Contains<T>(this IEnumerable<T> source, Predicate<T> predicate) {
            return (FindIndex(source, predicate) != -1);
        }

        public static T FindMin<T>(this IEnumerable<T> source, Comparison<T> comparison) {
            var enumerator = source.GetEnumerator();

            if(!enumerator.MoveNext()) {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            T minObj = enumerator.Current;

            while(enumerator.MoveNext()) {
                T obj = enumerator.Current;

                if(comparison(obj, minObj) < 0) {
                    minObj = obj;
                }
            }

            return minObj;
        }

        public static T FindMax<T>(this IEnumerable<T> source, Comparison<T> comparison) {
            var enumerator = source.GetEnumerator();

            if(!enumerator.MoveNext()) {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            T maxObj = enumerator.Current;

            while(enumerator.MoveNext()) {
                T obj = enumerator.Current;

                if(comparison(obj, maxObj) > 0) {
                    maxObj = obj;
                }
            }

            return maxObj;
        }

    }

}
