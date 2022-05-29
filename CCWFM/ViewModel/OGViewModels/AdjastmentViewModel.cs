using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using System;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.AuthenticationHelpers;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using CCWFM.Models.Inv;
using CCWFM.WarehouseService;
using System.Windows.Controls;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;
using CCWFM.Models.Items;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView;

namespace CCWFM.ViewModel.OGViewModels
{
    public class AdjustmentHeader : TblAdjustmentHeader
    {
        public AdjustmentHeader()
        {
            DocDate = DateTime.Now;
            CreationDate = DateTime.Now;
            Code = "";
            CreatedBy = LoggedUserInfo.Iserial;
        }
        [Required]
        public new string CountReference
        {
            get { return base.CountReference; }
            set { base.CountReference = value; }
        }
        [Range(1, double.MaxValue)]
        public new int WarehouseIserial
        {
            get { return base.WarehouseIserial; }
            set { base.WarehouseIserial = value; }
        }
        ObservableCollection<AdjustmentDetail> tblAdjustmentDetails;
        public new ObservableCollection<AdjustmentDetail> TblAdjustmentDetails
        {
            get { return tblAdjustmentDetails ?? (tblAdjustmentDetails = new ObservableCollection<AdjustmentDetail>()); }
            set { tblAdjustmentDetails = value; }
        }
        ObservableCollection<AdjustmentTempDetail> tblAdjustmentTempDetails;
        public new ObservableCollection<AdjustmentTempDetail> TblAdjustmentTempDetails
        {
            get { return tblAdjustmentTempDetails ?? (tblAdjustmentTempDetails = new ObservableCollection<AdjustmentTempDetail>()); }
            set { tblAdjustmentTempDetails = value; }
        }
    }
    public class AdjustmentDetail : TblAdjustmentDetail
    {
        [Range(1, double.MaxValue)]
        public new int ItemDimIserial
        {
            get { return base.ItemDimIserial; }
            set
            {
                base.ItemDimIserial = value;
                //هنا هشتغل
            }
        }
        bool isQuantityFocused;
        public bool IsQuantityFocused
        {
            get { return isQuantityFocused; }
            set { isQuantityFocused = value; RaisePropertyChanged(nameof(IsQuantityFocused)); }
        }
    }

