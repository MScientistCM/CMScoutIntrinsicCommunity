using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CMScoutIntrinsic {

    class NavigationService {
        public NavigationService(Frame_ frame) {
            _frame = frame;
        }

        public async Task ExposeAsync() {
            _defaultNamespace = typeof(App).GetTypeInfo().Assembly.GetName().Name;

            _frame.ContentNavigatedFrom += OnFrameContentNavigatedFrom;
            _frame.ContentNavigatedTo   += OnFrameContentNavigatedTo;
            _frame.ContentRemoved       += OnFrameContentRemoved;

            _back  = new RelayCommand(param => GoBack(), param => CanGoBack());

            await Task.FromResult(false);
        }

        public Boolean CanGoBack() {
            return _frame.CanGoBack;
        }

        public void GoBack() {
            _frame.GoBack();
        }

        public void RemoveBackEntry() {
            _frame.RemoveBackEntry();
        }

        public void Navigate(PageViewModelBase viewModel) {
            String viewModelTypeName = viewModel.GetType().Name;
            String viewTypeName      = viewModelTypeName.Replace("ViewModel", "Page");
            Type   viewType          = Type.GetType(String.Format("{0}.{1}", _defaultNamespace, viewTypeName));

            viewModel.Back = _back;

            _frame.Navigate(viewType, viewModel);
        }

        public async Task ShowContentDialogAsync(DialogViewModelBase viewModel) {
            String viewModelTypeName = viewModel.GetType().Name;
            String viewTypeName      = viewModelTypeName.Replace("ViewModel", "Dialog");
            Type   viewType          = Type.GetType(String.Format("{0}.{1}", _defaultNamespace, viewTypeName));

            ContentDialog view = (ContentDialog)Activator.CreateInstance(viewType);

            //view.MaxWidth  = Window.Current.Bounds.Width;
            //view.MaxHeight = Window.Current.Bounds.Height;

            viewModel.Hide = new RelayCommand(param => view.Hide());

            view.DataContext = viewModel;

            view.Closing += OnDialogClosing;

            await view.ShowAsync();

            view.Closing -= OnDialogClosing;
        }

        public async Task<StorageFile> PickSingleFileAsync(String[] fileTypeFilters) {
            FileOpenPicker picker = new FileOpenPicker();

            foreach(String fileTypeFilter in fileTypeFilters) {
                picker.FileTypeFilter.Add(fileTypeFilter);
            }

            return await picker.PickSingleFileAsync();
        }

        public async Task<StorageFile> PickSaveFileAsync(Pair< String, List<String> >[] fileTypeFilters, String suggestedFileName) {
            FileSavePicker picker = new FileSavePicker();

            foreach(Pair< String, List<String> > fileTypeFilter in fileTypeFilters) {
                picker.FileTypeChoices.Add(fileTypeFilter.First, fileTypeFilter.Second);
            }

            picker.SuggestedFileName = suggestedFileName;

            return await picker.PickSaveFileAsync();
        }



        private void OnFrameContentNavigatedFrom(Object sender, NavigationEventArgs_ args) {
            Page_             view      = (Page_)args.Content;
            PageViewModelBase viewModel = (PageViewModelBase)view.DataContext;

            Boolean isBack = (args.NavigationMode == NavigationMode.Back);

            viewModel.CallOnNavigatedFrom(isBack);
        }

        private void OnFrameContentNavigatedTo(Object sender, NavigationEventArgs_ args) {
            Page_             view      = (Page_)args.Content;
            PageViewModelBase viewModel = null;

            Boolean isNew = (args.NavigationMode == NavigationMode.New);

            if(isNew) {
                viewModel = (PageViewModelBase)args.Parameter;

                view.DataContext = viewModel;
            }
            else {
                viewModel = (PageViewModelBase)view.DataContext;
            }

            viewModel.CallOnNavigatedTo(isNew);
        }

        private void OnFrameContentRemoved(Object sender, NavigationEventArgs_ args) {
            Page_             view      = (Page_)args.Content;
            PageViewModelBase viewModel = (PageViewModelBase)view.DataContext;

            viewModel.CallOnRemoved();
        }

        private void OnDialogClosing(ContentDialog sender, ContentDialogClosingEventArgs args) {
            DialogViewModelBase viewModel = (DialogViewModelBase)sender.DataContext;

            viewModel.CallOnDialogClosing(args);
        }



        private Frame_   _frame;
        private String   _defaultNamespace;
        private ICommand _back;
    }

}
