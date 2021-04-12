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
using CCWFM.Helpers.Utilities;

namespace CCWFM.ViewModel.OGViewModels
{
    public class AdjustmentTempDetail : TblAdjustmentTempDetail
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
    
    public class AdjustmentTempViewModel : ViewModelStructuredBase
    {
        WarehouseService.WarehouseServiceClient WarehouseClient = Helpers.Services.Instance.GetWarehouseServiceClient();
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
        public AdjustmentTempViewModel(bool hasCost) : base(PermissionItemName.Adjustment)
        {
            this.hasCost = hasCost;
            if (!DesignerProperties.IsInDesignTool)
            {
                OpenItemSearch = new RelayCommand<object>((o) =>
                {
                    try
                    {
                        if (SelectedMainRow.TblWarehouse != null && !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouse.Code))
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
                                    var temp = SelectedMainRow.TblAdjustmentTempDetails.FirstOrDefault(td =>
                                    td.ItemDimIserial == item.ItemDimFromIserial);
                                    if (0 == item.DifferenceQuantity)
                                        continue;
                                    if (temp == null)// مش موجود
                                    {
                                        var adjustmentDetail = new AdjustmentTempDetail()
                                        {
                                            AdjustmentHeaderIserial = SelectedMainRow.Iserial,
                                            ItemDimIserial = item.ItemDimFromIserial,
                                            AvailableQuantity = item.AvailableQuantity,
                                            CountedQuantity = item.CountedQuantity,
                                            DifferenceQuantity = item.DifferenceQuantity,
                                            ItemAdjustment = item,
                                        };
                                        ValidateDetailRow(adjustmentDetail);
                                        SaveDetailRow();
                                    }
                                    else// لو موجود هحدث الكمية
                                    {
                                        temp.ItemAdjustment.AvailableQuantity = item.AvailableQuantity;
                                        temp.ItemAdjustment.CountedQuantity = item.CountedQuantity;
                                        temp.ItemAdjustment.DifferenceQuantity = item.DifferenceQuantity;
                                        temp.AvailableQuantity = item.AvailableQuantity;
                                        temp.CountedQuantity = item.CountedQuantity;
                                        temp.DifferenceQuantity = item.DifferenceQuantity;
                                        SelectedDetailRow = temp;
                                        SaveDetailRow();
                                    }
                                }
                                RaisePropertyChanged(nameof(TotalCounted));
                                RaisePropertyChanged(nameof(TotalAvailable));
                            };
                            var childWindowSeach = new ItemDimensionAdjustmentSearchChildWindow(vm,((AdjustmentView)o).hasCost);
                            childWindowSeach.Show();
                            vm.Title = strings.Adjustment;
                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouse);
                    }
                    catch (Exception ex) { throw ex; }
                });
               
                DeleteAdjustmentDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete)
                    {
                        DeleteDetailRow();
                    }
                }, (o) =>
                {
                    return SelectedMainRow != null && !SelectedMainRow.Approved;
                });
                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    if (SelectedMainRow.TblAdjustmentTempDetails.Count < PageSize)
                    {
                        return;
                    }
                    if (SelectedMainRow.TblAdjustmentTempDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    {
                        GetDetailData();
                    }
                });

                WarehouseClient.GetAdjustmentTempDetailCompleted += (s, e) =>
                {
                    AdjustmentTempDetail temp = new AdjustmentTempDetail();
                    temp.InjectFrom(e.Result);
                    ValidateDetailRow(temp);
                    SelectedDetailRow.ItemAdjustment.CountedQuantity += 1;
                    if (ItemDimQuantity >= 0)
                    {
                        SelectedDetailRow.ItemAdjustment.CountedQuantity += ItemDimQuantity;
                    }
                    SelectedDetailRow.CountedQuantity = SelectedDetailRow.ItemAdjustment.CountedQuantity;
                    SelectedDetailRow.DifferenceQuantity = SelectedDetailRow.ItemAdjustment.DifferenceQuantity;
                    SaveDetailRow();
                    ItemDimQuantityStr = string.Empty;
                    ItemDimIserialStr = string.Empty;
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                };
                WarehouseClient.GetItemDimensionQuantitiesByDateCompleted += (s, e) =>
                {
                    InsertOrUpdateItemDetail(e.Result);
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
                TempRecordLostFocus = new RelayCommand<object>((o) =>
                {
                    if (SelectedMainRow.TblWarehouse == null)
                    {
                        MessageBox.Show(strings.PleaseSelectWarehouse);
                        return;
                    }
                    WarehouseClient.GetItemDimensionQuantitiesByDateAsync(
                        SelectedMainRow.TblWarehouse.Code,
                        SelectedDetailRow.ItemDimIserial,
                        SelectedMainRow.DocDate);
                    isQuantityTextChanged = true;
                });

                ReturnToBarcode = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Enter)
                    {
                        foreach (var item in SelectedMainRow.TblAdjustmentTempDetails)
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
                    //foreach (var item in SelectedMainRow.TblAdjustmentTempDetails)
                    //{
                    //    item.IsQuantityFocused = false;
                    //}
                    //SelectedDetailRow.IsQuantityFocused = true;
                });
                
                WarehouseClient.GetAdjustmentTempDetailsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new AdjustmentTempDetail();
                        newrow.InjectFrom(row);
                        SelectedMainRow.TblAdjustmentTempDetails.Add(newrow);
                    }
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                    Loading = false;
                    if (IsExport && grid != null)
                    {
                        IsExport = false;
                        SelectedMainRow.TblAdjustmentTempDetails.Select(aTD => new {
                            ItemDim = aTD.ItemDimIserial,
                            ItemId = aTD.ItemAdjustment.ItemId,
                            ItemName = aTD.ItemAdjustment.ItemName,
                            itemCode = aTD.ItemAdjustment.ItemCode,
                            Color = aTD.ItemAdjustment.ColorFromCode,
                            Size = aTD.ItemAdjustment.SizeFrom,
                            BatchNo = aTD.ItemAdjustment.BatchNoFrom,
                            Available = aTD.ItemAdjustment.AvailableQuantity,
                            OldQuantity = aTD.OldQuantity,
                            CountedQuantity = aTD.CountedQuantity,
                            Differance = aTD.DifferenceQuantity,
                        })
                        .ExportExcel(fileStream, "Adjustment Items");
                    }
                };

                WarehouseClient.UpdateOrInsertAdjustmentTempDetailCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.TblAdjustmentTempDetails.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                WarehouseClient.DeleteAdjustmentTempDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblAdjustmentTempDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblAdjustmentTempDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                };
                Approve = new RelayCommand<object>((o) => {
                    view = o as ControlsOverride.ChildWindowsOverride;
                    WarehouseClient.ApproveAdjustmentTempDetailAsync(SelectedMainRow.Iserial);
                });
                WarehouseClient.ApproveAdjustmentTempDetailCompleted += (s, e) =>
                {
                    if (e.Error != null)
                        throw e.Error;
                    if (view != null)
                    {
                        view.Close();
                    }
                };

                ExportToExcel = new RelayCommand<object>((o) => {
                    IsExport = true;
                    grid = o as Os.Controls.DataGrid.OsGrid;
                    var sDialog = new SaveFileDialog { Filter = "Excel Files(*.xls)|*.xls" };
                    if (sDialog.ShowDialog() == true)
                    {
                        fileStream = sDialog.OpenFile();
                    }
                    if (SelectedMainRow != null)
                    {
                        SelectedMainRow.TblAdjustmentTempDetails.Clear();
                        WarehouseClient.GetAdjustmentTempDetailsAsync(0, int.MaxValue, SelectedMainRow.Iserial, null);
                    }
                });
                Exit = new RelayCommand<object>((o) => {
                    var view = o as ControlsOverride.ChildWindowsOverride;
                    if (view != null) view.Close();
                });

                SearchDetails = new RelayCommand<object>((o) => {
                    if (((KeyEventArgs)(o)).Key == Key.Enter)
                    {
                        SelectedMainRow.TblAdjustmentTempDetails.Clear();// هنفضى الريكوردات
                        GetDetailData();
                    }
                });
            }
        }
        bool isQuantityTextChanged = false, IsExport = false;
        Os.Controls.DataGrid.OsGrid grid;
        Stream fileStream = null;
        ControlsOverride.ChildWindowsOverride view = null;
        private void InsertOrUpdateItemDetail(Web.DataLayer.ItemDimensionAdjustmentSearchModel e)
        {
            var temp = SelectedMainRow.TblAdjustmentTempDetails.FirstOrDefault(ad =>
              ad.ItemDimIserial == e.ItemDimFromIserial);
            var qtemp = e.AvailableQuantity;
            if (e.ItemDimFromIserial > 0)
            {
                if (temp != null)// هعمل تحديث
                {
                    SelectedDetailRow = temp;
                    if (!isQuantityTextChanged)
                    {
                        if (ItemDimQuantity >= 0)
                            SelectedDetailRow.ItemAdjustment.CountedQuantity += ItemDimQuantity;
                        else
                            SelectedDetailRow.ItemAdjustment.CountedQuantity += 1;
                    }
                    else
                        isQuantityTextChanged = false;
                    SelectedDetailRow.CountedQuantity = SelectedDetailRow.ItemAdjustment.CountedQuantity;
                    SelectedDetailRow.DifferenceQuantity = SelectedDetailRow.ItemAdjustment.DifferenceQuantity;
                    SaveDetailRow();
                    ItemDimQuantityStr = string.Empty;
                    ItemDimIserialStr = string.Empty;
                    RaisePropertyChanged(nameof(TotalCounted));
                    RaisePropertyChanged(nameof(TotalAvailable));
                }
                else// هعمل واحد جديد او هجيب القديم
                {
                    WarehouseClient.GetAdjustmentTempDetailAsync(
                   SelectedMainRow.Iserial, e.ItemDimFromIserial);
                }
            }
        }

        string itemDimQuantity;

        #region Methods
        private void ValidateDetailRow(AdjustmentTempDetail adjustmentDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblAdjustmentTempDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblAdjustmentTempDetails.Add(adjustmentDetail);
            }
            else
            {
                SelectedMainRow.TblAdjustmentTempDetails.Add(adjustmentDetail);
            }
            SelectedDetailRow = adjustmentDetail;
            RaisePropertyChanged(nameof(TotalCounted));
            RaisePropertyChanged(nameof(TotalAvailable));
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.Approved && SelectedMainRow.TblAdjustmentTempDetails.Any(td => 0 == td.DifferenceQuantity))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
        #endregion

        #region Operations        
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
                WarehouseClient.GetAdjustmentTempDetailsAsync(SelectedMainRow.TblAdjustmentTempDetails.Count,
                    PageSize, SelectedMainRow.Iserial, isSearch ? SearchRow : null);
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var res = MessageBox.Show("Are You sure to Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WarehouseClient.DeleteAdjustmentTempDetailAsync((TblAdjustmentTempDetail)new TblAdjustmentTempDetail().InjectFrom(SelectedDetailRow), SelectedMainRow.TblAdjustmentTempDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblAdjustmentTempDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblAdjustmentTempDetails.Count - 1))
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
                SelectedMainRow.TblAdjustmentTempDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new AdjustmentTempDetail
                {
                    AdjustmentHeaderIserial = SelectedMainRow.Iserial
                });
                RaisePropertyChanged(nameof(TotalCounted));
                RaisePropertyChanged(nameof(TotalAvailable));
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
                    var rowToSave = new TblAdjustmentTempDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    WarehouseClient.UpdateOrInsertAdjustmentTempDetailAsync(rowToSave, SelectedMainRow.TblAdjustmentTempDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        #endregion

        #region Properties        
        private AdjustmentHeader _selectedMainRow;
        public AdjustmentHeader SelectedMainRow
        {
            get
            {
                if (_selectedMainRow == null)
                {
                    _selectedMainRow = new AdjustmentHeader()
                    {
                        IsOpeningBalance = hasCost,
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
                DeleteAdjustmentDetail.RaiseCanExecuteChanged();
                GetDetailData();
                IsNewChanged();
            }
        }
        private AdjustmentTempDetail _selectedDetailRow;
        public AdjustmentTempDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new AdjustmentTempDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }
        
        private Web.DataLayer.TblAdjustmentTempDetailSearch searchRow;
        public Web.DataLayer.TblAdjustmentTempDetailSearch SearchRow
        {
            get { return searchRow ?? (searchRow = new Web.DataLayer.TblAdjustmentTempDetailSearch()); }
            set { searchRow = value; RaisePropertyChanged(nameof(SearchRow)); }
        }
        public decimal TotalCounted
        {
            get { return SelectedMainRow.TblAdjustmentTempDetails.Sum(td => td.CountedQuantity); }
        }
        public decimal TotalAvailable
        {
            get { return SelectedMainRow.TblAdjustmentTempDetails.Sum(td => td.AvailableQuantity); }
        }
        #endregion

        #region Commands
        RelayCommand<object> openItemSearch;
        public RelayCommand<object> OpenItemSearch
        {
            get { return openItemSearch; }
            set { openItemSearch = value; RaisePropertyChanged(nameof(OpenItemSearch)); }
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
        RelayCommand<object> loadingDetailRows;
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }
        RelayCommand<object> tempRecordLostFocus;
        public RelayCommand<object> TempRecordLostFocus
        {
            get { return tempRecordLostFocus; }
            set { tempRecordLostFocus = value; RaisePropertyChanged(nameof(TempRecordLostFocus)); }
        }
        RelayCommand<object> approve;
        public RelayCommand<object> Approve
        {
            get { return approve; }
            set { approve = value; RaisePropertyChanged(nameof(Approve)); }
        }
        RelayCommand<object> exportToExcel;
        public RelayCommand<object> ExportToExcel
        {
            get { return exportToExcel; }
            set { exportToExcel = value; RaisePropertyChanged(nameof(ExportToExcel)); }
        }
        RelayCommand<object> exit;
        public RelayCommand<object> Exit
        {
            get { return exit; }
            set { exit = value; RaisePropertyChanged(nameof(Exit)); }
        }
        RelayCommand<object> searchDetails;
        public RelayCommand<object> SearchDetails
        {
            get { return searchDetails; }
            set { searchDetails = value; RaisePropertyChanged(nameof(SearchDetails)); }
        }
        bool isRefFocused;
        private bool hasCost;

        private bool isSearch
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SearchRow.ItemDimIserialStr) ||
                    !string.IsNullOrWhiteSpace(SearchRow.ItemCode) ||
                    !string.IsNullOrWhiteSpace(SearchRow.ItemName) ||
                    !string.IsNullOrWhiteSpace(SearchRow.ColorCode) ||
                    !string.IsNullOrWhiteSpace(SearchRow.Size) ||
                    !string.IsNullOrWhiteSpace(SearchRow.BatchNo) ||
                    !string.IsNullOrWhiteSpace(SearchRow.AvailableQuantityStr) ||
                    !string.IsNullOrWhiteSpace(SearchRow.CountedQuantityStr) ||
                    !string.IsNullOrWhiteSpace(SearchRow.DifferenceQuantityStr);
            }
        }

        public bool IsRefFocused
        {
            get { return isRefFocused; }
            set { isRefFocused = value; RaisePropertyChanged(nameof(IsRefFocused)); }
        }
        #endregion
        
    }
}