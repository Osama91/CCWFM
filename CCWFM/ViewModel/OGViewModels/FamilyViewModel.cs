using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblFamilyViewModel : GenericViewModel
    {
        private ObservableCollection<TblSubFamilyViewModel> _tblSubProductGroupField;

        public ObservableCollection<TblSubFamilyViewModel> DetailsList
        {
            get
            {
                return _tblSubProductGroupField ?? (_tblSubProductGroupField = new ObservableCollection<TblSubFamilyViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblSubProductGroupField, value) != true))
                {
                    _tblSubProductGroupField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        private bool _includeSub;

        public bool IncludeSub
        {
            get { return _includeSub; }
            set { _includeSub = value; RaisePropertyChanged("IncludeSub"); }
        }
    }

    public class TblSubFamilyViewModel : GenericViewModel
    {
        private int _tblFamilyField;

        public int TblFamily
        {
            get
            {
                return _tblFamilyField;
            }
            set
            {
                if ((_tblFamilyField.Equals(value) != true))
                {
                    _tblFamilyField = value;
                    RaisePropertyChanged("TblFamily");
                }
            }
        }

        private int? _SubFamilyLink;

        public int? SubFamilyLink
        {
            get { return _SubFamilyLink; }
            set { _SubFamilyLink = value;RaisePropertyChanged("SubFamilyLink"); }
        }

    }

    #endregion ViewModels

    public class FamilyViewModel : ViewModelBase
    {
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();


        public FamilyViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.FamilyForm.ToString());
                MainRowList = new SortableCollectionView<TblFamilyViewModel>();
                SelectedMainRow = new TblFamilyViewModel();
                SelectedDetailRow = new TblSubFamilyViewModel();

                lkpClient.GetTblFamilyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblFamilyViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
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
                    }
                };
                lkpClient.GetTblSubFamilyCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSubFamilyViewModel();
                        newrow.InjectFrom(row);
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    Loading = false;
                    DetailFullCount = sv.fullCount;
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                    else if (SelectedMainRow.DetailsList.Count <= PageSize)
                    {
                        SelectedDetailRow = SelectedMainRow.DetailsList.FirstOrDefault();
                    }
                    if (Export)
                    {
                        Export = false;                       
                    }
                };
                lkpClient.UpdateOrInsertTblFamilyCompleted += (s, x) =>
                {
                    var savedRow = (TblFamilyViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                lkpClient.UpdateOrInsertTblSubFamilyCompleted += (s, x) =>
                {
                    var savedRow = (TblSubFamilyViewModel)SelectedMainRow.DetailsList.ElementAt(x.outindex);
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };

                lkpClient.DeleteTblFamilyCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                    if (!MainRowList.Any())
                    {
                        AddNewMainRow(false);
                    }
                };

                lkpClient.DeleteTblSubFamilyCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            lkpClient.GetTblFamilyAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
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
                            Loading = true;
                            lkpClient.DeleteTblFamilyAsync(
                                (LkpData.TblFamily)new LkpData.TblFamily().InjectFrom(row), MainRowList.IndexOf(row));
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

            var newrow = new TblFamilyViewModel();

            MainRowList.Insert(currentRowIndex + 1, newrow);
            SelectedMainRow = newrow;
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }

                    var saveRow = new LkpData.TblFamily();
                    saveRow.InjectFrom(SelectedMainRow);
                    lkpClient.UpdateOrInsertTblFamilyAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        public void GetDetailData()
        {
            if (DetailSortBy == null)
                DetailSortBy = "it.Code";
            if (SelectedMainRow != null)
                lkpClient.GetTblSubFamilyAsync(SelectedMainRow.DetailsList.Count, int.MaxValue, SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
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
                        if (row.Iserial != 0)
                        {
                            if (AllowDelete != true)
                            {
                                MessageBox.Show(strings.AllowDeleteMsg);
                                return;
                            }

                            lkpClient.DeleteTblSubFamilyAsync((LkpData.TblSubFamily)new LkpData.TblSubFamily().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                            if (!SelectedMainRow.DetailsList.Any())
                            {
                                AddNewDetailRow(false);
                            }
                        }
                    }
                }
            }
        }

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblSubFamilyViewModel
            {
                TblFamily = SelectedMainRow.Iserial,
                Code = SelectedMainRow.Code
            };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);

            SelectedDetailRow = newrow;
        }

        public void SaveDetailRow()
        {
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    if (AllowUpdate != true)
                    {
                        MessageBox.Show(strings.AllowAddMsg);
                        return;
                    }

                    var save = SelectedDetailRow.Iserial == 0;
                    var rowToSave = new LkpData.TblSubFamily();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    lkpClient.UpdateOrInsertTblSubFamilyAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblFamilyViewModel> _mainRowList;

        public SortableCollectionView<TblFamilyViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblFamilyViewModel> _selectedMainRows;

        public ObservableCollection<TblFamilyViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblFamilyViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblFamilyViewModel _selectedMainRow;

        public TblFamilyViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblSubFamilyViewModel _selectedDetailRow;

        public TblSubFamilyViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblSubFamilyViewModel> _selectedDetailRows;

        public ObservableCollection<TblSubFamilyViewModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblSubFamilyViewModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        #endregion Prop

        public void Report()
        {
            const string reportName = "StyleFamilyReport";

            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }

            //var para = new ObservableCollection<string> { TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }
    }
}