    public class AdjustmentViewModel : ViewModelStructuredBase
    {
        WarehouseServiceClient WarehouseClient = Helpers.Services.Instance.GetWarehouseServiceClient();
        string itemDimIserial;
        public int ItemDimIserial { get { int o = 0; int.TryParse(itemDimIserial, out o); return o; } }
        public string ItemDimIserialStr
        {
            get { return itemDimIserial; }
            set { itemDimIserial = value; RaisePropertyChanged(nameof(ItemDimIserialStr)); }
        }      
        public decimal ItemDimQuantity { get { decimal o = -1; if (decimal.TryParse(itemDimQuantity, out o)) return o; else return -1; } }
        public string ItemDimQuantityStr
        {
            get { return itemDimQuantity; }
            set { itemDimQuantity = value; RaisePropertyChanged(nameof(ItemDimQuantityStr)); }
        }
        public AdjustmentViewModel() : base(PermissionItemName.Adjustment)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetComboData();
                OpenItemSearch = new RelayCommand<object>((o) =>
                {
                    try
                    {
                        var w = o as AdjustmentView;
                        if (w != null && SelectedMainRow.TblWarehouse != null && !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouse.Code))
                        {
                            var vm = new ItemDimensionAdjustmentSearchViewModel();
                            vm.WarehouseCode = SelectedMainRow.TblWarehouse.Code;
                            vm.SiteIserial = SelectedMainRow.TblWarehouse.TblSite;
                            vm.DocDate = SelectedMainRow.DocDate;
                            vm.AppliedSearchResultList.CollectionChanged += (s, e) =>
                            {
                                // هنا هبدا اعبى الى جاى من السيرش
                                foreach (var item in vm.AppliedSearchResultList)
                                {
                                    var temp = SelectedMainRow.TblAdjustmentDetails.FirstOrDefault(td =>
                                    td.ItemDimIserial == item.ItemDimFromIserial);
                                    //if (0 == item.DifferenceQuantity)
                                    //    continue;
                                    if (temp == null)// مش موجود
                                    {
                                        var adjustmentDetail = new AdjustmentDetail()
                                        {
                                            AdjustmentHeaderIserial = SelectedMainRow.Iserial,
                                            ItemDimIserial = item.ItemDimFromIserial,
                                            AvailableQuantity = item.AvailableQuantity,
                                            CountedQuantity = item.CountedQuantity,
                                            DifferenceQuantity = item.DifferenceQuantity,
                                            ItemAdjustment = item,
                                        };
                                        ValidateDetailRow(adjustmentDetail);
                                    }
                                    else// لو موجود هحدث الكمية
                                    {
                                        temp.ItemAdjustment.AvailableQuantity = item.AvailableQuantity;
                                        temp.ItemAdjustment.CountedQuantity = item.CountedQuantity;
                                        temp.ItemAdjustment.DifferenceQuantity = item.DifferenceQuantity;
                                        temp.AvailableQuantity = item.AvailableQuantity;
                                        temp.CountedQuantity = item.CountedQuantity;
                                        temp.DifferenceQuantity = item.DifferenceQuantity;
                                    }
                                }
                                RaisePropertyChanged(nameof(TotalCounted));
                                RaisePropertyChanged(nameof(TotalAvailable));
                            };
                            var childWindowSeach = new ItemDimensionAdjustmentSearchChildWindow(vm, hasCost: w.hasCost);
                            childWindowSeach.Show();
                            vm.Title = strings.Adjustment;
                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouse);
                    }
                    catch (Exception ex) { throw ex; }
                });
                ApproveCommand = new RelayCommand(() =>
                {
                    SelectedMainRow.Approved = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    if (NewCommand.CanExecute(null))
                        NewCommand.Execute(null);
                }, () => CheckCanApprove());
                DeleteAdjustmentDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblAdjustmentDetails.Remove(SelectedDetailRow);
                            if (SelectedMainRow.TblAdjustmentDetails.Count == 0)
                            {
                                AddNewDetailRow(false);
                            }
                        }
                        else
                            DeleteDetailRow();
                        RaisePropertyChanged(nameof(TotalCounted));
                        RaisePropertyChanged(nameof(TotalAvailable));
                        RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    }
                }, (o) =>
                {
                    return SelectedMainRow != null && !SelectedMainRow.Approved;
                });
                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    if (SelectedMainRow.TblAdjustmentDetails.Count < PageSize)
                    {
                        return;
                    }
                    if (SelectedMainRow.TblAdjustmentDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    {
                        GetDetailData();
                    }
                });
                WarehouseClient.GetItemDimensionQuantitiesByDateCompleted += (s, e) =>
                {
                    var temp = SelectedMainRow.TblAdjustmentDetails.FirstOrDefault(ad =>
                      ad.ItemDimIserial == e.Result.ItemDimFromIserial);
                    var qtemp = e.Result.AvailableQuantity;
                    if (e.Result.ItemDimFromIserial > 0)
                    {
                        if (temp != null)
                        {
                            SelectedDetailRow = temp;
                            SelectedDetailRow.ItemAdjustment.CountedQuantity += 1;
                            SelectedDetailRow.CountedQuantity = SelectedDetailRow.ItemAdjustment.CountedQuantity;
                        }
                        else// هعمل واحد جديد
                        {
                            temp = new AdjustmentDetail()
                            {
                                AdjustmentHeaderIserial = SelectedMainRow.Iserial,
                                CountedQuantity = 1,
                                ItemAdjustment = e.Result,
                                ItemDimIserial = e.Result.ItemDimFromIserial,
                            };
                            temp.AvailableQuantity = qtemp;
                            temp.ItemAdjustment.AvailableQuantity = qtemp;
                            temp.ItemAdjustment.CountedQuantity = 1;
                            ValidateDetailRow(temp);
                        }
                        if (ItemDimQuantity >= 0)
                        {
                            SelectedDetailRow.ItemAdjustment.CountedQuantity = ItemDimQuantity;
                            SelectedDetailRow.CountedQuantity = SelectedDetailRow.ItemAdjustment.CountedQuantity;
                        }
                    }
                        ItemDimQuantityStr = string.Empty;
                        ItemDimIserialStr = string.Empty;
                        RaisePropertyChanged(nameof(TotalCounted));
                        RaisePropertyChanged(nameof(TotalAvailable));
                };
                GetDetailItem = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Enter)
                    {
                        if (SelectedMainRow.TblWarehouse == null)
                        {
                            MessageBox.Show(strings.PleaseSelectWarehouse);
                            return;
                        }
                        WarehouseClient.GetItemDimensionQuantitiesByDateAsync(
                            SelectedMainRow.TblWarehouse.Code,
                            ItemDimIserial,
                            SelectedMainRow.DocDate);
                    }
                });
                ReturnToBarcode = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Enter)
                    {
                        foreach (var item in SelectedMainRow.TblAdjustmentDetails)
                        {
                            item.IsQuantityFocused = false;
                        }
                        IsRefFocused = true;
                    }
                });
                DetailSelectionChanged = new RelayCommand<object>((o) =>
                {
                    //var e = o as SelectionChangedEventArgs;
                    //// هنا هنقل الفوكس للكمية شوف بقى ازاى
                    //IsRefFocused = false;
                    //foreach (var item in SelectedMainRow.TblAdjustmentDetails)
                    //{
                    //    item.IsQuantityFocused = false;
                    //}
                    //SelectedDetailRow.IsQuantityFocused = true;
                });
                PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "AdjustmentApprove") != null)
                    {
                        CanApprove = true;
                        ApproveCommand.RaiseCanExecuteChanged();
                    }
                };
                GetCustomePermissions(PermissionItemName.Adjustment.ToString());

                MainRowList = new ObservableCollection<AdjustmentHeader>();
                AddNewMainRow(false);

                WarehouseClient.GetUserAsignedWarehousesForAdjustmentCompleted += (s, e) =>
                {
                    UserWarehouseList.Clear();
                    foreach (var item in e.Result)
                    {
                        TblAuthWarehouse temp = new TblAuthWarehouse();
                        temp.InjectFrom(item);
                        UserWarehouseList.Add(temp);
                    }
                };
                WarehouseClient.GetUserAsignedWarehousesForAdjustmentAsync(LoggedUserInfo.Iserial);

                WarehouseClient.GetAdjustmentCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new AdjustmentHeader() { IsOpeningBalance = isOpenningBalance, DocDate = DateTime.Now, CreationDate = DateTime.Now };
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (SearchWindow != null)
                    {
                        SearchWindow.FullCount = sv.fullCount;
                        SearchWindow.Loading = false;
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                    if (SelectedMainRow.Iserial <= 0)
                        SelectedMainRow.IsOpeningBalance = isOpenningBalance;
                };
                WarehouseClient.GetAdjustmentDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new AdjustmentDetail();
                        newrow.InjectFrom(row);
                        SelectedMainRow.TblAdjustmentDetails.Add(newrow);
                    }
                    if (!SelectedMainRow.TblAdjustmentDetails.Any())
                    {
                        AddNewDetailRow(false);
                    }
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    Loading = false;
                };
                WarehouseClient.UpdateOrInsertAdjustmentHeaderCompleted += (s, x) =>
                {
                    var savedRow = MainRowList.ElementAt(x.outindex);
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblAdjustmentDetails.Clear();
                        foreach (var item in x.Result.TblAdjustmentDetails)
                        {
                            var detailTemp = new AdjustmentDetail();
                            detailTemp.InjectFrom(item);
                            savedRow.TblAdjustmentDetails.Add(detailTemp);
                        }
                    }
                    savedRow.TblWarehouse = WarehouseList.FirstOrDefault(w => w.Iserial == savedRow.WarehouseIserial);
                    RaisePropertyChanged(nameof(IsReadOnly));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    DeleteCommand.RaiseCanExecuteChanged();
                    ApproveCommand.RaiseCanExecuteChanged();
                    DeleteAdjustmentDetail.RaiseCanExecuteChanged();
                    IsNewChanged();
                };
                WarehouseClient.UpdateOrInsertAdjustmentDetailCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.TblAdjustmentDetails.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                WarehouseClient.DeleteAdjustmentCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                WarehouseClient.DeleteAdjustmentDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.TblAdjustmentDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblAdjustmentDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                WarehouseClient.GetLookUpWarehouseForAdjustmentCompleted += (s, e) =>
                {
                    foreach (var row in e.Result)
                    {
                        var newrow = new TblWarehouse();
                        newrow.InjectFrom(row);
                        WarehouseList.Add(newrow);
                    }
                    Loading = false;
                };
                WarehouseClient.InsertImportedItemsCompleted += (s, e) =>
                {
                    if (e.IsCounting)
                    {
                        CountingHeaderIserial = e.Result;// رقم الهيدر
                        Loading = false;
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < RemainningImportList.Count; i = i + step)
                        {
                            bool approve = (i + step >= RemainningImportList.Count);//هل دى اخر لفة
                            var temp = new ObservableCollection<ImportedItemDimensionModel>(RemainningImportList.Skip(i).Take(step));
                            WarehouseClient.InsertRemainingImportedItemsAsync(e.Result, temp);// First Time
                            requestes++;
                        }
                        ImportHeaderIserial = e.Result;
                        Loading = false;
                    }
                };
                WarehouseClient.InsertRemainingImportedItemsCompleted += (s, e) =>
                {
                    requestes--;// على اساس ان الريكويست اسرع من الريسبونس
                   
                    foreach (var item in e.Result)
                    {
                        error += item + "\r\n";
                    }                    
                    if (e.Error != null)
                    {
                        requestes = -1;
                        throw e.Error;
                    }
                    else if (requestes == 0)// كده ده اخر واحد
                    {
                        if (string.IsNullOrWhiteSpace(error))
                        {
                            MessageBox.Show("Import Completed Succesfully");
                            if (CountingHeaderIserial > 0)//كده فى كونتينج
                                WarehouseClient.ApproveAdjustmentByIserialAsync(ImportHeaderIserial, LoggedUserInfo.Iserial, CountingHeaderIserial);
                            else
                                WarehouseClient.ApproveAdjustmentByIserialAsync(ImportHeaderIserial, LoggedUserInfo.Iserial, -1);
                            ImportHeaderIserial = -1;
                            CountingHeaderIserial = -1;
                            return;
                        }
                        WarehouseClient.DeleteAdjustmentByIserialAsync(ImportHeaderIserial, CountingHeaderIserial);
                        ImportHeaderIserial = -1;
                        CountingHeaderIserial = -1;
                        if (MessageBox.Show("Import Completed, Do you want to view logs?", "Info", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            new LogView(error).Show();
                            error = "";
                        }
                    }
                };
                WarehouseClient.ApproveAdjustmentByIserialCompleted += (s, e) =>
                {

                };
                WarehouseClient.DeleteAdjustmentByIserialCompleted += (s, e) =>
                {

                };
                OpenTempData = new RelayCommand<object>((o) =>
                {
                    var w = o as AdjustmentView;
                    if (w != null && ValidHeaderData() && !IsHeaderHasDetails)
                    {
                        AdjustmentTempView tempView = new AdjustmentTempView(w.hasCost);
                        tempView.Show();
                        var temp = new AdjustmentTempViewModel(isOpenningBalance);
                        temp.SelectedMainRow = this.SelectedMainRow;
                        tempView.DataContext = temp;
                        if (SelectedMainRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblAdjustmentDetails = new ObservableCollection<AdjustmentDetail>();
                            SaveMainRow();
                        }
                    }
                    else
                        MessageBox.Show("Can not Insert Temp Records");
                });
                ImportFromExcelCommand = new RelayCommand(() => {
                    if (SelectedMainRow.WarehouseIserial <= 0) { MessageBox.Show(strings.PleaseSelectWarehouse); return; }
                    if (string.IsNullOrWhiteSpace(SelectedMainRow.CountReference)) { MessageBox.Show(strings.ReqCountReference); return; }
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Excel Files (*.xls)|*.xls";
                    if (ofd.ShowDialog() == true)
                    {
                        if (!isOpenningBalance)
                        {
                            if (MessageBox.Show("If this is counting adjustment press Ok, if not press Cancel", "Adjustment", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                IsCountingAdjustment = true;
                            }
                        }
                        else IsCountingAdjustment = false;
                        var importedList = new ObservableCollection<ImportedItemDimensionModel>();
                        var fs = ofd.File.OpenRead();

                        var book = Workbook.Open(fs);
                        var sheet = book.Worksheets[0];
                        var itemIdIndex = 0;
                        var colorIndex = 0;
                        var sizeIndex = 0;
                        var batchNoIndex = 0;
                        var quantityIndex = 0;
                        var costIndex = 0;
                        for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                        {
                            switch (sheet.Cells[0, j].StringValue.ToLower())
                            {
                                case "itemid":
                                    itemIdIndex = j;
                                    break;
                                case "color":
                                    colorIndex = j;
                                    break;
                                case "size":
                                    sizeIndex = j;
                                    break;
                                case "batchno":
                                    batchNoIndex = j;
                                    break;
                                case "quantity":
                                    quantityIndex = j;
                                    break;
                                case "cost":
                                    costIndex = j;
                                    break;
                            }
                        }
                        for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
                        {
                            var newemp = new ImportedItemDimensionModel();
                            if (sheet.Cells[i, itemIdIndex].Value != null)
                            {
                                var itemId = sheet.Cells[i, itemIdIndex].Value.ToString().ToUpper().Trim();
                                newemp.ItemCode = itemId;
                            }
                            if (sheet.Cells[i, colorIndex].Value != null)
                            {
                                var color = sheet.Cells[i, colorIndex].Value.ToString().ToUpper().Trim();
                                newemp.Color = color;
                            }
                            if (sheet.Cells[i, sizeIndex].Value != null)
                            {
                                var size = sheet.Cells[i, sizeIndex].Value.ToString().ToUpper().Trim();
                                newemp.Size = size;
                            }
                            if (sheet.Cells[i, batchNoIndex].Value != null)
                            {
                                var batchNo = sheet.Cells[i, batchNoIndex].Value.ToString().ToUpper().Trim();
                                newemp.BatchNo = batchNo;
                            }
                            if (sheet.Cells[i, quantityIndex].Value != null)
                            {
                                var qty = sheet.Cells[i, quantityIndex].Value.ToString().ToUpper().Trim();
                                newemp.Qty = Convert.ToDecimal(qty);
                            }
                            if (sheet.Cells[i, costIndex].Value != null)
                            {
                                var cost = sheet.Cells[i, costIndex].Value.ToString().ToUpper().Trim();
                                decimal temp = 0;
                                decimal.TryParse(cost, out temp);
                                newemp.Cost = temp;
                            }
                            importedList.Add(newemp);
                        }
                        InsertImportedDetail(importedList);
                    }
                });
                GetMaindata();
            }
        }
        int step = 300, requestes = 0, ImportHeaderIserial = -1, CountingHeaderIserial = -1;
        string error = "", itemDimQuantity;
        bool IsCountingAdjustment = false;

        ObservableCollection<ImportedItemDimensionModel> RemainningImportList;

        #region Methods

        private void InsertImportedDetail(ObservableCollection<ImportedItemDimensionModel> importedList)
        {
            TblAdjustmentHeader headerRow = new TblAdjustmentHeader();
            headerRow.InjectFrom(SelectedMainRow);
            headerRow.Approved = true;
            headerRow.ApproveDate = DateTime.Now;
            headerRow.ApprovedBy = LoggedUserInfo.Iserial;
            if (ValidData())
            {
                var temp = new ObservableCollection<ImportedItemDimensionModel>(importedList.Skip(0).Take(0));
                //bool approve = importedList.Count <= step;
                WarehouseClient.InsertImportedItemsAsync(headerRow, temp, false);// Adjustment Header
                System.Threading.Thread.Sleep(1500);
                if (IsCountingAdjustment)
                    WarehouseClient.InsertImportedItemsAsync(headerRow, temp, true);// Counting Header
                //if (!approve)//ده كله
                //{
                    RemainningImportList = new ObservableCollection<ImportedItemDimensionModel>(
                        importedList.Skip(0));
                //}
            }

        }
        private void ValidateDetailRow(AdjustmentDetail adjustmentDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblAdjustmentDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblAdjustmentDetails.Add(adjustmentDetail);
            }
            else
            {
                SelectedMainRow.TblAdjustmentDetails.Add(adjustmentDetail);
            }
            SelectedDetailRow = adjustmentDetail;
            RaisePropertyChanged(nameof(TotalCounted));
            RaisePropertyChanged(nameof(TotalAvailable));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }
        private bool CheckCanApprove()
        {
            return CanApprove && !SelectedMainRow.Approved && UserWarehouseList.Any(uw =>
            uw.PermissionType == 1 || uw.PermissionType == (short)AuthWarehouseType.Adjustment
            && uw.WarehouseIserial == SelectedMainRow.WarehouseIserial);
        }
        public bool ValidHeaderData()
        {
            if (string.IsNullOrWhiteSpace(SelectedMainRow.CountReference))
            {
                MessageBox.Show(strings.ReqCountReference);
                return false;
            }
            if (!UserWarehouseList.Any(uw =>
                 uw.WarehouseIserial == SelectedMainRow.WarehouseIserial))
            {
                MessageBox.Show("Please Choose one of your assigned warehouses");
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.Approved)
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
      
        #endregion

        #region Operations

        public void GetMaindata()
        {
            //if (SortBy == null)
                SortBy = "it.Iserial desc";
            WarehouseClient.GetAdjustmentAsync(MainRowList.Count, PageSize, SortBy, Filter,
                ValuesObjects, LoggedUserInfo.Iserial, isOpenningBalance);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WarehouseClient.DeleteAdjustmentAsync((TblAdjustmentHeader)new
                        TblAdjustmentHeader().InjectFrom(SelectedMainRow),
                        MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                var temp = new AdjustmentHeader()
                {
                    IsOpeningBalance = isOpenningBalance,
                    DocDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                };
                SelectedMainRow = temp;
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                AddNewDetailRow(false);
                RaisePropertyChanged(nameof(TotalCounted));
                RaisePropertyChanged(nameof(TotalAvailable));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                isvalid &= UserWarehouseList.Any(uw =>
                 uw.WarehouseIserial == SelectedMainRow.WarehouseIserial);
                if (isvalid)
                {
                    var saveRow = new TblAdjustmentHeader()
                    {
                        DocDate = DateTime.Now,
                        CreationDate = DateTime.Now,
                    };
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblAdjustmentDetails = new ObservableCollection<TblAdjustmentDetail>();
                    if (string.IsNullOrWhiteSpace(saveRow.Code))
                        saveRow.Code = "";
                    foreach (var item in SelectedMainRow.TblAdjustmentDetails.Where(W=>W.ItemDimIserial!=0))
                    {
                        var detailTemp = new TblAdjustmentDetail();
                        detailTemp.InjectFrom(item);
                        detailTemp.TblItemDim = null;
                        detailTemp.TblAdjustmentHeader = saveRow;
                        saveRow.TblAdjustmentDetails.Add(detailTemp);
                    }

                    var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    if (mainRowIndex < 0)
                    {
                        MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    }
                    WarehouseClient.UpdateOrInsertAdjustmentHeaderAsync(saveRow
                    , MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
                }
                else
                {
                    MessageBox.Show("Please Choose one of your assigned warehouses");
                }
            }
        }
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
                WarehouseClient.GetAdjustmentDetailAsync(SelectedMainRow.TblAdjustmentDetails.Count, PageSize, SelectedMainRow.Iserial);
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You sure to Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    //foreach (var row in SelectedDetailRows)
                    //{
                    //    WarehouseClient.DeleteAdjustmentDetailAsync((TblAdjustmentDetail)new TblAdjustmentDetail().InjectFrom(row), SelectedMainRow.TblAdjustmentDetails.IndexOf(row));
                    //}
                        WarehouseClient.DeleteAdjustmentDetailAsync((TblAdjustmentDetail)new TblAdjustmentDetail().InjectFrom(SelectedDetailRow),
                            SelectedMainRow.TblAdjustmentDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblAdjustmentDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblAdjustmentDetails.Count - 1))
            {
                if (checkLastRow && SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(
                        SelectedDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.TblAdjustmentDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new AdjustmentDetail
                {
                    AdjustmentHeaderIserial = SelectedMainRow.Iserial
                });
                RaisePropertyChanged(nameof(TotalCounted));
                RaisePropertyChanged(nameof(TotalAvailable));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new TblAdjustmentDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    WarehouseClient.UpdateOrInsertAdjustmentDetailAsync(rowToSave, SelectedMainRow.TblAdjustmentDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            WarehouseClient.GetLookUpWarehouseForAdjustmentAsync(LoggedUserInfo.Iserial);
        }

        #endregion

        #region Properties

        private ObservableCollection<AdjustmentHeader> _mainRowList;
        public ObservableCollection<AdjustmentHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }
        private ObservableCollection<AdjustmentHeader> _selectedMainRows;
        public ObservableCollection<AdjustmentHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<AdjustmentHeader>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private AdjustmentHeader _selectedMainRow;
        public AdjustmentHeader SelectedMainRow
        {
            get
            {
                if (_selectedMainRow == null)
                {
                    _selectedMainRow = new AdjustmentHeader()
                    {
                        IsOpeningBalance = isOpenningBalance,
                        DocDate = DateTime.Now,
                        CreationDate = DateTime.Now,
                    };
                }
                return _selectedMainRow;
            }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                ApproveCommand.RaiseCanExecuteChanged();
                DeleteAdjustmentDetail.RaiseCanExecuteChanged();
                GetDetailData();
                IsNewChanged();
            }
        }
        private AdjustmentDetail _selectedDetailRow;
        public AdjustmentDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new AdjustmentDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }
        private ObservableCollection<AdjustmentDetail> _selectedDetailRows;
        public ObservableCollection<AdjustmentDetail> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<AdjustmentDetail>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged(nameof(SelectedDetailRows)); }
        }

        #region Combo Data

        ObservableCollection<TblWarehouse> _warehouseList = new ObservableCollection<TblWarehouse>();
        public ObservableCollection<TblWarehouse> WarehouseList
        {
            get { return _warehouseList; }
            set { _warehouseList = value; RaisePropertyChanged(nameof(WarehouseList)); }
        }

        #endregion

        public virtual bool IsReadOnly
        {
            get { return SelectedMainRow != null && SelectedMainRow.Iserial > 0 && SelectedMainRow.Approved; }
        }
        public virtual bool IsHeaderHasDetails
        {
            get { return SelectedMainRow.TblAdjustmentDetails.Any(d => d.ItemDimIserial > 0) || IsReadOnly; }
        }
        private bool canApprove;
        public bool CanApprove
        {
            get { return canApprove; }
            set { canApprove = value; RaisePropertyChanged(nameof(CanApprove)); ApproveCommand.RaiseCanExecuteChanged(); }
        }

        ObservableCollection<TblAuthWarehouse> _userWarehouseList = new ObservableCollection<TblAuthWarehouse>();
        public ObservableCollection<TblAuthWarehouse> UserWarehouseList
        {
            get { return _userWarehouseList ?? (_userWarehouseList = new ObservableCollection<TblAuthWarehouse>()); }
            set { _userWarehouseList = value; RaisePropertyChanged(nameof(UserWarehouseList)); }
        }
        public override bool IsNew
        {
            get { return SelectedMainRow.Iserial <= 0; }//base.IsNew && 
            set { base.IsNew = value; }
        }
        public decimal TotalCounted
        {
            get { return SelectedMainRow.TblAdjustmentDetails.Sum(td => td.CountedQuantity); }
        }
        public decimal TotalAvailable
        {
            get { return SelectedMainRow.TblAdjustmentDetails.Sum(td => td.AvailableQuantity); }
        }
     
        #endregion

        #region Commands

        RelayCommand<object> openItemSearch;
        public RelayCommand<object> OpenItemSearch
        {
            get { return openItemSearch; }
            set { openItemSearch = value; RaisePropertyChanged(nameof(OpenItemSearch)); }
        }

        RelayCommand approveTransfer;
        public RelayCommand ApproveCommand
        {
            get { return approveTransfer; }
            set { approveTransfer = value; RaisePropertyChanged(nameof(ApproveCommand)); }
        }

        RelayCommand<object> deleteTransferDetail;
        public RelayCommand<object> DeleteAdjustmentDetail
        {
            get { return deleteTransferDetail; }
            set { deleteTransferDetail = value; RaisePropertyChanged(nameof(DeleteAdjustmentDetail)); }
        }

        RelayCommand<object> getDetailItem;
        public RelayCommand<object> GetDetailItem
        {
            get { return getDetailItem; }
            set { getDetailItem = value; RaisePropertyChanged(nameof(GetDetailItem)); }
        }

        RelayCommand<object> returnToBarcode;
        public RelayCommand<object> ReturnToBarcode
        {
            get { return returnToBarcode; }
            set { returnToBarcode = value; RaisePropertyChanged(nameof(ReturnToBarcode)); }
        }

        RelayCommand<object> detailSelectionChanged;
        public RelayCommand<object> DetailSelectionChanged
        {
            get { return detailSelectionChanged; }
            set { detailSelectionChanged = value; RaisePropertyChanged(nameof(DetailSelectionChanged)); }
        }
        RelayCommand importFromExcelCommand;
        public RelayCommand ImportFromExcelCommand
        {
            get { return importFromExcelCommand; }
            set { importFromExcelCommand = value; RaisePropertyChanged(nameof(ImportFromExcelCommand)); }
        }
        RelayCommand<object> openTempData;
        public RelayCommand<object> OpenTempData
        {
            get { return openTempData; }
            set { openTempData = value; RaisePropertyChanged(nameof(OpenTempData)); }
        }
        RelayCommand<object> loadingDetailRows;
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }
        bool isRefFocused;
        internal bool isOpenningBalance;

        public bool IsRefFocused
        {
            get { return isRefFocused; }
            set { isRefFocused = value; RaisePropertyChanged(nameof(IsRefFocused)); }
        }
        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
            RaisePropertyChanged(nameof(IsReadOnly));
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData() && ValidDetailData();
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<AdjustmentHeader> vm =
                new GenericSearchViewModel<AdjustmentHeader>() { Title = "Adjustment Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) =>
            {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) =>
            {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header=strings.Code,
                        PropertyPath=nameof(AdjustmentHeader.Code),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WareHouse,
                        PropertyPath= string.Format("{0}.{1}", nameof(AdjustmentHeader.TblWarehouse),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(AdjustmentHeader.TblWarehouse),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Date,
                        PropertyPath=nameof(AdjustmentHeader.DocDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Approved,
                        PropertyPath=nameof(AdjustmentHeader.Approved),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ApproveDate,
                        PropertyPath=nameof(AdjustmentHeader.ApproveDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                };
        }
        public override void DeleteRecord()
        {
            this.DeleteMainRow();
            AddNewMainRow(false);
            base.DeleteRecord();
        }
        public override bool CanDeleteRecord()
        {
            return SelectedMainRow.Iserial > 0 &&
            SelectedMainRow.Approved == false;
        }
        public override void Cancel()
        {
            MainRowList.Clear();
            SelectedMainRows.Clear();
            SelectedDetailRows.Clear();
            AddNewMainRow(false);
            RaisePropertyChanged(nameof(IsReadOnly));
            base.Cancel();
        }
        public override void Print()
        {
            base.Print();
            var rVM = new GenericReportViewModel();

            rVM.GenerateReport("AdjustmentDocument", new ObservableCollection<string>() { SelectedMainRow.Iserial.ToString() });
        }
        #endregion

    }
}