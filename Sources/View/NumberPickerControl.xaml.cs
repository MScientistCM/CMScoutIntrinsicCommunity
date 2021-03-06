using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CMScoutIntrinsic {

    sealed class NumberPickerControl : ContentControl {
        public static readonly DependencyProperty FromProperty              = DependencyProperty.Register("From",              typeof(Int32),  typeof(NumberPickerControl), new PropertyMetadata(0,     OnFromChanged));
        public static readonly DependencyProperty ToProperty                = DependencyProperty.Register("To",                typeof(Int32),  typeof(NumberPickerControl), new PropertyMetadata(0,     OnToChanged));
        public static readonly DependencyProperty SelectedNumberProperty    = DependencyProperty.Register("SelectedNumber",    typeof(Int32),  typeof(NumberPickerControl), new PropertyMetadata(0,     OnSelectedNumberChanged));
        public static readonly DependencyProperty HideSignButtonsProperty   = DependencyProperty.Register("HideSignButtons",   typeof(Int32),  typeof(NumberPickerControl), new PropertyMetadata(false, OnHideSignButtonsChanged));
        public static readonly DependencyProperty ButtonNumberWidthProperty = DependencyProperty.Register("ButtonNumberWidth", typeof(Double), typeof(NumberPickerControl), new PropertyMetadata(48.0,  OnButtonNumberWidthChanged));

        public static Int32 GetFrom(DependencyObject obj) { return (Int32)obj.GetValue(FromProperty); }
        public static void SetFrom(DependencyObject obj, Int32 value) { obj.SetValue(FromProperty, value); }

        public static Int32 GetTo(DependencyObject obj) { return (Int32)obj.GetValue(ToProperty); }
        public static void SetTo(DependencyObject obj, Int32 value) { obj.SetValue(ToProperty, value); }

        public static Int32 GetSelectedNumber(DependencyObject obj) { return (Int32)obj.GetValue(SelectedNumberProperty); }
        public static void SetSelectedNumber(DependencyObject obj, Int32 value) { obj.SetValue(SelectedNumberProperty, value); }

        public static Boolean GetHideSignButtons(DependencyObject obj) { return (Boolean)obj.GetValue(HideSignButtonsProperty); }
        public static void SetHideSignButtons(DependencyObject obj, Boolean value) { obj.SetValue(HideSignButtonsProperty, value); }

        public static Double GetButtonNumberWidth(DependencyObject obj) { return (Double)obj.GetValue(ButtonNumberWidthProperty); }
        public static void SetButtonNumberWidth(DependencyObject obj, Double value) { obj.SetValue(ButtonNumberWidthProperty, value); }

        public Int32   From              { get { return (Int32)GetValue(FromProperty);               } set { SetValue(FromProperty,              value); } }
        public Int32   To                { get { return (Int32)GetValue(ToProperty);                 } set { SetValue(ToProperty,                value); } }
        public Int32   SelectedNumber    { get { return (Int32)GetValue(SelectedNumberProperty);     } set { SetValue(SelectedNumberProperty,    value); } }
        public Boolean HideSignButtons   { get { return (Boolean)GetValue(HideSignButtonsProperty);  } set { SetValue(HideSignButtonsProperty,   value); } }
        public Double  ButtonNumberWidth { get { return (Double)GetValue(ButtonNumberWidthProperty); } set { SetValue(ButtonNumberWidthProperty, value); } }

        private static void OnFromChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((NumberPickerControl)obj).OnFromChanged(args); }
        private static void OnToChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((NumberPickerControl)obj).OnToChanged(args); }
        private static void OnSelectedNumberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((NumberPickerControl)obj).OnSelectedNumberChanged(args); }
        private static void OnHideSignButtonsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((NumberPickerControl)obj).OnHideSignButtonsChanged(args); }
        private static void OnButtonNumberWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((NumberPickerControl)obj).OnButtonNumberWidthChanged(args); }



        public NumberPickerControl() {
            this.DefaultStyleKey = typeof(NumberPickerControl);
        }

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _buttonMinusCommand = new RelayCommand(
                param => {
                    if(SelectedNumber > From) {
                        SelectedNumber = SelectedNumber - 1;
                    }
                },

                param => SelectedNumber > From
            );

            _buttonPlusCommand = new RelayCommand(
                param => {
                    if(SelectedNumber < To) {
                        SelectedNumber = SelectedNumber + 1;
                    }
                },

                param => SelectedNumber < To
            );

            _buttonMinCommand = new RelayCommand(
                param => {
                    if(SelectedNumber != From) {
                        SelectedNumber = From;

                        _buttonNumber.Flyout.Hide();
                    }
                },

                param => SelectedNumber != From
            );

            _buttonMaxCommand = new RelayCommand(
                param => {
                    if(SelectedNumber != To) {
                        SelectedNumber = To;

                        _buttonNumber.Flyout.Hide();
                    }
                },

                param => SelectedNumber != To
            );

            _buttonMinus  = (Button)GetTemplateChild("ButtonMinus");
            _buttonPlus   = (Button)GetTemplateChild("ButtonPlus");
            _buttonNumber = (Button)GetTemplateChild("ButtonNumber");
            _buttonMin    = (Button)GetTemplateChild("ButtonMin");
            _buttonMax    = (Button)GetTemplateChild("ButtonMax");
            _listView     = (ListView)GetTemplateChild("ListView");

            _buttonMinus.Command = _buttonMinusCommand;
            _buttonPlus.Command  = _buttonPlusCommand;

            _buttonNumber.Flyout.Opened += OnFlyoutOpened;

            _buttonMin.Command   = _buttonMinCommand;
            _buttonMax.Command   = _buttonMaxCommand;

            _listView.ItemClick += OnListViewItemClick;

            UpdateListViewItemsSource();
        }

        private void OnFromChanged(DependencyPropertyChangedEventArgs args) {
            UpdateListViewItemsSource();
        }

        private void OnToChanged(DependencyPropertyChangedEventArgs args) {
            UpdateListViewItemsSource();
        }

        private void OnSelectedNumberChanged(DependencyPropertyChangedEventArgs args) {
            _buttonMinusCommand?.RaiseCanExecuteChanged();
            _buttonPlusCommand?.RaiseCanExecuteChanged();
            _buttonMinCommand?.RaiseCanExecuteChanged();
            _buttonMaxCommand?.RaiseCanExecuteChanged();
        }

        private void OnHideSignButtonsChanged(DependencyPropertyChangedEventArgs args) {
        }

        private void OnButtonNumberWidthChanged(DependencyPropertyChangedEventArgs args) {
        }

        private void OnFlyoutOpened(Object sender, Object args) {
            _listView.SelectedItem = SelectedNumber;

            _listView.UpdateLayout();

            _listView.ScrollIntoView(_listView.SelectedItem);
        }

        private void OnListViewItemClick(Object sender, ItemClickEventArgs args) {
            SelectedNumber = (Int32)args.ClickedItem;

            _buttonNumber.Flyout.Hide();
        }

        private void UpdateListViewItemsSource() {
            if(_listView == null) {
                return;
            }

            ObservableCollectionEx<Int32> itemsSource = (ObservableCollectionEx<Int32>)_listView.ItemsSource;

            if(itemsSource == null) {
                itemsSource = new ObservableCollectionEx<Int32>();

                _listView.ItemsSource = itemsSource;
            }

            List<Int32> newItemsSource = new List<Int32>();

            for(Int32 n = From; n <= To; ++n) {
                newItemsSource.Add(n);
            }

            itemsSource.Reset(newItemsSource);
        }



        private RelayCommand _buttonMinusCommand;
        private RelayCommand _buttonPlusCommand;
        private RelayCommand _buttonMinCommand;
        private RelayCommand _buttonMaxCommand;
        private Button       _buttonMinus;
        private Button       _buttonPlus;
        private Button       _buttonNumber;
        private Button       _buttonMin;
        private Button       _buttonMax;
        private ListView     _listView;
    }

}
