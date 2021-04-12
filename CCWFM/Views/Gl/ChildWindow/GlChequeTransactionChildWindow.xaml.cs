using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.PrintPreviews;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class GlChequeTransactionChildWindow
    {
        private readonly GlChequeTransactionViewModel _viewModel;

        public GlChequeTransactionChildWindow(GlChequeTransactionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Grid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetMaindata();
            }
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.SelectedMainRow = RecieveHeaderDataGrid.SelectedItem as TblGlChequeTransactionHeaderViewModel;
            _viewModel.GetDetailData();
            DialogResult = true;
        }
        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }

        private void BtnprintCheque_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Glclient.PrintCheckAsync((int)_viewModel.SelectedDetailRow.TblBankCheque, LoggedUserInfo.DatabasEname);
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