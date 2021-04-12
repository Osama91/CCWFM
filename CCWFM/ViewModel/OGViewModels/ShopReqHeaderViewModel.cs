using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using GenericTable = CCWFM.GlService.GenericTable;
using TblShopAcc = CCWFM.CRUDManagerService.TblShopAcc;
using TblShopComment = CCWFM.CRUDManagerService.TblShopComment;
using TblShopReqAcc = CCWFM.CRUDManagerService.TblShopReqAcc;
using TblShopReqComment = CCWFM.CRUDManagerService.TblShopReqComment;
using TblShopReqHeader = CCWFM.CRUDManagerService.TblShopReqHeader;
using TblShopReqInv = CCWFM.CRUDManagerService.TblShopReqInv;
using TblStore = CCWFM.CRUDManagerService.TblStore;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblShopReqHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private DateTime _creationDateField;

        private int _tblItemDownLoadDefField;

        private ObservableCollection<TblShopReqAcc> _tblShopReqAccsField;

        private ObservableCollection<TblShopReqComment> _tblShopReqCommentsField;

        private int _tblStoreField;

        private int _userIserialField;

        private int _weekField;

        private int _yearField;

        public DateTime CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public int TblItemDownLoadDef
        {
            get
            {
                return _tblItemDownLoadDefField;
            }
            set
            {
                if ((_tblItemDownLoadDefField.Equals(value) != true))
                {
                    _tblItemDownLoadDefField = value;
                    RaisePropertyChanged("TblItemDownLoadDef");
                }
            }
        }

        public ObservableCollection<TblShopReqAcc> TblShopReqAccs
        {
            get
            {
                return _tblShopReqAccsField ?? (_tblShopReqAccsField = new ObservableCollection<TblShopReqAcc>());
            }
            set
            {
                if ((ReferenceEquals(_tblShopReqAccsField, value) != true))
                {
                    _tblShopReqAccsField = value;
                    RaisePropertyChanged("TblShopReqAccs");
                }
            }
        }

        public ObservableCollection<TblShopReqComment> TblShopReqComments
        {
            get
            {
                return _tblShopReqCommentsField ?? (_tblShopReqCommentsField = new ObservableCollection<TblShopReqComment>());
            }
            set
            {
                if ((ReferenceEquals(_tblShopReqCommentsField, value) != true))
                {
                    _tblShopReqCommentsField = value;
                    RaisePropertyChanged("TblShopReqComments");
                }
            }
        }

        private ObservableCollection<TblShopReqInvViewModel> _tblShopReqInvsField;

        public ObservableCollection<TblShopReqInvViewModel> TblShopReqInvs
        {
            get
            {
                return _tblShopReqInvsField ?? (_tblShopReqInvsField = new ObservableCollection<TblShopReqInvViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblShopReqInvsField, value) != true))
                {
                    _tblShopReqInvsField = value;
                    RaisePropertyChanged("TblShopReqInvs");
                }
            }
        }

        public int TblStore
        {
            get
            {
                return _tblStoreField;
            }
            set
            {
                if ((_tblStoreField.Equals(value) != true))
                {
                    _tblStoreField = value;
                    RaisePropertyChanged("TblStore");
                }
            }
        }

        public int UserIserial
        {
            get
            {
                return _userIserialField;
            }
            set
            {
                if ((_userIserialField.Equals(value) != true))
                {
                    _userIserialField = value;
                    RaisePropertyChanged("UserIserial");
                }
            }
        }

        public int Week
        {
            get
            {
                return _weekField;
            }
            set
            {
                if ((_weekField.Equals(value) != true))
                {
                    _weekField = value;
                    RaisePropertyChanged("Week");
                }
            }
        }

        public int Year
        {
            get
            {
                return _yearField;
            }
            set
            {
                if ((_yearField.Equals(value) != true))
                {
                    _yearField = value;
                    RaisePropertyChanged("Year");
                }
            }
        }
    }

    public class TblShopReqInvViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private readonly GlServiceClient _shopAccClient = new GlServiceClient();

        public TblShopReqInvViewModel()
        {
            _shopAccClient.GetTblItemPricebyCodeCompleted += (s, sv) =>
            {
                Colors = sv.Result;
                Sizes = sv.sizeIserials;
            };
        }

        private double _qtyField;

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
                }
            }
        }

        private ObservableCollection<GlService.TblColorTest> _colors;

        public ObservableCollection<GlService.TblColorTest> Colors
        {
            get { return _colors; }
            set { _colors = value; RaisePropertyChanged("Colors"); }
        }

        private ObservableCollection<GlService.TblSizeRetail> _sizes;

        public ObservableCollection<GlService.TblSizeRetail> Sizes
        {
            get { return _sizes; }
            set { _sizes = value; RaisePropertyChanged("Sizes"); }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _tbliteMpriceField;

        private viewstyle _tbliteMprice1Field;

        private int? _tblColorField;

        private GlService.TblColorTest _tblColor1Field;

        private int _tblShopReqHeaderField;

        private int? _tblSizeField;

        private GlService.TblSizeRetail _tblSize1Field;

        [Required]
        public string TBLITEMprice
        {
            get
            {
                return _tbliteMpriceField;
            }
            set
            {
                _tbliteMpriceField = value;
                _shopAccClient.GetTblItemPricebyCodeAsync(_tbliteMpriceField, LoggedUserInfo.DatabasEname);
                RaisePropertyChanged("TBLITEMprice");
            }
        }

        public viewstyle SearchPerRow
        {
            get
            {
                return _tbliteMprice1Field;
            }
            set
            {
                if ((ReferenceEquals(_tbliteMprice1Field, value) != true))
                {
                    _tbliteMprice1Field = value;
                    TBLITEMprice = SearchPerRow.Code;
                    RaisePropertyChanged("SearchPerRow");
                }
            }
        }

        [Required]
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

        public GlService.TblColorTest TblColor1
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
                    if (TblColor1 != null) TblColor = TblColor1.ISERIAL;
                    RaisePropertyChanged("TblColor1");
                }
            }
        }

        public int TblShopReqHeader
        {
            get
            {
                return _tblShopReqHeaderField;
            }
            set
            {
                if ((_tblShopReqHeaderField.Equals(value) != true))
                {
                    _tblShopReqHeaderField = value;
                    RaisePropertyChanged("TblShopReqHeader");
                }
            }
        }

        [Required]
        public int? TblSize
        {
            get
            {
                return _tblSizeField;
            }
            set
            {
                if ((_tblSizeField.Equals(value) != true))
                {
                    _tblSizeField = value;
                    RaisePropertyChanged("TblSize");
                }
            }
        }

        public GlService.TblSizeRetail TblSize1
        {
            get
            {
                return _tblSize1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblSize1Field, value) != true))
                {
                    _tblSize1Field = value;
                    TblSize = TblSize1.Iserial;
                    RaisePropertyChanged("TblSize1");
                }
            }
        }
    }

    public class ShopReqHeaderViewModel : ViewModelBase
    {
        public ShopReqHeaderViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Year = DateTime.Now.Year;
                Week = GetIso8601WeekOfYear(DateTime.Now);
                MainRowList = new SortableCollectionView<TblShopReqHeaderViewModel>();
                SelectedMainRow = new TblShopReqHeaderViewModel();
                var shopAccClient = new GlServiceClient();
                shopAccClient.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        SelectedMainRow.TblShopReqAccs.Add(new TblShopReqAcc
                        {
                            TblShopAcc = variable.Iserial,
                            TblShopAcc1 = new TblShopAcc
                            {
                                Iserial = variable.Iserial,
                                Code = variable.Code,
                                Ename = variable.Ename,
                                Aname = variable.Aname,
                            }
                        });
                    }
                };
                shopAccClient.GetGenericAsync("TblShopAcc", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var tblItemDownLoadDefClient = new GlServiceClient();
                tblItemDownLoadDefClient.GetGenericCompleted += (s, sv) =>
                {
                    TblItemDownLoadDefList = sv.Result;
                };
                tblItemDownLoadDefClient.GetGenericAsync("TblItemDownLoadDef", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                SelectedMainRow.TblShopReqComments = new ObservableCollection<TblShopReqComment>();
                var shopCommentsClient = new GlServiceClient();
                shopCommentsClient.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        SelectedMainRow.TblShopReqComments.Add(new TblShopReqComment
                        {
                            TblShopComments = variable.Iserial,
                            TblShopComment = new TblShopComment
                            {
                                Iserial = variable.Iserial,
                                Code = variable.Code,
                                Ename = variable.Ename,
                                Aname = variable.Aname,
                            }
                        });
                    }
                };
                shopCommentsClient.GetGenericAsync("TblShopComments", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                Client.GetTblShopReqHeaderCompleted += (s, sv) =>
                {
                    SelectedMainRow.Iserial = 0;
                    if (sv.Result != null)
                    {
                        if (sv.Result.Any())
                        {
                            SelectedMainRow.Iserial = sv.Result.FirstOrDefault().Iserial;
                            SelectedMainRow.TblStore = sv.Result.FirstOrDefault().TblStore;
                            SelectedMainRow.TblItemDownLoadDef = sv.Result.FirstOrDefault().TblItemDownLoadDef;
                            SelectedMainRow.Year = sv.Result.FirstOrDefault().Year;
                            SelectedMainRow.Week = sv.Result.FirstOrDefault().Week;
                        }
                    }
                    GetDetailData(SelectedMainRow.Iserial);
                    Loading = false;
                };

                Client.GetTblShopReqAccCompleted += (s, sv) =>
                {
                    foreach (var variable in SelectedMainRow.TblShopReqAccs)
                    {
                        variable.Qty = 0;
                    }
                    foreach (var variable in sv.Result)
                    {
                        SelectedMainRow.TblShopReqAccs.FirstOrDefault(x => x.TblShopAcc == variable.TblShopAcc).Qty = variable.Qty;
                    }
                };

                Client.GetTblShopReqCommentsCompleted += (s, sv) =>
                {
                    foreach (var variable in SelectedMainRow.TblShopReqComments)
                    {
                        variable.Comments = "";
                    }
                    foreach (var variable in sv.Result)
                    {
                        SelectedMainRow.TblShopReqComments.FirstOrDefault(x => x.TblShopComments == variable.TblShopComments).Comments = variable.Comments;
                    }
                };

                Client.GetTblShopReqInvCompleted += (s, sv) =>
                {
                    SelectedMainRow.TblShopReqInvs.Clear();
                    foreach (var variable in sv.Result)
                    {
                        var newrow = new TblShopReqInvViewModel
                        {
                            Qty = variable.Qty,
                            Iserial = variable.Iserial,
                            TBLITEMprice = variable.TBLITEMprice,
                            SearchPerRow = new viewstyle()
                            {
                                Code = variable.TBLITEMprice,
                            },
                            TblColor = variable.TblColor,
                            TblSize = variable.TblSize,
                            TblColor1 = new GlService.TblColorTest()
                            {
                                ISERIAL = variable.TblColor1.ISERIAL,
                                Code = variable.TblColor1.Code,
                                Aname = variable.TblColor1.Aname,
                                Ename = variable.TblColor1.Ename,
                            },

                            TblSize1 = new GlService.TblSizeRetail()
                            {
                                Iserial = variable.TblSize1.Iserial,

                                Code = variable.TblSize1.Code,
                                Aname = variable.TblSize1.Aname,
                                Ename = variable.TblSize1.Ename,
                            }
                        };

                        SelectedMainRow.TblShopReqInvs.Add(newrow);
                    }
                    if (!sv.Result.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrDeleteTblShopReqInvCompleted += (s, sv) =>
                {
                    var savedRow = SelectedMainRow.TblShopReqInvs.ElementAt(sv.outindex);

                    if (savedRow != null)
                    {
                        savedRow.Iserial = sv.Result.Iserial;
                    }
                };

                if (LoggedUserInfo.Store != null)
                {
                    StorePerRow =  new TblStore().InjectFrom(LoggedUserInfo.Store) as TblStore;
                }
                StoreList = new SortableCollectionView<TblStore>();

                if (LoggedUserInfo.AllowedStores != null && LoggedUserInfo.Company.Code != "HQ")
                    Client.SearchBysStoreNameAsync(new ObservableCollection<int>(LoggedUserInfo.AllowedStores), LoggedUserInfo.Iserial, null, null, LoggedUserInfo.DatabasEname);

                Client.SearchBysStoreNameCompleted += (s, sv) =>
                {
                    StoreList = sv.Result;
                };

                GetMaindata();
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblShopReqInvs.IndexOf(SelectedShopReqInvRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblShopReqInvs.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.TblShopReqInvs.Insert(currentRowIndex + 1, new TblShopReqInvViewModel());
            }
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            if (TblStore != 0 && TblItemDownLoadDef != 0)
            {
                Loading = true;
                Client.GetTblShopReqHeaderAsync(TblStore, TblItemDownLoadDef, Week, Year, LoggedUserInfo.DatabasEname);
            }
        }

        public void GetDetailData(int headerIserial)
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            if (TblStore != 0 && TblItemDownLoadDef != 0)
            {
                Loading = true;
                Client.GetTblShopReqAccAsync(headerIserial, LoggedUserInfo.DatabasEname);
                Client.GetTblShopReqCommentsAsync(headerIserial, LoggedUserInfo.DatabasEname);
                Client.GetTblShopReqInvAsync(headerIserial, LoggedUserInfo.DatabasEname);
            }
        }

        public void SaveMainRow()
        {
            if (SelectedShopReqAccRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedShopReqAccRow, new ValidationContext(SelectedShopReqAccRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedShopReqAccRow.Iserial == 0;

                    var header = new TblShopReqHeader
                    {
                        Week = Week,
                        TblItemDownLoadDef = TblItemDownLoadDef,
                        TblStore = TblStore,
                        Year = Year,
                    };

                    var saveRow = new TblShopReqAcc();
                    saveRow.InjectFrom(SelectedShopReqAccRow);
                    saveRow.TblShopReqHeader1 = header;
                    saveRow.TblShopAcc1 = null;
                    saveRow.TblShopAcc = SelectedShopReqAccRow.TblShopAcc;
                    Client.UpdateOrDeleteTblShopReqAccAsync(saveRow, save, 0, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
                }
            }
        }

        public void SaveComment()
        {
            if (SelectedShopReqCommentRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedShopReqCommentRow, new ValidationContext(SelectedShopReqCommentRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var header = new TblShopReqHeader
                    {
                        Week = Week,
                        TblItemDownLoadDef = TblItemDownLoadDef,
                        TblStore = TblStore,
                        Year = Year,
                    };

                    var save = SelectedShopReqCommentRow.Iserial == 0;
                    var saveRow = new TblShopReqComment();
                    saveRow.InjectFrom(SelectedShopReqCommentRow);
                    saveRow.TblShopComment = null;
                    saveRow.TblShopReqHeader1 = header;

                    Client.UpdateOrDeleteTblShopReqCommentsAsync(saveRow, save, 0, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
                }
            }
        }

        public void SaveWarehouse()
        {
            if (SelectedShopReqInvRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedShopReqInvRow, new ValidationContext(SelectedShopReqInvRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var header = new TblShopReqHeader
                    {
                        Week = Week,
                        TblItemDownLoadDef = TblItemDownLoadDef,
                        TblStore = TblStore,
                        Year = Year,
                    };

                    var save = SelectedShopReqInvRow.Iserial == 0;
                    var saveRow = new TblShopReqInv
                    {
                        Qty = SelectedShopReqInvRow.Qty,
                        Iserial = SelectedShopReqInvRow.Iserial,
                        TBLITEMprice = SelectedShopReqInvRow.TBLITEMprice,
                        TblColor = (int)SelectedShopReqInvRow.TblColor,
                        TblSize = (int)SelectedShopReqInvRow.TblSize,
                        TblShopReqHeader1 = header
                    };

                    Client.UpdateOrDeleteTblShopReqInvAsync(saveRow, save, SelectedMainRow.TblShopReqInvs.IndexOf(SelectedShopReqInvRow), LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblShopReqHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblShopReqHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblShopReqInvViewModel> _selectedMainRows;

        public ObservableCollection<TblShopReqInvViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblShopReqInvViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblStore _storePerRow;

        public TblStore StorePerRow
        {
            get { return _storePerRow ?? (_storePerRow = new TblStore()); }
            set
            {
                _storePerRow = value;
                RaisePropertyChanged("StorePerRow");
                if (StorePerRow != null)
                {
                    TblStore = StorePerRow.iserial;
                }
            }
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

        private int _tblStore;

        public int TblStore
        {
            get { return _tblStore; }
            set { _tblStore = value; RaisePropertyChanged("TblStore"); }
        }

        private int _tblItemDownLoadDef;

        public int TblItemDownLoadDef
        {
            get { return _tblItemDownLoadDef; }
            set { _tblItemDownLoadDef = value; RaisePropertyChanged("TblItemDownLoadDef"); }
        }

        private int _week;

        public int Week
        {
            get { return _week; }
            set { _week = value; RaisePropertyChanged("Week"); }
        }

        private int _year;

        public int Year
        {
            get { return _year; }
            set { _year = value; RaisePropertyChanged("Year"); }
        }

        private TblShopReqHeaderViewModel _selectedMainRow;

        public TblShopReqHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblShopReqHeaderViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblShopReqAcc _selectedShopReqAccRow;

        public TblShopReqAcc SelectedShopReqAccRow
        {
            get { return _selectedShopReqAccRow; }
            set { _selectedShopReqAccRow = value; RaisePropertyChanged("SelectedShopReqAccRow"); }
        }

        private TblShopReqComment _selectedShopReqCommentRow;

        public TblShopReqComment SelectedShopReqCommentRow
        {
            get { return _selectedShopReqCommentRow; }
            set { _selectedShopReqCommentRow = value; RaisePropertyChanged("SelectedShopReqCommentRow"); }
        }

        private TblShopReqInvViewModel _selectedShopReqInvRow;

        public TblShopReqInvViewModel SelectedShopReqInvRow
        {
            get { return _selectedShopReqInvRow; }
            set { _selectedShopReqInvRow = value; RaisePropertyChanged("SelectedShopReqInvRow"); }
        }

        private ObservableCollection<GenericTable> _tblItemDownLoadDefList;

        public ObservableCollection<GenericTable> TblItemDownLoadDefList
        {
            get { return _tblItemDownLoadDefList; }
            set { _tblItemDownLoadDefList = value; RaisePropertyChanged("TblItemDownLoadDefList"); }
        }

        private CCWFM.CRUDManagerService.GenericTable _brand;

        public CCWFM.CRUDManagerService.GenericTable Brand
        {
            get { return _brand; }
            set { _brand = value;
                RaisePropertyChanged("Brand");
            }
        }
        
        #endregion Prop

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
                            Client.DeleteTblShopReqInvAsync(
                                row.Iserial, LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.TblShopReqInvs.Remove(row);
                        }
                    }
                }
            }
        }
    }
}