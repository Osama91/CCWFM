using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using GenericTable = CCWFM.CRUDManagerService.GenericTable;


namespace CCWFM.ViewModel.OGViewModels
{
    public class RecInvProductionViewModel : ViewModelBase
    {
        public PurchasePlanService.PurchasePlanClient PurchasePlanClient = new PurchasePlanService.PurchasePlanClient();

        public RecInvProductionViewModel()
        {
            if (!IsDesignTime)
            {
                MiscValueTypeList = new ObservableCollection<GenericTable>
                {
                    new GenericTable {Iserial = 0, Code = "%", Ename = "%", Aname = "%"},
                    new GenericTable {Iserial = 1, Code = "Value", Ename = "Value", Aname = "Value"}
                };
                GetItemPermissions(PermissionItemName.RecInvProd.ToString());
                Client = new CRUD_ManagerServiceClient();
                MainRowList = new SortableCollectionView<TblRecInvHeaderProdViewModel>();
                SelectedMainRow = new TblRecInvHeaderProdViewModel();


                var journalAccountTypeClient = new GlService.GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var currencyClient = new GlService.GlServiceClient();
                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = new ObservableCollection<CRUDManagerService.GenericTable>();
                    foreach (var item in sv.Result)
                    {
                        CurrencyList.Add(new CRUDManagerService.GenericTable().InjectFrom(item) as CRUDManagerService.GenericTable);
                    }
                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                PurchasePlanClient.GetTblMarkupProdAsync(0, int.MaxValue, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);

                PurchasePlanClient.GetTblMarkupProdCompleted += (s, sv) =>
                {
                    MarkupList = new ObservableCollection<TblMarkupProd>();
                    foreach (var item in sv.Result)
                    {
                        MarkupList.Add(new TblMarkupProd().InjectFrom(item) as TblMarkupProd);
                    }
                };

                var tblRecInvHeaderTypeProdClient = new CRUD_ManagerServiceClient();
                tblRecInvHeaderTypeProdClient.GetGenericCompleted += (s, sv) =>
                {
                    TblRecInvHeaderTypeProdList = sv.Result;
                };
                tblRecInvHeaderTypeProdClient.GetGenericAsync("TblRecInvHeaderTypeProd", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Client.GetTblRecInvHeaderProdCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRecInvHeaderProdViewModel();
                        newrow.InjectFrom(row);
                        if (newrow.Status == 0)
                        {
                            newrow.VisPosted = true;
                        }

                        newrow.TblRecInvHeaderTypeProdPerRow = new GenericTable();
                        if (row.TblRecInvHeaderTypeProd1 != null)
                        {
                            newrow.TblRecInvHeaderTypeProdPerRow = new GenericTable().InjectFrom(row.TblRecInvHeaderTypeProd1) as GenericTable;
                        }
                        newrow.VendorPerRow = new CRUDManagerService.Vendor { vendor_code = row.TblSupplier };

                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                };

                Client.GetTblRecInvMainDetailProdCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblRecInvMainDetailProdViewModel { CurrencyPerRow = new GenericTable() };
                        newrow.CurrencyPerRow.InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == row.TblCurrency));
                        var firstOrDefault = sv.itemsList.FirstOrDefault(w => w.ItemGroup == row.ItemType && w.Iserial == row.TblItem);
                        if (firstOrDefault != null)
                            newrow.Style = firstOrDefault.Code;
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    TotalCost = sv.TotalAmount;
                    TotalQty = sv.TotalQty;
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                };

