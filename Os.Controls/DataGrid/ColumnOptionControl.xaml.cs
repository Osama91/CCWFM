using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Os.Controls.DataGrid
{
    /// <summary>
    /// This control is added before the HeaderContent in the ColumHeader.  It provides the ability to group and pin the columns
    /// </summary>
    public partial class ColumnOptionControl : INotifyPropertyChanged
    {
        private readonly ColumnOptionItem _addPin = new ColumnOptionItem(Enums.ColumnOption.PinColumn, "Pin Column", "/Os.Controls;component/Images/PinUp.png");
        private readonly ColumnOptionItem _addGroup = new ColumnOptionItem(Enums.ColumnOption.AddGrouping, "Add Grouping", "/Os.Controls;component/Images/GroupBy.png");
        private readonly ColumnOptionItem _removePin = new ColumnOptionItem(Enums.ColumnOption.UnpinColumn, "Unpin Column", "/Os.Controls;component/Images/pinDown.png");
        private readonly ColumnOptionItem _removeGroup = new ColumnOptionItem(Enums.ColumnOption.RemoveGrouping, "Remove Grouping", "/Os.Controls;component/Images/RemoveGroupBy.png");

        private bool _canGroup = true;

        public bool CanGroup
        {
            get { return _canGroup; }
            set
            {
                if (value != _canGroup)
                {
                    _canGroup = value;
                    OnPropertyChanged("CanGroup");
                    SetOptions();
                }
            }
        }

        private bool _canPin = true;

        public bool CanPin
        {
            get { return _canPin; }
            set
            {
                if (value != _canPin)
                {
                    _canPin = value;
                    OnPropertyChanged("CanPin");
                    SetOptions();
                }
            }
        }

        private bool _isGrouped;

        public bool IsGrouped
        {
            get { return _isGrouped; }
            set
            {
                if (value != _isGrouped)
                {
                    _isGrouped = value;
                    OnPropertyChanged("IsGrouped");
                    SetOptions();
                }
            }
        }

        private bool _isPinned;

        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                if (value != _isPinned)
                {
                    _isPinned = value;
                    OnPropertyChanged("IsPinned");
                    SetOptions();
                }
            }
        }

        private void SetOptions()
        {
            ColumnOptions.Clear();
            if (CanPin)
            {
                if (IsPinned)
                    ColumnOptions.Add(_removePin);
                else
                    ColumnOptions.Add(_addPin);
            }
            if (CanGroup)
            {
                if (IsGrouped)
                    ColumnOptions.Add(_removeGroup);
                else
                    ColumnOptions.Add(_addGroup);
            }
            if (!CanGroup && !CanPin)
                Visibility = Visibility.Collapsed;
        }

        private ColumnOptionItem _selectedColumnOptionItem;

        public ColumnOptionItem SelectedColumnOptionItem
        {
            get { return _selectedColumnOptionItem; }
            set
            {
                if (_selectedColumnOptionItem != value)
                {
                    _selectedColumnOptionItem = value;
                    OnPropertyChanged("SelectedColumnOptionItem");
                    CbOptions.IsDropDownOpen = false;
                }
            }
        }

        public ObservableCollection<ColumnOptionItem> ColumnOptions { get; private set; }

        private FilterColumnInfo _filterColumnInfo;

        public FilterColumnInfo FilterColumnInfo
        {
            get { return _filterColumnInfo; }
            set
            {
                if (value != null && value != _filterColumnInfo)
                {
                    _filterColumnInfo = value;
                }
            }
        }

        private DataGridColumn _column;

        public DataGridColumn Column
        {
            get { return _column; }
            set
            {
                if (value != null && value != _column)
                {
                    _column = value;
                }
            }
        }

        public ColumnOptionControl()
        {
            ColumnOptions = new ObservableCollection<ColumnOptionItem>();
            InitializeComponent();
            ColumnOptions.Add(_addPin);
            ColumnOptions.Add(_addGroup);
            DataContext = this;
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// If a filterColumnInfo is set, the control is visible.  As each ColumnOptionControl is loaded in the grid, the relating column
        /// is used to build FilterColumnInfo for the control.
        /// </summary>
        /// <param name="filterColumnInfo">Binding information about the column</param>
        internal void ResetOptionValues(FilterColumnInfo filterColumnInfo)
        {
            FilterColumnInfo = filterColumnInfo;
            if (!CanGroup && !CanPin)
                Visibility = Visibility.Collapsed;
            else
            {
                if (!string.IsNullOrWhiteSpace(filterColumnInfo.PropertyPath))
                    Visibility = Visibility.Visible;
                else
                    Visibility = Visibility.Collapsed;
            }
        }

        #region IPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion IPropertyChanged
    }
}