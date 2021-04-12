using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblServiceViewModel : GenericViewModel
    {
        private string _tblLkpServiceGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqServiceGroup")]
        public string ServiceGroup
        {
            get
            {
                return _tblLkpServiceGroup;
            }
            set
            {
                if ((ReferenceEquals(_tblLkpServiceGroup, value) != true))
                {
                    _tblLkpServiceGroup = value;
                    RaisePropertyChanged("ServiceGroup");
                }
            }
        }

        private double _defaultPrice;

        public double DefaultPrice
        {
            get { return _defaultPrice; }
            set { _defaultPrice = value; RaisePropertyChanged("DefaultPrice");}
        }
        
    }

    public class ServiceCodingViewModel : ViewModelBase
    {
        public ServiceCodingViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblServiceViewModel>();
                SelectedMainRow = new TblServiceViewModel();

                var tblAssetsTypeclient = new CRUD_ManagerServiceClient();
                tblAssetsTypeclient.GetGenericAsync("TblServiceGroup", "%%", "%%", "%%", "Iserial", "ASC");

                tblAssetsTypeclient.GetGenericCompleted += (s, sv) =>
                {
                    ServiceGroupList = sv.Result;
                };

                Client.GetTblServiceCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblServiceViewModel();

                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertTblServiceCompleted += (s, x) =>
                {
                    var savedRow = (TblServiceViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblServiceCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblServiceAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Client.DeleteTblServiceAsync(
                                (TblService)new TblService().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
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

                MainRowList.Insert(currentRowIndex + 1, new TblServiceViewModel());
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
                    var saveRow = new TblService();                    
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblServiceAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Iserial);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblServiceViewModel> _mainRowList;

        public SortableCollectionView<TblServiceViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblServiceViewModel> _selectedMainRows;

        public ObservableCollection<TblServiceViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblServiceViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _serviceGroupList;

        public ObservableCollection<GenericTable> ServiceGroupList
        {
            get { return _serviceGroupList ?? (_serviceGroupList = new ObservableCollection<GenericTable>()); }
            set { _serviceGroupList = value; RaisePropertyChanged("ServiceGroupList"); }
        }



        private TblServiceViewModel _selectedMainRow;

        public TblServiceViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}