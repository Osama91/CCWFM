using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using CCWFM.Views.OGView.ChildWindows;
using _ClientProxy = CCWFM.CRUDManagerService;
using CCWFM.RouteCardService;

namespace CCWFM.Views
{
    public partial class RouteCard
    {
        private RouteCard_StyleChooserChild _childStyleChooser;

        public List<_ClientProxy.TblSize> AllSizes { get; set; }

        public RouteCardHeaderViewModel MyViewModel = new RouteCardHeaderViewModel();

        public int PostOrNo { get; set; }

        private RouteCardSearchResultChild _searchWindow;

        public RouteCard()
        {
            InitializeComponent();
            LayoutRoot.DataContext = MyViewModel;
            UserIserial = LoggedUserInfo.Iserial;
            MyViewModel.PremCompleted += (s, sv) =>
            {
                if (MyViewModel.CustomePermissions.FirstOrDefault(w => w.Code == "EditIssue") != null)
                {
                    DgfabricIssue.Columns.FirstOrDefault(w => w.SortMemberPath == "Qty").IsReadOnly = false;
                }
                else
                {
                    DgfabricIssue.Columns.FirstOrDefault(w => w.SortMemberPath == "Qty").IsReadOnly = true;
                }
            };
        }

        public int UserIserial { get; set; }

        public FormMode FormMode { get; set; }

