using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblBrandSectionFamilySizeGroupViewModelViewModel : PropertiesViewModelBase
    {

        private string _ename;

        public string Ename
        {
            get { return _ename; }
            set { _ename = value; RaisePropertyChanged("Ename"); }
        }


        private int _iserialField;
        private string _brandCodeField;
        private int _tblLkpBrandSectionField;

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

    }

    public class BrandSectionFamilySizeGroupViewModelViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public BrandSectionFamilySizeGroupViewModelViewModel()
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
            lkpClient.GetTblFamilyLinkCompleted += (s, sv) =>
            {
                FamilyList.Clear();
                foreach (var row in sv.Result)
                {
                    FamilyList.Add(row.TblFamily1);
                }
            };
        }
        public void SaveMainRow()
        {
            CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

            _client.SaveBrandSectionFamilySizeGroupAsync(SelectedBrand.Iserial, SelectedSection.Iserial,
                SelectedFamily.Iserial,
                SelectedSizeGroup.Iserial);
            _client.SaveBrandSectionFamilySizeGroupCompleted += (s, sv) =>
            {
                switch (sv.Result)
                {
                    case 1:
                        MessageBox.Show("Operation Completed");
                        break;
                    case 2:
                        MessageBox.Show("Already Exist");
                        break;
                    default:
                        MessageBox.Show("Unexpected error, Operation Canceled");
                        break;
                }
            };
        }

        #region Prop

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

        private ObservableCollection<LkpData.TblFamily> _familyList;

        public ObservableCollection<LkpData.TblFamily> FamilyList
        {
            get { return _familyList ?? (_familyList = new ObservableCollection<LkpData.TblFamily>()); }
            set { _familyList = value; RaisePropertyChanged("FamilyList"); }
        }
        private ObservableCollection<TblSizeGroup> _sizeGroupList;

        public ObservableCollection<TblSizeGroup> SizeGroupList
        {
            get { return _sizeGroupList ?? (_sizeGroupList = new ObservableCollection<TblSizeGroup>()); }
            set { _sizeGroupList = value; RaisePropertyChanged("SizeGroupList"); }
        }

        private LkpData.TblLkpBrandSection _selectedSection;
        public LkpData.TblLkpBrandSection SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value; RaisePropertyChanged("SelectedSection");
                lkpClient.GetTblFamilyLinkAsync(SelectedBrand.Brand_Ename, SelectedSection.Iserial);
                CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
                _client.FamilyCategory_GetTblSizeGroupLinkCompleted += (s, sv) =>
                {
                    SizeGroupList.Clear();
                    foreach (var row in sv.Result)
                    {
                        SizeGroupList.Add(row.TblSizeGroup1);
                    }
                };
                _client.FamilyCategory_GetTblSizeGroupLinkAsync(SelectedBrand.Brand_Ename, SelectedSection.Iserial.ToString());
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

        private LkpData.TblFamily _selectedFamily;
        public LkpData.TblFamily SelectedFamily
        {
            get { return _selectedFamily; }
            set
            {
                _selectedFamily = value;

                if (_selectedFamily != null)
                {
                    RaisePropertyChanged("SelectedFamily");
                    // lkpClient.GetTblSizeGroupLinkAsync(SelectedBrand.Brand_Ename, SelectedSection.Iserial);
                }
            }
        }
        private TblSizeGroup _selectedSizeGroup;
        public TblSizeGroup SelectedSizeGroup
        {
            get { return _selectedSizeGroup; }
            set
            {
                _selectedSizeGroup = value;
                if (_selectedSizeGroup != null)
                {
                    RaisePropertyChanged("SelectedSizeGroup");
                }
            }
        }
        #endregion Prop
    }
}
