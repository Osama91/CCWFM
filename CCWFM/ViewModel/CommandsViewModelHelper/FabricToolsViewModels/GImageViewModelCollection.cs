using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.Enums;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class GImageViewModelCollection : Web.DataLayer.PropertiesViewModelBase
    {
        private string _fabricCode;
        private ObservableCollection<GImageViewModel> _imageCollection;

        private bool _isBusy;

        private ObservableCollection<_Proxy.tbl_FabricAttriputes> _fabricAttrCollection;

        /// <summary>
        /// For testing issues to keep track of the total size of loaded/uploaded images!
        /// </summary>
        private double _myVar;

        public GImageViewModelCollection()
        {
            ImageCollection = new ObservableCollection<GImageViewModel>();
            ImageCollection.CollectionChanged += ImageCollection_CollectionChanged;

            FabricAttrCollection = new ObservableCollection<_Proxy.tbl_FabricAttriputes>();
            FabricAttrCollection.CollectionChanged += FabricAttrCollection_CollectionChanged;

            FillFabricAttributes();
        }

        public ObservableCollection<_Proxy.tbl_FabricAttriputes> FabricAttrCollection
        {
            get { return _fabricAttrCollection; }
            set { _fabricAttrCollection = value; RaisePropertyChanged("FabricAttrCollection"); }
        }

        public string FabricCode
        {
            get { return _fabricCode; }
            set
            {
                _fabricCode = value; RaisePropertyChanged("FabricAttrCollection");
                ImageCollection.ToList().ForEach(x => x.G_FabricId = FabricCode);
            }
        }

        public bool g_IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; RaisePropertyChanged("g_IsBusy"); }
        }

        public ObservableCollection<GImageViewModel> ImageCollection
        {
            get
            {
                return _imageCollection;
            }
            set
            {
                _imageCollection = value;
                RaisePropertyChanged("ImageCollection");
            }
        }

        public double MyProperty
        {
            get { return _myVar; }
            set { _myVar = value; RaisePropertyChanged("MyProperty"); }
        }

        public void LoadImages(string fabCode)
        {
            ImageCollection = new ObservableCollection<GImageViewModel>();
            g_IsBusy = true;
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetFabricImagesByFabricCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        foreach (var item in e.Result)
                        {
                            ImageCollection
                                .Add(FabricSetupsViewModelMapper
                                .MapToViewModelObject(item, fabCode));
                        }
                        g_IsBusy = false;
                    }
                    RaisePropertyChanged("ImageCollection");
                    g_IsBusy = false;
                };
            client.GetFabricImagesByFabricAsync(fabCode);
        }

        public void SaveImages()
        {
            if (ImageCollection.Count > 0)
            {
                g_IsBusy = true;
                var newImages = new ObservableCollection<_Proxy.tbl_FabricImage>();
                var updatedImages = new ObservableCollection<_Proxy.tbl_FabricImage>();

                foreach (var item in ImageCollection)
                {
                    if (item._ImageState == ImageCondition.NewAdded)
                    {
                        newImages.Add(FabricSetupsViewModelMapper.MapToModelObject(item));
                    }
                    else if (item._ImageState == ImageCondition.LoadedAndChanged)
                    {
                        updatedImages.Add(FabricSetupsViewModelMapper.MapToModelObject(item));
                    }
                }
                var client = new _Proxy.CRUD_ManagerServiceClient();
                if (newImages.Count > 0)
                {
                    client.AddFabricGalaryCompleted += (s, e) =>
                        {
                            if (e.Error != null)
                            {
                                g_IsBusy = false;
                                throw new Exception("Error!, Data was not added\n" + e.Error.Message);
                            }
                            else
                            {
                                MessageBox.Show("Data was added successfully");
                                g_IsBusy = false;
                            }
                            g_IsBusy = false;
                        };
                    client.AddFabricGalaryAsync(newImages);
                }
                if (updatedImages.Count > 0)
                {
                    client.UpdateFabricGalaryCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            g_IsBusy = false;
                            throw new Exception("Error!, Data was not Updated\n" + e.Error.Message);
                        }
                        else
                        {
                            MessageBox.Show("Data was updated successfully");
                            g_IsBusy = false;
                        }
                        g_IsBusy = false;
                    };
                    client.UpdateFabricGalaryAsync(updatedImages);
                }
            }
            else
            {
                MessageBox.Show("data are not valid for saving!");
            }
        }

        private void FabricAttrCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (_Proxy.tbl_FabricAttriputes item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (_Proxy.tbl_FabricAttriputes item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void FillFabricAttributes()
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetAllFabAttributesCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    foreach (var item in e.Result)
                    {
                        FabricAttrCollection.Add(item);
                    }
                }
            };
            client.GetAllFabAttributesAsync();
            client.CloseAsync();
        }

        private void ImageCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (GImageViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (GImageViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}