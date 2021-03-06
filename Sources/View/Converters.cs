using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace CMScoutIntrinsic {

    class ConditionConverter : DependencyObject, IValueConverter {
        public String Condition { get; set; }

        public static readonly DependencyProperty OnRightProperty = DependencyProperty.Register("OnRight", typeof(Object), typeof(ConditionConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty IfTrueProperty  = DependencyProperty.Register("IfTrue",  typeof(Object), typeof(ConditionConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty IfFalseProperty = DependencyProperty.Register("IfFalse", typeof(Object), typeof(ConditionConverter), new PropertyMetadata(null));

        public Object OnRight { get { return (Object)GetValue(OnRightProperty); } set { SetValue(OnRightProperty, value); } }
        public Object IfTrue  { get { return (Object)GetValue(IfTrueProperty);  } set { SetValue(IfTrueProperty,  value); } }
        public Object IfFalse { get { return (Object)GetValue(IfFalseProperty); } set { SetValue(IfFalseProperty, value); } }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Object onLeft = value;

            Object onRight = (onLeft != null && OnRight != null && onLeft.GetType() != OnRight.GetType() ? System.Convert.ChangeType(OnRight, onLeft.GetType()) : OnRight);

            Boolean result;

            if(Condition == "==") {
                result = Equals(onLeft, onRight);
            }
            else if(Condition == "<") {
                result = IsLess(onLeft, onRight);
            }
            else if(Condition == "!=") {
                result = !Equals(onLeft, onRight);
            }
            else if(Condition == ">") {
                result = !IsLess(onLeft, onRight) && !Equals(onLeft, onRight);
            }
            else if(Condition == "<=") {
                result = IsLess(onLeft, onRight) || Equals(onLeft, onRight);
            }
            else if(Condition == ">=") {
                result = !IsLess(onLeft, onRight);
            }
            else {
                throw new Exception(String.Format("ConditionConverter. Unsupported condition \"{0}\"", Condition));
            }

            return (result ? IfTrue : IfFalse);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }

        private Boolean IsLess(Object onLeft, Object onRight) {
            if(onLeft != null) {
                if(onRight != null) {
                    return ( ((IComparable)onLeft).CompareTo(onRight) < 0 );
                }
                else {
                    return false;
                }
            }
            else {
                if(onRight != null) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }

    class BinaryOperationConverter : DependencyObject, IValueConverter {
        public String Operation { get; set; }

        public static readonly DependencyProperty OnRightProperty = DependencyProperty.Register("OnRight", typeof(Object), typeof(BinaryOperationConverter), new PropertyMetadata(null));

        public Object OnRight { get { return (Object)GetValue(OnRightProperty); } set { SetValue(OnRightProperty, value); } }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            if(value == null) { return null; }

            Object onLeft  = value;
            Object onRight = OnRight;

            Decimal dOnLeft  = System.Convert.ToDecimal(onLeft);
            Decimal dOnRight = System.Convert.ToDecimal(onRight);

            Decimal result;

            if(Operation == "+") {
                result = dOnLeft + dOnRight;
            }
            else if(Operation == "-") {
                result = dOnLeft - dOnRight;
            }
            else if(Operation == "*") {
                result = dOnLeft * dOnRight;
            }
            else if(Operation == "/") {
                result = dOnLeft / dOnRight;
            }
            else {
                throw new Exception(String.Format("BinaryOperationConverter. Unsupported operation \"{0}\"", Operation));
            }

            return System.Convert.ChangeType(result, onLeft.GetType());
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class UnaryOperationConverter : IValueConverter {
        public String Operation { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            if(value == null) { return null; }

            Object onLeft = value;

            Decimal dOnLeft = System.Convert.ToDecimal(onLeft);

            Decimal result;

            if(Operation == "-") {
                result = - dOnLeft;
            }
            else if(Operation == "Floor") {
                result = Math.Floor(dOnLeft);
            }
            else {
                throw new Exception(String.Format("UnaryOperationConverter. Unsupported operation \"{0}\"", Operation));
            }

            return System.Convert.ChangeType(result, onLeft.GetType());
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class ChangeTypeConverter : IValueConverter {
        public Type Type { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            return System.Convert.ChangeType(value, Type);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class Case : DependencyObject {
        public static readonly DependencyProperty KeyProperty   = DependencyProperty.Register("Key",   typeof (Object), typeof (Case), new PropertyMetadata(null));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (Object), typeof (Case), new PropertyMetadata(null));

        public Object Key   { get { return (Object)GetValue(KeyProperty);   } set { SetValue(KeyProperty, value);   } }
        public Object Value { get { return (Object)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
    }   

    [ContentProperty(Name="Cases")]
    class SwitchConverter : DependencyObject, IValueConverter {
        public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register("Default", typeof (Object), typeof (SwitchConverter), new PropertyMetadata(null));

        public Object Default { get { return (Object)GetValue(DefaultProperty); } set { SetValue(DefaultProperty, value); } }

        public Boolean DefaultIsSource { get; set; }

        public List<Case> Cases { get; set; }

        public SwitchConverter() {
            Cases = new List<Case>();
        }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            foreach(Case case_ in Cases) {
                Object key;

                if(value != null && case_.Key != null && value.GetType() != case_.Key.GetType()) {
                    if(value.GetType().GetTypeInfo().IsEnum) {
                        key = Helpers.GetEnumValue(case_.Key, value.GetType());
                    }
                    else {
                        key = System.Convert.ChangeType(case_.Key, value.GetType());
                    }
                }
                else {
                    key = case_.Key;
                }

                if(Equals(key, value)) {
                    return case_.Value;
                }
            }

            return DefaultIsSource ? value : Default;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }
    }

    class GetVisualParentConverter : IValueConverter {
        public Type Type { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            if(value is DependencyObject) {
                Type type = Type;

                DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)value);

                while(parent != null) {
                    if(parent.GetType() == type) {
                        return parent;
                    }

                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class GetPropertyValueConverter : IValueConverter {
        public String PropertyName { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            if(value != null) {
                String propertyName = (parameter != null ? (String)parameter : PropertyName);

                PropertyInfo propertyInfo = value.GetType().GetProperty(propertyName);

                if(propertyInfo != null) {
                    return propertyInfo.GetValue(value);
                }
                else {
                    Debug.WriteLine("GetPropertyValueConverter: can't get property \"{0}\" for object \"{1}\"", propertyName, value);
                }
            }

            return null;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }
    
    class StringFormatConverter : MultiValueConverterBase {
        public String Format { get; set; }

        public override Object Convert(Object[] values, Type targetType, Object parameter, String language) {
            String format = (parameter != null ? (String)parameter : Format);

            return String.Format(format, values);
        }

        public override Object[] ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class ChainConverter : List<IValueConverter>, IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Object val = value;

            foreach(IValueConverter converter in this) {
                val = converter.Convert(val, targetType, parameter, language);
            }

            return val;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }
    }

    class EnumerableToElementAtConverter : MultiValueConverterBase {
        public override Object Convert(Object[] values, Type targetType, Object parameter, String language) {
            foreach(Object value in values) { if(value == null) { return null; } }

            return ((IEnumerable)(values[0])).ElementAt((Int32)values[1]);
        }

        public override Object[] ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class DoubleToThicknessConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            return new Thickness((Double)value);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class LocalizationConverter : IValueConverter {
        public String Context { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            String source  = (String)value;
            String context =  (parameter != null ? (String)parameter : Context);

            return source;//((App)Application.Current).LocalizationService.Translate(context, source);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }



    class PlayerToHeaderStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staffVM = (CMStaffVM)value;

            return String.Format(
                "{0}{1}{2}",
                staffVM.FirstName,
                (String.IsNullOrEmpty(staffVM.FirstName) ? String.Empty : " ") + staffVM.SecondName,
                String.IsNullOrEmpty(staffVM.ClubShortName) ? String.Empty : " (" + staffVM.ClubShortName + ")"
            );
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class PlayerToSubheaderStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staffVM = (CMStaffVM)value;

            String s = String.Empty;
            
            if(staffVM.DateOfBirth != null) {
                s += "Born " + staffVM.DateOfBirth.Value.ToString("d") + " (Age " + staffVM.Age.Value.ToString() + "). ";
            }

            s += staffVM.Value.FirstNation.Nationality;

            if(staffVM.Value.IntApps > 0) {
                s += " (" + staffVM.Value.IntApps.ToString() + " " + (staffVM.Value.IntApps > 1 ? "caps" : "cap");

                if(staffVM.Value.IntGoals > 0) {
                    s += "/" + staffVM.Value.IntGoals.ToString() + " " + (staffVM.Value.IntGoals > 1 ? "goals" : "goal");
                }

                s += ")";
            }

            s += ".";

            return s;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class PlayerToPositionShortStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staffVM = (CMStaffVM)value;

            List<String> positions = DataService.Positions.Where(item => item.IsSuitableFor(staffVM.Value)).Select(item => item.Code).ToList();

            List<String> sides = new List<Side> {
                DataService.Sides.Find(item => item.Code == "R"),
                DataService.Sides.Find(item => item.Code == "L"),
                DataService.Sides.Find(item => item.Code == "C"),
            }
            .Where(item => item.IsSuitableFor(staffVM.Value)).Select(item => item.Code).ToList();

            return String.Format(
                "{0}{1}",
                positions.Count > 0 ? String.Join("/", positions)                   : String.Empty,
                sides.Count     > 0 ? String.Format(" {0}", String.Join("", sides)) : String.Empty
            );
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class PlayerToPositionStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staffVM = (CMStaffVM)value;

            List<String> positions = DataService.Positions.Where(item => item.IsSuitableFor(staffVM.Value)).Select(item => item.Name).ToList();

            List<String> sides = new List<Side> {
                    DataService.Sides.Find(item => item.Code == "R"),
                    DataService.Sides.Find(item => item.Code == "L"),
                    DataService.Sides.Find(item => item.Code == "C"),
                }
                .Where(item => item.IsSuitableFor(staffVM.Value)).Select(item => item.Name).ToList();

            return String.Format(
                "{0}{1}",
                positions.Count > 0 ? String.Join("/", positions)                      : String.Empty,
                sides.Count     > 0 ? String.Format(" ({0})", String.Join("/", sides)) : String.Empty
            );
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class ContractTypeToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Byte type = (Byte)value;

            foreach(ContractType contractType in DataService.ContractTypes) {
                if(contractType.IsSuitableFor(type)) {
                    return contractType.Name;
                }
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class SquadStatusToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Byte status = (Byte)value;

            foreach(SquadStatus squadStatus in DataService.SquadStatuses) {
                if(squadStatus.IsSuitableFor(status)) {
                    return squadStatus.Name;
                }
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class TransferStatusToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Byte status = (Byte)value;

            foreach(TransferStatus transferStatus in DataService.TransferStatuses) {
                if(transferStatus.IsSuitableFor(status)) {
                    return transferStatus.Name;
                }
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class ContractProtectionToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            String unprotectedReason = (String)value;

            return String.IsNullOrEmpty(unprotectedReason) ? "Yes" : String.Format("No ({0})", unprotectedReason, 0);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class LoanStatusToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staff = (CMStaffVM)value;

            if(staff.Value.LoanContract != null) {
                return String.Format("On loan from {0} until {1}", staff.Value.Contract.Club.ShortName, staff.Value.LoanContract.DateEnded.Value.ToString("dd MMMM yyyy"));
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class FutureToStringConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            CMStaffVM staff = (CMStaffVM)value;

            if(staff.Value.Contract != null) {
                if(staff.Value.Contract.LeavingOnBosman) {
                    return String.Format("Leaving the club under the Bosman ruling on {0}", staff.Value.Contract.DateEnded.Value.ToString("dd MMMM yyyy"));
                }
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }

    class ComparePlayersToAttributeValueConverter : DependencyObject, IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            ComparePlayersViewModel viewModel = (ComparePlayersViewModel)value;
            Int32                   index     = Int32.Parse((String)parameter);

            return Math.Abs(viewModel.LeftPlayer.Attributes[index].Value - viewModel.RightPlayer.Attributes[index].Value);
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }
    }

    class ComparePlayersToAttributeValueBackgroundConverter : DependencyObject, IValueConverter {
        public Brush HighlightBrush  { get; set; }

        public static readonly DependencyProperty LeftPlayerProperty  = DependencyProperty.Register("LeftPlayer",  typeof(CMStaffVM), typeof(ComparePlayersToAttributeValueBackgroundConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty RightPlayerProperty = DependencyProperty.Register("RightPlayer", typeof(CMStaffVM), typeof(ComparePlayersToAttributeValueBackgroundConverter), new PropertyMetadata(null));

        public CMStaffVM LeftPlayer  { get { return (CMStaffVM)GetValue(LeftPlayerProperty);  } set { SetValue(LeftPlayerProperty,  value); } }
        public CMStaffVM RightPlayer { get { return (CMStaffVM)GetValue(RightPlayerProperty); } set { SetValue(RightPlayerProperty, value); } }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            RatingPosition ratingPosition = (RatingPosition)value;
            Int32          index          = Int32.Parse((String)parameter);

            Int32 ratingPositionIndex = DataService.RatingPositions.FindIndex(item => ReferenceEquals(item, ratingPosition));

            App app = (App)Application.Current;

            WeightsSet weightsSet = app.SettingsService.GetWeightsSets().Find(item => item.IsLast == true);

            Byte[] weights = weightsSet.Weights[ratingPositionIndex];

            List<Int32> indexes = new List<Int32>();

            for(Int32 i = 0; i < weights.Length; ++i) {
                if(weights[i] != 0) {
                    indexes.Add(i);
                }
            }

            Int32 maxCount = 10;

            if(indexes.Count > maxCount) {
                indexes.Sort( (x, y) => weights[y].CompareTo(weights[x]) );

                Int32 count = maxCount;

                for(; count < indexes.Count; ++count) {
                    if(weights[ indexes[count] ] != weights[ indexes[maxCount - 1] ]) {
                        break;
                    }
                }

                if(count < indexes.Count) {
                    indexes.RemoveRange(count, indexes.Count - count);
                }
            }

            Boolean isHighlited = indexes.Contains(index);

            Int32 leftValue  = LeftPlayer.Value.AttributeValues[index].IntrinsicNormalized;
            Int32 rightValue = RightPlayer.Value.AttributeValues[index].IntrinsicNormalized;

            if(DataService.Attributes[index].IsLessBetter) {
                leftValue  = 21 - leftValue;
                rightValue = 21 - rightValue;
            }

            Int32 diff = leftValue - rightValue;

            if(isHighlited) {
                if(diff > 0) {
                    return HighlightBrush;
                }
                else if(diff < 0) {
                    return HighlightBrush;
                }
                else {
                }
            }
            else {
                if(diff > 0) {
                }
                else if(diff < 0) {
                }
                else {
                }
            }

            return null;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }
    }

    class ComparePlayersToAttributeValueStyleConverter : DependencyObject, IValueConverter {
        public Style LeftStyle  { get; set; }
        public Style RightStyle { get; set; }
        public Style SameStyle  { get; set; }

        public static readonly DependencyProperty LeftPlayerProperty  = DependencyProperty.Register("LeftPlayer",  typeof(CMStaffVM), typeof(ComparePlayersToAttributeValueStyleConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty RightPlayerProperty = DependencyProperty.Register("RightPlayer", typeof(CMStaffVM), typeof(ComparePlayersToAttributeValueStyleConverter), new PropertyMetadata(null));

        public CMStaffVM LeftPlayer  { get { return (CMStaffVM)GetValue(LeftPlayerProperty);  } set { SetValue(LeftPlayerProperty,  value); } }
        public CMStaffVM RightPlayer { get { return (CMStaffVM)GetValue(RightPlayerProperty); } set { SetValue(RightPlayerProperty, value); } }

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            RatingPosition ratingPosition = (RatingPosition)value;
            Int32          index          = Int32.Parse((String)parameter);

            Int32 ratingPositionIndex = DataService.RatingPositions.FindIndex(item => ReferenceEquals(item, ratingPosition));

            App app = (App)Application.Current;

            WeightsSet weightsSet = app.SettingsService.GetWeightsSets().Find(item => item.IsLast == true);

            Byte[] weights = weightsSet.Weights[ratingPositionIndex];

            List<Int32> indexes = new List<Int32>();

            for(Int32 i = 0; i < weights.Length; ++i) {
                if(weights[i] != 0) {
                    indexes.Add(i);
                }
            }

            Int32 maxCount = 10;

            if(indexes.Count > maxCount) {
                indexes.Sort( (x, y) => weights[y].CompareTo(weights[x]) );

                Int32 count = maxCount;

                for(; count < indexes.Count; ++count) {
                    if(weights[ indexes[count] ] != weights[ indexes[maxCount - 1] ]) {
                        break;
                    }
                }

                if(count < indexes.Count) {
                    indexes.RemoveRange(count, indexes.Count - count);
                }
            }

            Boolean isHighlited = indexes.Contains(index);

            Int32 leftValue  = LeftPlayer.Value.AttributeValues[index].IntrinsicNormalized;
            Int32 rightValue = RightPlayer.Value.AttributeValues[index].IntrinsicNormalized;

            if(DataService.Attributes[index].IsLessBetter) {
                leftValue  = 21 - leftValue;
                rightValue = 21 - rightValue;
            }

            Int32 diff = leftValue - rightValue;

            if(isHighlited) {
                if(diff > 0) {
                    return LeftStyle;
                }
                else if(diff < 0) {
                    return RightStyle;
                }
                else {
                    return SameStyle;
                }
            }
            else {
                if(diff > 0) {
                    return LeftStyle;
                }
                else if(diff < 0) {
                    return RightStyle;
                }
                else {
                    return SameStyle;
                }
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotSupportedException();
        }
    }

}
