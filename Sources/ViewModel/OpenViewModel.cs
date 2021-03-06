using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class OpenViewModel : ViewModelBase {

        public OpenViewModel() {
            App app = (App)Application.Current;

            app.SettingsService.MruFilesChanged += OnMruFilesChanged;
            app.DataService.LoadStarted         += OnLoadStarted;
            app.DataService.LoadFinished        += OnLoadFinished;

            MruFiles = new ObservableCollectionEx<MruFileVM>();

            RefreshMruFiles();
        }

        public Boolean IsOpened {
            get {
                return _isOpened;
            }

            set {
                _isOpened = value;

                RaisePropertyChanged();
            }
        }

        public ObservableCollectionEx<MruFileVM> MruFiles { get; set; }

        public ICommand OpenOldFile {
            get {
                return _openOldFile ?? (
                    _openOldFile = new RelayCommand(
                        async param => {
                            MruFileVM mruFile = (MruFileVM)param;

                            IsOpened = false;

                            await OpenFileAsync(mruFile.Value.File);
                        }
                    )
                );
            }
        }

        public ICommand OpenNewFile {
            get {
                return _openNewFile ?? (
                    _openNewFile = new RelayCommand(
                        async param => {
                            App app = (App) Application.Current;

                            StorageFile file = await app.NavigationService.PickSingleFileAsync(new [] { "*", ".sav" });

                            if(file != null) {
                                await OpenFileAsync(file);
                            }
                        }
                    )
                );
            }
        }

        public StorageFile OpenedFile { get; private set; }

        private void RefreshMruFiles() {
            App app = (App)Application.Current;

            MruFiles.Clear();

            foreach(MruFile mruFile in app.SettingsService.GetMruFiles()) {
                MruFiles.Add(new MruFileVM(mruFile));
            }
        }

        private async Task OpenFileAsync(StorageFile file) {
            App app = (App)Application.Current;

            await app.NavigationService.ShowContentDialogAsync(new LoadViewModel(file));
        }

        private void OnMruFilesChanged(Object sender, EventArgs args) {
            RefreshMruFiles();
        }

        private void OnLoadStarted(Object sender, DataService.LoadStartedEventArgs args) {
            OpenedFile = null;
            RaisePropertyChanged(nameof(OpenedFile));
        }

        private void OnLoadFinished(Object sender, DataService.LoadFinishedEventArgs args) {
            App app = (App)Application.Current;

            if(args.ErrorMessage == null) {
                app.SettingsService.UpdateMruFile(args.File);

                OpenedFile = args.File;
                RaisePropertyChanged(nameof(OpenedFile));
            }
            else {
                app.SettingsService.RemoveMruFile(args.File);
            }
        }



        private Boolean      _isOpened;
        private RelayCommand _openOldFile;
        private RelayCommand _openNewFile;
    }

}
