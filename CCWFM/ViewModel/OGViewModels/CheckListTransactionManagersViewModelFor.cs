using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class CheckListTransactionManagersViewModelViewModel : ViewModelBase
    {
        public CheckListTransactionManagersViewModelViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new ObservableCollection<GenericViewModel>();
                SelectedMainRow = new GenericViewModel();
                SelectedDetailRow = new TblCheckTransactionViewModel();
                DetailListCollectionView = new PagedCollectionView(DetailList);
                DetailListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("CheckListGroupPerRow.Ename"));
                DetailListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("CheckListDesignGroupHeader1PerRow.Ename"));
                Client.GetTblCheckListLinkCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCheckTransactionViewModel
                        {
                            Aname = row.TblCheckListItem1.Aname,
                            Ename = row.TblCheckListItem1.Ename,
                            Iserial = row.Iserial,
                            Code = row.TblCheckListItem1.Code,
                            TblCheckListItem = row.Iserial,
                            TransDate = DateTime.Now,
                            TblAuthUser = LoggedUserInfo.Iserial,
                            TblCheckListDesignGroupHeader1 = row.TblCheckListDesignGroupHeader1,
                            TblCheckListDesignGroupHeader2 = row.TblCheckListDesignGroupHeader2,
                            CheckListGroupPerRow = row.TblCheckListGroup1,
                        };

                        if (row.TblCheckListDesignGroupHeader11 != null)
                            newrow.CheckListDesignGroupHeader1PerRow.InjectFrom(row.TblCheckListDesignGroupHeader11);

                        if (row.TblCheckListDesignGroupHeader21 != null)
                            newrow.CheckListDesignGroupHeader2PerRow.InjectFrom(row.TblCheckListDesignGroupHeader21);

                        if (LoggedUserInfo.Store != null) newrow.Store = LoggedUserInfo.Store.code;
                        if (LoggedUserInfo.Company != null) newrow.Company = LoggedUserInfo.Company.Code;
                        DetailList.Add(newrow);
                    }
                };

                Client.GetTblUserCheckListAsync(LoggedUserInfo.Iserial);
                Client.GetTblUserCheckListCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        Client.GetTblCheckListLinkAsync(variable.TblCheckListGroup);
                    }
                };

                Client.GetTblCheckListTransactionForManagerCompleted += (s, sv) =>
                {
                    var test = DetailList;

                    foreach (var variable in sv.Result.Select(x => x.TblAuthUser).Distinct())
                    {
                        if (DetailList.Count == 0)
                        {
                            foreach (var tblCheckTransactionViewModel in test)
                            {
                            }
                        }
                    }

                    foreach (var row in DetailList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                        row.Notes = null;
                        row.Approved = false;
                    }
                    foreach (var row in sv.Result)
                    {
                        var subCheckListRow = DetailList.SingleOrDefault(x =>
                            x.TblCheckListItem == row.TblCheckListItem);
                        if (subCheckListRow != null)
                        {
                            subCheckListRow.Checked = true;
                            subCheckListRow.Notes = row.Notes;
                            subCheckListRow.Yes = row.Yes;
                            subCheckListRow.No = row.No;
                        }
                    }
                };
            }
        }

        public void GetChecklistForManager()
        {
            Client.GetTblCheckListTransactionForManagerAsync(LoggedUserInfo.Code, Date);
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged("Date"); }
        }

        #region Prop

        private ObservableCollection<GenericViewModel> _mainRowList;

        public ObservableCollection<GenericViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new ObservableCollection<GenericViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblCheckTransactionViewModel> _detailList;

        public ObservableCollection<TblCheckTransactionViewModel> DetailList
        {
            get { return _detailList ?? (_detailList = new ObservableCollection<TblCheckTransactionViewModel>()); }
            set { _detailList = value; RaisePropertyChanged("DetailList"); }
        }

        private GenericViewModel _selectedMainRow;

        public GenericViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("GenericViewModel"); }
        }

        private PagedCollectionView _detailListCollectionView;

        public PagedCollectionView DetailListCollectionView
        {
            get
            {
                return _detailListCollectionView;
            }
            set
            {
                _detailListCollectionView = value;
                RaisePropertyChanged("DetailListCollectionView");
            }
        }

        private TblCheckTransactionViewModel _selectedDetailRow;

        public TblCheckTransactionViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop
    }
}