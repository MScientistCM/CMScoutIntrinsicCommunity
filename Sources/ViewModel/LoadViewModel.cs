using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CMScoutIntrinsic {

    class LoadViewModel : DialogViewModelBase {
        public LoadViewModel(StorageFile file) {
            ExposeAync(file);
        }

        private async void ExposeAync(StorageFile file) {
            App app = (App)Application.Current;

            app.DataService.LoadStarted  += OnLoadStarted;
            app.DataService.LoadFinished += OnLoadFinished;

            await app.DataService.LoadAsync(file);
        }

        public StorageFile File         { get { return _file;         } set { _file         = value; RaisePropertyChanged(); } }
        public Boolean     IsLoading    { get { return _isLoading;    } set { _isLoading    = value; RaisePropertyChanged(); } }
        public String      ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; RaisePropertyChanged(); } }

        protected override void OnOnDialogClosing(ContentDialogClosingEventArgs args) {
            base.OnOnDialogClosing(args);

            if(IsLoading) {
                args.Cancel = true;
            }
        }

        private void OnLoadStarted(Object sender, DataService.LoadStartedEventArgs args) {
            File      = args.File;
            IsLoading = true;
        }

        private void OnLoadFinished(Object sender, DataService.LoadFinishedEventArgs args) {
            IsLoading = false;

            if(args.ErrorMessage != null) {
                ErrorMessage = args.ErrorMessage;
            }
            else {
                Hide.Execute(null);
            }
        }



        private StorageFile _file;
        private Boolean     _isLoading;
        private String      _errorMessage;
    }

}
