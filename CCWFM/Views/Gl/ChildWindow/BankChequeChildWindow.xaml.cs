using System.Threading;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.PrintPreviews;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class BankChequeChildWindow
    {
        private readonly BankViewModel _viewModel;
        public BankChequeChildWindow(BankViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SelectedMainRow.DetailsList.Clear();           
            viewModel.GetDetailRow();
            _viewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        private void BtnDeleteCheque_Onclick(object sender, RoutedEventArgs e)
        {
            var child = new DeleteBankChequeChildWindow(_viewModel);
            child.Show();
            
        }

        private void BtnprintCheque_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Glclient.PrintCheckAsync(_viewModel.SelectedDetailRow.Iserial,LoggedUserInfo.DatabasEname);
            _viewModel.Glclient.PrintCheckCompleted += (s, sv) =>
            {
                var printingPage = new BarcodePrintPreview(sv.Result, 4, (_viewModel.SelectedMainRow.Code), true);
                var currentUi = Thread.CurrentThread.CurrentUICulture;
                printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                printingPage.Show();  
            };
           
        }
    }
}