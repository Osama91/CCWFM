using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class FabricSetupsViewModelCollection : ViewModelBase
    {
        #region [ Fields and Properties ]

        private ObservableCollection<FabricSetupsViewModel> _fabricAtrrViewModelList;

        public ObservableCollection<FabricSetupsViewModel> FabricAtrrViewModelList
        {
            get { return _fabricAtrrViewModelList; }
            set
            {
                _fabricAtrrViewModelList = value;
                RaisePropertyChanged("FabricAtrrViewModelList");
            }
        }

        private FabricSetupsViewModel _selectedFabricSetup;

        public FabricSetupsViewModel SelectedFabricSetup
        {
            get { return _selectedFabricSetup; }
            set { _selectedFabricSetup = value; }
        }

        #endregion [ Fields and Properties ]

        #region [ Constructors ]

        public FabricSetupsViewModelCollection()
        {
            FabricAtrrViewModelList = new ObservableCollection<FabricSetupsViewModel>();
            FabricAtrrViewModelList.CollectionChanged += FabricAtrrViewModelList_CollectionChanged;
            //   FillAllFabAtrr();
        }

        #endregion [ Constructors ]

        #region [ Events Handelrs ]

        private void FabricAtrrViewModelList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (FabricSetupsViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (FabricSetupsViewModel item in e.OldItems)
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