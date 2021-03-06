using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CMScoutIntrinsic {

    class MruFileVM {
        public MruFileVM(MruFile value) {
            Value = value;
        }

        public readonly MruFile Value;

        public String Path => Value.File.Path;
    }

    class SideVM : ViewModelBase {
        public event EventHandler IsCheckedChanged;

        public SideVM(Side value) {
            Value = value;
        }

        public readonly Side Value;

        public String Name => Value.Name;
        public String Code => Value.Code;

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

    class PositionVM : ViewModelBase {
        public event EventHandler IsCheckedChanged;

        public PositionVM(Position value) {
            Value = value;
        }

        public readonly Position Value;

        public String Name => Value.Name;
        public String Code => Value.Code;

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

    class RatingPositionVM : ViewModelBase {
        public RatingPositionVM(RatingPosition value) {
            Value = value;
        }

        public readonly RatingPosition Value;

        public String Name => Value.Name;
        public String Code => Value.Code;
    }

    class AttributeVM : ViewModelBase {
        public event EventHandler ValueChanged;
        public event EventHandler IsCheckedChanged;

        public AttributeVM(Attribute attribute) {
            Attribute = attribute;
        }

        public readonly Attribute Attribute;

        public String Name  => Attribute.Name;

        public Int16  Value {
            get {
                return _value;
            }
            
            set {
                _value = value;
                
                RaisePropertyChanged();

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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



        private Int16   _value;
        private Boolean _isChecked;
    }

    class RatingVM : ViewModelBase {
        public RatingVM (RatingPosition ratingPosition) {
            RatingPosition = ratingPosition;
        }

        public readonly RatingPosition RatingPosition;

        public String  Name => RatingPosition.Name;
        public String  Code => RatingPosition.Code;
        public Double  Value   { get { return _value;  } set { _value  = value; RaisePropertyChanged(); } }

        private Double _value;
    }

    class CMContractVM : ViewModelBase {
        public CMContractVM(CMContract value) {
            Value = value;
        }

        public readonly CMContract Value;

        public Byte      Type                   => Value.Type;
        public Byte      SquadStatus            => Value.SquadStatus;
        public Byte      TransferStatus         => Value.TransferStatus;
        public DateTime? DateStarted            => Value.DateStarted;
        public DateTime? DateEnded              => Value.DateEnded;
        public Int32     Wage                   => Value.Wage;
        public String    UnprotectedReason      => Value.UnprotectedReason;
        public Boolean   HasBonuses             => Value.GoalBonus != -1 || Value.AssistBonus != -1 || Value.CleanSheetBonus != -1; 
        public Int32     GoalBonus              => Value.GoalBonus;
        public Int32     AssistBonus            => Value.AssistBonus;
        public Int32     CleanSheetBonus        => Value.CleanSheetBonus;
        public Boolean   HasRC                  => Value.HasNonPromotionRC || Value.HasMinimumFeeRC || Value.HasNonPlayingRC || Value.HasRelegationRC || Value.HasManagerJobRC;
        public Boolean   HasNonPromotionRC      => Value.HasNonPromotionRC;
        public Boolean   IsNonPromotionRCActive => Value.IsNonPromotionRCActive;
        public Boolean   HasMinimumFeeRC        => Value.HasMinimumFeeRC;
        public Boolean   IsMinimumFeeRCActive   => Value.IsMinimumFeeRCActive;
        public Boolean   HasNonPlayingRC        => Value.HasNonPlayingRC;
        public Boolean   IsNonPlayingRCActive   => Value.IsNonPlayingRCActive;
        public Boolean   HasRelegationRC        => Value.HasRelegationRC;
        public Boolean   IsRelegationRCActive   => Value.IsRelegationRCActive;
        public Boolean   HasManagerJobRC        => Value.HasManagerJobRC;
        public Boolean   IsManagerJobRCActive   => Value.IsManagerJobRCActive;
        public Int32     ReleaseFee             => Value.ReleaseFee;
    }

    class CMStaffVM : ViewModelBase {
        public CMStaffVM(CMStaff value) {
            Value = value;

            Attributes = new AttributeVM[DataService.Attributes.Length];

            for(Int32 i = 0; i < DataService.Attributes.Length; ++i) {
                Attributes[i] = new AttributeVM(DataService.Attributes[i]);
            }

            Ratings = new RatingVM[DataService.RatingPositions.Length];

            for(Int32 i = 0; i < DataService.RatingPositions.Length; ++i) {
                Ratings[i] = new RatingVM(DataService.RatingPositions[i]);
            }

            Contract     = (Value.Contract     != null ? new CMContractVM(Value.Contract)     : null);
            LoanContract = (Value.LoanContract != null ? new CMContractVM(Value.LoanContract) : null);
        }

        public readonly CMStaff Value;

        public String SecondName {
            get {
                Boolean hasCommonName = !String.IsNullOrEmpty(Value.CommonName?.Name);
                Boolean hasSecondName = !String.IsNullOrEmpty(Value.SecondName?.Name);
                Boolean hasFirstName  = !String.IsNullOrEmpty(Value.FirstName?.Name);

                if(hasCommonName) {
                    return Value.CommonName.Name;
                }
                else if(hasSecondName) {
                    return Value.SecondName.Name;
                }
                else if(hasFirstName) {
                    return Value.FirstName.Name;
                }
                else {
                    return String.Empty;
                }
            }
        }

        public String FirstName {
            get {
                Boolean hasCommonName = !String.IsNullOrEmpty(Value.CommonName?.Name);
                Boolean hasSecondName = !String.IsNullOrEmpty(Value.SecondName?.Name);
                Boolean hasFirstName  = !String.IsNullOrEmpty(Value.FirstName?.Name);

                if(hasCommonName) {
                    return String.Empty;
                }
                else {
                    if(hasSecondName) {
                        if(hasFirstName) {
                            return Value.FirstName.Name;
                        }
                        else {
                            return String.Empty;
                        }
                    }
                    else {
                        return String.Empty;
                    }
                }
            }
        }

        public String FirstNationName         => !String.IsNullOrEmpty(Value.FirstNation?.Name)         ? Value.FirstNation.Name         : String.Empty;
        public String SecondNationName        => !String.IsNullOrEmpty(Value.SecondNation?.Name)        ? Value.SecondNation.Name        : String.Empty;
        public String FirstNationCode         => !String.IsNullOrEmpty(Value.FirstNation?.Code)         ? Value.FirstNation.Code         : String.Empty;
        public String SecondNationCode        => !String.IsNullOrEmpty(Value.SecondNation?.Code)        ? Value.SecondNation.Code        : String.Empty;
        public String FirstNationNationality  => !String.IsNullOrEmpty(Value.FirstNation?.Nationality)  ? Value.FirstNation.Nationality  : String.Empty;
        public String SecondNationNationality => !String.IsNullOrEmpty(Value.SecondNation?.Nationality) ? Value.SecondNation.Nationality : String.Empty;
        public String ClubName                => !String.IsNullOrEmpty(Value.ClubJob?.Name)             ? Value.ClubJob.Name             : String.Empty;
        public String ClubShortName           => !String.IsNullOrEmpty(Value.ClubJob?.Name)             ? Value.ClubJob.ShortName        : String.Empty;

        public DateTime?    DateOfBirth    => Value.DateOfBirth;
        public Int32?       Age            => Value.Age;
        public Int32        Value_         => Value.Value;
        public CMContractVM Contract       { get; }
        public CMContractVM LoanContract   { get; }

        public AttributeVM[]             Attributes { get; }

        public void UpdateAttributes(CA18ViewMode ca18ViewMode) {
            for(Int32 i = 0; i < Attributes.Length; ++i) {
                Attributes[i].Value = (
                    ca18ViewMode == CA18ViewMode.Intrinsic            ? Value.AttributeValues[i].Intrinsic            :
                    ca18ViewMode == CA18ViewMode.IntrinsicGraemeKelly ? Value.AttributeValues[i].IntrinsicGraemeKelly :
                    ca18ViewMode == CA18ViewMode.IntrinsicNormalized  ? Value.AttributeValues[i].IntrinsicNormalized  :
                                                                        Value.AttributeValues[i].InGame
                );
            }
        }

        public SByte Anticipation        => Value.Player.Anticipation;
        public SByte Creativity          => Value.Player.Creativity;
        public SByte Crossing            => Value.Player.Crossing;
        public SByte Decisions           => Value.Player.Decisions;
        public SByte Dribbling           => Value.Player.Dribbling;
        public SByte Finishing           => Value.Player.Finishing;
        public SByte Heading             => Value.Player.Heading;
        public SByte LongShots           => Value.Player.LongShots;
        public SByte Marking             => Value.Player.Marking;
        public SByte OffTheBall          => Value.Player.OffTheBall;
        public SByte Passing             => Value.Player.Passing;
        public SByte Penalties           => Value.Player.Penalties;
        public SByte Positioning         => Value.Player.Positioning;
        public SByte Tackling            => Value.Player.Tackling;
        public SByte ThrowIns            => Value.Player.ThrowIns;
        public SByte Handling            => Value.Player.Handling;
        public SByte OneOnOnes           => Value.Player.OneOnOnes;
        public SByte Reflexes            => Value.Player.Reflexes;
        public SByte Acceleration        => Value.Player.Acceleration;
        public SByte Agility             => Value.Player.Agility;
        public SByte Balance             => Value.Player.Balance;
        public SByte Corners             => Value.Player.Corners;
        public SByte Flair               => Value.Player.Flair;
        public SByte InjuryProneness     => Value.Player.InjuryProneness;
        public SByte Jumping             => Value.Player.Jumping;
        public SByte NaturalFitness      => Value.Player.NaturalFitness;
        public SByte Pace                => Value.Player.Pace;
        public SByte SetPieces           => Value.Player.SetPieces;
        public SByte Stamina             => Value.Player.Stamina;
        public SByte Strength            => Value.Player.Strength;
        public SByte Technique           => Value.Player.Technique;
        public SByte WorkRate            => Value.Player.WorkRate;
        public SByte Adaptability        => Value.Adaptability;
        public SByte Aggression          => Value.Player.Aggression;
        public SByte Ambition            => Value.Ambition;
        public SByte Bravery             => Value.Player.Bravery;
        public SByte Consistency         => Value.Player.Consistency;
        public SByte Determination       => Value.Determination;
        public SByte Dirtiness           => Value.Player.Dirtiness;
        public SByte ImportantMatches    => Value.Player.ImportantMatches;
        public SByte Loyality            => Value.Loyality;
        public SByte Influence           => Value.Player.Influence;
        public SByte Pressure            => Value.Pressure;
        public SByte Professionalism     => Value.Professionalism;
        public SByte Sportsmanship       => Value.Sportsmanship;
        public SByte Teamwork            => Value.Player.Teamwork;
        public SByte Temperament         => Value.Temperament;
        public SByte Versatility         => Value.Player.Versatility;

        public SByte RightFoot           => Value.Player.RightFoot;
        public SByte LeftFoot            => Value.Player.LeftFoot;

        public Int16 CurrentAbility      => Value.Player.CurrentAbility;
        public Int16 PotentialAbility    => Value.Player.PotentialAbility;

        public Int16 CurrentReputation   => Value.Player.CurrentReputation;
        public Int16 HomeReputation      => Value.Player.HomeReputation;
        public Int16 WorldReputation     => Value.Player.WorldReputation;

        public SByte RightSide           => Value.Player.RightSide;
        public SByte LeftSide            => Value.Player.LeftSide;
        public SByte CentreSide          => Value.Player.CentreSide;

        public SByte Goalkeeper          => Value.Player.Goalkeeper;
        public SByte Sweeper             => Value.Player.Sweeper;
        public SByte Defender            => Value.Player.Defender;
        public SByte DefensiveMidfielder => Value.Player.DefensiveMidfielder;
        public SByte Midfielder          => Value.Player.Midfielder;
        public SByte AttackingMidfielder => Value.Player.AttackingMidfielder;
        public SByte Attacker            => Value.Player.Attacker;
        public SByte WingBack            => Value.Player.WingBack;
        public SByte FreeRole            => Value.Player.FreeRole;

        public RatingVM[] Ratings { get; }
        public RatingVM   Rating  { get; private set; }

        public void UpdateRatings() {
            for(Int32 i = 0; i < Ratings.Length; ++i) {
                Ratings[i].Value = Value.Ratings[i];
            }
        }

        public void UpdateRating(String code) {
            Rating = Ratings.Where(
                item => {
                    if(code == "BP") {
                        return item.RatingPosition.IsSuitableFor(Value);
                    }
                    else if(code == "B") {
                        return true;
                    }
                    else {
                        return item.RatingPosition.Code == code;
                    }
                }
            ).FindMax( (a, b) => a.Value.CompareTo(b.Value) );

            RaisePropertyChanged(nameof(Rating));
        }
    }

    class FilterVM : ViewModelBase {
        public FilterVM(Filter value) {
            Value = value;

            // Sides
            Sides = new SideVM[Value.Sides.Length];

            for(Int32 i = 0; i < Value.Sides.Length; ++i) {
                Sides[i] = new SideVM(DataService.Sides[i]);

                Sides[i].IsChecked = Value.Sides[i];

                Sides[i].IsCheckedChanged += OnSideIsCheckedChanged;
            }

            // Positions
            Positions = new PositionVM[Value.Positions.Length];

            for(Int32 i = 0; i < Value.Positions.Length; ++i) {
                Positions[i] = new PositionVM(DataService.Positions[i]);

                Positions[i].IsChecked = Value.Positions[i];

                Positions[i].IsCheckedChanged += OnPositionIsCheckedChanged;
            }

            // Attributes
            Attributes = new AttributeVM[Value.Attributes.Length];

            for(Int32 i = 0; i < Value.Attributes.Length; ++i) {
                Attributes[i] = new AttributeVM(DataService.Attributes[i]);

                Attributes[i].Value = value.Attributes[i];

                Attributes[i].ValueChanged += OnAttributeValueChanged;
            }
        }

        public readonly Filter Value;

        public Boolean  IsLast                    { get { return Value.IsLast;                    } set { Value.IsLast                    = value; RaisePropertyChanged(); } }
        public String   Name                      { get { return Value.Name;                      } set { Value.Name                      = value; RaisePropertyChanged(); } }
        public String   PlayerName                { get { return Value.PlayerName;                } set { Value.PlayerName                = value; RaisePropertyChanged(); } }
        public String   NationGroupName           { get { return Value.NationGroupName;           } set { Value.NationGroupName           = value; RaisePropertyChanged(); } }
        public String   ClubName                  { get { return Value.ClubName;                  } set { Value.ClubName                  = value; RaisePropertyChanged(); } }
        public String   DivisionName              { get { return Value.DivisionName;              } set { Value.DivisionName              = value; RaisePropertyChanged(); } }
        public Int32    AgeFrom                   { get { return Value.AgeFrom;                   } set { Value.AgeFrom                   = value; RaisePropertyChanged(); } }
        public Int32    AgeTo                     { get { return Value.AgeTo;                     } set { Value.AgeTo                     = value; RaisePropertyChanged(); } }
        public Int32    CAFrom                    { get { return Value.CAFrom;                    } set { Value.CAFrom                    = value; RaisePropertyChanged(); } }
        public Int32    CATo                      { get { return Value.CATo;                      } set { Value.CATo                      = value; RaisePropertyChanged(); } }
        public Int32    PAFrom                    { get { return Value.PAFrom;                    } set { Value.PAFrom                    = value; RaisePropertyChanged(); } }
        public Int32    PATo                      { get { return Value.PATo;                      } set { Value.PATo                      = value; RaisePropertyChanged(); } }
        public Int32    ValueFrom                 { get { return Value.ValueFrom;                 } set { Value.ValueFrom                 = value; RaisePropertyChanged(); } }
        public Int32    ValueTo                   { get { return Value.ValueTo;                   } set { Value.ValueTo                   = value; RaisePropertyChanged(); } }
        public Int32    WageFrom                  { get { return Value.WageFrom;                  } set { Value.WageFrom                  = value; RaisePropertyChanged(); } }
        public Int32    WageTo                    { get { return Value.WageTo;                    } set { Value.WageTo                    = value; RaisePropertyChanged(); } }
        public Boolean  IsFreeTransfer            { get { return Value.IsFreeTransfer;            } set { Value.IsFreeTransfer            = value; RaisePropertyChanged(); } }
        public Boolean  IsContractExpired         { get { return Value.IsContractExpired;         } set { Value.IsContractExpired         = value; RaisePropertyChanged(); } }
        public Boolean  IsContractExpiring        { get { return Value.IsContractExpiring;        } set { Value.IsContractExpiring        = value; RaisePropertyChanged(); } }
        public Boolean  IsContractUnprotected     { get { return Value.IsContractUnprotected;     } set { Value.IsContractUnprotected     = value; RaisePropertyChanged(); } }
        public Boolean? IsLeavingOnBosman         { get { return Value.IsLeavingOnBosman;         } set { Value.IsLeavingOnBosman         = value; RaisePropertyChanged(); } }
        public Boolean? IsTransferArranged        { get { return Value.IsTransferArranged;        } set { Value.IsTransferArranged        = value; RaisePropertyChanged(); } }
        public Boolean  IsTransferListedByClub    { get { return Value.IsTransferListedByClub;    } set { Value.IsTransferListedByClub    = value; RaisePropertyChanged(); } }
        public Boolean  IsTransferListedByRequest { get { return Value.IsTransferListedByRequest; } set { Value.IsTransferListedByRequest = value; RaisePropertyChanged(); } }
        public Boolean  IsListedForLoan           { get { return Value.IsListedForLoan;           } set { Value.IsListedForLoan           = value; RaisePropertyChanged(); } }

        public SideVM[]      Sides      { get; }
        public PositionVM[]  Positions  { get; }
        public AttributeVM[] Attributes { get; }

        private void OnSideIsCheckedChanged(Object sender, EventArgs args) {
            Int32 i = Sides.FindIndex(item => ReferenceEquals(item, sender));

            Value.Sides[i] = Sides[i].IsChecked;
        }

        private void OnPositionIsCheckedChanged(Object sender, EventArgs args) {
            Int32 i = Positions.FindIndex(item => ReferenceEquals(item, sender));

            Value.Positions[i] = Positions[i].IsChecked;
        }

        private void OnAttributeValueChanged(Object sender, EventArgs args) {
            Int32 i = Attributes.FindIndex(item => ReferenceEquals(item, sender));

            Value.Attributes[i] = (SByte)Attributes[i].Value;
        }
    }

    class NationGroupVM {
        public NationGroupVM(CMNation nation) {
            Name    = nation.Name;
            Code    = nation.Code;
            Nations = new List<CMNation> { nation };
        }

        public NationGroupVM(String name, String code, List<CMNation> nations) {
            Name    = name;
            Code    = code;
            Nations = nations;
        }

        public String         Name { get; set; }
        public String         Code { get; set; }
        public List<CMNation> Nations;
    }

    class CMDivisionVM {
        public CMDivisionVM(CMDivision value) {
            Name = value.Name;
        }

        public CMDivisionVM(String name) {
            Name = name;
        }

        public String Name { get; }
    }

    class CMClubVM {
        public CMClubVM(CMClub value) {
            Name = value.Name;
        }

        public CMClubVM(String name) {
            Name = name;
        }

        public String Name { get; }
    }

    class WeightVM : ViewModelBase {
        public event EventHandler ValueChanged;

        public WeightVM() {
        }

        public Int32 Value {
            get {
                return _value;
            }
            
            set {
                _value = value;
                
                RaisePropertyChanged();

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }



        private Int32 _value;
    }

    class WeightsSetVM : ViewModelBase {
        public WeightsSetVM(WeightsSet value) {
            Value = value;

            Weights = new WeightVM[Value.Weights.Length][];

            for(Int32 i = 0; i < Value.Weights.Length; ++i) {
                Byte[] weights = Value.Weights[i];

                Weights[i] = new WeightVM[weights.Length];

                for(Int32 j = 0; j < weights.Length; ++j) {
                    Byte weight = weights[j];

                    WeightVM weightVM = new WeightVM();

                    weightVM.Value = weight;

                    weightVM.ValueChanged += OnWeightValueChanged;

                    Weights[i][j] = weightVM;
                }
            }
        }

        public readonly WeightsSet Value;

        public Boolean IsLast { get { return Value.IsLast; } set { Value.IsLast = value; RaisePropertyChanged(); } }
        public String  Name   { get { return Value.Name;   } set { Value.Name   = value; RaisePropertyChanged(); } }

        public WeightVM[][] Weights { get; }



        private void OnWeightValueChanged(Object sender, EventArgs args) {
            for(Int32 i = 0; i < Weights.Length; ++i) {
                for(Int32 j = 0; j < Weights[i].Length; ++j) {
                    if(ReferenceEquals(Weights[i][j], sender)) {
                        Value.Weights[i][j] = (Byte)Weights[i][j].Value;
                    }
                }
            }
        }
    }

    // CompareItemsPanelVM
    class CompareItemsPanelVM : ViewModelBase {
        public CompareItemsPanelVM(Action onCompare) {
            _onCompare = onCompare;
        }

        public void AddItem(Object item) {
            if(LeftItem == null) {
                LeftItem = item;
            }
            else if(RightItem == null) {
                RightItem = item;
            }
            else {
                LeftItem  = RightItem;
                RightItem = item;
            }
        }

        public Object LeftItem {
            get {
                return _leftItem;
            }
            
            private set {
                _leftItem  = value;
                
                RaisePropertyChanged();

                RaisePropertyChanged(nameof(IsEmpty));
                RaisePropertyChanged(nameof(State));

                _removeLeftItem?.RaiseCanExecuteChanged();
                _clear?.RaiseCanExecuteChanged();
                _compare?.RaiseCanExecuteChanged();
            }
        }

        public Object RightItem {
            get {
                return _rightItem;
            } 
            
            private set {
                _rightItem = value;
                
                RaisePropertyChanged();

                RaisePropertyChanged(nameof(IsEmpty));
                RaisePropertyChanged(nameof(State));

                _removeRightItem?.RaiseCanExecuteChanged();
                _clear?.RaiseCanExecuteChanged();
                _compare?.RaiseCanExecuteChanged();

            }
        }

        public ICommand RemoveLeftItem {
            get {
                return _removeLeftItem ?? (
                    _removeLeftItem = new RelayCommand(
                        param => {
                            LeftItem  = RightItem;
                            RightItem = null;
                        },
                        param => _leftItem != null
                    )
                );
            }
        }

        public ICommand RemoveRightItem {
            get {
                return _removeRightItem ?? (
                    _removeRightItem = new RelayCommand(
                        param => {
                            RightItem = null;
                        },
                        param => _rightItem != null
                    )
                );
            }
        }

        public ICommand Clear {
            get {
                return _clear ?? (
                    _clear = new RelayCommand(
                        param => {
                            RightItem = null;
                            LeftItem  = null;
                        },
                        param => _leftItem != null || _rightItem != null
                    )
                );
            }
        }

        public ICommand Compare {
            get {
                return _compare ?? (
                    _compare = new RelayCommand(
                        param => {
                            _onCompare.Invoke();
                        },
                        param => _leftItem != null && _rightItem != null
                    )
                );
            }
        }

        public Boolean IsEmpty => _leftItem == null && _rightItem == null;

        public String State => (_leftItem == null && _rightItem == null ? "Collapsed" : "Visible");



        private Action       _onCompare;
        private Object       _leftItem;
        private Object       _rightItem;
        private RelayCommand _removeLeftItem;
        private RelayCommand _removeRightItem;
        private RelayCommand _clear;
        private RelayCommand _compare;
    }

}
