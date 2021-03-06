using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CMScoutIntrinsic {

    class Frame_ : ContentControl, INavigate {
		public Frame_() {
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment   = VerticalAlignment.Stretch;

            ContentTransitions = new TransitionCollection();
            ContentTransitions.Add(new EntranceThemeTransition());

            _entries = new List<PageStackEntry_>();
		}

        public event Action<Object, NavigationEventArgs_> ContentNavigatedFrom;
        public event Action<Object, NavigationEventArgs_> ContentNavigatedTo;
        public event Action<Object, NavigationEventArgs_> ContentRemoved;

        public Boolean CanGoBack {
            get {
                return _entries.Count > 1;
            }
        }

        public void GoBack() {
            if(CanGoBack) {
                PageStackEntry_ oldEntry = _entries[_entries.Count - 1];
                PageStackEntry_ newEntry = _entries[_entries.Count - 2];

                _entries.RemoveAt(_entries.Count - 1);

                Navigate(oldEntry, newEntry, null, NavigationMode.Back);

                NavigationEventArgs_ args = new NavigationEventArgs_ (
                    oldEntry.Content,
                    NavigationMode.Back,
                    null,
                    null,
                    oldEntry.SourcePageType,
                    null
                );

                (oldEntry.Content as Page_)?.CallOnRemoved(args);

                ContentRemoved?.Invoke(this, args);
            }
        }

        public void RemoveBackEntry() {
            if(CanGoBack) {
                PageStackEntry_ oldEntry = _entries[_entries.Count - 2];

                _entries.RemoveAt(_entries.Count - 2);

                NavigationEventArgs_ args = new NavigationEventArgs_ (
                    Content        = oldEntry.Content,
                    NavigationMode.Back,
                    null,
                    null,
                    oldEntry.SourcePageType,
                    null
                );

                (oldEntry.Content as Page_)?.CallOnRemoved(args);

                ContentRemoved?.Invoke(this, args);
            }
        }

        public Boolean Navigate(Type sourcePageType) {
            return Navigate(sourcePageType, null);
        }

        public Boolean Navigate(Type sourcePageType, Object parameter) {
            PageStackEntry_ oldEntry = _entries.LastOrDefault();

            PageStackEntry_ newEntry = new PageStackEntry_(
                Activator.CreateInstance(sourcePageType),
                null,
                parameter,
                sourcePageType
            );

            _entries.Add(newEntry);

            Navigate(oldEntry, newEntry, parameter, NavigationMode.New);

            return true;
        }

        private void Navigate(PageStackEntry_ oldEntry, PageStackEntry_ newEntry, Object parameter, NavigationMode navigationMode) {
            Content = newEntry.Content;

            if(oldEntry != null) {
                NavigationEventArgs_ args = new NavigationEventArgs_ (
                    oldEntry.Content,
                    navigationMode,
                    null,
                    null,
                    oldEntry.SourcePageType,
                    null
                );

                (oldEntry.Content as Page_)?.CallOnNavigatedFrom_(args);

                ContentNavigatedFrom?.Invoke(this, args);
            }

            {
                NavigationEventArgs_ args = new NavigationEventArgs_ (
                    newEntry.Content,
                    navigationMode,
                    null,
                    parameter,
                    newEntry.SourcePageType,
                    null
                );

                ContentNavigatedTo?.Invoke(this, args);

                (newEntry.Content as Page_)?.CallOnNavigatedTo_(args);
            }
        }

        

        private List<PageStackEntry_> _entries;
    }

    class Page_ : Page {

        public void CallOnNavigatedFrom_(NavigationEventArgs_ args) { OnNavigatedFrom_(args); }
        public void CallOnNavigatedTo_(NavigationEventArgs_ args)   { OnNavigatedTo_(args);   }
        public void CallOnRemoved(NavigationEventArgs_ args)        { OnRemoved(args);        }

        protected virtual void OnNavigatedFrom_(NavigationEventArgs_ args) {}
        protected virtual void OnNavigatedTo_(NavigationEventArgs_ args)   {}
        protected virtual void OnRemoved(NavigationEventArgs_ args)        {}
    }

    class GenericPage_<TViewModel> : Page_ where TViewModel : class {
        public static readonly DependencyProperty VMProperty = DependencyProperty.Register(
            "VM",
            typeof(TViewModel),
            typeof(GenericPage_<>),
            new PropertyMetadata(null, null)
        );

        public static TViewModel GetVM(DependencyObject obj) { return (TViewModel)obj.GetValue(VMProperty); }
        public static void SetVM(DependencyObject obj, TViewModel value) { obj.SetValue(VMProperty, value); }

        public TViewModel VM { get { return (TViewModel)GetValue(VMProperty); } set { SetValue(VMProperty, value); } }

        public GenericPage_() {
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            this.VM = this.DataContext as TViewModel;

            OnDataContextChanged_(args);
        }

        protected virtual void OnDataContextChanged_(DataContextChangedEventArgs args) {}
    }

    sealed class PageStackEntry_ {
        public PageStackEntry_(Object content, NavigationTransitionInfo navigationTransitionInfo, Object parameter, Type sourcePageType) {
            Content                  = content;
            NavigationTransitionInfo = navigationTransitionInfo;
            Parameter                = parameter;
            SourcePageType           = sourcePageType;
        }

        public Object                   Content                  { get; private set; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; private set; }
        public Object                   Parameter                { get; private set; }
        public Type                     SourcePageType           { get; private set; }
    }

    sealed class NavigatingCancelEventArgs_ {
        public NavigatingCancelEventArgs_(Boolean cancel, Object content, NavigationMode navigationMode, NavigationTransitionInfo navigationTransitionInfo, Object parameter, Type sourcePageType) {
            Cancel                   = cancel;
            Content                  = content;
            NavigationMode           = navigationMode;
            NavigationTransitionInfo = navigationTransitionInfo;
            Parameter                = parameter;
            SourcePageType           = sourcePageType;
        }

        public Boolean                  Cancel                   { get; set; }
        public Object                   Content                  { get; private set; }
        public NavigationMode           NavigationMode           { get; private set; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; private set; }
        public Object                   Parameter                { get; private set; }
        public Type                     SourcePageType           { get; private set; }
    }

    sealed class NavigationEventArgs_ {
        public NavigationEventArgs_(Object content, NavigationMode navigationMode, NavigationTransitionInfo navigationTransitionInfo, Object parameter, Type sourcePageType, Uri uri) {
            Content                  = content;
            NavigationMode           = navigationMode;
            NavigationTransitionInfo = navigationTransitionInfo;
            Parameter                = parameter;
            SourcePageType           = sourcePageType;
            Uri                      = uri;
        }

        public Object                   Content                  { get; private set; }
        public NavigationMode           NavigationMode           { get; private set; }
        public NavigationTransitionInfo NavigationTransitionInfo { get; private set; }
        public Object                   Parameter                { get; private set; }
        public Type                     SourcePageType           { get; private set; }
        public Uri                      Uri                      { get; private set; }
    }

}
