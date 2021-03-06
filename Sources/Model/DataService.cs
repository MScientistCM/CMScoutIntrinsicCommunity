using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CMScoutIntrinsic {

    class MruFile {
        public readonly StorageFile File;
        public readonly String      Token;

        public MruFile(StorageFile file, String token) {
            File  = file;
            Token = token;
        }
    }

    class Attribute {
        public String  Name         { get; }
        public String  Code         { get; }
        public Boolean IsCA18       { get; }
        public Boolean IsLessBetter { get; }

        public readonly Func<CMStaff, SByte> IntrinsicValueExtractor;

        public Attribute(String name, String code, Boolean isCA18, Boolean isLessBetter, Func<CMStaff, SByte> intrinsicValueExtractor) {
            Name                   = name;
            Code                    = code;
            IsCA18                  = isCA18;
            IsLessBetter            = isLessBetter;
            IntrinsicValueExtractor = intrinsicValueExtractor;
        }
    }

    class AttributeValue {
        public readonly SByte Intrinsic;
        public readonly Int16 IntrinsicGraemeKelly;
        public readonly SByte IntrinsicNormalized;
        public readonly SByte InGame;

        public AttributeValue(SByte intrinsic, Int16 intrinsicGraemeKelly, SByte intrinsicNormalized, SByte inGame) {
            Intrinsic            = intrinsic;
            IntrinsicGraemeKelly = intrinsicGraemeKelly;
            IntrinsicNormalized  = intrinsicNormalized;
            InGame               = inGame;
        }
    }

    class Side {
        public String Name { get; }
        public String Code { get; }

        public readonly Func<CMStaff, Boolean> IsSuitableFor;

        public Side(String name, String code, Func<CMStaff, Boolean> isSuitableFor) {
            Name          = name;
            Code          = code;
            IsSuitableFor = isSuitableFor;
        }
    }

    class Position {
        public String Name { get; }
        public String Code { get; }

        public readonly Func<CMStaff, Boolean> IsSuitableFor;

        public Position(String name, String code, Func<CMStaff, Boolean> isSuitableFor) {
            Name          = name;
            Code          = code;
            IsSuitableFor = isSuitableFor;
        }
    }

    class RatingPosition {
        public String Name { get; }
        public String Code { get; }

        public readonly Func<CMStaff, Boolean> IsSuitableFor;

        public RatingPosition(String name, String code, Func<CMStaff, Boolean> isSuitableFor) {
            Name          = name;
            Code          = code;
            IsSuitableFor = isSuitableFor;
        }
    }

    class ContractType {
        public Int32  Id   { get; }
        public String Name { get; }

        public readonly Func<Byte, Boolean> IsSuitableFor;

        public ContractType(Int32 id, String name, Func<Byte, Boolean> isSuitableFor) {
            Id            = id;
            Name          = name;
            IsSuitableFor = isSuitableFor;
        }
    }

    class SquadStatus {
        public Int32  Id   { get; }
        public String Name { get; }

        public readonly Func<Byte, Boolean> IsSuitableFor;

        public SquadStatus(Int32 id, String name, Func<Byte, Boolean> isSuitableFor) {
            Id            = id;
            Name          = name;
            IsSuitableFor = isSuitableFor;
        }
    }

    class TransferStatus {
        public Int32  Id   { get; }
        public String Name { get; }

        public readonly Func<Byte, Boolean> IsSuitableFor;

        public TransferStatus(Int32 id, String name, Func<Byte, Boolean> isSuitableFor) {
            Id            = id;
            Name          = name;
            IsSuitableFor = isSuitableFor;
        }
    }

    class CMBlock {
        public Int32  Position;
        public Int32  Size;
        public String Name;
    }
    class CMDate {
        public Int16 Day;
        public Int16 Year;
        public Int32 IsLeapYear;
    }

    class CMNation {
        public Int32  Id;
        public String Name;
        public SByte  GenderName;
        public String ShortName;
        public SByte  GenderShortName;
        public String Code;
        public String Nationality;
        public Int32  ContinentId;
        public SByte  Region;
        public SByte  ActualRegion;
        public SByte  FirstLanguage;
        public SByte  SecondLanguage;
        public SByte  ThirdLanguage;
        public Int32  CapitalId;
        public SByte  StateOfDevelopment;
        public SByte  GroupMembership;
        public Int32  NationalStadiumId;
        public SByte  GameImportance;
        public SByte  LeagueStandard;
        public Int16  NumberClubs;
        public Int32  NumberStaff;
        public Int16  UpdateDay;
        public Int16  Reputation;
        public Int32  ForeColour1Id;
        public Int32  BackColour1Id;
        public Int32  ForeColour2Id;
        public Int32  BackColour2Id;
        public Int32  ForeColour3Id;
        public Int32  BackColour3Id;
        public Double FIFACoefficient;
        public Double FIFACoefficient91;
        public Double FIFACoefficient92;
        public Double FIFACoefficient93;
        public Double FIFACoefficient94;
        public Double FIFACoefficient95;
        public Double FIFACoefficient96;
        public Double UEFACoefficient91;
        public Double UEFACoefficient92;
        public Double UEFACoefficient93;
        public Double UEFACoefficient94;
        public Double UEFACoefficient95;
        public Double UEFACoefficient96;
        public Int32  RivalNation1Id;
        public Int32  RivalNation2Id;
        public Int32  RivalNation3Id;
        public SByte  LeagueSelected;
        public Int32  ShortlistOffset;
        public SByte  GamesPlayed;
    }

    class CMDivision {
        public Int32    Id;
        public String   Name;
        public SByte    GenderName;
        public String   ShortName;
        public SByte    GenderShortName;
        public String   Code;
        public SByte    Scope;
        public SByte    Selected;
        public Int32    ContinentId;
        public Int32    NationId;
        public Int32    ForeColourId;
        public Int32    BackColourId;
        public Int16    Reputation;

        public CMNation Nation;
    } 

    class CMClub {
        public Int32   Id;
        public String  Name;
        public SByte   GenderName;
        public String  ShortName;
        public SByte   GenderShortName;
        public Int32   NationId;
        public Int32   DivisionId;
        public Int32   LastDivisionId;
        public SByte   LastPosition;
        public Int32   ReserveDivisionId;
        public SByte   ProfessionalStatus;
        public Int32   Cash;
        public Int32   StadiumId;
        public SByte   OwnStadium;
        public Int32   ReserveStadiumId;
        public SByte   MatchDay;
        public Int32   Attendance;
        public Int32   MinAttendance;
        public Int32   MaxAttendance;
        public SByte   Training;
        public Int16   Reputation;
        public SByte   PLC;
        public Int32   ForeColour1Id;
        public Int32   BackColour1Id;
        public Int32   ForeColour2Id;
        public Int32   BackColour2Id;
        public Int32   ForeColour3Id;
        public Int32   BackColour3Id;
        public Int32   FavStaff1Id;
        public Int32   FavStaff2Id;
        public Int32   FavStaff3Id;
        public Int32   DisStaff1Id;
        public Int32   DisStaff2Id;
        public Int32   DisStaff3Id;
        public Int32   Rival1Id;
        public Int32   Rival2Id;
        public Int32   Rival3Id;
        public Int32   ChairmanId;
        public Int32[] DirectorsIds;
        public Int32   ManagerId;
        public Int32   AssistantManagerId;
        public Int32[] SquadIds;
        public Int32[] CoachIds;
        public Int32[] ScoutIds;
        public Int32[] PhysioIds;
        public Int32   EuroFlag;
        public SByte   EuroSeeding;
        public Int32[] TeamSelectedIds;
        public Int32[] TacticTrainingIds;
        public Int32   TacticSelected;
        public SByte   HasLinkedClub;
    }

    class CMName {
        public String Name;
        public Int32  Id;
        public Int32  NationId;
        public SByte  Count;
    }

    class CMContract {
        public Int32     Id;
        public Int32     ClubId;
        public Byte[]    Unknown1;
        public Int32     Wage;
        public Int32     GoalBonus;
        public Int32     AssistBonus;
        public Int32     CleanSheetBonus;
        public Byte      NonPromotionRC;
        public Byte      MinimumFeeRC;
        public Byte      NonPlayingRC;
        public Byte      RelegationRC;
        public Byte      ManagerJobRC;
        public Int32     ReleaseFee;
        public DateTime? DateStarted;
        public DateTime? DateEnded;
        public Byte      Type;
        public Byte[]    Unknown2;
        public Boolean   LeavingOnBosman;
        public Int32     TransferArrangedForClubId;
        public Byte      TransferStatus;
        public Byte      SquadStatus;

        public CMClub    Club;
        public CMClub    TransferArrangedForClub;
        public String    UnprotectedReason;
        public Boolean   HasNonPromotionRC;
        public Boolean   IsNonPromotionRCActive;
        public Boolean   HasMinimumFeeRC;
        public Boolean   IsMinimumFeeRCActive;
        public Boolean   HasNonPlayingRC;
        public Boolean   IsNonPlayingRCActive;
        public Boolean   HasRelegationRC;
        public Boolean   IsRelegationRCActive;
        public Boolean   HasManagerJobRC;
        public Boolean   IsManagerJobRCActive;
    }

    class CMPlayer {
        public Int32 Id;
        public Byte  SquardNumber;
        public Int16 CurrentAbility;
        public Int16 PotentialAbility;
        public Int16 HomeReputation;
        public Int16 CurrentReputation;
        public Int16 WorldReputation;
        public SByte Goalkeeper;
        public SByte Sweeper;
        public SByte Defender;
        public SByte DefensiveMidfielder;
        public SByte Midfielder;
        public SByte AttackingMidfielder;
        public SByte Attacker;
        public SByte WingBack;
        public SByte RightSide;
        public SByte LeftSide;
        public SByte CentreSide;
        public SByte FreeRole;
        public SByte Acceleration;
        public SByte Aggression;
        public SByte Agility;
        public SByte Anticipation;
        public SByte Balance;
        public SByte Bravery;
        public SByte Consistency;
        public SByte Corners;
        public SByte Crossing;
        public SByte Decisions;
        public SByte Dirtiness;
        public SByte Dribbling;
        public SByte Finishing;
        public SByte Flair;
        public SByte SetPieces;
        public SByte Handling;
        public SByte Heading;
        public SByte ImportantMatches;
        public SByte InjuryProneness;
        public SByte Jumping;
        public SByte Influence;
        public SByte LeftFoot;
        public SByte LongShots;
        public SByte Marking;
        public SByte OffTheBall;
        public SByte NaturalFitness;
        public SByte OneOnOnes;
        public SByte Pace;
        public SByte Passing;
        public SByte Penalties;
        public SByte Positioning;
        public SByte Reflexes;
        public SByte RightFoot;
        public SByte Stamina;
        public SByte Strength;
        public SByte Tackling;
        public SByte Teamwork;
        public SByte Technique;
        public SByte ThrowIns;
        public SByte Versatility;
        public SByte Creativity;
        public SByte WorkRate;
        public SByte Morale;
    }

    class CMStaff {
        public Int32      Id;
        public Int32      FirstNameId;
        public Int32      SecondNameId;
        public Int32      CommonNameId;
        public DateTime?  DateOfBirth;
        public Int16      YearOfBirth;
        public Int32      FirstNationId;
        public Int32      SecondNationId;
        public Byte       IntApps;
        public Byte       IntGoals;
        public Int32      NationalJobId;
        public SByte      JobForNation;
        public DateTime?  DateJoinedNation;
        public DateTime?  DateExpiresNation;
        public Int32      ClubJobId;
        public SByte      JobForClub;
        public DateTime?  DateJoinedClub;
        public DateTime?  DateExpiresClub;
        public Int32      Wage;
        public Int32      Value;
        public SByte      Adaptability;
        public SByte      Ambition;
        public SByte      Determination;
        public SByte      Loyality;
        public SByte      Pressure;
        public SByte      Professionalism;
        public SByte      Sportsmanship;
        public SByte      Temperament;
        public SByte      PlayingSquad;
        public SByte      Classification;
        public SByte      ClubValuation;
        public Int32      PlayerId;
        public Int32      StaffPreferencesId;
        public Int32      NonPlayerId;
        public SByte      SquadSelectedFor;

        public CMName     FirstName;
        public CMName     SecondName;
        public CMName     CommonName;
        public CMNation   FirstNation;
        public CMNation   SecondNation;
        public CMClub     ClubJob;
        public CMPlayer   Player;
        public Int32?     Age;
        public CMContract Contract;
        public CMContract LoanContract;

        public AttributeValue[] AttributeValues;

        public Double[]  Ratings;
    }

    enum CA18ViewMode {
        Intrinsic,
        IntrinsicGraemeKelly,
        IntrinsicNormalized,
        InGame
    }

    class Filter {
        public Boolean   IsLast;
        public String    Name;
        public String    PlayerName;
        public String    NationGroupName;
        public String    ClubName;
        public String    DivisionName;
        public Int32     AgeFrom;
        public Int32     AgeTo;
        public Int32     CAFrom;
        public Int32     CATo;
        public Int32     PAFrom;
        public Int32     PATo;
        public Int32     ValueFrom;
        public Int32     ValueTo;
        public Int32     WageFrom;
        public Int32     WageTo;
        public Boolean   IsFreeTransfer;
        public Boolean   IsContractExpired;
        public Boolean   IsContractExpiring;
        public Boolean   IsContractUnprotected;
        public Boolean?  IsLeavingOnBosman;
        public Boolean?  IsTransferArranged;
        public Boolean   IsTransferListedByClub;
        public Boolean   IsTransferListedByRequest;
        public Boolean   IsListedForLoan;
        public Boolean[] Sides;
        public Boolean[] Positions;
        public SByte[]   Attributes;

        public Filter() {
            CAFrom =   1;
            CATo   = 200;

            PAFrom =   1;
            PATo   = 200;

            // Sides
            Sides = new Boolean[DataService.Sides.Length];

            for(Int32 i = 0; i < Sides.Length; ++i) {
                Sides[i] = false;
            }

            // Positions
            Positions = new Boolean[DataService.Positions.Length];

            for(Int32 i = 0; i < Positions.Length; ++i) {
                Positions[i] = false;
            }

            // Attributes
            Attributes = new SByte[DataService.Attributes.Length];

            for(Int32 i = 0; i < Attributes.Length; ++i) {
                Attributes[i] = 1;
            }
        }
    }

    // https://en.wikipedia.org/wiki/Defender_(association_football)

    // Creativity   = Vision
    // Off The Ball = Movement 

    class WeightsSet {
        public Boolean  IsLast;
        public String   Name;
        public Byte[][] Weights;

        public WeightsSet() {
            Weights = new Byte[DataService.RatingPositions.Length][];

            for(Int32 i = 0; i < DataService.RatingPositions.Length; ++i) {
                Weights[i] = new Byte[DataService.Attributes.Length];
            }
        }
    }

    class DataService {

        public static String EUNationGroupName = "** EU Nation **";
        public static String EUNationGroupCode = "EUN";

        public static String NoClubName        = "** No Club **";

        public static Side[] Sides = {
            new Side("Left",   "L", staff => IsLeftSide(staff)   ),
            new Side("Centre", "C", staff => IsCentreSide(staff) ),
            new Side("Right",  "R", staff => IsRightSide(staff)  ),
        };

        public static Position[] Positions = {
            new Position("Goalkeeper",           "GK", staff => IsGoalkeeper(staff)          ),
            new Position("Sweeper",              "SW", staff => IsSweeper(staff)             ),
            new Position("Defender",             "D",  staff => IsDefender(staff)            ),
            new Position("Defensive Midfielder", "DM", staff => IsDefensiveMidfielder(staff) ),
            new Position("Midfielder",           "M",  staff => IsMidfielder(staff)          ),
            new Position("Attacking Midfielder", "AM", staff => IsAttackingMidfielder(staff) ),
            new Position("Forward",              "F",  staff => IsForward(staff)             ),
            new Position("Striker",              "S",  staff => IsStriker(staff)             ),
        };

        public static RatingPosition[] RatingPositions = {
            new RatingPosition("Goalkeeper",           "GK", staff => IsGoalkeeper(staff)                   ),
            new RatingPosition("Defender",             "D",  staff => IsSweeper(staff) || IsDefender(staff) ),
            new RatingPosition("Defensive Midfielder", "DM", staff => IsDefensiveMidfielder(staff)          ),
            new RatingPosition("Midfielder",           "M",  staff => IsMidfielder(staff)                   ),
            new RatingPosition("Attacking Midfielder", "AM", staff => IsAttackingMidfielder(staff)          ),
            new RatingPosition("Attacker",             "A",  staff => IsForward(staff) || IsStriker(staff)  ),
            new RatingPosition("Wing Back",            "WB", staff => IsWingBack(staff)                     ),
        };

        public static Attribute[] Attributes = {
            new Attribute("Anticipation",      "Ant", true,  false, staff => staff.Player.Anticipation    ), //  0
            new Attribute("Creativity",        "Cre", true,  false, staff => staff.Player.Creativity      ), //  1
            new Attribute("Crossing",          "Cro", true,  false, staff => staff.Player.Crossing        ), //  2
            new Attribute("Decisions",         "Dec", true,  false, staff => staff.Player.Decisions       ), //  3
            new Attribute("Dribbling",         "Dri", true,  false, staff => staff.Player.Dribbling       ), //  4
            new Attribute("Finishing",         "Fin", true,  false, staff => staff.Player.Finishing       ), //  5
            new Attribute("Heading",           "Hea", true,  false, staff => staff.Player.Heading         ), //  6
            new Attribute("Long Shots",        "Lon", true,  false, staff => staff.Player.LongShots       ), //  7
            new Attribute("Marking",           "Mar", true,  false, staff => staff.Player.Marking         ), //  8
            new Attribute("Off The Ball",      "Otb", true,  false, staff => staff.Player.OffTheBall      ), //  9
            new Attribute("Passing",           "Pas", true,  false, staff => staff.Player.Passing         ), // 10
            new Attribute("Penalties",         "Pen", true,  false, staff => staff.Player.Penalties       ), // 11
            new Attribute("Positioning",       "Pos", true,  false, staff => staff.Player.Positioning     ), // 12
            new Attribute("Tackling",          "Tac", true,  false, staff => staff.Player.Tackling        ), // 13
            new Attribute("Throw Ins",         "Thr", true,  false, staff => staff.Player.ThrowIns        ), // 14
            new Attribute("Handling",          "Han", true,  false, staff => staff.Player.Handling        ), // 15
            new Attribute("One On Ones",       "Ooo", true,  false, staff => staff.Player.OneOnOnes       ), // 16
            new Attribute("Reflexes",          "Ref", true,  false, staff => staff.Player.Reflexes        ), // 17

            new Attribute("Acceleration",      "Acc", false, false, staff => staff.Player.Acceleration    ), // 18
            new Attribute("Agility",           "Agi", false, false, staff => staff.Player.Agility         ), // 19
            new Attribute("Balance",           "Bal", false, false, staff => staff.Player.Balance         ), // 20
            new Attribute("Corners",           "Cor", false, false, staff => staff.Player.Corners         ), // 21
            new Attribute("Flair",             "Fla", false, false, staff => staff.Player.Flair           ), // 22
            new Attribute("Injury Proneness",  "Inj", false, true , staff => staff.Player.InjuryProneness ), // 23
            new Attribute("Jumping",           "Jum", false, false, staff => staff.Player.Jumping         ), // 24
            new Attribute("Natural Fitness",   "Nat", false, false, staff => staff.Player.NaturalFitness  ), // 25
            new Attribute("Pace",              "Pac", false, false, staff => staff.Player.Pace            ), // 26
            new Attribute("Set Pieces",        "Set", false, false, staff => staff.Player.SetPieces       ), // 27
            new Attribute("Stamina",           "Sta", false, false, staff => staff.Player.Stamina         ), // 28
            new Attribute("Strength",          "Str", false, false, staff => staff.Player.Strength        ), // 29
            new Attribute("Technique",         "Tec", false, false, staff => staff.Player.Technique       ), // 30
            new Attribute("Work Rate",         "Wor", false, false, staff => staff.Player.WorkRate        ), // 31

            new Attribute("Adaptability",      "Ada", false, false, staff => staff.Adaptability           ), // 32
            new Attribute("Aggression",        "Agg", false, false, staff => staff.Player.Aggression      ), // 33
            new Attribute("Ambition",          "Amb", false, false, staff => staff.Ambition               ), // 34
            new Attribute("Bravery",           "Bra", false, false, staff => staff.Player.Bravery         ), // 35
            new Attribute("Consistency",       "Con", false, false, staff => staff.Player.Consistency     ), // 36
            new Attribute("Determination",     "Det", false, false, staff => staff.Determination          ), // 37
            new Attribute("Dirtiness",         "Dir", false, true , staff => staff.Player.Dirtiness       ), // 38
            new Attribute("Important Matches", "Imp", false, false, staff => staff.Player.ImportantMatches), // 39
            new Attribute("Influence",         "Inf", false, false, staff => staff.Player.Influence       ), // 40
            new Attribute("Loyality",          "Loy", false, false, staff => staff.Loyality               ), // 41
            new Attribute("Pressure",          "Pre", false, false, staff => staff.Pressure               ), // 42
            new Attribute("Professionalism",   "Pro", false, false, staff => staff.Professionalism        ), // 43
            new Attribute("Sportsmanship",     "Spo", false, false, staff => staff.Sportsmanship          ), // 44
            new Attribute("Teamwork",          "Tea", false, false, staff => staff.Player.Teamwork        ), // 45
            new Attribute("Temperament",       "Tem", false, false, staff => staff.Temperament            ), // 46
            new Attribute("Versatility",       "Ver", false, false, staff => staff.Player.Versatility     ), // 47
        };

        public static ContractType[] ContractTypes = {
            new ContractType( 0, "** Any Contract Type **",    type => false),
            new ContractType( 1, "Invalid",                    type => type == 0x00 || type == 0x40 || type == 0x80 || type == 0xC0),
            new ContractType( 2, "Full Time Monthly Contract", type => type == 0x01 || type == 0x41),
            new ContractType( 3, "Full Time Contract",         type => type == 0x02 || type == 0x42 || (type >= 0x08 && type <= 0x3F) || (type >= 0x48 && type <= 0x7F)),
            new ContractType( 4, "N/A",                        type => type == 0x03 || type == 0x04 || type == 0x43 || type == 0x44 || type == 0x83 || type == 0x84 || type == 0xC3 || type == 0xC4),
            new ContractType( 5, "Full Time Trial Contract",   type => type == 0x05 || type == 0x45),
            new ContractType( 6, "Full Time Loan Contract",    type => type == 0x06 || type == 0x07 || type == 0x46 || type == 0x47),
            new ContractType( 7, "Part Time Monthly Contract", type => type == 0x81 || type == 0xC1),
            new ContractType( 8, "Part Time Contract",         type => type == 0x82 || type == 0xC2 || (type >= 0x88 && type <= 0xBF) || (type >= 0xC8 && type <= 0xFF)),
            new ContractType( 9, "Part Time Trial Contract",   type => type == 0x85 || type == 0xC5),
            new ContractType(10, "Part Time Loan Contract",    type => type == 0x86 || type == 0x87 || type == 0xC6 || type == 0xC7),
        };

        public static SquadStatus[] SquadStatuses = {
            new SquadStatus(0, "** Any Squad Status **",                         status => false),
            new SquadStatus(1, "Uncertain",                                      status => (status & 240) == 0),
            new SquadStatus(2, "This player is indispensable to the club",       status => (status & 224) == 0),
            new SquadStatus(3, "This player is important first team player",     status => (status & 208) == 0),
            new SquadStatus(4, "This player is used in a squad rotation system", status => (status & 192) == 0),
            new SquadStatus(5, "This player is backup for the first team",       status => (status & 176) == 0),
            new SquadStatus(6, "This player is hot prospect for the future",     status => (status & 160) == 0),
            new SquadStatus(7, "This player is decent young player",             status => (status & 144) == 0),
            new SquadStatus(8, "This player is not needed by the club",          status => (status & 128) == 0),
            new SquadStatus(9, "This player is on trial",                        status => (status & 112) == 0),
        };

        public static TransferStatus[] TransferStatuses = {
            new TransferStatus(0, "** Any Transfer Status **",  status => false),
            new TransferStatus(1, "Transfer listed by club",    status => (status & 1) == 1),
            new TransferStatus(2, "Transfer listed by request", status => (status & 8) == 8),
            new TransferStatus(3, "Listed for loan",            status => (status & 2) == 2),
            new TransferStatus(4, "Unknown",                    status => !((status & 1) == 1 || (status & 8) == 8 ||  (status & 2) == 2)),
        };

        public class LoadStartedEventArgs : EventArgs {
            public StorageFile File;
        }

        public class LoadFinishedEventArgs : EventArgs {
            public StorageFile File;
            public String      ErrorMessage;
        }

        public event Action<Object, LoadStartedEventArgs>  LoadStarted;
        public event Action<Object, LoadFinishedEventArgs> LoadFinished;

        public DataService() {
        }

        public async Task ExposeAsync() {
            await Task.FromResult(false);
        }

        public DateTime?        GameDate;
        public Int32?           MinAge;
        public Int32?           MaxAge;
        public List<CMNation>   Nations;
        public List<CMDivision> Divisions;
        public List<CMClub>     Clubs;
        public List<CMStaff>    Staffs;

        public Boolean IsLoaded() {
            return (GameDate != null);
        }

        public async Task LoadAsync(StorageFile file) {
            GameDate                     = null;
            MinAge                       = null;
            MaxAge                       = null;
            Nations                      = null;
            Divisions                    = null;
            Clubs                        = null;
            Staffs                       = null;
            _clubNameToClubIdMap         = null;
            _divisionNameToDivisionIdMap = null;
            _squadIdToClubIdsMap         = null;
            _ca18AttributeValueRanges    = null;

            String errorMessage = null;

            LoadStarted?.Invoke(this, new LoadStartedEventArgs { File = file });

            try {
                await Task.Run(
                    async () => {
                        DateTime?                        gameDate                    = null;
                        Int32?                           minAge                      = null;
                        Int32?                           maxAge                      = null;
                        List<CMNation>                   nations                     = null;
                        List<CMDivision>                 divisions                   = null;
                        List<CMClub>                     clubs                       = null;
                        List<CMStaff>                    staffs                      = null;
                        Dictionary<String, Int32>        clubNameToClubIdMap         = null;
                        Dictionary<String, Int32>        divisionNameToDivisionIdMap = null;
                        Dictionary< Int32, List<Int32> > squadIdToClubIdsMap         = null;
                        Pair<SByte, SByte>[]             ca18AttributeValueRanges    = null;

                        using (Stream stream = await file.OpenStreamForReadAsync()) {
                            using(BinaryReader br = new BinaryReader(stream)) {
                                Boolean isCompressed = (br.ReadInt32() == 4);

                                Debug.WriteLine("isCompressed: {0}", isCompressed);

                                // Skip 4 bytes
                                br.ReadBytes(4);

                                Int32 blocksCount = br.ReadInt32();

                                Debug.WriteLine("blocksCount: {0}", blocksCount);

                                List<CMBlock> blocks = new List<CMBlock>();

                                for(Int32 i = 0; i < blocksCount; ++i) {
                                    CMBlock block = new CMBlock {
                                        Position = br.ReadInt32(),
                                        Size     = br.ReadInt32(),
                                        Name     = ReadCMString(br, 260),
                                    };

                                    blocks.Add(block);

                                    //Debug.WriteLine("{0,9} {1,9} {2}", block.Position, block.Size, block.Name);
                                }

                                // general.dat
                                {
                                    CMBlock block = blocks.Find(item => item.Name == "general.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    br.ReadBytes(3944);

                                    gameDate = ReadDateTime(br);
                                }

                                Debug.WriteLine("gameDate: {0:dd-MM-yyyy}", gameDate, 0);

                                // nation.dat
                                nations = new List<CMNation>();

                                {
                                    CMBlock block = blocks.Find(item => item.Name == "nation.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 nationsCount = block.Size / 290;

                                    for(Int32 i = 0; i < nationsCount; ++i) {
                                        CMNation nation = new CMNation {
                                            Id                 = br.ReadInt32(),
                                            Name               = ReadCMString(br, 51),
                                            GenderName         = br.ReadSByte(),
                                            ShortName          = ReadCMString(br, 26),
                                            GenderShortName    = br.ReadSByte(),
                                            Code               = ReadCMString(br, 4),
                                            Nationality        = ReadCMString(br, 26),
                                            ContinentId        = br.ReadInt32(),
                                            Region             = br.ReadSByte(),
                                            ActualRegion       = br.ReadSByte(),
                                            FirstLanguage      = br.ReadSByte(),
                                            SecondLanguage     = br.ReadSByte(),
                                            ThirdLanguage      = br.ReadSByte(),
                                            CapitalId          = br.ReadInt32(),
                                            StateOfDevelopment = br.ReadSByte(),
                                            GroupMembership    = br.ReadSByte(),
                                            NationalStadiumId  = br.ReadInt32(),
                                            GameImportance     = br.ReadSByte(),
                                            LeagueStandard     = br.ReadSByte(),
                                            NumberClubs        = br.ReadInt16(),
                                            NumberStaff        = br.ReadInt32(),
                                            UpdateDay          = br.ReadInt16(),
                                            Reputation         = br.ReadInt16(),
                                            ForeColour1Id      = br.ReadInt32(),
                                            BackColour1Id      = br.ReadInt32(),
                                            ForeColour2Id      = br.ReadInt32(),
                                            BackColour2Id      = br.ReadInt32(),
                                            ForeColour3Id      = br.ReadInt32(),
                                            BackColour3Id      = br.ReadInt32(),
                                            FIFACoefficient    = br.ReadDouble(),
                                            FIFACoefficient91  = br.ReadDouble(),
                                            FIFACoefficient92  = br.ReadDouble(),
                                            FIFACoefficient93  = br.ReadDouble(),
                                            FIFACoefficient94  = br.ReadDouble(),
                                            FIFACoefficient95  = br.ReadDouble(),
                                            FIFACoefficient96  = br.ReadDouble(),
                                            UEFACoefficient91  = br.ReadDouble(),
                                            UEFACoefficient92  = br.ReadDouble(),
                                            UEFACoefficient93  = br.ReadDouble(),
                                            UEFACoefficient94  = br.ReadDouble(),
                                            UEFACoefficient95  = br.ReadDouble(),
                                            UEFACoefficient96  = br.ReadDouble(),
                                            RivalNation1Id     = br.ReadInt32(),
                                            RivalNation2Id     = br.ReadInt32(),
                                            RivalNation3Id     = br.ReadInt32(),
                                            LeagueSelected     = br.ReadSByte(),
                                            ShortlistOffset    = br.ReadInt32(),
                                            GamesPlayed        = br.ReadSByte(),
                                        };

                                        nations.Add(nation);

                                        //Debug.WriteLine("Nation.  {0,3}  {1}  {2,-20}  {3}", nation.Id, nation.Code, nation.ShortName, nation.Name);
                                    }
                                }

                                Debug.WriteLine("nations.Count: {0}", nations.Count);

                                // club_comp.dat
                                divisions                   = new List<CMDivision>();
                                divisionNameToDivisionIdMap = new Dictionary<String, Int32>();

                                {
                                    CMBlock block = blocks.Find(item => item.Name == "club_comp.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 divisionsCount = block.Size / 107;

                                    for(Int32 i = 0; i < divisionsCount; ++i) {
                                        CMDivision division = new CMDivision {
                                            Id                 = br.ReadInt32(),
                                            Name               = ReadCMString(br, 51),
                                            GenderName         = br.ReadSByte(),
                                            ShortName          = ReadCMString(br, 26),
                                            GenderShortName    = br.ReadSByte(),
                                            Code               = ReadCMString(br, 4),
                                            Scope              = br.ReadSByte(),
                                            Selected           = br.ReadSByte(),
                                            ContinentId        = br.ReadInt32(),
                                            NationId           = br.ReadInt32(),
                                            ForeColourId       = br.ReadInt32(),
                                            BackColourId       = br.ReadInt32(),
                                            Reputation         = br.ReadInt16(),
                                        };

                                        division.Nation = GetNationFromId(division.NationId, nations);

                                        divisions.Add(division);

                                        //Debug.WriteLine("Division.  {0,3}  {1}", division.Id, division.Name);

                                        if(divisionNameToDivisionIdMap.ContainsKey(division.Name)) {
                                            Debug.WriteLine("Dublicate division \"{0}\" (\"{1}\", \"{2}\")", division.Name, division.ShortName, GetDivisionFromId(divisionNameToDivisionIdMap[division.Name], divisions).ShortName);
                                        }
                                        else {
                                            divisionNameToDivisionIdMap.Add(division.Name, division.Id);
                                        }
                                    }
                                }

                                Debug.WriteLine("divisions.Count: {0}", divisions.Count);

                                // club.dat
                                clubs               = new List<CMClub>();
                                clubNameToClubIdMap = new Dictionary<String, Int32>();
                                squadIdToClubIdsMap = new Dictionary< Int32, List<Int32> >();

                                {
                                    CMBlock block = blocks.Find(item => item.Name == "club.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 clubsCount = block.Size / 581;

                                    for(Int32 i = 0; i < clubsCount; ++i) {
                                        CMClub club = new CMClub {
                                            Id                 = br.ReadInt32(),
                                            Name               = ReadCMString(br, 51),
                                            GenderName         = br.ReadSByte(),
                                            ShortName          = ReadCMString(br, 26),
                                            GenderShortName    = br.ReadSByte(),
                                            NationId           = br.ReadInt32(),
                                            DivisionId         = br.ReadInt32(),
                                            LastDivisionId     = br.ReadInt32(),
                                            LastPosition       = br.ReadSByte(),
                                            ReserveDivisionId  = br.ReadInt32(),
                                            ProfessionalStatus = br.ReadSByte(),
                                            Cash               = br.ReadInt32(),
                                            StadiumId          = br.ReadInt32(),
                                            OwnStadium         = br.ReadSByte(),
                                            ReserveStadiumId   = br.ReadInt32(),
                                            MatchDay           = br.ReadSByte(),
                                            Attendance         = br.ReadInt32(),
                                            MinAttendance      = br.ReadInt32(),
                                            MaxAttendance      = br.ReadInt32(),
                                            Training           = br.ReadSByte(),
                                            Reputation         = br.ReadInt16(),
                                            PLC                = br.ReadSByte(),
                                            ForeColour1Id      = br.ReadInt32(),
                                            BackColour1Id      = br.ReadInt32(),
                                            ForeColour2Id      = br.ReadInt32(),
                                            BackColour2Id      = br.ReadInt32(),
                                            ForeColour3Id      = br.ReadInt32(),
                                            BackColour3Id      = br.ReadInt32(),
                                            FavStaff1Id        = br.ReadInt32(),
                                            FavStaff2Id        = br.ReadInt32(),
                                            FavStaff3Id        = br.ReadInt32(),
                                            DisStaff1Id        = br.ReadInt32(),
                                            DisStaff2Id        = br.ReadInt32(),
                                            DisStaff3Id        = br.ReadInt32(),
                                            Rival1Id           = br.ReadInt32(),
                                            Rival2Id           = br.ReadInt32(),
                                            Rival3Id           = br.ReadInt32(),
                                            ChairmanId         = br.ReadInt32(),
                                            DirectorsIds       = ReadIds(br, 3),
                                            ManagerId          = br.ReadInt32(),
                                            AssistantManagerId = br.ReadInt32(),
                                            SquadIds           = ReadIds(br, 50),
                                            CoachIds           = ReadIds(br, 5),
                                            ScoutIds           = ReadIds(br, 7),
                                            PhysioIds          = ReadIds(br, 3),
                                            EuroFlag           = br.ReadInt32(),
                                            EuroSeeding        = br.ReadSByte(),
                                            TeamSelectedIds    = ReadIds(br, 20),
                                            TacticTrainingIds  = ReadIds(br, 4),
                                            TacticSelected     = br.ReadInt32(),
                                            HasLinkedClub      = br.ReadSByte(),
                                        };

                                        clubs.Add(club);

                                        //Debug.WriteLine("Club. {0,6} {1}", club.Id, club.Name);

                                        if(clubNameToClubIdMap.ContainsKey(club.Name)) {
                                            Debug.WriteLine("Dublicate club \"{0}\" (\"{1}\", \"{2}\")", club.Name, club.ShortName, GetClubFromId(clubNameToClubIdMap[club.Name], clubs).ShortName);
                                        }
                                        else {
                                            clubNameToClubIdMap.Add(club.Name, club.Id);
                                        }

                                        foreach(Int32 squadId in club.SquadIds) {
                                            if(squadId >= 0) {
                                                if(squadIdToClubIdsMap.TryGetValue(squadId, out List<Int32> clubIds)) {
                                                    clubIds.Add(club.Id);
                                                }
                                                else {
                                                    squadIdToClubIdsMap.Add(squadId, new List<Int32> { club.Id, });
                                                }
                                            }
                                        }

                                        //if(club.Name == "River Plate") {
                                        //    Debug.WriteLine("River Plate. i: {0}.", i);
                                        //}
                                    }
                                }

                                Debug.WriteLine("clubs.Count: {0}", clubs.Count);

                                // names
                                Func<BinaryReader, String, List<CMName> > loadNames =
                                    (br_, blockName) => {
                                        List<CMName> names = new List<CMName>();

                                        CMBlock block = blocks.Find(item => item.Name == blockName);

                                        br_.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                        Int32 namesCount = block.Size / 60;

                                        for(Int32 i = 0; i < namesCount; ++i) {
                                            CMName name = new CMName {
                                                Name     = ReadCMString(br_, 51),
                                                Id       = br_.ReadInt32(),
                                                NationId = br_.ReadInt32(),
                                                Count    = br_.ReadSByte(),
                                            };

                                            names.Add(name);
                                        }

                                        return names;
                                    };

                                List<CMName> firstNames  = loadNames(br, "first_names.dat");
                                List<CMName> secondNames = loadNames(br, "second_names.dat");
                                List<CMName> commonNames = loadNames(br, "common_names.dat");

                                Debug.WriteLine("firstNames.Count:  {0}", firstNames.Count);
                                Debug.WriteLine("secondNames.Count: {0}", secondNames.Count);
                                Debug.WriteLine("commonNames.Count: {0}", commonNames.Count);

                                // player.dat
                                List<CMPlayer> players = new List<CMPlayer>();

                                {
                                    CMBlock block = blocks.Find(item => item.Name == "player.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 playersCount = block.Size / 70;

                                    for(Int32 i = 0; i < playersCount; ++i) {
                                        CMPlayer player = new CMPlayer {
                                            Id                  = br.ReadInt32(),
                                            SquardNumber        = br.ReadByte(),
                                            CurrentAbility      = br.ReadInt16(),
                                            PotentialAbility    = br.ReadInt16(),
                                            HomeReputation      = br.ReadInt16(),
                                            CurrentReputation   = br.ReadInt16(),
                                            WorldReputation     = br.ReadInt16(),
                                            Goalkeeper          = br.ReadSByte(),
                                            Sweeper             = br.ReadSByte(),
                                            Defender            = br.ReadSByte(),
                                            DefensiveMidfielder = br.ReadSByte(),
                                            Midfielder          = br.ReadSByte(),
                                            AttackingMidfielder = br.ReadSByte(),
                                            Attacker            = br.ReadSByte(),
                                            WingBack            = br.ReadSByte(),
                                            RightSide           = br.ReadSByte(),
                                            LeftSide            = br.ReadSByte(),
                                            CentreSide          = br.ReadSByte(),
                                            FreeRole            = br.ReadSByte(),
                                            Acceleration        = br.ReadSByte(),
                                            Aggression          = br.ReadSByte(),
                                            Agility             = br.ReadSByte(),
                                            Anticipation        = br.ReadSByte(),
                                            Balance             = br.ReadSByte(),
                                            Bravery             = br.ReadSByte(),
                                            Consistency         = br.ReadSByte(),
                                            Corners             = br.ReadSByte(),
                                            Crossing            = br.ReadSByte(),
                                            Decisions           = br.ReadSByte(),
                                            Dirtiness           = br.ReadSByte(),
                                            Dribbling           = br.ReadSByte(),
                                            Finishing           = br.ReadSByte(),
                                            Flair               = br.ReadSByte(),
                                            SetPieces           = br.ReadSByte(),
                                            Handling            = br.ReadSByte(),
                                            Heading             = br.ReadSByte(),
                                            ImportantMatches    = br.ReadSByte(),
                                            InjuryProneness     = br.ReadSByte(),
                                            Jumping             = br.ReadSByte(),
                                            Influence           = br.ReadSByte(),
                                            LeftFoot            = br.ReadSByte(),
                                            LongShots           = br.ReadSByte(),
                                            Marking             = br.ReadSByte(),
                                            OffTheBall          = br.ReadSByte(),
                                            NaturalFitness      = br.ReadSByte(),
                                            OneOnOnes           = br.ReadSByte(),
                                            Pace                = br.ReadSByte(),
                                            Passing             = br.ReadSByte(),
                                            Penalties           = br.ReadSByte(),
                                            Positioning         = br.ReadSByte(),
                                            Reflexes            = br.ReadSByte(),
                                            RightFoot           = br.ReadSByte(),
                                            Stamina             = br.ReadSByte(),
                                            Strength            = br.ReadSByte(),
                                            Tackling            = br.ReadSByte(),
                                            Teamwork            = br.ReadSByte(),
                                            Technique           = br.ReadSByte(),
                                            ThrowIns            = br.ReadSByte(),
                                            Versatility         = br.ReadSByte(),
                                            Creativity          = br.ReadSByte(),
                                            WorkRate            = br.ReadSByte(),
                                            Morale              = br.ReadSByte(),
                                        };

                                        players.Add(player);
                                    }
                                }

                                Debug.WriteLine("players.Count: {0}", players.Count);

                                // staff.dat
                                staffs = new List<CMStaff>();

                                ca18AttributeValueRanges = new Pair<SByte, SByte>[ Attributes.Count(item => item.IsCA18) ];

                                for(Int32 j = 0; j < ca18AttributeValueRanges.Length; ++j) {
                                    ca18AttributeValueRanges[j] = new Pair<SByte, SByte>(SByte.MaxValue, SByte.MinValue);
                                }

                                {
                                    CMBlock block = blocks.Find(item => item.Name == "staff.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 staffsCount = block.Size / 110;

                                    for(Int32 i = 0; i < staffsCount; ++i) {
                                        CMStaff staff = new CMStaff {
                                            Id                 = br.ReadInt32(),
                                            FirstNameId        = br.ReadInt32(),
                                            SecondNameId       = br.ReadInt32(),
                                            CommonNameId       = br.ReadInt32(),
                                            DateOfBirth        = ReadDateTime(br),
                                            YearOfBirth        = br.ReadInt16(),
                                            FirstNationId      = br.ReadInt32(),
                                            SecondNationId     = br.ReadInt32(),
                                            IntApps            = br.ReadByte(),
                                            IntGoals           = br.ReadByte(),
                                            NationalJobId      = br.ReadInt32(),
                                            JobForNation       = br.ReadSByte(),
                                            DateJoinedNation   = ReadDateTime(br),
                                            DateExpiresNation  = ReadDateTime(br),
                                            ClubJobId          = br.ReadInt32(),
                                            JobForClub         = br.ReadSByte(),
                                            DateJoinedClub     = ReadDateTime(br),
                                            DateExpiresClub    = ReadDateTime(br),
                                            Wage               = br.ReadInt32(),
                                            Value              = br.ReadInt32(),
                                            Adaptability       = br.ReadSByte(),
                                            Ambition           = br.ReadSByte(),
                                            Determination      = br.ReadSByte(),
                                            Loyality           = br.ReadSByte(),
                                            Pressure           = br.ReadSByte(),
                                            Professionalism    = br.ReadSByte(),
                                            Sportsmanship      = br.ReadSByte(),
                                            Temperament        = br.ReadSByte(),
                                            PlayingSquad       = br.ReadSByte(),
                                            Classification     = br.ReadSByte(),
                                            ClubValuation      = br.ReadSByte(),
                                            PlayerId           = br.ReadInt32(),
                                            StaffPreferencesId = br.ReadInt32(),
                                            NonPlayerId        = br.ReadInt32(),
                                            SquadSelectedFor   = br.ReadSByte(),
                                        };

                                        staff.FirstName       = GetNameFromId(staff.FirstNameId, firstNames);
                                        staff.SecondName      = GetNameFromId(staff.SecondNameId, secondNames);
                                        staff.CommonName      = GetNameFromId(staff.CommonNameId, commonNames);
                                        staff.FirstNation     = GetNationFromId(staff.FirstNationId, nations);
                                        staff.SecondNation    = (staff.SecondNationId != staff.FirstNationId ? GetNationFromId(staff.SecondNationId, nations) : null);
                                        staff.ClubJob         = GetClubFromId(staff.ClubJobId, clubs);
                                        staff.Player          = GetPlayerFromId(staff.PlayerId, players);
                                        staff.Age             = (staff.DateOfBirth != null ? DiffInYears(staff.DateOfBirth.Value, gameDate.Value) : (Int32?)null);

                                        staffs.Add(staff);

                                        //if(staff.FirstName.Name == "Vladimir" && staff.SecondName.Name == "Filatov") {
                                        //    Debug.WriteLine(
                                        //        "Vladimir Filatov. i: {0}. NonPlayerId: {1}.",
                                        //        i,
                                        //        staff.NonPlayerId
                                        //    );
                                        //}

                                        if(IsValidPlayer(staff)) {
                                            if(staff.Age != null) {
                                                if(minAge == null || minAge > staff.Age) {
                                                    minAge = staff.Age;
                                                }

                                                if(maxAge == null || maxAge < staff.Age) {
                                                    maxAge = staff.Age;
                                                }
                                            }

                                            Int32 j = 0;

                                            foreach(Attribute a in Attributes) {
                                                if(a.IsCA18) {
                                                    Pair<SByte, SByte> p = ca18AttributeValueRanges[j];
                                                    SByte              v = a.IntrinsicValueExtractor(staff);

                                                    if(p.First  > v) { p.First  = v; }
                                                    if(p.Second < v) { p.Second = v; }

                                                    ++j;
                                                }
                                            }
                                        }
                                    }
                                }

                                Debug.WriteLine("staffs.Count: {0}", staffs.Count);

                                List<SByte>[] attributesValues = new List<SByte>[Attributes.Length];

                                for(Int32 i = 0; i < Attributes.Length; ++i) {
                                    attributesValues[i] = new List<SByte>();
                                }

                                Int32[][] attributesDistribution = new Int32[Attributes.Length][];

                                for(Int32 i = 0; i < Attributes.Length; ++i) {
                                    attributesDistribution[i] = new Int32[20];
                                }

                                Int32 validPlayersCount = 0;

                                foreach(CMStaff staff in staffs) {
                                    if(IsValidPlayer(staff)) {
                                        ++validPlayersCount;

                                        staff.AttributeValues = new AttributeValue[Attributes.Length];

                                        Int32 i = 0;
                                        Int32 j = 0;

                                        foreach(Attribute a in Attributes) {
                                            SByte intrinsic            = a.IntrinsicValueExtractor(staff);
                                            Int16 intrinsicGraemeKelly = 0;
                                            SByte intrinsicNormalized  = 0;
                                            SByte inGame               = 0;

                                            if(a.IsCA18) {
                                                {
                                                    intrinsicGraemeKelly = (Byte)intrinsic;
                                                }

                                                {
                                                    Int16 minValue = ca18AttributeValueRanges[j].First;
                                                    Int16 maxValue = ca18AttributeValueRanges[j].Second;

                                                    Double r = 20.0 * (intrinsic - minValue) / (maxValue - minValue) + 0.5;

                                                    if(r < 1) {
                                                        r = 1;
                                                    }
                                                    else if(r > 20) {
                                                        r = 20;
                                                    }

                                                    intrinsicNormalized = (SByte)Math.Truncate(r);

                                                    ++j;
                                                }

                                                {
                                                    if(i == 0 || i == 3 || i == 6 || i == 7 || i == 10 || i == 11 || i == 12 || i == 13) {
                                                        inGame = HighConvert(staff.Player.CurrentAbility, intrinsic);
                                                    }
                                                    else {
                                                        Boolean isGK = (staff.Player.Goalkeeper > 14);

                                                        if(i == 15 || i == 16 || i == 17) {
                                                            if(isGK) {
                                                                inGame = HighConvert(staff.Player.CurrentAbility, intrinsic);
                                                            }
                                                            else {
                                                                inGame = LowConvert(staff.Player.CurrentAbility, intrinsic);
                                                            }
                                                        }
                                                        else if(i == 1 || i == 2 || i == 4 || i == 5 || i == 8 || i == 9 || i == 14) {
                                                            if(isGK) {
                                                                inGame = LowConvert(staff.Player.CurrentAbility, intrinsic);
                                                            }
                                                            else {
                                                                inGame = HighConvert(staff.Player.CurrentAbility, intrinsic);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                intrinsicGraemeKelly = intrinsic;
                                                intrinsicNormalized  = intrinsic;
                                                inGame               = intrinsic;
                                            }

                                            staff.AttributeValues[i] = new AttributeValue(intrinsic, intrinsicGraemeKelly, intrinsicNormalized, inGame);

                                            Boolean isGKAttr_ = (i == 15 || i == 16 || i == 17);
                                            Boolean isGK_     = (staff.Player.Goalkeeper > 14);

                                            if(!isGKAttr_ || isGK_) {
                                                attributesValues[i].Add(intrinsic);
                                            }

                                            ++(attributesDistribution[i][intrinsicNormalized - 1]);

                                            ++i;
                                        }

                                        staff.Ratings = new Double[RatingPositions.Length];
                                    }
                                }

                                Func<Int32, Int32, Int32, SByte> toIntrinsicNormalized2 = (i1, i2, c) => {
                                    Double r = 20.0 * (i1 + i2 - 1) / 2 / (c - 1) + 0.5;

                                    if(r < 1) {
                                        r = 1;
                                    }
                                    else if(r > 20) {
                                        r = 20;
                                    }

                                    return (SByte)Math.Truncate(r);
                                };

                                Dictionary<SByte, SByte>[] intrinsicNormalized2MapList = new Dictionary<SByte, SByte>[Attributes.Length];

                                for(Int32 i = 0; i < Attributes.Length; ++i) {
                                    List<SByte> attributeValues = attributesValues[i];

                                    attributeValues.Sort();

                                    Dictionary<SByte, SByte> intrinsicNormalized2Map = new Dictionary<SByte, SByte>();

                                    intrinsicNormalized2MapList[i] = intrinsicNormalized2Map;

                                    Int32 k = 0;

                                    for(Int32 j = 1; j < attributeValues.Count; ++j) {
                                        if(attributeValues[k] != attributeValues[j]) {
                                            intrinsicNormalized2Map[ attributeValues[k] ] = toIntrinsicNormalized2(k, j, attributeValues.Count);

                                            k = j;
                                        }
                                    }

                                    intrinsicNormalized2Map[ attributeValues[k] ] = toIntrinsicNormalized2(k, attributeValues.Count, attributeValues.Count);

                                    Debug.WriteLine("***** {0} *****", Attributes[i].Name, 0);

                                    foreach(KeyValuePair<SByte, SByte> p in intrinsicNormalized2Map) {
                                        Debug.WriteLine("{0,4} -> {1,2}", p.Key, p.Value);
                                    }

                                    Debug.WriteLine("");
                                }

                                //Debug.WriteLine("***** Attributes distribution *****");
                                //Debug.WriteLine("Name      1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20");
                                //Debug.WriteLine("---------------------------------------------------------------------------------------");

                                //for(Int32 i = 0; i < Attributes.Length; ++i) {
                                //    Attribute a  = Attributes[i];
                                //    Int32[]   ad = attributesDistribution[i];

                                //    Debug.WriteLine(
                                //        "{0}     {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3} {8,3} {9,3} {10,3} {11,3} {12,3} {13,3} {14,3} {15,3} {16,3} {17,3} {18,3} {19,3} {20,3}",
                                //        a.Code,
                                //        (Int32)Math.Round(100.0 * ad[ 0] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 1] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 2] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 3] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 4] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 5] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 6] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 7] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 8] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[ 9] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[10] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[11] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[12] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[13] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[14] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[15] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[16] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[17] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[18] / validPlayersCount),
                                //        (Int32)Math.Round(100.0 * ad[19] / validPlayersCount)
                                //    );
                                //}

                                // contract.dat
                                {
                                    CMBlock block = blocks.Find(item => item.Name == "contract.dat");

                                    br.BaseStream.Seek(block.Position, SeekOrigin.Begin);

                                    Int32 contractsPreCount = br.ReadInt32();
                                    Int32 contractsCount    = br.ReadInt32();

                                    Debug.WriteLine("contractsPreCount: {0}", contractsPreCount);
                                    Debug.WriteLine("contractsCount: {0}", contractsCount);

                                    Byte[] contractsPreBytes = null;

                                    for(Int32 i = 0; i < contractsPreCount; ++i) {
                                        contractsPreBytes = br.ReadBytes(21);
                                    }

                                    if(contractsPreCount > 0) {
                                        contractsCount = BitConverter.ToInt32(contractsPreBytes, 17);

                                        Debug.WriteLine("contractsCount: {0}", contractsCount);
                                    }

                                    for(Int32 i = 0; i < contractsCount; ++i) {
                                        CMContract contract = new CMContract {
                                            Id                        = br.ReadInt32(),
                                            ClubId                    = br.ReadInt32(),
                                            Unknown1                  = br.ReadBytes(4),
                                            Wage                      = br.ReadInt32(),
                                            GoalBonus                 = br.ReadInt32(),
                                            AssistBonus               = br.ReadInt32(),
                                            CleanSheetBonus           = br.ReadInt32(),
                                            NonPromotionRC            = br.ReadByte(),
                                            MinimumFeeRC              = br.ReadByte(),
                                            NonPlayingRC              = br.ReadByte(),
                                            RelegationRC              = br.ReadByte(),
                                            ManagerJobRC              = br.ReadByte(),
                                            ReleaseFee                = br.ReadInt32(),
                                            DateStarted               = ReadDateTime(br),
                                            DateEnded                 = ReadDateTime(br),
                                            Type                      = br.ReadByte(),
                                            Unknown2                  = br.ReadBytes(19),
                                            LeavingOnBosman           = (br.ReadByte() != 0),
                                            TransferArrangedForClubId = br.ReadInt32(),
                                            TransferStatus            = br.ReadByte(),
                                            SquadStatus               = br.ReadByte(),
                                        };

                                        if(contract.TransferArrangedForClubId >= 0 && contract.TransferArrangedForClubId >= clubs.Count) {
                                            contract.TransferArrangedForClubId = -1;
                                        }

                                        contract.Club                    = GetClubFromId(contract.ClubId, clubs);
                                        contract.TransferArrangedForClub = GetClubFromId(contract.TransferArrangedForClubId, clubs);
                                        contract.HasNonPromotionRC       = contract.NonPromotionRC > 0;
                                        contract.IsNonPromotionRCActive  = contract.NonPromotionRC > 1;
                                        contract.HasMinimumFeeRC         = contract.MinimumFeeRC   > 0;
                                        contract.IsMinimumFeeRCActive    = contract.MinimumFeeRC   > 1;
                                        contract.HasNonPlayingRC         = contract.NonPlayingRC   > 0;
                                        contract.IsNonPlayingRCActive    = contract.NonPlayingRC   > 1;
                                        contract.HasRelegationRC         = contract.RelegationRC   > 0;
                                        contract.IsRelegationRCActive    = contract.RelegationRC   > 1;
                                        contract.HasManagerJobRC         = contract.ManagerJobRC   > 0;
                                        contract.IsManagerJobRCActive    = contract.ManagerJobRC   > 1;

                                        if(contract.Id >= 0 && contract.Id < staffs.Count) {
                                            CMStaff staff = staffs[contract.Id];

                                            if(staff.Contract == null) {
                                                staff.Contract = contract;
                                            }
                                            else {
                                                if(staff.Contract.ClubId == staff.ClubJobId) {
                                                    staff.LoanContract = staff.Contract;
                                                    staff.Contract     = contract;
                                                }
                                                else {
                                                    staff.LoanContract = contract;
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach(CMStaff staff in staffs) {
                                    if(staff.Contract != null) {
                                        staff.Contract.UnprotectedReason = "Unknown";

                                        if(staff.Contract.DateStarted != null) {
                                            if(staff.Contract.DateStarted.Value.Year < 2001 || (staff.Contract.DateStarted.Value.Year == 2001 && staff.Contract.DateStarted.Value.DayOfYear < 244)) {
                                                staff.Contract.UnprotectedReason = null;
                                            }
                                            else {
                                                if(staff.DateOfBirth != null) {
                                                    Int32 d1 = DiffInYears(staff.DateOfBirth.Value, staff.Contract.DateStarted.Value);

                                                    if(d1 >= 28) {
                                                        if(gameDate.Value.Year - staff.Contract.DateStarted.Value.Year < 2 || (gameDate.Value.Year - staff.Contract.DateStarted.Value.Year == 2 && gameDate.Value.DayOfYear < staff.Contract.DateStarted.Value.DayOfYear)) {
                                                            staff.Contract.UnprotectedReason = null;
                                                        }
                                                        else {
                                                            staff.Contract.UnprotectedReason = "At least 2 years since start of contract";
                                                        }
                                                    }
                                                    else {
                                                        if(gameDate.Value.Year - staff.Contract.DateStarted.Value.Year < 3 || (gameDate.Value.Year - staff.Contract.DateStarted.Value.Year == 3 && gameDate.Value.DayOfYear < staff.Contract.DateStarted.Value.DayOfYear)) {
                                                            staff.Contract.UnprotectedReason = null;
                                                        }
                                                        else {
                                                            staff.Contract.UnprotectedReason = "At least 3 years since start of contract";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        GameDate                     = gameDate;
                        MinAge                       = minAge;
                        MaxAge                       = maxAge;
                        Nations                      = nations;
                        Divisions                    = divisions;
                        Clubs                        = clubs;
                        Staffs                       = staffs;
                        _divisionNameToDivisionIdMap = divisionNameToDivisionIdMap;
                        _clubNameToClubIdMap         = clubNameToClubIdMap;
                        _squadIdToClubIdsMap         = squadIdToClubIdsMap;
                        _ca18AttributeValueRanges    = ca18AttributeValueRanges;
                    }
                );
            }
            catch(Exception e) {
                errorMessage = e.Message;
            }

            LoadFinished?.Invoke(this, new LoadFinishedEventArgs { File = file, ErrorMessage = errorMessage });
        }

        public static Boolean IsValidPlayer(CMStaff staff) {
            return ((staff.Player != null)                            &&
                    (!String.IsNullOrEmpty(staff.FirstName?.Name)  ||
                     !String.IsNullOrEmpty(staff.SecondName?.Name) ||
                     !String.IsNullOrEmpty(staff.CommonName?.Name)  )  );
        }

        public static Boolean IsEUNation(CMStaff staff) {
            return (staff.FirstNation  != null && staff.FirstNation.GroupMembership  == 2) ||
                   (staff.SecondNation != null && staff.SecondNation.GroupMembership == 2   );
        }

        public static Boolean IsGoalkeeper(CMStaff staff) {
            return (staff.Player.Goalkeeper > 14);
        }

        public static Boolean IsSweeper(CMStaff staff) {
            return (staff.Player.Sweeper > 14);
        }

        public static Boolean IsDefender(CMStaff staff) {
            return (staff.Player.Defender > 14);
        }

        public static Boolean IsDefensiveMidfielder(CMStaff staff) {
            return (staff.Player.DefensiveMidfielder > 14);
        }

        public static Boolean IsMidfielder(CMStaff staff) {
            return (staff.Player.Midfielder > 14 && !IsDefensiveMidfielder(staff) && !IsAttackingMidfielder(staff));
        }

        public static Boolean IsAttackingMidfielder(CMStaff staff) {
            return ( (staff.Player.AttackingMidfielder > 14 || staff.Player.WingBack > 14) &&
                     !IsDefensiveMidfielder(staff)                                         &&
                     (staff.Player.Attacker <= 14 || staff.Player.Midfielder > 14)          );
        }

        public static Boolean IsForward(CMStaff staff) {
            return ( (staff.Player.Attacker > 14) &&
                     (staff.Player.AttackingMidfielder > 14 || staff.Player.LeftSide > 14 || staff.Player.RightSide > 14 || staff.Player.FreeRole > 14) );
        }

        public static Boolean IsStriker(CMStaff staff) {
            return (staff.Player.Attacker > 14 && !IsForward(staff));
        }

        public static Boolean IsWingBack(CMStaff staff) {
            return (staff.Player.Midfielder > 14 || staff.Player.AttackingMidfielder > 14 || staff.Player.WingBack > 14) && (staff.Player.LeftSide > 14 || staff.Player.RightSide > 14);
        }

        public static Boolean IsRightSide(CMStaff staff) {
            return (staff.Player.RightSide > 14 && !IsGoalkeeper(staff));
        }

        public static Boolean IsLeftSide(CMStaff staff) {
            return (staff.Player.LeftSide > 14 && !IsGoalkeeper(staff));
        }

        public static Boolean IsCentreSide(CMStaff staff) {
            return (staff.Player.CentreSide > 14 && !IsGoalkeeper(staff));
        }

        public static void CalculateRating(CMStaff staff, WeightsSet weightsSet) {
            for(Int32 i = 0; i < RatingPositions.Length; ++i) {
                staff.Ratings[i] = CalculateRating(staff, weightsSet.Weights[i]);
            }
        }

        private static Double CalculateRating(CMStaff staff, Byte[] weights) {
            Double r = 0;
            Int32  n = 0;

            for(Int32 i = 0; i < staff.AttributeValues.Length; ++i) {
                Attribute a = Attributes[i];
                Byte      w = weights[i];
                Int32     v = staff.AttributeValues[i].IntrinsicNormalized;

                if(a.IsLessBetter) {
                    v = 21 - v;
                }

                r += 1.0 * w * v / 20;
                n += w;
            }

            if(n != 0) {
                r = 100 * r / n;
            }

            return r;
        }

        public Boolean IsClubSquad(String clubName, Int32 squadId) {
            if(_clubNameToClubIdMap.TryGetValue(clubName, out Int32 clubId)) {
                if(_squadIdToClubIdsMap.TryGetValue(squadId, out List<Int32> squadClubIds)) {
                    return squadClubIds.Contains(clubId);
                }
            }

            return false;
        }

        public Boolean IsDivisionSquad(String divisionName, Int32 squadId) {
            if(_divisionNameToDivisionIdMap.TryGetValue(divisionName, out Int32 divisionId)) {
                if(_squadIdToClubIdsMap.TryGetValue(squadId, out List<Int32> squadClubIds)) {
                    foreach(Int32 squadClubId in squadClubIds) {
                        CMClub club = GetClubFromId(squadClubId, Clubs);

                        if(club != null) {
                            if(club.DivisionId == divisionId) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static Int32 DiffInYears(DateTime dateFrom, DateTime dateTo) {
            Int32 m = (dateFrom <= dateTo ? 1 : -1);

            DateTime dateFrom_ = (m > 0 ? dateFrom : dateTo  );
            DateTime dateTo_   = (m > 0 ? dateTo   : dateFrom);

            return ((new DateTime(1, 1, 1) + (dateTo_ - dateFrom_)).Year - 1) * m;
        }

        private Int32[] ReadIds(BinaryReader br, Int32 size) {
            Int32[] ids = new Int32[size];

            for(Int32 i = 0; i < size; ++i) {
                ids[i] = br.ReadInt32();
            }

            return ids;
        }

        private String ReadCMString(BinaryReader br, Int32 size) {
            Byte[] bb = br.ReadBytes(size);

            Int32 c = 0;

            foreach(Byte b in bb) {
                if(b == 0) {
                    break;
                }

                ++c;
            }

            return Encoding.GetEncoding("iso-8859-1").GetString(bb, 0, c);
        }

        private CMDate ReadCMDate(BinaryReader br) {
            return new CMDate {
                Day        = br.ReadInt16(),
                Year       = br.ReadInt16(),
                IsLeapYear = br.ReadInt32(),
            };
        }

        private DateTime? ReadDateTime(BinaryReader br) {
            return GetDateFromCMDate(ReadCMDate(br));
        }

        private DateTime? GetDateFromCMDate(CMDate cmDate) {
            if(cmDate.Day == 0 && cmDate.Year == 0 /*&& cmDate.IsLeapYear == 0*/) {
                return null;
            }
            else {
                return new DateTime(cmDate.Year, 1, 1).AddDays(cmDate.Day);
            }
        }

        private CMName GetNameFromId(Int32 id, List<CMName> a) {
            return (id < 0 ? null : a[id]);
        }

        private CMNation GetNationFromId(Int32 id, List<CMNation> a) {
            return (id < 0 ? null : a[id]);
        }

        private CMDivision GetDivisionFromId(Int32 id, List<CMDivision> a) {
            return (id < 0 ? null : a[id]);
        }

        private CMClub GetClubFromId(Int32 id, List<CMClub> a) {
            return (id < 0 ? null : a[id]);
        }

        private CMPlayer GetPlayerFromId(Int32 id, List<CMPlayer> a) {
            return (id < 0 ? null : a[id]);
        }

        private SByte HighConvert(Int16 currentAbility, SByte attributeValue) {
            Double d = (attributeValue / 10.0) + (currentAbility / 20.0) + 10;

            Double r = (d * d / 30.0) + (d / 3.0) + 0.5;

            if(r < 1) {
                r = 1;
            }
            else if(r > 20) {
                r = 20;
            }

            return (SByte)Math.Truncate(r);
        }

        private SByte LowConvert(Int16 currentAbility, SByte attributeValue) {
            Double d = (attributeValue / 10.0) + (currentAbility / 200.0) + 10;

            Double r = (d * d / 30.0) + (d / 3.0) + 0.5;

            if(r < 1) {
                r = 1;
            }
            else if(r > 20) {
                r = 20;
            }

            return (SByte)Math.Truncate(r);
        }



        private Dictionary<String, Int32>        _divisionNameToDivisionIdMap;
        private Dictionary<String, Int32>        _clubNameToClubIdMap;
        private Dictionary< Int32, List<Int32> > _squadIdToClubIdsMap;
        private Pair<SByte, SByte>[]             _ca18AttributeValueRanges;
    }

}
