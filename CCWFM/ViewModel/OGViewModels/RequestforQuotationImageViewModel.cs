using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class RequestforSampleImageViewModel : ViewModelBase
    {
        private TblRequestForSampleViewModel _requestForSample;

        public TblRequestForSampleViewModel RequestForSample
        {
            get { return _requestForSample; }
            set
            {
                _requestForSample = value;
                RaisePropertyChanged("RequestForSample");
            }
        }

        public RequestforSampleImageViewModel(TblRequestForSampleViewModel requestForSample)
        {
            RequestForSample = requestForSample;
            MainRowList = new SortableCollectionView<TblStyleImageViewModel>();
            MainRowList.CollectionChanged += MainRowList_CollectionChanged;
            SelectedMainRow = new TblStyleImageViewModel();

            Client.GetTblRequestForSampleImageCompleted += (s, sv) =>
            {
                MainRowList.Clear();
                foreach (var row in sv.Result)
                {
                    var newrow = new TblStyleImageViewModel();
                    newrow.InjectFrom(row);
                    MainRowList.Add(newrow);
                }
                Loading = false;
            };

            Client.MaxIserialOfRequestForSampleImageCompleted += (s, sv) =>
            {
                FolderPath = "Uploads" + "/" + sv.imagePath;

                string folderName = FolderPath + "/" + RequestForSample.Code;

                var counter = 0;
                foreach (var item in MainRowList)
                {
                    if (item.Iserial == 0)
                    {
                        var maxIserial = sv.Result;

                        item.ImagePath = folderName + "/" + maxIserial + counter + ".png";
                        item.FolderPath = folderName;
                        item.UploadFile(item.ImagePerRow);

                        counter++;
                    }
                }

                var isvalid = false;

                foreach (var tblStyleImageViewModel in MainRowList)
                {
                    isvalid = Validator.TryValidateObject(tblStyleImageViewModel, new ValidationContext(tblStyleImageViewModel, null, null), null, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                if (isvalid)
                {
                    var savelist = new ObservableCollection<TblRequestForSampleImage>();
                    GenericMapper.InjectFromObCollection(savelist, MainRowList);

                    Client.UpdateOrInsertTblRequestForSampleImageAsync(savelist);
                }
            };
            Client.GetTblRequestForSampleImageAsync(RequestForSample.Iserial);
            Client.UpdateOrInsertTblRequestForSampleImageCompleted += (s, x) => Client.GetTblRequestForSampleImageAsync(RequestForSample.Iserial);

            Client.DeleteTblRequestForSampleImageCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    throw ev.Error;
                }

                var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
            };
        }

        private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblStyleImageViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblStyleImageViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "DefaultImage")
            {
                foreach (var tblStyleImageViewModel in MainRowList.Where(x => x != SelectedMainRow && x.DefaultImage))
                {
                    tblStyleImageViewModel.DefaultImage = false;
                }
            }
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        Client.DeleteTblRequestForSampleImageAsync(
                            (TblRequestForSampleImage)new TblRequestForSampleImage().InjectFrom(row), MainRowList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new TblStyleImageViewModel
                {
                    TblRequestForSample = RequestForSample.Iserial
                });
            }
        }

        #region Props

        private SortableCollectionView<TblStyleImageViewModel> _mainRowList;

        public SortableCollectionView<TblStyleImageViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblStyleImageViewModel> _selectedMainRows;

        public ObservableCollection<TblStyleImageViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStyleImageViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblStyleImageViewModel _selectedMainRow;

        public TblStyleImageViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private string _folderPath;

        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; RaisePropertyChanged("FolderPath"); }
        }

        #endregion Props

        internal void SaveImages()
        {
            Client.MaxIserialOfRequestForSampleImageAsync();
        }
    }
}