        private void _ChildStyleChooser_SubmitAction(object sender, EventArgs e)
        {
            try
            {
                foreach (var variable in _childStyleChooser.ReturnResult)
                {
                    variable.WareHouseDegreeList = MyViewModel.WareHouseDegreeList;
                    MyViewModel.RouteCardDetails.Add(variable);
                }
            }
            catch (Exception ex)
            {
                var errw = new ErrorWindow(ex);
                errw.Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TryToOpenChild();
        }

        private void TryToOpenChild()
        {
            try
            {
                var myViewModel = (LayoutRoot.DataContext as RouteCardHeaderViewModel);
                if (myViewModel != null && myViewModel.TransactionType == 9)
                {
                    var child = new RouteCardSearchResultChild(myViewModel, true);
                    child.Show();
                }
                else
                {
                    _childStyleChooser = new RouteCard_StyleChooserChild((int)CmbRouteGroup.SelectedValue, CmbDirection.SelectedIndex, (int)cmbRouteType.SelectedValue, myViewModel != null && myViewModel.BoolCuttingQty, myViewModel);
                    _childStyleChooser.Show();

                    _childStyleChooser.SubmitAction += _ChildStyleChooser_SubmitAction;
                }
            }
            catch (Exception)
            {
                //if the data is not yet loaded notify the user
                MessageBox.Show("Data is not yet loaded; this could be due to a slow connection!\nPlease wait a little while and try again!", "", MessageBoxButton.OK);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (sender == BtnSave)
            {
                PostOrNo = 0;
            }
            else if (sender == BtnPostToAx)
            {
                PostOrNo = 1;
            }

            if (MyViewModel.TransactionType== 10||MyViewModel.TransactionType==7)
            {
                if (MyViewModel.RouteCardFabricViewModelList.Any())
                {
                    foreach (var item in MyViewModel.RouteCardFabricViewModelList)
                    {
                        if (item.CostPerUnit==0|| item.CostPerUnit==null)
                        {
                            MessageBox.Show("Cost Cannot Be 0");
                            return;
                        }

                    }

                }

            }

            

            if (MyViewModel.Iserial == 0)
            {

                var proxy = new RouteCardServiceClient();
                proxy.GetMaxRouteCardTransactionIDAsync((int)CmbRouteGroup.SelectedValue, CmbDirection.SelectedIndex,
                    MyViewModel.TransactionType);
                proxy.GetMaxRouteCardTransactionIDCompleted +=
                    _proxy_GetMaxRouteCardTransactionIDCompleted;
            }

            else
            {
                var proxy = new RouteCardServiceClient();
                proxy.GetMaxRouteCardTransactionIDAsync((int)CmbRouteGroup.SelectedValue, CmbDirection.SelectedIndex,
                    MyViewModel.TransactionType);
                proxy.GetMaxRouteCardTransactionIDCompleted += (s, ev)
                    =>
                {
                    var myViewModel = (LayoutRoot.DataContext as RouteCardHeaderViewModel);

                    var routerCardFabricRow = new ObservableCollection<RouteCardService.RouteCardFabric>();

                    if (myViewModel != null)
                    {

                        MyViewModel.TransID = (ev.Result + 1);
                        foreach (var routeFabricRow in myViewModel.RouteCardFabricViewModelList)
                        {
                            routerCardFabricRow.Add(RouteCardMappers.MapToViewModel(routeFabricRow));
                        }

                        var rch = new RouteCardService.RouteCardHeader();
                        var rcd = new ObservableCollection<RouteCardService.RouteCardDetail>();
                        try
                        {
                            RouteCardMappers.MapToViewModel(myViewModel, rch);

                            foreach (var item in myViewModel.RouteCardDetails)
                            {
                                foreach (var sizeItem in item.RoutCardSizes.Where(x => x.SizeCode != ""))
                                {
                                    rcd.Add(new RouteCardService.RouteCardDetail
                                    {
                                        TblColor = item.TblColor,
                                        Degree = item.Degree,
                                        TblSalesOrder = item.TblSalesOrder,
                                        Trans_TransactionHeader = rch.TransID,
                                        Size = sizeItem.SizeCode,
                                        SizeQuantity = sizeItem.SizeConsumption,
                                        RoutGroupID = rch.RoutGroupID,
                                        Direction = rch.Direction,
                                        ObjectIndex = item.ObjectIndex,
                                        Price = item.Price,
                                        Notes = item.Notes,
                                        Blocks = item.Blocks,
                                        TblWarehouse = item.TblWarehouse
                                    });
                                }
                            }
                            myViewModel.SaveRouteCard(rch, rcd, "update", PostOrNo);
                            tBlockTransID.Text = rch.TransID.ToString(CultureInfo.InvariantCulture);
                            //btnCancel_Click(null, null);
                        }
                        catch (Exception ex)
                        {
                            var err = new ErrorWindow(ex);
                        }

                    }

                };
            }
        }

        private void _proxy_GetMaxRouteCardTransactionIDCompleted(object sender, GetMaxRouteCardTransactionIDCompletedEventArgs e)
        {
            var rch = new RouteCardService.RouteCardHeader();

            var routerCardFabricRow = new ObservableCollection<RouteCardService.RouteCardFabric>();

            foreach (var routeFabricRow in MyViewModel.RouteCardFabricViewModelList)
            {
                routerCardFabricRow.Add(RouteCardMappers.MapToViewModel(routeFabricRow));
            }
            var rcd = new ObservableCollection<RouteCardService.RouteCardDetail>();
            try
            {
                switch (CmbDirection.SelectedIndex)
                {
                    case 0:
                        rch.Direction = 0;
                        break;

                    case 1:
                        rch.Direction = 1;
                        break;
                }

                MyViewModel.TransID = (e.Result + 1);

                RouteCardMappers.MapToViewModel(MyViewModel, rch);
                foreach (var item in MyViewModel.RouteCardDetails)
                {
                    foreach (var sizeItem in item.RoutCardSizes.Where(x => x.SizeCode != ""))
                    {
                        rcd.Add(new RouteCardService.RouteCardDetail
                        {
                            TblColor = item.TblColor,
                            Degree = item.Degree,
                            TblSalesOrder = item.TblSalesOrder,
                            Trans_TransactionHeader = rch.TransID,
                            Size = sizeItem.SizeCode,
                            SizeQuantity = sizeItem.SizeConsumption,
                            RoutGroupID = rch.RoutGroupID,
                            Direction = rch.Direction,
                            ObjectIndex = item.ObjectIndex,
                            Price = item.Price,
                            Notes = item.Notes,
                            Blocks = item.Blocks,
                            TblWarehouse = item.TblWarehouse,

                        });
                    }
                }

                MyViewModel.SaveRouteCard(rch, rcd, "add", PostOrNo);
                tBlockTransID.Text = rch.TransID.ToString(CultureInfo.InvariantCulture);
                // btnCancel_Click(null, null);

                btnEdit_Checked(null, null);
            }
            catch (Exception ex)
            {
                var err = new ErrorWindow(ex);
            }
        }

        private void dGridRouteCardDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = new Grid { HorizontalAlignment = HorizontalAlignment.Left };
            for (var counter = 0; counter < 15; counter++)
            {
                temp.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(49) });
            }
            icSizes.Items.Clear();
            var i = 0;
            var strTemp = (RouteCardViewModel)DGridRouteCardDetails.SelectedItem;

