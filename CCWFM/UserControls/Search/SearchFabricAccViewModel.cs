using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.UserControls.Search
{
    public class SearchFabricAccViewModel : ViewModelBase
    {
        public SearchFabricAccViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();
                
                MainRowList = new ObservableCollection<ItemsDto>();
                SelectedMainRow = new ItemsDto();

                Client.GetItemWithUnitAndItemGroupCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        foreach (var row in sv.Result)
                        {
                            //var newrow = new FabricAccSearch();

                            //newrow.InjectFrom(row);
                            var newrow = (ItemsDto)new ItemsDto().InjectFrom(row);//; row
                            if (!MainRowList.Contains(newrow))
                            {
                                //var fabric = sv.mainFabricList.FirstOrDefault(x => x.Iserial == newrow.tbl_FabricAttriputes);

                                //if (fabric != null)
                                //{
                                //    newrow.FabricPerRow = fabric;
                                //}
                                //newrow.SeasonPerRow = new GenericTable();
                                //newrow.SizeGroupPerRow = new TblSizeGroup();

                                MainRowList.Add(newrow);
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
            Client.GetItemWithUnitAndItemGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private ObservableCollection<ItemsDto> _mainRowList;

        public ObservableCollection<ItemsDto> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ItemsDto _selectedMainRow;

        public ItemsDto SelectedMainRow
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