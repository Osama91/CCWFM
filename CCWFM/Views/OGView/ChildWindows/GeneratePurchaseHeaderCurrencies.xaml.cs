using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class  GeneratePurchaseHeaderCurrencies
    {
        private readonly GeneratePurchaseHeaderCurrenciesViewModel _viewModel;

        public GeneratePurchaseHeaderCurrencies(GeneratePurchaseHeaderCurrenciesViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Remove(_viewModel.SelectedMainRow);
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.MainRowList.Remove(_viewModel.SelectedMainRow);
            }
        }
        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }
    }
}