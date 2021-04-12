using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;
using System.Linq;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseOrderRequestPaymentChild
    {

        public PurchaseOrderRequestPaymentChild(PurchaseOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SelectedMainRow.PaymentList.Clear();
            viewModel.GetPaymentData();
            if (!viewModel.SelectedMainRow.PaymentSettings.Any())
            {
                viewModel.SelectedMainRow.PaymentSettings.Add(new PaymentSettingModel()
                {
                    PaymentSettingPerRow = viewModel.SchedulesSettingList.FirstOrDefault(),
                   PaymentScheduleSetting= viewModel.SchedulesSettingList.FirstOrDefault().Iserial,
                    InstallmentCount = 1,
                    InstallmentInterval = 1
                });
                //viewModel.SelectedMainRow.PaymentSettings.Add(new PaymentSettingModel());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SelectedMainRow.PaymentList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.PaymentDetailFilter = filter;
            viewModel.PaymentDetailValuesObjects = valueObjecttemp;
            viewModel.GetPaymentData();

        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            if (viewModel.SelectedMainRow.PaymentList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.SelectedMainRow.PaymentList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;


                viewModel.GetPaymentData();
            }
        }


        private void DgResults_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            if (e.Key == Key.Delete)
            {

                List<PaymentSettingModel> list = DgResults.SelectedItems as List<PaymentSettingModel>;
                foreach (var row in list.ToList())
                {
                    viewModel.SelectedMainRow.PaymentSettings.Remove(row as PaymentSettingModel);
                }
            }
            else
                if (e.Key == Key.Down)
            {
                viewModel.SelectedMainRow.PaymentSettings.Add(new PaymentSettingModel()
                {
                    PaymentSettingPerRow = viewModel.SchedulesSettingList.FirstOrDefault(),
                    PaymentScheduleSetting = viewModel.SchedulesSettingList.FirstOrDefault().Iserial,
                    InstallmentCount = 1,
                    InstallmentInterval = 1
                });
            }
        }


        private void DetailGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            if (e.Key == Key.Delete)
            {
                viewModel.SelectedPaymentRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    viewModel.SelectedPaymentRows.Add(row as TblPurchaseOrderHeaderRequestPaymentModel);
                }
                viewModel.DeletePaymentRow();
            }
            else if (e.Key == Key.Down)
            {
                var currentRowIndex = (viewModel.SelectedMainRow.PaymentList.IndexOf(viewModel.SelectedPaymentRow));
                if (currentRowIndex == (viewModel.SelectedMainRow.PaymentList.Count - 1))
                {
                    viewModel.AddNewPaymentRow(true);
                }
            }
        }
        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SavePaymentRow();
        }


        private void DetailGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            //if (viewModel.SelectedPaymentRow.Status)
            //{
            //    e.Cancel = true;
            //}
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.GeneratePayment();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;
            viewModel.SavePaymentRow();




        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {

            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            var viewModel = (PurchaseOrderRequestViewModel)DataContext;

            viewModel.SelectedMainRow.sumTotal();
        }
    }
}