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
    public class GlExpensisViewModel : ViewModelBase
    {
        public GlExpensisViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.GlExpensis.ToString());
                Glclient = new GlServiceClient();

                var glExpensisClient = new GlServiceClient();
                glExpensisClient.GetGenericCompleted += (s, sv) =>
                {
                    GlExpensisGroupList = sv.Result;
                };
                glExpensisClient.GetGenericAsync("TblGLExpensisGroup", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                MainRowList = new SortableCollectionView<TblGlExpensisViewModel>();
                SelectedMainRow = new TblGlExpensisViewModel();

                Glclient.GetTblGlExpensisCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlExpensisViewModel();
                        newrow.InjectFrom(row);
                        newrow.GlExpensisGroupPerRow = new GenericTable();
                        if (row.TblGLExpensisGroup1 != null) newrow.GlExpensisGroupPerRow.InjectFrom(row.TblGLExpensisGroup1);

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
                    if (Export)
                    {
                        Export = false;
                        AllowExport = true;
                        //ExportGrid.ExportExcel("Account");
                    }
                };

                Glclient.UpdateOrInsertTblGlExpensisCompleted += (s, ev) =>
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
                    Loading = false;
                };
                Glclient.DeleteTblGlExpensisCompleted += (s, ev) =>
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
            Glclient.GetTblGlExpensisAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }

        public void GetFullMainData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Export = true;
            Glclient.GetTblGlExpensisAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblGlExpensisViewModel();
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
                    var saveRow = new TblGLExpensi();
                    saveRow.InjectFrom(SelectedMainRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblGlExpensisAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
                    }
                }
            }
        }

        public void SaveOldRow(TblGlExpensisViewModel oldRow)
        {
            if (oldRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(oldRow,
                    new ValidationContext(oldRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = oldRow.Iserial == 0;
                    if (AllowUpdate != true && !save)
                    {
                        MessageBox.Show(strings.AllowUpdateMsg);
                        return;
                    }
                    var saveRow = new TblGLExpensi();
                    saveRow.InjectFrom(oldRow);

                    if (!Loading)
                    {
                        Loading = true;
                        Glclient.UpdateOrInsertTblGlExpensisAsync(saveRow, save, MainRowList.IndexOf(oldRow),
                         LoggedUserInfo.DatabasEname);
                    }
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
                            Glclient.DeleteTblGlExpensisAsync((TblGLExpensi)new TblGLExpensi().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        private ObservableCollection<GenericTable> _glExpensisList;

        public ObservableCollection<GenericTable> GlExpensisGroupList
        {
            get { return _glExpensisList; }
            set { _glExpensisList = value; RaisePropertyChanged("GlExpensisGroupList"); }
        }

        private SortableCollectionView<TblGlExpensisViewModel> _mainRowList;

        public SortableCollectionView<TblGlExpensisViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGlExpensisViewModel> _selectedMainRows;

        public ObservableCollection<TblGlExpensisViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlExpensisViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblGlExpensisViewModel _selectedMainRow;

        public TblGlExpensisViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblGlExpensisViewModel : GenericViewModel
    {
        private GenericTable _glExpensisGroupPerRow;

        public GenericTable GlExpensisGroupPerRow
        {
            get { return _glExpensisGroupPerRow; }
            set
            {
                if ((ReferenceEquals(_glExpensisGroupPerRow, value) != true))
                {
                    _glExpensisGroupPerRow = value;
                    RaisePropertyChanged("GlExpensisGroupPerRow");
                    if (GlExpensisGroupPerRow != null)
                    {
                        if (GlExpensisGroupPerRow.Iserial != 0)
                        {
                            TblGlExpensisGroup = GlExpensisGroupPerRow.Iserial;
                        }
                    }
                }
            }
        }

        private int? _tblTblGlExpensisGroupField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqGlExpensisGroup")]
        public int? TblGlExpensisGroup
        {
            get
            {
                return _tblTblGlExpensisGroupField;
            }
            set
            {
                if ((_tblTblGlExpensisGroupField.Equals(value) != true))
                {
                    _tblTblGlExpensisGroupField = value;
                    RaisePropertyChanged("TblGlExpensisGroup");
                }
            }
        }
    }

    #endregion ViewModels
}