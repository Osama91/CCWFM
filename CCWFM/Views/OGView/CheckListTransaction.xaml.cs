using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class CheckListTransaction
    {
        public CheckListTransaction()
        {
            InitializeComponent();
            var viewModel = (CheckListTransactionViewModel)LayoutRoot.DataContext;
            DataContext = viewModel;
            
        }      
        private void MainCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var checkListTransactionRow = check.DataContext as TblCheckTransactionViewModel;
            checkListTransactionRow.UpdatedAllowed = true;
            
            checkListTransactionRow.Checked = (bool)check.IsChecked;
        }
    }
}