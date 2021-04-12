using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class SmlHeaderViewModel : ViewModelBase
    {
        #region [ Events ]

        #endregion [ Events ]

        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        private List<Brand> _brands;
        private List<GenericTable> _seasons;

        private Brand _selectedBrand;

        private TblLkpSeason _selectedSeason;

        private SeasonalMasterCollection _smlCollection;

        private List<GenericViewModel> _statusList;

        public List<GenericViewModel> StatusList
        {
            get { return _statusList ?? (_statusList = new List<GenericViewModel>()); }
            set
            {
                _statusList = value;
                RaisePropertyChanged("StatusList");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Brand", Description = "Brand")]
        public List<Brand> Brands
        {
            get { return _brands ?? (_brands = new List<Brand>()); }
            set
            {
                _brands = value;
                RaisePropertyChanged("Brands");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Season", Description = "Season")]
        public List<GenericTable> Seasons
        {
            get { return _seasons ?? (_seasons = new List<GenericTable>()); }
            set
            {
                _seasons = value;
                RaisePropertyChanged("Seasons");
            }
        }

        public Brand SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                if (value == null || _selectedBrand == value) return;
                _selectedBrand = value;
                RaisePropertyChanged("SelectedBrand");
            }
        }

        public TblLkpSeason SelectedSeason
        {
            get { return _selectedSeason; }
            set
            {
                if (value == null || _selectedSeason == value) return;
                _selectedSeason = value;
                RaisePropertyChanged("SelectedSeason");
            }
        }

        public SeasonalMasterCollection SmlCollection
        {
            get { return _smlCollection ?? (_smlCollection = new SeasonalMasterCollection()); }
            set { _smlCollection = value; RaisePropertyChanged("SmlCollection"); }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public SmlHeaderViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                SmlCollection = new SeasonalMasterCollection();
                SmlCollection.OnRefresh += MainRowList_OnRefresh;
                Client = new CRUD_ManagerServiceClient();
                Client.GetAllBrandsCompleted += (s, e) =>
                {
                    try
                    {
                        Brands = new List<Brand>(e.Result);
                    }
                    catch (Exception ex)
                    {
                        var err = new ErrorWindow(ex);
                        err.Show();
                    }
                };
                Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

                var seasonClientclient = new CRUD_ManagerServiceClient();
                seasonClientclient.GetGenericCompleted += (s, e) =>
                {
                    Seasons = new List<GenericTable>(e.Result);
                };
                seasonClientclient.GetGenericAsync("TblLkpSeason", "%%", "%%", "%%", "Iserial", "ASC");

                FillGenericCollection("tbl_lkp_SMLStatus", StatusList);
                Client.GetSmlCompleted += (s, e) =>
                    {
                        Loading = false;
                        foreach (var item in e.Result)
                        {
                            //                            SmlCollection.Add(MapItem(item));
                        }
                    };

                GetMaindata();
            }
            else
            {
                SmlCollection.Add(new SeasonalMasterListViewModel { StyleCode = "Test Style", Description = "Test Desc", ColorCode = "BL1" });
                SmlCollection.Add(new SeasonalMasterListViewModel { StyleCode = "2Test Style", Description = "Test Desc2", ColorCode = "BL2" });
            }
        }

        //private SeasonalMasterListViewModel MapItem(tblSeasonalMasterList item)
        //{
        //    var tmp = new SeasonalMasterListViewModel
        //        {
        //            Iserial = item.Iserial,
        //            BrandCode = item.BrandCode,
        //            SeasonCode = item.SeasonCode,
        //            Qty = (int)item.Qty,
        //            ColorCode = item.ColorCode,
        //            SizeRange = item.SizeRange,
        //            StyleCode = item.StyleCode,
        //            CraetionDate = item.CreationDate,
        //            DeliveryDate = (DateTime)item.DelivaryDate,
        //            Description = item.Description,
        //            SelectedStatus = StatusList.FirstOrDefault(x => x.Iserial == item.StatusID),
        //            ObjStatus = new ObjectStatus { IsNew = false, IsEmpty = false, IsSavedDBItem = true },

        //            ParentObj = this
        //        };
        //    foreach (var detail in item.tblSeasonalMasterListDetails)
        //    {
        //        tmp.Sizes.Add(MapDetailItem(detail));
        //    }
        //    return tmp;
        //}

        //private SmlSizeDetails MapDetailItem(tblSeasonalMasterListDetail detail)
        //{
        //    var tmp = new SmlSizeDetails
        //        {
        //            Iserial = detail.Iserial,
        //            SizeRatio = (decimal)detail.Ratio,
        //            SizeCode = detail.Size,
        //            ParentSerial = detail.ParentSerial
        //        };
        //    return tmp;
        //}

        #endregion [ Constructors ]

        #region [ Commands ]

        private CommandsExecuter _newDetailCommand;

        public CommandsExecuter NewDetailCommand
        {
            get { return _newDetailCommand ?? (_newDetailCommand = new CommandsExecuter(NewDetail) { IsEnabled = true }); }
            set { _newDetailCommand = value; }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void NewDetail()
        {
            if (SmlCollection.Any())
            {
                var valiationCollection = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(SmlCollection[SmlCollection.Count - 1],
                       new ValidationContext(SmlCollection[SmlCollection.Count - 1], null, null), valiationCollection, true);
                if (!isvalid) return;
            }
            SmlCollection.Add(new SeasonalMasterListViewModel
                {
                    ParentObj = this,
                    SeasonCode = SelectedSeason != null ? SelectedSeason.Code : null,
                    BrandCode = SelectedBrand != null ? SelectedBrand.Brand_Code : null,
                    ObjStatus = new ObjectStatus { IsNew = true, IsEmpty = true, IsSavedDBItem = false }
                });
        }

        #endregion [ Commands bound methods ]

        #region [ Internal Logic ]

        private static void FillGenericCollection(string tablEname, IList<GenericViewModel> objectToFill)
        {
            var serviceClient = new CRUD_ManagerServiceClient();
            serviceClient.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            serviceClient.GetGenericCompleted += (s, ev) =>
            {
                int i = 0;
                foreach (var item in ev.Result)
                {
                    objectToFill.Add(new GenericViewModel
                    {
                        Iserial = item.Iserial,
                        Code = item.Code,
                        Aname = item.Aname,
                        Ename = item.Ename
                    });
                    objectToFill[i].Status.IsChanged = false;
                    objectToFill[i].Status.IsNew = false;
                    objectToFill[i].Status.IsSavedDBItem = true;
                    i++;
                }
            };
            serviceClient.CloseAsync();
        }

        private void MainRowList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                SmlCollection.Clear();
                SortBy = null;
                foreach (var sortDesc in SmlCollection.SortDescriptions)
                {
                    SortBy = SortBy + "it." + sortDesc.PropertyName + (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        #endregion [ Internal Logic ]

        public void GetMaindata()

        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            //  Client.GetSmlAsync(SmlCollection.Count, 20, SortBy, Filter, ValuesObjects);
        }
    }
}