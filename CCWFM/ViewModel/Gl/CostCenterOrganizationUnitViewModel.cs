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

namespace CCWFM.ViewModel.Gl
{
    public class CostCenterOrganizationUnitViewModel : ViewModelBase
    {
        public CostCenterOrganizationUnitViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.CostCenterOrganizationUnit.ToString());
                MainRowList = new ObservableCollection<TblCostCenterOrganizationUnitViewModel>();

                //var CostCenterTypeClient = new GlServiceClient();
                //CostCenterTypeClient.GetGenericCompleted += (s, sv) => { DepreciationMethodList = sv.Result; };
                //CostCenterTypeClient.GetGenericAsync("TblDepreciationMethod", "%%", "%%", "%%", "Iserial", "ASC",
                //    LoggedUserInfo.DatabasEname);
                Glclient.UpdateOrInsertTblCostCenterOrganizationUnitsCompleted += (s, x) =>
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

                Glclient.GetTblCostCenterAsync(0, int.MaxValue, null, "It.Iserial", null, null, LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial, 0, 0);
                Glclient.GetTblCostCenterCompleted += (s, sv) =>
                 {
                     CostCenterList = sv.Result;
                     };
                Glclient.DeleteTblCostCenterOrganizationUnitCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    SelectedMainRow = new TblCostCenterOrganizationUnitViewModel();
                };
                Glclient.GetPayrollOrganizationUnitAsync();
                Glclient.GetPayrollOrganizationUnitCompleted += (s, sv) =>
                  {

                      PayrollOrganizationUnitList = sv.Result;
                  };
                Glclient.GetTblCostCenterOrganizationUnitCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostCenterOrganizationUnitViewModel();
                        newrow.InjectFrom(row);
                
                        newrow.CostCenterPerRow = new TblCostCenter();
                        if (row.TblCostCenter1 != null) newrow.CostCenterPerRow = row.TblCostCenter1;

                        //newrow.SumCostCenterPerRow = new TblCostCenter();
                        //if (row.TblCostCenter2 != null) newrow.SumCostCenterPerRow = row.TblCostCenter2;
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
            var newrow = new TblCostCenterOrganizationUnitViewModel();
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
                    Glclient.DeleteTblCostCenterOrganizationUnitAsync(
                        (TblCostCenterOrganizationUnit)new TblCostCenterOrganizationUnit().InjectFrom(SelectedMainRow), 0, LoggedUserInfo.DatabasEname);
                }
                else
                {
                    SelectedMainRow = new TblCostCenterOrganizationUnitViewModel();
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
                    var saveRow = new TblCostCenterOrganizationUnit();

                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblCostCenterOrganizationUnitsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
            Glclient.GetTblCostCenterOrganizationUnitAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private TblCostCenterOrganizationUnitViewModel _selectedMainRow;

        public TblCostCenterOrganizationUnitViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblCostCenterOrganizationUnitViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblCostCenterOrganizationUnitViewModel> _selectedMainRows;

        public ObservableCollection<TblCostCenterOrganizationUnitViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostCenterOrganizationUnitViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private ObservableCollection<TblCostCenterOrganizationUnitViewModel> _mainRowList;

        public ObservableCollection<TblCostCenterOrganizationUnitViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        
        private ObservableCollection<PayrollOrganizationUnit> _PayrollOrganizationUnitList;

        public ObservableCollection<PayrollOrganizationUnit> PayrollOrganizationUnitList
        {
            get { return _PayrollOrganizationUnitList; }
            set
            {
                _PayrollOrganizationUnitList = value;
                RaisePropertyChanged("PayrollOrganizationUnitList");
            }
        }
        private ObservableCollection<GlService.TblCostCenter> _CostCeterList;

        public ObservableCollection<GlService.TblCostCenter> CostCenterList
        {
            get { return _CostCeterList; }
            set
            {
                _CostCeterList = value;
                RaisePropertyChanged("CostCenterList");
            }
        }

        #endregion Prop
    }

    public class TblCostCenterOrganizationUnitViewModel : Web.DataLayer.PropertiesViewModelBase
    { 
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
        private int? _TblCostCenter;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCostCenter")]
        public int? TblCostCenter
        {
            get { return _TblCostCenter; }
            set { _TblCostCenter = value; RaisePropertyChanged("TblCostCenter"); }
        }    
        private TblCostCenter _CostCenterPerRow;

        public TblCostCenter CostCenterPerRow
        {
            get { return _CostCenterPerRow; }
            set
            {
                if ((ReferenceEquals(_CostCenterPerRow, value) != true))
                {
                    _CostCenterPerRow = value;
                    RaisePropertyChanged("CostCenterPerRow");
                    if (_CostCenterPerRow != null)
                    {
                        if (_CostCenterPerRow.Iserial != 0)
                        {
                            TblCostCenter = _CostCenterPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private int? _TblOrganizationUnit;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqOrganizationUnit")]
        public int? TblOrganizationUnit
        {
            get { return _TblOrganizationUnit; }
            set { _TblOrganizationUnit = value; RaisePropertyChanged("TblOrganizationUnit"); }
        }
        private PayrollOrganizationUnit _OrganizationUnitPerRow;

        public PayrollOrganizationUnit OrganizationUnitPerRow
        {
            get { return _OrganizationUnitPerRow; }
            set
            {
                if ((ReferenceEquals(_OrganizationUnitPerRow, value) != true))
                {
                    _OrganizationUnitPerRow = value;
                    RaisePropertyChanged("OrganizationUnitPerRow");
                    if (_OrganizationUnitPerRow != null)
                    {
                        if (_OrganizationUnitPerRow.Iserial != 0)
                        {
                            TblOrganizationUnit = _OrganizationUnitPerRow.Iserial;
                        }
                    }
                }
            }
        }
    }
}