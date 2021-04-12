using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblCheckTransactionViewModel : GenericViewModel
    {
        private bool _yes;

        public bool Yes
        {
            get { return _yes; }
            set
            {
                _yes = value; RaisePropertyChanged("Yes");
            }
        }

        private bool _no;

        public bool No
        {
            get { return _no; }
            set
            {
                _no = value; RaisePropertyChanged("No");
            }
        }

        private string _store;

        public string Store
        {
            get { return _store; }
            set { _store = value; RaisePropertyChanged("Store"); }
        }

        private string _company;

        public string Company
        {
            get { return _company; }
            set { _company = value; RaisePropertyChanged("Company"); }
        }

        private int? _tblCheckListDesignGroupHeader1;

        public int? TblCheckListDesignGroupHeader1
        {
            get { return _tblCheckListDesignGroupHeader1; }
            set { _tblCheckListDesignGroupHeader1 = value; RaisePropertyChanged("TblCheckListDesignGroupHeader1"); }
        }

        private int? _tblCheckListDesignGroupHeader2;

        public int? TblCheckListDesignGroupHeader2
        {
            get { return _tblCheckListDesignGroupHeader2; }
            set { _tblCheckListDesignGroupHeader2 = value; RaisePropertyChanged("TblCheckListDesignGroupHeader2"); }
        }

        private int _tblAuthUser;

        public int TblAuthUser
        {
            get { return _tblAuthUser; }
            set { _tblAuthUser = value; RaisePropertyChanged("TblAuthUser"); }
        }

        private DateTime _transDate;

        public DateTime TransDate
        {
            get { return _transDate; }
            set { _transDate = value; RaisePropertyChanged("TransDate"); }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        private bool _approved;

        public bool Approved
        {
            get { return _approved; }
            set { _approved = value; RaisePropertyChanged("Approved"); }
        }

        private TblCheckListGroup _checklistGroupPerRow;

        public TblCheckListGroup CheckListGroupPerRow
        {
            get { return _checklistGroupPerRow ?? (_checklistGroupPerRow = new TblCheckListGroup()); }
            set { _checklistGroupPerRow = value; RaisePropertyChanged("CheckListGroupPerRow"); }
        }

        private GenericTable _tblCheckListDesignGroupHeader1PerRow;

        public GenericTable CheckListDesignGroupHeader1PerRow
        {
            get { return _tblCheckListDesignGroupHeader1PerRow ?? (_tblCheckListDesignGroupHeader1PerRow = new GenericTable()); }
            set
            {
                _tblCheckListDesignGroupHeader1PerRow = value; RaisePropertyChanged("CheckListDesignGroupHeader1PerRow");

                if (_tblCheckListDesignGroupHeader1PerRow != null)
                {
                    if (_tblCheckListDesignGroupHeader1PerRow.Iserial != 0)
                    {
                        _tblCheckListDesignGroupHeader1 = _tblCheckListDesignGroupHeader1PerRow.Iserial;
                    }
                }
            }
        }

        private GenericTable _tblCheckListDesignGroupHeader22;

        public GenericTable CheckListDesignGroupHeader2PerRow
        {
            get { return _tblCheckListDesignGroupHeader22 ?? (_tblCheckListDesignGroupHeader22 = new GenericTable()); }
            set
            {
                _tblCheckListDesignGroupHeader22 = value; RaisePropertyChanged("CheckListDesignGroupHeader2PerRow");
                if (_tblCheckListDesignGroupHeader22 != null)
                {
                    if (_tblCheckListDesignGroupHeader22.Iserial != 0)
                    {
                        _tblCheckListDesignGroupHeader2 = _tblCheckListDesignGroupHeader22.Iserial;
                    }
                }
            }
        }

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
                if (UpdatedAllowed)
                {
                    _client.UpdateOrDeleteTblCheckListTransactionAsync((TblCheckListTransaction)new TblCheckListTransaction().InjectFrom(this), Checked, 0);
                    UpdatedAllowed = false;
                }
            }
        }

        private bool _updatedAllow;

        public bool UpdatedAllowed
        {
            get { return _updatedAllow; }
            set { _updatedAllow = value; RaisePropertyChanged("UpdatedAllowed"); }
        }

        private int _tblCheckListItemField;

        public int TblCheckListItem
        {
            get
            {
                return _tblCheckListItemField;
            }
            set
            {
                if ((_tblCheckListItemField.Equals(value) != true))
                {
                    _tblCheckListItemField = value;
                    RaisePropertyChanged("TblCheckListItem");
                }
            }
        }

        private int _tblCheckListGroup;

        public int TblCheckListGroup
        {
            get
            {
                return _tblCheckListGroup;
            }
            set
            {
                if ((_tblCheckListGroup.Equals(value) != true))
                {
                    _tblCheckListGroup = value;
                    RaisePropertyChanged("TblCheckListGroup");
                }
            }
        }
    }

    public class CheckListTransactionViewModel : ViewModelBase
    {
        public CheckListTransactionViewModel()
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
                    Client.GetTblCheckListTransactionAsync(LoggedUserInfo.Iserial);
                };

                Client.GetTblUserCheckListAsync(LoggedUserInfo.Iserial);
                Client.GetTblUserCheckListCompleted += (s, sv) =>
                {
                    foreach (var variable in sv.Result)
                    {
                        Client.GetTblCheckListLinkAsync(variable.TblCheckListGroup);
                    }
                };

                Client.GetTblCheckListTransactionCompleted += (s, sv) =>
                {
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