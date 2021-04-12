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
using GenericTable = CCWFM.CRUDManagerService.GenericTable;

namespace CCWFM.ViewModel.Gl
{
    public class AssetGroupViewModel : ViewModelBase
    {
        public AssetGroupViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.AssetGroup.ToString());
                MainRowList = new ObservableCollection<TblAssetGroupViewModel>();

                var accountTypeClient = new GlServiceClient();       
                Glclient.UpdateOrInsertTblAssetGroupCompleted += (s, x) =>
                {
                    if (x.Error != null)
                    {
                        MessageBox.Show(x.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(x.outindex).InjectFrom(x.Result);
                    }
                    catch (Exception)
                    {
                    }
                    Loading = false;
                };
                
                Glclient.DeleteTblAssetGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    SelectedMainRow = new TblAssetGroupViewModel();
                };

                Glclient.GetTblAssetGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAssetGroupViewModel();
                        newrow.InjectFrom(row);
                
                        newrow.AccountPerRow = new TblAccount();
                        if (row.TblAccount1 != null) newrow.AccountPerRow = row.TblAccount1;

                        newrow.SumAccountPerRow = new TblAccount();
                        if (row.TblAccount2 != null) newrow.SumAccountPerRow = row.TblAccount2;
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
                GetMaindata();
            }
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
            var newrow = new TblAssetGroupViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void DeleteMainRow()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                if (SelectedMainRow.Iserial != 0)
                {
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;
                    }
                    Loading = true;
                    Glclient.DeleteTblAssetGroupAsync(
                        (TblAssetGroup)new TblAssetGroup().InjectFrom(SelectedMainRow), 0, LoggedUserInfo.DatabasEname);
                }
                else
                {
                    SelectedMainRow = new TblAssetGroupViewModel();
                }
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (save)
                    {
                        if (AllowAdd != true)
                        {
                            MessageBox.Show(strings.AllowAddMsg);
                            return;
                        }
                    }
                    else
                    {
                        if (AllowUpdate != true)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }
                    }
                    var saveRow = new TblAssetGroup();

                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblAssetGroupAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblAssetGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private TblAssetGroupViewModel _selectedMainRow;

        public TblAssetGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblAssetGroupViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblAssetGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblAssetGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAssetGroupViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private ObservableCollection<TblAssetGroupViewModel> _mainRowList;

        public ObservableCollection<TblAssetGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        //private ObservableCollection<GlService.GenericTable> _depreciationMethodList;

        //public ObservableCollection<GlService.GenericTable> DepreciationMethodList
        //{
        //    get { return _depreciationMethodList; }
        //    set
        //    {
        //        _depreciationMethodList = value;
        //        RaisePropertyChanged("DepreciationMethodList");
        //    }
        //}

        #endregion Prop
    }

    public class TblAssetGroupViewModel : Web.DataLayer.PropertiesViewModelBase
    {
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


        private int? _tblAccount;

        public int? TblAccount
        {
            get { return _tblAccount; }
            set { _tblAccount = value; RaisePropertyChanged("TblAccount"); }
        }

        private int? _sumAccount;

        public int? SumAccount
        {
            get { return _sumAccount; }
            set { _sumAccount = value; RaisePropertyChanged("SumAccount"); }
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
                    if (_accountPerRow != null)
                    {
                        if (_accountPerRow.Iserial != 0)
                        {
                            TblAccount = _accountPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private TblAccount _sumAccountPerRow;

        public TblAccount SumAccountPerRow
        {
            get { return _sumAccountPerRow; }
            set
            {
                if ((ReferenceEquals(_sumAccountPerRow, value) != true))
                {
                    _sumAccountPerRow = value;
                    RaisePropertyChanged("SumAccountPerRow");
                    if (_sumAccountPerRow != null)
                    {
                        if (_sumAccountPerRow.Iserial != 0)
                        {
                            SumAccount = _sumAccountPerRow.Iserial;
                        }
                    }
                }
            }
        }

        
    }
}