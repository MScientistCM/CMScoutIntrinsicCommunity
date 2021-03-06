using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CMScoutIntrinsic {

    // ListViewBaseBehavior
    class ListViewBaseBehavior : DependencyObject, IBehavior {
        public DependencyObject AssociatedObject { get; private set; }

        public static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand),       typeof(ListViewBaseBehavior), new PropertyMetadata(null, OnItemClickCommandChanged));
        public static readonly DependencyProperty ScrollFunctionProperty   = DependencyProperty.Register("ScrollFunction",   typeof(Action<Object>), typeof(ListViewBaseBehavior), new PropertyMetadata(null, OnScrollFunctionChanged));

        public static ICommand GetItemClickCommand(DependencyObject obj) { return (ICommand)obj.GetValue(ItemClickCommandProperty); }
        public static void SetItemClickCommand(DependencyObject obj, ICommand value) { obj.SetValue(ItemClickCommandProperty, value); }

        public static Action<Object> GetScrollFunction(DependencyObject obj) { return (Action<Object>)obj.GetValue(ScrollFunctionProperty); }
        public static void SetScrollFunction(DependencyObject obj, Action<Object> value) { obj.SetValue(ScrollFunctionProperty, value); }

        private static void OnItemClickCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
        }

        private static void OnScrollFunctionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
        }

        public ICommand       ItemClickCommand { get { return (ICommand)GetValue(ItemClickCommandProperty);     } set { SetValue(ItemClickCommandProperty, value); } }
        public Action<Object> ScrollFunction   { get { return (Action<Object>)GetValue(ScrollFunctionProperty); } set { SetValue(ScrollFunctionProperty,   value); } }

        public void Attach(DependencyObject associatedObject) {
            AssociatedObject = associatedObject;

            ListViewBase listViewBase = (ListViewBase)AssociatedObject;

            listViewBase.ItemClick += OnItemClick;
            listViewBase.Loaded    += OnLoaded;
        }

        public void Detach() {
            ListViewBase listViewBase = (ListViewBase)AssociatedObject;

            listViewBase.Loaded    -= OnLoaded;
            listViewBase.ItemClick -= OnItemClick;
        }

        private void OnItemClick(Object sender, ItemClickEventArgs args) {
            ListViewBase listViewBase = (ListViewBase)AssociatedObject;

            ItemClickCommand?.Execute(args.ClickedItem);
        }

        private void OnLoaded(Object sender, RoutedEventArgs args) {
            ListViewBase listViewBase = (ListViewBase)AssociatedObject;

            if(ScrollFunction == null) {
                ScrollFunction = 
                    item => {
                        listViewBase.ScrollIntoView(item);
                    };
            }
        }
    }

    // FlyoutBehavior
    class FlyoutBehavior : DependencyObject, IBehavior {
        public DependencyObject AssociatedObject { get; private set; }

        public static readonly DependencyProperty IsOpenedProperty              = DependencyProperty.Register("IsOpened",              typeof(Boolean), typeof(FlyoutBehavior), new PropertyMetadata(false, OnIsOpenedChanged));
        public static readonly DependencyProperty IsLightDismissEnabledProperty = DependencyProperty.Register("IsLightDismissEnabled", typeof(Boolean), typeof(FlyoutBehavior), new PropertyMetadata(true,  OnIsLightDismissEnabledChanged));

        public static Boolean GetIsLightDismissEnabled(DependencyObject obj) { return (Boolean)obj.GetValue(IsLightDismissEnabledProperty); }
        public static void SetIsLightDismissEnabled(DependencyObject obj, Boolean value) { obj.SetValue(IsLightDismissEnabledProperty, value); }

        public static Boolean GetIsOpened(DependencyObject obj) { return (Boolean)obj.GetValue(IsOpenedProperty); }
        public static void SetIsOpened(DependencyObject obj, Boolean value) { obj.SetValue(IsOpenedProperty, value); }

        private static void OnIsOpenedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((FlyoutBehavior)obj).OnIsOpenedChanged(args);
        }

        private static void OnIsLightDismissEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((FlyoutBehavior)obj).OnIsLightDismissEnabledChanged(args);
        }

        public Boolean IsOpened              { get { return (Boolean)GetValue(IsOpenedProperty);              } set { SetValue(IsOpenedProperty, value);              } }
        public Boolean IsLightDismissEnabled { get { return (Boolean)GetValue(IsLightDismissEnabledProperty); } set { SetValue(IsLightDismissEnabledProperty, value); } }

        public void Attach(DependencyObject associatedObject) {
            AssociatedObject = associatedObject;

            FlyoutBase flyout = GetFlyout();

            flyout.Opened  += OnFlyoutOpened;
            flyout.Closed  += OnFlyoutClosed;
            flyout.Closing += OnFlyoutClosing;
        }

        public void Detach() {
            FlyoutBase flyout = GetFlyout();

            flyout.Closing -= OnFlyoutClosing;
            flyout.Closed  -= OnFlyoutClosed;
            flyout.Opened  -= OnFlyoutOpened;
        }

        private void OnIsOpenedChanged(DependencyPropertyChangedEventArgs args) {
            if(_ignoreIsOpenedChanged) {
                return;
            }

            FlyoutBase flyout = GetFlyout();

            if(IsOpened) {
                flyout.ShowAt((FrameworkElement)AssociatedObject);
            }
            else {
                flyout.Hide();
            }
        }

        private void OnIsLightDismissEnabledChanged(DependencyPropertyChangedEventArgs args) {
        }

        private void OnFlyoutOpened(Object sender, Object args) {
            _ignoreIsOpenedChanged = true;

            IsOpened = true;

            _ignoreIsOpenedChanged = false;
        }

        private void OnFlyoutClosed(Object sender, Object args) {
            _ignoreIsOpenedChanged = true;

            IsOpened = false;

            _ignoreIsOpenedChanged = false;
        }

        private void OnFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args) {
            if(!IsLightDismissEnabled) {
                args.Cancel = true;
            }
        }

        private FlyoutBase GetFlyout() {
            if(AssociatedObject is Button) {
                return ((Button)AssociatedObject).Flyout;
            }
            else {
                return FlyoutBase.GetAttachedFlyout((FrameworkElement)AssociatedObject);
            }
        }



        private Boolean _ignoreIsOpenedChanged;
    }

    // NumericTextBoxBehavior
    class NumericTextBoxBehavior : DependencyObject, IBehavior {
        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject) {
            AssociatedObject = associatedObject;

            TextBox textBox = (TextBox)AssociatedObject;

            _text = textBox.Text;

            textBox.TextChanged += OnTextBoxTextChanged;
        }

        public void Detach() {
            TextBox textBox = (TextBox)AssociatedObject;

            textBox.TextChanged -= OnTextBoxTextChanged;
        }

        private void OnTextBoxTextChanged(Object sender, TextChangedEventArgs args) {
            TextBox textBox = (TextBox)sender;

            String text = textBox.Text;

            if(String.IsNullOrEmpty(text)) {
                textBox.Text = "0";
            }
            else if(Int32.TryParse(text, out Int32 v)) {
                textBox.Text = v.ToString();
            }
            else {
                textBox.Text = _text;
            }

            _text = textBox.Text;

            if(textBox.Text != text) {
                textBox.SelectionStart  = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
        }



        private String _text;
    }

    // ThreeStateCheckBoxBehavior
    class ThreeStateCheckBoxBehavior : DependencyObject, IBehavior {
        public DependencyObject AssociatedObject { get; private set; }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(Object), typeof(ThreeStateCheckBoxBehavior), new PropertyMetadata(null, OnIsCheckedChanged));

        public static Object GetIsChecked(DependencyObject obj) { return (Object)obj.GetValue(IsCheckedProperty); }
        public static void SetIsChecked(DependencyObject obj, Object value) { obj.SetValue(IsCheckedProperty, value); }

        private static void OnIsCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((ThreeStateCheckBoxBehavior)obj).OnIsCheckedChanged(args);
        }

        public Object IsChecked { get { return (Object)GetValue(IsCheckedProperty); } set { SetValue(IsCheckedProperty, value); } }

        public void Attach(DependencyObject associatedObject) {
            AssociatedObject = associatedObject;

            CheckBox checkBox = (CheckBox)AssociatedObject;

            checkBox.IsChecked = (Boolean?)IsChecked;

            checkBox.Checked       += OnCheckBoxChecked;
            checkBox.Unchecked     += OnCheckBoxUnchecked;
            checkBox.Indeterminate += OnCheckBoxIndeterminate;
        }

        public void Detach() {
            CheckBox checkBox = (CheckBox)AssociatedObject;

            checkBox.Indeterminate -= OnCheckBoxIndeterminate;
            checkBox.Unchecked     -= OnCheckBoxUnchecked;
            checkBox.Checked       -= OnCheckBoxChecked;
        }

        private void OnIsCheckedChanged(DependencyPropertyChangedEventArgs args) {
            CheckBox checkBox = (CheckBox)AssociatedObject;

            checkBox.IsChecked = (Boolean?)IsChecked;
        }

        private void OnCheckBoxChecked(Object sender, RoutedEventArgs args) {
            OnCheckBoxIsCheckedChanged();
        }

        private void OnCheckBoxUnchecked(Object sender, RoutedEventArgs args) {
            OnCheckBoxIsCheckedChanged();
        }

        private void OnCheckBoxIndeterminate(Object sender, RoutedEventArgs args) {
            OnCheckBoxIsCheckedChanged();
        }

        private void OnCheckBoxIsCheckedChanged() {
            CheckBox checkBox = (CheckBox)AssociatedObject;

            IsChecked = checkBox.IsChecked;
        }
    }

    // GoToStateBehavior
    class GoToStateBehavior : DependencyObject, IBehavior {
        public DependencyObject AssociatedObject { get; private set; }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(String), typeof(GoToStateBehavior), new PropertyMetadata(null, OnStateChanged));

        public static String GetState(DependencyObject obj) { return (String)obj.GetValue(StateProperty); }
        public static void SetState(DependencyObject obj, String value) { obj.SetValue(StateProperty, value); }

        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((GoToStateBehavior)obj).OnStateChanged(args);
        }

        public String State { get { return (String)GetValue(StateProperty); } set { SetValue(StateProperty, value); } }

        public void Attach(DependencyObject associatedObject) {
            AssociatedObject = associatedObject;

            ((FrameworkElement)AssociatedObject).Loaded += OnLoaded;
        }

        public void Detach() {
        }

        private void OnLoaded(Object sender, RoutedEventArgs routedEventArgs) {
            ((FrameworkElement)AssociatedObject).Loaded -= OnLoaded;

            _isLoaded = true;

            GoToState(false);
        }

        private void OnStateChanged(DependencyPropertyChangedEventArgs args) {
            GoToState(true);
        }

        private void GoToState(Boolean useTransitions) {
            if(_isLoaded) {
                String state = State;

                if(state != null) {
                    VisualStateManager.GoToState((Control)AssociatedObject, state, useTransitions);
                }
            }
        }



        private Boolean _isLoaded;
    }

}
