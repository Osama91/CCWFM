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

namespace CCWFM.ViewModel.OGViewModels
{
    public class TransferHeader : TblTransferHeader
    {
        public TransferHeader()
        {
            DocDate = DateTime.Now;
            CodeFrom = "";
            CodeTo = "";
            CreatedBy = LoggedUserInfo.Iserial;
            CreationDate = DateTime.Now;
        }
        [Range(1, double.MaxValue)]
        public new int WarehouseFrom
        {
            get { return base.WarehouseFrom; }
            set { base.WarehouseFrom = value; }
        }
        [Range(1, double.MaxValue)]
        public new int WarehouseTo
        {
            get { return base.WarehouseTo; }
            set { base.WarehouseTo = value; }
        }
        ObservableCollection<TransferDetail> tblTransferDetails;
        public new ObservableCollection<TransferDetail> TblTransferDetails
        {
            get { return tblTransferDetails ?? (tblTransferDetails = new ObservableCollection<TransferDetail>()); }
            set { tblTransferDetails = value; RaisePropertyChanged(nameof(TblTransferDetails)); }
        }
    }
    public class TransferDetail : TblTransferDetail
    {
        [Range(1, double.MaxValue)]
        public new int ItemDimFrom
        {
            get { return base.ItemDimFrom; }
            set { base.ItemDimFrom = value; }
        }
        [Range(1, double.MaxValue)]
        public new int ItemDimTo
        {
            get { return base.ItemDimTo; }
            set { base.ItemDimTo = value; }
        }
        ItemDimensionSearchModel itemTransfer;
        public new ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new ItemDimensionSearchModel()); }
            set { itemTransfer = value; RaisePropertyChanged(nameof(ItemTransfer)); }
        }
        bool isQuantityFocused;
        public bool IsQuantityFocused
        {
            get { return isQuantityFocused; }
            set { isQuantityFocused = value; RaisePropertyChanged(nameof(IsQuantityFocused)); }
        }

        private string _itemFPCode;
        public string ItemFPCode
        {
            get { return _itemFPCode; }
            set { _itemFPCode = value; RaisePropertyChanged(nameof(ItemFPCode)); }
        }
        
        private string _itemFPName;
        public string ItemFPName
        {
            get { return _itemFPName; }
            set { _itemFPName = value; RaisePropertyChanged(nameof(ItemFPName)); }
        }
        private int? _itemFPIserial;
        public int? ItemFPIserial
        {
            get { return _itemFPIserial; }
            set { _itemFPIserial = value; RaisePropertyChanged(nameof(ItemFPIserial)); }
        }
    }
    public class TransferViewModel : ViewModelStructuredBase
    {
        WarehouseService.WarehouseServiceClient WarehouseClient =
                 new WarehouseService.WarehouseServiceClient();
        int itemDimFromIserial;
        public int ItemDimFromIserial
        {
            get { return itemDimFromIserial; }
            set { itemDimFromIserial = value; }
        }
        public TransferViewModel() : base(PermissionItemName.TransferForm)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                OpenItemSearch = new RelayCommand(() => {
                    try
                    {
                        if (SelectedMainRow.TblWarehouseFrom != null &&
                        !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouseFrom.Code) &&
                        SelectedMainRow.TblWarehouseTo != null &&
                        !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouseTo.Code))
                        {
                            var vm = new ItemDimensionSearchViewModel();
                            vm.WarehouseCode = SelectedMainRow.TblWarehouseFrom.Code;
                            vm.WarehouseToCode = SelectedMainRow.TblWarehouseTo.Code;
                            vm.SiteIserial = SelectedMainRow.TblWarehouseFrom.TblSite;
                            vm.AppliedSearchResultList.CollectionChanged += (s, e) => {
                                // هنا هبدا اعبى الى جاى من السيرش
                                foreach (var item in vm.AppliedSearchResultList)
                                {
                                    var temp = SelectedMainRow.TblTransferDetails.FirstOrDefault(td => td.ItemDimFrom == item.ItemDimFromIserial &&
                                    td.ItemTransfer.ColorToId == item.ColorToId &&
                                    (td.ItemTransfer.SizeTo == item.SizeTo || (string.IsNullOrEmpty(td.ItemTransfer.SizeTo) && string.IsNullOrEmpty(item.SizeTo))) &&
                                    (td.ItemTransfer.BatchNoTo == item.BatchNoTo || (string.IsNullOrEmpty(td.ItemTransfer.BatchNoTo) && string.IsNullOrEmpty(item.BatchNoTo))));
                                    decimal allTransferedQuantity = SelectedMainRow.TblTransferDetails.Where(td =>
                                    td.ItemTransfer.ItemDimFromIserial == item.ItemDimFromIserial).Sum(td => td.ItemTransfer.TransferredQuantity);

                                    if (temp == null)// مش موجود
                                    {
                                        if ((item.AvailableQuantity < (allTransferedQuantity + item.TransferredQuantity) && item.PendingQuantity >= 0) ||
                                      ((item.AvailableQuantity + item.PendingQuantity) < (allTransferedQuantity + item.TransferredQuantity) && item.PendingQuantity >= 0))
                                        {
                                            MessageBox.Show(strings.CheckQuantities);
                                            return;
                                        }
                                        var transferDetail = new TransferDetail()
                                        {
                                            TransferHeader = SelectedMainRow.Iserial,
                                            ItemDimFrom = item.ItemDimFromIserial,
                                            ItemDimTo = item.ItemDimToIserial,
                                            Quantity = item.TransferredQuantity,
                                            ItemTransfer = item,
                                        };
                                        ValidateDetailRow(transferDetail);
                                    }
                                    else// لو موجود هحدث الكمية
                                    {
                                        if ((item.AvailableQuantity < (allTransferedQuantity - temp.ItemTransfer.AvailableQuantity + item.TransferredQuantity) && item.PendingQuantity >= 0) ||
                                        ((item.AvailableQuantity + item.PendingQuantity) < (allTransferedQuantity - temp.ItemTransfer.AvailableQuantity + item.TransferredQuantity) && item.PendingQuantity >= 0))
                                        {
                                            MessageBox.Show(strings.CheckQuantities);
                                            return;
                                        }
                                        temp.ItemTransfer.AvailableQuantity = item.AvailableQuantity;
                                        temp.ItemTransfer.PendingQuantity = item.PendingQuantity;
                                        temp.ItemTransfer.TransferredQuantity = item.TransferredQuantity;
                                        temp.Quantity = item.TransferredQuantity;
                                    }
                                }
                                RaisePropertyChanged(nameof(Total));
                            };
                            var childWindowSeach = new ItemDimensionSearchChildWindow(vm);
                            childWindowSeach.Show();
                            childWindowSeach.IsTransfer = true;
                            childWindowSeach.QuantityTitle = strings.Transferred;
                            vm.FromTitle = string.Format("From {0}", SelectedMainRow.TblWarehouseFrom.Ename);
                            vm.ToTitle = string.Format("To {0}", SelectedMainRow.TblWarehouseTo.Ename);
                            vm.Title = strings.TransferItem;
                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouse);
                    }
                    catch (Exception ex) { throw ex; }
                });
                OpenFPItemSearch = new RelayCommand(() => 
                {
                    try
                    {
                        if (SelectedMainRow.TblWarehouseFrom != null &&
                        !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouseFrom.Code) &&
                        SelectedMainRow.TblWarehouseTo != null &&
                        !string.IsNullOrWhiteSpace(SelectedMainRow.TblWarehouseTo.Code))
                        {
                            var vm = new ItemFPSearchViewModel();
                            vm.FPAppliedSearchResultList.CollectionChanged += (s, e) =>
                            {
                                if(vm.FPAppliedSearchResultList.Count >0 )
                                {
                                    var SearchItem = vm.FPAppliedSearchResultList.FirstOrDefault();
                                    if (SelectedMainRow.TblTransferDetails != null && SelectedMainRow.TblTransferDetails.Count > 0)
                                    {
                                        SelectedDetailRow.ItemFPName = SearchItem.ItemPerRow.Name;
                                        SelectedDetailRow.ItemFPCode = SearchItem.ItemPerRow.Code;
                                        SelectedDetailRow.ItemFPIserial = SearchItem.ItemPerRow.Iserial;

                                        foreach (var item in SelectedMainRow.TblTransferDetails)
                                        {
                                            if (item.ItemTransfer.ItemCode == SelectedDetailRow.ItemTransfer.ItemCode)
                                            {
                                                item.ItemFPName = SearchItem.ItemPerRow.Name;
                                                item.ItemFPCode = SearchItem.ItemPerRow.Code;
                                                item.ItemFPIserial = SearchItem.ItemPerRow.Iserial;
                                            }
                                        }
                                    }
                                }
                            };
                             var childWindowSeach = new ItemFPSearchChildWindow(vm);
                             childWindowSeach.Show();
                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouse);
                    } catch (Exception ex) { throw ex; }
                });

                ApproveTransfer = new RelayCommand(() => {
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    SelectedMainRow.Approved = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                        if (NewCommand.CanExecute(null))
                            NewCommand.Execute(null);
                }, () => CheckCanApprove());
                DeleteTransferDetail = new RelayCommand<object>((o) =>
                  {
                      if (((KeyEventArgs)(o)).Key == Key.Delete) {
                          if (SelectedMainRow.Iserial <= 0 || SelectedDetailRow.Iserial <= 0)
                          {
                              SelectedMainRow.TblTransferDetails.Remove(SelectedDetailRow);
                              if (SelectedMainRow.TblTransferDetails.Count == 0)
                              {
                                  AddNewDetailRow(false);
                              }
                          }
                          else
                              DeleteDetailRow();
                      }
                      RaisePropertyChanged(nameof(IsHeaderHasDetails));
                  },(o)=> {
                      return SelectedMainRow != null && !SelectedMainRow.Approved;
                  });
                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    //if (SelectedMainRow.TblTransferDetails.Count < PageSize)
                    //{
                    //    return;
                    //}
                    //if (SelectedMainRow.TblTransferDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    //{
                    //    GetDetailData();
                    //}
                });
                WarehouseClient.GetItemDimensionQuantitiesCompleted += (s, e) =>
                {
                    var temp = SelectedMainRow.TblTransferDetails.FirstOrDefault(ad =>
                      ad.ItemDimFrom == e.Result.ItemDimFromIserial);
                    var qtemp = e.Result.AvailableQuantity;
                    if (temp != null)
                    {
                        SelectedDetailRow = temp;
                    }
                    else//هعمل واحد جديد
                    {
                        temp = new TransferDetail()
                        {
                            TransferHeader = SelectedMainRow.Iserial,
                            ItemDimFrom = e.Result.ItemDimFromIserial,
                            ItemDimTo=e.Result.ItemDimToIserial,
                        };
                        temp.ItemTransfer.InjectFrom(e.Result);
                        temp.ItemTransfer.ColorPerRow.InjectFrom(e.Result.ColorPerRow);
                        temp.ItemTransfer.AvailableQuantity = qtemp;
                        temp.ItemTransfer.TransferredQuantity = 0;
                        temp.Quantity = 0;
                        ValidateDetailRow(temp);
                    }
                };
                GetDetailItem = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Enter)
                    {
                        if (SelectedMainRow.TblWarehouseFrom == null || SelectedMainRow.TblWarehouseTo == null)
                        {
                            MessageBox.Show(strings.PleaseSelectWarehouse);
                            return;
                        }
                        WarehouseClient.GetItemDimensionQuantitiesAsync(
                            SelectedMainRow.TblWarehouseFrom.Code,
                            SelectedMainRow.TblWarehouseTo.Code,
                            ItemDimFromIserial,
                            SelectedMainRow.DocDate);
                    }
                });
                //SearchComboFrom = new RelayCommand<object>((o) =>
                //{
                //    if (((KeyEventArgs)(o)).Key == Key.F2)
                //    {if (SelectedMainRow.TblWarehouseFrom == null) SelectedMainRow.TblWarehouseFrom = new WarehouseService.TblWarehouse();
                //    new GenericSearchViewModel<TblWarehouse>().SearchLookup(WarehouseListFrom, SelectedMainRow.TblWarehouseFrom
                //            , new SilverlightCommands.RelayCommand((p) => { int x = 0;x++; }), "search warehouse", new Models.LookupItemModel(),
                //            new SilverlightCommands.RelayCommand((p) => { int y = 0;y++; }));
                //    }
                //});
                ReturnToBarcode = new RelayCommand<object>((o) =>
                {
                    //if (((KeyEventArgs)(o)).Key == Key.Enter)
                    //{
                    //    foreach (var item in SelectedMainRow.TblTransferDetails)
                    //    {
                    //        item.IsQuantityFocused = false;
                    //    }
                    //    IsRefFocused = true;
                    //}
                    RaisePropertyChanged(nameof(Total));
                });
                DetailSelectionChanged = new RelayCommand<object>((o) =>
                {
                    var e = o as SelectionChangedEventArgs;
                    // هنا هنقل الفوكس للكمية شوف بقى ازاى
                    IsRefFocused = false;
                    foreach (var item in SelectedMainRow.TblTransferDetails)
                    {
                        item.IsQuantityFocused = false;
                    }
                    SelectedDetailRow.IsQuantityFocused = true;
                });
                this.PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "TransferApprove") != null)
                    {
                        CanApprove = true;
                    }
                };
                this.GetCustomePermissions(PermissionItemName.TransferForm.ToString());

                MainRowList = new ObservableCollection<TransferHeader>();
                AddNewMainRow(false);

                WarehouseClient.GetUserAsignedWarehousesForTransferCompleted += (s, e) =>
                {
                    UserWarehouseList.Clear();
                    foreach (var item in e.Result)
                    {
                        UserWarehouseList.Add(item);
                    }
                };
                WarehouseClient.GetUserAsignedWarehousesForTransferAsync(LoggedUserInfo.Iserial);
                
                WarehouseClient.GetTransferCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TransferHeader();
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
                };
                WarehouseClient.GetTransferDetailCompleted += (s, sv) =>
                {
                    SelectedMainRow.TblTransferDetails.Clear();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TransferDetail();
                        newrow.InjectFrom(row);
                        newrow.ItemTransfer.InjectFrom(row.ItemTransfer);
                        newrow.ItemTransfer.ColorPerRow.InjectFrom(row.ItemTransfer.ColorPerRow);
                        SelectedMainRow.TblTransferDetails.Add(newrow);
                    }
                    if (!SelectedMainRow.TblTransferDetails.Any())
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                WarehouseClient.UpdateOrInsertTransferHeaderCompleted += (s, x) =>
                {
                    TransferHeader savedRow = null;
                    if (x.outindex >= 0)
                        savedRow = MainRowList.ElementAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblWarehouseFrom = WarehouseListFrom.FirstOrDefault(w => w.Iserial == savedRow.WarehouseFrom);
                        savedRow.TblWarehouseTo = WarehouseListTo.FirstOrDefault(w => w.Iserial == savedRow.WarehouseTo);
                        savedRow.TblTransferDetails.Clear();
                        foreach (var item in x.Result.TblTransferDetails)
                        {
                            var detailTemp = new TransferDetail();
                            detailTemp.InjectFrom(item);
                            detailTemp.ItemTransfer.InjectFrom(item.ItemTransfer);
                            detailTemp.ItemTransfer.ColorPerRow.InjectFrom(item.ItemTransfer.ColorPerRow);
                            savedRow.TblTransferDetails.Add(detailTemp);
                        }
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    DeleteCommand.RaiseCanExecuteChanged();
                    ApproveTransfer.RaiseCanExecuteChanged();
                    DeleteTransferDetail.RaiseCanExecuteChanged();
                   // IsNewChanged();
                };
                WarehouseClient.UpdateOrInsertTransferDetailCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.TblTransferDetails.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                WarehouseClient.DeleteTransferCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                WarehouseClient.DeleteTransferDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblTransferDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblTransferDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };

                WarehouseClient.GetLookUpWarehouseForTransferFromCompleted += (s, e) => {
                    foreach (var row in e.Result)
                    {
                        var newrow = new TblWarehouse();
                        newrow.InjectFrom(row);
                        WarehouseListFrom.Add(newrow);
                    }
                    Loading = false;
                };
                WarehouseClient.GetLookUpWarehouseForTransferToCompleted += (s, e) => {
                    foreach (var row in e.Result)
                    {
                        var newrow = new TblWarehouse();
                        newrow.InjectFrom(row);
                        WarehouseListTo.Add(newrow);
                    }
                    Loading = false;
                };

                GetComboData();
                GetMaindata();
            }
        }
        private void ValidateDetailRow(TransferDetail transferDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblTransferDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblTransferDetails.Add(transferDetail);
            }
            else
            {
                SelectedMainRow.TblTransferDetails.Add(transferDetail);
            }
            SelectedDetailRow = transferDetail;
            RaisePropertyChanged(nameof(Total));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }
        private bool CheckCanApprove()
        {
            return CanApprove && !SelectedMainRow.Approved && UserWarehouseList.Any(uw =>
            uw.PermissionType == (short)AuthWarehouseType.TransferTo || uw.PermissionType == (short)AuthWarehouseType.TransferToFrom
            && uw.WarehouseIserial == SelectedMainRow.WarehouseTo);
        }

        #region Operations

        public void GetMaindata()
        {
            //if (SortBy == null)
                SortBy = "it.Iserial desc";
            WarehouseClient.GetTransferAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,LoggedUserInfo.Iserial);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WarehouseClient.DeleteTransferAsync((TblTransferHeader)new
                        TblTransferHeader().InjectFrom(SelectedMainRow),
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
                SelectedMainRow = new TransferHeader()
                {
                    TblTransferDetails = new ObservableCollection<TransferDetail>()
                };
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                AddNewDetailRow(false);
                RaisePropertyChanged(nameof(Total));
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
                 uw.WarehouseIserial == SelectedMainRow.WarehouseFrom ||
                 uw.WarehouseIserial == SelectedMainRow.WarehouseTo);
                if (isvalid)
                {
                    var saveRow = new TblTransferHeader()
                    {
                        DocDate = DateTime.Now,
                        CreatedBy = 1,
                        CreationDate = DateTime.Now,
                        Approved = false,
                        LastChangeDate = DateTime.Now,
                        CodeFrom = "",
                        CodeTo = "",
                    };
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblTransferDetails = new ObservableCollection<TblTransferDetail>();
                    foreach (var item in SelectedMainRow.TblTransferDetails)
                    {
                        var detailTemp = new TblTransferDetail();
                        detailTemp.InjectFrom(item);
                        detailTemp.ItemTransfer = new Web.DataLayer.ItemDimensionSearchModel();
                        saveRow.TblTransferDetails.Add(detailTemp);
                    }

                    var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    if (mainRowIndex < 0)
                    {
                        MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    }
                    WarehouseClient.UpdateOrInsertTransferHeaderAsync(saveRow, mainRowIndex, LoggedUserInfo.Iserial);
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
                WarehouseClient.GetTransferDetailAsync(0, int.MaxValue, SelectedMainRow.Iserial);//SelectedMainRow.TblTransferDetails.Count, PageSize
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    //foreach (var row in SelectedDetailRows)
                    //{
                    //    WarehouseClient.DeleteTransferDetailAsync((TblTransferDetail)new TblTransferDetail().InjectFrom(row), SelectedMainRow.TblTransferDetails.IndexOf(row));
                    //}
                    WarehouseClient.DeleteTransferDetailAsync((TblTransferDetail)new TblTransferDetail().InjectFrom(SelectedDetailRow),
                        SelectedMainRow.TblTransferDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblTransferDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblTransferDetails.Count - 1))
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
                SelectedMainRow.TblTransferDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new TransferDetail
                {
                    TransferHeader = SelectedMainRow.Iserial
                });
                RaisePropertyChanged(nameof(Total));
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
                    var rowToSave = new TblTransferDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    WarehouseClient.UpdateOrInsertTransferDetailAsync(rowToSave, SelectedMainRow.TblTransferDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            WarehouseClient.GetLookUpWarehouseForTransferFromAsync(LoggedUserInfo.Iserial);
            WarehouseClient.GetLookUpWarehouseForTransferToAsync(LoggedUserInfo.Iserial);
        }
      
        #endregion

        #region Properties

        private ObservableCollection<TransferHeader> _mainRowList;
        public ObservableCollection<TransferHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }
        private ObservableCollection<TransferHeader> _selectedMainRows;
        public ObservableCollection<TransferHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TransferHeader>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private TransferHeader _selectedMainRow;
        public TransferHeader SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new TransferHeader()
                {
                    TblTransferDetails = new ObservableCollection<TransferDetail>()
                });
            }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                ApproveTransfer.RaiseCanExecuteChanged();
                DeleteTransferDetail.RaiseCanExecuteChanged();
             //   IsNewChanged();
                GetDetailData();
            }
        }
        private TransferDetail _selectedDetailRow;
        public TransferDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new TransferDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }
        private ObservableCollection<TransferDetail> _selectedDetailRows;
        public ObservableCollection<TransferDetail> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TransferDetail>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged(nameof(SelectedDetailRows)); }
        }

        private ItemFPSearchViewModel _selectedItemFPRow;
        public ItemFPSearchViewModel SelectedItemFPRow
        {
            get { return _selectedItemFPRow ?? (_selectedItemFPRow = new ItemFPSearchViewModel()); }
            set { _selectedItemFPRow = value; RaisePropertyChanged(nameof(SelectedItemFPRow)); }
        }

        #region Combo Data

        ObservableCollection<TblWarehouse> _warehouseListFrom = new ObservableCollection<TblWarehouse>();
        public ObservableCollection<TblWarehouse> WarehouseListFrom
        {
            get { return _warehouseListFrom; }
            set { _warehouseListFrom = value; RaisePropertyChanged(nameof(WarehouseListFrom)); }
        }
        ObservableCollection<TblWarehouse> _warehouseListTo = new ObservableCollection<TblWarehouse>();
        public ObservableCollection<TblWarehouse> WarehouseListTo
        {
            get { return _warehouseListTo; }
            set { _warehouseListTo = value; RaisePropertyChanged(nameof(WarehouseListTo)); }
        }

        #endregion

        public virtual bool IsReadOnly
        {
            get { return SelectedMainRow != null && SelectedMainRow.Iserial > 0 && SelectedMainRow.Approved; }
        }
        public virtual bool IsHeaderHasDetails
        {
            get { return SelectedMainRow.TblTransferDetails.Any(d => d.ItemDimFrom > 0) || IsReadOnly; }
        }
        private bool canApprove;
        public bool CanApprove
        {
            get { return canApprove; }
            set { canApprove = value; RaisePropertyChanged(nameof(CanApprove)); ApproveTransfer.RaiseCanExecuteChanged(); }
        }

        ObservableCollection<TblAuthWarehouse> _userWarehouseList = new ObservableCollection<TblAuthWarehouse>();
        public ObservableCollection<TblAuthWarehouse> UserWarehouseList
        {
            get { return _userWarehouseList ?? (_userWarehouseList = new ObservableCollection<TblAuthWarehouse>()); }
            set { _userWarehouseList = value; RaisePropertyChanged(nameof(UserWarehouseList)); }
        }
        public override bool IsNew
        {
            get { return SelectedMainRow.Iserial > 0; }//base.IsNew && 
            set { base.IsNew = value; }
        }
     
        public decimal Total
        {
            get { return SelectedMainRow.TblTransferDetails.Sum(td => td.Quantity); }
        }
        #endregion

        #region Commands

        RelayCommand openItemSearch;
        public RelayCommand OpenItemSearch
        {
            get { return openItemSearch; }
            set { openItemSearch = value; RaisePropertyChanged(nameof(OpenItemSearch)); }
        }

        RelayCommand openFPItemSearch;
        public RelayCommand OpenFPItemSearch
        {
            get { return openFPItemSearch; }
            set { openFPItemSearch = value; RaisePropertyChanged(nameof(OpenFPItemSearch)); }
        }

        RelayCommand approveTransfer;
        public RelayCommand ApproveTransfer
        {
            get { return approveTransfer; }
            set { approveTransfer = value; RaisePropertyChanged(nameof(ApproveTransfer)); }
        }

        RelayCommand<object> deleteTransferDetail;
        public RelayCommand<object> DeleteTransferDetail
        {
            get { return deleteTransferDetail; }
            set { deleteTransferDetail = value; RaisePropertyChanged(nameof(DeleteTransferDetail)); }
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

        RelayCommand<object> searchComboFrom;
        public RelayCommand<object> SearchComboFrom
        {
            get { return searchComboFrom; }
            set { searchComboFrom = value; RaisePropertyChanged(nameof(SearchComboFrom)); }
        }
        RelayCommand<object> loadingDetailRows;
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }
        bool isRefFocused;
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
            if ((SelectedMainRow.Approved && SelectedMainRow.Iserial <= 0 && SelectedMainRow.TblTransferDetails.Any(td =>
                    (td.Quantity > td.ItemTransfer.AvailableQuantity && td.ItemTransfer.PendingQuantity >= 0) ||
                    (td.Quantity > (td.ItemTransfer.AvailableQuantity + td.ItemTransfer.PendingQuantity) &&
                    td.ItemTransfer.PendingQuantity < 0))) ||
                (SelectedMainRow.Approved && SelectedMainRow.Iserial > 0 && SelectedMainRow.TblTransferDetails.Any(td =>
                    (td.Quantity > td.ItemTransfer.AvailableQuantity && td.ItemTransfer.PendingQuantity >= 0) ||
                    (td.Quantity > (td.ItemTransfer.AvailableQuantity + td.ItemTransfer.PendingQuantity + td.Quantity) &&
                    td.ItemTransfer.PendingQuantity < 0))))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TransferHeader> vm =
                new GenericSearchViewModel<TransferHeader>() { Title = "Transfer Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) => {
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
                        Header="Code From",
                        PropertyPath=nameof(TransferHeader.CodeFrom),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WarehouseFrom,
                        PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header="Code To",
                        PropertyPath=nameof(TransferHeader.CodeTo),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.WarehouseTo,
                        PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Date,
                        PropertyPath=nameof(TransferHeader.DocDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Approved,
                        PropertyPath=nameof(TransferHeader.Approved),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ApproveDate,
                        PropertyPath=nameof(TransferHeader.ApproveDate),
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
            
            rVM.GenerateReport("TransferDocument", new ObservableCollection<string>() {SelectedMainRow.Iserial.ToString() });
        }
        #endregion
    }
}