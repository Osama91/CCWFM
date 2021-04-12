using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindow
{
    public partial class RecieveSalesIssueChildWindow
    {
        private readonly SalesOrderRequestInvoiceViewModel _viewModel;

        public RecieveSalesIssueChildWindow(SalesOrderRequestInvoiceViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.GetRecieveHeaderListData();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetRecieveDetailData();
            DialogResult = true;
        }
        private void DetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.RecieveHeaderList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailSubFilter = filter;
            _viewModel.DetailSubValuesObjects = valueObjecttemp;
            _viewModel.GetRecieveHeaderListData();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.RecieveHeaderList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.RecieveHeaderList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetRecieveHeaderListData();
            }
        }

        private void Btnchoose_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
            {
                var row = btn.DataContext as TblSalesIssueHeaderModel;
                if (!_viewModel.RecieveHeaderChoosedList.Select(x=>x.DocCode).Contains(row.DocCode))
                {
                    _viewModel.RecieveHeaderChoosedList.Add(row);
                }
            }
        }

        private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
            {
                var row = btn.DataContext as TblSalesIssueHeaderModel;
                
                if (_viewModel.RecieveHeaderChoosedList.Contains(row))
                {
                    _viewModel.RecieveHeaderChoosedList.Remove(row);
                }
            }
        }
     
        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {

            _viewModel.GetRecFromTo();
        }
    }
}