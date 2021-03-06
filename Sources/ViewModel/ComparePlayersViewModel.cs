using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class ComparePlayersViewModel : PageViewModelBase {
        public ComparePlayersViewModel(CMStaff leftPlayer, CMStaff rightPlayer) {
            App app = (App)Application.Current;

            LeftPlayer  = new CMStaffVM(leftPlayer);
            RightPlayer = new CMStaffVM(rightPlayer);

            LeftPlayer.UpdateAttributes(CA18ViewMode.InMatchNormalized);
            LeftPlayer.UpdateRatings();

            RightPlayer.UpdateAttributes(CA18ViewMode.InMatchNormalized);
            RightPlayer.UpdateRatings();

            // Rating positions
            RatingPosition[] intersectedRatingPositions = DataService.RatingPositions;

            {
                IEnumerable<RatingPosition> leftRatingPositions = DataService.RatingPositions.Where(item => item.IsSuitableFor(LeftPlayer.Value));

                if(leftRatingPositions != null && leftRatingPositions.Any()) {
                    IEnumerable<RatingPosition> rightRatingPositions = DataService.RatingPositions.Where(item => item.IsSuitableFor(RightPlayer.Value));

                    if(rightRatingPositions != null && rightRatingPositions.Any()) {
                        IEnumerable<RatingPosition> intersectedRatingPositions_ = leftRatingPositions.Intersect(rightRatingPositions);

                        if(intersectedRatingPositions_ != null && intersectedRatingPositions_.Any()) {
                            intersectedRatingPositions = intersectedRatingPositions_.ToArray();
                        }
                    }
                }
            }

            _ratingPositionsGroup = new RadioButtonGroupVM<RatingPosition>(DataService.RatingPositions, intersectedRatingPositions[intersectedRatingPositions.Length / 2], true);

            _ratingPositionsGroup.ItemChecked += OnRatingPositionsGroupItemChecked;

            UpdateRating();

            app.SettingsService.CA18HighlightChanged += OnCA18HighlightChanged;
        }

        protected override void OnRemoved() {
            App app = (App)Application.Current;

            app.SettingsService.CA18HighlightChanged -= OnCA18HighlightChanged;
        }

        public CMStaffVM LeftPlayer  { get; }
        public CMStaffVM RightPlayer { get; }

        public ObservableCollectionEx< RadioButtonVM<RatingPosition> > RatingPositions        => _ratingPositionsGroup.View;
        public RatingPosition                                          SelectedRatingPosition => _ratingPositionsGroup.GetCheckedItem();

        public static Attribute[] Attributes => DataService.Attributes;

        public Boolean CA18Highlight {
            get {
                return false; //((App)Application.Current).SettingsService.GetCA18Highlight();
            }
        }



        private void OnRatingPositionsGroupItemChecked(Object sender, RadioButtonGroupVM<RatingPosition>.ItemCheckedEventArgs args) {
            RaisePropertyChanged(nameof(SelectedRatingPosition));

            UpdateRating();
        }

        private void UpdateRating() {
            LeftPlayer.UpdateRating(SelectedRatingPosition.Code);
            RightPlayer.UpdateRating(SelectedRatingPosition.Code);
        }

        private void OnCA18HighlightChanged(Object sender, EventArgs eventArgs) {
            RaisePropertyChanged(nameof(CA18Highlight));
        }



        private RadioButtonGroupVM<RatingPosition> _ratingPositionsGroup;
    }

}
