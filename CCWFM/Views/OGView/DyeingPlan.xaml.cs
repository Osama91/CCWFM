using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class DyeingPlan
    {
        private readonly DyeingPlanViewModel _dyedPlanViewModel = new DyeingPlanViewModel();
        private readonly DyeingOrderViewModel _dyedOrderViewModel = new DyeingOrderViewModel();

        public DyeingPlan()
        {
            InitializeComponent();
            DataContext = _dyedPlanViewModel;
            SwitchFormMode(FormMode.Standby);
            GeneratedDyedOrderTab.DataContext = _dyedOrderViewModel;
            _dyedOrderViewModel.SubmitSearchAction += DyedOrderViewModel_SubmitSearchAction;
        }

        private void DyedOrderViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        private void StackPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            var panel = (StackPanel)sender;
            _dyedPlanViewModel.SelectedRow = panel.DataContext as TblDyeingPlanViewModel;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.SaveHeader();
        }

        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            // AddNewRow();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var myUri = new Uri(HtmlPage.Document.DocumentUri, string.Format("report/DyeingPlanReport.aspx"));

            HtmlPage.Window.Navigate(myUri);
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (TabControl)sender;
            if (tab.SelectedItem == Summary)
            {
                _dyedPlanViewModel.GenerateSummary();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.GenerateDyeingOrder();
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("You are about to delete a Dyed Order permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _dyedOrderViewModel.DeleteOrder();
            }
        }

        private void btnAddNewMainOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            _dyedOrderViewModel.DyeingOrderMainDetailList.Add(new TblDyeingOrdersMainDetailViewModel
            {
                DyeingProductionOrder = _dyedOrderViewModel.DyeingOrderHeader.DyeingProductionOrder,
                Saved = false,
                TransactionDate = _dyedOrderViewModel.DyeingOrderHeader.TransactionDate,
            });
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _dyedOrderViewModel.SaveOrder();
        }

        private void DtnPostOrderMainDetails_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            _dyedOrderViewModel.PostMainDetail();
        }

        private void btnDeleteOrderMainDetails_Click(object sender, RoutedEventArgs e)
        {
            var r =
           MessageBox
           .Show("You are about to delete a Dyed Order Main Detail permanently!!\nPlease note that this action cannot be undone!"
                   , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                try
                {
                    _dyedOrderViewModel.DeleteMainDetailsOrder();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnAddNewOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            if (_dyedOrderViewModel.SelectedMainDetails.TransactionType == 0)
            {
                _dyedOrderViewModel.SelectedMainDetails.TblDyeingOrderDetails.Add(new TblDyeingOrderDetailsViewModel
                {
                    DyeingProductionOrder = _dyedOrderViewModel.DyeingOrderHeader.DyeingProductionOrder,
                    TransactionType = _dyedOrderViewModel.SelectedMainDetails.TransactionType,
                    TransId = _dyedOrderViewModel.SelectedMainDetails.TransId,
                });
            }
            else
            {
                var childWindow = new RecieveDyedFabric(_dyedOrderViewModel);
                childWindow.Show();
            }
        }

        private void btnDeleteOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            _dyedOrderViewModel.DeleteDetailsOrder();
        }

        private void btnPrintTransaction_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "IssueDyedFabricReport";
            if (_dyedOrderViewModel.SelectedMainDetails.TransactionType == 0)
            {
                reportName = "IssueDyedFabricReport";
            }
            else
            {
                reportName = "RecieveDyedFabricReport";
            }

            // _dyedOrderViewModel.SelectedMainDetails.TransId, _dyedOrderViewModel.SelectedMainDetails.DyeingProductionOrder, LoggedUserInfo.WFM_UserName)
            var para = new ObservableCollection<string>();// { ViewModel.TransactionHeader.Iserial.ToString() };
            para.Add(_dyedOrderViewModel.SelectedMainDetails.TransId.ToString(CultureInfo.InvariantCulture));
            para.Add(_dyedOrderViewModel.SelectedMainDetails.DyeingProductionOrder.ToString(CultureInfo.InvariantCulture));
            para.Add(LoggedUserInfo.WFM_UserName);

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);

            //var myUri = new Uri(HtmlPage.Document.DocumentUri, String.Format("report/IssueFabricReport.aspx?TransId={0}&DyeingProductionOrder={1}&User={2}", _dyedOrderViewModel.SelectedMainDetails.TransId, _dyedOrderViewModel.SelectedMainDetails.DyeingProductionOrder, LoggedUserInfo.WFM_UserName));
            //HtmlPage.Window.Navigate(myUri, "_blank");
        }

        private void btnDyeingOrderServices_Click(object sender, RoutedEventArgs e)
        {
            _dyedOrderViewModel.DyeingOrderServices();
        }

        public enum FormMode
        {
            Standby,
            Search,
            Add,
            Update,
            Read
        }

        public FormMode _FormMode { get; set; }

        public FormMode FormPlanMode { get; set; }

        #region DyedOrder Form Modes

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    BtnPrintPreviewOrder.IsEnabled = false;
                    //ControlDoc.IsEnabled = ControlTransactionDate.IsEnabled = ControlVendor.IsEnabled = true;
                    _dyedOrderViewModel.DyeingOrderHeader.Enabled = true;
                    break;

                case FormMode.Standby:

                    _dyedOrderViewModel.DyeingOrderHeader.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = false;
                    BtnEditOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;
                    BtnPrintPreviewOrder.IsEnabled = false;
                    ClearScreen();
                    break;

                case FormMode.Search:

                    _dyedOrderViewModel.DyeingOrderHeader.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                    BtnPrintPreviewOrder.IsEnabled = false;
                    break;

                case FormMode.Update:
                    _dyedOrderViewModel.DyeingOrderHeader.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = true;
                    BtnPrintPreviewOrder.IsEnabled = false;
                    break;

                case FormMode.Read:
                    _dyedOrderViewModel.DyeingOrderHeader.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    BtnSaveOrder.IsEnabled = false;
                    BtnEditOrder.IsEnabled = true;
                    BtnPrintPreviewOrder.IsEnabled = true;

                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            //BtnDeleteOrder.Visibility = Visibility.Visible;
            //BtnDeleteOrder.IsEnabled = true;
            BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
        }

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            //     PrintPreviews.RoutePrintPreview _PrintDialog = new PrintPreviews.RoutePrintPreview(this.MyViewModelProperty);
            //  _PrintDialog.Show();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancelOrder.IsEnabled = false;
            BtnCancelOrder.Visibility = Visibility.Collapsed;
            //BtnDeleteOrder.Visibility = Visibility.Collapsed;
            //BtnDeleteOrder.IsEnabled = false;
            BtnShowSearchOrder.IsChecked = false;
        }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
            _dyedOrderViewModel.ResetMode();
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        
        {
            _dyedOrderViewModel.DyeingOrderHeaderList.Clear();
            var childWindowSeach = new DyeingOrderSearchResultsChild(_dyedOrderViewModel);
            childWindowSeach.Show();
            _dyedOrderViewModel.SearchHeader();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        #endregion DyedOrder Form Modes

        #region DyedPlanFormModes

        private void ClearPlanScreen()
        {
            BtnEditOrderPlan.IsChecked = false;
            BtnAddNewOrderPlan.IsChecked = false;
        }

        public void SwitchPlanFormMode(FormMode formMode)
        {
            BtnPostToAx.Visibility = Visibility.Collapsed;
            BtnPostToAx.IsEnabled = false;
            switch (formMode)
            {
                case FormMode.Add:
                    ClearPlanScreen();
                    BtnSaveOrderPlan.IsEnabled = true;
                    BtnShowSearchOrderPlan.Visibility = Visibility.Collapsed;
                    BtnPrintPreviewOrderPlan.IsEnabled = false;
                    _dyedPlanViewModel.HeaderViewModel.Enabled = true;

                    break;

                case FormMode.Standby:

                    _dyedPlanViewModel.HeaderViewModel.Enabled = false;
                    BtnAddNewOrderPlan.IsEnabled = true;
                    BtnAddNewOrderPlan.Visibility = Visibility.Visible;
                    BtnSearchOrderPlan.IsEnabled = true;
                    BtnSaveOrderPlan.IsEnabled = false;
                    BtnEditOrderPlan.IsEnabled = false;
                    BtnShowSearchOrderPlan.Visibility = Visibility.Visible;
                    BtnShowSearchOrderPlan.IsEnabled = true;
                    BtnPrintPreviewOrderPlan.IsEnabled = false;
                    ClearPlanScreen();
                    break;

                case FormMode.Search:

                    _dyedPlanViewModel.HeaderViewModel.Enabled = true;
                    BtnAddNewOrderPlan.IsEnabled = false;
                    BtnAddNewOrderPlan.Visibility = Visibility.Collapsed;
                    BtnSaveOrderPlan.IsEnabled = false;
                    BtnShowSearchOrderPlan.IsEnabled = false;
                    BtnPrintPreviewOrderPlan.IsEnabled = false;
                    break;

                case FormMode.Update:
                    _dyedPlanViewModel.HeaderViewModel.Enabled = true;
                    BtnAddNewOrderPlan.Visibility = Visibility.Visible;
                    BtnSaveOrderPlan.IsEnabled = true;
                    BtnPrintPreviewOrderPlan.IsEnabled = false;
                    break;

                case FormMode.Read:
                    _dyedPlanViewModel.HeaderViewModel.Enabled = false;
                    BtnAddNewOrderPlan.Visibility = Visibility.Collapsed;
                    BtnSearchOrderPlan.IsEnabled = false;
                    BtnSaveOrderPlan.IsEnabled = false;
                    BtnEditOrderPlan.IsEnabled = true;
                    BtnPrintPreviewOrderPlan.IsEnabled = true;
                    BtnPostToAx.Visibility = Visibility.Visible;
                    BtnPostToAx.IsEnabled = true;
                    break;
            }
        }

        private void btnEditOrderPlan_Checked(object sender, RoutedEventArgs e)
        {
            FormPlanMode = FormMode.Update;
            SwitchPlanFormMode(FormPlanMode);
            BtnCancelOrderPlan.Visibility = Visibility.Visible;
            BtnCancelOrderPlan.IsEnabled = true;
            BtnDeleteOrderPlan.Visibility = Visibility.Visible;
            BtnDeleteOrderPlan.IsEnabled = true;
            BtnEditOrderPlan.IsEnabled = false;
        }

        private void btnAddNewOrderPlan_Checked(object sender, RoutedEventArgs e)
        {
            FormPlanMode = FormMode.Add;
            SwitchPlanFormMode(FormPlanMode);
            BtnCancelOrderPlan.Visibility = Visibility.Visible;
            BtnCancelOrderPlan.IsEnabled = true;
        }

        private void btnShowSearchOrderPlan_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrderPlan.Visibility = Visibility.Visible;
            BtnCancelOrderPlan.IsEnabled = true;
            FormPlanMode = FormMode.Search;
            SwitchPlanFormMode(FormPlanMode);
        }

        private void btnCancelOrderPlan_Click(object sender, RoutedEventArgs e)
        {
            ResetPlanMode();
            BtnCancelOrderPlan.IsEnabled = false;
            BtnCancelOrderPlan.Visibility = Visibility.Collapsed;
            BtnDeleteOrderPlan.Visibility = Visibility.Collapsed;
            BtnDeleteOrderPlan.IsEnabled = false;
            BtnShowSearchOrderPlan.IsChecked = false;
        }

        private void ResetPlanMode()
        {
            FormPlanMode = FormMode.Standby;
            SwitchPlanFormMode(FormPlanMode);
            //    _dyedPlanViewModel.ResetMode();
        }

        private void btnSearchOrderPlan_Click(object sender, RoutedEventArgs e)
        {
            var childWindowSeach = new DyeingPlanSearchResults(_dyedPlanViewModel);
            childWindowSeach.Show();

            _dyedPlanViewModel.SearchHeader();

            FormPlanMode = FormMode.Read;
            SwitchPlanFormMode(FormPlanMode);
        }

        #endregion DyedPlanFormModes

        private void btnSaveOrderPlan_Click(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.SaveHeader();
        }

        private void btnPrintPreviewOrderPlan_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "Dyeing Plan Report";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "Dyeing Plan Report"; }

            var para = new ObservableCollection<string> { _dyedPlanViewModel.HeaderViewModel.DocNo.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void btnDeleteOrderPlan_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("You are about to delete a Dyed OrderPlan permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _dyedPlanViewModel.DeletePlan();
            }
        }

        private void TabMainDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabMainDetails != null)
            {
                if (TabMainDetails.SelectedIndex == 1)
                {
                    _dyedOrderViewModel.GetFabricInventSumWithBatches();
                }
            }
        }

        private void btnFabricLot_Click(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.GenerateFabricLots();
        }

        private void BtnSalesOrderBomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.GetBillOfMaterials();
        }

        private void BtnAddFreePlanRow_OnClick(object sender, RoutedEventArgs e)
        {
            _dyedPlanViewModel.HeaderViewModel.DyeingViewModelList.Add(new TblDyeingPlanViewModel());
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
           var row=  SummaryGrid.SelectedItem;
            if (e.Key == Key.Delete)
            {
                _dyedPlanViewModel.DyeingSummeryViewModelList.Remove(row as DyeingSummeryViewModel);
            }

        }

        private void ListDyeingItems_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _dyedPlanViewModel.HeaderViewModel.DyeingViewModelList.Remove((TblDyeingPlanViewModel)ListDyeingItems.SelectedItem);
            }

        }
    }
}