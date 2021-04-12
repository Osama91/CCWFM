using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Os.Controls.DataGrid.Events;

namespace Os.Controls.DataGrid
{
    public partial class OsGrid
    {
        #region TurnOffValidationsOnBeginEdit

        private ValidationSummary _validationSummary;
        private bool _validationFilterBehaviorApplied;
        private bool _validationFilterBehaviorRestored;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _validationSummary = (ValidationSummary)GetTemplateChild("ValidationSummary");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                CommitEdit();
              
            }
            if (e.Key == Key.Enter)
            {
                CommitEdit();
             //   e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnBeginningEdit(DataGridBeginningEditEventArgs e)
        {
            
            if (!_validationFilterBehaviorApplied)
            {
                _validationSummary.Filter = ValidationSummaryFilters.None;
                _validationFilterBehaviorApplied = true;
            }

            base.OnBeginningEdit(e);
        }

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            if (_validationFilterBehaviorApplied && !_validationFilterBehaviorRestored)
            {
                _validationSummary.Filter = ValidationSummaryFilters.All;
                _validationFilterBehaviorRestored = true;
            }

            base.OnRowEditEnding(e);
        }

        protected override void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
        {
            _validationFilterBehaviorApplied = false;
            _validationFilterBehaviorRestored = false;

            base.OnRowEditEnded(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            CommitEdit();
            base.OnSelectionChanged(e);
        }

        #endregion TurnOffValidationsOnBeginEdit

        #region FilteredItemsSource DependencyProperty

        public static readonly DependencyProperty FilteredItemsSourceProperty =
                                                                DependencyProperty.Register("FilteredItemsSource", typeof(IEnumerable), typeof(OsGrid),
                                                                new PropertyMetadata(null, OnFilteredItemsSourceChanged));

        public static readonly DependencyProperty IsReadOnlyCustomProperty = DependencyProperty.Register("IsReadOnlyCustom", typeof(bool), typeof(OsGrid), new PropertyMetadata(default(bool)));

        public IEnumerable FilteredItemsSource
        {
            get { return (IEnumerable)GetValue(FilteredItemsSourceProperty); }
            set { SetValue(FilteredItemsSourceProperty, value); }
        }

        public static void OnFilteredItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as OsGrid;
            if (args.NewValue != null)
            {
                var view = new PagedCollectionView((IEnumerable)args.NewValue);
                var srcT = args.NewValue.GetType().GetInterfaces().First(i => i.Name.StartsWith("IEnumerable"));
                if (fil != null)
                {
                    fil.FilterType = srcT.GetGenericArguments().FirstOrDefault();
                    fil.OrginalSource = (IEnumerable)args.NewValue;
                    if (fil.AutoGenerateColumns)
                    {
                        fil.FilterHeaders.Clear();
                        fil.ColumnOptionControls.Clear();
                    }
                    fil.ItemsSource = view;
                }
            }
            if (fil != null) fil.ResetColumnHeaders();
        }

        #endregion FilteredItemsSource DependencyProperty

        public List<ColumnFilterControl> FilterHeaders { get; set; }

        public List<ColumnOptionControl> ColumnOptionControls { get; set; }

        private List<PropertyGroupDescription> ColumnGroups { get; set; }

