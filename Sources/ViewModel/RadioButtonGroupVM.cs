using System;
using System.Collections.Generic;
using System.Linq;

namespace CMScoutIntrinsic {

    class RadioButtonVM<T> : ViewModelBase {

        public EventHandler IsCheckedChanged;

        public RadioButtonVM(T item, Boolean isChecked) {
            Item          = item;
            _isChecked    = isChecked;
        }

        public T Item { get; set; }

        public Boolean IsChecked {
            get {
                return _isChecked;
            }

            set {
                _isChecked = value;

                RaisePropertyChanged();

                IsCheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetIsChecked(Boolean isChecked) {
            _isChecked = isChecked;

            RaisePropertyChanged(nameof(IsChecked));
        }



        private Boolean _isChecked;
    }

    class RadioButtonGroupVM<T> {

        public class ItemCheckedEventArgs : EventArgs {
            public T Item;
        }

        public event Action<Object, ItemCheckedEventArgs> ItemChecked; 

        public RadioButtonGroupVM(IEnumerable<T> items, T checkedtem, Boolean forceUncheck) {
            View = new ObservableCollectionEx< RadioButtonVM<T> >();

            foreach(T item in items) {
                RadioButtonVM<T> radioButtonVM = new RadioButtonVM<T>(item, EqualityComparer<T>.Default.Equals(item, checkedtem));

                radioButtonVM.IsCheckedChanged += OnIsCheckedChanged;
                
                View.Add(radioButtonVM);
            }

            _forceUncheck = forceUncheck;

            _checkedRadioButton = View.FirstOrDefault(item => item.IsChecked);
        }
        
        public ObservableCollectionEx< RadioButtonVM<T> > View { get; }

        public T GetCheckedItem() {
            return (_checkedRadioButton != null ? _checkedRadioButton.Item : default(T));
        }

        public Int32 GetCheckedItemIndex() {
            return  (_checkedRadioButton != null ? View.FindIndex(item => ReferenceEquals(item, _checkedRadioButton)) : -1);
        }



        private void OnIsCheckedChanged(Object sender, EventArgs args) {
            RadioButtonVM<T> senderRadioButton = ((RadioButtonVM<T>)sender);

            if(_forceUncheck) {
                if(View.FirstOrDefault(item => item.IsChecked) == null) {
                    Helpers.InvokeInUiThreadAsync(() => senderRadioButton.SetIsChecked(true));
                }
                else {
                    foreach(RadioButtonVM<T> radioButton in View) {
                        if(!ReferenceEquals(radioButton, sender)) {
                            radioButton.SetIsChecked(false);
                        }
                    }

                    _checkedRadioButton = senderRadioButton;

                    ItemChecked?.Invoke(this, new ItemCheckedEventArgs { Item = senderRadioButton.Item });
                }
            }
            else {
                if(senderRadioButton.IsChecked) {
                    _checkedRadioButton = senderRadioButton;

                    ItemChecked?.Invoke(this, new ItemCheckedEventArgs { Item = senderRadioButton.Item });
                }
            }
        }



        private Boolean          _forceUncheck;
        private RadioButtonVM<T> _checkedRadioButton;
    }

}
