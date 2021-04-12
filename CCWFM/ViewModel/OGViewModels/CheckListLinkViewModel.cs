using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    public class TblCheckListLinkViewModel : GenericViewModel
    {
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

        private TblCheckListGroup _checklistGroupPerRow;

        public TblCheckListGroup CheckListGroupPerRow
        {
            get { return _checklistGroupPerRow; }
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
                    _client.UpdateOrDeleteTblCheckListLinkAsync((TblCheckListLink)new TblCheckListLink().InjectFrom(this), Checked, 0);
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

    #endregion ViewModels

    public class CheckListLinkViewModel : ViewModelBase
    {
        public CheckListLinkViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new ObservableCollection<GenericViewModel>();
                SelectedMainRow = new GenericViewModel();
                SelectedDetailRow = new TblCheckListLinkViewModel();

                Client.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new GenericViewModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code
                        };
                        MainRowList.Add(newrow);
                    }
                };
                Client.GetGenericAsync("TblCheckListGroup", "%%", "%%", "%%", "Iserial", "ASC");

                var detailservice = new CRUD_ManagerServiceClient();

                detailservice.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblCheckListLinkViewModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code,
                            TblCheckListItem = row.Iserial
                        };
                        DetailList.Add(newrow);
                    }
                };
                detailservice.GetGenericAsync("TblCheckListItem", "%%", "%%", "%%", "Iserial", "ASC");

                var header1Service = new CRUD_ManagerServiceClient();

                header1Service.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new GenericTable
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code,
                        };
                        CheckListDesignGroupHeader1List.Add(newrow);
                    }
                };
                header1Service.GetGenericAsync("TblCheckListDesignGroupHeader1", "%%", "%%", "%%", "Iserial", "ASC");
                var header2 = new CRUD_ManagerServiceClient();
                header2.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new GenericTable
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code,
                        };
                        CheckListDesignGroupHeader2List.Add(newrow);
                    }
                };
                header2.GetGenericAsync("TblCheckListDesignGroupHeader2", "%%", "%%", "%%", "Iserial", "ASC");

                Client.GetTblCheckListLinkCompleted += (s, sv) =>
                {
                    foreach (var row in DetailList)
                    {
                        row.UpdatedAllowed = false;
                        row.Checked = false;
                        row.CheckListDesignGroupHeader1PerRow = null;
                        row.CheckListDesignGroupHeader2PerRow = null;
                        row.TblCheckListDesignGroupHeader1 = null;
                        row.TblCheckListDesignGroupHeader2 = null;
                    }
                    foreach (var row in sv.Result)
                    {
                        var subCheckListRow = DetailList.SingleOrDefault(x =>
                            x.Iserial == row.TblCheckListItem);
                        if (subCheckListRow != null)
                        {
                            subCheckListRow.Checked = true;
                            if (row.TblCheckListDesignGroupHeader11 != null)
                                subCheckListRow.CheckListDesignGroupHeader1PerRow.InjectFrom(row.TblCheckListDesignGroupHeader11);

                            if (row.TblCheckListDesignGroupHeader21 != null)
                                subCheckListRow.CheckListDesignGroupHeader2PerRow.InjectFrom(row.TblCheckListDesignGroupHeader21);
                        }
                    }
                };
            }
        }

        #region Prop

        public void SectionChanged()
        {
            if (SelectedMainRow != null) Client.GetTblCheckListLinkAsync(SelectedMainRow.Iserial);
        }

        private ObservableCollection<GenericViewModel> _mainRowList;

        public ObservableCollection<GenericViewModel> MainRowList
        {
            get { return _mainRowList ?? (_mainRowList = new ObservableCollection<GenericViewModel>()); }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<GenericTable> _checkListDesignGroupHeader1List;

        public ObservableCollection<GenericTable> CheckListDesignGroupHeader1List
        {
            get { return _checkListDesignGroupHeader1List ?? (_checkListDesignGroupHeader1List = new ObservableCollection<GenericTable>()); }
            set { _checkListDesignGroupHeader1List = value; RaisePropertyChanged("CheckListDesignGroupHeader1List"); }
        }

        private ObservableCollection<GenericTable> _checkListDesignGroupHeader2List;

        public ObservableCollection<GenericTable> CheckListDesignGroupHeader2List
        {
            get { return _checkListDesignGroupHeader2List ?? (_checkListDesignGroupHeader2List = new ObservableCollection<GenericTable>()); }
            set { _checkListDesignGroupHeader2List = value; RaisePropertyChanged("CheckListDesignGroupHeader2List"); }
        }

        private ObservableCollection<TblCheckListLinkViewModel> _detailList;

        public ObservableCollection<TblCheckListLinkViewModel> DetailList
        {
            get { return _detailList ?? (_detailList = new ObservableCollection<TblCheckListLinkViewModel>()); }
            set { _detailList = value; RaisePropertyChanged("DetailList"); }
        }

        private GenericViewModel _selectedMainRow;

        public GenericViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("GenericViewModel"); }
        }

        private TblCheckListLinkViewModel _selectedDetailRow;

        public TblCheckListLinkViewModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        #endregion Prop
    }
}