        [Bindable(false)]
        [Category("Appearance")]
        [DefaultValue("True")]
        public bool CanGroup
        {           
            set
            {
                foreach (var option in ColumnOptionControls)
                    option.CanGroup = value;
            }
        }
        


        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("True")]
        public bool CanPin
        {
            set
            {
                foreach (var option in ColumnOptionControls)
                    option.CanPin = value;
            }
        }

        public Type FilterType { get; set; }

        public IEnumerable OrginalSource { get; set; }

        public bool IsReadOnlyCustom
        {
            get { return (bool)GetValue(IsReadOnlyCustomProperty); }
            set
            {
                SetValue(IsReadOnlyCustomProperty, value);
                IsReadOnly = (bool)GetValue(IsReadOnlyCustomProperty);
            }
        }

        public OsGrid()
        {
            FilterHeaders = new List<ColumnFilterControl>();
            ColumnOptionControls = new List<ColumnOptionControl>();
            ColumnGroups = new List<PropertyGroupDescription>();
            InitializeComponent();
        }

        //Since the ColumnFilterHeader control is defined in the headerStyle for the grid, we can only get a reference to it once it loads.  We
        //then we have to find the column that the ColumnFilterHeader is associated with.  We do this by comparing the Header
        //and HeaderContent for each column.
        private void ColumnFilterHeader_Loaded(object sender, RoutedEventArgs e)
        {
            var header = sender as ColumnFilterControl;
            if (!FilterHeaders.Any())
            {
                if (header != null) header.TxtFilter.Focus();
            }
            FilterHeaders.Add(header);
            if (header != null && header.HeaderContent != null)
            {
                var column = Columns.FirstOrDefault(c => c.Header != null && c.Header.ToString() == header.HeaderContent.ToString());
                
                header.Column = column;
                header.Grid = this;
                header.PropertyChanged += header_PropertyChanged;
                if (FilterType != null)
                    header.ResetFilterValues(CreateFilterColumnInfo(column));
            }
        }

        //Since the ColumnOptionControl control is defined in the headerStyle for the grid, we can only get a reference to it once it loads.
        //We then have to find the column that the ColumnOptionControl associated with.  We do this by comparing the
        //Column Hedaer, and the control Tags property
        private void ColumnOptionControl_Loaded(object sender, RoutedEventArgs e)
        {
            var optionCtrl = sender as ColumnOptionControl;
            ColumnOptionControls.Add(optionCtrl);
            if (optionCtrl != null && optionCtrl.Tag != null)
            {
                var column = Columns.FirstOrDefault(c => c.Header != null && c.Header.ToString() == optionCtrl.Tag.ToString());
                optionCtrl.Column = column;
                optionCtrl.PropertyChanged += optionCtrl_PropertyChanged;
                if (FilterType != null)
                    optionCtrl.ResetOptionValues(CreateFilterColumnInfo(column));
            }
        }

        //If the user selects a column option.  We need to perform the operation
        private void optionCtrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var optionCtrl = sender as ColumnOptionControl;
            if (e.PropertyName == "SelectedColumnOptionItem")
            {
                if (optionCtrl != null)
                    switch (optionCtrl.SelectedColumnOptionItem.ColumnOption)
                    {
                        case Enums.ColumnOption.AddGrouping:
                            optionCtrl.IsGrouped = true;
                            var v = new PagedCollectionView(FilteredItemsSource);

                            var newGroup = new PropertyGroupDescription(optionCtrl.FilterColumnInfo.PropertyPath, optionCtrl.FilterColumnInfo.Converter);
                            ColumnGroups.Add(newGroup);

                            foreach (var item in ColumnGroups)
                            {
                                v.GroupDescriptions.Add(item);
                            }

                            ItemsSource = v;
                            break;

                        case Enums.ColumnOption.RemoveGrouping:
                            optionCtrl.IsGrouped = false;
                            var view = new PagedCollectionView(FilteredItemsSource);
                            var group = ColumnGroups.FirstOrDefault(c => c.PropertyName == optionCtrl.FilterColumnInfo.PropertyPath);
                            if (@group != null)
                            {
                                ColumnGroups.Remove(@group);
                            }
                            foreach (var item in ColumnGroups)
                            {
                                view.GroupDescriptions.Add(item);
                            }
                            ItemsSource = view;
                            break;

                        case Enums.ColumnOption.PinColumn:
                            optionCtrl.IsPinned = true;
                            var frozenCount = FrozenColumnCount;
                            if (optionCtrl.Column.DisplayIndex >= FrozenColumnCount)
                                frozenCount++;
                            optionCtrl.Column.DisplayIndex = 0;
                            FrozenColumnCount = frozenCount;
                            break;

                        case Enums.ColumnOption.UnpinColumn:
                            optionCtrl.IsPinned = false;
                            if (FrozenColumnCount==0)
                            {
                                FrozenColumnCount = 1;
                            }
                            optionCtrl.Column.DisplayIndex = FrozenColumnCount - 1;
                            FrozenColumnCount = FrozenColumnCount - 1;
                            break;
                    }
            }
        }

        //Simple helper method which creates the FilterColumnInfo object for a column.  If a binding
        //is defined for the column we use that.  If that fails we fallback to using the SortMemberPath
        private FilterColumnInfo CreateFilterColumnInfo(DataGridColumn column)
        {
            var col = new FilterColumnInfo();
            var boundColumn = column as DataGridBoundColumn;
            if (column != null)
            {
                if (boundColumn != null && column.SortMemberPath == null)
                {
                    col.PropertyPath = boundColumn.Binding.Path.Path;
                   
                    try
                    {
                        var propertyType = col.PropertyType = FilterType.GetProperty(boundColumn.Binding.Path.Path).PropertyType;

                        if (propertyType != null)
                        {
                            col.PropertyType = propertyType;
                        }
                    }
                    catch (Exception)
                    {
                        var t1 = typeof(String);
                        col.PropertyType = new TypeDelegator(t1);
                    }

                    col.Converter = boundColumn.Binding.Converter;
                    col.ConverterCultureInfo = boundColumn.Binding.ConverterCulture;
                    col.ConverterParameter = boundColumn.Binding.ConverterParameter;
                }
                else if (column.SortMemberPath.Length > 0)
                {
                    col.PropertyPath = column.SortMemberPath;
                    try
                    {
                        var propertyType = FilterType.GetProperty(column.SortMemberPath).PropertyType;
                        if (propertyType != null)
                        {
                            if (propertyType == typeof(DateTime?))
                            {
                                propertyType = typeof(DateTime);
                            }
                            col.PropertyType = propertyType;
                        }
                    }
                    catch (Exception)
                    {
                        var t1 = typeof(String);
                        col.PropertyType = new TypeDelegator(t1);
                    }
                }
            }
            return col;
        }

        public event EventHandler<FilterEvent> OnFilter;

        //Each time any ColumnFilterHeader values change.  We need to generate the new predicate for the PagedCollectionView
        private void header_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = ItemsSource as PagedCollectionView;
            Predicate<object> predicate = null;

            foreach (var filter in FilterHeaders.Where(x => x.HasPredicate))
            {
                predicate = predicate == null
                    ? filter.GeneratePredicate()
                    : predicate.And(filter.GeneratePredicate());
            }
            CommitEdit();
            if (view != null) view.Filter = predicate;
        }

        public void FireFilter()
        {
            OnFilter(this, new FilterEvent { FiltersPredicate = FilterHeaders.Where(x => x.HasPredicate) });
        }

        /// <summary>
        /// Reset the Filter values for every column
        /// </summary>
        public void ResetColumnHeaders()
        {
            foreach (var header in FilterHeaders)
                header.ResetFilterValues(CreateFilterColumnInfo(header.Column));
            foreach (var option in ColumnOptionControls)
                option.ResetOptionValues(CreateFilterColumnInfo(option.Column));
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Space)
            //    this.BeginEdit();
        }
    }
}