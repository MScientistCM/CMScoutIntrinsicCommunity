using System;
using System.Windows.Input;

namespace CMScoutIntrinsic {

    class PageViewModelBase : ViewModelBase {
        public PageViewModelBase() {
        }

        public ICommand Back { get; set; }

        public void CallOnNavigatedFrom(Boolean isBack) { OnNavigatedFrom(isBack); }
        public void CallOnNavigatedTo(Boolean isNew)    { OnNavigatedTo(isNew);    }
        public void CallOnRemoved()                     { OnRemoved();             }

        protected virtual void OnNavigatedFrom(Boolean isBack) {}
        protected virtual void OnNavigatedTo(Boolean isNew)    {}
        protected virtual void OnRemoved()                     {}
    }

}
