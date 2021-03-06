using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class FiltersViewModel : ViewModelBase {

        public event EventHandler FilterApplied;

        public FiltersViewModel() {
            App app = (App)Application.Current;

            app.DataService.LoadStarted  += OnLoadStarted;
            app.DataService.LoadFinished += OnLoadFinished;

            Filters = new ObservableCollectionEx<FilterVM>();

            foreach(Filter filter in app.SettingsService.GetFilters()) {
                Filters.Add(new FilterVM(filter));
            }

            SelectedFilter = Filters.Find(item => item.IsLast);

            if(SelectedFilter == null) {
                if(Filters.Count != 0) {
                    SelectedFilter = Filters[0];
                }
            }

            _nationGroups = new ListCollectionViewSource<NationGroupVM>(new List<NationGroupVM>(), ShowNationGroup, null);

            _clubs = new ListCollectionViewSource<CMClubVM>(new List<CMClubVM>(), ShowClub, null);

            _divisions = new ListCollectionViewSource<CMDivisionVM>(new List<CMDivisionVM>(), ShowDivision, null);
        }

        public static TransferStatus[] TransferStatuses => DataService.TransferStatuses;

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

        public ObservableCollectionEx<FilterVM> Filters { get; }

        public FilterVM SelectedFilter {
            get {
                return _selectedFilter;
            }

            set {
                _selectedFilter = value;

                RaisePropertyChanged();

                _new?.RaiseCanExecuteChanged();
                _copy?.RaiseCanExecuteChanged();
                _delete?.RaiseCanExecuteChanged();
                _moveUp?.RaiseCanExecuteChanged();
                _moveDown?.RaiseCanExecuteChanged();
                _reset?.RaiseCanExecuteChanged();
                _resetTab?.RaiseCanExecuteChanged();
                _apply?.RaiseCanExecuteChanged();
            }
        }

        public Int32 SelectedContentIndex {
            get {
                return _selectedContentIndex;
            }

            set {
                _selectedContentIndex = value;

                RaisePropertyChanged();
            }
        }

        public ICommand New {
            get {
                return _new ?? (
                    _new = new RelayCommand(
                        param => {
                            App app = (App)Application.Current;

                            Filter filter = new Filter { Name = "New Filter", };

                            app.SettingsService.GetFilters().Add(filter);

                            FilterVM filterVM = new FilterVM(filter);

                            Filters.Add(filterVM);

                            SelectedFilter = filterVM;
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
                            if(SelectedFilter != null) {
                                App app = (App)Application.Current;

                                Int32 i = Filters.FindIndex(item => ReferenceEquals(item, SelectedFilter));

                                Filter filter = new Filter();

                                filter.Name                      = SelectedFilter.Value.Name + " (Copy)";
                                filter.PlayerName                = SelectedFilter.Value.PlayerName;
                                filter.NationGroupName           = SelectedFilter.Value.NationGroupName;
                                filter.ClubName                  = SelectedFilter.Value.ClubName;
                                filter.DivisionName               = SelectedFilter.Value.DivisionName;
                                filter.AgeFrom                   = SelectedFilter.Value.AgeFrom;
                                filter.AgeTo                     = SelectedFilter.Value.AgeTo;
                                filter.CAFrom                    = SelectedFilter.Value.CAFrom;
                                filter.CATo                      = SelectedFilter.Value.CATo;
                                filter.PAFrom                    = SelectedFilter.Value.PAFrom;
                                filter.PATo                      = SelectedFilter.Value.PATo;
                                filter.ValueFrom                 = SelectedFilter.Value.ValueFrom;
                                filter.ValueTo                   = SelectedFilter.Value.ValueTo;
                                filter.WageFrom                  = SelectedFilter.Value.WageFrom;
                                filter.WageTo                    = SelectedFilter.Value.WageTo;
                                filter.IsFreeTransfer            = SelectedFilter.Value.IsFreeTransfer;
                                filter.IsContractExpired         = SelectedFilter.Value.IsContractExpired;
                                filter.IsContractExpiring        = SelectedFilter.Value.IsContractExpiring;
                                filter.IsContractUnprotected     = SelectedFilter.Value.IsContractUnprotected;
                                filter.IsLeavingOnBosman         = SelectedFilter.Value.IsLeavingOnBosman;
                                filter.IsTransferArranged        = SelectedFilter.Value.IsTransferArranged;
                                filter.IsTransferListedByClub    = SelectedFilter.Value.IsTransferListedByClub;
                                filter.IsTransferListedByRequest = SelectedFilter.Value.IsTransferListedByRequest;
                                filter.IsListedForLoan           = SelectedFilter.Value.IsListedForLoan;

                                for(Int32 j = 0; j < filter.Sides.Length; ++j) {
                                    filter.Sides[j] = SelectedFilter.Value.Sides[j];
                                }

                                for(Int32 j = 0; j < filter.Positions.Length; ++j) {
                                    filter.Positions[j] = SelectedFilter.Value.Positions[j];
                                }

                                for(Int32 j = 0; j < filter.Attributes.Length; ++j) {
                                    filter.Attributes[j] = SelectedFilter.Value.Attributes[j];
                                }

                                app.SettingsService.GetFilters().Insert(i + 1, filter);

                                FilterVM filterVM = new FilterVM(filter);

                                Filters.Insert(i + 1, filterVM);

                                SelectedFilter = filterVM;
                            }
                        },
                        param => SelectedFilter != null
                    )
                );
            }
        }

        public ICommand Delete {
            get {
                return _delete ?? (
                    _delete = new RelayCommand(
                        param => {
                            if(SelectedFilter != null) {
                                App app = (App)Application.Current;

                                Int32 i = Filters.FindIndex(item => ReferenceEquals(item, SelectedFilter));

                                Boolean isFirst = (i == 0);
                                Boolean isLast  = (i == Filters.Count - 1);

                                app.SettingsService.GetFilters().RemoveAt(i);

                                Filters.RemoveAt(i);

                                if(Filters.Count != 0) {
                                    if(isFirst) {
                                        SelectedFilter = Filters[0];
                                    }
                                    else if(isLast) {
                                        SelectedFilter = Filters[Filters.Count - 1];
                                    }
                                    else {
                                        SelectedFilter = Filters[i];
                                    }
                                }
                            }
                        },
                        param => SelectedFilter != null
                    )
                );
            }
        }

        public ICommand MoveUp {
            get {
                return _moveUp ?? (
                    _moveUp = new RelayCommand(
                        param => {
                            if(SelectedFilter != null && !ReferenceEquals(Filters.First(), SelectedFilter)) {
                                App app = (App)Application.Current;

                                FilterVM selectedFilter = SelectedFilter;

                                Int32 i = Filters.FindIndex(item => ReferenceEquals(item, selectedFilter));

                                app.SettingsService.GetFilters().RemoveAt(i);
                                app.SettingsService.GetFilters().Insert(i - 1, selectedFilter.Value);

                                Filters.Move(i, i - 1);

                                SelectedFilter = selectedFilter;
                            }
                        },
                        param => SelectedFilter != null && !ReferenceEquals(Filters.First(), SelectedFilter)
                    )
                );
            }
        }

        public ICommand MoveDown {
            get {
                return _moveDown ?? (
                    _moveDown = new RelayCommand(
                        param => {
                            if(SelectedFilter != null && !ReferenceEquals(Filters.Last(), SelectedFilter)) {
                                App app = (App)Application.Current;

                                FilterVM selectedFilter = SelectedFilter;

                                Int32 i = Filters.FindIndex(item => ReferenceEquals(item, selectedFilter));

                                app.SettingsService.GetFilters().RemoveAt(i);
                                app.SettingsService.GetFilters().Insert(i + 1, selectedFilter.Value);

                                Filters.Move(i, i + 1);

                                SelectedFilter = selectedFilter;
                            }
                        },
                        param => SelectedFilter != null && !ReferenceEquals(Filters.Last(), SelectedFilter)
                    )
                );
            }
        }

        public ICommand Reset {
            get {
                return _reset ?? (
                    _reset = new RelayCommand(
                        param => {
                            ResetTabImpl(null);
                        },
                        param => SelectedFilter != null
                    )
                );
            }
        }

        public ICommand ResetTab {
            get {
                return _resetTab ?? (
                    _resetTab = new RelayCommand(
                        param => {
                            ResetTabImpl(SelectedContentIndex);
                        },
                        param => SelectedFilter != null
                    )
                );
            }
        }

        public ICommand Apply {
            get {
                return _apply ?? (
                    _apply = new RelayCommand(
                        param => {
                            if(SelectedFilter != null) {
                                foreach(FilterVM filter in Filters) {
                                    filter.IsLast = ReferenceEquals(filter, SelectedFilter);
                                }

                                IsOpened = false;

                                FilterApplied?.Invoke(this, EventArgs.Empty);
                            }
                        },
                        param => SelectedFilter != null
                    )
                );
            }
        }

        // Nation groups
        public Boolean AreNationGroupsOpened {
            get {
                return _areNationGroupsOpened;
            }

            set {
                _areNationGroupsOpened = value;

                RaisePropertyChanged();

                if(_areNationGroupsOpened) {
                    NationGroupSearchString = null;

                    SelectedNationGroup = _nationGroups.View.Find(item => item.Name == SelectedFilter.NationGroupName);

                    _scrollToNationGroup?.Invoke(SelectedNationGroup);
                }
            }
        }

        public ObservableCollectionEx<NationGroupVM> NationGroups => _nationGroups.View;

        public NationGroupVM SelectedNationGroup {
            get {
                return _selectedNationGroup;
            }

            set {
                _selectedNationGroup = value;

                RaisePropertyChanged();
            }
        }

        public String NationGroupSearchString {
            get {
                return _nationGroupSearchString;
            }

            set {
                _nationGroupSearchString = value;

                RaisePropertyChanged();

                _nationGroups.Refresh();
            }
        }

        public Action<Object> ScrollToNationGroup {
            get {
                return _scrollToNationGroup;
            }

            set {
                _scrollToNationGroup = value;

                RaisePropertyChanged();

                _scrollToNationGroup?.Invoke(SelectedNationGroup);
            }
        }

        public ICommand NationGroupActivated {
            get {
                return _nationGroupActivated ?? (
                    _nationGroupActivated = new RelayCommand(
                        param => {
                            NationGroupVM nationGroupVM = (NationGroupVM)param;

                            SelectedFilter.NationGroupName = nationGroupVM.Name;

                            AreNationGroupsOpened = false;
                        }
                    )
                );
            }
        }

        public List<CMNation> GetNationGroupNations(String nationGroupName) {
            return _nationGroups.Source.Find(item => item.Name == nationGroupName)?.Nations;
        }

        private Boolean ShowNationGroup(NationGroupVM nationGroup) {
            return (String.IsNullOrEmpty(nationGroup.Name)                                                           ||
                    String.IsNullOrEmpty(_nationGroupSearchString)                                                   ||
                    Helpers.Contains(nationGroup.Name, _nationGroupSearchString, StringComparison.OrdinalIgnoreCase) ||
                    Helpers.Contains(nationGroup.Code, _nationGroupSearchString, StringComparison.OrdinalIgnoreCase)  );
        }

        // Clubs
        public Boolean AreClubsOpened {
            get {
                return _areClubsOpened;
            }

            set {
                _areClubsOpened = value;

                RaisePropertyChanged();

                if(_areClubsOpened) {
                    ClubSearchString = null;

                    SelectedClub = _clubs.View.Find(item => item.Name == SelectedFilter.ClubName);

                    _scrollToClub?.Invoke(SelectedClub);
                }
            }
        }

        public ObservableCollectionEx<CMClubVM> Clubs => _clubs.View;

        public CMClubVM SelectedClub {
            get {
                return _selectedClub;
            }

            set {
                _selectedClub = value;

                RaisePropertyChanged();
            }
        }

        public String ClubSearchString {
            get {
                return _clubSearchString;
            }

            set {
                _clubSearchString = value;

                RaisePropertyChanged();

                _clubs.Refresh();
            }
        }

        public Action<Object> ScrollToClub {
            get {
                return _scrollToClub;
            }

            set {
                _scrollToClub = value;

                RaisePropertyChanged();

                _scrollToClub?.Invoke(SelectedClub);
            }
        }

        public ICommand ClubActivated {
            get {
                return _clubActivated ?? (
                           _clubActivated = new RelayCommand(
                        param => {
                            CMClubVM clubVM = (CMClubVM)param;

                            SelectedFilter.ClubName = clubVM.Name;

                            AreClubsOpened = false;
                        }
                    )
                );
            }
        }

        private Boolean ShowClub(CMClubVM club) {
            return (String.IsNullOrEmpty(club.Name)                                                    ||
                    String.IsNullOrEmpty(_clubSearchString)                                            ||
                    Helpers.Contains(club.Name, _clubSearchString, StringComparison.OrdinalIgnoreCase)  );
        }

        // Divisions
        public Boolean AreDivisionsOpened {
            get {
                return _areDivisionsOpened;
            }

            set {
                _areDivisionsOpened = value;

                RaisePropertyChanged();

                if(_areDivisionsOpened) {
                    DivisionSearchString = null;

                    SelectedDivision = _divisions.View.Find(item => item.Name == SelectedFilter.DivisionName);

                    _scrollToDivision?.Invoke(SelectedDivision);
                }
            }
        }

        public ObservableCollectionEx<CMDivisionVM> Divisions => _divisions.View;

        public CMDivisionVM SelectedDivision {
            get {
                return _selectedDivision;
            }

            set {
                _selectedDivision = value;

                RaisePropertyChanged();
            }
        }

        public String DivisionSearchString {
            get {
                return _divisionSearchString;
            }

            set {
                _divisionSearchString = value;

                RaisePropertyChanged();

                _divisions.Refresh();
            }
        }

        public Action<Object> ScrollToDivision {
            get {
                return _scrollToDivision;
            }

            set {
                _scrollToDivision = value;

                RaisePropertyChanged();

                _scrollToDivision?.Invoke(SelectedDivision);
            }
        }

        public ICommand DivisionActivated {
            get {
                return _divisionActivated ?? (
                    _divisionActivated = new RelayCommand(
                        param => {
                            CMDivisionVM DivisionVM = (CMDivisionVM)param;

                            SelectedFilter.DivisionName = DivisionVM.Name;

                            AreDivisionsOpened = false;
                        }
                    )
                );
            }
        }

        private Boolean ShowDivision(CMDivisionVM Division) {
            return (String.IsNullOrEmpty(Division.Name)                                                        ||
                    String.IsNullOrEmpty(_divisionSearchString)                                                ||
                    Helpers.Contains(Division.Name, _divisionSearchString, StringComparison.OrdinalIgnoreCase)  );
        }



        public Int32 MaxAge => ((App)Application.Current).DataService.MaxAge ?? 0;



        private void OnLoadStarted(Object sender, DataService.LoadStartedEventArgs args) {
            {
                _nationGroups.Source.Clear();

                _nationGroups.Refresh();
            }

            {
                _clubs.Source.Clear();

                _clubs.Refresh();
            }

            {
                _divisions.Source.Clear();

                _divisions.Refresh();
            }
        }

        private void OnLoadFinished(Object sender, DataService.LoadFinishedEventArgs args) {
            if(args.ErrorMessage == null) {
                App app = (App)Application.Current;

                {
                    _nationGroups.Source.Add(new NationGroupVM(null, null, null));

                    _nationGroups.Source.Add(new NationGroupVM(DataService.EUNationGroupName, DataService.EUNationGroupCode, null));
#if DEBUG
                    List<CMNation> nations = new List<CMNation>();

                    foreach(String code in new [] { "ARM", "AZE", "BLR", "EST", "GEO", "KAZ", "KGZ", "LVA", "LTU", "MOL", "RUS", "TJK", "TKM", "UKR", "UZB", }) {
                        nations.Add(app.DataService.Nations.Find(item => item.Code == code));
                    }

                    _nationGroups.Source.Add(new NationGroupVM("Soviet Union", "SUN", nations));
#endif
                    foreach(CMNation nation in app.DataService.Nations) {
                        _nationGroups.Source.Add(new NationGroupVM(nation));
                    }

                    _nationGroups.Refresh();
                }

                {
                    _clubs.Source.Add(new CMClubVM( (String)null) );

                    _clubs.Source.Add(new CMClubVM(DataService.NoClubName));

                    foreach(CMClub club in app.DataService.Clubs) {
                        _clubs.Source.Add(new CMClubVM(club));
                    }

                    _clubs.Refresh();
                }

                {
                    _divisions.Source.Add(new CMDivisionVM( (String)null) );

                    foreach(CMDivision division in app.DataService.Divisions) {
                        _divisions.Source.Add(new CMDivisionVM(division));
                    }

                    _divisions.Refresh();
                }

                RaisePropertyChanged(nameof(MaxAge));
            }
        }

        private void ResetTabImpl(Int32? tabIndex) {
            if(SelectedFilter != null) {
                if(tabIndex == null || tabIndex.Value == 0) {
                    SelectedFilter.PlayerName      = null;
                    SelectedFilter.NationGroupName = null;
                    SelectedFilter.ClubName        = null;
                    SelectedFilter.AgeFrom         = 0;
                    SelectedFilter.AgeTo           = 0;

                    foreach(SideVM sideVM in SelectedFilter.Sides) {
                        sideVM.IsChecked = false;
                    }

                    foreach(PositionVM positionVM in SelectedFilter.Positions) {
                        positionVM.IsChecked = false;
                    }
                }

                if(tabIndex == null || tabIndex.Value == 1) {
                    SelectedFilter.ValueFrom                 = 0;
                    SelectedFilter.ValueTo                   = 0;
                    SelectedFilter.WageFrom                  = 0;
                    SelectedFilter.WageTo                    = 0;
                    SelectedFilter.IsFreeTransfer            = false;
                    SelectedFilter.IsContractExpired         = false;
                    SelectedFilter.IsContractExpiring        = false;
                    SelectedFilter.IsContractUnprotected     = false;
                    SelectedFilter.IsLeavingOnBosman         = null;
                    SelectedFilter.IsTransferArranged        = null;
                    SelectedFilter.IsTransferListedByClub    = false;
                    SelectedFilter.IsTransferListedByRequest = false;
                    SelectedFilter.IsListedForLoan           = false;
                }

                if(tabIndex == null || tabIndex.Value == 2) {
                    foreach(AttributeVM attributeVM in SelectedFilter.Attributes) {
                        attributeVM.Value = 1;
                    }
                }

                if(tabIndex == null || tabIndex.Value == 3) {
                    SelectedFilter.DivisionName = null;
                }
            }
        }



        private Boolean                                 _isOpened;
        private FilterVM                                _selectedFilter;
        private Int32                                   _selectedContentIndex;
        private RelayCommand                            _new;
        private RelayCommand                            _copy;
        private RelayCommand                            _delete;
        private RelayCommand                            _moveUp;
        private RelayCommand                            _moveDown;
        private RelayCommand                            _reset;
        private RelayCommand                            _resetTab;
        private RelayCommand                            _apply;

        private Boolean                                 _areNationGroupsOpened;
        private ListCollectionViewSource<NationGroupVM> _nationGroups;
        private NationGroupVM                           _selectedNationGroup;
        private String                                  _nationGroupSearchString;
        private Action<Object>                          _scrollToNationGroup;
        private RelayCommand                            _nationGroupActivated;

        private Boolean                                 _areClubsOpened;
        private ListCollectionViewSource<CMClubVM>      _clubs;
        private CMClubVM                                _selectedClub;
        private String                                  _clubSearchString;
        private Action<Object>                          _scrollToClub;
        private RelayCommand                            _clubActivated;

        private Boolean                                 _areDivisionsOpened;
        private ListCollectionViewSource<CMDivisionVM>  _divisions;
        private CMDivisionVM                            _selectedDivision;
        private String                                  _divisionSearchString;
        private Action<Object>                          _scrollToDivision;
        private RelayCommand                            _divisionActivated;
    }

}
