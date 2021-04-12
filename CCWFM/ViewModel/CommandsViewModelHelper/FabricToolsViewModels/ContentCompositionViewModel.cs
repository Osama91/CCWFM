using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class ContentCompositionViewModel : ViewModelBase
    {
        #region [ Fields and Properties ]

        public static ObservableCollection<int> SelectedContents = new ObservableCollection<int>();

        private ObservableCollection<GenericViewModel> _contentsList;

        public ObservableCollection<GenericViewModel> ContentsList
        {
            get { return _contentsList; }
            set
            {
                _contentsList = value;
                RaisePropertyChanged("ContentsList");
            }
        }

        private GenericViewModel _fabContent;

        public GenericViewModel FabContent
        {
            get { return _fabContent ?? (_fabContent = new GenericViewModel()); }
            set { _fabContent = value; RaisePropertyChanged("FabContent"); }
        }

        private int _fabContentId;

        public int FabContentID
        {
            get { return _fabContentId; }
            set
            {
                _fabContentId = value;
                RaisePropertyChanged("FabContentID");
            }
        }

        private int _contentCompositionSerial;

        public int ContentCompositionSerial
        {
            get { return _contentCompositionSerial; }
            set { _contentCompositionSerial = value; RaisePropertyChanged("ContentCompositionSerial"); }
        }

        private double _contentPercentage;

        public double ContentPercentage
        {
            get { return _contentPercentage; }
            set
            {
                _contentPercentage = Math.Round(value, 3);
                RaisePropertyChanged("ContentPercentage");
                if (_contentPercentage < 1)
                    throw new ValidationException("Value cannot be less than 1%");
            }
        }

        #endregion [ Fields and Properties ]

        #region [ Constructors ]

        public ContentCompositionViewModel()
        {
            ContentsList = new ObservableCollection<GenericViewModel>();
            ContentsList.CollectionChanged += GenericViewModel_CollectionChanged;
            FillAllFabContents();
            ContentPercentage = 1;
        }

        #endregion [ Constructors ]

        #region [ Private Methods ]

        private void FillAllFabContents()
        {
            Client.GetGenericAsync("tbl_lkp_Contents", "%%", "%%", "%%", "Iserial", "ASC");
            Client.GetGenericCompleted += (s, ev) =>
            {
                var i = 0;
                foreach (var item in ev.Result)
                {
                    if (!SelectedContents.Contains(item.Iserial))
                    {
                        ContentsList.Add(new GenericViewModel
                        {
                            Iserial = item.Iserial,
                            Code = item.Code,
                            Aname = item.Aname,
                            Ename = item.Ename
                        });
                        ContentsList[i].Status.IsChanged = false;
                        ContentsList[i].Status.IsNew = false;
                        ContentsList[i].Status.IsSavedDBItem = true;
                        i++;
                    }
                }
            };
        }

        #endregion [ Private Methods ]

        #region [ Event Handlers ]

        private void GenericViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (GenericViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (GenericViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion [ Event Handlers ]
    }
}