using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{

    public class TblBrandStoreTargetHeaderForManagmentViewModel : PropertiesViewModelBase
    {
      

        private int _monthField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMonth")]
        public int   Month
        {
            get
            {
                return _monthField;
            }
            set
            {
                if ((_monthField.Equals(value) != true))
                {
                    _monthField = value;
                    RaisePropertyChanged("Month");
                    
                }
            }
        }

        private int _brandAmount;


        public int BrandAmount
        {
            get
            {
                return _brandAmount;
            }
            set
            {
                if ((_brandAmount.Equals(value) != true))
                {
                    _brandAmount = value;
                    RaisePropertyChanged("BrandAmount");

                }
            }
        }

        private int _glserialField;

        private int? _tblItemDownLoadDefField;

        private TblItemDownLoadDef _tblItemDownLoadDef1Field;
        

        private int _yearField;
        
        

        public int Glserial
        {
            get
            {
                return _glserialField;
            }
            set
            {
                if ((_glserialField.Equals(value) != true))
                {
                    _glserialField = value;
                    RaisePropertyChanged("Glserial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public int? TblItemDownLoadDef
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

        public TblItemDownLoadDef ItemDownloadDefPerRow
        {
            get
            {
                return _tblItemDownLoadDef1Field;
            }
            set
            {
                if ((ReferenceEquals(_tblItemDownLoadDef1Field, value) != true))
                {
                    _tblItemDownLoadDef1Field = value;
                    RaisePropertyChanged("ItemDownloadDefPerRow");
                    if (ItemDownloadDefPerRow != null)
                    {
                            TblItemDownLoadDef = ItemDownloadDefPerRow.iserial;
             
                    }
                }
            }
        }
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqYear")]
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
    public class BrandStoreTargetManagmentViewModel : ViewModelBase
    {
        public BrandStoreTargetManagmentViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.BrandStoreTargetForManagement.ToString());
                MainRowList = new SortableCollectionView<TblBrandStoreTargetHeaderForManagmentViewModel>();
                SelectedMainRow = new TblBrandStoreTargetHeaderForManagmentViewModel();

                Client.GetTblBrandStoreTargetHeaderForManagmentCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBrandStoreTargetHeaderForManagmentViewModel();
                        newrow.ItemDownloadDefPerRow=new TblItemDownLoadDef();
                        newrow.ItemDownloadDefPerRow = row.TblItemDownLoadDef1;
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

                Client.UpdateOrInsertTblBrandStoreTargetHeaderCompleted += (s, x) =>
                {
                    var savedRow = (TblBrandStoreTargetHeaderForManagmentViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.GlSerial";

            Loading = true;
            Client.GetTblBrandStoreTargetHeaderForManagmentAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
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
            var newrow = new TblBrandStoreTargetHeaderForManagmentViewModel
            {
                Year = DateTime.Now.Year,
            };

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Glserial == 0;
                    var saveRow = new CRUDManagerService.TblBrandStoreTargetHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    Loading = true;
                    Client.UpdateOrInsertTblBrandStoreTargetHeaderAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),LoggedUserInfo.DatabasEname);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblBrandStoreTargetHeaderForManagmentViewModel> _mainRowList;

        public SortableCollectionView<TblBrandStoreTargetHeaderForManagmentViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblBrandStoreTargetHeaderForManagmentViewModel> _selectedMainRows;

        public ObservableCollection<TblBrandStoreTargetHeaderForManagmentViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBrandStoreTargetHeaderForManagmentViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _BrandStoreTargetManagmentViewModeGroupList;

        public ObservableCollection<GenericTable> BrandStoreTargetManagmentViewModeGroupList
        {
            get { return _BrandStoreTargetManagmentViewModeGroupList ?? (_BrandStoreTargetManagmentViewModeGroupList = new ObservableCollection<GenericTable>()); }
            set { _BrandStoreTargetManagmentViewModeGroupList = value; RaisePropertyChanged("BrandStoreTargetManagmentViewModeGroupList"); }
        }       

        private TblBrandStoreTargetHeaderForManagmentViewModel _selectedMainRow;

        public TblBrandStoreTargetHeaderForManagmentViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}