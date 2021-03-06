using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CMScoutIntrinsic {

    sealed class AppBarButton : ContentControl {

        public static readonly DependencyProperty IsCompactProperty = DependencyProperty.Register("IsCompact", typeof(Boolean),     typeof(AppBarButton), new PropertyMetadata(false, OnIsCompactChanged));
        public static readonly DependencyProperty IconProperty      = DependencyProperty.Register("Icon",      typeof(IconElement), typeof(AppBarButton), new PropertyMetadata(null,  OnIconChanged));
        public static readonly DependencyProperty LabelProperty     = DependencyProperty.Register("Label",     typeof(String),      typeof(AppBarButton), new PropertyMetadata(null,  OnLabelChanged));
        public static readonly DependencyProperty CommandProperty   = DependencyProperty.Register("Command",   typeof(ICommand),    typeof(AppBarButton), new PropertyMetadata(null,  OnCommandChanged));

        public static Boolean GetIsCompact(DependencyObject obj) { return (Boolean)obj.GetValue(IsCompactProperty); }
        public static void SetIsCompact(DependencyObject obj, Boolean value) { obj.SetValue(IsCompactProperty, value); }

        public static IconElement GetIcon(DependencyObject obj) { return (IconElement)obj.GetValue(IconProperty); }
        public static void SetIcon(DependencyObject obj, IconElement value) { obj.SetValue(IconProperty, value); }

        public static String GetLabel(DependencyObject obj) { return (String)obj.GetValue(LabelProperty); }
        public static void SetLabel(DependencyObject obj, String value) { obj.SetValue(LabelProperty, value); }

        public static ICommand CommandLabel(DependencyObject obj) { return (ICommand)obj.GetValue(CommandProperty); }
        public static void SetCommand(DependencyObject obj, ICommand value) { obj.SetValue(CommandProperty, value); }

        public Boolean     IsCompact { get { return (Boolean)GetValue(IsCompactProperty); } set { SetValue(IsCompactProperty, value); } }
        public IconElement Icon      { get { return (IconElement)GetValue(IconProperty);  } set { SetValue(IconProperty,      value); } }
        public String      Label     { get { return (String)GetValue(LabelProperty);      } set { SetValue(LabelProperty,     value); } }
        public ICommand    Command   { get { return (ICommand)GetValue(CommandProperty);  } set { SetValue(CommandProperty,   value); } }

        private static void OnIsCompactChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}

        public AppBarButton() {
            this.DefaultStyleKey = typeof(AppBarButton);

            this.AccessKeyInvoked += OnAccessKeyInvoked;
        }

        private void OnAccessKeyInvoked(UIElement sender, AccessKeyInvokedEventArgs args) {
            args.Handled = true;

            if(Command != null) {
                if(Command.CanExecute(null)) {
                    Command.Execute(null);
                }
            }
        }
    }

}
