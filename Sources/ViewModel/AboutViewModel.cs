using System;
using System.Windows.Input;
using Windows.System;

namespace CMScoutIntrinsic {

    class AboutViewModel : ViewModelBase {

        public AboutViewModel() {
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

        public String AppVersion => Helpers.GetAppMajorVersion();

        public ICommand RateApp {
            get {
                if(_rateApp == null) {
                    _rateApp = new RelayCommand(
                        async param => {
                            await Launcher.LaunchUriAsync(new Uri(String.Format("ms-windows-store://review/?ProductId={0}", Helpers.GetAppProductId())));
                        }
                    );
                }

                return _rateApp;
            }
        }


        
        private Boolean      _isOpened;
        private RelayCommand _rateApp;
    }

}
