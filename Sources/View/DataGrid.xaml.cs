using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CMScoutIntrinsic {

    enum DataGridColumnSizeMode {
        Stretch = 0,
        //Auto    = 1,
        Fixed   = 2
    }

    enum DataGridColumnSortDirection {
        None       = 0,
        Ascending  = 1,
        Descending = 2
    }

    sealed class DataGridColumn : DependencyObject, INotifyPropertyChanged  {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler IsVisibleChanged;

        private void RaiseIsVisibleChanged() {
            IsVisibleChanged?.Invoke(this, EventArgs.Empty);
        }



        public static readonly DependencyProperty StyleProperty                            = DependencyProperty.Register("Style",                            typeof(Style),                  typeof(DataGridColumn), new PropertyMetadata(null, OnStyleChanged));
        public static readonly DependencyProperty ColumnNameProperty                       = DependencyProperty.Register("ColumnName",                       typeof(String),                 typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty SizeModeProperty                         = DependencyProperty.Register("SizeMode",                         typeof(DataGridColumnSizeMode), typeof(DataGridColumn), new PropertyMetadata(DataGridColumnSizeMode.Stretch, null));
        public static readonly DependencyProperty WidthProperty                            = DependencyProperty.Register("Width",                            typeof(Double),                 typeof(DataGridColumn), new PropertyMetadata(0.0, null));
        public static readonly DependencyProperty IsVisibleProperty                        = DependencyProperty.Register("IsVisible",                        typeof(Boolean),                typeof(DataGridColumn), new PropertyMetadata(true, OnIsVisibleChanged));
        public static readonly DependencyProperty CanSortProperty                          = DependencyProperty.Register("CanSort",                          typeof(Boolean),                typeof(DataGridColumn), new PropertyMetadata(false, null));
        public static readonly DependencyProperty IsDefaultSortDirectionDescendingProperty = DependencyProperty.Register("IsDefaultSortDirectionDescending", typeof(Boolean),                typeof(DataGridColumn), new PropertyMetadata(false, null));
        public static readonly DependencyProperty TextProperty                             = DependencyProperty.Register("Text",                             typeof(String),                 typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty LocStringProperty                        = DependencyProperty.Register("LocString",                        typeof(LocString),              typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty HeaderStyleProperty                      = DependencyProperty.Register("HeaderStyle",                      typeof(Style),                  typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty CellContentTemplateProperty              = DependencyProperty.Register("CellContentTemplate",              typeof(DataTemplate),           typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty HeaderItemsSourceProperty                = DependencyProperty.Register("HeaderItemsSource",                typeof(Object),                 typeof(DataGridColumn), new PropertyMetadata(null, null));
        public static readonly DependencyProperty ChildPropertyPathProperty                = DependencyProperty.Register("ChildPropertyPath",                typeof(String),                 typeof(DataGridColumn), new PropertyMetadata(null, null));

        public static Style GetStyle(DependencyObject obj) { return (Style)obj.GetValue(StyleProperty); }
        public static void SetStyle(DependencyObject obj, Style value) { obj.SetValue(StyleProperty, value); }

        public static String GetColumnName(DependencyObject obj) { return (String)obj.GetValue(ColumnNameProperty); }
        public static void SetColumnName(DependencyObject obj, String value) { obj.SetValue(ColumnNameProperty, value); }

        public static DataGridColumnSizeMode GetSizeMode(DependencyObject obj) { return (DataGridColumnSizeMode)obj.GetValue(SizeModeProperty); }
        public static void SetSizeMode(DependencyObject obj, DataGridColumnSizeMode value) { obj.SetValue(SizeModeProperty, value); }

        public static Double GetWidth(DependencyObject obj) { return (Double)obj.GetValue(WidthProperty); }
        public static void SetWidth(DependencyObject obj, Double value) { obj.SetValue(WidthProperty, value); }

        public static Boolean GetIsVisible(DependencyObject obj) { return (Boolean)obj.GetValue(IsVisibleProperty); }
        public static void SetIsVisible(DependencyObject obj, Boolean value) { obj.SetValue(IsVisibleProperty, value); }

        public static Boolean GetCanSort(DependencyObject obj) { return (Boolean)obj.GetValue(CanSortProperty); }
        public static void SetCanSort(DependencyObject obj, Boolean value) { obj.SetValue(CanSortProperty, value); }

        public static Boolean GetIsDefaultSortDirectionDescending(DependencyObject obj) { return (Boolean)obj.GetValue(IsDefaultSortDirectionDescendingProperty); }
        public static void SetIsDefaultSortDirectionDescending(DependencyObject obj, Boolean value) { obj.SetValue(IsDefaultSortDirectionDescendingProperty, value); }

        public static String GetText(DependencyObject obj) { return (String)obj.GetValue(TextProperty); }
        public static void SetText(DependencyObject obj, String value) { obj.SetValue(TextProperty, value); }

        public static LocString GetLocString(DependencyObject obj) { return (LocString)obj.GetValue(LocStringProperty); }
        public static void SetLocString(DependencyObject obj, LocString value) { obj.SetValue(LocStringProperty, value); }

        public static Style GetHeaderStyle(DependencyObject obj) { return (Style)obj.GetValue(HeaderStyleProperty); }
        public static void SetHeaderStyle(DependencyObject obj, Style value) { obj.SetValue(HeaderStyleProperty, value); }

        public static DataTemplate GetCellContentTemplate(DependencyObject obj) { return (DataTemplate)obj.GetValue(CellContentTemplateProperty); }
        public static void SetCellContentTemplate(DependencyObject obj, DataTemplate value) { obj.SetValue(CellContentTemplateProperty, value); }

        public static Object GetHeaderItemsSource(DependencyObject obj) { return (Object)obj.GetValue(HeaderItemsSourceProperty); }
        public static void SetHeaderItemsSource(DependencyObject obj, Object value) { obj.SetValue(HeaderItemsSourceProperty, value); }

        public static String GetChildPropertyPath(DependencyObject obj) { return (String)obj.GetValue(ChildPropertyPathProperty); }
        public static void SetChildPropertyPath(DependencyObject obj, String value) { obj.SetValue(ChildPropertyPathProperty, value); }

        private static void OnStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            Helpers.ApplyStyle(obj, args.NewValue as Style);
        }

        private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((DataGridColumn)obj).RaiseIsVisibleChanged();
        }



        public Style                       Style                            { get { return (Style)GetValue(StyleProperty);                              } set { SetValue(StyleProperty,                            value); } }
        public String                      ColumnName                       { get { return (String)GetValue(ColumnNameProperty);                        } set { SetValue(ColumnNameProperty,                       value); } }
        public DataGridColumnSizeMode      SizeMode                         { get { return GetSizeMode();                                               } set { SetValue(SizeModeProperty,                         value); } }
        public Double                      Width                            { get { return (Double)GetValue(WidthProperty);                             } set { SetValue(WidthProperty,                            value); } }
        public Boolean                     IsVisible                        { get { return (Boolean)GetValue(IsVisibleProperty);                        } set { SetValue(IsVisibleProperty,                        value); } }
        public Boolean                     CanSort                          { get { return (Boolean)GetValue(CanSortProperty);                          } set { SetValue(CanSortProperty,                          value); } }
        public Boolean                     IsDefaultSortDirectionDescending { get { return (Boolean)GetValue(IsDefaultSortDirectionDescendingProperty); } set { SetValue(IsDefaultSortDirectionDescendingProperty, value); } }
        public DataGridColumnSortDirection SortDirection                    { get { return _sortDirection;                                              } set { _sortDirection = value; RaisePropertyChanged();            } }
        public String                      Text                             { get { return (String)GetValue(TextProperty);                              } set { SetValue(TextProperty,                             value); } }
        public LocString                   LocString                        { get { return (LocString)GetValue(LocStringProperty);                      } set { SetValue(LocStringProperty,                        value); } }
        public Style                       HeaderStyle                      { get { return (Style)GetValue(HeaderStyleProperty);                        } set { SetValue(HeaderStyleProperty,                      value); } }
        public DataTemplate                CellContentTemplate              { get { return (DataTemplate)GetValue(CellContentTemplateProperty);         } set { SetValue(CellContentTemplateProperty,              value); } }
        public Object                      HeaderItemsSource                { get { return (Object)GetValue(HeaderItemsSourceProperty);                 } set { SetValue(HeaderItemsSourceProperty,                value); } }
        public String                      ChildPropertyPath                { get { return (String)GetValue(ChildPropertyPathProperty);                 } set { SetValue(ChildPropertyPathProperty,                value); } }

        public Object                      Item                             { get; set; }
        public Int32                       Index                            { get; set; }
        public Int32                       Count                            { get; set; }

        private DataGridColumnSizeMode GetSizeMode() {
            return (DataGridColumnSizeMode)Helpers.GetEnumValue(GetValue(SizeModeProperty), typeof(DataGridColumnSizeMode));
        }

        private DataGridColumnSortDirection _sortDirection;
    }

    enum DataGridRowState {
        Normal,
        PointerOver,
        Pressed,
        Selected,
        PointerOverSelected,
        PressedSelected,
    }

    sealed class DataGridRow : ViewModelBase {
        public Double                              Height                { get; set; }
        public Double                              LinesThickness        { get; set; }
        public Brush                               LinesBrush            { get; set; }
        public Brush                               RowBackground         { get; set; }
        public IValueConverter                     RowBackgroundSelector { get; set; }
        public List<DataGridColumn>                Columns               { get; set; }
        public Object                              Item                  { get; set; }
        public Object                              Parent                { get; set; }
        public Action<DataGridRow, DataGridColumn> OnCellTapped          { get; set; }
        public Action<DataGridRow, DataGridColumn> OnCellRightTapped     { get; set; }

        public DataGridRowState State {
            get {
                if(_isSelected && _isPointerPressed) {
                    return DataGridRowState.PressedSelected;
                }

                if(_isSelected && _isPointerOver) {
                    return DataGridRowState.PointerOverSelected;
                }

                if(_isSelected) {
                    return DataGridRowState.Selected;
                }

                if(_isPointerPressed) {
                    return DataGridRowState.Pressed;
                }

                if(_isPointerOver) {
                    return DataGridRowState.PointerOver;
                }

                return DataGridRowState.Normal;
            }
        }

        public Boolean IsSelected       { get {return _isSelected;       } set { _isSelected       = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(State)); } }
        public Boolean IsPointerOver    { get {return _isPointerOver;    } set { _isPointerOver    = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(State)); } }
        public Boolean IsPointerPressed { get {return _isPointerPressed; } set { _isPointerPressed = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(State)); } }



        private Boolean _isSelected;
        private Boolean _isPointerOver;
        private Boolean _isPointerPressed;
    }

    sealed class DataGrid : ContentControl {

        public static readonly DependencyProperty ItemsSourceProperty                = DependencyProperty.Register("ItemsSource",                typeof(Object),                                     typeof(DataGrid), new PropertyMetadata(null,                       OnItemsSourceChanged));
        public static readonly DependencyProperty SortColumnProperty                 = DependencyProperty.Register("SortColumn",                 typeof(Pair<String, Boolean>),                      typeof(DataGrid), new PropertyMetadata(null,                       OnSortColumnChanged));
        public static readonly DependencyProperty HeaderHeightProperty               = DependencyProperty.Register("HeaderHeight",               typeof(Double),                                     typeof(DataGrid), new PropertyMetadata(32.0,                       OnHeaderHeightChanged));
        public static readonly DependencyProperty RowHeightProperty                  = DependencyProperty.Register("RowHeight",                  typeof(Double),                                     typeof(DataGrid), new PropertyMetadata(48.0,                       OnRowHeightChanged));
        public static readonly DependencyProperty LinesThicknessProperty             = DependencyProperty.Register("LinesThickness",             typeof(Double),                                     typeof(DataGrid), new PropertyMetadata(1.0,                        OnLinesThicknessChanged));
        public static readonly DependencyProperty LinesBrushProperty                 = DependencyProperty.Register("LinesBrush",                 typeof(Brush),                                      typeof(DataGrid), new PropertyMetadata(null,                       OnLinesBrushChanged));
        public static readonly DependencyProperty RowBackgroundProperty              = DependencyProperty.Register("RowBackground",              typeof(Brush),                                      typeof(DataGrid), new PropertyMetadata(null,                       OnRowBackgroundChanged));
        public static readonly DependencyProperty RowBackgroundSelectorProperty      = DependencyProperty.Register("RowBackgroundSelector",      typeof(IValueConverter),                            typeof(DataGrid), new PropertyMetadata(null,                       OnRowBackgroundSelectorChanged));
        public static readonly DependencyProperty SelectionModeProperty              = DependencyProperty.Register("SelectionMode",              typeof(ListViewSelectionMode),                      typeof(DataGrid), new PropertyMetadata(ListViewSelectionMode.None, OnSelectionModeChanged));
        public static readonly DependencyProperty SelectedItemProperty               = DependencyProperty.Register("SelectedItem",               typeof(Object),                                     typeof(DataGrid), new PropertyMetadata(null,                       OnSelectedItemChanged));
        public static readonly DependencyProperty CellTappedCommandProperty          = DependencyProperty.Register("CellTappedCommand",          typeof(ICommand),                                   typeof(DataGrid), new PropertyMetadata(null,                       OnCellTappedCommandChanged));
        public static readonly DependencyProperty CellRightTappedCommandProperty     = DependencyProperty.Register("CellRightTappedCommand",     typeof(ICommand),                                   typeof(DataGrid), new PropertyMetadata(null,                       OnCellRightTappedCommandChanged));
        public static readonly DependencyProperty ScrollFunctionProperty             = DependencyProperty.Register("ScrollFunction",             typeof(Action<Object>),                             typeof(DataGrid), new PropertyMetadata(null,                       OnScrollFunctionChanged));
        public static readonly DependencyProperty SetCanColumnSortFunctionProperty   = DependencyProperty.Register("SetCanColumnSortFunction",   typeof(Action<String, Boolean>),                    typeof(DataGrid), new PropertyMetadata(null,                       OnSetCanColumnSortFunctionChanged));
        public static readonly DependencyProperty SetIsColumnVisibleFunctionProperty = DependencyProperty.Register("SetIsColumnVisibleFunction", typeof(Action<String, Boolean>),                    typeof(DataGrid), new PropertyMetadata(null,                       OnSetIsColumnVisibleFunctionChanged));
        public static readonly DependencyProperty ColumnsProperty                    = DependencyProperty.Register("Columns",                    typeof(DependencyObjectCollection<DataGridColumn>), typeof(DataGrid), new PropertyMetadata(null,                       OnColumnsChanged));

        public static Object GetItemsSource(DependencyObject obj) { return (Object)obj.GetValue(ItemsSourceProperty); }
        public static void SetItemsSource(DependencyObject obj, Object value) { obj.SetValue(ItemsSourceProperty, value); }

        public static Pair<String, Boolean> GetSortColumn(DependencyObject obj) { return (Pair<String, Boolean>)obj.GetValue(SortColumnProperty); }
        public static void SetSortColumn(DependencyObject obj, Pair<String, Boolean> value) { obj.SetValue(SortColumnProperty, value); }

        public static Double GetHeaderHeight(DependencyObject obj) { return (Double)obj.GetValue(HeaderHeightProperty); }
        public static void SetHeaderHeight(DependencyObject obj, Double value) { obj.SetValue(HeaderHeightProperty, value); }

        public static Double GetRowHeight(DependencyObject obj) { return (Double)obj.GetValue(RowHeightProperty); }
        public static void SetRowHeight(DependencyObject obj, Double value) { obj.SetValue(RowHeightProperty, value); }

        public static Double GetLinesThickness(DependencyObject obj) { return (Double)obj.GetValue(LinesThicknessProperty); }
        public static void SetLinesThickness(DependencyObject obj, Double value) { obj.SetValue(LinesThicknessProperty, value); }

        public static Brush GetLinesBrush(DependencyObject obj) { return (Brush)obj.GetValue(LinesBrushProperty); }
        public static void SetLinesBrush(DependencyObject obj, Brush value) { obj.SetValue(LinesBrushProperty, value); }

        public static Brush GetRowBackground(DependencyObject obj) { return (Brush)obj.GetValue(RowBackgroundProperty); }
        public static void SetRowBackground(DependencyObject obj, Brush value) { obj.SetValue(RowBackgroundProperty, value); }

        public static IValueConverter GetRowBackgroundSelector(DependencyObject obj) { return (IValueConverter)obj.GetValue(RowBackgroundSelectorProperty); }
        public static void SetRowBackgroundSelector(DependencyObject obj, IValueConverter value) { obj.SetValue(RowBackgroundSelectorProperty, value); }

        public static ListViewSelectionMode GetSelectionMode(DependencyObject obj) { return (ListViewSelectionMode)obj.GetValue(SelectionModeProperty); }
        public static void SetSelectionMode(DependencyObject obj, ListViewSelectionMode value) { obj.SetValue(SelectionModeProperty, value); }

        public static Object GetSelectedItem(DependencyObject obj) { return (Object)obj.GetValue(SelectedItemProperty); }
        public static void SetSelectedItem(DependencyObject obj, Object value) { obj.SetValue(SelectedItemProperty, value); }

        public static ICommand GetCellTappedCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CellTappedCommandProperty); }
        public static void SetCellTappedCommand(DependencyObject obj, ICommand value) { obj.SetValue(CellTappedCommandProperty, value); }

        public static ICommand GetCellRightTappedCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CellRightTappedCommandProperty); }
        public static void SetCellRightTappedCommand(DependencyObject obj, ICommand value) { obj.SetValue(CellRightTappedCommandProperty, value); }

        public static Action<Object> GetScrollFunction(DependencyObject obj) { return (Action<Object>)obj.GetValue(ScrollFunctionProperty); }
        public static void SetScrollFunction(DependencyObject obj, Action<Object> value) { obj.SetValue(ScrollFunctionProperty, value); }

        public static Action<String, Boolean> GetSetCanColumnSortFunction(DependencyObject obj) { return (Action<String, Boolean>)obj.GetValue(SetCanColumnSortFunctionProperty); }
        public static void SetSetCanColumnSortFunctionFunction(DependencyObject obj, Action<String, Boolean> value) { obj.SetValue(SetCanColumnSortFunctionProperty, value); }

        public static Action<String, Boolean> GetSetIsColumnVisibleFunction(DependencyObject obj) { return (Action<String, Boolean>)obj.GetValue(SetIsColumnVisibleFunctionProperty); }
        public static void SetSetIsColumnVisibleFunction(DependencyObject obj, Action<String, Boolean> value) { obj.SetValue(SetIsColumnVisibleFunctionProperty, value); }

        public static DependencyObjectCollection<DataGridColumn> GetColumns(DependencyObject obj) { return (DependencyObjectCollection<DataGridColumn>)obj.GetValue(ColumnsProperty); }
        public static void SetColumns(DependencyObject obj, DependencyObjectCollection<DataGridColumn> value) { obj.SetValue(ColumnsProperty, value); }

        public Object                                     ItemsSource                { get { return (Object)GetValue(ItemsSourceProperty);                                 } set { SetValue(ItemsSourceProperty,                value); } }
        public Pair<String, Boolean>                      SortColumn                 { get { return (Pair<String, Boolean>)GetValue(SortColumnProperty);                   } set { SetValue(SortColumnProperty,                 value); } }
        public Double                                     HeaderHeight               { get { return (Double)GetValue(HeaderHeightProperty);                                } set { SetValue(HeaderHeightProperty,               value); } }
        public Double                                     RowHeight                  { get { return (Double)GetValue(RowHeightProperty);                                   } set { SetValue(RowHeightProperty,                  value); } }
        public Double                                     LinesThickness             { get { return (Double)GetValue(LinesThicknessProperty);                              } set { SetValue(LinesThicknessProperty,             value); } }
        public Brush                                      LinesBrush                 { get { return (Brush)GetValue(LinesBrushProperty);                                   } set { SetValue(LinesBrushProperty,                 value); } }
        public Brush                                      RowBackground              { get { return (Brush)GetValue(RowBackgroundProperty);                                } set { SetValue(RowBackgroundProperty,              value); } }
        public IValueConverter                            RowBackgroundSelector      { get { return (IValueConverter)GetValue(RowBackgroundSelectorProperty);              } set { SetValue(RowBackgroundSelectorProperty,      value); } }
        public ListViewSelectionMode                      SelectionMode              { get { return (ListViewSelectionMode)GetValue(SelectionModeProperty);                } set { SetValue(SelectionModeProperty,              value); } }
        public Object                                     SelectedItem               { get { return (Object)GetValue(SelectedItemProperty);                                } set { SetValue(SelectedItemProperty,               value); } }
        public ICommand                                   CellTappedCommand          { get { return (ICommand)GetValue(CellTappedCommandProperty);                         } set { SetValue(CellTappedCommandProperty,          value); } }
        public ICommand                                   CellRightTappedCommand     { get { return (ICommand)GetValue(CellRightTappedCommandProperty);                    } set { SetValue(CellRightTappedCommandProperty,     value); } }
        public Action<Object>                             ScrollFunction             { get { return (Action<Object>)GetValue(ScrollFunctionProperty);                      } set { SetValue(ScrollFunctionProperty,             value); } }
        public Action<String, Boolean>                    SetCanColumnSortFunction   { get { return (Action<String, Boolean>)GetValue(SetCanColumnSortFunctionProperty);   } set { SetValue(SetCanColumnSortFunctionProperty,   value); } }
        public Action<String, Boolean>                    SetIsColumnVisibleFunction { get { return (Action<String, Boolean>)GetValue(SetIsColumnVisibleFunctionProperty); } set { SetValue(SetIsColumnVisibleFunctionProperty, value); } }
        public DependencyObjectCollection<DataGridColumn> Columns                    { get { return (DependencyObjectCollection<DataGridColumn>)GetValue(ColumnsProperty); } set { SetValue(ColumnsProperty,                    value); } }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((DataGrid)obj).OnItemsSourceChanged(args); }
        private static void OnSortColumnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((DataGrid)obj).OnSortColumnChanged(args); }
        private static void OnHeaderHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnRowHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnLinesThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnLinesBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnRowBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnRowBackgroundSelectorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnSelectionModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) { ((DataGrid)obj).OnSelectedItemChanged(args); }
        private static void OnCellTappedCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnCellRightTappedCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnScrollFunctionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnSetCanColumnSortFunctionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnSetIsColumnVisibleFunctionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}
        private static void OnColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {}



        public DataGrid() {
            this.DefaultStyleKey = typeof(DataGrid);

            Columns = new DependencyObjectCollection<DataGridColumn>();
        }



        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _columns = new List<DataGridColumn>();

            foreach(DataGridColumn dataGridColumn in Columns) {
                if(dataGridColumn.HeaderItemsSource is ICollection) {
                    Int32 c = ((ICollection)dataGridColumn.HeaderItemsSource).Count;
                    Int32 i = 0;

                    foreach(Object item in (ICollection)dataGridColumn.HeaderItemsSource) {
                        DataGridColumn newDataGridColumn = new DataGridColumn {
                            Style                            = dataGridColumn.Style,
                            ColumnName                       = String.Format("{0}{1}", dataGridColumn.ColumnName, i),
                            SizeMode                         = dataGridColumn.SizeMode,
                            Width                            = dataGridColumn.Width,
                            IsVisible                        = dataGridColumn.IsVisible,
                            CanSort                          = dataGridColumn.CanSort,
                            IsDefaultSortDirectionDescending = dataGridColumn.IsDefaultSortDirectionDescending,
                            SortDirection                    = dataGridColumn.SortDirection,
                            Text                             = dataGridColumn.Text,
                            LocString                        = dataGridColumn.LocString,
                            HeaderStyle                      = dataGridColumn.HeaderStyle,
                            CellContentTemplate              = dataGridColumn.CellContentTemplate,
                            ChildPropertyPath                = dataGridColumn.ChildPropertyPath,

                            Item                             = item,
                            Index                            = i,
                            Count                            = c,
                        };

                        newDataGridColumn.IsVisibleChanged += OnDataGridColumnIsVisibleChanged;

                        _columns.Add(newDataGridColumn);

                        ++i;
                    }
                }
                else {
                    dataGridColumn.Item  = dataGridColumn.HeaderItemsSource;
                    dataGridColumn.Index = 0;
                    dataGridColumn.Count = 1;

                    dataGridColumn.IsVisibleChanged += OnDataGridColumnIsVisibleChanged;

                    _columns.Add(dataGridColumn);
                }
            }

            _gridRoot   = (Grid)GetTemplateChild("GridRoot");
            _gridHeader = (Grid)GetTemplateChild("GridHeader");
            _listView   = (ListView)GetTemplateChild("ListView");

            _gridHeader.RowDefinitions.Add(new RowDefinition { Height = new GridLength(HeaderHeight), });

            for(Int32 i = 0; i < _columns.Count; ++i) {
                DataGridColumn column = _columns[i];

                _gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0), });

                DataGridColumnHeaderItem columnHeaderItem = new DataGridColumnHeaderItem();

                Helpers.ApplyStyle(columnHeaderItem, column.HeaderStyle);

                columnHeaderItem.SetValue(Grid.RowProperty,    0);
                columnHeaderItem.SetValue(Grid.ColumnProperty, i);

                columnHeaderItem.DataContext = new DataGridCellDataContext {
                    Parent = this.DataContext,
                    Row    = null,
                    Column = column,
                    Child  = null,
                };

                columnHeaderItem.Tapped += OnDataGridColumnHeaderItemTapped;

                _gridHeader.Children.Add(columnHeaderItem);
            }

            OnDataGridColumnIsVisibleChanged();

            SynchronizeSortColumn();

            SynchronizeItemsSource(NotifyCollectionChangedAction.Reset, null, null, 0);

            SynchronizeSelectedItems(true);

            SetCanColumnSortFunction = (columnName, canSort) => {
                _columns.Find(item => item.ColumnName == columnName).CanSort = canSort;
            };

            SetIsColumnVisibleFunction = (columnName, isVisible) => {
                _columns.Find(item => item.ColumnName == columnName).IsVisible = isVisible;
            };

            _listView.SelectionChanged += OnListViewSelectionChanged;

            _listView.Loaded += (sender, args) => {
                ScrollFunction =
                    item => {
                        if(item != null) {
                            ObservableCollectionEx<DataGridRow> dstItemsSource = (ObservableCollectionEx<DataGridRow>)_listView.ItemsSource;

                            _listView.UpdateLayout();

                            _listView.ScrollIntoView(dstItemsSource.First(dataGridRow => ReferenceEquals(dataGridRow.Item, item)), ScrollIntoViewAlignment.Leading);
                        }
                    };
            };
        }

        private void OnDataGridColumnIsVisibleChanged(Object sender, EventArgs args) {
            OnDataGridColumnIsVisibleChanged();
        }

        private void OnDataGridColumnIsVisibleChanged() {
            Int32 c = _columns.Count(item => item.IsVisible);

            Int32 j = 0;

            for(Int32 i = 0; i < _columns.Count; ++i) {
                DataGridColumn           column   = _columns[i];
                ColumnDefinition         columnDefinition = _gridHeader.ColumnDefinitions[i];
                DataGridColumnHeaderItem columnHeaderItem = (DataGridColumnHeaderItem)_gridHeader.Children[i];

                if(column.IsVisible) {
                    GridLength gridLength =
                        column.SizeMode ==
                            DataGridColumnSizeMode.Stretch ? new GridLength(1, GridUnitType.Star) 
                                                           : new GridLength(column.Width);

                    columnDefinition.Width = gridLength;

                    columnHeaderItem.BorderThickness = new Thickness(
                        j > 0     ? LinesThickness : 0,
                        0,
                        j < c - 1 ? LinesThickness : 0,
                        0
                    );

                    columnHeaderItem.BorderBrush = (SolidColorBrush)Application.Current.Resources["DataGridColumnHeaderSeparatorBrush"];

                    ++j;
                }
                else {
                    columnDefinition.Width = new GridLength(0);

                    columnHeaderItem.BorderThickness = new Thickness(0, 0, 0, 0);
                    columnHeaderItem.BorderBrush     = null;
                }
            }
        }

        private void OnDataGridColumnHeaderItemTapped(Object sender, TappedRoutedEventArgs args) {
            DataGridColumnHeaderItem dataGridColumnHeaderItem = (DataGridColumnHeaderItem)sender;

            DataGridColumn dataGridColumn = ((DataGridCellDataContext)dataGridColumnHeaderItem.DataContext).Column;

            if(dataGridColumn.CanSort) {
                Boolean ascending = (SortColumn == null || SortColumn.First != dataGridColumn.ColumnName ? !dataGridColumn.IsDefaultSortDirectionDescending                         :
                                                                                                           (dataGridColumn.SortDirection == DataGridColumnSortDirection.Descending) );

                SortColumn = new Pair<String, Boolean>(dataGridColumn.ColumnName, ascending);
            }
        }

        private void OnSortColumnChanged(DependencyPropertyChangedEventArgs args) {
            SynchronizeSortColumn();
        }

        private void SynchronizeSortColumn() {
            if(_columns == null) {
                return;
            }

            foreach(DataGridColumn dataGridColumn in _columns) {
                if(SortColumn != null) {
                    if(dataGridColumn.ColumnName == SortColumn.First) {
                        if(SortColumn.Second) {
                            dataGridColumn.SortDirection = DataGridColumnSortDirection.Ascending;
                        }
                        else {
                            dataGridColumn.SortDirection = DataGridColumnSortDirection.Descending;
                        }
                    }
                    else {
                        dataGridColumn.SortDirection = DataGridColumnSortDirection.None;
                    }
                }
                else {
                    dataGridColumn.SortDirection = DataGridColumnSortDirection.None;
                }
            }
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args) {
            if(args.OldValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)args.OldValue).CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            if(args.NewValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)args.NewValue).CollectionChanged += OnItemsSourceCollectionChanged;
            }

            SynchronizeItemsSource(NotifyCollectionChangedAction.Reset, null, null, 0);
        }

        private void OnItemsSourceCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args) {
            SynchronizeItemsSource(args.Action, args.OldItems, args.NewItems, args.NewStartingIndex);
        }

        private void SynchronizeItemsSource(NotifyCollectionChangedAction action, IList oldItems, IList newItems, Int32 newStartingIndex) {
            if(_listView == null) {
                return;
            }

            if(ItemsSource == null) {
                _listView.ItemsSource = null;

                return;
            }

            if(_listView.ItemsSource == null) {
                _listView.ItemsSource = new ObservableCollectionEx<DataGridRow>();
            }

            Func<Object, DataGridRow> newDataGridRow = item =>
                new DataGridRow {
                    Height                = RowHeight,
                    LinesThickness        = LinesThickness,
                    LinesBrush            = LinesBrush,
                    RowBackground         = RowBackground,
                    RowBackgroundSelector = RowBackgroundSelector,
                    Columns               = _columns,
                    IsSelected            = false,
                    Item                  = item,
                    Parent                = this.DataContext,
                    OnCellTapped          = OnCellTapped,
                    OnCellRightTapped     = OnCellRightTapped,
                };

            IEnumerable<Object>                 srcItemsSource = (IEnumerable<Object>)ItemsSource;
            ObservableCollectionEx<DataGridRow> dstItemsSource = (ObservableCollectionEx<DataGridRow>)_listView.ItemsSource;

            if(action == NotifyCollectionChangedAction.Reset) {
                List<DataGridRow> newDstItems = new List<DataGridRow>();

                foreach(Object srcItem in srcItemsSource) {
                    newDstItems.Add(newDataGridRow(srcItem));
                }

                dstItemsSource.Reset(newDstItems);
            }
            else if(action == NotifyCollectionChangedAction.Remove) {
                if(oldItems != null) {
                    foreach(Object srcItem in oldItems) {
                        dstItemsSource.Remove(dstItemsSource.FirstOrDefault(item => ReferenceEquals(item.Item, srcItem)));
                    }
                }
            }
            else if(action == NotifyCollectionChangedAction.Add) {
                if(newItems != null) {
                    foreach(Object srcItem in newItems) {
                        dstItemsSource.Insert(newStartingIndex++, newDataGridRow(srcItem));
                    }
                }
            }
            else if(action == NotifyCollectionChangedAction.Move) {
                throw new NotImplementedException();
            }
            else if(action == NotifyCollectionChangedAction.Replace) {
                throw new NotImplementedException();
            }
            else {
                throw new Exception(String.Format("DataGrid.SynchronizeItemsSource: unknown action {0}", action));
            }

            // http://stackoverflow.com/questions/1256793/mvvm-sync-collections
        }

        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args) {
            SynchronizeSelectedItems(true);
        }

        private void OnListViewSelectionChanged(Object sender, SelectionChangedEventArgs args) {
            SynchronizeSelectedItems(false);
        }

        private void SynchronizeSelectedItems(Boolean fromCode) {
            if(_isSynchronizingSelectedItems) {
                return;
            }

            _isSynchronizingSelectedItems = true;

            if(_listView != null) {
                if(_listView.ItemsSource != null) {
                    ObservableCollectionEx<DataGridRow> dstItemsSource = (ObservableCollectionEx<DataGridRow>)_listView.ItemsSource;

                    if(fromCode) {
                        _listView.SelectedItem = dstItemsSource.FirstOrDefault(item => ReferenceEquals(item.Item, SelectedItem));
                    }
                    else {
                        SelectedItem = ((DataGridRow)_listView.SelectedItem)?.Item;
                    }

                    foreach(DataGridRow dataGridRow in dstItemsSource) {
                        dataGridRow.IsSelected = (
                            (ReferenceEquals(_listView.SelectedItem, dataGridRow))                                                                                           ||
                            (SelectionMode == ListViewSelectionMode.Multiple && _listView.SelectedItems.FirstOrDefault(item => ReferenceEquals(item, dataGridRow)) != null)
                        );
                    }
                }
            }

            _isSynchronizingSelectedItems = false;
        }

        private void OnCellTapped(DataGridRow dataGridRow, DataGridColumn dataGridColumn) {
            CellTappedCommand?.Execute(new Pair<Object, String>(dataGridRow.Item, dataGridColumn.ColumnName));
        }

        private void OnCellRightTapped(DataGridRow dataGridRow, DataGridColumn dataGridColumn) {
            CellRightTappedCommand?.Execute(new Pair<Object, String>(dataGridRow.Item, dataGridColumn.ColumnName));
        }



        private List<DataGridColumn> _columns;
        private Grid                 _gridRoot;
        private Grid                 _gridHeader;
        private ListView             _listView;
        private Boolean              _isSynchronizingSelectedItems;
    }

    sealed class DataGridColumnHeaderItem : ContentControl {
        public DataGridColumnHeaderItem() {
            this.DefaultStyleKey = typeof(DataGridColumnHeaderItem);
        }



        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _contentPresenter = (ContentPresenter)GetTemplateChild("ContentPresenter");
        }



        private ContentPresenter _contentPresenter;
    }

    sealed class DataGridRowItem : ContentControl {
        public DataGridRowItem() {
            this.DefaultStyleKey = typeof(DataGridRowItem);
        }

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _gridRoot    = (Grid)GetTemplateChild("GridRoot");
            _gridContent = (Grid)GetTemplateChild("GridContent");

            this.DataContextChanged += OnDataContextChanged;

            _gridRoot.PointerEntered     += OnPointerEntered;
            _gridRoot.PointerExited      += OnPointerExited;
            _gridRoot.PointerPressed     += OnPointerPressed;
            _gridRoot.PointerReleased    += OnPointerReleased;
            _gridRoot.PointerCanceled    += OnPointerCanceled;
            _gridRoot.PointerCaptureLost += OnPointerCaptureLost;
            _gridRoot.Tapped             += OnTapped;
            _gridRoot.RightTapped        += OnRightTapped;
            _gridRoot.Holding            += OnHolding;

            UpdateDataContext();

            OnDataGridColumnIsVisibleChanged();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            if(_dataGridRow != null) {
                foreach(DataGridColumn dataGridColumn in _dataGridRow.Columns) {
                    dataGridColumn.IsVisibleChanged -= OnDataGridColumnIsVisibleChanged;
                }
            }

            _dataGridRow = (DataGridRow)this.DataContext;

            if(_dataGridRow != null) {
                foreach(DataGridColumn dataGridColumn in _dataGridRow.Columns) {
                    dataGridColumn.IsVisibleChanged += OnDataGridColumnIsVisibleChanged;
                }
            }

            UpdateDataContext();

            OnDataGridColumnIsVisibleChanged();
        }

        private void UpdateDataContext() {
            if(_isInitialized) {
                for(Int32 i = 0; i < _gridContent.Children.Count; ++i) {
                    FrameworkElement cellContent = (FrameworkElement)_gridContent.Children[i];

                    UpdateCellDataContext(i, cellContent);
                }
            }
            else {
                if(_dataGridRow != null) {
                    _gridContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_dataGridRow.Height), });

                    for(Int32 i = 0; i < _dataGridRow.Columns.Count; ++i) {
                        DataGridColumn dataGridColumn = _dataGridRow.Columns[i];

                        _gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0), });

                        Grid cellContent = new Grid();

                        cellContent.Children.Add((UIElement)dataGridColumn.CellContentTemplate.LoadContent());

                        if(_dataGridRow.LinesThickness > 0) {
                            Rectangle rect = new Rectangle();

                            rect.StrokeThickness = _dataGridRow.LinesThickness;
                            rect.Stroke          = _dataGridRow.LinesBrush;

                            cellContent.Children.Add(rect);
                        }

                        UpdateCellDataContext(i, cellContent);

                        cellContent.SetValue(Grid.RowProperty,    0);
                        cellContent.SetValue(Grid.ColumnProperty, i);

                        _gridContent.Children.Add(cellContent);
                    }

                    _isInitialized = true;
                }
            }

            if(_isInitialized) {
                if(_dataGridRow == null) {
                    _gridContent.Background = null;
                }
                else {
                    if(_dataGridRow.RowBackgroundSelector != null) {
                        _gridContent.Background = (Brush)_dataGridRow.RowBackgroundSelector.Convert(_dataGridRow.Item, typeof(Brush), null, String.Empty);
                    }
                    else {
                        _gridContent.Background = _dataGridRow.RowBackground;
                    }
                }
            }
        }

        private void UpdateCellDataContext(Int32 cellIndex, FrameworkElement cellContent) {
            if(_dataGridRow == null) {
                cellContent.DataContext = null;
            }
            else {
                Object         row    = _dataGridRow.Item;
                DataGridColumn column = _dataGridRow.Columns[cellIndex];
                Object         child  = null;

                if(column.ChildPropertyPath != null) {
                    Object value = null;

                    if(String.IsNullOrEmpty(column.ChildPropertyPath)) {
                        value = row;
                    }
                    else {
                        value = Helpers.GetValue(row, column.ChildPropertyPath);
                    }

                    if(value is IEnumerable) {
                        child = ((IEnumerable)value).ElementAt(column.Index);
                    }
                    else {
                        child = value;
                    }
                }

                cellContent.DataContext = new DataGridCellDataContext {
                    Parent = _dataGridRow.Parent,
                    Row    = row,
                    Column = column,
                    Child  = child,
                };
            }
        }

        private void OnDataGridColumnIsVisibleChanged(Object sender, EventArgs args) {
            OnDataGridColumnIsVisibleChanged();
        }

        private void OnDataGridColumnIsVisibleChanged() {
            if(_dataGridRow != null) {
                for(Int32 i = 0; i < _dataGridRow.Columns.Count; ++i) {
                    DataGridColumn dataGridColumn = _dataGridRow.Columns[i];

                    GridLength gridLength = 
                        dataGridColumn.IsVisible ? dataGridColumn.SizeMode == DataGridColumnSizeMode.Stretch ? new GridLength(1, GridUnitType.Star) :
                                                                                                               new GridLength(dataGridColumn.Width)
                                                 : new GridLength(0);

                    _gridContent.ColumnDefinitions[i].Width = gridLength;
                }
            }
        }
        
        private void OnPointerEntered(Object sender, PointerRoutedEventArgs args) {
            if(_dataGridRow != null) {
                _dataGridRow.IsPointerOver = true;
            }
        }

        private void OnPointerExited(Object sender, PointerRoutedEventArgs args) {
            if(_dataGridRow != null) {
                _dataGridRow.IsPointerOver    = false;
                _dataGridRow.IsPointerPressed = false;
            }
        }

        private void OnPointerPressed(Object sender, PointerRoutedEventArgs args) {
            if(_dataGridRow != null) {
                _dataGridRow.IsPointerPressed = true;
            }
        }

        private void OnPointerReleased(Object sender, PointerRoutedEventArgs args) {
            if(_dataGridRow != null) {
                _dataGridRow.IsPointerPressed = false;
            }
        }

        private void OnPointerCaptureLost(Object sender, PointerRoutedEventArgs args) {
            if(_dataGridRow != null) {
                _dataGridRow.IsPointerPressed = false;
                _dataGridRow.IsPointerOver    = false;
            }
        }

        private void OnPointerCanceled(Object sender, PointerRoutedEventArgs args) {
        }

        private void OnTapped(Object sender, TappedRoutedEventArgs args) {
            if(_dataGridRow != null) {
                Point point = args.GetPosition(_gridRoot);

                for(Int32 i = 0; i < _gridContent.Children.Count; ++i) {
                    UIElement cellContent = _gridContent.Children[i];

                    Rect rect = cellContent.TransformToVisual(_gridRoot).TransformBounds(new Rect(0, 0, cellContent.RenderSize.Width, cellContent.RenderSize.Height));

                    if(rect.Contains(point)) {
                        DataGridColumn dataGridColumn = _dataGridRow.Columns[i];

                        _dataGridRow.OnCellTapped.Invoke(_dataGridRow, dataGridColumn);

                        break;
                    }
                }
            }
        }

        private void OnRightTapped(Object sender, RightTappedRoutedEventArgs args) {
            if(_dataGridRow != null) {
                Point point = args.GetPosition(_gridRoot);

                for(Int32 i = 0; i < _gridContent.Children.Count; ++i) {
                    UIElement cellContent = _gridContent.Children[i];

                    Rect rect = cellContent.TransformToVisual(_gridRoot).TransformBounds(new Rect(0, 0, cellContent.RenderSize.Width, cellContent.RenderSize.Height));

                    if(rect.Contains(point)) {
                        DataGridColumn dataGridColumn = _dataGridRow.Columns[i];

                        _dataGridRow.OnCellRightTapped.Invoke(_dataGridRow, dataGridColumn);

                        break;
                    }
                }
            }
        }

        private void OnHolding(Object sender, HoldingRoutedEventArgs args) {
        }



        private Grid        _gridRoot;
        private Grid        _gridContent;
        private Boolean     _isInitialized;
        private DataGridRow _dataGridRow;
    }
    
    sealed class DataGridCellDataContext :ViewModelBase {
        public Object         Parent { get; set; }
        public Object         Row    { get; set; }
        public DataGridColumn Column { get; set; }
        public Object         Child  { get { return _child; } set { _child = value; RaisePropertyChanged(); } }

        private Object _child;
    }

}
