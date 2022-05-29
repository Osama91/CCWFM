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
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class CostDimChildViewModel : ViewModelBase
    {
        public CostDimChildViewModel(LedgerHeaderViewModel ledgerHeaderViewModel)
        {
            if (!IsDesignTime)
            {
                LedgerHeaderViewModel = ledgerHeaderViewModel;
                GetItemPermissions(PermissionItemName.Account.ToString());
                Glclient = new GlServiceClient();

                MainRowList = new SortableCollectionView<TblCostDimDetailViewModel>();
                SelectedMainRow = new TblCostDimDetailViewModel();

                Glclient.GetTblCostDimSetupHeaderForAccountCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                        foreach (var row in sv.Result.TblCostDimDetails)
                        {
                            var newrow = new TblCostDimDetailViewModel();
                            newrow.InjectFrom(row);
                            newrow.CostCenterPerRow = row.TblCostCenter1;
                            MainRowList.Add(newrow);
                        }

                    Loading = false;

                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.SaveTblCostDimHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        ledgerHeaderViewModel.SelectedDetailRow.TblCostDimHeader = ev.Result;
                        ledgerHeaderViewModel.SaveMainRow(true);
                    }
                    catch (Exception)
                    {
                    }
                };
                GetMaindata();
            }
        }

        public LedgerHeaderViewModel LedgerHeaderViewModel { get; set; }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblCostDimSetupHeaderForAccountAsync((int)LedgerHeaderViewModel.SelectedDetailRow.TblJournalAccountType, LedgerHeaderViewModel.SelectedDetailRow.TblCostDimHeader
                , LoggedUserInfo.DatabasEname);
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
            var newrow = new TblCostDimDetailViewModel();
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
                    var costdimist = new ObservableCollection<TblCostDimDetail>();
                    foreach (var tblCostDimDetailViewModel in MainRowList)
                    {
                        costdimist.Add(new TblCostDimDetail().InjectFrom(tblCostDimDetailViewModel) as TblCostDimDetail);
                    }

                    var saveRow = new TblCostDimDetail();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.SaveTblCostDimHeaderAsync((int)LedgerHeaderViewModel.SelectedDetailRow.TblJournalAccountType, costdimist,
                        LoggedUserInfo.DatabasEname);
                }
            }
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                foreach (var row in SelectedMainRows)
                {
                    MainRowList.Remove(row);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblCostDimDetailViewModel> _mainRowList;

        public SortableCollectionView<TblCostDimDetailViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblCostDimDetailViewModel> _selectedMainRows;

        public ObservableCollection<TblCostDimDetailViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostDimDetailViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblCostDimDetailViewModel _selectedMainRow;

        public TblCostDimDetailViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblCostDimDetailViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _tblCostDimHeader;

        public int TblCostDimHeader
        {
            get { return _tblCostDimHeader; }
            set { _tblCostDimHeader = value; RaisePropertyChanged("TblCostDimHeader"); }
        }

        private int? _TblCostCenter;

        public int? TblCostCenter
        {
            get { return _TblCostCenter; }
            set { _TblCostCenter = value; RaisePropertyChanged("TblCostCenter"); }
        }

        private TblCostCenter _costCenterTypePerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _costCenterTypePerRow; }
            set
            {
                if ((ReferenceEquals(_costCenterTypePerRow, value) != true))
                {
                    _costCenterTypePerRow = value;
                    RaisePropertyChanged("CostCenterPerRow");
                    if (CostCenterPerRow != null) TblCostCenter = CostCenterPerRow.Iserial;
                }
            }
        }

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
    }

    #endregion ViewModels
}