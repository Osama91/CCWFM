using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class FabricSetupsWFViewModelCollection : ViewModelBase
    {
        #region [ Fields and Properties ]

        private ObservableCollection<FabricSetupsWFViewModel> _fabricAtrrViewModelList;

        public ObservableCollection<FabricSetupsWFViewModel> FabricAtrrViewModelList
        {
            get { return _fabricAtrrViewModelList; }
            set
            {
                _fabricAtrrViewModelList = value;
                RaisePropertyChanged("FabricAtrrViewModelList");
            }
        }

        private FabricSetupsWFViewModel _selectedFabricSetup;

        public FabricSetupsWFViewModel SelectedFabricSetup
        {
            get { return _selectedFabricSetup; }
            set { _selectedFabricSetup = value; }
        }

        #endregion [ Fields and Properties ]

        #region [ Constructors ]

        public FabricSetupsWFViewModelCollection()
        {
            FabricAtrrViewModelList = new ObservableCollection<FabricSetupsWFViewModel>();
            FabricAtrrViewModelList.CollectionChanged += FabricAtrrViewModelList_CollectionChanged;
            //   FillAllFabAtrr();
        }

        #endregion [ Constructors ]

        #region [ Events Handelrs ]

        private void FabricAtrrViewModelList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (FabricSetupsWFViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (FabricSetupsWFViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion [ Events Handelrs ]

        //#region Public Mehtods

        //public void FillAllFabAtrr()
        //{
        //    var client = new _Proxy.CRUD_ManagerServiceClient();
        //    client.GetAllFabAttributesCompleted += (s, e) =>
        //        {
        //            if (e.Result.Count > 0)
        //            {
        //                foreach (var item in e.Result)
        //                {
        //                    var temp = new FabricSetupsViewModel();
        //                    FabricSetupsViewModelMapper.MapToViewModelObject(temp, item);
        //                    FabricAtrrViewModelList.Add(temp);
        //                }
        //            }
        //        };
        //    client.GetAllFabAttributesAsync();
        //}

        //#endregion Public Mehtods
    }
}