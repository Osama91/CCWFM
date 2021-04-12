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
using CCWFM.Helpers.Utilities;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class SequenceViewModel : ViewModelBase
    {
        public SequenceViewModel()
        {
            if (!IsDesignTime)
            {
                GetItemPermissions(PermissionItemName.Sequence.ToString());
                Glclient = new GlServiceClient();
                MainRowList = new SortableCollectionView<TblSequenceViewModel>();
                SelectedMainRow = new TblSequenceViewModel();

                Glclient.GetTblSequenceCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblSequenceViewModel();
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
                    if (Export)
                    {
                        Export = false;
                        ExportGrid.ExportExcel("Sequence");
                    }
                };

                Glclient.UpdateOrInsertTblSequencesCompleted += (s, ev) =>
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
                Glclient.DeleteTblSequenceCompleted += (s, ev) =>
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
                    SortBy = SortBy + "it." + sortDesc.PropertyName +
                             (sortDesc.Direction == ListSortDirection.Ascending ? " ASC" : " DESC");
                }
                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblSequenceAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects,
                LoggedUserInfo.DatabasEname);
        }

        public void AddNewMainRow(bool checkLastRow)
        {
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
            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }
            var newrow = new TblSequenceViewModel();
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
                    var saveRow = new TblSequence();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblSequencesAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow),
                        LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblSequenceAsync((TblSequence)new TblSequence().InjectFrom(row),
                                MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        private SortableCollectionView<TblSequenceViewModel> _mainRowList;

        public SortableCollectionView<TblSequenceViewModel> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private ObservableCollection<TblSequenceViewModel> _selectedMainRows;

        public ObservableCollection<TblSequenceViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSequenceViewModel>()); }
            set
            {
                _selectedMainRows = value;
                RaisePropertyChanged("SelectedMainRows");
            }
        }

        private TblSequenceViewModel _selectedMainRow;

        public TblSequenceViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblSequenceViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private string _codeField;

        private string _formatField;

        private int _highestField;

        private int _iserialField;

        private int _lowestField;

        private int _nextRecField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string Code
        {
            get
            {
                return _codeField;
            }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    _codeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqFormat")]
        public string Format
        {
            get
            {
                return _formatField;
            }
            set
            {
                if ((ReferenceEquals(_formatField, value) != true))
                {
                    _formatField = value;
                    RaisePropertyChanged("Format");
                }
            }
        }

        public int Highest
        {
            get
            {
                return _highestField;
            }
            set
            {
                if ((_highestField.Equals(value) != true))
                {
                    _highestField = value;
                    RaisePropertyChanged("Highest");
                }
            }
        }

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

        public int Lowest
        {
            get
            {
                return _lowestField;
            }
            set
            {
                if ((_lowestField.Equals(value) != true))
                {
                    _lowestField = value;
                    RaisePropertyChanged("Lowest");
                }
            }
        }

        public int NextRec
        {
            get
            {
                return _nextRecField;
            }
            set
            {
                if ((_nextRecField.Equals(value) != true))
                {
                    _nextRecField = value;
                    RaisePropertyChanged("NextRec");
                }
            }
        }

        private int _numberOfInt;

        public int NumberOfInt
        {
            get
            {
                return _numberOfInt;
            }
            set
            {
                if ((_numberOfInt.Equals(value) != true))
                {
                    _numberOfInt = value;
                    RaisePropertyChanged("NumberOfInt");
                }
            }
        }

        private bool _manual;

        public bool Manual
        {
            get { return _manual; }
            set { _manual = value; RaisePropertyChanged("Manual"); }
        }

        private bool _useDateTime;

        public bool UseDateTime
        {
            get { return _useDateTime; }
            set { _useDateTime = value; RaisePropertyChanged("UseDateTime"); }
        }
    }

    #endregion ViewModels
}