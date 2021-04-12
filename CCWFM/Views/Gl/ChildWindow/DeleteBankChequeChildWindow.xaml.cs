using System;
using System.Windows;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class DeleteBankChequeChildWindow
    {
        private readonly BankViewModel _viewModel;

        public DeleteBankChequeChildWindow(BankViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            
        }

        private void DeleteOkButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteCheck(Convert.ToInt32(TxtFrom.Text), Convert.ToInt32(TxtQty.Text));
            this.DialogResult = false;
        }


        
        private void DeleteCance_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}