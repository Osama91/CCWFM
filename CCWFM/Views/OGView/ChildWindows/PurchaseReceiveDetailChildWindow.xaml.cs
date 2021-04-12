using System.Windows;
using CCWFM.ViewModel.OGViewModels;
using System.Windows.Controls;
using Os.Controls.DataGrid.Events;
using CCWFM.Helpers.AuthenticationHelpers;
using System.Collections.Generic;
using System.Windows.Input;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseReceiveDetailChildWindow
    {
        readonly PurchaseOrderRequestViewModel _viewModel;
        public PurchaseReceiveDetailChildWindow(PurchaseOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
            LayoutRoot.DataContext = viewModel;
            
        }

        private void RecDetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            viewModel.SelectedSubDetailRow.DetailList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.RecDetailFilter = filter;
            viewModel.RecDetailValuesObjects = valueObjecttemp;
            viewModel.GetRecDetail();

        }

        private void RecDetailGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            if (viewModel.SelectedSubDetailRow.DetailList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.SelectedSubDetailRow.DetailList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.GetRecDetail();
            }
        }

        private void RecDetailGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            if (e.Key == Key.Delete)
            {
                viewModel.SelectedRecDetails.Clear();
                foreach (var row in RecDetailGrid.SelectedItems)
                {
                    viewModel.SelectedRecDetails.Add(row as TblPurchaseReceiveDetailModel);
                }
                viewModel.DeleteRecDetail();
            }
        }

        private void RecDetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SaveRecDetailRow();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        //private void OkButton_Click(object sender, RoutedEventArgs e)
        //{        
        //    DialogResult = false;
        //}          
    }
}