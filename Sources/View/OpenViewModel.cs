using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CMScoutIntrinsic {

    class MainViewModel : PageViewModelBase {
        public MainViewModel() {
        }

        public ICommand Open {
            get {
                if(_open == null) {
                    _open = new RelayCommand(
                        async param => {
                            OpenDialog openDialog = new OpenDialog();

                            await openDialog.ShowAsync();
                        }
                    );
                }

                return _open;
            }
        }



        private RelayCommand _open;
    }

}
