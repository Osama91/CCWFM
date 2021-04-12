using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls
{
    public partial class SearchCostCenter
    {
        private readonly SearchCostCenterUserControl _userControl;
        private readonly SearchCostCenterViewModel _viewModel = new SearchCostCenterViewModel();

        public SearchCostCenter(SearchCostCenterUserControl searchCostCenterUserControl)
        {
            InitializeComponent();
            if (searchCostCenterUserControl.JournalAccountType != null)
            {
                _viewModel.CostCenterType = searchCostCenterUserControl.JournalAccountType.Iserial;
            }
            else
            {
                _viewModel.CostCenterType = null;
            }
            if (searchCostCenterUserControl.EntityAccount != null)
            {
                _viewModel.EntityAccount = searchCostCenterUserControl.EntityAccount.Iserial;
            }

            if (searchCostCenterUserControl.EntityAccountType != null)
            {
                _viewModel.EntityAccountType = searchCostCenterUserControl.EntityAccountType.Iserial;
            }
            DataContext = _viewModel;
            _userControl = searchCostCenterUserControl;
            _viewModel.GetMaindata();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _userControl.SearchPerRow = MainGrid.SelectedItem as TblCostCenter;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _userControl.SearchPerRow = null;
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _userControl.SearchPerRow = MainGrid.SelectedItem as TblCostCenter;
            DialogResult = true;
        }
    }
}