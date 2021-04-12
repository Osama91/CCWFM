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
using CCWFM.Helpers.Utilities;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class CashDepositeTypeViewModel : ViewModelBase
    {
        public CashDepositeTypeViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.CashdepositeTypeForm.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblCashDepositeTypeViewModel>();
                SelectedMainRow = new TblCashDepositeTypeViewModel();

 
                if (MainRowList.Any() && (SelectedMainRow == null))
                {
                    SelectedMainRow = MainRowList.FirstOrDefault();
                }

                //if (FullCount == 0 && MainRowList.Count == 0)
                //{
                //    AddNewMainRow(false);
                //}

                Glclient.GetTblCashDepositTypeCompleted  += (s, sv) =>
                {
                    CashDepositTypeList = sv.Result;
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCashDepositeTypeViewModel();
                        newrow.TblSequencePerRow = new GenericTable();
                        newrow.DepositeTypeGroupPerRow = new GenericTable();
                        newrow.InjectFrom(row);

                        if (row.TblSequence1 != null) newrow.TblSequencePerRow.InjectFrom(row.TblSequence1);
                        if (row.DepositeTypeGroup != null && row.DepositeTypeGroup != 0) newrow.DepositeTypeGroupPerRow.InjectFrom(CashDepositTypeList.FirstOrDefault(x=>x.Iserial == newrow.DepositeTypeGroup));

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
                    if (Export)
                    {
                        Export = false;
                        ExportGrid.ExportExcel("CashDepositeType");
                    }
                };

                Glclient.UpdateOrInsertTblCashDepositTypeCompleted += (s, ev) =>
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
                };
                Glclient.DeleteTblCashDepositTypesCompleted  += (s, ev) =>
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

                Glclient.GetCashDepositeSequenceCompleted += (s, sv) =>
                {
                    SequenceList = sv.Result;
                };

                Glclient.GetCashDepositeSequenceAsync(LoggedUserInfo.DatabasEname);

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
            Glclient.GetTblCashDepositTypeAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
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
            var newrow = new TblCashDepositeTypeViewModel();
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
                    var saveRow = new TblCashDepositType();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblCashDepositTypeAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblCashDepositTypesAsync((TblCashDepositType)new TblCashDepositType().InjectFrom(row),
                                MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        #region Prop

        private SortableCollectionView<TblCashDepositeTypeViewModel> _mainRowList;

        public SortableCollectionView<TblCashDepositeTypeViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblCashDepositeTypeViewModel> _selectedMainRows;

        public ObservableCollection<TblCashDepositeTypeViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCashDepositeTypeViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblCashDepositeTypeViewModel _selectedMainRow;

        public TblCashDepositeTypeViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        private ObservableCollection<TblSequence> _sequenceList;

        public ObservableCollection<TblSequence> SequenceList
        {
            get { return _sequenceList; }
            set { _sequenceList = value; RaisePropertyChanged("SequenceList"); }
        }

        private ObservableCollection<TblCashDepositType> _cashDepositTypeList;
        public ObservableCollection<TblCashDepositType> CashDepositTypeList
        {
            get { return _cashDepositTypeList; }
            set { _cashDepositTypeList = value; RaisePropertyChanged("CashDepositTypeList"); }
        }


        #endregion Prop
    }

    #region ViewModels

    public class TblCashDepositeTypeViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserialField;

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

        private string _enameField;

        public string Ename
        {
            get
            {
                return _enameField;
            }
            set
            {
                if ((ReferenceEquals(_enameField, value) != true))
                {
                    _enameField = value;
                    RaisePropertyChanged("Ename");
                }
            }
        }

        private string _anameField;

        public string Aname
        {
            get
            {
                return _anameField;
            }
            set
            {
                if ((ReferenceEquals(_anameField, value) != true))
                {
                    _anameField = value;
                    RaisePropertyChanged("Aname");
                }
            }
        }

        
        private int _tblSequenceField;

        public int TblSequence
        {
            get
            {
                return _tblSequenceField;
            }
            set
            {
                if ((_tblSequenceField.Equals(value) != true))
                {
                    _tblSequenceField = value;
                    RaisePropertyChanged("TblSequence");
                }
            }
        }

        private string _ledgerDescriptionHeaderField;

        public string LedgerDescriptionHeader
        {
            get
            {
                return _ledgerDescriptionHeaderField;
            }
            set
            {
                if ((ReferenceEquals(_ledgerDescriptionHeaderField, value) != true))
                {
                    _ledgerDescriptionHeaderField = value;
                    RaisePropertyChanged("LedgerDescriptionHeader");
                }
            }
        }


        private string _ledgerDescriptionDetailField;

        public string LedgerDescriptionDetail
        {
            get
            {
                return _ledgerDescriptionDetailField;
            }
            set
            {
                if ((ReferenceEquals(_ledgerDescriptionDetailField, value) != true))
                {
                    _ledgerDescriptionDetailField = value;
                    RaisePropertyChanged("LedgerDescriptionDetail");
                }
            }
        }
        private int _depositeTypeGroupField;

        public int DepositeTypeGroup
        {
            get
            {
                return _depositeTypeGroupField;
            }
            set
            {
                if ((_depositeTypeGroupField.Equals(value) != true))
                {
                    _depositeTypeGroupField = value;
                    RaisePropertyChanged("DepositeTypeGroup");
                }
            }
        }

        
        private GenericTable _tblSequencePerRow;

        public GenericTable TblSequencePerRow
        {
            get { return _tblSequencePerRow; }
            set
            {
                if ((ReferenceEquals(_tblSequencePerRow, value) != true))
                {
                    _tblSequencePerRow = value;
                    RaisePropertyChanged("TblSequencePerRow");
                    if (_tblSequencePerRow != null)
                    {
                        if (_tblSequencePerRow.Iserial != 0)
                        {
                            TblSequence = _tblSequencePerRow.Iserial;
                        }

                    }

                }
            }
        }

        private GenericTable _depositeTypeGroupPerRow;

        public GenericTable DepositeTypeGroupPerRow
        {
            get { return _depositeTypeGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_depositeTypeGroupPerRow, value) != true))
                {
                    _depositeTypeGroupPerRow = value;
                    RaisePropertyChanged("DepositeTypeGroupPerRow");
                    if (_depositeTypeGroupPerRow != null)
                    {
                        if (_depositeTypeGroupPerRow.Iserial != 0)
                        {
                            DepositeTypeGroup = _depositeTypeGroupPerRow.Iserial;
                        }

                    }

                }
            }
        }
    }

    #endregion ViewModels
}
