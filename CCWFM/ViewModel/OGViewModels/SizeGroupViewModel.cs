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

    public class TblSizeGroupViewModel : GenericViewModel
    {
        private SortableCollectionView<TblSizeCodeViewModel> _tblSizeGroupField;

        public SortableCollectionView<TblSizeCodeViewModel> DetailsList
        {
            get
            {
                return _tblSizeGroupField ?? (_tblSizeGroupField = new SortableCollectionView<TblSizeCodeViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblSizeGroupField, value) != true))
                {
                    _tblSizeGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }
    }

    public class TblSizeCodeViewModel : PropertiesViewModelBase
    {
        private int _sizeCodeSizeGroup;
        private string _sizeCodeSizeCode;
        private int _sizeCodeid;
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

        public int TblSizeGroup
        {
            get
            {
                return _sizeCodeSizeGroup;
            }
            set
            {
                if ((Equals(_sizeCodeSizeGroup, value) != true))
                {
                    _sizeCodeSizeGroup = value;
                    RaisePropertyChanged("TblSizeGroup");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string SizeCode
        {
            get
            {
                return _sizeCodeSizeCode;
            }
            set
            {
                if ((ReferenceEquals(_sizeCodeSizeCode, value) != true))
                {
                    _sizeCodeSizeCode = value;
                    RaisePropertyChanged("SizeCode");
                }
            }
        }

        public int Id
        {
            get
            {
                return _sizeCodeid;
            }
            set
            {
                if ((_sizeCodeid.Equals(value) != true))
                {
                    _sizeCodeid = value;
                    RaisePropertyChanged("Id");
                }
            }
        }
    }

    #endregion ViewModels

    public class SizeGroupViewModel : ViewModelBase
    {
        public SizeGroupViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblSizeGroupViewModel>();
                SelectedMainRow = new TblSizeGroupViewModel();
                SelectedDetailRow = new TblSizeCodeViewModel();
                
                Client.GetTblSizeGroupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSizeGroupViewModel();
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
                Client.GetTblSizeCodeCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSizeCodeViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }

                    Loading = false;
                };
                Client.UpdateOrInsertTblSizeGroupCompleted += (s, x) =>
                {
                    var savedRow = (TblSizeGroupViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                Client.UpdateOrInsertTblSizeCodeCompleted += (s, x) =>
                {
                    var savedRow = (TblSizeCodeViewModel)SelectedMainRow.DetailsList.GetItemAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);

                };

                Client.DeleteTblSizeGroupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                Client.DeleteTblSizeCodeCompleted += (s, ev) =>
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
            Client.GetTblSizeGroupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                        Client.DeleteTblSizeGroupAsync(
                            (TblSizeGroup)new TblSizeGroup().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblSizeGroupViewModel());
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
                    var saveRow = new TblSizeGroup();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblSizeGroupAsync(saveRow, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (SelectedMainRow != null)
                Client.GetTblSizeCodeAsync(SelectedMainRow.DetailsList.Count, PageSize, SelectedMainRow.Iserial);
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
                        Client.DeleteTblSizeCodeAsync((TblSize)new TblSize().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
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

                SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, new TblSizeCodeViewModel
                {
                    TblSizeGroup = SelectedMainRow.Iserial
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
                    var rowToSave = new TblSize();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    Client.UpdateOrInsertTblSizeCodeAsync(rowToSave, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
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

        private SortableCollectionView<TblSizeGroupViewModel> _mainRowList;

        public SortableCollectionView<TblSizeGroupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSizeGroupViewModel> _selectedMainRows;

        public ObservableCollection<TblSizeGroupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSizeGroupViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblSizeGroupViewModel _selectedMainRow;

        public TblSizeGroupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblSizeCodeViewModel _selectedDetailRow;

        public TblSizeCodeViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblSizeCodeViewModel> _selectedDetailRows;

        public ObservableCollection<TblSizeCodeViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSizeCodeViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop
    }
}