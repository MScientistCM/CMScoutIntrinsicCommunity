using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();

            this.EnteredBackground += OnEnteredBackground;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args) {
            Frame_ rootFrame = Window.Current.Content as Frame_;

            if(rootFrame == null) {
                // WindowWidth and WindowHeight
                Window.Current.SizeChanged += (sender_, args_) => {
                    RaisePropertyChanged(nameof(WindowWidth));
                    RaisePropertyChanged(nameof(WindowHeight));
                };

                // LocalizationService
                LocalizationService = new LocalizationService();
                await LocalizationService.ExposeAsync();

                //ApplicationView.PreferredLaunchViewSize = new Size(1024 - 2, 768 - 33 - 40);
                ApplicationView.PreferredLaunchViewSize = new Size(1366 - 2, 837 - 33);

                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

                rootFrame = new Frame_();

                Window.Current.Content = rootFrame;

                if(!args.PrelaunchActivated) {
                    Window.Current.Activate();
                }

                SettingsService   = new SettingsService();
                DataService       = new DataService();
                NavigationService = new NavigationService(rootFrame);

                await SettingsService.ExposeAsync();
                await DataService.ExposeAsync();
                await NavigationService.ExposeAsync();

                MainViewModel = new MainViewModel();

                if(args.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }

                NavigationService.Navigate(MainViewModel);

                Debug.WriteLine("Installation folder path: \"{0}\"", Package.Current.InstalledLocation.Path, 0);
                Debug.WriteLine("Local folder path: \"{0}\"", ApplicationData.Current.LocalFolder.Path, 0);
            }
            else {
                if(!args.PrelaunchActivated) {
                    Window.Current.Activate();
                }
            }
        }

        private void OnEnteredBackground(Object sender, EnteredBackgroundEventArgs args) {
            var deferral = args.GetDeferral();

            //TODO: Save application state and stop any background activity
            SettingsService.SaveSettings();

            deferral.Complete();
        }



        public Double WindowWidth  => Window.Current.Bounds.Width;
        public Double WindowHeight => Window.Current.Bounds.Height;



        internal LocalizationService LocalizationService { get; private set; }
        internal SettingsService     SettingsService     { get; private set; }
        internal DataService         DataService         { get; private set; }
        internal NavigationService   NavigationService   { get; private set; }
        internal MainViewModel       MainViewModel       { get; private set; }
    }

}
