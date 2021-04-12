using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;

namespace CCWFM.UserControls.Search
{
    public class SearchStyleViewModel : ViewModelBase
    {
        public SearchStyleViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new ObservableCollection<TblStyle>();
                SelectedMainRow = new TblStyle();

                Client.GetTblStyleCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            //var newrow = new TblStyle();

                            //newrow.InjectFrom(row);
                            if (!MainRowList.Contains(row))
                            {
                                //var fabric = sv.mainFabricList.FirstOrDefault(x => x.Iserial == newrow.tbl_FabricAttriputes);

                                //if (fabric != null)
                                //{
                                //    newrow.FabricPerRow = fabric;
                                //}
                                //newrow.SeasonPerRow = new GenericTable();
                                //newrow.SizeGroupPerRow = new TblSizeGroup();

                                MainRowList.Add(row);
                            }
                        }
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblStyleAsync(MainRowList.Count, PageSize, 1, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial,"","",null,null,null,null,null);
        }

        #region Prop

        private ObservableCollection<TblStyle> _mainRowList;

        public ObservableCollection<TblStyle> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private TblStyle _selectedMainRow;

        public TblStyle SelectedMainRow
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
}