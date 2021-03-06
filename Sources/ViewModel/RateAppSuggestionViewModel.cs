using System;
using System.Windows.Input;
using Windows.System;

namespace CMScoutIntrinsic {

    class RateAppSuggestionViewModel : DialogViewModelBase {
        public RateAppSuggestionViewModel() {
        }

        public ICommand RateApp {
            get {
                if(_rateApp == null) {
                    _rateApp = new RelayCommand(
                        async param => {
                            Hide.Execute(null);

                            await Launcher.LaunchUriAsync(new Uri(String.Format("ms-windows-store://review/?ProductId={0}", Helpers.GetAppProductId())));
                        }
                    );
                }

                return _rateApp;
            }
        }



        private RelayCommand _rateApp;
    }

}
