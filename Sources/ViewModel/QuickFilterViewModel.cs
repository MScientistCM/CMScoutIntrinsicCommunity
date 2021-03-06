using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class QuickFilterViewModel : ViewModelBase {

        public event EventHandler RatingPositionChanged;
        public event EventHandler FilterChanged;

        public QuickFilterViewModel() {
            // Sides
            List<Side> sides = new List<Side>{
                new Side(null, null, null),
            };

            sides.AddRange(DataService.Sides);

            _sidesGroup = new RadioButtonGroupVM<Side>(sides, sides[0], true);

            _sidesGroup.ItemChecked += OnSidesGroupItemChecked;

            // Positions
            List<Position> positions = new List<Position> {
                new Position(null, null, null),
            };

            positions.AddRange(DataService.Positions);

            _positionsGroup = new RadioButtonGroupVM<Position>(positions, positions[0], true);

            _positionsGroup.ItemChecked += OnPositionsGroupItemChecked;

            // Rating positions
            List<RatingPosition> ratingPositionsGroup = new List<RatingPosition> {
                new RatingPosition("Best regard position",     "BP", null),
                new RatingPosition("Best regardless position", "B",  null),
            };

            ratingPositionsGroup.AddRange(DataService.RatingPositions);

            _ratingPositionsGroup = new RadioButtonGroupVM<RatingPosition>(ratingPositionsGroup, ratingPositionsGroup[0], true);

            _ratingPositionsGroup.ItemChecked += OnRatingPositionsGroupItemChecked;

            // Player Name
            _palyerNameTimer = new DispatcherTimer();

            _palyerNameTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            _palyerNameTimer.Tick += (sender, args) => {
                _palyerNameTimer.Stop();

                FilterChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public String AppliedFilterName { get; private set; }

        public void UpdateAppliedFilterName(String appliedFilterName) {
            AppliedFilterName = appliedFilterName;
            RaisePropertyChanged(nameof(AppliedFilterName));
        }

        public String PlayerName {
            get {
                return _playerName;
            }

            set {
                _playerName = value;

                RaisePropertyChanged();

                _palyerNameTimer.Stop();
                _palyerNameTimer.Start();
            }
        }

        public ObservableCollectionEx< RadioButtonVM<Side> > Sides        => _sidesGroup.View;
        public Side                                          SelectedSide => _sidesGroup.GetCheckedItem();

        public ObservableCollectionEx< RadioButtonVM<Position> > Positions        => _positionsGroup.View;
        public Position                                          SelectedPosition => _positionsGroup.GetCheckedItem();

        public ObservableCollectionEx< RadioButtonVM<RatingPosition> > RatingPositions        => _ratingPositionsGroup.View;
        public RatingPosition                                          SelectedRatingPosition => _ratingPositionsGroup.GetCheckedItem();



        private void OnSidesGroupItemChecked(Object sender, RadioButtonGroupVM<Side>.ItemCheckedEventArgs args) {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPositionsGroupItemChecked(Object sender, RadioButtonGroupVM<Position>.ItemCheckedEventArgs args) {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnRatingPositionsGroupItemChecked(Object sender, RadioButtonGroupVM<RatingPosition>.ItemCheckedEventArgs args) {
            RaisePropertyChanged(nameof(SelectedRatingPosition));

            RatingPositionChanged?.Invoke(this, EventArgs.Empty);
        }



        private String                             _playerName;
        private DispatcherTimer                    _palyerNameTimer;
        private RadioButtonGroupVM<Side>           _sidesGroup;
        private RadioButtonGroupVM<Position>       _positionsGroup;
        private RadioButtonGroupVM<RatingPosition> _ratingPositionsGroup;
    }

}
