using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class ColumnVM : ViewModelBase {
        public event EventHandler IsCheckedChanged;

        public ColumnVM(String name) {
            Name = name;
        }

        public String Name { get; }

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



        private Boolean _isChecked;
    }



    class SettingsViewModel : ViewModelBase {

        public static readonly String[] Column2Names = {
            "Current Ability",
            "Potential Ability",
            "Value",
            "Wage",
            "Current Reputation",
            "Home Reputation",
            "World Reputation",
            "Left Foot",
            "Right Foot",
        };

        public SettingsViewModel() {
            App app = (App)Application.Current;

            app.SettingsService.CA18HighlightChanged += OnCA18HighlightChanged;
            app.SettingsService.CA18ViewModeChanged  += OnCA18ViewModeChanged;

            _CA18ViewModesGroup = new RadioButtonGroupVM<CA18ViewMode>(
                new [] {
                    CA18ViewMode.Intrinsic,
                    CA18ViewMode.IntrinsicGraemeKelly,
                    CA18ViewMode.IntrinsicNormalized,
                    CA18ViewMode.InGame,
                    CA18ViewMode.InMatch,
                    CA18ViewMode.InMatchNormalized
                },
                CA18ViewMode,
                false
            );

            _CA18ViewModesGroup.ItemChecked += OnCA18ViewModeChecked;

            Columns = new AttributeVM[DataService.Attributes.Length];

            List<String> columns = app.SettingsService.GetColumns();

            for(Int32 i = 0; i < DataService.Attributes.Length; ++i) {
                Columns[i] = new AttributeVM(DataService.Attributes[i]);

                Columns[i].IsChecked        =  columns.Contains(DataService.Attributes[i].Name);
                Columns[i].IsCheckedChanged += OnColumnIsCheckedChanged;
            }

            Columns2 = new ColumnVM[Column2Names.Length];

            List<String> columns2 = app.SettingsService.GetColumns2();

            for(Int32 i = 0; i < Column2Names.Length; ++i) {
                Columns2[i] = new ColumnVM(Column2Names[i]);

                Columns2[i].IsChecked        =  columns2.Contains(Column2Names[i]);
                Columns2[i].IsCheckedChanged += OnColumn2IsCheckedChanged;
            }
        }

        public Boolean IsOpened {
            get {
                return _isOpened;
            }

            set {
                _isOpened = value;

                RaisePropertyChanged();
            }
        }

        public Boolean LoadLastSave {
            get {
                return ((App)Application.Current).SettingsService.GetLoadLastSave();
            }

            set {
                ((App)Application.Current).SettingsService.SetLoadLastSave(value);


                RaisePropertyChanged();
            }
        }

        public Boolean ApplyLastFilter {
            get {
                return ((App)Application.Current).SettingsService.GetApplyLastFilter();
            }

            set {
                ((App)Application.Current).SettingsService.SetApplyLastFilter(value);


                RaisePropertyChanged();
            }
        }

        public ObservableCollectionEx< RadioButtonVM<CA18ViewMode> > CA18ViewModes => _CA18ViewModesGroup.View;

        public Boolean CA18Highlight {
            get {
                return ((App)Application.Current).SettingsService.GetCA18Highlight();
            }

            set {
                ((App)Application.Current).SettingsService.SetCA18Highlight(value);
            }
        }

        public CA18ViewMode CA18ViewMode => ((App)Application.Current).SettingsService.GetCA18ViewMode();

        public AttributeVM[] Columns  { get; }
        public ColumnVM[]    Columns2 { get; }



        private void OnCA18HighlightChanged(Object sender, EventArgs eventArgs) {
            RaisePropertyChanged(nameof(CA18Highlight));
        }

        private void OnCA18ViewModeChanged(Object sender, EventArgs eventArgs) {
            RaisePropertyChanged(nameof(CA18ViewMode));
        }

        private void OnCA18ViewModeChecked(Object sender, RadioButtonGroupVM<CA18ViewMode>.ItemCheckedEventArgs args) {
            ((App)Application.Current).SettingsService.SetCA18ViewMode(args.Item);
        }

        private void OnColumnIsCheckedChanged(Object sender, EventArgs args) {
            App app = (App)Application.Current;

            AttributeVM column = Columns.Find(item => ReferenceEquals(item, sender));

            if(column.IsChecked) {
                app.SettingsService.AddColumn(column.Name);
            }
            else {
                app.SettingsService.RemoveColumn(column.Name);
            }
        }

        private void OnColumn2IsCheckedChanged(Object sender, EventArgs args) {
            App app = (App)Application.Current;

            ColumnVM column2 = Columns2.Find(item => ReferenceEquals(item, sender));

            if(column2.IsChecked) {
                app.SettingsService.AddColumn2(column2.Name);
            }
            else {
                app.SettingsService.RemoveColumn2(column2.Name);
            }
        }



        private Boolean                          _isOpened;
        private RadioButtonGroupVM<CA18ViewMode> _CA18ViewModesGroup;
    }

}
