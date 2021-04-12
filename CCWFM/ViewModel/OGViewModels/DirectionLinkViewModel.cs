using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblLkpDirectionLinkViewModel : GenericViewModel
    {
        private readonly LkpData.LkpDataClient client = new LkpData.LkpDataClient();
        private int _tblLkpDirection;

        public int TblLkpDirection
        {
            get
            {
                return _tblLkpDirection;
            }
            set
            {
                if ((_tblLkpDirection.Equals(value) != true))
                {
                    _tblLkpDirection = value;
                    RaisePropertyChanged("TblLkpDirection");
                }
            }
        }

        private string _tblBrandField;

        private int _tblLkpBrandSectionField;

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    TblLkpDirection = Iserial;
                    client.UpdateOrDeleteTblDirectionLinkAsync(
                        (LkpData.TblLkpDirectionLink)new LkpData.TblLkpDirectionLink().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        public string TblBrand
        {
            get
            {
                return _tblBrandField;
            }
            set
            {
                if ((ReferenceEquals(_tblBrandField, value) != true))
                {
                    _tblBrandField = value;
                    RaisePropertyChanged("TblBrand");
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

    public class DirectionLinkViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public DirectionLinkViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var client = new CRUD_ManagerServiceClient();

                Client.GetAllBrandsCompleted += (s, sv) =>
                {
                    BrandsList = sv.Result;
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
                client.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblLkpDirectionLinkViewModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code
                        };
                        MainRowList.Add(newrow);
                    }
                };
                lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
                {
                    BrandSectionList.Clear();
                    foreach (var row in sv.Result)
                    {
                        BrandSectionList.Add(row.TblLkpBrandSection1);
                    }
                };
                lkpClient.GetTblDirectionLinkCompleted += (s, sv) =>
                {
                    MainSelectedRows.Clear();
                    MainSelectedRows = sv.Result;

                    foreach (var row in MainRowList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                    }
                    foreach (var row in MainSelectedRows)
                    {
                        var familyRow = MainRowList.SingleOrDefault(x =>
                            x.Iserial == row.TblLkpDirection);
                        familyRow.Checked = true;
                    }
                };

                client.GetGenericAsync("TblLkpDirection", "%%", "%%", "%%", "Iserial", "ASC");
            }
        }

        #region Prop

        private ObservableCollection<Brand> _brandsList;

        public ObservableCollection<Brand> BrandsList
        {
            get { return _brandsList; }
            set { _brandsList = value; RaisePropertyChanged("BrandsList"); }
        }

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private string _brandCode;

        public string BrandCode
        {
            get { return _brandCode; }
            set
            {
                _brandCode = value;

                if (_brandCode != null)
                {
                    RaisePropertyChanged("BrandCode");
                    lkpClient.GetTblBrandSectionLinkAsync(BrandCode, LoggedUserInfo.Iserial);
                    if (BrandSection != 0)
                    {
                        lkpClient.GetTblDirectionLinkAsync(BrandCode, BrandSection);
                    }
                }
            }
        }

        private int _brandSection;

        public int BrandSection
        {
            get { return _brandSection; }
            set
            {
                _brandSection = value; RaisePropertyChanged("BrandSection");
                if (BrandCode != null)
                {
                    lkpClient.GetTblDirectionLinkAsync(BrandCode, BrandSection);
                }
            }
        }

        private TblLkpDirectionLinkViewModel _selectedMainRow;

        public TblLkpDirectionLinkViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<LkpData.TblLkpDirectionLink> _mainSelectedRows;

        public ObservableCollection<LkpData.TblLkpDirectionLink> MainSelectedRows
        {
            get { return _mainSelectedRows ?? (_mainSelectedRows = new ObservableCollection<LkpData.TblLkpDirectionLink>()); }
            set { _mainSelectedRows = value; RaisePropertyChanged("MainSelectedRows"); }
        }

        private ObservableCollection<TblLkpDirectionLinkViewModel> _directionList;

        public ObservableCollection<TblLkpDirectionLinkViewModel> MainRowList
        {
            get { return _directionList ?? (_directionList = new ObservableCollection<TblLkpDirectionLinkViewModel>()); }
            set { _directionList = value; RaisePropertyChanged("MainRowList"); }
        }

        #endregion Prop
    }
}