using System;
using System.Globalization;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class CreateBankChequeChildWindow
    {
        private readonly BankViewModel _viewModel;

        public CreateBankChequeChildWindow(BankViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            var glclient = new GlServiceClient();
            glclient.GetMaxChequeAsync(_viewModel.SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
            glclient.GetMaxChequeCompleted += (s, sv) =>
            {
                try
                {
                    TxtFrom.Text = sv.Result.ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    TxtFrom.Text = "1";
                }
            };
        }

        private void CreateOkButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateCheck(Convert.ToInt64(TxtFrom.Text), Convert.ToInt32(TxtQty.Text));
            DialogResult = false;
        }

        private void CreateCance_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}