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
    public class CostCenterRouteGroupViewModel : ViewModelBase
    {
        public CostCenterRouteGroupViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.CostCenterRouteGroup.ToString());
                MainRowList = new ObservableCollection<TblCostCenterRouteGroupViewModel>();             
                Glclient.UpdateOrInsertTblCostCenterRouteGroupsCompleted += (s, x) =>
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
                Glclient.DeleteTblCostCenterRouteGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    SelectedMainRow = new TblCostCenterRouteGroupViewModel();
                };
                Client.GetTblRouteGroupAsync(0,int.MaxValue,"it.Iserial",null,null) ;
                Client.GetTblRouteGroupCompleted += (s, sv) =>
                  {

                      RouteGroupList = sv.Result;
                  };
                Glclient.GetTblCostCenterRouteGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCostCenterRouteGroupViewModel();
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
            var newrow = new TblCostCenterRouteGroupViewModel();
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
                    Glclient.DeleteTblCostCenterRouteGroupAsync(
                        (TblCostCenterRouteGroup)new TblCostCenterRouteGroup().InjectFrom(SelectedMainRow), 0, LoggedUserInfo.DatabasEname);
                }
                else
                {
                    SelectedMainRow = new TblCostCenterRouteGroupViewModel();
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
                    var saveRow = new TblCostCenterRouteGroup();

                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblCostCenterRouteGroupsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
            Glclient.GetTblCostCenterRouteGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private TblCostCenterRouteGroupViewModel _selectedMainRow;

        public TblCostCenterRouteGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblCostCenterRouteGroupViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<TblCostCenterRouteGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblCostCenterRouteGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblCostCenterRouteGroupViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private ObservableCollection<TblCostCenterRouteGroupViewModel> _mainRowList;

        public ObservableCollection<TblCostCenterRouteGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        
        private ObservableCollection<CRUDManagerService.TblRouteGroup> _RouteGroupList;

        public ObservableCollection<CRUDManagerService.TblRouteGroup> RouteGroupList
        {
            get { return _RouteGroupList; }
            set
            {
                _RouteGroupList = value;
                RaisePropertyChanged("RouteGroupList");
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

    public class TblCostCenterRouteGroupViewModel : Web.DataLayer.PropertiesViewModelBase
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

        private int? _TblRouteGroup;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqRouteGroup")]
        public int? TblRouteGroup
        {
            get { return _TblRouteGroup; }
            set { _TblRouteGroup = value; RaisePropertyChanged("TblRouteGroup"); }
        }
        private CRUDManagerService.TblRouteGroup _RouteGroupPerRow;

        public CRUDManagerService.TblRouteGroup RouteGroupPerRow
        {
            get { return _RouteGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_RouteGroupPerRow, value) != true))
                {
                    _RouteGroupPerRow = value;
                    RaisePropertyChanged("RouteGroupPerRow");
                    if (_RouteGroupPerRow != null)
                    {
                        if (_RouteGroupPerRow.Iserial != 0)
                        {
                            TblRouteGroup = _RouteGroupPerRow.Iserial;
                        }
                    }
                }
            }
        }
    }
}