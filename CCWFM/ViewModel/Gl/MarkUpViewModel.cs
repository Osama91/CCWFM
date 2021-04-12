using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class MarkupViewModel : ViewModelBase
    {
        public MarkupViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Markup.ToString());
                Glclient = new GlServiceClient();

                var tblRecInvHeaderTypeClient = new GlServiceClient();
                tblRecInvHeaderTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    MarkupGroupList = sv.Result;
                };
                tblRecInvHeaderTypeClient.GetGenericAsync("TblMarkupGroup", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                DirectionList = new ObservableCollection<GenericTable>
                {
                    new GenericTable {Iserial = 1, Code = "1", Ename = "1", Aname = "1"},
                    new GenericTable {Iserial = -1, Code = "-1", Ename = "-1", Aname = "-1"}
                };

                MainRowList = new SortableCollectionView<TblMarkupViewModel>();
                SelectedMainRow = new TblMarkupViewModel();

                Glclient.GetTblMarkupCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblMarkupViewModel { MarkupGroupPerRow = new GenericTable() };
                        newrow.MarkupGroupPerRow.InjectFrom(row.TblMarkupGroup1);
                        newrow.InjectFrom(row);

                        MainRowList.Add(newrow);
                    }

                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Glclient.UpdateOrInsertTblMarkupsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
                    catch (Exception)
                    {
                    }
                };
                Glclient.DeleteTblMarkupCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                GetMaindata();
            }
        }

        private void MainRowList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (!Loading)
            {
                MainRowList.Clear();
                SortBy = null;
                foreach (var sortDesc in MainRowList.SortDescriptions)
                {
                    SortBy = SortBy + "it." + sortDesc.PropertyName + (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblMarkupAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblMarkupViewModel();
            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow,
                    new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblMarkup();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblMarkupsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }
                            Glclient.DeleteTblMarkupAsync((TblMarkup)new TblMarkup().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
                        }
                        else
                        {
                            MainRowList.Remove(row);
                            if (!MainRowList.Any())
                            {
                                AddNewMainRow(false);
                            }
                        }
                    }
                }
            }
        }

        #region Prop

        private ObservableCollection<GenericTable> _currencyList;

        public ObservableCollection<GenericTable> CurrencyList
        {
            get { return _currencyList; }
            set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GenericTable> _markupGrouplist;

        public ObservableCollection<GenericTable> MarkupGrouplist
        {
            get { return _markupGrouplist; }
            set { _markupGrouplist = value; RaisePropertyChanged("MarkupGrouplist"); }
        }

        private SortableCollectionView<TblMarkupViewModel> _mainRowList;

        public SortableCollectionView<TblMarkupViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<GenericTable> _directionList;

        public ObservableCollection<GenericTable> DirectionList
        {
            get { return _directionList; }
            set { _directionList = value; RaisePropertyChanged("DirectionList"); }
        }

        private ObservableCollection<GenericTable> _markupGroupList;

        public ObservableCollection<GenericTable> MarkupGroupList
        {
            get { return _markupGroupList; }
            set { _markupGroupList = value; RaisePropertyChanged("MarkupGroupList"); }
        }

        private ObservableCollection<TblMarkupViewModel> _selectedMainRows;

        public ObservableCollection<TblMarkupViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblMarkupViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblMarkupViewModel _selectedMainRow;

        public TblMarkupViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblMarkupViewModel : GenericViewModel
    {
        public TblMarkupViewModel()
        {
            Direction = 1;
        }

        private int? _tblMarkupGroup;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMarkupGroup")]
        public int? TblMarkupGroup
        {
            get { return _tblMarkupGroup; }
            set { _tblMarkupGroup = value; RaisePropertyChanged("TblMarkupGroup"); }
        }

        private GenericTable _markupGroupPerRow;

        public GenericTable MarkupGroupPerRow
        {
            get { return _markupGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_markupGroupPerRow, value) != true))
                {
                    _markupGroupPerRow = value;
                    RaisePropertyChanged("MarkupGroupPerRow");
                    TblMarkupGroup = MarkupGroupPerRow.Iserial;
                }
            }
        }

        private int _direction;

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; RaisePropertyChanged("Direction"); }
        }

        private bool _itemEffect;

        public bool ItemEffect
        {
            get { return _itemEffect; }
            set { _itemEffect = value; RaisePropertyChanged("ItemEffect"); }
        }

        private int _markupGroup;

        public int MarkupGroup
        {
            get { return _markupGroup; }
            set { _markupGroup = value; RaisePropertyChanged("MarkupGroup"); }
        }
    }

    #endregion ViewModels
}