using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.Gl
{
    public class GlRuleJobViewModel : ViewModelBase
    {
        public GlRuleJobViewModel()
        {
            if (!IsDesignTime)
            {
                Client.GetGenericAsync("TblAuthJob", "%%", "%%", "%%", "Iserial", "ASC");
                Client.GetGenericCompleted += (s, sv) => { JobList = sv.Result; };
                GetItemPermissions(PermissionItemName.GlRuleJob.ToString());
                Glclient = new GlServiceClient();
                var glRuleHeaderClient = new GlServiceClient();
                glRuleHeaderClient.GetGenericCompleted += (s, sv) => { GlRuleHeaderList = sv.Result; };
                glRuleHeaderClient.GetGenericAsync("TblGlRuleHeader", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                MainRowList = new SortableCollectionView<TblGlRuleJobViewModel>();
                SelectedMainRow = new TblGlRuleJobViewModel();

                Glclient.GetTblGlRuleJobCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblGlRuleJobViewModel();
                        newrow.InjectFrom(row);

                        newrow.GlRuleHeaderPerRow = new GenericTable();
                        if (sv.userList.FirstOrDefault(x => x.Iserial == row.TblJob) != null)
                        {
                            newrow.JobPerRow = new CRUDManagerService.GenericTable();
                            newrow.JobPerRow.InjectFrom(sv.userList.FirstOrDefault(x => x.Iserial == row.TblJob));
                        }

                        newrow.GlRuleHeaderPerRow.InjectFrom(row.TblGlRuleHeader1);
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

                Glclient.UpdateOrInsertTblGlRuleJobsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                    }
                    try
                    {
                        MainRowList.ElementAt(ev.outindex).InjectFrom(ev.Result);
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                };
                Glclient.DeleteTblGlRuleJobCompleted += (s, ev) =>
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

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetTblGlRuleJobAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
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
            var newrow = new TblGlRuleJobViewModel();
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
                    var saveRow = new TblGlRuleJob();
                    saveRow.InjectFrom(SelectedMainRow);
                    Glclient.UpdateOrInsertTblGlRuleJobsAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.DatabasEname);
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
                            Glclient.DeleteTblGlRuleJobAsync((TblGlRuleJob)new TblGlRuleJob().InjectFrom(row), MainRowList.IndexOf(row), LoggedUserInfo.DatabasEname);
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

        private ObservableCollection<GenericTable> _glRuleHeaderList;

        public ObservableCollection<GenericTable> GlRuleHeaderList
        {
            get { return _glRuleHeaderList; }
            set { _glRuleHeaderList = value; RaisePropertyChanged("GlRuleHeaderList"); }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _jobList;

        public ObservableCollection<CRUDManagerService.GenericTable> JobList
        {
            get { return _jobList; }
            set
            {
                _jobList = value;
                RaisePropertyChanged("JobList");
            }
        }

        private SortableCollectionView<TblGlRuleJobViewModel> _mainRowList;

        public SortableCollectionView<TblGlRuleJobViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblGlRuleJobViewModel> _selectedMainRows;

        public ObservableCollection<TblGlRuleJobViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblGlRuleJobViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblGlRuleJobViewModel _selectedMainRow;

        public TblGlRuleJobViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }

    #region ViewModels

    public class TblGlRuleJobViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private int _iserialField;

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        private int? _tblJob;

        public int? TblJob
        {
            get { return _tblJob; }
            set { _tblJob = value; RaisePropertyChanged("TblJob"); }
        }

        private CRUDManagerService.GenericTable _jobPerRow;

        public CRUDManagerService.GenericTable JobPerRow
        {
            get { return _jobPerRow; }
            set
            {
                if ((ReferenceEquals(_jobPerRow, value) != true))
                {
                    _jobPerRow = value;
                    RaisePropertyChanged("JobPerRow");
                    if (_jobPerRow != null) TblJob = _jobPerRow.Iserial;
                }
            }
        }

        private int? _glRuleHeader;

        public int? TblGlRuleHeader
        {
            get { return _glRuleHeader; }
            set { _glRuleHeader = value; RaisePropertyChanged("TblGlRuleHeader"); }
        }

        private GenericTable _glRuleHeaderPerRow;

        public GenericTable GlRuleHeaderPerRow
        {
            get { return _glRuleHeaderPerRow; }
            set
            {
                _glRuleHeaderPerRow = value; RaisePropertyChanged("GlRuleHeaderPerRow");
                if (GlRuleHeaderPerRow != null) TblGlRuleHeader = GlRuleHeaderPerRow.Iserial;
            }
        }
    }

    #endregion ViewModels
}