                Client.UpdateOrInsertTblRecInvHeaderProdsCompleted += (s, ev) =>
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
                Client.DeleteTblRecInvHeaderProdCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.PostInvCompleted += (s, sv) =>
                {
                    if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);
                    SelectedMainRow.VisPosted = false;
                    MessageBox.Show("Posted Completed");
                };
                Client.GetTblRecieveDetailCompleted += (s, sv) =>
                {
                    if (sv.Result != null) SelectedMainRow.InjectFrom(sv.Result);
                    SelectedMainRow.VisPosted = true;
                    GetDetailData();
                };
                Client.GetTblRecieveHeaderProdFromToCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {

                            if (!RecieveHeaderChoosedList.Select(x => x.DocCode).Contains(row.DocCode))
                            {
                                RecieveHeaderChoosedList.Add(new TblReciveHeaderProdViewModel().InjectFrom(row) as TblReciveHeaderProdViewModel);
                            }
                        }
                    }
                };

                Client.GetTblRecieveHeaderProdCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblReciveHeaderProdViewModel();

                        newrow.InjectFrom(row);


                        RecieveHeaderList.Add(newrow);
                    }
                    Loading = false;
                    DetailSubFullCount = sv.fullCount;
                };
                Client.UpdateOrInsertTblRecInvMainDetailProdCompleted += (s, x) =>
                {
                    TotalCost = x.TotalAmount;
                    TotalQty = x.TotalQty;
                    Loading = false;
                };
                PurchasePlanClient.DeleteTblMarkupTransProdsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.MarkUpTransList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.MarkUpTransList.Remove(oldrow);
                    if (!SelectedMainRow.MarkUpTransList.Any())
                    {
                        AddNewMarkUpRow(false, true);
                    }
                };

                PurchasePlanClient.UpdateOrInsertTblMarkupTransProdsCompleted += (s, x) =>
                {
                    var markup = new TblMarkupProd();
                    try
                    {
                        var row = SelectedMainRow.MarkUpTransList.ElementAt(x.outindex);
                        if (row != null)
                        {
                            markup = row.TblMarkupProd1;
                        }
                        if (x.Result.Iserial == -1)
                        {
                            MessageBox.Show("This Markup Is Not Linked To Account");
                            row.TblMarkupProd1 = null;
                        }
                        else
                        {
                            if (x.Result.Type == 0)
                            {
                                SelectedMainRow.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);
                            }
                            else
                            {
                                SelectedDetailRow.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);
                            }
                            if (row != null) row.TblMarkupProd1 = markup;
                        }

                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }

                    Loading = false;
                };

                PurchasePlanClient.GetTblTblMarkupTransProdCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                       
                        var newrow = new TblMarkupTranProdViewModel();
                        newrow.InjectFrom(row);                        
                        newrow.CurrencyPerRow = new GenericTable();
                        newrow.CurrencyPerRow.InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == row.TblCurrency));
                        newrow.TblMarkupProd1 = new TblMarkupProd();
                        newrow.TblMarkupProd1.InjectFrom(MarkupList.FirstOrDefault(w => w.Iserial == row.TblMarkupProd));
                        newrow.TblMarkupProd = row.TblMarkupProd;
                        newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == newrow.TblJournalAccountType);
                        var entity = sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType);
                        newrow.EntityPerRow = new GlService.Entity();

                        if (entity != null)
                        {

                            newrow.EntityPerRow= new GlService.Entity().InjectFrom(entity) as GlService.Entity;
                        }
                        newrow.TblJournalAccountType = row.TblJournalAccountType;
                        newrow.EntityAccount = row.EntityAccount;
                        SelectedMainRow.MarkUpTransList.Add(newrow);
                    }

                    Loading = false;

                    if (SelectedMainRow.MarkUpTransList.Count == 0)
                    {
                        AddNewMarkUpRow(false, true);
                    }
                };
            }
        }

        private ObservableCollection<TblMarkupProd> _markupList;

        public ObservableCollection<TblMarkupProd> MarkupList
        {
            get { return _markupList; }
            set { _markupList = value; RaisePropertyChanged("MarkupList"); }
        }

        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private double _totalQty;

        public double TotalQty
        {
            get { return _totalQty; }
            set { _totalQty = value; RaisePropertyChanged("TotalQty"); }
        }

        private double _totalCost;

        public double TotalCost
        {
            get { return _totalCost; }
            set { _totalCost = value; RaisePropertyChanged("TotalCost"); }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc ";
            Loading = true;
            Client.GetTblRecInvHeaderProdAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblRecInvHeaderProdViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void AddNewMarkUpRow(bool checkLastRow, bool header)
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
                var journalrow = new GlService.GenericTable()
                {
                    Ename = "Vendor",
                    Aname = "Vendor",
                    Code = "Vendor",
                    Iserial = 2
                };

                //journalrow
                var newrow = new TblMarkupTranProdViewModel
                {
                    Type = 0,
                    TblRecInv = SelectedMainRow.Iserial,
                    MiscValueType = 1,
                    TblJournalAccountType = 2,
                    JournalAccountTypePerRow = journalrow,
                    ExchangeRate=1
                    //EntityPerRow = new GlService.Entity()
                    //{
                    //    TblJournalAccountType = 2,
                    //    Code = SelectedMainRow.VendorPerRow.vendor_code,
                    //}
                };

                SelectedMainRow.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
                SelectedMarkupRow = newrow;
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
                    var saveRow = new TblRecInvMainDetailProd();
                    saveRow.InjectFrom(SelectedDetailRow);

                    saveRow.TblRecInvHeaderProd1 = null;
                    if (!Loading)
                    {
                        saveRow.TblRecInvHeaderProd = SelectedMainRow.Iserial;
                        Loading = true;
                        int xxxx = SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow);
                        Client.UpdateOrInsertTblRecInvMainDetailProdAsync(saveRow, save, xxxx, LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }


        public void SaveDetailRows()
        {
            foreach (var item in SelectedMainRow.DetailsList)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(item,
                    new ValidationContext(item, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = item.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblRecInvMainDetailProd();
                    saveRow.InjectFrom(item);

                    saveRow.TblRecInvHeaderProd1 = null;

                    saveRow.TblRecInvHeaderProd = SelectedMainRow.Iserial;
                    int xxxx = SelectedMainRow.DetailsList.IndexOf(item);
                    Client.UpdateOrInsertTblRecInvMainDetailProdAsync(saveRow, save, xxxx, LoggedUserInfo.DatabasEname);
                }
            }       
    }

    //public void SaveMainRow()
    //{
    //	if (SelectedMainRow != null)
    //	{
    //		var valiationCollection = new List<ValidationResult>();

    //		var isvalid = Validator.TryValidateObject(SelectedMainRow,
    //			new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

    //		if (isvalid)
    //		{
    //			var save = SelectedMainRow.Iserial == 0;
    //			if (AllowUpdate != true && !save)
    //			{
    //				MessageBox.Show(strings.AllowUpdateMsg);
    //				return;
    //			}
    //			var saveRow = new TblRecInvHeaderProd();
    //			saveRow.InjectFrom(SelectedMainRow);

    //			saveRow.TblRecInvDetailProds = new ObservableCollection<TblRecInvDetailProd>();
    //			GenericMapper.InjectFromObCollection(saveRow.TblRecInvMainDetailProds, SelectedMainRow.DetailsList);
    //			//GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
    //			if (!Loading)
    //			{

    //				Loading = true;

    //				Client.UpdateOrInsertTblRecInvHeaderProdsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
    //					LoggedUserInfo.DatabasEname);
    //			}
    //		}
    //	}
    //}

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
                var saveRow = new PurchasePlanService.TblMarkupTransProd();
                saveRow.InjectFrom(SelectedMarkupRow);
                saveRow.TblRecInv = SelectedMainRow.Iserial;

                //saveRow.TblMarkup1 = null;

                //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                if (!Loading)
                {
                    Loading = true;

                    if (SelectedMarkupRow.Type == 0)
                    {
                        PurchasePlanClient.UpdateOrInsertTblMarkupTransProdsAsync(saveRow, save, SelectedMainRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
                  LoggedUserInfo.DatabasEname);
                    }
                    else
                    {
                        PurchasePlanClient.UpdateOrInsertTblMarkupTransProdsAsync(saveRow, save, SelectedDetailRow.MarkUpTransList.IndexOf(SelectedMarkupRow),
            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }
    }

    public void SaveMarkupRowOldRow(TblMarkupTranProdViewModel oldRow)
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
                var saveRow = new PurchasePlanService.TblMarkupTransProd();
                saveRow.InjectFrom(oldRow);
                saveRow.TblRecInv = SelectedMainRow.Iserial;

                //saveRow.TblMarkup1 = null;
                if (!Loading)
                {
                    Loading = true;
                    //GenericMapper.InjectFromObCollection(saveRow.TblRecInvDetails, SelectedMainRow.SubDetailsList);
                    if (oldRow.Type == 0)
                    {
                        PurchasePlanClient.UpdateOrInsertTblMarkupTransProdsAsync(saveRow, save,
                            SelectedMainRow.MarkUpTransList.IndexOf(oldRow), LoggedUserInfo.DatabasEname);
                    }
                    else
                    {
                        PurchasePlanClient.UpdateOrInsertTblMarkupTransProdsAsync(saveRow, save,
                            SelectedDetailRow.MarkUpTransList.IndexOf(oldRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }
    }

    public void DeleteMainRow()
    {
        var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
            MessageBoxButton.OKCancel);
        if (res == MessageBoxResult.OK)
        {
            Client.DeleteTblRecInvHeaderProdAsync((TblRecInvHeaderProd)new TblRecInvHeaderProd().InjectFrom(SelectedMainRow),
                MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
        }
    }

    public void GetDetailData()
    {
        if (DetailSortBy == null)
            DetailSortBy = "it.Iserial";
        Loading = true;
        if (SelectedMainRow != null)
            Client.GetTblRecInvMainDetailProdAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
    }

    public void GetMarkUpdata(bool header)
    {
        Loading = true;
        PurchasePlanClient.GetTblTblMarkupTransProdAsync(0, SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
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

    private ObservableCollection<GenericTable> _tblRecInvHeaderTypeProdList;

    public ObservableCollection<GenericTable> TblRecInvHeaderTypeProdList
    {
        get { return _tblRecInvHeaderTypeProdList; }
        set { _tblRecInvHeaderTypeProdList = value; RaisePropertyChanged("TblRecInvHeaderTypeProdList"); }
    }

    private SortableCollectionView<TblRecInvHeaderProdViewModel> _mainRowList;

    public SortableCollectionView<TblRecInvHeaderProdViewModel> MainRowList
    {
        get { return _mainRowList ?? (_mainRowList = new SortableCollectionView<TblRecInvHeaderProdViewModel>()); }
        set
        {
            _mainRowList = value;
            RaisePropertyChanged("MainRowList");
        }
    }

    private ObservableCollection<TblRecInvHeaderProdViewModel> _selectedMainRows;

    public ObservableCollection<TblRecInvHeaderProdViewModel> SelectedMainRows
    {
        get
        {
            return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblRecInvHeaderProdViewModel>());
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

    private TblRecInvHeaderProdViewModel _selectedMainRow;

    public TblRecInvHeaderProdViewModel SelectedMainRow
    {
        get { return _selectedMainRow; }
        set
        {
            _selectedMainRow = value;
            RaisePropertyChanged("SelectedMainRow");
        }
    }

    private TblMarkupTranProdViewModel _selectedMarkup;

    public TblMarkupTranProdViewModel SelectedMarkupRow
    {
        get { return _selectedMarkup; }
        set
        {
            _selectedMarkup = value;
            RaisePropertyChanged("SelectedMarkupRow");
        }
    }

    private ObservableCollection<TblMarkupTranProdViewModel> _SelectedMarkupRows;

    public ObservableCollection<TblMarkupTranProdViewModel> SelectedMarkupRows
    {
        get { return _SelectedMarkupRows ?? (_SelectedMarkupRows = new ObservableCollection<TblMarkupTranProdViewModel>()); }
        set
        {
            _SelectedMarkupRows = value;
            RaisePropertyChanged("SelectedMarkupRows");
        }
    }



    private TblRecInvMainDetailProdViewModel _selectedDetailRow;

    public TblRecInvMainDetailProdViewModel SelectedDetailRow
    {
        get { return _selectedDetailRow; }
        set
        {
            _selectedDetailRow = value;
            RaisePropertyChanged("SelectedDetailRow");
        }
    }

    private ObservableCollection<TblRecInvMainDetailProdViewModel> _selectedDetailRows;

    public ObservableCollection<TblRecInvMainDetailProdViewModel> SelectedDetailRows
    {
        get { return _selectedDetailRows; }
        set
        {
            _selectedDetailRows = value;
            RaisePropertyChanged("SelectedDetailRows");
        }
    }

    private ObservableCollection<TblReciveHeaderProdViewModel> _recieveHeaderList;

    public ObservableCollection<TblReciveHeaderProdViewModel> RecieveHeaderList
    {
        get { return _recieveHeaderList ?? (_recieveHeaderList = new ObservableCollection<TblReciveHeaderProdViewModel>()); }
        set
        {
            _recieveHeaderList = value;
            RaisePropertyChanged("RecieveHeaderList");
        }
    }

    private ObservableCollection<TblReciveHeaderProdViewModel> _recieveHeaderChoosedList;

    public ObservableCollection<TblReciveHeaderProdViewModel> RecieveHeaderChoosedList
    {
        get { return _recieveHeaderChoosedList ?? (_recieveHeaderChoosedList = new ObservableCollection<TblReciveHeaderProdViewModel>()); }
        set
        {
            _recieveHeaderChoosedList = value;
            RaisePropertyChanged("RecieveHeaderChoosedList");
        }
    }

    #endregion Prop

    public void GetRecieveHeaderListData()
    {
        Client.GetTblRecieveHeaderProdAsync(RecieveHeaderList.Count, PageSize, SelectedMainRow.TblRecInvHeaderTypeProd, SelectedMainRow.TblSupplier, "it.Iserial", DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
    }

    public void GetRecieveDetailData()
    {
        var row = new TblRecInvHeaderProd();

        row.InjectFrom(SelectedMainRow);
        if (RecieveHeaderChoosedList != null)
        {
            var headers = new ObservableCollection<int?>(RecieveHeaderChoosedList.Select(x => (int?)x.Iserial));
            Client.GetTblRecieveDetailAsync(headers, row, LoggedUserInfo.DatabasEname);
        }
    }

    public void DeleteMarkupRow(bool header)
    {

        if (SelectedMarkupRows != null)
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
              MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                foreach (var row in SelectedMarkupRows)
                {
                    if (row.Iserial != 0)
                    {
                        if (AllowDelete != true)
                        {
                            MessageBox.Show(strings.AllowDeleteMsg);
                            return;
                        }

                        PurchasePlanClient.DeleteTblMarkupTransProdsAsync((PurchasePlanService.TblMarkupTransProd)new PurchasePlanService.TblMarkupTransProd().InjectFrom(row), SelectedMainRow.MarkUpTransList.IndexOf(row), LoggedUserInfo.DatabasEname);
                    }
                    else
                    {
                        SelectedMainRow.MarkUpTransList.Remove(row);
                        if (!SelectedMainRow.MarkUpTransList.Any())
                        {
                            AddNewMarkUpRow(false, header);
                        }
                    }
                }
            }
        }


        //var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
        //    MessageBoxButton.OKCancel);
        //if (res == MessageBoxResult.OK)
        //{
        //    PurchasePlanClient.DeleteTblMarkupTransProdsAsync((TblMarkupTransProd)new TblMarkupTransProd().InjectFrom(SelectedMainRow),
        //        MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
        //}
    }

    public void Post()
    {
        var saveRow = new TblRecInvHeaderProd();
        saveRow.InjectFrom(SelectedMainRow);
        Client.PostInvAsync(saveRow, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
    }

    public void GetRecFromTo()
    {
        if (FromDate != null)
            if (ToDate != null)
                Client.GetTblRecieveHeaderProdFromToAsync((DateTime)FromDate, (DateTime)ToDate, SelectedMainRow.TblRecInvHeaderTypeProd, SelectedMainRow.TblSupplier, LoggedUserInfo.DatabasEname);
    }
}

#region Models

public class TblRecInvHeaderProdViewModel : PropertiesViewModelBase
{
    private Vendor _vendorPerRowVendor;

    public Vendor VendorPerRow
    {
        get { return _vendorPerRowVendor; }
        set
        {
            _vendorPerRowVendor = value; RaisePropertyChanged("VendorPerRow");
            if (_vendorPerRowVendor != null && _vendorPerRowVendor.vendor_code != null)
            {
                TblSupplier = _vendorPerRowVendor.vendor_code;
            }
        }
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

    public GenericTable TblRecInvHeaderTypeProdPerRow
    {
        get { return _tblRecInvHeaderTypPerRow; }
        set
        {
            _tblRecInvHeaderTypPerRow = value; RaisePropertyChanged("TblRecInvHeaderTypeProdPerRow");
            if (_tblRecInvHeaderTypPerRow != null) TblRecInvHeaderTypeProd = _tblRecInvHeaderTypPerRow.Iserial;
        }
    }

    private int _tblRecInvHeaderTypeProd;

    public int TblRecInvHeaderTypeProd
    {
        get { return _tblRecInvHeaderTypeProd; }
        set { _tblRecInvHeaderTypeProd = value; RaisePropertyChanged("TblRecInvHeaderTypeProd"); }
    }

    private int _status;

    public int Status
    {
        get { return _status; }
        set { _status = value; RaisePropertyChanged("Status"); }
    }

    private bool _visPosted;

    public bool VisPosted
    {
        get { return _visPosted; }
        set { _visPosted = value; RaisePropertyChanged("VisPosted"); }
    }

    public TblRecInvHeaderProdViewModel()
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

    private string _refNo;

    public string SupplierInv
    {
        get { return _refNo; }
        set
        {
            _refNo = value; RaisePropertyChanged("SupplierInv");
            Checkvalid();
        }
    }

    private ObservableCollection
        <TblRecInvDetailViewModel> _subdetailsList;

    public ObservableCollection<TblRecInvDetailViewModel> SubDetailsList
    {
        get { return _subdetailsList ?? (_subdetailsList = new ObservableCollection<TblRecInvDetailViewModel>()); }
        set
        {
            _subdetailsList = value;
            RaisePropertyChanged("SubDetailsList");
        }
    }

    private ObservableCollection<TblRecInvMainDetailProdViewModel> _detailsList;

    public ObservableCollection<TblRecInvMainDetailProdViewModel> DetailsList
    {
        get { return _detailsList ?? (_detailsList = new ObservableCollection<TblRecInvMainDetailProdViewModel>()); }
        set
        {
            _detailsList = value;
            RaisePropertyChanged("DetailsList");
        }
    }

    private ObservableCollection<TblMarkupTranProdViewModel> _markUpTransList;

    public ObservableCollection<TblMarkupTranProdViewModel> MarkUpTransList
    {
        get { return _markUpTransList ?? (_markUpTransList = new ObservableCollection<TblMarkupTranProdViewModel>()); }
        set
        {
            _markUpTransList = value;
            RaisePropertyChanged("MarkUpTransList");
        }
    }

    private string _tblSuplier;

    [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSupplier")]
    public string TblSupplier
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

    private void Checkvalid()
    {
        if (Iserial == 0)
        {
            if (TblSupplier != null && TransDate != null && SupplierInv != null && TblRecInvHeaderTypeProd != 0)
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

public class TblRecInvMainDetailProdViewModel : PropertiesViewModelBase
{

    private double _ExchangeRate;

    public double ExchangeRate
    {
        get { return _ExchangeRate; }
        set { _ExchangeRate = value; RaisePropertyChanged("ExchangeRate"); }
    }

    private string _BatchNo;
    public string BatchNo
    {
        get { return _BatchNo; }
        set { _BatchNo = value; RaisePropertyChanged("BatchNo"); }
    }


    private string _SizeCode;

    public string SizeCode
    {
        get { return _SizeCode; }
        set { _SizeCode = value; RaisePropertyChanged("SizeCode"); }
    }


    private string _itemType;

    public string ItemType
    {
        get { return _itemType; }
        set { _itemType = value; RaisePropertyChanged("ItemType"); }
    }

    private string _style;

    public string Style
    {
        get { return _style; }
        set { _style = value; RaisePropertyChanged("Style"); }
    }

    private double _costField;

    private int _iserialField;

    private double _qtyField;

    private int? _tblCurrencyField;

    private int? _tblItemField;

    private int? _tblRecInvHeaderField;

    private int? _tblSTaxField;

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

    private TblColor _tblColor1;

    public TblColor TblColor1
    {
        get { return _tblColor1; }
        set
        {
            _tblColor1 = value; RaisePropertyChanged("TblColor1");
            if (TblColor1 != null) TblColor = TblColor1.Iserial;
        }
    }

    private int? _tblColor;

    public int? TblColor
    {
        get
        {
            return _tblColor;
        }
        set
        {
            if ((_tblColor.Equals(value) != true))
            {
                _tblColor = value;
                RaisePropertyChanged("TblColor");
            }
        }
    }

    public double Cost
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

    private double _total;

    [ReadOnly(true)]
    public double Total
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

    private ObservableCollection<TblMarkupTranProdViewModel> _markUpTransList;

    public ObservableCollection<TblMarkupTranProdViewModel> MarkUpTransList
    {
        get { return _markUpTransList ?? (_markUpTransList = new ObservableCollection<TblMarkupTranProdViewModel>()); }
        set
        {
            _markUpTransList = value;
            RaisePropertyChanged("MarkUpTransList");
        }
    }

    [ReadOnly(true)]
    public double Qty
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

    private double _miscField;

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
}

public class TblRecInvDetailViewModel : PropertiesViewModelBase
{
    private double _ExchangeRate;

    public double ExchangeRate
    {
        get { return _ExchangeRate; }
        set { _ExchangeRate = value; RaisePropertyChanged("ExchangeRate"); }
    }

    private string _SizeCode;

    public string SizeCode
    {
        get { return _SizeCode; }
        set { _SizeCode = value; RaisePropertyChanged("SizeCode"); }
    }

    private string _BatchNo;

    public string BatchNo
    {
        get { return _BatchNo; }
        set { _BatchNo = value; RaisePropertyChanged("BatchNo"); }
    }
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

public class TblReciveHeaderProdViewModel : TblPurchaseReceiveHeader
{
    private bool _check;

    public bool Checked
    {
        get { return _check; }
        set { _check = value; RaisePropertyChanged("Checked"); }
    }
}

public class TblMarkupTranProdViewModel : PropertiesViewModelBase
{

        private double _ExchangeRate;

        public double ExchangeRate
        {
            get { return _ExchangeRate; }
            set { _ExchangeRate = value; RaisePropertyChanged("ExchangeRate"); }
        }

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


    private TBLsupplier _supplierPerRow;

    public TBLsupplier SupplierPerRow
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
                        TblSupplier = SupplierPerRow.Code;
                    }
                }
            }
        }
    }

    private string _tblSupplier;

    public string TblSupplier
    {
        get { return _tblSupplier; }
        set { _tblSupplier = value; RaisePropertyChanged("TblSupplier"); }
    }

    private int _iserialField;

    private double? _miscValueField;

    private int _tblMarkupField;

    private TblMarkupProd _tblMarkup1Field;

    private int? _tblcurrency;
    [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCurrency")]
    public int? TblCurrency
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

    private int _tblDyeingOrderInvoiceHeader;

    public int TblDyeingOrderInvoiceHeader
    {
        get { return _tblDyeingOrderInvoiceHeader; }
        set { _tblDyeingOrderInvoiceHeader = value; RaisePropertyChanged("TblDyeingOrderInvoiceHeader"); }
    }
    private int _TblSalesOrderRequestInvoiceHeader;

    public int TblSalesOrderRequestInvoiceHeader
    {
        get { return _TblSalesOrderRequestInvoiceHeader; }
        set { _TblSalesOrderRequestInvoiceHeader = value; RaisePropertyChanged("TblSalesOrderRequestInvoiceHeader"); }
    }

    private int _TblProductionOrderInvoiceHeader;

    public int TblProductionOrderInvoiceHeader
    {
        get { return _TblProductionOrderInvoiceHeader; }
        set { _TblProductionOrderInvoiceHeader = value; RaisePropertyChanged("TblProductionOrderInvoiceHeader"); }
    }



    private int _miscValueType;

    public int MiscValueType
    {
        get { return _miscValueType; }
        set { _miscValueType = value; RaisePropertyChanged("MiscValueType"); }
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
    public TblMarkupProd TblMarkupProd1
    {
        get { return _tblMarkup1Field; }
        set
        {
            _tblMarkup1Field = value; RaisePropertyChanged("TblMarkupProd1");
            if (TblMarkupProd1 != null) TblMarkupProd = TblMarkupProd1.Iserial;
        }
    }

    private GenericTable _currencyPerRow;

    public GenericTable CurrencyPerRow
    {
        get { return _currencyPerRow; }
        set
        {
            _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
            if (_currencyPerRow != null && _currencyPerRow.Iserial != 0)
                TblCurrency = CurrencyPerRow.Iserial;
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

    int _tblMarkupProdField;
    public int TblMarkupProd
    {
        get
        {
            return _tblMarkupProdField;
        }
        set
        {
            if ((_tblMarkupProdField.Equals(value) != true))
            {
                _tblMarkupProdField = value;
                RaisePropertyChanged("TblMarkupProd");
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

    private int _routeCardInvoiceHeader;
    public int RouteCardInvoiceHeader
    {
        get
        {
            return _routeCardInvoiceHeader;
        }
        set
        {
            if ((_routeCardInvoiceHeader.Equals(value) != true))
            {
                _routeCardInvoiceHeader = value;
                RaisePropertyChanged("RouteCardInvoiceHeader");
            }
        }
    }
}


    #endregion Models
}