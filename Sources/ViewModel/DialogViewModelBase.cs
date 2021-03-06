using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace CMScoutIntrinsic {

    class DialogViewModelBase : ViewModelBase {
        public DialogViewModelBase() {
        }

        public ICommand Hide { get; set; }

        public void CallOnDialogClosing(ContentDialogClosingEventArgs args) { OnOnDialogClosing(args); }

        protected virtual void OnOnDialogClosing(ContentDialogClosingEventArgs args) {}
    }

}
