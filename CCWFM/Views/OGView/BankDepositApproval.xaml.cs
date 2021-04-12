using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class BankDepositApproval
    {
        private readonly BankDepositViewModel _viewModel;

        public BankDepositApproval()
        {
            InitializeComponent();
            _viewModel = (BankDepositViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BrandSection_Click(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;

            if (chk.IsChecked == true)
            {
                _viewModel.SelectedMainRow.Status = 1;
            }
            else
            {
                _viewModel.SelectedMainRow.Status = 0;
            }

            _viewModel.SaveMainRow();
        }
    }
}