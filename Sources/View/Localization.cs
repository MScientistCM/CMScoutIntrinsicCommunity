using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CMScoutIntrinsic {

    public class LocString {
        public String Context { get; set; }
        public String Source  { get; set; }
    }

    public class Loc {

        // String
        public static readonly DependencyProperty StringProperty = DependencyProperty.RegisterAttached("String", typeof(LocString), typeof(Loc), new PropertyMetadata(null, OnStringChanged));
        public static LocString GetString(DependencyObject obj) { return (LocString)obj.GetValue(StringProperty); }
        public static void SetString(DependencyObject obj, LocString value) { obj.SetValue(StringProperty, value); }

        // Context
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context", typeof(String), typeof(Loc), new PropertyMetadata(null, OnContextChanged));
        public static String GetContext(DependencyObject obj) { return (String)obj.GetValue(ContextProperty); }
        public static void SetContext(DependencyObject obj, String value) { obj.SetValue(ContextProperty, value); }

        // Text
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(String), typeof(Loc), new PropertyMetadata(null, OnSourceChanged));
        public static String GetSource(DependencyObject obj) { return (String)obj.GetValue(SourceProperty); }
        public static void SetSource(DependencyObject obj, String value) { obj.SetValue(SourceProperty, value); }

        // Language
        public static readonly DependencyProperty LanguageProperty = DependencyProperty.RegisterAttached("Language", typeof(String), typeof(Loc), new PropertyMetadata(null, OnLanguageChanged));
        public static String GetLanguage(DependencyObject obj) { return (String)obj.GetValue(LanguageProperty); }
        public static void SetLanguage(DependencyObject obj, String value) { obj.SetValue(LanguageProperty, value); }



        private static void OnStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            LocString ls = GetString(sender);

            UpdateTarget(sender, ls?.Context, ls?.Source);
        }

        private static void OnContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            //UpdateTarget(sender, GetContext(sender), GetSource(sender));
        }

        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            UpdateTarget(sender, GetContext(sender), GetSource(sender));
        }

        private static void OnLanguageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
        }



        private static void UpdateTarget(DependencyObject target, String context, String source) {
            String trText = String.Empty;

            if(context != null && source != null) {
                trText = ((App)Application.Current).LocalizationService.Translate(context, source);
            }

            if(target is TextBlock) {
                ((TextBlock)target).Text = trText;
            }
            else if(target is CMScoutIntrinsic.AppBarButton) { // Control -> ContentControl
                ((CMScoutIntrinsic.AppBarButton)target).Label = trText;
            }
            else if(target is Windows.UI.Xaml.Controls.AppBarButton) { // Control -> ContentControl -> ButtonBase -> Button -> AppBarButton
                ((Windows.UI.Xaml.Controls.AppBarButton)target).Label = trText;
            }
            else if(target is Button) { // Control -> ContentControl -> ButtonBase -> Button
                ((Button)target).Content = trText;
            }
            else if(target is AppBarToggleButton) { // Control -> ContentControl -> ButtonBase -> ToggleButton -> AppBarToggleButton
                ((AppBarToggleButton)target).Label = trText;
            }
            else if(target is ToggleButton) { // Control -> ContentControl -> ButtonBase -> ToggleButton
                ((ToggleButton)target).Content = trText;
            }
            else if(target is HyperlinkButton) { // Control -> ContentControl -> ButtonBase -> HyperlinkButton
                ((HyperlinkButton)target).NavigateUri = new Uri(trText);
            }
            else if(target is ToggleMenuFlyoutItem) { // Control -> MenuFlyoutItemBase -> MenuFlyoutItem -> ToggleMenuFlyoutItem
                ((ToggleMenuFlyoutItem)target).Text = trText;
            }
            else if(target is PivotItem) { // Control -> ContentControl -> PivotItem
                ((PivotItem)target).Header = trText;
            }
            else if(target is GroupBoxControl) { // Control -> ContentControl
                ((GroupBoxControl)target).Header = trText;
            }
        }
    }

}
