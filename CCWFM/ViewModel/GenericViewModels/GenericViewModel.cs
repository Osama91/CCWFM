using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Helpers.Utilities;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.GenericViewModels
{
    public class GenericViewModelCollection : ViewModelBase
    {
        #region Prop

        private string _direction;
        private string _code;
        private string _ename;
        private string _aname;

        public string Direction
        {
            get { return _direction ?? "ASC"; }
            set { _direction = value; }
        }

        public new string Code
        {
            get { return _code ?? "%%"; }
            set { _code = value; }
        }

        public string Ename
        {
            get { return _ename ?? "%%"; }
            set { _ename = value; }
        }

        public string Aname
        {
            get { return _aname ?? "%%"; }
            set { _aname = value; }
        }

        private GenericViewModel _selectedMainRow;

        public GenericViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private SortableCollectionView<GenericViewModel> _mainRowList;

        public SortableCollectionView<GenericViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private SortableCollectionView<GenericViewModel> _selectedMainRows;

        public SortableCollectionView<GenericViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new SortableCollectionView<GenericViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        public string TablEname;

        #endregion Prop

        public GenericViewModelCollection(string tablEname, PermissionItemName userJobsForm)
        {
            if (DesignerProperties.IsInDesignTool)
                return;
            TablEname = tablEname;
            Client = new _Proxy.CRUD_ManagerServiceClient();
            GetItemPermissions(userJobsForm.ToString());

            Client.GetGenericCompleted += (s, ev) =>
            {
                Loading = false;
                GenericMapper.InjectFromObCollection(MainRowList, ev.Result);
                //FullCount = ev.fullCount;
                if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
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
                    ExportGrid.ExportExcel(userJobsForm.ToString());
                }
            };

            MainRowList = new SortableCollectionView<GenericViewModel>();

            Client.GenericUpdateOrInsertCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                }
                Loading = false;
                MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
            };

            Client.DeleteGenericCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                }
                Loading = false;
                var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
                if (!MainRowList.Any())
                {
                    AddNewMainRow(false);
                }
            };
            GetMaindata();
        }

        internal void GetMaindata()
        {
            if (Loading) return;
            if (SortBy == null)
                SortBy = "Iserial";
            Loading = true;
            Client.GetGenericAsync(TablEname, Code, Ename, Aname, SortBy, Direction);
        }

        public void GetMaindataFull(OsGrid mainGrid)
        {
            if (SortBy == null)
                SortBy = "Iserial";

            Export = true;
            Loading = true;
            ExportGrid = mainGrid;
            Client.GetGenericAsync(TablEname, Code, Ename, Aname, SortBy, Direction);
        }

        internal void AddNewMainRow(bool checkLastRow)
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

            var newrow = new GenericViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        internal void Delete()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                Loading = true;
                foreach (var item in SelectedMainRows)
                {
                    if (item.Iserial == 0)
                    {
                        MainRowList.Remove(item);
                        if (!MainRowList.Any())
                        {
                            AddNewMainRow(false);
                        }
                    }
                    else
                    {
                        if (AllowDelete != true)
                        {
                            MessageBox.Show(strings.AllowDeleteMsg);
                            return;
                        }
                        Loading = true;
                        Client.DeleteGenericAsync(TablEname, item.Iserial.ToString(CultureInfo.InvariantCulture),
                            MainRowList.IndexOf(item));
                    }
                }
            }
        }

        internal void SaveOrUpdate()
        {
            Loading = true;

            if (SelectedMainRow != null)
            {
                var save = SelectedMainRow.Iserial == 0;

                if (AllowUpdate != true && !save)
                {
                    MessageBox.Show(strings.AllowUpdateMsg);
                    return;
                }
                var saveRow = new _Proxy.GenericTable();
                saveRow.InjectFrom(SelectedMainRow);
                Client.GenericUpdateOrInsertAsync(TablEname, saveRow, MainRowList.IndexOf(SelectedMainRow));
            }
        }

        internal void SaveMainRow()
        {
            if (SelectedMainRow == null) return;
            var saveRow = new _Proxy.GenericTable();
            saveRow.InjectFrom(SelectedMainRow);
            var save = SelectedMainRow.Iserial == 0;
            if (AllowUpdate != true && !save)
            {
                MessageBox.Show(strings.AllowUpdateMsg);
                return;
            }
            Loading = true;
            Client.GenericUpdateOrInsertAsync(TablEname, saveRow, MainRowList.IndexOf(SelectedMainRow));
        }

        public void SaveImported(ObservableCollection<GenericViewModel> list)
        {
            foreach (var variable in list)
            {
                if (variable == null) return;
                var saveRow = new _Proxy.GenericTable();
                saveRow.InjectFrom(variable);
                var save = variable.Iserial == 0;
                if (AllowUpdate != true && !save)
                {
                    MessageBox.Show(strings.AllowUpdateMsg);
                    return;
                }
                Loading = true;
                Client.GenericUpdateOrInsertAsync(TablEname, saveRow, 0);
            }
        }
    }

    public class GenericViewModelCollectionGl : ViewModelBase
    {
        #region Prop

        private string _direction;
        private string _code;
        private string _ename;
        private string _aname;

        public string Direction
        {
            get { return _direction ?? "ASC"; }
            set { _direction = value; }
        }

        public new string Code
        {
            get { return _code ?? "%%"; }
            set { _code = value; }
        }

        public string Ename
        {
            get { return _ename ?? "%%"; }
            set { _ename = value; }
        }

        public string Aname
        {
            get { return _aname ?? "%%"; }
            set { _aname = value; }
        }

        private GenericViewModel _selectedMainRow;

        public GenericViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private SortableCollectionView<GenericViewModel> _mainRowList;

        public SortableCollectionView<GenericViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private SortableCollectionView<GenericViewModel> _selectedMainRows;

        public SortableCollectionView<GenericViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new SortableCollectionView<GenericViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        public string TablEname;

        #endregion Prop

        public GenericViewModelCollectionGl(string tablEname, PermissionItemName userJobsForm)
        {
            if (DesignerProperties.IsInDesignTool)
                return;
            TablEname = tablEname;
            Glclient = new GlServiceClient();
            GetItemPermissions(userJobsForm.ToString());

            Glclient.GetGenericCompleted += (s, ev) =>
            {
                Loading = false;
                GenericMapper.InjectFromObCollection(MainRowList, ev.Result);
                //FullCount = ev.fullCount;
                if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
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
                    ExportGrid.ExportExcel(userJobsForm.ToString());
                }
            };

            MainRowList = new SortableCollectionView<GenericViewModel>();

            Glclient.GenericUpdateOrInsertCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                }
                Loading = false;
                MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
            };

            Glclient.DeleteGenericCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                }
                Loading = false;
                var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
                if (!MainRowList.Any())
                {
                    AddNewMainRow(false);
                }
            };
            GetMaindata();
        }

        internal void GetMaindata()
        {
            if (Loading) return;
            if (SortBy == null)
                SortBy = "Iserial";
            Loading = true;
            Glclient.GetGenericAsync(TablEname, Code, Ename, Aname, SortBy, Direction, LoggedUserInfo.DatabasEname);
        }

        public void GetMaindataFull(OsGrid mainGrid)
        {
            if (SortBy == null)
                SortBy = "Iserial";

            Export = true;
            Loading = true;
            ExportGrid = mainGrid;
            Glclient.GetGenericAsync(TablEname, Code, Ename, Aname, SortBy, Direction, LoggedUserInfo.DatabasEname);
        }

        internal void AddNewMainRow(bool checkLastRow)
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

            var newrow = new GenericViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        internal void Delete()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                Loading = true;
                foreach (var item in SelectedMainRows)
                {
                    if (item.Iserial == 0)
                    {
                        MainRowList.Remove(item);
                        if (!MainRowList.Any())
                        {
                            AddNewMainRow(false);
                        }
                    }
                    else
                    {
                        if (AllowDelete != true)
                        {
                            MessageBox.Show(strings.AllowDeleteMsg);
                            return;
                        }
                        Loading = true;
                        Glclient.DeleteGenericAsync(TablEname, item.Iserial.ToString(CultureInfo.InvariantCulture),
                            LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        internal void SaveOrUpdate()
        {
            Loading = true;

            if (SelectedMainRow != null)
            {
                var save = SelectedMainRow.Iserial == 0;

                if (AllowUpdate != true && !save)
                {
                    MessageBox.Show(strings.AllowUpdateMsg);
                    return;
                }
                var saveRow = new GenericTable();
                saveRow.InjectFrom(SelectedMainRow);
                Glclient.GenericUpdateOrInsertAsync(TablEname, saveRow, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
            }
        }

        internal void SaveMainRow()
        {
            if (SelectedMainRow == null) return;
            var saveRow = new GenericTable();
            saveRow.InjectFrom(SelectedMainRow);
            var save = SelectedMainRow.Iserial == 0;
            if (AllowUpdate != true && !save)
            {
                MessageBox.Show(strings.AllowUpdateMsg);
                return;
            }
            Loading = true;
            Glclient.GenericUpdateOrInsertAsync(TablEname, saveRow, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
        }

        public void SaveImported(ObservableCollection<GenericViewModel> list)
        {
            foreach (var variable in list)
            {
                if (variable == null) return;
                var saveRow = new GenericTable();
                saveRow.InjectFrom(variable);
                var save = variable.Iserial == 0;
                if (AllowUpdate != true && !save)
                {
                    MessageBox.Show(strings.AllowUpdateMsg);
                    return;
                }
                Loading = true;
                Glclient.GenericUpdateOrInsertAsync(TablEname, saveRow, 0, LoggedUserInfo.DatabasEname);
            }
        }
    }

    #region ViewModels

    public class GenericViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private ObjectStatus _status;

        public GenericViewModel()
        {
            Status = new ObjectStatus { IsMarkedForDeletion = false };
        }

        public ObjectStatus Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private string _codeField;

        private int _iserialField;

        private string _aname;

        private string _ename;

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
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get { return _aname; }
            set
            {
                if ((ReferenceEquals(_aname, value) != true))
                {
                    if (value != null) _aname = value.Trim();
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get { return _ename; }
            set
            {
                if ((ReferenceEquals(_ename, value) != true))
                {
                    if (value != null) _ename = value.Trim();
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
                }
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string Code
        {
            get { return _codeField; }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    if (value != null) _codeField = value.Trim();
                    RaisePropertyChanged("Code");
                }
            }
        }

     
    }

    public class ObjectStatus : ViewModelBase
    {
        private bool _isLoading;
        private bool _isChanged;
        private bool _isMarkedForDeletion;
        private bool _isNew;
        private bool _isReadyForSaving;
        private bool _isSavedDbItem;

        private bool _isEmpty;

        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { _isEmpty = value; RaisePropertyChanged("IsEmpty"); }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; RaisePropertyChanged("IsChanged"); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChanged("IsLoading"); }
        }

        public bool IsMarkedForDeletion
        {
            get { return _isMarkedForDeletion; }
            set { _isMarkedForDeletion = value; RaisePropertyChanged("IsMarkedForDeletion"); }
        }

        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; RaisePropertyChanged("IsNew"); }
        }

        public bool IsReadyForSaving
        {
            get { return _isReadyForSaving; }
            set { _isReadyForSaving = value; RaisePropertyChanged("IsReadyForSaving"); }
        }

        public bool IsSavedDBItem
        {
            get { return _isSavedDbItem; }
            set { _isSavedDbItem = value; RaisePropertyChanged("IsSavedDBItem"); }
        }
    }
}

    #endregion ViewModels