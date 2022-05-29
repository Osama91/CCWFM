using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using GenericTable = CCWFM.GlService.GenericTable;
using TblColorTest = CCWFM.GlService.TblColorTest;
using TBLITEMprice = CCWFM.GlService.TBLITEMprice;
using TblMarkup = CCWFM.GlService.TblMarkup;
using TblMarkupTran = CCWFM.GlService.TblMarkupTran;
using TblRecInvDetail = CCWFM.GlService.TblRecInvDetail;
using TblRecInvHeader = CCWFM.GlService.TblRecInvHeader;
using TblRecInvMainDetail = CCWFM.GlService.TblRecInvMainDetail;
using TBLsupplier = CCWFM.CRUDManagerService.TBLsupplier;

namespace CCWFM.ViewModel.Gl
{

    public class RecInvViewModel : ViewModelBase
    {
        public bool CanPost { get; set; }
        public RecInvViewModel()
        {
            if (!IsDesignTime)
            {
                MiscValueTypeList = new ObservableCollection<GenericTable>
                {
                    new GenericTable {Iserial = 0, Code = "%", Ename = "%", Aname = "%"},
                    new GenericTable {Iserial = 1, Code = "Value", Ename = "Value", Aname = "Value"}
                };
                GetItemPermissions(PermissionItemName.RecInv.ToString());
                GetCustomePermissions(PermissionItemName.RecInv.ToString());


                this.PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "RecInvPosting") != null)
                    {
                        CanPost = true;
                    }
                };
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblRecInvHeaderViewModel>();
                SelectedMainRow = new TblRecInvHeaderViewModel();

                var currencyClient = new GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = sv.Result;
                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetTblMarkupAsync(0, int.MaxValue, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);

                Glclient.GetTblMarkupCompleted += (s, sv) =>
                {
                    MarkupList = sv.Result;
                };