            if (strTemp != null)
                foreach (var tblock in strTemp.RoutCardSizes.Select(item => new TextBlock
                {
                    Text = item.SizeCode,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Tag = item.SizeCode + "_" + item.SizeCode
                }))
                {
                    Grid.SetColumn(tblock, i);
                    temp.Children.Add(tblock);
                    i++;
                }
            icSizes.Items.Add(temp);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyViewModel.RouteCardDetails.Where(x => x.ObjectIndex == (string)((Image)sender).Tag);
            MyViewModel.RouteCardDetails.Remove(DGridRouteCardDetails.SelectedItem as RouteCardViewModel);
        }

        private void txtRowTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            MyViewModel.GrandTotal = MyViewModel.RouteCardDetails.Sum(x => x.RowTotal);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnDeleteCard.Visibility = Visibility.Collapsed;
            BtnDeleteCard.IsEnabled = false;
            BtnShowSearch.IsChecked = false;
        }

        private void ResetMode()
        {
            FormMode = FormMode.Standby;
            SwitchFormMode(FormMode);
        }

        private void ClearScreen()
        {
            MyViewModel.ClearViewModel();
            LayoutRoot.DataContext = MyViewModel;
            BtnEdit.IsChecked = false;
            BtnAddNewCard.IsChecked = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //_searchWindow.AssignSearchCriteria(MyViewModel);
            _searchWindow = new RouteCardSearchResultChild(MyViewModel, false);
            _searchWindow.SubmitSearchAction += searchWindow_SubmitSearchAction;
            _searchWindow.Show();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void searchWindow_SubmitSearchAction(object sender, EventArgs e)
        {
            if (((RouteCardSearchResultChild)sender).DialogResult == true)
            {
                try
                {
                    FormMode = FormMode.Read;
                    SwitchFormMode(FormMode);
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
            else
            {
                BtnEdit.IsChecked = false;
                BtnAddNewCard.IsChecked = false;
            }
        }

        private void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    tBlockTransID.IsReadOnly = true;
                    BtnSave.IsEnabled = true;
                    BtnShowSearch.Visibility = Visibility.Collapsed;
                    cmbVendor.IsEnabled = true;
                    CmbRouteGroup.IsEnabled = true;
                    DPickerOutDate.IsEnabled = true;
                    break;

                case FormMode.Standby:
                    tBlockTransID.IsReadOnly = true;
                    //   CmbDirection.IsEnabled = false;
                    cmbVendor.IsEnabled = false;
                    CmbRouteGroup.IsEnabled = false;
                    DPickerOutDate.IsEnabled = false;
                    BtnAddNewCard.IsEnabled = true;
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = true;
                    BtnSave.IsEnabled = false;
                    BtnEdit.IsEnabled = false;
                    BtnShowSearch.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    tBlockTransID.IsReadOnly = false;
                    //  CmbDirection.IsEnabled = true;
                    cmbVendor.IsEnabled = true;
                    CmbRouteGroup.IsEnabled = true;
                    DPickerOutDate.IsEnabled = true;
                    BtnAddNewCard.IsEnabled = false;
                    BtnAddNewCard.Visibility = Visibility.Collapsed;
                    BtnSave.IsEnabled = true;
                    BtnShowSearch.IsEnabled = false;
                    break;

                case FormMode.Update:
                    tBlockTransID.IsReadOnly = true;
                    cmbVendor.IsEnabled = true;
                    CmbRouteGroup.IsEnabled = false;
                    DPickerOutDate.IsEnabled = true;
                    BtnSave.IsEnabled = true;
                    break;

                case FormMode.Read:
                    tBlockTransID.IsReadOnly = true;
                    cmbVendor.IsEnabled = false;
                    CmbRouteGroup.IsEnabled = false;
                    DPickerOutDate.IsEnabled = true;
                    BtnSearch.IsEnabled = false;
                    BtnSave.IsEnabled = true;
                    BtnEdit.IsEnabled = true;

                    break;
            }
        }

        private void btnEdit_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Update;
            SwitchFormMode(FormMode);
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            BtnDeleteCard.Visibility = Visibility.Visible;
            BtnDeleteCard.IsEnabled = true;
            BtnEdit.IsEnabled = false;
        }

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
        }

        private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            var r =
            MessageBox
            .Show("You are about to delete a Route card permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                var proxy = new RouteCardServiceClient();
                try
                {
                    proxy.DeleteRoutCardCompleted += (s, ev) => MessageBox.Show("Route Card Deleted!", "Delete!", MessageBoxButton.OK);
                    proxy.DeleteRoutCardAsync(MyViewModel.Iserial, LoggedUserInfo.Iserial);
                    btnCancel_Click(null, null);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "RouteCardReportAr";

            if (TabRouteCard.SelectedIndex == 1)
            {
                var reportChild = new ReportsChildWindow(PermissionItemName.RouteCardForm.ToString(), MyViewModel.LastTransaction);
                reportChild.Show();
                return;
            }
            if (MyViewModel.RouteTypeInt == 9)
            {
                reportName = "StyleCostAndEstimatedCost";
            }

            var para = new ObservableCollection<string> { MyViewModel.LastTransaction.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void TabRouteCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabRouteCard != null)
            {
                if (TabRouteCard.SelectedIndex == 1)
                {
                    if ((MyViewModel.TransactionType == 1 || MyViewModel.TransactionType == 2 || MyViewModel.TransactionType == 5))//&&)// MyViewModel.IsPosted != true)
                    {
                        MyViewModel.LoadRouteFabric();
                    }
                }
            }
        }

        private void CbWareHouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbWarehouse = (sender) as ComboBox;
            if (cbWarehouse != null)
            {
                var row = cbWarehouse.DataContext as RouteCardFabricViewModel;
                var header = LayoutRoot.DataContext as RouteCardHeaderViewModel;
                if (row != null)
                {
                    if (cbWarehouse.SelectedValue != null)
                    {
                        if (header != null)
                        {
                            var singleOrDefault = header.WarehouseList.SingleOrDefault(x => x.WarehouseID == cbWarehouse.SelectedValue.ToString());
                            if (
                                singleOrDefault != null)
                                row.Site = singleOrDefault.SiteId;
                        }
                        row.Location = cbWarehouse.SelectedValue.ToString();
                    }
                }
            }
        }

        private void cmbBatches_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void BarcodeText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MyViewModel.GetInfoFromBarcode((DgfabricIssue.SelectedItem as RouteCardFabricViewModel));
            }
        }

        private void BarcodeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void BarcodeText_LostFocus(object sender, RoutedEventArgs e)
        {
            MyViewModel.GetInfoFromBarcode((DgfabricIssue.SelectedItem as RouteCardFabricViewModel));
        }

        private void btnDeleteFabricIssue_Click(object sender, RoutedEventArgs e)
        {
            MyViewModel.DeleteRouteCardFabric(DgfabricIssue.SelectedItem as RouteCardFabricViewModel);
        }

        private void AvaliableQuantity_Click(object sender, RoutedEventArgs e)
        {
            MyViewModel.LoadAvaliableQty();
        }

        private void BtnFree_OnClick(object sender, RoutedEventArgs e)
        {
            MyViewModel.GetFreeStyles();
        }

        private void Image_Add(object sender, MouseButtonEventArgs e)
        {
            var oldrow = DGridRouteCardDetails.SelectedItem as RouteCardViewModel;
            var row = new RouteCardViewModel()
            {
                ColorPerRow = oldrow.ColorPerRow,
                TblColor = oldrow.TblColor,
                Direction = oldrow.Direction,
                Notes = oldrow.Notes,
                Price = oldrow.Price,
                RouteCardHeaderIserial = oldrow.RouteCardHeaderIserial,
                SalesOrderPerRow = oldrow.SalesOrderPerRow,
                TblSalesOrder = oldrow.TblSalesOrder,
                Size = oldrow.Size,
                SizeQuantity = oldrow.SizeQuantity,
                TblWarehouse = oldrow.TblWarehouse,
                RouteGroupID = oldrow.RouteGroupID,
            };

            row.WareHouseDegreeList = new ObservableCollection<TblAuthWarehouse1>();

            foreach (var item in oldrow.WareHouseDegreeList)
            {
                row.WareHouseDegreeList.Add(item);
            }
            row.Degrees = oldrow.Degrees;
            row.Degree = oldrow.Degree;
            row.ObjectIndex = Guid.NewGuid().ToString("D");
            row.RoutCardSizes = new ObservableCollection<RoutCardSizeInfo>();
            row.RoutCardSizes.CollectionChanged += row.RoutCardSizes_CollectionChanged;
            if (oldrow != null)
                foreach (var variable in oldrow.RoutCardSizes)
                {
                    row.RoutCardSizes.Add(new RoutCardSizeInfo
                    {
                        SizeCode = variable.SizeCode,
                        IsTextBoxEnabled = variable.IsTextBoxEnabled,
                        SizeConsumption = variable.SizeConsumption,
                    });
                }
            MyViewModel.RouteCardDetails.Add(row);
        }

        private void chkProductionResidue_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}