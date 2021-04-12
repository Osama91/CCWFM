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

namespace CCWFM.ViewModel.Gl
{
    public class IncomeStatmentDesignViewModel : ViewModelBase
    {
        public IncomeStatmentDesignViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.IncomeStatmentDesign.ToString());
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblIncomeStatmentDesignHeaderViewModel>();
                SelectedMainRow = new TblIncomeStatmentDesignHeaderViewModel();
                JournalAccountTypeList = new ObservableCollection<GenericTable>();
                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Label"
                });

                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Account"
                });

                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Account With Subs"
                });

                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Cost Of Good Sold"
                });

                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Income Tax"
                });
                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Seprator"
                });

                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Double Seprator"
                });
                JournalAccountTypeList.Add(new GenericTable
                {
                    Code = "Total"
                });

                Glclient.GetTblIncomeStatmentDesignHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblIncomeStatmentDesignHeaderViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Count == 1)
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                    if (Export)
                    {
                        Export = false;
                        AllowExport = true;
                        //ExportGrid.ExportExcel("IncomeStatmentDesign");
                    }
                };

                Glclient.UpdateOrInsertTblIncomeStatmentDesignHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblIncomeStatmentDesignHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.GetTblIncomeStatmentDesignDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblIncomeStatmentDesignDetailViewModel();

                        newrow.InjectFrom(row);
                        if (row.Type.StartsWith("Account"))
                        {
                            newrow.Type = row.Type;
                            newrow.AccountPerRow = new TblAccount { Code = row.Description };
                        }
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;

                    if (SelectedMainRow.DetailsList.Count == 1)
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }

                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.UpdateOrInsertIncomeStatmentDesignDetailCompleted += (s, x) =>
                {
                    var savedRow = (TblIncomeStatmentDesignDetailViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    Loading = false;
                };

                Glclient.DeleteIncomeStatmentDesignDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    Loading = false;
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                GetMaindata();
            }
        }

        private void MainRowList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                MainRowList.Clear();
                SortBy = null;
                foreach (var sortDesc in MainRowList.SortDescriptions)
                {
                    SortBy = SortBy + "it." + sortDesc.PropertyName +
                             (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblIncomeStatmentDesignHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetTblIncomeStatmentDesignHeaderAsync(MainRowList.Count, int.MaxValue, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblIncomeStatmentDesignHeaderViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
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
                    var saveRow = new TblIncomeStatmentDesignHeader();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblIncomeStatmentDesignHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                                              LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblIncomeStatmentDesignHeaderViewModel oldRow)
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
                    var saveRow = new TblIncomeStatmentDesignHeader();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblIncomeStatmentDesignHeaderAsync(saveRow, save, MainRowList.IndexOf(oldRow),
                                              LoggedUserInfo.DatabasEname);
                    }
                }
            }
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
                            Glclient.DeleteTblIncomeStatmentDesignHeaderAsync((TblIncomeStatmentDesignHeader)new TblIncomeStatmentDesignHeader().InjectFrom(row),
                                 LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
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
                Glclient.GetTblIncomeStatmentDesignDetailAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailsValuesObjects, LoggedUserInfo.DatabasEname);
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
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Loading = true;
                            Glclient.DeleteIncomeStatmentDesignDetailAsync(
                                (TblIncomeStatmentDesignDetail)new TblIncomeStatmentDesignDetail().InjectFrom(row),
                       LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(SelectedDetailRow);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

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

            var newrow = new TblIncomeStatmentDesignDetailViewModel { TblIncomeStatmentDesignHeader = SelectedMainRow.Iserial };

            if (SelectedMainRow.DetailsList.Any())
            {
                newrow.RowOrder =
                    SelectedMainRow.DetailsList.OrderByDescending(x => x.RowOrder).FirstOrDefault().RowOrder + 1;
            }
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                    new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (SelectedDetailRow.Type.StartsWith("Account") && string.IsNullOrWhiteSpace(SelectedDetailRow.Description))
                {
                    isvalid = false;
                }
                if (isvalid)
                {
                    var save = SelectedDetailRow.Iserial == 0;
                    if (SelectedDetailRow.TblIncomeStatmentDesignHeader == 0)
                    {
                        SelectedDetailRow.TblIncomeStatmentDesignHeader = SelectedMainRow.Iserial;
                    }
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var rowToSave = new TblIncomeStatmentDesignDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);

                    Glclient.UpdateOrInsertIncomeStatmentDesignDetailAsync(rowToSave, save,
                        SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblIncomeStatmentDesignHeaderViewModel> _mainRowList;

        public SortableCollectionView<TblIncomeStatmentDesignHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<GenericTable> _journalAccountType;

        public ObservableCollection<GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountType; }
            set { _journalAccountType = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<TblIncomeStatmentDesignHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblIncomeStatmentDesignHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblIncomeStatmentDesignHeaderViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblIncomeStatmentDesignHeaderViewModel _selectedMainRow;

        public TblIncomeStatmentDesignHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblIncomeStatmentDesignDetailViewModel _selectedDetailRow;

        public TblIncomeStatmentDesignDetailViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblIncomeStatmentDesignDetailViewModel> _selectedDetailRows;

        public ObservableCollection<TblIncomeStatmentDesignDetailViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (SelectedDetailRows = new ObservableCollection<TblIncomeStatmentDesignDetailViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblIncomeStatmentDesignHeaderViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private SortableCollectionView<TblIncomeStatmentDesignDetailViewModel> _detailLst;

        public SortableCollectionView<TblIncomeStatmentDesignDetailViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new SortableCollectionView<TblIncomeStatmentDesignDetailViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }

        private int _iserialField;

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

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }
    }

    public class TblIncomeStatmentDesignDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private TblAccount _accountPerRow;

        public TblAccount AccountPerRow
        {
            get { return _accountPerRow; }
            set
            {
                _accountPerRow = value; RaisePropertyChanged("AccountPerRow");
                Description = AccountPerRow.Code;
            }
        }

        private GenericTable _tblJournalAccountType;

        public GenericTable JournalAccountTypePerRow
        {
            get { return _tblJournalAccountType; }
            set
            {
                if ((ReferenceEquals(_tblJournalAccountType, value) != true))
                {
                    _tblJournalAccountType = value;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                    if (JournalAccountTypePerRow != null) Type = JournalAccountTypePerRow.Code;
                }
            }
        }

        private int _rowOrder;

        public int RowOrder
        {
            get { return _rowOrder; }
            set { _rowOrder = value; RaisePropertyChanged("RowOrder"); }
        }

        private int _iserialField;

        private int _tblIncomeStatmentDesignField;

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

        public int TblIncomeStatmentDesignHeader
        {
            get
            {
                return _tblIncomeStatmentDesignField;
            }
            set
            {
                if ((_tblIncomeStatmentDesignField.Equals(value) != true))
                {
                    _tblIncomeStatmentDesignField = value;
                    RaisePropertyChanged("TblIncomeStatmentDesignHeader");
                }
            }
        }

        private string _type;

        [Required]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if ((ReferenceEquals(_type, value) != true))
                {
                    _type = value;
                    RaisePropertyChanged("Type");
                    if (Type.StartsWith("Account"))
                    {
                        IsAccount = Visibility.Visible;
                        NotAccount = Visibility.Collapsed;
                    }
                    else
                    {
                        IsAccount = Visibility.Collapsed;
                        NotAccount = Visibility.Visible;
                    }
                }
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        private bool _bold;

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; RaisePropertyChanged("Bold"); }
        }

        private bool _dr;

        public bool Dr
        {
            get { return _dr; }
            set { _dr = value; RaisePropertyChanged("Dr"); }
        }

        private bool _cr;

        public bool Cr
        {
            get { return _cr; }
            set { _cr = value; RaisePropertyChanged("Cr"); }
        }

        private Visibility _isAccount;

        public Visibility IsAccount
        {
            get { return _isAccount; }
            set { _isAccount = value; RaisePropertyChanged("IsAccount"); }
        }

        private Visibility _notAccount;

        public Visibility NotAccount
        {
            get { return _notAccount; }
            set { _notAccount = value; RaisePropertyChanged("NotAccount"); }
        }
    }

    #endregion ViewModels
}