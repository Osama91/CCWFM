using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.ViewModel.OGViewModels.Mappers;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class FabricDefectsLineCreationChildWindow
    {
// ReSharper disable once InconsistentNaming
        private readonly FabricDefectsViewModel ViewModel;
        private readonly ReservationViewModel _viewModel;

        public FabricDefectsLineCreationChildWindow(FabricDefectsViewModel viewModel)
        {
            InitializeComponent();           
            DataContext = viewModel;
            ViewModel = viewModel;

            ViewModel.Filter = null;
            if (ViewModel.ValuesObjects!=null)
            {
                ViewModel.ValuesObjects.Clear();
            }
         
        }

        public FabricDefectsLineCreationChildWindow(ReservationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.Filter = null;
            if (_viewModel.ValuesObjects != null)
            {
                _viewModel.ValuesObjects.Clear();
            }
        }

        public double Truncate(double d)
        {
            return Convert.ToDouble(Decimal.Truncate(Convert.ToDecimal(d)));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                var viewModel = _viewModel;

                var lines = DgFabricInsepectionLines.SelectedItems;
                foreach (OrderLineListViewModel item in lines)
                {
                    if (!viewModel.TransactionHeader.TransactionMainDetails.Contains(ReservationMappers.MaptoViewModel(item)))
                    {
                        viewModel.TransactionHeader.TransactionMainDetails.Add(ReservationMappers.MaptoViewModel(item));
                    }
                }
            }
            else
            {
                var viewModel = DataContext as FabricDefectsViewModel;
                var lines = DgFabricInsepectionLines.SelectedItems;
                foreach (OrderLineListViewModel item in lines)
                {
                    if (viewModel != null)
                    {
                        var inspectionRow = viewModel.TransactionDetails.Where(x => x.Fabric_Code == item.Fabric_Code && x.BatchNo == item.BatchNo && x.ColorCode == item.Color_Code).ToList();
                        var maxRoll = 0;
                        if (inspectionRow.Count() != 0)
                        {
                            maxRoll = inspectionRow.Max(x => x.RollNo);
                        }

                        for (var i = 1; i <= item.NoOfRolls; i++)
                        {
                            var detailsViewModel = new ObservableCollection<TblFabricInspectionDetailDefectsViewModel>();
                            foreach (var itemd in viewModel.DefectsList)
                            {
                                detailsViewModel.Add(new TblFabricInspectionDetailDefectsViewModel
                                {
                                    Iserial = 0,
                                    Tbl_fabricInspectionDetail = 0,
                                    DefectIserial = itemd.Iserial,
                                    DefectValue = 0,
                                });
                            }
                            
                            var ss = Convert.ToSingle((Decimal.Truncate((decimal)((item.TotalQty / item.NoOfRolls) * 1000)) / 1000).ToString("N3"));
                            viewModel.TransactionDetails.Add(new TblFabricInspectionDetailViewModel
                            {
                                LineNum = item.LineNum,
                                BatchNo = item.BatchNo,
                                ColorCode = item.Color_Code,
                                Fabric_Code = item.Fabric_Code,
                                RollNo = maxRoll + i,
                                Unit = item.PURCHUNIT,
                                UnitPrice = item.UnitPrice,
                                DetailsViewModel = detailsViewModel,
                                StoreRollQty = ss,
                                Degree = 1,
                                FinishedWarehouse = item.Warehouse,
                                FinishedSite = item.Site,
                                NoofPCs = 4,
                                TotalLineNumQty = item.TotalQty,
                                OrgLocation = item.Warehouse,
                            });
                        }
                    }
                }
            }


            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                var viewModel = DataContext as ReservationViewModel;
                if (viewModel != null)
                    viewModel.OrderLineList.Remove(DgFabricInsepectionLines.SelectedItem as OrderLineListViewModel);
            }
            else
            {
                var viewModel = DataContext as FabricDefectsViewModel;
                if (viewModel != null)
                    viewModel.OrderLineList.Remove(DgFabricInsepectionLines.SelectedItem as OrderLineListViewModel);
            }
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void DgFabricInsepectionLines_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel != null)
            {
                var viewModel = DataContext as ReservationViewModel;
                if (viewModel != null && viewModel.OrderLineList.Count < viewModel.PageSize)
                {
                    return;
                }
                if (viewModel != null && (viewModel.OrderLineList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.OrderLineList.Count != viewModel.FullCount))
                {
                    viewModel.GetOrderInfo();
                }
            }
            else
            {
                if (ViewModel.OrderLineList.Count < ViewModel.PageSize)
                {
                    return;
                }
                if (ViewModel.OrderLineList.Count - 2 < e.Row.GetIndex() && !ViewModel.Loading && ViewModel.OrderLineList.Count != ViewModel.FullCount)
                {
                    ViewModel.GetOrderInfo();
                }
            }
        }

        private void DgFabricInsepectionLines_OnOnFilter(object sender, FilterEvent e)
        {
            if (_viewModel != null)
            {
                _viewModel.OrderLineList.Clear();
                var counter = 0;
                _viewModel.Filter = null;

                _viewModel.ValuesObjects = new Dictionary<string, object>();

                foreach (var f in e.FiltersPredicate)
                {
                    string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                    object myObject = null;
                    try
                    {
                        myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                    }
                    catch (Exception)
                    {
                        myObject = "";
                    }
                    switch (f.SelectedFilterOperation.FilterOption)
                    {
                        case Enums.FilterOperation.EndsWith:
                            myObject = "%" + f.FilterText;
                            break;

                        case Enums.FilterOperation.StartsWith:
                            myObject = f.FilterText + "%";
                            break;

                        case Enums.FilterOperation.Contains:
                            myObject = "%" + f.FilterText + "%";
                            break;
                    }

                    _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                    if (counter > 0)
                    {
                        _viewModel.Filter = _viewModel.Filter + " and ";
                    }

                    _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                        f.SelectedFilterOperation.LinqUse + paramter;

                    counter++;
                }
                _viewModel.GetOrderInfo();
            }
            else
            {            
            ViewModel.OrderLineList.Clear();
            var counter = 0;
            ViewModel.Filter = null;

            ViewModel.ValuesObjects = new Dictionary<string, object>();
               
            foreach (var f in e.FiltersPredicate)
            {               
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }
                if (counter > 0)
                {
                    ViewModel.Filter = ViewModel.Filter + " and ";
                }

                if (ViewModel.TransactionHeader.TransactionType == 1)
                {
                    var filterRow =
            e.FiltersPredicate.FirstOrDefault(x => x.FilterColumnInfo.PropertyPath == "ITEMID");

                    if (e.FiltersPredicate.Any(x => x.FilterColumnInfo.PropertyPath == "ITEMID"))
                    {
                        if (filterRow != null) ViewModel.ValuesObjects.Add("DyedFabric", filterRow.FilterText);
                    }
                    paramter = "@DyedFabric";
                }
                if (ViewModel.TransactionHeader.TransactionType == 1 && f.FilterColumnInfo.PropertyPath == "ITEMID")
                {
                    ViewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                    ViewModel.Filter = ViewModel.Filter + "it.DyedFabric"+
                                        f.SelectedFilterOperation.LinqUse + paramter;
                }
                else
                {

                    ViewModel.Filter = ViewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                        f.SelectedFilterOperation.LinqUse + paramter;

                        ViewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);
                    }
             


                counter++;
            }
            ViewModel.GetOrderInfo();
            }
        }
    }
}