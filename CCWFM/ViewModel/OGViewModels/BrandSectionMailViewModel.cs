using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblBrandSectionMailViewModel : PropertiesViewModelBase
    {
        private EmployeesView _empPerRow;

        public EmployeesView EmpPerRow
        {
            get { return _empPerRow; }
            set
            {
                _empPerRow = value; RaisePropertyChanged("EmpPerRow");
                if (EmpPerRow != null) Emp = EmpPerRow.Emplid;
            }
        }

        private int _iserial;

        public int Iserial
        {
            get { return _iserial; }
            set { _iserial = value; RaisePropertyChanged("Iserial"); }
        }

        private string _emp;

        private int? _tblLkpBrandSectionField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrandSection")]
        public int? TblLkpBrandSection
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

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEmployee")]
        public string Emp
        {
            get
            {
                return _emp;
            }
            set
            {
                if ((ReferenceEquals(_emp, value) != true))
                {
                    _emp = value;
                    RaisePropertyChanged("Emp");
                }
            }
        }

        private string _tblBrand;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqBrand")]
        public string TblBrand
        {
            get { return _tblBrand; }
            set { _tblBrand = value; RaisePropertyChanged("TblBrand"); }
        }
    }

    public class BrandSectionMailViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        public BrandSectionMailViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
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
                MainRowList = new SortableCollectionView<TblBrandSectionMailViewModel>();
                SelectedMainRow = new TblBrandSectionMailViewModel();

                Client.GetTblBrandSectionMailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBrandSectionMailViewModel();
                        newrow.InjectFrom(row);

                        var newEmp = sv.EmpList.FirstOrDefault(x => x.EMPLID == newrow.Emp);

                        if (newEmp != null)
                            newrow.EmpPerRow = new EmployeesView
                            {
                                Emplid = newEmp.EMPLID,
                                Name = newEmp.name
                            };

                        MainRowList.Add(newrow);
                    }
                    Loading = false;

                    if (MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertTblBrandSectionMailCompleted += (s, x) =>
                {
                    var savedRow = (TblBrandSectionMailViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblBrandSectionMailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            MainRowList.Clear();

            Client.GetTblBrandSectionMailAsync(Brand, BrandSection);
        }

        private int _brandSection;

        public int BrandSection
        {
            get { return _brandSection; }
            set
            {
                _brandSection = value; RaisePropertyChanged("BrandSection");
                GetMaindata();
            }
        }

        private string _brand;

        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value; RaisePropertyChanged("Brand");
                lkpClient.GetTblBrandSectionLinkAsync(Brand, LoggedUserInfo.Iserial);
            }
        }

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList ?? (_brandList = new ObservableCollection<Brand>()); }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData. TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
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
                            Client.DeleteTblBrandSectionMailAsync(
                                (TblBrandSectionMail)new TblBrandSectionMail().InjectFrom(row), MainRowList.IndexOf(row));
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
                var newRow = new TblBrandSectionMailViewModel { TblBrand = Brand, TblLkpBrandSection = BrandSection };

                MainRowList.Insert(currentRowIndex + 1, newRow);
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
                    var saveRow = new TblBrandSectionMail();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblBrandSectionMailAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblBrandSectionMailViewModel> _mainRowList;

        public SortableCollectionView<TblBrandSectionMailViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblBrandSectionMailViewModel> _selectedMainRows;

        public ObservableCollection<TblBrandSectionMailViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBrandSectionMailViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBrandSectionMailViewModel _selectedMainRow;

        public TblBrandSectionMailViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}