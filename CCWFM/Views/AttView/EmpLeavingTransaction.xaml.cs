using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.AttView
{
    public partial class EmpLeavingTransaction
    {
        private readonly EmpLeavingTransactionViewModel _viewModel;

        public EmpLeavingTransaction()
        {
            InitializeComponent();

            _viewModel = (EmpLeavingTransactionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;

        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewMainRow(true);
            }
            else if (e.Key == Key.Delete)
            {
                //_viewModel.SelectedMainRows.Clear();
                //foreach (var row in MainGrid.SelectedItems)
                //{
                //    _viewModel.SelectedMainRows.Add(row as TblEmpLeavingTransactionViewModel);
                //}

                //_viewModel.DeleteMainRow();
            }
        }

        private void AddBttn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.MainRowList.Any())
            {
                _viewModel.AddNewMainRow(MainGrid.SelectedItem != null);
            }
            else
            {
                _viewModel.AddNewMainRow(false);
            }
        }

        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.SelectedMainRows.Clear();
            //foreach (var row in MainGrid.SelectedItems)
            //{
            //    _viewModel.SelectedMainRows.Add(row as TblEmpLeavingTransactionViewModel);
            //}
            //_viewModel.DeleteMainRow();
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }
    }
}