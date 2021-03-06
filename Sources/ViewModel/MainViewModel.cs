using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class MainViewModel : PageViewModelBase {
        public MainViewModel() {
            App app = (App)Application.Current;

            OpenViewModel              = new OpenViewModel();
            FiltersViewModel           = new FiltersViewModel();
            WeightsViewModel           = new WeightsViewModel();
            FavoritesViewModel         = new FavoritesViewModel(this);
            SettingsViewModel          = new SettingsViewModel();
            AboutViewModel             = new AboutViewModel();
            QuickFilterViewModel       = new QuickFilterViewModel();
            CompareItemsPanelViewModel = new CompareItemsPanelVM(OnCompareItems);

            app.SettingsService.ColumnsChanged      += OnColumnsChanged;
            app.SettingsService.Columns2Changed     += OnColumns2Changed;
            app.SettingsService.CA18ViewModeChanged += OnCA18ViewModeChanged;
            app.DataService.LoadStarted             += OnLoadStarted;
            app.DataService.LoadFinished            += OnLoadFinished;

            FiltersViewModel.FilterApplied             += OnFilterApplied;
            WeightsViewModel.WeightsSetApplied         += OnWeightsSetApplied;
            FavoritesViewModel.FavoritesFilterApplied  += OnFavoritesFilterApplied;
            FavoritesViewModel.FavoritesLoaded         += OnFavoritesLoaded;
            FavoritesViewModel.FavoritesChanged        += OnFavoritesChanged;
            QuickFilterViewModel.RatingPositionChanged += OnOnRatingPositionChanged;
            QuickFilterViewModel.FilterChanged         += OnQuickFilterChanged;

            _playersSortColumn = new Pair<String, Boolean>("Rating", false);

            _players = new ListCollectionViewSource<CMStaffVM>(new List<CMStaffVM>(), QuickShowPlayer, ComparePlayers);
        }

        protected override void OnNavigatedTo(Boolean isNew) {
            base.OnNavigatedTo(isNew);

            if(isNew) {
                App app = (App)Application.Current;

                if(app.SettingsService.GetLoadLastSave()) {
                    List<MruFile> mruFiles = app.SettingsService.GetMruFiles();

                    if(mruFiles.Count != 0) {
                        OpenViewModel.OpenOldFile.Execute(new MruFileVM(mruFiles[0]));
                    }
                }
            }
        }

        public OpenViewModel        OpenViewModel              { get; }
        public FiltersViewModel     FiltersViewModel           { get; }
        public WeightsViewModel     WeightsViewModel           { get; }
        public FavoritesViewModel   FavoritesViewModel         { get; }
        public SettingsViewModel    SettingsViewModel          { get; }
        public AboutViewModel       AboutViewModel             { get; }
        public QuickFilterViewModel QuickFilterViewModel       { get; }
        public CompareItemsPanelVM  CompareItemsPanelViewModel { get; }

        public ICommand Open {
            get {
                return _open ?? (
                    _open = new RelayCommand(
                        param => {
                            OpenViewModel.IsOpened = true;
                        }
                    )
                );
            }
        }

        public ICommand Reload {
            get {
                return _reload ?? (
                    _reload = new RelayCommand(
                        param => {
                            if(OpenViewModel.OpenedFile != null) {
                                App app = (App)Application.Current;

                                MruFile mruFile = app.SettingsService.GetMruFiles().Find(item => item.File.Path == OpenViewModel.OpenedFile.Path);

                                if(mruFile != null) {
                                    OpenViewModel.OpenOldFile.Execute(new MruFileVM(mruFile));
                                }
                            }
                        },
                        param => OpenViewModel.OpenedFile != null
                    )
                );
            }
        }

        public ICommand Filters {
            get {
                return _filters ?? (
                    _filters = new RelayCommand(
                        param => {
                            FiltersViewModel.IsOpened = true;
                        },
                        param => OpenViewModel.OpenedFile != null
                    )
                );
            }
        }

        public ICommand Weights {
            get {
                return _weights ?? (
                    _weights = new RelayCommand(
                        param => {
                            WeightsViewModel.IsOpened = true;
                        },
                        param => true
                    )
                );
            }
        }

        public ICommand Favorites {
            get {
                return _favorites ?? (
                    _favorites = new RelayCommand(
                        param => {
                            FavoritesViewModel.IsOpened = true;
                        },
                        param => true
                    )
                );
            }
        }

        public ICommand Settings {
            get {
                return _settings ?? (
                    _settings = new RelayCommand(
                        param => {
                            SettingsViewModel.IsOpened = true;
                        }
                    )
                );
            }
        }

        public ICommand About {
            get {
                return _about ?? (
                    _about = new RelayCommand(
                        param => {
                            AboutViewModel.IsOpened = true;
                        }
                    )
                );
            }
        }

        public ObservableCollectionEx<CMStaffVM> Players {
            get {
                return _players.View;
            }
        }

        public Int32 PlayersCount => _players.View.Count; 

        public CMStaffVM SelectedPlayer {
            get {
                return _selectedPlayer;
            }

            set {
                SetSelectedPlayer(value);
            }
        }

        private async void SetSelectedPlayer(CMStaffVM selectedPlayer) {
            _selectedPlayer = selectedPlayer;

            RaisePropertyChanged(nameof(SelectedPlayer));

            _ignorePlayerTapped = true;

            await Task.Delay(100);

            _ignorePlayerTapped = false;
        }

        public Pair<String, Boolean> PlayersSortColumn {
            get {
                return _playersSortColumn;
            }

            set {
                _playersSortColumn = value;

                RaisePropertyChanged();

                _players.Resort();
            }
        }

        public Action<Object> ScrollToPlayer {
            get {
                return _scrollToPlayer;
            }

            set {
                _scrollToPlayer = value;

                RaisePropertyChanged();
            }
        }

        public Action<String, Boolean> SetIsColumnVisible {
            get {
                return _setIsColumnVisible;
            }

            set {
                _setIsColumnVisible = value;

                RaisePropertyChanged();

                UpdateColumnsVisibility();
            }
        }

        public ICommand PlayerTapped {
            get {
                return _playerTapped ?? (
                    _playerTapped = new RelayCommand(
                        param => {
                            if(_ignorePlayerTapped) {
                                return;
                            }

                            Pair<Object, String> p = (Pair<Object, String>)param;

                            CMStaffVM player     = (CMStaffVM)p.First;
                            String    columnName = p.Second;

                            //Debug.WriteLine("PlayerTapped: {0} {1} ({2})", player.SecondName, player.FirstName, columnName);

                            if(ReferenceEquals(SelectedPlayer, player)) {
                                SelectedPlayer = null;
                            }
                            else {
                                SelectedPlayer = player;
                            }
                        }
                    )
                );
            }
        }

        public ICommand PlayerRightTapped {
            get {
                return _playerRightTapped ?? (
                    _playerRightTapped = new RelayCommand(
                        param => {
                            Pair<Object, String> p = (Pair<Object, String>)param;

                            CMStaffVM player     = (CMStaffVM)p.First;
                            String    columnName = p.Second;

                            CompareItemsPanelViewModel.AddItem(player);
                        }
                    )
                );
            }
        }

        public static Attribute[] Attributes => DataService.Attributes;



        private void OnColumnsChanged(Object sender, EventArgs args) {
            UpdateColumnsVisibility();
        }

        private void OnColumns2Changed(Object sender, EventArgs args) {
            UpdateColumnsVisibility();
        }

        private void OnCA18ViewModeChanged(Object sender, EventArgs args) {
            App app = (App)Application.Current;

            foreach(CMStaffVM player in _players.Source) {
                player.UpdateAttributes(app.SettingsService.GetCA18ViewMode());
            }
        }

        private void OnLoadStarted(Object sender, DataService.LoadStartedEventArgs args) {
            QuickFilterViewModel.UpdateAppliedFilterName(null);

            _filters?.RaiseCanExecuteChanged();
            _reload?.RaiseCanExecuteChanged();

            _players.Source.Clear();

            _players.Refresh();
            RaisePropertyChanged(nameof(PlayersCount));

            if(CompareItemsPanelViewModel.Clear.CanExecute(null)) {
                CompareItemsPanelViewModel.Clear.Execute(null);
            }
        }

        private async void OnLoadFinished(Object sender, DataService.LoadFinishedEventArgs args) {
            App app = (App) Application.Current;

            if(args.ErrorMessage == null) {
                await CalculateRatingsAsync();

                if(app.SettingsService.GetApplyLastFilter()) {
                    Filter filter = app.SettingsService.GetFilters().Find(item => item.IsLast);

                    if(filter != null) {
                        ApplyFilter(filter);
                    }
                }

                app.SettingsService.SetOpensCount(app.SettingsService.GetOpensCount() + 1);

                if(app.SettingsService.GetOpensCount() >= 5 && !app.SettingsService.WasRateAppSuggestionShown()) {
                    app.SettingsService.SetWasRateAppSuggestionShown(true);

                    await app.NavigationService.ShowContentDialogAsync(new RateAppSuggestionViewModel());
                }
            }

            _filters?.RaiseCanExecuteChanged();
            _reload?.RaiseCanExecuteChanged();
        }

        private void OnFilterApplied(Object sender, EventArgs args) {
            ApplyFilter(FiltersViewModel.SelectedFilter.Value);
        }

        private void OnFavoritesFilterApplied(Object sender, EventArgs args) {
            ApplyFavoritesFilter();
        }

        private void OnFavoritesLoaded(Object sender, EventArgs args) {
            ApplyFavoritesFilter();
        }

        private void OnFavoritesChanged(Object sender, EventArgs args) {
            if(QuickFilterViewModel.AppliedFilterName == "Favorite Players") {
                ApplyFavoritesFilter();
            }
        }

        private void ApplyFavoritesFilter() {
            Filter filter = new Filter();

            filter.IsFavorite = true;
            filter.Name       = "Favorite Players";

            ApplyFilter(filter);
        }

        private void ApplyFilter(Filter filter) {
            App app = (App)Application.Current;

            _players.Source.Clear();

            foreach(CMStaff staff in app.DataService.Staffs) {
                if(DataService.IsValidPlayer(staff)) {
                    if(ShowPlayer(staff, filter)) {
                        CMStaffVM staffVM = new CMStaffVM(staff);

                        staffVM.UpdateAttributes(app.SettingsService.GetCA18ViewMode());
                        staffVM.UpdateRatings();
                        staffVM.UpdateRating(QuickFilterViewModel.SelectedRatingPosition.Code);

                        _players.Source.Add(staffVM);
                    }
                }
            }

            _players.Refresh();
            RaisePropertyChanged(nameof(PlayersCount));

            QuickFilterViewModel.UpdateAppliedFilterName(filter.Name);
        }

        private async void OnWeightsSetApplied(Object sender, EventArgs args) {
            if(OpenViewModel.OpenedFile != null) {
                await CalculateRatingsAsync();

                foreach(CMStaffVM staffVM in _players.Source) {
                    staffVM.UpdateRatings();
                    staffVM.UpdateRating(QuickFilterViewModel.SelectedRatingPosition.Code);
                }

                if(_playersSortColumn.First == "Rating") {
                    _players.Resort();
                }
            }
        }

        private void OnOnRatingPositionChanged(Object sender, EventArgs eventArgs) {
            foreach(CMStaffVM player in _players.Source) {
                player.UpdateRating(QuickFilterViewModel.SelectedRatingPosition.Code);
            }

            if(_playersSortColumn.First == "Rating") {
                _players.Resort();
            }
        }

        private void OnQuickFilterChanged(Object sender, EventArgs args) {
            _players.Refresh();
            RaisePropertyChanged(nameof(PlayersCount));
        }

        private Boolean ShowPlayer(CMStaff player, Filter filter) {
            App app = (App)Application.Current;

            if(filter.IsFavorite) {
                if(!player.IsFavorite) {
                    return false;
                }
            }

            if(player.Age != null) {
                if(filter.AgeFrom != 0 && player.Age < filter.AgeFrom) {
                    return false;
                }

                if(filter.AgeTo != 0 && player.Age > filter.AgeTo) {
                    return false;
                }
            }

            if(player.Player.CurrentAbility < filter.CAFrom || player.Player.CurrentAbility > filter.CATo) {
                return false;
            }

            if(player.Player.PotentialAbility < filter.PAFrom || player.Player.PotentialAbility > filter.PATo) {
                return false;
            }

            if(filter.ValueFrom != 0) {
                if(player.Value < filter.ValueFrom) {
                    return false;
                }
            }

            if(filter.ValueTo != 0) {
                if(player.Value > filter.ValueTo) {
                    return false;
                }
            }

            if(filter.WageFrom != 0) {
                if(player.Contract == null || player.Contract.Wage < filter.WageFrom) {
                    return false;
                }
            }

            if(filter.WageTo != 0) {
                if(player.Contract == null || player.Contract.Wage > filter.WageTo) {
                    return false;
                }
            }

            if(filter.IsLeavingOnBosman != null) {
                Boolean isLeavingOnBosman = (player.Contract != null && player.Contract.LeavingOnBosman);

                if(isLeavingOnBosman != filter.IsLeavingOnBosman.Value) {
                    return false;
                }
            }

            if(filter.IsTransferArranged != null) {
                Boolean isTransferArranged = (player.Contract != null && player.Contract.TransferArrangedForClub != null);

                if(isTransferArranged != filter.IsTransferArranged.Value) {
                    return false;
                }
            }

            if(filter.IsFreeTransfer || filter.IsContractExpired || filter.IsContractExpiring || filter.IsContractUnprotected) {
                Boolean r = false;

                Boolean isFreeTransfer = (player.ClubJob == null);

                if(!r) {
                    if(filter.IsFreeTransfer) {
                        if(isFreeTransfer) {
                            r = true;
                        }
                    }
                }

                Boolean isContractExpired = (player.Contract == null || player.Contract.DateEnded.Value <= app.DataService.GameDate.Value);

                if(!r) {
                    if(filter.IsContractExpired) {
                        if(!isFreeTransfer && isContractExpired) {
                            r = true;
                        }
                    }
                }

                if(!r) {
                    if(filter.IsContractExpiring) {
                        if(!isContractExpired && player.Contract != null && DataService.DiffInYears(app.DataService.GameDate.Value, player.Contract.DateEnded.Value) == 0) {
                            r = true;
                        }
                    }
                }

                if(!r) {
                    if(filter.IsContractUnprotected) {
                        if(player.Contract != null && !String.IsNullOrEmpty(player.Contract.UnprotectedReason)) {
                            r = true;
                        }
                    }
                }

                if(!r) {
                    return false;
                }
            }

            if(filter.IsTransferListedByClub || filter.IsTransferListedByRequest || filter.IsListedForLoan) {
                Boolean r = false;

                if(!r) {
                    if(filter.IsTransferListedByClub) {
                        if(player.Contract != null && DataService.TransferStatuses[1].IsSuitableFor(player.Contract.TransferStatus)) {
                            r = true;
                        }
                    }
                }

                if(!r) {
                    if(filter.IsTransferListedByRequest) {
                        if(player.Contract != null && DataService.TransferStatuses[2].IsSuitableFor(player.Contract.TransferStatus)) {
                            r = true;
                        }
                    }
                }


                if(!r) {
                    if(filter.IsListedForLoan) {
                        if(player.Contract != null && DataService.TransferStatuses[3].IsSuitableFor(player.Contract.TransferStatus)) {
                            r = true;
                        }
                    }
                }


                if(!r) {
                    return false;
                }
            }

            if(!String.IsNullOrEmpty(filter.PlayerName)) {
                if( (player.CommonName?.Name == null || player.CommonName.Name.IndexOf(filter.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0) &&
                    (player.FirstName?.Name  == null || player.FirstName.Name.IndexOf (filter.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0) &&
                    (player.SecondName?.Name == null || player.SecondName.Name.IndexOf(filter.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0)  )
                {
                    return false;
                }
            }

            if(!String.IsNullOrEmpty(filter.ClubName)) {
                if(filter.ClubName == DataService.NoClubName) {
                    if(player.ClubJob != null) {
                        return false;
                    }
                }
                else {
                    if(!app.DataService.IsClubSquad(filter.ClubName, player.Id)) {
                        return false;
                    }
                }
            }

            if(!String.IsNullOrEmpty(filter.DivisionName)) {
                if(!app.DataService.IsDivisionSquad(filter.DivisionName, player.Id)) {
                    return false;
                }
            }

            if(!String.IsNullOrEmpty(filter.NationGroupName)) {
                if(filter.NationGroupName == DataService.EUNationGroupName) {
                    if(!DataService.IsEUNation(player)) {
                        return false;
                    }
                }
                else {
                    List<CMNation> nations = FiltersViewModel.GetNationGroupNations(filter.NationGroupName);

                    Boolean found = false;

                    if(nations != null) {
                        foreach(CMNation nation in nations) {
                            if(player.FirstNation?.Name == nation.Name || player.SecondNation?.Name == nation.Name) {
                                found = true;

                                break;
                            }
                        }
                    }

                    if(!found) {
                        return false;
                    }
                }
            }

            for(Int32 i = 0; i < player.AttributeValues.Length; ++i) {
                SByte v = player.AttributeValues[i].InMatchNormalized;

                if(DataService.Attributes[i].IsLessBetter) {
                    v = (SByte)(21 - v);
                }

                if(v < filter.Attributes[i]) {
                    return false;
                }
            }

            // Sides
            Int32 trueSidesLength = filter.Sides.Count(item => item == true);

            if(trueSidesLength != 0 && trueSidesLength != filter.Sides.Length) {
                Boolean isSuitable = false;

                for(Int32 i = 0; i < filter.Sides.Length; ++i) {
                    if(filter.Sides[i]) {
                        if(DataService.Sides[i].IsSuitableFor(player)) {
                            isSuitable = true;

                            break;
                        }
                    }
                }

                if(!isSuitable) {
                    return false;
                }
            }

            // Positions
            Int32 truePositionsLength = filter.Positions.Count(item => item == true);

            if(truePositionsLength != 0 && truePositionsLength != filter.Positions.Length) {
                Boolean isSuitable = false;

                for(Int32 i = 0; i < filter.Positions.Length; ++i) {
                    if(filter.Positions[i]) {
                        if(DataService.Positions[i].IsSuitableFor(player)) {
                            isSuitable = true;

                            break;
                        }
                    }
                }

                if(!isSuitable) {
                    return false;
                }
            }

            return true;
        }

        private Boolean QuickShowPlayer(CMStaffVM playerVM) {
            App app = (App)Application.Current;

            if(QuickFilterViewModel.SelectedSide.IsSuitableFor != null) {
                if(!QuickFilterViewModel.SelectedSide.IsSuitableFor(playerVM.Value)) {
                    return false;
                }
            }

            if(QuickFilterViewModel.SelectedPosition.IsSuitableFor != null) {
                if(!QuickFilterViewModel.SelectedPosition.IsSuitableFor(playerVM.Value)) {
                    return false;
                }
            }

            if(!String.IsNullOrEmpty(QuickFilterViewModel.PlayerName)) {
                if( (playerVM.Value.CommonName?.Name == null || playerVM.Value.CommonName.Name.IndexOf(QuickFilterViewModel.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0) &&
                    (playerVM.Value.FirstName?.Name  == null || playerVM.Value.FirstName.Name.IndexOf (QuickFilterViewModel.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0) &&
                    (playerVM.Value.SecondName?.Name == null || playerVM.Value.SecondName.Name.IndexOf(QuickFilterViewModel.PlayerName, StringComparison.OrdinalIgnoreCase)  < 0)  )
                {
                    return false;
                }
            }

            return true;
        }

        private Int32 ComparePlayers(CMStaffVM x, CMStaffVM y) {
            String  columnName = _playersSortColumn.First;
            Boolean ascending  = _playersSortColumn.Second;

            Comparison<CMStaffVM> comparison = null;

            Match match;

            if(columnName == "Rating") {
                comparison = (a, b) => a.Rating.Value.CompareTo(b.Rating.Value);
            }
            else if(columnName == "Name") {
                comparison = (a, b) => {
                    Int32 r = String.CompareOrdinal(a.SecondName, b.SecondName);

                    if(r != 0) { return  r; }

                    return String.CompareOrdinal(a.FirstName, b.FirstName);
                };
            }
            else if(columnName == "Nation") {
                comparison = (a, b) => {
                    Int32 r = String.CompareOrdinal(a.FirstNationCode, b.FirstNationCode);

                    if(r != 0) { return  r; }

                    return String.CompareOrdinal(a.SecondNationCode, b.SecondNationCode);
                };
            }
            else if(columnName == "Club") {
                comparison = (a, b) => Helpers.CompareNullable(a.ClubShortName, b.ClubShortName);
            }
            else if(columnName == "Position") {
                comparison = (a, b) => {
                    Func< CMStaffVM, List<Int32> > getPositions = 
                        s => {
                            List<Int32> v = new List<Int32>();

                            App app = (App)Application.Current;

                            if(DataService.IsGoalkeeper(s.Value))          { v.Add(0); }
                            if(DataService.IsSweeper(s.Value))             { v.Add(1); }
                            if(DataService.IsDefender(s.Value))            { v.Add(2); }
                            if(DataService.IsDefensiveMidfielder(s.Value)) { v.Add(3); }
                            if(DataService.IsMidfielder(s.Value))          { v.Add(4); }
                            if(DataService.IsAttackingMidfielder(s.Value)) { v.Add(5); }
                            if(DataService.IsForward(s.Value))             { v.Add(6); }
                            if(DataService.IsStriker(s.Value))             { v.Add(7); }

                            return v;
                        };

                    Func< CMStaffVM, List<Int32> > getSides = 
                        s => {
                            List<Int32> v = new List<Int32>();

                            App app = (App)Application.Current;

                            if(DataService.IsRightSide(s.Value))  { v.Add(0); }
                            if(DataService.IsLeftSide(s.Value))   { v.Add(1); }
                            if(DataService.IsCentreSide(s.Value)) { v.Add(2); }

                            return v;
                        };

                    Func< List<Int32>, List<Int32>, Int32 > cmpV = 
                        (av, bv) => {
                            Int32 r_;

                            for(Int32 i = 0; i < Math.Min(av.Count, bv.Count); ++i) {
                                r_ = av[i].CompareTo(bv[i]);

                                if(r_ != 0) { return r_; }
                            }

                            r_ = av.Count.CompareTo(bv.Count);

                            if(r_ != 0) { return r_; }

                            return 0;
                        };

                    Int32 r;

                    r = cmpV(getPositions(a), getPositions(b));

                    if(r != 0) { return r; }

                    r = cmpV(getSides(a), getSides(b));

                    if(r != 0) { return r; }

                    return 0;
                };
            }
            else if(columnName == "Age") {
                comparison = (a, b) => Helpers.CompareNullable(a.Age, b.Age);
            }
            else if(columnName == "Current Ability") {
                comparison = (a, b) => a.CurrentAbility.CompareTo(b.CurrentAbility);
            }
            else if(columnName == "Potential Ability") {
                comparison = (a, b) => a.PotentialAbility.CompareTo(b.PotentialAbility);
            }
            else if(columnName == "Value") {
                comparison = (a, b) => a.Value_.CompareTo(b.Value_);
            }
            else if(columnName == "Wage") {
                comparison = (a, b) => {
                    Int32 wageA = a.Contract != null ? a.Contract.Wage : 0;
                    Int32 wageB = b.Contract != null ? b.Contract.Wage : 0;

                    return wageA.CompareTo(wageB);
                };
            }
            else if(columnName == "Current Reputation") {
                comparison = (a, b) => a.CurrentReputation.CompareTo(b.CurrentReputation);
            }
            else if(columnName == "Home Reputation") {
                comparison = (a, b) => a.HomeReputation.CompareTo(b.HomeReputation);
            }
            else if(columnName == "World Reputation") {
                comparison = (a, b) => a.WorldReputation.CompareTo(b.WorldReputation);
            }
            else if(columnName == "Left Foot") {
                comparison = (a, b) => a.LeftFoot.CompareTo(b.LeftFoot);
            }
            else if(columnName == "Right Foot") {
                comparison = (a, b) => a.RightFoot.CompareTo(b.RightFoot);
            }
            else if((match = Regex.Match(columnName, "Attribute(\\d+)")).Success) {
                Int32 i = Int32.Parse(match.Groups[1].Value);

                comparison = (a, b) => a.Value.AttributeValues[i].InMatch.CompareTo(b.Value.AttributeValues[i].InMatch);
            }
            else {
                throw new Exception(String.Format("Cannot compare column with name \"{0}\"", columnName));
            }

            return comparison(x, y) * (ascending ? 1 : -1);
        }

        private async Task CalculateRatingsAsync() {
            RatingsCalculationViewModel  ratingsCalculationViewModel = new RatingsCalculationViewModel();

            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            timer.Tick += async (sender, args) => {
                timer.Stop();

                App app = (App)Application.Current;

                await app.NavigationService.ShowContentDialogAsync(ratingsCalculationViewModel);
            };

            timer.Start();

            await Task.Run(
                () => {
                    App app = (App)Application.Current;

                    WeightsSet weightsSet = app.SettingsService.GetWeightsSets().Find(item => item.IsLast == true);

                    if(weightsSet != null) {
                        foreach(CMStaff staff in app.DataService.Staffs) {
                            if(DataService.IsValidPlayer(staff)) {
                                DataService.CalculateRating(staff, weightsSet);
                            }
                        }
                    }
                }
            );

            timer.Stop();

            ratingsCalculationViewModel.Hide?.Execute(null);
        }


        private void UpdateColumnsVisibility() {
            if(_setIsColumnVisible == null) {
                return;
            }

            App app = (App)Application.Current;

            List<String> columns = app.SettingsService.GetColumns();

            for(Int32 i = 0; i < Attributes.Length; ++i) {
                _setIsColumnVisible(String.Format("Attribute{0}", i), columns.Contains(Attributes[i].Name));
            }

            List<String> columns2 = app.SettingsService.GetColumns2();

            for(Int32 i = 0; i < SettingsViewModel.Column2Names.Length; ++i) {
                _setIsColumnVisible(SettingsViewModel.Column2Names[i], columns2.Contains(SettingsViewModel.Column2Names[i]));
            }
        }

        private void OnCompareItems() {
            App app = (App)Application.Current;

            CMStaffVM leftPlayer  = (CMStaffVM)CompareItemsPanelViewModel.LeftItem;
            CMStaffVM rightPlayer = (CMStaffVM)CompareItemsPanelViewModel.RightItem;

            app.NavigationService.Navigate(new ComparePlayersViewModel(leftPlayer.Value, rightPlayer.Value));
        }



        private RelayCommand                        _open;
        private RelayCommand                        _reload;
        private RelayCommand                        _filters;
        private RelayCommand                        _weights;
        private RelayCommand                        _favorites;
        private RelayCommand                        _settings;
        private RelayCommand                        _about;
        private ListCollectionViewSource<CMStaffVM> _players;
        private CMStaffVM                           _selectedPlayer;
        private Pair<String, Boolean>               _playersSortColumn;
        private Action<Object>                      _scrollToPlayer;
        private Action<String, Boolean>             _setIsColumnVisible;
        private RelayCommand                        _playerTapped;
        private RelayCommand                        _playerRightTapped;
        private Boolean                             _ignorePlayerTapped;
    }

}
