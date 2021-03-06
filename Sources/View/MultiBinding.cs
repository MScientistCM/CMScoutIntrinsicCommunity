using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Windows.Foundation.Collections;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace CMScoutIntrinsic {

    class DependencyObjectCollection<T> : DependencyObjectCollection, INotifyCollectionChanged
        where T : DependencyObject
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly List<T> _oldItems = new List<T>();

        public DependencyObjectCollection() {
            VectorChanged += DependencyObjectCollectionVectorChanged;
        }

        private void DependencyObjectCollectionVectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs args) {
            Int32 index = (Int32)args.Index;

            switch(args.CollectionChange) {
                case CollectionChange.Reset:
                    foreach(DependencyObject item in this) {
                        VerifyType(item);
                    }

                    RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                    _oldItems.Clear();

                    break;

                case CollectionChange.ItemInserted:
                    VerifyType(this[index]);

                    RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[index], index));

                    _oldItems.Insert(index, (T)this[index]);

                    break;

                case CollectionChange.ItemRemoved:
                    RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _oldItems[index], index));

                    _oldItems.RemoveAt(index);

                    break;

                case CollectionChange.ItemChanged:
                    VerifyType(this[index]);

                    RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], _oldItems[index]));

                    _oldItems[index] = (T)this[index];

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args) {
            CollectionChanged?.Invoke(this, args);
        }

        private void VerifyType(DependencyObject item) {
            if(item is T) {
            }
            else {
                throw new InvalidOperationException("Invalid item type added to collection");
            }
        }
    }



    class MultiBindingItem : DependencyObject {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(MultiBindingItem), new PropertyMetadata(null, OnValueChanged));

        public Object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            MultiBindingItem multiBindingItem = (MultiBindingItem)sender;

            multiBindingItem.Update();
        }

        internal MultiBindingItemCollection Parent { get; set; }

        private void Update() {
            MultiBindingItemCollection parent = Parent;

            parent?.Update();
        }
    }



    class MultiBindingItemCollection : DependencyObjectCollection<MultiBindingItem> {
        private Boolean _updating;

        internal static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object[]), typeof(MultiBindingItemCollection), new PropertyMetadata(null, OnValueChanged));

        public Object[] Value { get { return (Object[])GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            MultiBindingItemCollection multiBindingItemCollection = (MultiBindingItemCollection)sender;

            multiBindingItemCollection.UpdateSource();
        }

        public MultiBindingItemCollection() {
            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args) {
            if(args.OldItems != null) {
                foreach (MultiBindingItem item in args.OldItems) {
                    item.Parent = null;
                }
            }

            if(args.NewItems != null) {
                foreach (MultiBindingItem item in args.NewItems) {
                    item.Parent = this;
                }
            }

            Update();
        }

        internal void Update() {
            if(_updating) {
                return;
            }

            try {
                _updating = true;

                Value = this
                    .OfType<MultiBindingItem>()
                    .Select(x => x.Value)
                    .ToArray();
            }
            finally {
                _updating = false;
            }
        }

        private void UpdateSource() {
            if(_updating) {
                return;
            }

            try {
                _updating = true;

                for(Int32 index = 0; index < this.Count; ++index) {
                    MultiBindingItem multiBindingItem = this[index] as MultiBindingItem;

                    if(multiBindingItem != null) {
                        multiBindingItem.Value = Value[index];
                    }
                }
            }
            finally {
                _updating = false;
            }
        }
    }


    [ContentProperty(Name = "Items")]
    [TypeConstraint(typeof(FrameworkElement))]
    class MultiBindingBehavior : Behavior<FrameworkElement> {

        public static readonly DependencyProperty PropertyNameProperty       = DependencyProperty.Register("PropertyName",       typeof(String),                     typeof(MultiBindingBehavior), new PropertyMetadata(null,               OnPropertyChanged));
        public static readonly DependencyProperty ItemsProperty              = DependencyProperty.Register("Items",              typeof(MultiBindingItemCollection), typeof(MultiBindingBehavior), null);
        public static readonly DependencyProperty ConverterProperty          = DependencyProperty.Register("Converter",          typeof(IValueConverter),            typeof(MultiBindingBehavior), new PropertyMetadata(null,               OnPropertyChanged));
        public static readonly DependencyProperty ConverterParameterProperty = DependencyProperty.Register("ConverterParameter", typeof(Object),                     typeof(MultiBindingBehavior), new PropertyMetadata(null,               OnPropertyChanged));
        public static readonly DependencyProperty ModeProperty               = DependencyProperty.Register("Mode",               typeof(BindingMode),                typeof(MultiBindingBehavior), new PropertyMetadata(BindingMode.OneWay, OnPropertyChanged));

        public String                     PropertyName       { get { return (String)GetValue(PropertyNameProperty);              }         set { SetValue(PropertyNameProperty,       value); } }
        public MultiBindingItemCollection Items              { get { return (MultiBindingItemCollection)GetValue(ItemsProperty); } private set { SetValue(ItemsProperty,              value); } }
        public IValueConverter            Converter          { get { return (IValueConverter)GetValue(ConverterProperty);        }         set { SetValue(ConverterProperty,          value); } }
        public Object                     ConverterParameter { get { return GetValue(ConverterParameterProperty);                }         set { SetValue(ConverterParameterProperty, value); } }
        public BindingMode                Mode               { get { return (BindingMode)GetValue(ModeProperty);                 }         set { SetValue(ModeProperty,               value); } }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            MultiBindingBehavior multiBindingBehavior = (MultiBindingBehavior)sender;

            multiBindingBehavior.Update();
        }

        public MultiBindingBehavior() {
            Items = new MultiBindingItemCollection();
        }

        protected override void OnAttached() {
            base.OnAttached();

            Update();
        }

        private void Update() {
            if(AssociatedObject == null || String.IsNullOrEmpty(PropertyName)) {
                return;
            }

            String targetProperty = PropertyName;
            Type targetType;

            if(targetProperty.Contains(".")) {
                String[] propertyNameParts = targetProperty.Split('.');

                targetType = Type.GetType(String.Format("Windows.UI.Xaml.Controls.{0}, Windows", propertyNameParts[0]));

                targetProperty = propertyNameParts[1];
            }
            else {
                targetType = AssociatedObject.GetType();
            }

            PropertyInfo targetDependencyPropertyField = null;

            while(targetDependencyPropertyField == null && targetType != null) {
                TypeInfo targetTypeInfo = targetType.GetTypeInfo();

                targetDependencyPropertyField = targetTypeInfo.GetDeclaredProperty(targetProperty + "Property");

                targetType = targetTypeInfo.BaseType;
            }

            DependencyProperty targetDependencyProperty = (DependencyProperty)targetDependencyPropertyField.GetValue(null);

            Binding binding = new Binding {
                Path               = new PropertyPath("Value"),
                Source             = Items,
                Converter          = Converter,
                ConverterParameter = ConverterParameter,
                Mode               = Mode,
            };

            BindingOperations.SetBinding(AssociatedObject, targetDependencyProperty, binding);
        }
    }



    abstract class MultiValueConverterBase : DependencyObject, IValueConverter {
        public abstract Object Convert(Object[] values, Type targetType, Object parameter, String language);
        public abstract Object[] ConvertBack(Object value, Type targetType, Object parameter, String language);

        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, String language) {
            Object[] values = (value is Object[]? (Object[])value : new Object[] { value });

            return Convert(values, targetType, parameter, language);
        }

        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, String language) {
            return ConvertBack(value, targetType, parameter, language);
        }
    }

}