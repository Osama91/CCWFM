using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblSeasonalMasterListViewModel : PropertiesViewModelBase
    {
        public TblSeasonalMasterListViewModel()
        {
            SeasonalMasterListDetail = new ObservableCollection<TblSeasonalMasterListDetail>();
            SeasonalMasterListDetail.CollectionChanged += SeasonalMasterListDetail_CollectionChanged;
        }

        private ObservableCollection<TblStoreIntialOrderViewModel> _tblStoreIntialOrders;

        public ObservableCollection<TblStoreIntialOrderViewModel> DetailList
        {
            get { return _tblStoreIntialOrders ?? (_tblStoreIntialOrders = new ObservableCollection<TblStoreIntialOrderViewModel>()); }
            set { _tblStoreIntialOrders = value; RaisePropertyChanged("DetailList"); }
        }

        private void SeasonalMasterListDetail_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSeasonalMasterListDetail item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSeasonalMasterListDetail item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "ProductionPerSize":
                    if (ManualCalculation)
                    {
                        foreach (var row in SeasonalMasterListDetail)
                        {
                            if (Qty > 0 && row.ProductionPerSize > 0)
                            {
                                var result = ((double)row.ProductionPerSize / Qty);
                                row.Ratio = Math.Round(result, 2) * 10;
                            }
                        }
                        Qty = SeasonalMasterListDetail.Sum(x => x.ProductionPerSize);
                    }
                    break;

                case "Ratio":

                    if (!ManualCalculation)
                    {
                        foreach (var row in SeasonalMasterListDetail)
                        {
                            if (Qty > 0 && row.Ratio > 0)
                            {
                                var sum = SeasonalMasterListDetail.Sum(x => x.Ratio);
                                row.ProductionPerSize = Convert.ToInt32(Math.Floor(Qty * (row.Ratio / sum)));
                            }
                            else if (row.Ratio == 0)
                            {
                                row.ProductionPerSize = 0;
                            }
                        }

                        if (Qty != SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                        {
                            if (Qty > 50)
                            {


                                var Largest = SeasonalMasterListDetail.OrderByDescending(w => w.Ratio).FirstOrDefault();
                                if (Qty < SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                                {
                                    Largest.ProductionPerSize = Largest.ProductionPerSize - (SeasonalMasterListDetail.Sum(x => x.ProductionPerSize) - Qty);
                                }
                                else if (Qty > SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                                {
                                    Largest.ProductionPerSize = Largest.ProductionPerSize + (Qty - SeasonalMasterListDetail.Sum(x => x.ProductionPerSize));

                                }
                            }
                            else
                            {
                                Qty = SeasonalMasterListDetail.Sum(x => x.ProductionPerSize);
                            }
                        }



                    }

                    break;
            }
        }

        private bool _canceled;

        public bool Canceled
        {
            get { return _canceled; }
            set { _canceled = value; RaisePropertyChanged("Canceled"); }
        }

        private bool _canEditDeliveryDate;

        public bool CanEditDeliveryDate
        {
            get { return _canEditDeliveryDate; }
            set { _canEditDeliveryDate = value; RaisePropertyChanged("CanEditDeliveryDate"); }
        }

        private DateTime _delivaryDateField;

        private int _iserialField;

        private int _qtyField;

        private string _SupplierColorCodeField;

        private int? _tblColorField;

        private int _tblSalesOrderColorThemeField;

        private int? _tblStyleField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDeliveryDate")]
        public DateTime DelivaryDate
        {
            get
            {
                return _delivaryDateField;
            }
            set
            {
                if ((_delivaryDateField.Equals(value) != true))
                {
                    _delivaryDateField = value;
                    RaisePropertyChanged("DelivaryDate");
                }
            }
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

        [Range(1, 999999999, ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqQty")]
        public int Qty
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

                    if (!ManualCalculation)
                    {
                        foreach (var row in SeasonalMasterListDetail)
                        {
                            if (Qty > 0 && row.Ratio > 0)
                            {
                                var sum = SeasonalMasterListDetail.Sum(x => x.Ratio);
                                if (row.ProductionPerSize != Convert.ToInt32(Qty * (row.Ratio / sum)))
                                {
                                    row.ProductionPerSize = Convert.ToInt32(Qty * (row.Ratio / sum));
                                }
                            }
                            else if (row.Ratio == 0)
                            {
                                row.ProductionPerSize = 0;
                            }
                        }
                        if (SeasonalMasterListDetail.Any())
                        {

                            if (Qty != SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                            {
                                if (Qty > 50)
                                {
                                    var Largest = SeasonalMasterListDetail.OrderByDescending(w => w.Ratio).FirstOrDefault();
                                    if (Qty < SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                                    {
                                        Largest.ProductionPerSize = Largest.ProductionPerSize - (SeasonalMasterListDetail.Sum(x => x.ProductionPerSize) - Qty);
                                    }
                                    else if (Qty > SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                                    {
                                        Largest.ProductionPerSize = Largest.ProductionPerSize + (Qty - SeasonalMasterListDetail.Sum(x => x.ProductionPerSize));

                                    }
                                }
                                else
                                {
                                    Qty = SeasonalMasterListDetail.Sum(x => x.ProductionPerSize);
                                }
                            }
                            //                     if (Qty != SeasonalMasterListDetail.Sum(x => x.ProductionPerSize))
                            //{
                            //	Qty = SeasonalMasterListDetail.Sum(x => x.ProductionPerSize);
                            //}
                        }
                    }
                }
            }
        }

        public string SupplierColorCode
        {
            get
            {
                return _SupplierColorCodeField;
            }
            set
            {

                _SupplierColorCodeField = value;
                RaisePropertyChanged("SupplierColorCode");

            }
        }

        public int? TblColor
        {
            get
            {
                return _tblColorField;
            }
            set
            {
                if ((_tblColorField.Equals(value) != true))
                {
                    _tblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }

        private TblColor _tblColor1Field;

        public TblColor ColorPerRow
        {
            get
            {
                return _tblColor1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblColor1Field, value) != true))
                {
                    _tblColor1Field = value;
                    RaisePropertyChanged("ColorPerRow");
                }
            }
        }

        private TblSalesOrderColorTheme _tblSalesOrderColorTheme1Field;

        public TblSalesOrderColorTheme ThemePerRow
        {
            get
            {
                return _tblSalesOrderColorTheme1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblSalesOrderColorTheme1Field, value) != true))
                {
                    _tblSalesOrderColorTheme1Field = value;
                    RaisePropertyChanged("ThemePerRow");
                    if (ThemePerRow != null)
                    {
                        if(ThemePerRow.DeliveryDate != null)
                          DelivaryDate = (DateTime)ThemePerRow.DeliveryDate;
                        TblSalesOrderColorTheme = ThemePerRow.Iserial;
                    }
                }
            }
        }

        private TblStyle _stylePerRow;

        private bool _lockedRow;

        public TblStyle StylePerRow
        {
            get { return _stylePerRow; }
            set
            {
                if ((ReferenceEquals(_stylePerRow, value) != true))
                {
                    _stylePerRow = value; RaisePropertyChanged("StylePerRow");
                    if (StylePerRow != null)
                    {
                        TblStyle = StylePerRow.Iserial;

                        //   var validTime = StylePerRow.TblLkpSeason1.TblSeasonTracks.FirstOrDefault(x => x.Brand == StylePerRow.Brand && x.TblSeasonTrackType1.Code == "SeasonalMasterList"
                        //     && x.TblLkpBrandSection == StylePerRow.TblLkpBrandSection);

                        //if (validTime != null)
                        //{
                        //    if (validTime.FromDate <= DateTime.Now && validTime.ToDate >= DateTime.Now)
                        //    {
                        //        LockedRow = false;
                        //    }
                        //    else
                        //    {
                        //        LockedRow = true;
                        //    }
                        //}
                        //else
                        //{
                        //    LockedRow = true;
                        //}
                    }
                }
            }
        }

        public bool LockedRow
        {
            get { return _lockedRow; }
            set
            {
                _lockedRow = value; RaisePropertyChanged("LockedRow");
            }
        }

        [Range(1, 999999999, ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqTheme")]
        public int TblSalesOrderColorTheme
        {
            get
            {
                return _tblSalesOrderColorThemeField;
            }
            set
            {
                if ((_tblSalesOrderColorThemeField.Equals(value) != true))
                {
                    _tblSalesOrderColorThemeField = value;
                    RaisePropertyChanged("TblSalesOrderColorTheme");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStyleCode")]
        public int? TblStyle
        {
            get
            {
                return _tblStyleField;
            }
            set
            {
                if ((_tblStyleField.Equals(value) != true))
                {
                    _tblStyleField = value;
                    RaisePropertyChanged("TblStyle");
                }
            }
        }

        private ObservableCollection<TblSeasonalMasterListDetail> _tblSalesOrderSizeRatiosField;

        public ObservableCollection<TblSeasonalMasterListDetail> SeasonalMasterListDetail
        {
            get
            {
                return _tblSalesOrderSizeRatiosField;
            }
            set
            {
                if ((ReferenceEquals(_tblSalesOrderSizeRatiosField, value) != true))
                {
                    _tblSalesOrderSizeRatiosField = value;
                    RaisePropertyChanged("SeasonalMasterListDetail");
                }
            }
        }

        private bool _manualCalculationField;

        public bool ManualCalculation
        {
            get
            {
                return _manualCalculationField;
            }
            set
            {
                if ((_manualCalculationField.Equals(value) != true))
                {
                    _manualCalculationField = value;
                    RaisePropertyChanged("ManualCalculation");
                }
            }
        }
    }

    public class TblStoreIntialOrderViewModel : PropertiesViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _style;

        public string Style
        {
            get { return _style; }
            set { _style = value; RaisePropertyChanged("Style"); }
        }

        private string _color;

        public string Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChanged("Color"); }
        }

        private int? _store;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStore")]
        public int? Store
        {
            get { return _store; }
            set { _store = value; RaisePropertyChanged("Store"); }
        }

        private int _qty;

        public int Qty
        {
            get { return _qty; }
            set { _qty = value; RaisePropertyChanged("Qty"); }
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow; }
            set
            {
                _storePerRow = value; RaisePropertyChanged("StorePerRow");
                if (StorePerRow != null) Store = StorePerRow.iserial;
            }
        }
    }

    public class SeasonalMasterListViewModel : ViewModelBase
    {
        public SeasonalMasterListViewModel(StyleHeaderViewModel selectedStyle)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.SeasonalMasterListForm.ToString());
                SelectedStyleViewModel = selectedStyle;
                MainRowList = new ObservableCollection<TblSeasonalMasterListViewModel>();
                SelectedMainRow = new TblSeasonalMasterListViewModel();
                MainRowList.CollectionChanged += MainRowList_CollectionChanged;

                if (LoggedUserInfo.AllowedStores != null)
                    Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, null, null, LoggedUserInfo.DatabasEname);

                Client.SearchBysStoreNameCompleted += (s, sv) =>
                {
                    StoreList = sv.Result;
                };
                Client.GetSmlCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSeasonalMasterListViewModel();

                        newrow.CanEditDeliveryDate = CanChangeDeliveryDate;

                        newrow.InjectFrom(row);
                        newrow.StylePerRow = row.TblStyle1;
                        newrow.ThemePerRow =
                            SelectedStyleViewModel.ThemesList.SingleOrDefault(
                                x => x.Iserial == row.TblSalesOrderColorTheme);
                        newrow.ColorPerRow =
                            SelectedStyleViewModel.ColorList.SingleOrDefault(
                                x => x.Iserial == row.TblColor);
                        var sizeRatioList = new ObservableCollection<TblSeasonalMasterListDetail>();
                        foreach (var sizeCode in SelectedStyleViewModel.Sizes.OrderBy(x => x.Id))
                        {
                            sizeRatioList.Add(new TblSeasonalMasterListDetail
                            {
                                Size = sizeCode.SizeCode,
                            });
                        }
                        //Added By  Hashem to Map with the NEW Stitch System Ratios with Sizes 

                        ObservableCollection<TblSeasonalMasterListDetail> data = new ObservableCollection<TblSeasonalMasterListDetail>();

                        if(row.TblSeasonalMasterListDetails.Count() != 0 )
                        {
                            //foreach (var r in row.TblSeasonalMasterListDetails)
                            //{
                                foreach (var item in SelectedStyleViewModel.Sizes.OrderBy(x => x.Id))
                                {
                                    TblSeasonalMasterListDetail d = new TblSeasonalMasterListDetail();
                                    d = row.TblSeasonalMasterListDetails.Where(x => x.Size == item.SizeCode).FirstOrDefault();
                                    data.Add(d);
                                }
                            //}
                           
                        }


                        //Commented By Hashem  to Map with the NEW Stitch System Ratios with Sizes 
                        //GenericMapper.InjectFromObCollection(newrow.SeasonalMasterListDetail,
                        //row.TblSeasonalMasterListDetails.Count() != 0 ? row.TblSeasonalMasterListDetails : sizeRatioList);

                        GenericMapper.InjectFromObCollection(newrow.SeasonalMasterListDetail,
                       row.TblSeasonalMasterListDetails.Count() != 0 ? data : sizeRatioList);
                        try
                        {
                            newrow.DelivaryDate = row.DelivaryDate.Value;
                        }
                        catch { }
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.GetTblStoreIntialOrderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStoreIntialOrderViewModel();

                        newrow.InjectFrom(row);
                        newrow.StorePerRow = sv.Stores.FirstOrDefault(x => x.iserial == row.Store);
                        SelectedMainRow.DetailList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Client.UpdateOrInsertTblSmlCompleted += (s, x) =>
                {
                    if (x.Error != null)
                    {
                        MessageBox.Show(x.Error.Message);
                    }
                    var savedRow = (TblSeasonalMasterListViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);


                        foreach (var item in x.Result.TblSeasonalMasterListDetails)
                        {
                            var newsizeRow = savedRow.SeasonalMasterListDetail.FirstOrDefault(w => w.Size == item.Size);
                            if (newsizeRow != null)
                            {
                                newsizeRow.Iserial = item.Iserial;
                            }

                        }
                    }
                    Loading = false;
                    if (savedRow.Iserial == 0)
                    {
                        var amountAva = x.AmountAvaliable;
                        var amountExc = x.AmountExceeded;
                        var qtyAvaliable = x.QtyAvaliable;
                        var qtyExc = x.QtyExceeded;

                        MessageBox.Show("Brand Budget Is Not Enough For The current Amounts Or Quantities, Amount Avaliable= " + amountAva + " and  Amount Exceeded= " + amountExc + "Qty Avaliable= " + qtyAvaliable + " and  Qty Exceeded= " + qtyExc);
                    }
                };

                Client.UpdateOrInsertTblStoreIntialOrderCompleted += (s, x) =>
                {
                    var savedRow = SelectedMainRow.DetailList.ElementAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };
                Client.DeleteSmlCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    Loading = false;
                };
                Client.DeleteTblStoreIntialOrderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailList.FirstOrDefault(x => x.Style == ev.Result.Style && x.Color == ev.Result.Color && x.Store == ev.Result.Store);
                    if (oldrow != null) SelectedMainRow.DetailList.Remove(oldrow);
                    Loading = false;
                };
                GetMaindata();
            }
        }

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblSeasonalMasterListViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblSeasonalMasterListViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "LockedRow":

                    if (MainRowList.Count(x => x.LockedRow && x.Iserial == 0) > 0)
                    {
                        //     MainRowList.Remove(SelectedMainRow);
                    }

                    break;
            }
        }

        public void GetMaindata()
        {
            //CheckEnable DeliveryDate
            LkpData.LkpDataClient _lkClient = new LkpData.LkpDataClient();
            _lkClient.CanChangeDeliveryDateAsync(SelectedStyleViewModel.SelectedMainRow.Iserial);
            _lkClient.CanChangeDeliveryDateCompleted += (s, sv) => {
                CanChangeDeliveryDate = sv.Result;

                if (SortBy == null)
                    SortBy = "it.Iserial";
                Loading = true;

                Client.GetSmlAsync(MainRowList.Count, int.MaxValue, SelectedStyleViewModel.SelectedMainRow.Iserial, SortBy, Filter, ValuesObjects);
            };
        }

        public void GetDetaildata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;

            Client.GetTblStoreIntialOrderAsync(SelectedMainRow.DetailList.Count, int.MaxValue, SelectedStyleViewModel.SelectedMainRow.StyleCode, SelectedMainRow.ColorPerRow.Code, SortBy, DetailFilter, DetailValuesObjects);
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Client.DeleteSmlAsync(
                                (TblSeasonalMasterList)new TblSeasonalMasterList().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        if (AllowDelete != true)
                        {
                            MessageBox.Show(strings.AllowDeleteMsg);
                            return;
                        }
                        Loading = true;
                        Client.DeleteTblStoreIntialOrderAsync((TblStoreIntialOrder)new TblStoreIntialOrder().InjectFrom(row), SelectedMainRow.DetailList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }

            var sizeRatioList = new ObservableCollection<TblSeasonalMasterListDetail>();
            foreach (var sizeCode in SelectedStyleViewModel.Sizes.OrderBy(x => x.Id))
            {
                sizeRatioList.Add(new TblSeasonalMasterListDetail
                {
                    Size = sizeCode.SizeCode,
                });
            }

            var style = new TblStyle();
            style.InjectFrom(SelectedStyleViewModel.SelectedMainRow);

            var prevRow = new TblSeasonalMasterListViewModel();

            if (currentRowIndex != -1)
            {
                prevRow = MainRowList.ElementAt(currentRowIndex);
            }

            var newrow = new TblSeasonalMasterListViewModel
            {
                TblStyle = SelectedStyleViewModel.SelectedMainRow.Iserial,
                StylePerRow = style,
            };
            GenericMapper.InjectFromObCollection(newrow.SeasonalMasterListDetail,
                prevRow.SeasonalMasterListDetail.Count() != 0 ? prevRow.SeasonalMasterListDetail : sizeRatioList);

            newrow.ThemePerRow = prevRow.ThemePerRow;

            foreach (var size in newrow.SeasonalMasterListDetail)
            {
                size.ProductionPerSize = 0;
            }

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.DetailList.Insert(currentRowIndex + 1, new TblStoreIntialOrderViewModel()
                {
                    Style = SelectedStyleViewModel.SelectedMainRow.StyleCode,
                    Color = SelectedMainRow.ColorPerRow.Code
                });
            }
        }

        public void SaveMainRow()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid && !Loading)//&& !SelectedMainRow.LockedRow)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblSeasonalMasterList();
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.TblSeasonalMasterListDetails = SelectedMainRow.SeasonalMasterListDetail;
                    Loading = true;
                    Client.UpdateOrInsertTblSmlAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void SaveDetailRow()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid && !Loading)//&& !SelectedMainRow.LockedRow)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    var saveRow = new TblStoreIntialOrder();
                    saveRow.InjectFrom(SelectedDetailRow);

                    Loading = true;
                    Client.UpdateOrInsertTblStoreIntialOrderAsync(saveRow, save, SelectedMainRow.DetailList.IndexOf(SelectedDetailRow));
                }
            }
        }

        public void SaveMainList()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            foreach (var row in MainRowList)
            {
                if (row != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(row, new ValidationContext(row, null, null),
                        valiationCollection, true);

                    if (isvalid && !Loading) //&& !SelectedMainRow.LockedRow)
                    {
                        var save = row.Iserial == 0;
                        var saveRow = new TblSeasonalMasterList();
                        saveRow.InjectFrom(row);
                        saveRow.TblSeasonalMasterListDetails = row.SeasonalMasterListDetail;
                        if (row == MainRowList.LastOrDefault())
                        {
                            Loading = true;
                        }
                        Client.UpdateOrInsertTblSmlAsync(saveRow, save, MainRowList.IndexOf(row));
                    }
                }
            }
        }

        #region Prop

        //        public event EventHandler StyleCompletedCompleted;

        private bool _canChangeDeliveryDate;

        public bool CanChangeDeliveryDate
        {
            get { return _canChangeDeliveryDate; }
            set { _canChangeDeliveryDate = value; RaisePropertyChanged("CanChangeDeliveryDate"); }
        }

        private
        ObservableCollection<TblSeasonalMasterListViewModel> _mainRowList;

        public ObservableCollection<TblSeasonalMasterListViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSeasonalMasterListViewModel> _selectedMainRows;

        public ObservableCollection<TblSeasonalMasterListViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSeasonalMasterListViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<TblStoreIntialOrderViewModel> _selectedDetailRows;

        public ObservableCollection<TblStoreIntialOrderViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblStoreIntialOrderViewModel>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private StyleHeaderViewModel _selectedStyleViewModel;

        public StyleHeaderViewModel SelectedStyleViewModel
        {
            get { return _selectedStyleViewModel; }
            set { _selectedStyleViewModel = value; RaisePropertyChanged("SelectedStyleViewModel"); }
        }

        private TblSeasonalMasterListViewModel _selectedMainRow;

        public TblSeasonalMasterListViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblStore> _storeList;

        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set
            {
                _storeList = value;
                RaisePropertyChanged("StoreList");
            }
        }

        private TblStoreIntialOrderViewModel _selectedDetailRow;

        public TblStoreIntialOrderViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop

        public void PreviewReport()
        {
            const string reportName = "SeasonMasterList";

            var reportViewmodel = new GenericReportViewModel();
            var para = new ObservableCollection<string>
            {
                LoggedUserInfo.Iserial.ToString(),
                SelectedStyleViewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture)
            };

            reportViewmodel.GenerateReport(reportName, para);
        }

        public void CalcRatio()
        {
            if (AllowUpdate != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            foreach (var tblSeasonalMasterListViewModel in MainRowList)
            {
                foreach (var tblSeasonalMasterListDetail in tblSeasonalMasterListViewModel.SeasonalMasterListDetail)
                {
                    tblSeasonalMasterListDetail.Ratio =
                        SelectedMainRow
                            .SeasonalMasterListDetail.FirstOrDefault(
                                x => x.Size == tblSeasonalMasterListDetail.Size)
                            .Ratio;
                }
            }

            SaveMainList();
        }
    }
}