using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CMScoutIntrinsic {

   abstract class NotifyPropertyChangedBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
