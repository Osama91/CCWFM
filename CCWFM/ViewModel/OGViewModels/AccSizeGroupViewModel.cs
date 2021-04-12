using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblAccSizeGroupViewModel : GenericViewModel
    {
        private SortableCollectionView<TblAccSizeCodeViewModel> _tblAccSizeGroupField;

        public SortableCollectionView<TblAccSizeCodeViewModel> DetailsList
        {
            get
            {
                return _tblAccSizeGroupField ?? (_tblAccSizeGroupField = new SortableCollectionView<TblAccSizeCodeViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblAccSizeGroupField, value) != true))
                {
                    _tblAccSizeGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }
    }

    public class TblAccSizeCodeViewModel : PropertiesViewModelBase
    {
        private int _accSizeCodeAccSizeGroup;
        private string _accSizeCodeAccSizeCode;
        private int _iserialField;

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int TblAccSizeGroup
        {
            get
            {
                return _accSizeCodeAccSizeGroup;
            }
            set
            {
                if ((Equals(_accSizeCodeAccSizeGroup, value) != true))
                {
                    _accSizeCodeAccSizeGroup = value;
                    RaisePropertyChanged("TblAccSizeGroup");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqSize")]
        public string SizeCode
        {
            get
            {
                return _accSizeCodeAccSizeCode;
            }
            set
            {
                if ((ReferenceEquals(_accSizeCodeAccSizeCode, value) != true))
                {
                    if (value != null) _accSizeCodeAccSizeCode = value.Trim();
                    RaisePropertyChanged("SizeCode");
                }
            }
        }
    }

    #endregion ViewModels

    public class AccSizeGroupViewModel : ViewModelBase
    {
        public AccSizeGroupViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblAccSizeGroupViewModel>();
                SelectedMainRow = new TblAccSizeGroupViewModel();
                SelectedDetailRow = new TblAccSizeCodeViewModel();
                



                Client.GetTblAccSizeGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccSizeGroupViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                Client.GetTblAccSizeCodeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAccSizeCodeViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    if (sv.Result.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                };
                Client.UpdateOrInsertTblAccSizeGroupCompleted += (s, x) =>
                {
                    var savedRow = (TblAccSizeGroupViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblAccSizeCodeCompleted += (s, x) =>
                {
                    var savedRow = (TblAccSizeCodeViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                    //if (x.Result.TblSalaryRelation1 != null)
                    //{
                    //    var headerIserial = x.Result.TblSalaryRelation;

                    //    SelectedMainRow.Iserial = headerIserial;
                    //}
                };

                Client.DeleteTblAccSizeGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblAccSizeCodeCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Client.GetTblAccSizeGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                        Client.DeleteTblAccSizeGroupAsync(
                            (TblAccSizeGroup)new TblAccSizeGroup().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblAccSizeGroupViewModel());
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var saveRow = new TblAccSizeGroup();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblAccSizeGroupAsync(saveRow, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                Client.GetTblAccSizeCodeAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial,
                    DetailSortBy, DetailFilter, DetailValuesObjects);
            }
        }

        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedDetailRows)
                    {
                        Client.DeleteTblAccSizeCodeAsync((TblAccSize)new TblAccSize().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.DetailsList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblAccSizeCodeViewModel
                {
                    TblAccSizeGroup = SelectedMainRow.Iserial
                });
            }
        }

        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new TblAccSize();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblAccSizeCodeAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        #region Prop

        private ObservableCollection<Brand> _brandList;

        public ObservableCollection<Brand> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; RaisePropertyChanged("BrandList"); }
        }

        private SortableCollectionView<TblAccSizeGroupViewModel> _mainRowList;

        public SortableCollectionView<TblAccSizeGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblAccSizeGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblAccSizeGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAccSizeGroupViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblAccSizeGroupViewModel _selectedMainRow;

        public TblAccSizeGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblAccSizeCodeViewModel _selectedDetailRow;

        public TblAccSizeCodeViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblAccSizeCodeViewModel> _selectedDetailRows;

        public ObservableCollection<TblAccSizeCodeViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblAccSizeCodeViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}