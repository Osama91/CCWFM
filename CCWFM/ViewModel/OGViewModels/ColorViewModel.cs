using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblColorViewModel : GenericViewModel
    {
        private double _defaultPrice;

        public double DefaultPrice
        {
            get { return _defaultPrice; }
            set { _defaultPrice = value; RaisePropertyChanged("DefaultPrice"); }
        }
        private byte[] _imageField;

        public byte[] Image
        {
            get
            {
                return _imageField;
            }
            set
            {
                if ((ReferenceEquals(_imageField, value) != true))
                {
                    _imageField = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        private GenericTable _colorGroupPerRow;

        public GenericTable ColorGroupPerRow
        {
            get { return _colorGroupPerRow; }
            set { _colorGroupPerRow = value; RaisePropertyChanged("ColorGroupPerRow"); }
        }

        private int? _tblLkpColorGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqColorGroup")]
        public int? TblLkpColorGroup
        {
            get
            {
                return _tblLkpColorGroup;
            }
            set
            {
                if ((_tblLkpColorGroup.Equals(value) != true))
                {
                    _tblLkpColorGroup = value;
                    RaisePropertyChanged("TblLkpColorGroup");
                }
            }
        }
    }

    public class ColorViewModel : ViewModelBase
    {
        public ColorViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.ColorCodeForm.ToString());
                MainRowList = new SortableCollectionView<TblColorViewModel>();
                SelectedMainRow = new TblColorViewModel();

                Client.GetGenericCompleted += (s, sv) =>
                {
                    ColorGroupList = sv.Result;
                };

                Client.GetGenericAsync("TblLkpColorGroup", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetTblColorCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblColorViewModel { ColorGroupPerRow = new GenericTable() };
                        if (row.TblLkpColorGroup1 != null) newrow.ColorGroupPerRow.InjectFrom(row.TblLkpColorGroup1);

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

                Client.UpdateOrInsertTblColorCompleted += (s, x) =>
                {
                    var savedRow = (TblColorViewModel)MainRowList.GetItemAt(x.outindex);
                    Loading = false;
                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                    }
                };
                Client.DeleteTblColorCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    Loading = false;
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

       

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";

            Loading = true;
            Client.GetTblColorAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowAddMsg);
                                return;
                            }

                            Loading = true;

                            Client.DeleteTblColorAsync(
                                (TblColor)new TblColor().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblColorViewModel();

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblColor();
                    saveRow.InjectFrom(SelectedMainRow);
                    Loading = true;
                    Client.UpdateOrInsertTblColorAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblColorViewModel> _mainRowList;

        public SortableCollectionView<TblColorViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblColorViewModel> _selectedMainRows;

        public ObservableCollection<TblColorViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblColorViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _colorGroupList;

        public ObservableCollection<GenericTable> ColorGroupList
        {
            get { return _colorGroupList ?? (_colorGroupList = new ObservableCollection<GenericTable>()); }
            set { _colorGroupList = value; RaisePropertyChanged("ColorGroupList"); }
        }

        private int _colorGroup;

        public int ColorGroup
        {
            get { return _colorGroup; }
            set { _colorGroup = value; RaisePropertyChanged("ColorGroup"); }
        }

        private TblColorViewModel _selectedMainRow;

        public TblColorViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop

        internal void GetMaindataFull(DataGrid mainGrid)
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Export = true;
            Loading = true;
            ExportGrid = mainGrid;
            Client.GetTblColorAsync(MainRowList.Count, int.MaxValue, SortBy, Filter, ValuesObjects);
        }
    }
}