                var tblRecInvHeaderTypeClient = new GlServiceClient();
                tblRecInvHeaderTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    TblRecInvHeaderTypeList = sv.Result;
                };
                tblRecInvHeaderTypeClient.GetGenericAsync("TblRecInvHeaderType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Glclient.GetTblRecInvHeaderCompleted += (s, sv) =>
             {
                 foreach (var row in sv.Result)
                 {
                     var newrow = new TblRecInvHeaderViewModel();
                     newrow.InjectFrom(row);
                     if (newrow.Status == 0)
                     {
                         if(CanPost)
                         newrow.VisPosted = true;
                     }
                     newrow.SupplierPerRow = new TBLsupplier();
                     row.TBLsupplier1.TblPO1Header = null;
                        
                     row.TBLsupplier1.TblRecInvHeaders = null;
                     row.TBLsupplier1.EntityKey = null;
                    
                     //row.TBLsupplier1.TblMarkupTrans = null;
                     newrow.SupplierPerRow.InjectFrom(row.TBLsupplier1);
                     newrow.TblRecInvHeaderTypePerRow = new GenericTable();
                     if (row.TblRecInvHeaderType1 != null)
                     {
                         newrow.TblRecInvHeaderTypePerRow = new GenericTable().InjectFrom(row.TblRecInvHeaderType1) as GenericTable;
                     }
                     newrow.StorePerRow = new CRUDManagerService.TblStore(); // row.TblStore1;
                     if (row.TblStore1 != null)
                     {
                            
                         row.TblStore1.EntityKey = null;
                            
                         row.TblStore1.TblBankDeposits = null;
                            
                         row.TblStore1.TBLSTORETRANS = null;
                         row.TblStore1.TblBrandStoreTargetDetails = null;
                         row.TblStore1.TblPO1Header = null;
                         row.TblStore1.TblPOHeaders = null;
                         row.TblStore1.TblPromoHeaders = null;
                         row.TblStore1.TblRecInvHeaders = null;
                         //	 if (row.TblStore1 != null) newrow.StorePerRow.InjectFrom(row.TblStore1);
                     }

                     if (row.TblAccount != null)
                     {
                         newrow.AccountPerRow = new TblAccount
                         {
                             Code = row.TblAccount1.Code,
                             Iserial = row.TblAccount1.Iserial,
                             Ename = row.TblAccount1.Ename,
                             Aname = row.TblAccount1.Aname
                         };
                     }
                     MainRowList.Add(newrow);
                 }
                 Loading = false;
                 FullCount = sv.fullCount;
                 if (MainRowList.Any() && (SelectedMainRow == null))
                 {
                     SelectedMainRow = MainRowList.FirstOrDefault();
                 }
             };

                Glclient.UpdateOrInsertTblRecInvHeadersCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblRecInvHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                Glclient.GetRecInvStyleCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        SelectedMainRow.StyleDetailList.Add(new TblRecInvMainDetailViewModel
                        {
                            TBLITEMprice = new TBLITEMprice
                            {
                                Style = variable.Style
                            },

                            Qty = variable.Quantity??0,
                            Cost = variable.Cost??0
                        });
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;
                };

                Glclient.GetRecInvStyleColorCompleted += (s, sv) =>
                {
                    try
                    {
                        foreach (var variable in sv.Result)
                        {
                            SelectedMainRow.StyleColorDetailsList.Add(new TblRecInvMainDetailViewModel
                            {
                                TBLITEMprice = new TBLITEMprice
                                {
                                    Style = variable.Style,
                                    TblColor1 = new TblColorTest { Aname = variable.ColorName, Code = variable.ColorCode, Ename = variable.ColorName }
                                },
                                Qty = variable.Quantity ?? 0,
                                Cost = variable.Cost ?? 0,
                                ContractCost = variable.ContractCost,

                                ContractQty = variable.ContractQty,
                                ContractTotal = variable.ContractQty * variable.ContractCost,
                            });
                        }
                    }
                    catch (Exception)
                    {

                        
                    }
                
                    Loading = false;
                };

                Glclient.GetTblRecInvMainDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRecInvMainDetailViewModel { CurrencyPerRow = new GenericTable() };
                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        if (LoggedUserInfo.DatabasEname=="MAN"|| LoggedUserInfo.DatabasEname== "Sw"   || LoggedUserInfo.DatabasEname == "CA")
                        {
                            row.TBLITEMprice = new TBLITEMprice();
                            row.TBLITEMprice.Style = sv.Items.FirstOrDefault(w => w.iserial == row.TblItem).Style;
                            row.TBLITEMprice.TblColor1 = new TblColorTest();
                            row.TBLITEMprice.TblColor1.Code = sv.Items.FirstOrDefault(w => w.iserial == row.TblItem).ColorCode;
                            row.TBLITEMprice.TblColor1.Ename = sv.Items.FirstOrDefault(w => w.iserial == row.TblItem).ColorName;
                            row.TBLITEMprice.TblSize1 = new TblSizeRetail();
                            row.TBLITEMprice.TblSize1.Code = sv.Items.FirstOrDefault(w => w.iserial == row.TblItem).SizeCode;
                            row.TBLITEMprice.TblSize1.Ename = sv.Items.FirstOrDefault(w => w.iserial == row.TblItem).SizeCode;
                        }
               

                        newrow.InjectFrom(row);

                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    TotalCost = sv.TotalAmount;
                    TotalQty = sv.TotalQty;
                    Loading = false;
                    DetailFullCount = sv.fullCount;

                    if (SelectedMainRow.DetailsList.Any() &&
                     (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                };

                Glclient.PostInvCompleted += (s, sv) =>
                {
                    if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);
                    SelectedMainRow.VisPosted = false;
                    MessageBox.Show("Posted Completed");
                };
                Glclient.GetTblRecieveDetailCompleted += (s, sv) =>
                {
                    if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);

                    if (CanPost)
                    SelectedMainRow.VisPosted = true;
                    GetDetailData();
                    GetRecInvStyle();
                    GetRecInvStyleColor();
                };
                Glclient.GetTblRecieveHeaderFromToCompleted += (s, sv) =>
                {
                    DetailSubFilter = null;
                    DetailSubValuesObjects = new Dictionary<string, object>();
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            if (!RecieveHeaderChoosedList.Select(x => x.Code).Contains(row.Code))
                            {
                                RecieveHeaderChoosedList.Add(new TblReciveHeaderViewModel().InjectFrom(row) as TblReciveHeaderViewModel);
                            }
                        }
                    }
                };
                Glclient.GetTblReturnHeaderFromToCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            if (!RecieveHeaderChoosedList.Select(x => x.Code).Contains(row.Code))
                            {
                                RecieveHeaderChoosedList.Add(
                                    new TblReciveHeaderViewModel().InjectFrom(row) as TblReciveHeaderViewModel);
                            }
                        }
                    }
                };

                var journalAccountTypeClient = new GlService.GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);


                //Glclient.GetTblReturnDetailCompleted += (s, sv) =>
                //{
                //    if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);

                //    if (CanPost)
                //        SelectedMainRow.VisPosted = true;
                //    GetDetailData();
                //    GetRecInvStyle();
                //    GetRecInvStyleColor();
                //};
                Glclient.GetTblRecieveHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblReciveHeaderViewModel();

                        newrow.InjectFrom(row);

                        RecieveHeaderList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;
                };
                Glclient.GetTblReturnHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblReciveHeaderViewModel();

                        newrow.InjectFrom(row);

                        RecieveHeaderList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;
                };
                Glclient.UpdateOrInsertTblRecInvMainDetailCompleted += (s, x) =>
                {
                    TotalCost = x.TotalAmount;
                    TotalQty = x.TotalQty;
                    Loading = false;
                };
                Glclient.UpdateOrInsertTblMarkupTransCompleted += (s, x) =>
                {
                    var markup = new TblMarkup();
                    try
                    {
                        var row = SelectedMainRow.MarkUpTransList.ElementAt(x.outindex);
                        if (row != null)
                        {
                            markup = row.TblMarkup1;
                        }
                        if (x.Result.Type == 0)
                        {
                            SelectedMainRow.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);
                        }
                        else
                        {
                            SelectedDetailRow.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);
                        }
                        row.TblMarkup1 = markup;
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }

                    Loading = false;
                };

                Glclient.InvoiceRecInvHeaderCompleted += (s, x) =>
                {
                    var msg = "";

                    if (x.Result.Any())
                    {
                        foreach (var item in x.Result)
                        {
                            
                            msg = msg+ "Style :" + item.Style + " Color :" + item.ColorCode + " Total Invoiced Qty :" + item.Quantity + " ContractQty :" + item.ContractQty + " Difference :" + (item.Quantity-item.ContractQty) + " Contract No :" + item.ContractCode  + "\r\n";
                        }
                        MessageBox.Show(msg);

                    }
                    else {

                        this.SelectedMainRow.Invoiced = true;
                    }

                };
            }
        }

        internal void Invoice()
        {
        
            Glclient.InvoiceRecInvHeaderAsync(SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);

            //GlService.invoice();
        }

        private ObservableCollection<TblMarkup> _markupList;

        public ObservableCollection<TblMarkup> MarkupList
        {
            get { return _markupList; }
            set { _markupList = value; RaisePropertyChanged("MarkupList"); }
        }

        private decimal _totalQty;

        public decimal TotalQty
        {
            get { return _totalQty; }
            set { _totalQty = value; RaisePropertyChanged("TotalQty"); }
        }

        private decimal _totalCost;

        public decimal TotalCost
        {
            get { return _totalCost; }
            set { _totalCost = value; RaisePropertyChanged("TotalCost"); }
        }

        public void GetRecInvStyle()
        {
            Loading = true;
            Glclient.GetRecInvStyleAsync(SelectedMainRow.StyleDetailList.Count, PageSize, SelectedMainRow.Iserial, "it.style", DetailSubFilter, DetailSubValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void GetRecInvStyleColor()
        {
            Loading = true;
            Glclient.GetRecInvStyleColorAsync(SelectedMainRow.StyleColorDetailsList.Count, PageSize, SelectedMainRow.Iserial, "it.style", null, null,
                LoggedUserInfo.DatabasEname);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial Desc";
            Loading = true;
            Glclient.GetTblRecInvHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblRecInvHeaderViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void DeleteMainRow()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                Glclient.DeleteTblRecInvHeaderAsync((TblRecInvHeader)new TblRecInvHeader().InjectFrom(SelectedMainRow),
                    MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
            }
        }
        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblRecInvHeader();
                    saveRow.InjectFrom(SelectedMainRow);

                    saveRow.TblRecInvDetails = new ObservableCollection<TblRecInvDetail>();
                    GenericMapper.InjectFromObCollection(saveRow.TblRecInvMainDetails, SelectedMainRow.DetailsList);
                    //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                    if (!Loading)
                    {
                        Loading = true;

                        Glclient.UpdateOrInsertTblRecInvHeadersAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }


        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblRecInvMainDetail();
                    saveRow.InjectFrom(SelectedDetailRow);
                    saveRow.TblCurrency1 = null;
                    saveRow.TBLITEMprice = null;
                    saveRow.TblRecInvHeader1 = null;
                    if (!Loading)
                    {
                        Loading = true;
                        int xxxx = SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow);
                        Glclient.UpdateOrInsertTblRecInvMainDetailAsync(saveRow, save, xxxx, LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void AddNewMarkUpRow(bool checkLastRow, bool header, double misc)
        {
            if (header)
            {
                var currentRowIndex = (SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow));

                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                        new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }
                if (AllowAdd != true)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }
                var newrow = new TblMarkupTransViewModel
                {
                    Type = 0,
                    TblRecInv = SelectedMainRow.Iserial,
                    MiscValueType = 1,
                    MiscValue = misc,                    
                    ExchangeRate = 1

                };

                if (misc != 0)
                {
                    newrow.MiscValue = misc;
                    newrow.Disabled = true;
                }


                //if (SelectedMainRow.SupplierPerRow != null)
                //{
                //    newrow.ent = SelectedMainRow.SupplierPerRow;
                //}
                SelectedMainRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
                SelectedMarkupRow = newrow;
            }
            else
            {
                var currentRowIndex = (SelectedDetailRow.MarkUpTransList.IndexOf(SelectedMarkupRow));

                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                        new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }
                if (AllowAdd != true)
                {
                    MessageBox.Show(strings.AllowAddMsg);
                    return;
                }
                var newrow = new TblMarkupTransViewModel { Type = 0, TblRecInv = SelectedDetailRow.Iserial,ExchangeRate=1 };
                SelectedDetailRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
                SelectedMarkupRow = newrow;
            }
        }

        public void SaveMarkupRow()
        {
            if (SelectedMarkupRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
                    new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMarkupRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblMarkupTran();
                    saveRow.InjectFrom(SelectedMarkupRow);

                    if (SelectedMainRow.Iserial==0)
                    {
                        MessageBox.Show("Transaction Not Saved PLease Check");
                        return;
                    }

                    saveRow.TblRecInv = SelectedMainRow.Iserial;
                    saveRow.TblCurrency1 = null;
                    //saveRow.TblMarkup1 = null;

                    //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                    if (!Loading)
                    {
                        Loading = true;

                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (SelectedMarkupRow.Type == 0)
                        {
                            Glclient.UpdateOrInsertTblMarkupTransAsync(saveRow, save, SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
                      LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            Glclient.UpdateOrInsertTblMarkupTransAsync(saveRow, save, SelectedDetailRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
                LoggedUserInfo.DatabasEname);
                        }
                    }
                }
            }
        }

        public void SaveMarkupRowOldRow(TblMarkupTransViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblMarkupTran();
                    saveRow.InjectFrom(oldRow);
                    saveRow.TblRecInv = SelectedMainRow.Iserial;
                    if (SelectedMainRow.Iserial == 0)
                    {
                        MessageBox.Show("Transaction Not Saved PLease Check");
                        return;
                    }

                    saveRow.TblCurrency1 = null;
                    //saveRow.TblMarkup1 = null;
                    if (!Loading)
                    {
                        Loading = true;
                        //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (oldRow.Type == 0)
                        {
                            Glclient.UpdateOrInsertTblMarkupTransAsync(saveRow, save,
                                SelectedMainRow.MarkUpTransList.IndexOf(oldRow),
                                LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            Glclient.UpdateOrInsertTblMarkupTransAsync(saveRow, save,
                                SelectedDetailRow.MarkUpTransList.IndexOf(oldRow),
                                LoggedUserInfo.DatabasEname);
                        }
                    }
                }
            }
        }


        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Iserial";
            Loading = true;
            if (SelectedMainRow != null)
                Glclient.GetTblRecInvMainDetailAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetMarkUpdata(bool header)
        {
            var client = new GlServiceClient();
            if (header)
            {
                SelectedMainRow.MarkUpTransList.Clear();

                client.GetTblMarkupTransAsync(0, SelectedMainRow.Iserial,
              LoggedUserInfo.DatabasEname);

                client.GetTblMarkupTransCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblMarkupTransViewModel();
                        //newrow.SupplierPerRow = new TBLsupplier();                 
                        //row.TBLsupplier1.TblMarkupTrans = null;
                        //row.TBLsupplier1.TblPO1Header = null;
                        //row.TBLsupplier1.TblRecInvHeaders = null;
                        //row.TBLsupplier1.EntityKey = null;
                        //if (row.TBLsupplier1 != null) newrow.SupplierPerRow.InjectFrom(row.TBLsupplier1);
                        newrow.InjectFrom(row);
                        newrow.CurrencyPerRow = new GenericTable();

                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        newrow.TblMarkup1 = row.TblMarkup1;

                        newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == newrow.TblJournalAccountType);
                        var entity = sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType);
                        newrow.EntityPerRow = new GlService.Entity();

                        if (entity != null)
                        {

                            newrow.EntityPerRow = new GlService.Entity().InjectFrom(entity) as GlService.Entity;
                        }
                        newrow.TblJournalAccountType = row.TblJournalAccountType;
                        newrow.EntityAccount = row.EntityAccount??0;

                        SelectedMainRow.MarkUpTransList.Add(newrow);
                    }

                    Loading = false;

                    if (SelectedMainRow.MarkUpTransList.Count == 0)
                    {
                        var misc = (double?)sv.TotalMisc;


                        AddNewMarkUpRow(false, true, misc??0);
                    }
                };
            }
            else
            {
                client.GetTblMarkupTransAsync(1, SelectedDetailRow.Iserial,
              LoggedUserInfo.DatabasEname);
                client.GetTblMarkupTransCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblMarkupTransViewModel();
                        //newrow.SupplierPerRow = new TBLsupplier();

                        //if (row.TBLsupplier1 != null) newrow.SupplierPerRow.InjectFrom(row.TBLsupplier1);
                        newrow.InjectFrom(row);
                        newrow.CurrencyPerRow = new GenericTable();

                        newrow.CurrencyPerRow.InjectFrom(row.TblCurrency1);
                        newrow.TblMarkup1 = row.TblMarkup1;
                        newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == newrow.TblJournalAccountType);
                        var entity = sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType);
                        newrow.EntityPerRow = new GlService.Entity();

                        if (entity != null)
                        {

                            newrow.EntityPerRow = new GlService.Entity().InjectFrom(entity) as GlService.Entity;
                        }
                        newrow.TblJournalAccountType = row.TblJournalAccountType;
                        newrow.EntityAccount = row.EntityAccount??0;
                        SelectedDetailRow.MarkUpTransList.Add(newrow);
                    }

                    Loading = false;

                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        //AddNewMarkUpRow(false, true);
                    }
                };
            }

            Loading = true;
        }

        #region Prop

        private ObservableCollection<GenericTable> _miscValueType;

        public ObservableCollection<GenericTable> MiscValueTypeList
        {
            get { return _miscValueType; }
            set { _miscValueType = value; RaisePropertyChanged("MiscValueTypeList"); }
        }

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _tblRecInvHeaderTypeList;

        public ObservableCollection<GenericTable> TblRecInvHeaderTypeList
        {
            get { return _tblRecInvHeaderTypeList; }
            set { _tblRecInvHeaderTypeList = value; RaisePropertyChanged("TblRecInvHeaderTypeList"); }
        }

        private SortableCollectionView<TblRecInvHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblRecInvHeaderViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblRecInvHeaderViewModel>()); }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblRecInvHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblRecInvHeaderViewModel> SelectedMainRows
        {
            get
            {
                return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblRecInvHeaderViewModel>());
            }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private DateTime? _toDateTime;

        public DateTime? ToDate
        {
            get { return _toDateTime ?? (_toDateTime = DateTime.Now); }
            set { _toDateTime = value; RaisePropertyChanged("ToDate"); }
        }

        private DateTime? _fromDateTime;

        public DateTime? FromDate
        {
            get { return _fromDateTime ?? (_fromDateTime = DateTime.Now); }
            set { _fromDateTime = value; RaisePropertyChanged("FromDate"); }
        }

        private TblRecInvHeaderViewModel _selectedMainRow;

        public TblRecInvHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }
        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private TblMarkupTransViewModel _selectedMarkup;

        public TblMarkupTransViewModel SelectedMarkupRow
        {
            get { return _selectedMarkup; }
            set
            {
                _selectedMarkup = value;
                RaisePropertyChanged("SelectedMarkupRow");
            }
        }

        private TblRecInvMainDetailViewModel _selectedDetailRow;

        public TblRecInvMainDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblRecInvMainDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblRecInvMainDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows; }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        private ObservableCollection<TblReciveHeaderViewModel> _recieveHeaderList;

        public ObservableCollection<TblReciveHeaderViewModel> RecieveHeaderList
        {
            get { return _recieveHeaderList ?? (_recieveHeaderList = new ObservableCollection<TblReciveHeaderViewModel>()); }
            set
            {
                _recieveHeaderList = value;
                RaisePropertyChanged("RecieveHeaderList");
            }
        }

        private ObservableCollection<TblReciveHeaderViewModel> _recieveHeaderChoosedList;

        public ObservableCollection<TblReciveHeaderViewModel> RecieveHeaderChoosedList
        {
            get { return _recieveHeaderChoosedList ?? (_recieveHeaderChoosedList = new ObservableCollection<TblReciveHeaderViewModel>()); }
            set
            {
                _recieveHeaderChoosedList = value;
                RaisePropertyChanged("RecieveHeaderChoosedList");
            }
        }

        #endregion Prop

        internal void SearchHeader()
        {
        }

        public void GetRecieveHeaderListData()
        {
            //if (SelectedMainRow.TblRecInvHeaderType == 1)
            //{
                Glclient.GetTblRecieveHeaderAsync(RecieveHeaderList.Count, PageSize, (int)SelectedMainRow.TblSupplier, "it.glserial desc", DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
            //}
            //else
            //{
            //    Glclient.GetTblReturnHeaderAsync(RecieveHeaderList.Count, PageSize, SelectedMainRow.TblRecInvHeaderType, (int)SelectedMainRow.TblSupplier, "it.glserial", DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
            //}
        }

        public void GetRecieveDetailData()
        {
            var row = new TblRecInvHeader();

            row.InjectFrom(SelectedMainRow);
            //var headers = new ObservableCollection<int>(RecieveHeaderChoosedList.Select(x => x.glserial));


            Dictionary<int, int> headers = new Dictionary<int, int>();

            foreach (var item in RecieveHeaderChoosedList)
            {
                headers.Add(item.glserial, item.TblRecInvHeaderType);
                    
            }
            //if (SelectedMainRow.TblRecInvHeaderType == 1)
            //{
                Glclient.GetTblRecieveDetailAsync(headers, row, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
            //}
            //else
            //{
            //    Glclient.GetTblReturnDetailAsync(headers, row, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
            //}
        }

        public void DeleteMarkupRow(bool header)
        {
        }

        public void UpdateCostInRecInv(RecInvStyle invstyle)
        {
            Glclient.UpdateCostInRecInvAsync(invstyle, null, LoggedUserInfo.DatabasEname);
        }

        public void UpdateCostInRecInv(RecInvStyleColor invstyleColor)
        {
            Glclient.UpdateCostInRecInvAsync(null, invstyleColor, LoggedUserInfo.DatabasEname);
        }

        public void Post()
        {

            if (SelectedMainRow.Invoiced)
            {

                var saveRow = new TblRecInvHeader();
                saveRow.InjectFrom(SelectedMainRow);
                Glclient.PostInvAsync(saveRow, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
            }
            else {
                MessageBox.Show("Transaction Must Be Invoiced First");
}
        }

        public void GetRecFromTo()
        {
            if (SelectedMainRow.TblRecInvHeaderType == 1)
            {
                if (FromDate != null)
                    if (ToDate != null)
                        if (SelectedMainRow.TblSupplier != null)
                            Glclient.GetTblRecieveHeaderFromToAsync((DateTime)FromDate, (DateTime)ToDate, SelectedMainRow.TblRecInvHeaderType, (int)SelectedMainRow.TblSupplier, LoggedUserInfo.DatabasEname);
            }
            else
            {
                if (FromDate != null)
                    if (ToDate != null)
                        if (SelectedMainRow.TblSupplier != null)
                            Glclient.GetTblReturnHeaderFromToAsync((DateTime)FromDate, (DateTime)ToDate, SelectedMainRow.TblRecInvHeaderType, (int)SelectedMainRow.TblSupplier, LoggedUserInfo.DatabasEname);
            }
        }
    }

    #region ViewModels

    public class TblRecInvHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private bool _Invoiced;

        public bool Invoiced
        {
            get { return _Invoiced; }
            set { _Invoiced = value; RaisePropertyChanged("Invoiced"); }
        }

        private int? _TblAccount;

        public int? TblAccount
        {
            get { return _TblAccount; }
            set { _TblAccount = value; RaisePropertyChanged("TblAccount"); }
        }

        private TblAccount _accountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _accountPerRow; }
            set
            {
                if ((ReferenceEquals(_accountPerRow, value) != true))
                {
                    _accountPerRow = value;
                    RaisePropertyChanged("AccountPerRow");
                    TblAccount = _accountPerRow.Iserial;
                }
            }
        }

        private CRUDManagerService.TblStore _storePerRow;

        public CRUDManagerService.TblStore StorePerRow
        {
            get { return _storePerRow; }
            set
            {
                _storePerRow = value; RaisePropertyChanged("StorePerRow");
                TblStore = StorePerRow.iserial;
            }
        }

        private int? _tblStore;

        public int? TblStore
        {
            get { return _tblStore; }
            set { _tblStore = value; RaisePropertyChanged("TblStore"); }
        }

        private double _miscValueField;

        public double Misc
        {
            get
            {
                return _miscValueField;
            }
            set
            {
                if ((_miscValueField.Equals(value) != true))
                {
                    _miscValueField = value;
                    RaisePropertyChanged("Misc");
                }
            }
        }

        private GenericTable _tblRecInvHeaderTypPerRow;

        public GenericTable TblRecInvHeaderTypePerRow
        {
            get { return _tblRecInvHeaderTypPerRow; }
            set
            {
                _tblRecInvHeaderTypPerRow = value; RaisePropertyChanged("TblRecInvHeaderTypePerRow");
                if (_tblRecInvHeaderTypPerRow != null) TblRecInvHeaderType = _tblRecInvHeaderTypPerRow.Iserial;
            }
        }

        private int _tblRecInvHeaderType;

        public int TblRecInvHeaderType
        {
            get { return _tblRecInvHeaderType; }
            set { _tblRecInvHeaderType = value; RaisePropertyChanged("TblRecInvHeaderType"); }
        }

        private int _status;

        public int Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private int _CreatedBy;

        public int CreatedBy
        {
            get { return _CreatedBy; }
            set { _CreatedBy = value; RaisePropertyChanged("CreatedBy"); }
        }

        private int _PostBy;

        public int PostBy
        {
            get { return _PostBy; }
            set { _PostBy = value; RaisePropertyChanged("PostBy"); }
        }

        private bool _Posted;

        public bool Posted
        {
            get { return _Posted; }
            set { _Posted = value; RaisePropertyChanged("Posted"); }
        }

        private bool _visPosted;

        public bool VisPosted
        {
            get { return _visPosted; }
            set { _visPosted = value; RaisePropertyChanged("VisPosted"); }
        }

        public TblRecInvHeaderViewModel()
        {
            CreationDate = DateTime.Now;
        }

        private DateTime? _transDate;

        private DateTime? _creationDate;
        private DateTime? _postDate;
        private int _iserialField;

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; RaisePropertyChanged("Code"); }
        }

        private string _supplierInv;

        public string SupplierInv
        {
            get { return _supplierInv; }
            set
            {
                _supplierInv = value; RaisePropertyChanged("SupplierInv");
                Checkvalid();
            }
        }

        private SortableCollectionView<TblRecInvDetailViewModel> _subdetailsList;

        public SortableCollectionView<TblRecInvDetailViewModel> SubDetailsList
        {
            get { return _subdetailsList ?? (_subdetailsList = new SortableCollectionView<TblRecInvDetailViewModel>()); }
            set
            {
                _subdetailsList = value;
                RaisePropertyChanged("SubDetailsList");
            }
        }

        private SortableCollectionView<TblRecInvMainDetailViewModel> _detailsList;

        public SortableCollectionView<TblRecInvMainDetailViewModel> DetailsList
        {
            get { return _detailsList ?? (_detailsList = new SortableCollectionView<TblRecInvMainDetailViewModel>()); }
            set
            {
                _detailsList = value;
                RaisePropertyChanged("DetailsList");
            }
        }

        private SortableCollectionView<TblMarkupTransViewModel> _markUpTransList;

        public SortableCollectionView<TblMarkupTransViewModel> MarkUpTransList
        {
            get { return _markUpTransList ?? (_markUpTransList = new SortableCollectionView<TblMarkupTransViewModel>()); }
            set
            {
                _markUpTransList = value;
                RaisePropertyChanged("MarkUpTransList");
            }
        }

        private SortableCollectionView<TblRecInvMainDetailViewModel> _styleDetailList;

        public SortableCollectionView<TblRecInvMainDetailViewModel> StyleDetailList
        {
            get { return _styleDetailList ?? (_styleDetailList = new SortableCollectionView<TblRecInvMainDetailViewModel>()); }
            set
            {
                _styleDetailList = value;
                RaisePropertyChanged("StyleDetailList");
            }
        }

        private SortableCollectionView<TblRecInvMainDetailViewModel> _styleColordetailsList;

        public SortableCollectionView<TblRecInvMainDetailViewModel> StyleColorDetailsList
        {
            get { return _styleColordetailsList ?? (_styleColordetailsList = new SortableCollectionView<TblRecInvMainDetailViewModel>()); }
            set
            {
                _styleColordetailsList = value;
                RaisePropertyChanged("StyleColorDetailsList");
            }
        }

        private int? _tblSuplier;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSupplier")]
        public int? TblSupplier
        {
            get { return _tblSuplier; }
            set
            {
                _tblSuplier = value;
                RaisePropertyChanged("TblSupplier");
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public DateTime? PostDate
        {
            get { return _postDate; }
            set
            {
                _postDate = value;
                RaisePropertyChanged("PostDate");
            }
        }

        public DateTime? TransDate
        {
            get { return _transDate; }
            set
            {
                _transDate = value;
                RaisePropertyChanged("TransDate");
                Checkvalid();
            }
        }

        public DateTime? CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                RaisePropertyChanged("CreationDate");
            }
        }

        private CRUDManagerService.TBLsupplier _supplierPerRow;

        public CRUDManagerService.TBLsupplier SupplierPerRow
        {
            get
            {
                return _supplierPerRow;
            }
            set
            {
                if ((ReferenceEquals(_supplierPerRow, value) != true))
                {
                    _supplierPerRow = value;
                    RaisePropertyChanged("SupplierPerRow");
                    if (SupplierPerRow != null)
                    {
                        if (SupplierPerRow.Iserial != 0)
                        {
                            TblSupplier = SupplierPerRow.Iserial;
                            Checkvalid();
                        }
                    }
                }
            }
        }

        private void Checkvalid()
        {
            if (Iserial == 0)
            {
                if (TblSupplier != null && TransDate != null && SupplierInv != null && TblRecInvHeaderType != 0)
                {
                    Valid = true;
                }
                else
                {
                    Valid = false;
                }
            }
        }

        private bool _valid;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; RaisePropertyChanged("Valid"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }
    }

    public class TblRecInvMainDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private decimal _costField;

        private int _iserialField;

        private decimal _qtyField;

        private int? _tblCurrencyField;

        private int? _tblItemField;

        private int? _tblRecInvHeaderField;

        private int? _tblSTaxField;

        private TBLITEMprice _tbliteMprice;

        public TBLITEMprice TBLITEMprice
        {
            get { return _tbliteMprice; }
            set { _tbliteMprice = value; RaisePropertyChanged("TBLITEMprice"); }
        }

        private GenericTable _currencyPerRow;

        public GenericTable CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (CurrencyPerRow != null) TblCurrency = CurrencyPerRow.Iserial;
            }
        }

        public decimal Cost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
                    Total = Cost * Qty;
                }
            }
        }

        private decimal _total;

        [ReadOnly(true)]
        public decimal Total
        {
            get { return _total; }
            set { _total = value; RaisePropertyChanged("Total"); }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        private SortableCollectionView<TblMarkupTransViewModel> _markUpTransList;

        public SortableCollectionView<TblMarkupTransViewModel> MarkUpTransList
        {
            get { return _markUpTransList ?? (_markUpTransList = new SortableCollectionView<TblMarkupTransViewModel>()); }
            set
            {
                _markUpTransList = value;
                RaisePropertyChanged("MarkUpTransList");
            }
        }

        [ReadOnly(true)]
        public decimal Qty
        {
            get
            {
                return _qtyField;
            }
            set
            {
                if ((_qtyField.Equals(value) != true))
                {
                    _qtyField = value;
                    RaisePropertyChanged("Qty");
                    Total = Cost * Qty;
                }
            }
        }

        public int? TblCurrency
        {
            get
            {
                return _tblCurrencyField;
            }
            set
            {
                if ((_tblCurrencyField.Equals(value) != true))
                {
                    _tblCurrencyField = value;
                    RaisePropertyChanged("TblCurrency");
                }
            }
        }

        public int? TblItem
        {
            get
            {
                return _tblItemField;
            }
            set
            {
                if ((_tblItemField.Equals(value) != true))
                {
                    _tblItemField = value;
                    RaisePropertyChanged("TblItem");
                }
            }
        }

        public int? TblRecInvHeader
        {
            get
            {
                return _tblRecInvHeaderField;
            }
            set
            {
                if ((_tblRecInvHeaderField.Equals(value) != true))
                {
                    _tblRecInvHeaderField = value;
                    RaisePropertyChanged("TblRecInvHeader");
                }
            }
        }

        public int? TblSTax
        {
            get
            {
                return _tblSTaxField;
            }
            set
            {
                if ((_tblSTaxField.Equals(value) != true))
                {
                    _tblSTaxField = value;
                    RaisePropertyChanged("TblSTax");
                }
            }
        }

        private decimal _miscField;

        public decimal Misc
        {
            get
            {
                return _miscField;
            }
            set
            {
                if ((_miscField.Equals(value) != true))
                {
                    _miscField = value;
                    RaisePropertyChanged("Misc");
                }
            }
        }


        decimal _ContractQty;
        [ReadOnly(true)]
        public decimal ContractQty
        {
            get
            {
                return _ContractQty;
            }
            set
            {
                //if ((_ContractQty.Equals(value) != true))
                //{
                    _ContractQty = value;
                    RaisePropertyChanged("ContractQty");
                   
                //}
            }
        }
        decimal _ContractCost;
        [ReadOnly(true)]
        public decimal ContractCost
        {
            get
            {
                return _ContractCost;
            }
            set
            {
                //if ((_ContractQty.Equals(value) != true))
                //{
                _ContractCost = value;
                RaisePropertyChanged("ContractCost");

                //}
            }
        }

        decimal _ContractTotal;
        [ReadOnly(true)]
        public decimal ContractTotal
        {
            get
            {
                return _ContractTotal;
            }
            set
            {
                //if ((_ContractQty.Equals(value) != true))
                //{
                _ContractTotal = value;
                RaisePropertyChanged("ContractTotal");

                //}
            }
        }


    }

    public class TblRecInvDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private double? _costField;

        private int _dserialField;

        private int? _flgField;

        private int _glserialField;

        private double _miscField;

        private int? _tblRecInvHeaderField;

        private int _tblitemField;

        public double? Cost
        {
            get
            {
                return _costField;
            }
            set
            {
                if ((_costField.Equals(value) != true))
                {
                    _costField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }

        public int Dserial
        {
            get
            {
                return _dserialField;
            }
            set
            {
                if ((_dserialField.Equals(value) != true))
                {
                    _dserialField = value;
                    RaisePropertyChanged("Dserial");
                }
            }
        }

        public int? Flg
        {
            get
            {
                return _flgField;
            }
            set
            {
                if ((_flgField.Equals(value) != true))
                {
                    _flgField = value;
                    RaisePropertyChanged("Flg");
                }
            }
        }

        public int Glserial
        {
            get
            {
                return _glserialField;
            }
            set
            {
                if ((_glserialField.Equals(value) != true))
                {
                    _glserialField = value;
                    RaisePropertyChanged("Glserial");
                }
            }
        }

        public double Misc
        {
            get
            {
                return _miscField;
            }
            set
            {
                if ((_miscField.Equals(value) != true))
                {
                    _miscField = value;
                    RaisePropertyChanged("Misc");
                }
            }
        }

        public int? TblRecInvHeader
        {
            get
            {
                return _tblRecInvHeaderField;
            }
            set
            {
                if ((_tblRecInvHeaderField.Equals(value) != true))
                {
                    _tblRecInvHeaderField = value;
                    RaisePropertyChanged("TblRecInvHeader");
                }
            }
        }

        public int Tblitem
        {
            get
            {
                return _tblitemField;
            }
            set
            {
                if ((_tblitemField.Equals(value) != true))
                {
                    _tblitemField = value;
                    RaisePropertyChanged("Tblitem");
                }
            }
        }
    }

    public class TblReciveHeaderViewModel : RecieveView
    {
        private bool _check;

        public bool Checked
        {
            get { return _check; }
            set { _check = value; RaisePropertyChanged("Checked"); }
        }
    }

    public class TblMarkupTransViewModel : Web.DataLayer.PropertiesViewModelBase
    {


        private GlService.GenericTable _JournalAccountTypePerRow;
        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _JournalAccountTypePerRow; }
            set
            {
                _JournalAccountTypePerRow = value; RaisePropertyChanged("JournalAccountTypePerRow");
                TblJournalAccountType = _JournalAccountTypePerRow.Iserial;
            }
        }

        int TblJournalAccountTypeField;
        public int TblJournalAccountType
        {
            get
            {
                return this.TblJournalAccountTypeField;
            }
            set
            {
                if ((this.TblJournalAccountTypeField.Equals(value) != true))
                {
                    this.TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        private GlService.Entity _EntityPerRow;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEntity")]
        public GlService.Entity EntityPerRow
        {
            get { return _EntityPerRow; }
            set
            {
                _EntityPerRow = value; RaisePropertyChanged("EntityPerRow");
                EntityAccount = _EntityPerRow.Iserial;
            }
        }

        int EntityAccountField;
        public int EntityAccount
        {
            get
            {
                return this.EntityAccountField;
            }
            set
            {
                if ((this.EntityAccountField.Equals(value) != true))
                {
                    this.EntityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }



        //private CRUDManagerService.TBLsupplier _supplierPerRow;

        //public CRUDManagerService.TBLsupplier SupplierPerRow
        //{
        //    get
        //    {
        //        return _supplierPerRow;
        //    }
        //    set
        //    {
        //        if ((ReferenceEquals(_supplierPerRow, value) != true))
        //        {
        //            _supplierPerRow = value;
        //            RaisePropertyChanged("SupplierPerRow");
        //            if (SupplierPerRow != null)
        //            {
        //                if (SupplierPerRow.Iserial != 0)
        //                {
        //                    TblSupplier = SupplierPerRow.Iserial;
        //                }
        //            }
        //        }
        //    }
        //}



        private int _iserialField;

        private double? _miscValueField;

        private int _tblMarkupField;

        private TblMarkup _tblMarkup1Field;

        private int _tblcurrency;

        public int TblCurrency
        {
            get { return _tblcurrency; }
            set { _tblcurrency = value; RaisePropertyChanged("TblCurrency"); }
        }

        private bool _vendorEffect;

        public bool VendorEffect
        {
            get { return _vendorEffect; }
            set { _vendorEffect = value; RaisePropertyChanged("VendorEffect"); }
        }

        private int _miscValueType;

        public int MiscValueType
        {
            get { return _miscValueType; }
            set { _miscValueType = value; RaisePropertyChanged("MiscValueType"); }
        }

        public TblMarkup TblMarkup1
        {
            get { return _tblMarkup1Field; }
            set
            {
                _tblMarkup1Field = value; RaisePropertyChanged("TblMarkup1");
                if (TblMarkup1 != null) TblMarkup = TblMarkup1.Iserial;
            }
        }

        private GenericTable _currencyPerRow;

        public GenericTable CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (_currencyPerRow != null && _currencyPerRow.Iserial!=0) TblCurrency = CurrencyPerRow.Iserial;
            }
        }

        private int _tblRecInvField;

        private int _typeField;

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public double? MiscValue
        {
            get
            {
                return _miscValueField;
            }
            set
            {
                if ((_miscValueField.Equals(value) != true))
                {
                    _miscValueField = value;
                    RaisePropertyChanged("MiscValue");
                }
            }
        }

        public int TblMarkup
        {
            get
            {
                return _tblMarkupField;
            }
            set
            {
                if ((_tblMarkupField.Equals(value) != true))
                {
                    _tblMarkupField = value;
                    RaisePropertyChanged("TblMarkup");
                }
            }
        }

        public int TblRecInv
        {
            get
            {
                return _tblRecInvField;
            }
            set
            {
                if ((_tblRecInvField.Equals(value) != true))
                {
                    _tblRecInvField = value;
                    RaisePropertyChanged("TblRecInv");
                }
            }
        }

        public int Type
        {
            get
            {
                return _typeField;
            }
            set
            {
                if ((_typeField.Equals(value) != true))
                {
                    _typeField = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        private bool _disabled;

        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; RaisePropertyChanged("Disabled"); }
        }

        private double _ExchangeRate;

        public double ExchangeRate
        {
            get { return _ExchangeRate; }
            set { _ExchangeRate = value; RaisePropertyChanged("ExchangeRate"); }
        }

    }

    #endregion ViewModels
}