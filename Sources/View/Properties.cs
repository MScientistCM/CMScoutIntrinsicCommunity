using System;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    public static class Properties {

        // EnString
        public static readonly DependencyProperty EnStringProperty = DependencyProperty.RegisterAttached("EnString", typeof(String), typeof(Properties), null);
        public static String GetEnString(DependencyObject obj) { return (String)obj.GetValue(EnStringProperty); }
        public static void SetEnString(DependencyObject obj, String value) { obj.SetValue(EnStringProperty, value); }

        // DataContext
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached("DataContext", typeof(Object), typeof(Properties), null);
        public static Object GetDataContext(DependencyObject obj) { return (Object)obj.GetValue(DataContextProperty); }
        public static void SetDataContext(DependencyObject obj, Object value) { obj.SetValue(DataContextProperty, value); }

        // Style
        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(Properties), new PropertyMetadata(null, OnStyleChanged));
        public static Style GetStyle(DependencyObject obj) { return (Style)obj.GetValue(StyleProperty); }
        public static void SetStyle(DependencyObject obj, Style value) { obj.SetValue(StyleProperty, value); }

        private static void OnStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            Helpers.ApplyStyle(sender, args.NewValue as Style);
        }
    }

}
