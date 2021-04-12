using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.AttView
{
    public partial class EmpLeavingTransactioForManagment
    {
        private readonly EmpLeavingTransactionForManagmentViewModel _viewModel;

        public EmpLeavingTransactioForManagment()
        {
            InitializeComponent();

            _viewModel = (EmpLeavingTransactionForManagmentViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;

        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {         
                 if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblEmpLeavingTransactionViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblEmpLeavingTransactionViewModel);
            }
            _viewModel.DeleteMainRow();
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
    }
}