using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel
{
    public class GenericItemSearchViewModel : ViewModelBase
    {
        #region Properties

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();

        public string DataArea { get; set; }

        private string _itemGroup;

        public string ItemGroup
        {
            get { return _itemGroup; }
            set { _itemGroup = value; RaisePropertyChanged("ItemGroup"); }
        }

        private int _itemType;

        public int ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
                RaisePropertyChanged("ItemType");
                CheckItemTypes();
            }
        }

        private ObservableCollection<CRUD_ManagerServiceAxItem> _itemList;

        public ObservableCollection<CRUD_ManagerServiceAxItem> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; RaisePropertyChanged("ItemList"); }
        }

        private ObservableCollection<INVENTDIM> _itemDetailsList;

        public ObservableCollection<INVENTDIM> ItemDetailsList
        {
            get { return _itemDetailsList; }
            set { _itemDetailsList = value; RaisePropertyChanged("ItemDetailsList"); }
        }

        private ObservableCollection<GenericViewModel> _genericViewModel;

        public ObservableCollection<GenericViewModel> GenericViewModelList
        {
            get { return _genericViewModel; }
            set { _genericViewModel = value; RaisePropertyChanged("GenericViewModelList"); }
        }

        private bool _mainGroupEnabled;

        public bool MainGroupEnabled
        {
            get { return _mainGroupEnabled; }
            set { _mainGroupEnabled = value; RaisePropertyChanged("MainGroupEnabled"); }
        }

        private string _itemid;

        public string ItemId
        {
            get { _itemid = _itemid ?? ""; return _itemid; }
            set
            {
                _itemid = value; RaisePropertyChanged("ItemId");
            }
        }

        private string _itemDescription;

        public string ItemDescription
        {
            get { _itemDescription = _itemDescription ?? ""; return _itemDescription; }
            set
            {
                _itemDescription = value; RaisePropertyChanged("ItemDescription");
            }
        }

        private bool _subGroupEnabled;

        public bool SubGroupEnabled
        {
            get { return _subGroupEnabled; }
            set { _subGroupEnabled = value; RaisePropertyChanged("SubGroupEnabled"); }
        }

        private void CheckItemTypes()
        {
            switch (_itemType)
            {
                case 0:

                    MainGroupEnabled = true;
                    SubGroupEnabled = false;
                    ItemGroup = "Fabric";
                    FillGenericCollection("tbl_FabricCategories", GenericViewModelList);
                    break;

                case 1:
                    MainGroupEnabled = true;
                    SubGroupEnabled = true;
                    ItemGroup = "ACCESSORIEs";
                    FillGenericCollection("tbl_lkp_AccessoryGroup", GenericViewModelList);
                    break;

                case 2:
                    MainGroupEnabled = false;
                    _mainGroupEnabled = false;
                    SubGroupEnabled = false;
                    ItemGroup = "Srv";
                    GenericViewModelList.Clear();
                    break;
            }
        }

        #endregion Properties

        public GenericItemSearchViewModel()
        {
            GenericViewModelList = new ObservableCollection<GenericViewModel>();

            ItemType = 0;

            DataArea = "ccm";
            _webService.SearchedAxItemsCompleted += (s, ev) =>
            {
                ItemList = new ObservableCollection<CRUD_ManagerServiceAxItem>();

                ItemList = ev.Result;
            };

            _webService.SearchedAxItemsDetailsCompleted += (s, d) =>
            {
                ItemDetailsList = new ObservableCollection<INVENTDIM>();
                ItemDetailsList = d.Result;
                //       DgItemDetails.ItemsSource = d.Result;
            };
        }

        private void FillGenericCollection(string tablEname, ObservableCollection<GenericViewModel> objectToFill)
        {
            objectToFill.Clear();
            var client = new CRUD_ManagerServiceClient();
            client.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            client.GetGenericCompleted += (s, ev) =>
            {
                foreach (var item in ev.Result)
                {
                    objectToFill.Add(new GenericViewModel
                    {
                        Iserial = item.Iserial,
                        Code = item.Code,
                        Aname = item.Aname,
                        Ename = item.Ename
                    });
                }
            };
            client.CloseAsync();
        }

        public void Search()
        {
            _webService.SearchedAxItemsAsync(DataArea, ItemGroup, ItemId, ItemDescription);
        }

        public void SearchedAxItemsDetails(CRUD_ManagerServiceAxItem selectedItem)
        {
            _webService.SearchedAxItemsDetailsAsync(DataArea, selectedItem.ItemId);
        }
    }
}