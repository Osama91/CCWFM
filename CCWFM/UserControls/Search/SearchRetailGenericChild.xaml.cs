using System.Windows;
using CCWFM.CRUDManagerService;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls.Search
{
    public partial class SearchRetailGenericChild
    {
        private readonly SearchRetailGenericViewModel _viewModel = new SearchRetailGenericViewModel();
        private readonly SearchRetailGeneric _userControl;

        public SearchRetailGenericChild(SearchRetailGeneric searchEmployeeUserControl)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _userControl = searchEmployeeUserControl;
            _viewModel.TablEname = searchEmployeeUserControl.TableNamePerRow;
            _viewModel.GetMaindata();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedIndex != -1)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as GenericTable;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
        
        
            _viewModel.MainRowList.Clear();
            _viewModel.Code = _viewModel.Aname = _viewModel.Ename = null;
            foreach (ColumnFilterControl f in e.FiltersPredicate)
            {
                _viewModel.Filter = null;
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.Contains:
                        _viewModel.Filter = "%" + f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.EndsWith:
                        _viewModel.Filter = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        _viewModel.Filter = f.FilterText + "%";
                        break;
                }
                switch (f.FilterColumnInfo.PropertyPath)
                {
                    case "Code":
                        _viewModel.Code = _viewModel.Filter;
                        break;

                    case "Ename":
                        _viewModel.Ename = _viewModel.Filter;
                        break;

                    case "AName":
                        _viewModel.Aname = _viewModel.Filter;
                        break;
                }
            }
            if (!_viewModel.Loading)
            {
                _viewModel.GetMaindata();
            }
        }

        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }
    }
}