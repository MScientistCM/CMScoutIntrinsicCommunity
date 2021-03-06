using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class WeightsViewModel : ViewModelBase {

        public event EventHandler WeightsSetApplied;

        public WeightsViewModel() {
            App app = (App)Application.Current;

            RatingPositions = new RatingPositionVM[DataService.RatingPositions.Length];

            for(Int32 i = 0; i < DataService.RatingPositions.Length; ++i) {
                RatingPositions[i] = new RatingPositionVM(DataService.RatingPositions[i]);
            }

            Attributes = new AttributeVM[DataService.Attributes.Length];

            for(Int32 i = 0; i < DataService.Attributes.Length; ++i) {
                Attributes[i] = new AttributeVM(DataService.Attributes[i]);
            }

            WeightsSets = new ObservableCollectionEx<WeightsSetVM>();

            foreach(WeightsSet weightsSet in app.SettingsService.GetWeightsSets()) {
                WeightsSets.Add(new WeightsSetVM(weightsSet));
            }

            SelectedWeightsSet = WeightsSets.Find(item => item.IsLast);

            if(SelectedWeightsSet == null) {
                if(WeightsSets.Count != 0) {
                    SelectedWeightsSet = WeightsSets[0];
                }
            }
        }

        public Boolean IsOpened {
            get {
                return _isOpened;
            }

            set {
                _isOpened = value;

                RaisePropertyChanged();

                App app = (App)Application.Current;

                if(!_isOpened) {
                    app.SettingsService.SaveSettings();
                }
            }
        }

        public RatingPositionVM[] RatingPositions { get; }
        public AttributeVM[]      Attributes      { get; }

        public Int32 MaxWeight => 25;

        public ObservableCollectionEx<WeightsSetVM> WeightsSets { get; }

        public WeightsSetVM SelectedWeightsSet {
            get {
                return _selectedWeightsSet;
            }

            set {
                _selectedWeightsSet = value;

                RaisePropertyChanged();

                _new?.RaiseCanExecuteChanged();
                _copy?.RaiseCanExecuteChanged();
                _delete?.RaiseCanExecuteChanged();
                _moveUp?.RaiseCanExecuteChanged();
                _moveDown?.RaiseCanExecuteChanged();
                _reset?.RaiseCanExecuteChanged();
                _apply?.RaiseCanExecuteChanged();
                _import?.RaiseCanExecuteChanged();
                _export?.RaiseCanExecuteChanged();
            }
        }

        public ICommand New {
            get {
                return _new ?? (
                    _new = new RelayCommand(
                        param => {
                            App app = (App)Application.Current;

                            WeightsSet weightsSet = new WeightsSet { Name = "New Weights Set", };

                            app.SettingsService.GetWeightsSets().Add(weightsSet);

                            WeightsSetVM weightsSetVM = new WeightsSetVM(weightsSet);

                            WeightsSets.Add(weightsSetVM);

                            SelectedWeightsSet = weightsSetVM;
                        },
                        param => true
                    )
                );
            }
        }

        public ICommand Copy {
            get {
                return _copy ?? (
                    _copy = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null) {
                                App app = (App)Application.Current;

                                Int32 i = WeightsSets.FindIndex(item => ReferenceEquals(item, SelectedWeightsSet));

                                WeightsSet weightsSet = new WeightsSet();

                                weightsSet.Name = SelectedWeightsSet.Value.Name + " (Copy)";

                                for(Int32 j = 0; j < weightsSet.Weights.Length; ++j) {
                                    for(Int32 k = 0; k < weightsSet.Weights[j].Length; ++k) {
                                        weightsSet.Weights[j][k] = SelectedWeightsSet.Value.Weights[j][k];
                                    }
                                }

                                app.SettingsService.GetWeightsSets().Insert(i + 1, weightsSet);

                                WeightsSetVM weightsSetVM = new WeightsSetVM(weightsSet);

                                WeightsSets.Insert(i + 1, weightsSetVM);

                                SelectedWeightsSet = weightsSetVM;
                            }
                        },
                        param => SelectedWeightsSet != null
                    )
                );
            }
        }

        public ICommand Delete {
            get {
                return _delete ?? (
                    _delete = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null) {
                                App app = (App)Application.Current;

                                Int32 i = WeightsSets.FindIndex(item => ReferenceEquals(item, SelectedWeightsSet));

                                Boolean isFirst = (i == 0);
                                Boolean isLast  = (i == WeightsSets.Count - 1);

                                app.SettingsService.GetWeightsSets().RemoveAt(i);

                                WeightsSets.RemoveAt(i);

                                if(WeightsSets.Count != 0) {
                                    if(isFirst) {
                                        SelectedWeightsSet = WeightsSets[0];
                                    }
                                    else if(isLast) {
                                        SelectedWeightsSet = WeightsSets[WeightsSets.Count - 1];
                                    }
                                    else {
                                        SelectedWeightsSet = WeightsSets[i];
                                    }
                                }
                            }
                        },
                        param => SelectedWeightsSet != null
                    )
                );
            }
        }

        public ICommand MoveUp {
            get {
                return _moveUp ?? (
                    _moveUp = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null && !ReferenceEquals(WeightsSets.First(), SelectedWeightsSet)) {
                                App app = (App)Application.Current;

                                WeightsSetVM selectedWeightsSet = SelectedWeightsSet;

                                Int32 i = WeightsSets.FindIndex(item => ReferenceEquals(item, selectedWeightsSet));

                                app.SettingsService.GetWeightsSets().RemoveAt(i);
                                app.SettingsService.GetWeightsSets().Insert(i - 1, selectedWeightsSet.Value);

                                WeightsSets.Move(i, i - 1);

                                SelectedWeightsSet = selectedWeightsSet;
                            }
                        },
                        param => SelectedWeightsSet != null && !ReferenceEquals(WeightsSets.First(), SelectedWeightsSet)
                    )
                );
            }
        }

        public ICommand MoveDown {
            get {
                return _moveDown ?? (
                    _moveDown = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null && !ReferenceEquals(WeightsSets.Last(), SelectedWeightsSet)) {
                                App app = (App)Application.Current;

                                WeightsSetVM selectedWeightsSet = SelectedWeightsSet;

                                Int32 i = WeightsSets.FindIndex(item => ReferenceEquals(item, selectedWeightsSet));

                                app.SettingsService.GetWeightsSets().RemoveAt(i);
                                app.SettingsService.GetWeightsSets().Insert(i + 1, selectedWeightsSet.Value);

                                WeightsSets.Move(i, i + 1);

                                SelectedWeightsSet = selectedWeightsSet;
                            }
                        },
                        param => SelectedWeightsSet != null && !ReferenceEquals(WeightsSets.Last(), SelectedWeightsSet)
                    )
                );
            }
        }

        public ICommand Reset {
            get {
                return _reset ?? (
                    _reset = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null) {
                                for(Int32 i = 0; i < SelectedWeightsSet.Weights.Length; ++i) {
                                    for(Int32 j = 0; j < SelectedWeightsSet.Weights[i].Length; ++j) {
                                        SelectedWeightsSet.Weights[i][j].Value = 0;
                                    }
                                }
                            }
                        },
                        param => SelectedWeightsSet != null
                    )
                );
            }
        }

        public ICommand Apply {
            get {
                return _apply ?? (
                    _apply = new RelayCommand(
                        param => {
                            if(SelectedWeightsSet != null) {
                                foreach(WeightsSetVM weightsSet in WeightsSets) {
                                    weightsSet.IsLast = ReferenceEquals(weightsSet, SelectedWeightsSet);
                                }

                                IsOpened = false;

                                WeightsSetApplied?.Invoke(this, EventArgs.Empty);
                            }
                        },
                        param => SelectedWeightsSet != null
                    )
                );
            }
        }

        public ICommand Import {
            get {
                return _import ?? (
                    _import = new RelayCommand(
                        async param => {
                            App app = (App) Application.Current;

                            StorageFile file = await app.NavigationService.PickSingleFileAsync(new [] { "*", ".txt" });

                            if(file != null) {
                                WeightsSet weightsSet = await app.SettingsService.LoadWeightsSetAsync(file);

                                app.SettingsService.GetWeightsSets().Add(weightsSet);

                                WeightsSetVM weightsSetVM = new WeightsSetVM(weightsSet);

                                WeightsSets.Add(weightsSetVM);

                                SelectedWeightsSet = weightsSetVM;
                            }

                            IsOpened = true;
                        },
                        param => true
                    )
                );
            }
        }

        public ICommand Export {
            get {
                return _export ?? (
                    _export = new RelayCommand(
                        async param => {
                            if(SelectedWeightsSet != null) {
                                App app = (App) Application.Current;

                                StorageFile file = await app.NavigationService.PickSaveFileAsync(
                                    new [] {
                                        new Pair< String, List<String> >("Plain Text", new List<String> { ".txt", }),
                                    },
                                    String.Format("WeightsSet_{0}.txt", SelectedWeightsSet.Name)
                                );

                                if(file != null) {
                                    await app.SettingsService.SaveWeightsSetAsync(file, SelectedWeightsSet.Value);
                                }

                                IsOpened = true;
                            }
                        },
                        param => SelectedWeightsSet != null
                    )
                );
            }
        }



        private Boolean      _isOpened;
        private WeightsSetVM _selectedWeightsSet;
        private RelayCommand _new;
        private RelayCommand _copy;
        private RelayCommand _delete;
        private RelayCommand _moveUp;
        private RelayCommand _moveDown;
        private RelayCommand _reset;
        private RelayCommand _apply;
        private RelayCommand _import;
        private RelayCommand _export;
    }

}
