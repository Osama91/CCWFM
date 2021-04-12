using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.Views.OGView.SearchChildWindows;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.AttView
{
    public partial class EmployeeBehalf
    {
        private readonly EmployeeBehalfViewModel _viewModel;

        public EmployeeBehalf()
        {
            InitializeComponent();
            _viewModel = (EmployeeBehalfViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            
            _viewModel.EmpViewList.Clear();
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

      
        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRowExc();
        }

    

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var child = new EmployeeChildWindow(_viewModel) {formsEnum = EmployeeChildWindow.FormsEnum.Manager};
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            child.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            child.Show();
        }

        private void SearchOperator_Click(object sender, RoutedEventArgs e)
        {
            var child = new EmployeeChildWindow(_viewModel) {formsEnum = EmployeeChildWindow.FormsEnum.Operator};
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            child.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            child.Show();
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRowExc();
        }
    }
}