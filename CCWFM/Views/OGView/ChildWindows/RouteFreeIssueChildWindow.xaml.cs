using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class RouteFreeIssueChildWindow
    {
        private readonly RouteFreeIssueViewModel ViewModel = new RouteFreeIssueViewModel();
        private readonly RouteCardHeaderViewModel _viewModel;
        private readonly PurchaseOrderRequestViewModel _viewModel2;

        public RouteFreeIssueChildWindow(RouteCardHeaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = ViewModel;
            var distinctRoute = viewModel.RouteCardDetails.GroupBy(x => new { x.SalesOrderPerRow, x.ColorPerRow });

            foreach (var variable in distinctRoute.Select(x => x.Key.SalesOrderPerRow).Distinct())
            {
                ViewModel.SalesOrderList.Add(variable);
            }

            _viewModel = viewModel;

            ViewModel.WarehouseList = _viewModel.WarehouseList;

            ViewModel.MainRowList.Add(new RouteCardFabricViewModel());
        }

        public RouteFreeIssueChildWindow(PurchaseOrderRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = ViewModel;
            DgfabricIssue.Columns[0].Visibility = Visibility.Collapsed;
            DgfabricIssue.Columns[1].Visibility = Visibility.Collapsed;
            _viewModel2 = viewModel;
            ViewModel.MainRowList.Add(new RouteCardFabricViewModel());
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            if (_viewModel2 != null)
            {
                foreach (var variable in ViewModel.MainRowList)
                {
                    string batch = "";
                    if (!string.IsNullOrEmpty(variable.Barcode))
                    {
                        batch = variable.Barcode;
                    }
                    else
                    {
                        batch = variable.Barcode;
                    }
                    variable.IsFree = true;
                    _viewModel2.SelectedMainRow.DetailsList.Add(
                        new TblPurchaseOrderDetailRequestModel()
                        {
                            TblPurchaseOrderHeaderRequest = _viewModel2.SelectedMainRow.Iserial,
                            BasicPrice = variable.CostPerUnit??0,
                            Price = variable.CostPerUnit??0,
                            FabricColor = variable.FabricColor,
                            ColorPerRow = variable.FabricColorPerRow,
                            ItemPerRow = variable.ItemPerRow,
                            ItemId = variable.ItemId,
                            BatchNo = batch,
                            Unit = variable.Unit,
                            IsAcc = variable.IsAcc,
                            ItemType = variable.ItemPerRow.ItemGroup,
                            Qty = variable.Qty ?? 0,
                        }
                        );



                    //_viewModel.RouteCardService.GetAxItemPriceAsync(variable.ItemId, batch, variable.FabricColorPerRow.Code, variable.Warehouse);
                }
            }
            else
            {
                foreach (var variable in ViewModel.MainRowList)
                {
                    variable.IsFree = true;
                    variable.ItemGroup = variable.ItemPerRow.ItemGroup;
                    _viewModel.RouteCardFabricViewModelList.Add(variable);
                    string batch = "";
                    if (!string.IsNullOrEmpty(variable.Batch))
                    {
                        batch = variable.Batch;
                    }
                    if (!string.IsNullOrEmpty(variable.Barcode))
                    {
                        batch = variable.Barcode;
                    }


                    //else
                    //{
                    //    batch = variable.Size;
                    //}

                    _viewModel.RouteCardService.GetAxItemPriceAsync(_viewModel.TransactionType, variable.ItemGroup ?? "", variable.ItemId ?? "", batch ?? "", variable.Size??"", variable.FabricColorPerRow.Code ?? "", variable.Warehouse ?? "");
                }
            }


            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedMainRow.ColorList.Clear();
            var senderitem = sender as ComboBox;
            var salesorder = Convert.ToInt32(senderitem.SelectedValue);
            foreach (var variable in _viewModel.RouteCardDetails.Where(x => x.TblSalesOrder == salesorder)
                .Select(x => x.ColorPerRow).Distinct())
            {
                ViewModel.SelectedMainRow.ColorList.Add(variable);
            }
        }

        private void CbWareHouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbWarehouse = (sender) as ComboBox;
            var row = cbWarehouse.DataContext as RouteCardFabricViewModel;

            if (row != null)
            {
                if (cbWarehouse.SelectedValue != null)
                {
                    row.Site = ViewModel.WarehouseList.SingleOrDefault(x => x.WarehouseID == cbWarehouse.SelectedValue.ToString()).SiteId;
                    row.Location = cbWarehouse.SelectedValue.ToString();
                }
            }
        }

        private void btnDeleteFabricIssue_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainRowList.Remove(ViewModel.SelectedMainRow);
        }

        private void BtnBarcode_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new CuttingOrderChildWindow(DgfabricIssue.SelectedItem as RouteCardFabricViewModel);
            child.Show();
            child.SubmitClicked += (s, sv) =>
            {
                if (child.CuttingOrderListChildWindows != null)
                {
                    if (child.CuttingOrderListChildWindows.Count == 1)
                    {
                        var row = child.CuttingOrderListChildWindows.FirstOrDefault();

                        if (row != null)
                        {
                            ViewModel.SelectedMainRow.Barcode = row.Barcode;
                            ViewModel.SelectedMainRow.Qty = row.StoreRollQty;
                            ViewModel.SelectedMainRow.Warehouse = row.Warehouse;
                            ViewModel.SelectedMainRow.Site = row.Site;
                            ViewModel.SelectedMainRow.Location = row.Warehouse;
                        }
                    }
                    else
                    {
                        foreach (var row in child.CuttingOrderListChildWindows)
                        {
                            var newrow = new RouteCardFabricViewModel();
                            newrow.InjectFrom(ViewModel.SelectedMainRow);
                            newrow.Barcode = row.Barcode;
                            newrow.Qty = row.StoreRollQty;
                            newrow.Warehouse = row.Warehouse;
                            newrow.Site = row.Site;
                            newrow.Location = row.Warehouse;
                            ViewModel.MainRowList.Add(newrow);
                        }
                    }
                }
            };
        }
    }
}