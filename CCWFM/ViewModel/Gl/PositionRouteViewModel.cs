using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.Gl
{
    public class PositionRouteViewModel : ViewModelBase
    {
        public PositionRouteViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.PositionRoute.ToString());
                Glclient = new GlServiceClient();
                Client.GetEmpPositionAsync("");
                Client.GetEmpPositionCompleted += (s, sv) =>
                 {
                     PositionList = sv.Result;
                 };


                MainRowList = new ObservableCollection<TblPositionRouteHeaderViewModel>();
                SelectedMainRow = new TblPositionRouteHeaderViewModel();
                PositionList = new ObservableCollection<string>();
                Glclient.GetTblPositionRouteHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPositionRouteHeaderViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblPositionRouteHeaderCompleted += (s, ev) =>
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
                Glclient.DeleteTblPositionRouteHeaderCompleted += (s, ev) =>
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

                Glclient.GetTblPositionRouteCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblPositionRouteViewModel();

                        newrow.InjectFrom(row);
                        newrow.FromStorePerRow = new CRUDManagerService.TblStore().InjectFrom(row.TblStore) as CRUDManagerService.TblStore;
                        newrow.ToStorePerRow = new CRUDManagerService.TblStore().InjectFrom(row.TblStore1) as CRUDManagerService.TblStore;
                        if (sv.EmpList.FirstOrDefault(w => w.Emplid == newrow.Emplid)!=null)
                        {
                            newrow.EmpPerRow = new CRUDManagerService.EmployeesView().InjectFrom(sv.EmpList.FirstOrDefault(w => w.Emplid == newrow.Emplid)) as CRUDManagerService.EmployeesView;

                        }
                        else
                        {
                            newrow.EmpPerRow = new CRUDManagerService.EmployeesView() { Emplid = newrow.Emplid };
                            
                        }
                        SelectedMainRow.DetailsList.Add(newrow);
                    }

                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (SelectedMainRow.DetailsList.Any() && (SelectedDetailRow == null))
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblPositionRouteCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        SelectedMainRow.DetailsList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                Glclient.DeleteTblPositionRouteCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblPositionRouteHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblPositionRouteHeaderViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (!Loading)
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
                        var saveRow = new TblPositionRouteHeader();
                        saveRow.InjectFrom(SelectedMainRow);
                        if (Loading == false)
                        {
                            Loading = true;
                            Glclient.UpdateOrInsertTblPositionRouteHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                                LoggedUserInfo.DatabasEname);
                        }
                    }
                }
            }
        }

        public void SaveOldRow(TblPositionRouteHeaderViewModel oldRow)
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
                    var saveRow = new TblPositionRouteHeader();
                    saveRow.InjectFrom(oldRow);

                    if (Loading == false)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblPositionRouteHeaderAsync(saveRow, save, MainRowList.IndexOf(oldRow),
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
                            Glclient.DeleteTblPositionRouteHeaderAsync((TblPositionRouteHeader)new TblPositionRouteHeader().InjectFrom(row), 0, LoggedUserInfo.DatabasEname);
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
                Glclient.GetTblPositionRouteAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
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
                            Glclient.DeleteTblPositionRouteAsync(
                                (TblPositionRoute)new TblPositionRoute().InjectFrom(row), 1,
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

            var newrow = new TblPositionRouteViewModel
            {
                TblPositionRouteHeader = SelectedMainRow.Iserial,
                DocDate = SelectedMainRow.DocDate
            };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);
            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (!Loading)
            {
                if (SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow,
                        new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (isvalid)
                    {
                        var save = SelectedDetailRow.Iserial == 0;
                        if (SelectedDetailRow.TblPositionRouteHeader == 0)
                        {
                            SelectedDetailRow.TblPositionRouteHeader = SelectedMainRow.Iserial;
                        }
                        if (AllowUpdate != true && !save)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }

                        var rowToSave = new TblPositionRoute();
                        rowToSave.InjectFrom(SelectedDetailRow);
                        Loading = true;
                        Glclient.UpdateOrInsertTblPositionRouteAsync(rowToSave, save,
                            SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }
        public void SaveOldDetailRow(TblPositionRouteViewModel oldRow)
        {
            if (!Loading)
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
                        var saveRow = new TblPositionRoute();
                        saveRow.InjectFrom(oldRow);

                        if (SelectedMainRow != null && SelectedMainRow.DetailsList != null)
                        {
                            Loading = true;

                            Glclient.UpdateOrInsertTblPositionRouteAsync(saveRow, save, SelectedMainRow.DetailsList.IndexOf(oldRow),
                                LoggedUserInfo.DatabasEname);
                        }
                    }
                }
            }
        }

        #region Prop     

        private ObservableCollection<TblPositionRouteHeaderViewModel> _mainRowList;

        public ObservableCollection<TblPositionRouteHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }
        private ObservableCollection<string> _PositionList;

        public ObservableCollection<string> PositionList
        {
            get { return _PositionList; }
            set
            {
                _PositionList = value;
                RaisePropertyChanged("PositionList");
            }
        }


        private ObservableCollection<TblPositionRouteHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblPositionRouteHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblPositionRouteHeaderViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblPositionRouteHeaderViewModel _selectedMainRow;

        public TblPositionRouteHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private TblPositionRouteViewModel _selectedDetailRow;

        public TblPositionRouteViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set
            {
                _selectedDetailRow = value;
                RaisePropertyChanged("SelectedDetailRow");
            }
        }

        private ObservableCollection<TblPositionRouteViewModel> _selectedDetailRows;

        public ObservableCollection<TblPositionRouteViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblPositionRouteViewModel>()); }
            set
            {
                _selectedDetailRows = value;
                RaisePropertyChanged("SelectedDetailRows");
            }
        }



        #endregion Prop
    }

    #region ViewModels

    public class TblPositionRouteHeaderViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblPositionRouteViewModel> _detailLst;

        public ObservableCollection<TblPositionRouteViewModel> DetailsList
        {
            get { return _detailLst ?? (_detailLst = new ObservableCollection<TblPositionRouteViewModel>()); }
            set { _detailLst = value; RaisePropertyChanged("DetailsList"); }
        }
        private int? CreatedByField;

        private DateTime? CreationDateField;

        private DateTime? DocDateField;

        private int IserialField;

        private DateTime? LastModifiedDateField;

        private int? ModifiedByField;


        public int? CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {
                if ((this.CreatedByField.Equals(value) != true))
                {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }


        public DateTime? CreationDate
        {
            get
            {
                return this.CreationDateField;
            }
            set
            {
                if ((this.CreationDateField.Equals(value) != true))
                {
                    this.CreationDateField = value;
                    this.RaisePropertyChanged("CreationDate");
                }
            }
        }

        [Required]
        public System.DateTime? DocDate
        {
            get
            {
                return this.DocDateField;
            }
            set
            {
                if ((this.DocDateField.Equals(value) != true))
                {
                    this.DocDateField = value;
                    this.RaisePropertyChanged("DocDate");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }


        public DateTime? LastModifiedDate
        {
            get
            {
                return this.LastModifiedDateField;
            }
            set
            {
                if ((this.LastModifiedDateField.Equals(value) != true))
                {
                    this.LastModifiedDateField = value;
                    this.RaisePropertyChanged("LastModifiedDate");
                }
            }
        }


        public System.Nullable<int> ModifiedBy
        {
            get
            {
                return this.ModifiedByField;
            }
            set
            {
                if ((this.ModifiedByField.Equals(value) != true))
                {
                    this.ModifiedByField = value;
                    this.RaisePropertyChanged("ModifiedBy");
                }
            }
        }
    }

    public class TblPositionRouteViewModel : PropertiesViewModelBase
    {
        private int? CreatedByField;

        private DateTime? CreationDateField;

        private System.DateTime? DocDateField;

        private string FromPositionField;

        private int? FromStoreField;

        private int IserialField;

        private DateTime? LastModifiedDateField;

        private int? ModifiedByField;

        private string TblEmployeeField;

        private int? TblPositionRouteHeaderField;

        private CRUDManagerService.TblStore TblStoreField;

        private CRUDManagerService.TblStore TblStore1Field;


        private string ToPositionField;

        private int? ToStoreField;


        public System.Nullable<int> CreatedBy
        {
            get
            {
                return this.CreatedByField;
            }
            set
            {
                if ((this.CreatedByField.Equals(value) != true))
                {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }


        public DateTime? CreationDate
        {
            get
            {
                return this.CreationDateField;
            }
            set
            {
                if ((this.CreationDateField.Equals(value) != true))
                {
                    this.CreationDateField = value;
                    this.RaisePropertyChanged("CreationDate");
                }
            }
        }


        public System.DateTime? DocDate
        {
            get
            {
                return this.DocDateField;
            }
            set
            {
                if ((this.DocDateField.Equals(value) != true))
                {
                    this.DocDateField = value;
                    this.RaisePropertyChanged("DocDate");
                }
            }
        }

        [Required]
        public string FromPosition
        {
            get
            {
                return this.FromPositionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.FromPositionField, value) != true))
                {
                    this.FromPositionField = value;
                    this.RaisePropertyChanged("FromPosition");
                }
            }
        }

        [Required]
        public int? FromStore
        {
            get
            {
                return this.FromStoreField;
            }
            set
            {
                if ((this.FromStoreField.Equals(value) != true))
                {
                    this.FromStoreField = value;
                    this.RaisePropertyChanged("FromStore");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }


        public DateTime? LastModifiedDate
        {
            get
            {
                return this.LastModifiedDateField;
            }
            set
            {
                if ((this.LastModifiedDateField.Equals(value) != true))
                {
                    this.LastModifiedDateField = value;
                    this.RaisePropertyChanged("LastModifiedDate");
                }
            }
        }


        public int? ModifiedBy
        {
            get
            {
                return this.ModifiedByField;
            }
            set
            {
                if ((this.ModifiedByField.Equals(value) != true))
                {
                    this.ModifiedByField = value;
                    this.RaisePropertyChanged("ModifiedBy");
                }
            }
        }


        public string Emplid
        {
            get
            {
                return this.TblEmployeeField;
            }
            set
            {

                this.TblEmployeeField = value;
                this.RaisePropertyChanged("Emplid");

            }
        }


        public int? TblPositionRouteHeader
        {
            get
            {
                return TblPositionRouteHeaderField;
            }
            set
            {
                if ((this.TblPositionRouteHeaderField.Equals(value) != true))
                {
                    this.TblPositionRouteHeaderField = value;
                    this.RaisePropertyChanged("TblPositionRouteHeader");
                }
            }
        }

        CRUDManagerService.EmployeesView _EmpPerRow;
        public CRUDManagerService.EmployeesView EmpPerRow
        {
            get
            {
                return this._EmpPerRow;
            }
            set
            {
                if ((object.ReferenceEquals(this._EmpPerRow, value) != true))
                {
                    this._EmpPerRow = value;
                    if (value != null && value.Emplid != "")
                    {
                        Emplid = value.Emplid;

                    }
                    this.RaisePropertyChanged("EmpPerRow");
                }
            }
        }
        public CRUDManagerService.TblStore FromStorePerRow
        {
            get
            {
                return this.TblStoreField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblStoreField, value) != true))
                {
                    this.TblStoreField = value;
                    if (value != null && value.iserial != 0)
                    {
                        FromStore = value.iserial;

                    }
                    this.RaisePropertyChanged("FromStorePerRow");
                }
            }
        }


        public CRUDManagerService.TblStore ToStorePerRow
        {
            get
            {
                return this.TblStore1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblStore1Field, value) != true))
                {
                    this.TblStore1Field = value;
                    if (value != null && value.iserial != 0)
                    {
                        ToStore = value.iserial;
                    }
                    this.RaisePropertyChanged("ToStorePerRow");
                }
            }
        }


        [Required]
        public string ToPosition
        {
            get
            {
                return this.ToPositionField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ToPositionField, value) != true))
                {
                    this.ToPositionField = value;
                    this.RaisePropertyChanged("ToPosition");
                }
            }
        }

        [Required]
        public int? ToStore
        {
            get
            {
                return this.ToStoreField;
            }
            set
            {
                if ((this.ToStoreField.Equals(value) != true))
                {
                    this.ToStoreField = value;
                    this.RaisePropertyChanged("ToStore");
                }
            }
        }
    }

    #endregion ViewModels
}