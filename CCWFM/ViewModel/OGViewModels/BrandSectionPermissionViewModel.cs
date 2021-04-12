using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblBrandSectionPermissionViewModel : PropertiesViewModelBase
    {
        private int _tblAuthPermissionField;

        private string _ename;

        public string Ename
        {
            get { return _ename; }
            set { _ename = value; RaisePropertyChanged("Ename"); }
        }

        public int TblAuthPermission
        {
            get { return _tblAuthPermissionField; }
            set
            {
                if ((_tblAuthPermissionField.Equals(value) != true))
                {
                    _tblAuthPermissionField = value;
                    RaisePropertyChanged("TblAuthPermission");
                }
            }
        }

        private string _brandCodeField;

        private bool _financialField;

        private int _iserialField;

        private bool _retailField;

        private int _tblLkpBrandSectionField;

        private bool _technicalField;

        public string BrandCode
        {
            get
            {
                return _brandCodeField;
            }
            set
            {
                if ((ReferenceEquals(_brandCodeField, value) != true))
                {
                    _brandCodeField = value;
                    RaisePropertyChanged("BrandCode");
                }
            }
        }

        public bool Financial
        {
            get
            {
                return _financialField;
            }
            set
            {
                if ((_financialField.Equals(value) != true))
                {
                    _financialField = value;
                    RaisePropertyChanged("Financial");
                }
            }
        }

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

        public bool Retail
        {
            get
            {
                return _retailField;
            }
            set
            {
                if ((_retailField.Equals(value) != true))
                {
                    _retailField = value;
                    RaisePropertyChanged("Retail");
                }
            }
        }

        public int TblLkpBrandSection
        {
            get
            {
                return _tblLkpBrandSectionField;
            }
            set
            {
                if ((_tblLkpBrandSectionField.Equals(value) != true))
                {
                    _tblLkpBrandSectionField = value;
                    RaisePropertyChanged("TblLkpBrandSection");
                }
            }
        }

        public bool Technical
        {
            get
            {
                return _technicalField;
            }
            set
            {
                if ((_technicalField.Equals(value) != true))
                {
                    _technicalField = value;
                    RaisePropertyChanged("Technical");
                }
            }
        }
    }

    public class BrandSectionPermissionViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        public BrandSectionPermissionViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;
            Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

            Client.GetAllBrandsCompleted += (d, s) =>
            {
                BrandList = s.Result;
            };

            lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
            {
                BrandSectionList.Clear();
                foreach (var row in sv.Result)
                {
                    BrandSectionList.Add(row.TblLkpBrandSection1);
                }
            };

            Client.GetUserSpectialPermissionsAsync();
            Client.GetUserSpectialPermissionsCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    PermissionList.Add(new TblBrandSectionPermissionViewModel
                    {
                        TblAuthPermission = row.Iserial,
                        Ename = row.Ename,
                    });
                }
            };

            Client.GetTblBrandSectionPermissionCompleted += (s, sv) =>
            {
                foreach (var oldrow in PermissionList)
                {
                    oldrow.Iserial = 0;
                    oldrow.Financial = oldrow.Retail = oldrow.Technical = false;
                }

                foreach (var row in sv.Result)
                {
                    var permissionRow = PermissionList.SingleOrDefault(x => x.TblAuthPermission == row.TblAuthPermission && row.BrandCode == SelectedBrand.Brand_Code && row.TblLkpBrandSection == SelectedBrandSection.Iserial
                        );
                    if (permissionRow != null) permissionRow.InjectFrom(row);
                }
            };

            Client.UpdateOrInsertTblBrandSectionPermissionCompleted += (s, x) =>
            {
                var savedRow = (TblBrandSectionPermissionViewModel)PermissionList.GetItemAt(x.outindex);

                if (savedRow != null) savedRow.InjectFrom(x.Result);
            };

            Client.DeleteTblBrandSectionPermissionCompleted += (s, ev) =>
            {
                var oldrow = PermissionList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null)
                {
                    oldrow.Iserial = 0;
                    oldrow.Financial = oldrow.Retail = oldrow.Technical = false;
                }
            };
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
                            Client.DeleteTblBrandSectionPermissionAsync(
                                (TblBrandSectionPermission)new TblBrandSectionPermission().InjectFrom(row), PermissionList.IndexOf(row));
                        }
                        else
                        {
                            row.Iserial = 0;
                            row.Financial = row.Retail = row.Technical = false;
                        }
                    }
                }
            }
        }

        internal void GetPermission()
        {
            if (SelectedBrandSection != null)
                Client.GetTblBrandSectionPermissionAsync(SelectedBrand.Brand_Code, SelectedBrandSection.Iserial);
        }

        public void SaveMainRow()
        {
            if (SelectedBrandSectionPermission != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedBrandSectionPermission, new ValidationContext(SelectedBrandSectionPermission, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedBrandSectionPermission.Iserial == 0;
                    var saveRow = new TblBrandSectionPermission();
                    saveRow.InjectFrom(SelectedBrandSectionPermission);

                    saveRow.BrandCode = SelectedBrand.Brand_Code;
                    saveRow.TblLkpBrandSection = SelectedBrandSection.Iserial;

                    Client.UpdateOrInsertTblBrandSectionPermissionAsync(saveRow, save, PermissionList.IndexOf(SelectedBrandSectionPermission));
                }
            }
        }

        #region Prop

        private TblBrandSectionPermissionViewModel _selctedBrandSectionPermission;

        public TblBrandSectionPermissionViewModel SelectedBrandSectionPermission
        {
            get { return _selctedBrandSectionPermission; }
            set
            {
                _selctedBrandSectionPermission = value; RaisePropertyChanged("SelectedBrandSectionPermission");
            }
        }

        private ObservableCollection<TblBrandSectionPermissionViewModel> _selectedMainRows;

        public ObservableCollection<TblBrandSectionPermissionViewModel> SelectedMainRows
        {
            get { return _selectedMainRows; }
            set
            {
                _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows");
            }
        }

        private SortableCollectionView<TblBrandSectionPermissionViewModel> _permissionList;

        public SortableCollectionView<TblBrandSectionPermissionViewModel> PermissionList
        {
            get { return _permissionList ?? (_permissionList = new SortableCollectionView<TblBrandSectionPermissionViewModel>()); }
            set { _permissionList = value; RaisePropertyChanged("PermissionList"); }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private TblLkpBrandSection _selectedBrandSection;

        public TblLkpBrandSection SelectedBrandSection
        {
            get { return _selectedBrandSection; }
            set
            {
                _selectedBrandSection = value; RaisePropertyChanged("SelectedBrandSection");
                GetPermission();
            }
        }

        private Brand _selectedBrand;

        public Brand SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                _selectedBrand = value;

                if (_selectedBrand != null)
                {
                    RaisePropertyChanged("SelectedBrand");
                    lkpClient.GetTblBrandSectionLinkAsync(SelectedBrand.Brand_Code, LoggedUserInfo.Iserial);
                }
            }
        }

        #endregion Prop
    }
}