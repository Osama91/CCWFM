using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblApprovalViewModel : PropertiesViewModelBase
    {
        private DateTime _approvalDateField;

        private int _approvedStatusField;

        private string _commentField;

        private int _iserialField;

        private int _tblAuthUserField;

        public DateTime ApprovalDate
        {
            get { return _approvalDateField; }
            set
            {
                if ((_approvalDateField.Equals(value) != true))
                {
                    _approvalDateField = value;
                    RaisePropertyChanged("ApprovalDate");
                }
            }
        }

        private int _approvalType;

        public int ApprovalType
        {
            get { return _approvalType; }
            set { _approvalType = value; RaisePropertyChanged("ApprovalType"); }
        }

        public int ApprovedStatus
        {
            get { return _approvedStatusField; }
            set
            {
                if ((_approvedStatusField.Equals(value) != true))
                {
                    _approvedStatusField = value;
                    RaisePropertyChanged("ApprovedStatus");
                }
            }
        }

        public string Comment
        {
            get { return _commentField; }
            set
            {
                if ((ReferenceEquals(_commentField, value) != true))
                {
                    _commentField = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                    if (Iserial != 0)
                    {
                        Locked = true;
                    }
                }
            }
        }

        public int TblAuthUser
        {
            get { return _tblAuthUserField; }
            set
            {
                if ((_tblAuthUserField.Equals(value) != true))
                {
                    _tblAuthUserField = value;
                    RaisePropertyChanged("TblAuthUser");
                }
            }
        }

        private GenericTable _approvalPerRow;

        public GenericTable ApprovalPerRow
        {
            get { return _approvalPerRow ?? (_approvalPerRow = new GenericTable()); }
            set { _approvalPerRow = value; RaisePropertyChanged("ApprovalPerRow"); }
        }

        private TblAuthUser _userPerRow;

        public TblAuthUser UserPerRow
        {
            get { return _userPerRow ?? (_userPerRow = new TblAuthUser()); }
            set
            {
                _userPerRow = value; RaisePropertyChanged("UserPerRow");
                if (UserPerRow != null) TblAuthUser = UserPerRow.Iserial;
            }
        }

        private bool _locked;

        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; RaisePropertyChanged("Locked"); }
        }

        private int _tblSalesOrderField;

        public int TblSalesOrder
        {
            get { return _tblSalesOrderField; }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }
    }

    public class ApprovalViewModel : ViewModelBase
    {
        private int _salesorder;

        public int Salesorder
        {
            get { return _salesorder; }
            set
            {
                _salesorder = value;
                RaisePropertyChanged("Salesorder");
            }
        }

        private int _user;

        public int User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged("User");
            }
        }

        public int Perm { get; set; }
        ProductionService.ProductionServiceClient ProductionClient = new ProductionService.ProductionServiceClient();

        public ApprovalViewModel(StyleHeaderViewModel styleViewModel)
        {
            TempStyleViewModel = styleViewModel;
            Perm = 301;
            if (!DesignerProperties.IsInDesignTool)
            {
                User = LoggedUserInfo.Iserial;

                Salesorder = styleViewModel.SelectedDetailRow.Iserial;
                
                var approvalTypeService = new CRUD_ManagerServiceClient();

                approvalTypeService.GetGenericAsync("TblApprovalTypes", "%%", "%%", "%%", "Iserial", "ASC");

                approvalTypeService.GetGenericCompleted += (s, sv) =>
                {
                    var tblUserBrandSection =
                        LoggedUserInfo.UserBrandSection.FirstOrDefault(
                            x =>
                                x.BrandCode == styleViewModel.SelectedMainRow.Brand &&
                                x.TblLkpBrandSection == styleViewModel.SelectedMainRow.TblLkpBrandSection);

                    switch (styleViewModel.SelectedDetailRow.SalesOrderType)
                    {
                        case (int)SalesOrderType.SalesOrderPo:

                            Perm = 303;
                            break;
                    }

                    if (tblUserBrandSection != null)
                    {
                        ApprovalTypesList = sv.Result;
                        var userPerm =
                            tblUserBrandSection.TblUserBrandSectionPermissions.SingleOrDefault(
                                x => x.TblAuthPermission == Perm);

                        if (userPerm != null)
                        {
                            if (!userPerm.Retail)
                            {
                                var tempRow = sv.Result.SingleOrDefault(x => x.Code == "Retail");
                                ApprovalTypesList.Remove(tempRow);
                            }
                            if (!userPerm.Financial)
                            {
                                var tempRow = sv.Result.SingleOrDefault(x => x.Code == "Financial");
                                ApprovalTypesList.Remove(tempRow);
                            }
                            if (!userPerm.Technical)
                            {
                                var tempRow = sv.Result.SingleOrDefault(x => x.Code == "Technical");
                                ApprovalTypesList.Remove(tempRow);
                            }
                        }
                        else
                        {
                            ApprovalTypesList.Clear();
                        }
                    }

                    if (ApprovalTypesList != null && ApprovalTypesList.Count > 0)
                    {
                        GetMaindata();
                    }
                };

                MainRowList = new ObservableCollection<TblApprovalViewModel>();
                SelectedMainRow = new TblApprovalViewModel();

                Client.GetTblApprovalCompleted += (s, sv) =>
                {
                    if (sv.Error!=null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }

                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblApprovalViewModel();
                        newrow.InjectFrom(row);
                        newrow.ApprovalPerRow.InjectFrom(row.TblApprovalType);
                        newrow.UserPerRow = row.TblAuthUser1;
                        MainRowList.Add(newrow);
                    }

                    foreach (var row in ApprovalTypesList)
                    {
                        if (row.Iserial == 1 && !styleViewModel.SelectedDetailRow.IsRetailApproved ||
                            row.Iserial == 2 && !styleViewModel.SelectedDetailRow.IsFinancialApproved
                                || row.Iserial == 3 && !styleViewModel.SelectedDetailRow.IsTechnicalApproved
                             )
                        {
                            AddNewMainRow(row);
                        }
                    }

                    Loading = false;
                };

                ProductionClient.GetTblUsersApprovalStatusAsync(LoggedUserInfo.Iserial);

                ProductionClient.GetTblUsersApprovalStatusCompleted += (s, sv) =>
                {
                    ApprovalStatusList.Clear();
                    foreach (var item in sv.Result.ToList())
                    {
                        ApprovalStatusList.Add(new GenericTable().InjectFrom(item) as GenericTable);
                    }
                    
                };

                Client.UpdateOrInsertTblApprovalCompleted += (s, x) =>
                {
                    Salesorder.InjectFrom(x.Result.TblSalesOrder);
                    var savedRow = (TblApprovalViewModel)MainRowList.GetItemAt(x.outindex);                    
                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                    OnApproveCompleted();
                    TempStyleViewModel.Loading = false;
                    Loading = false;
                };
                Client.DeleteTblApprovalCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
             
            }
        }

        public event EventHandler ApproveCompleted;

        protected virtual void OnApproveCompleted()
        {
            var handler = ApproveCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        StyleHeaderViewModel _tempStyleViewModel;
        public StyleHeaderViewModel TempStyleViewModel
        {
            get { return _tempStyleViewModel; }
            set { _tempStyleViewModel = value; RaisePropertyChanged("TempStyleViewModel"); }
        }

        public void GetMaindata()
        {
            Client.GetTblApprovalAsync(Salesorder);
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
                            Client.DeleteTblApprovalAsync(
                                (TblApproval)new TblApproval().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void SaveMainRow()
        {
            foreach (var row in MainRowList.Where(x => x.Iserial == 0))
            {
                var isvalid = Validator.TryValidateObject(row,
                    new ValidationContext(row, null, null), null, true);

                if (isvalid)
                {
                    var saveRow = new TblApproval();
                    saveRow.InjectFrom(row);

                    //CheckTNA
                    if (saveRow.ApprovalType == 1 && saveRow.ApprovedStatus == 1)
                    {
                        LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                        _client.CheckTNAAsync(SelectedMainRow.TblSalesOrder, LoggedUserInfo.WFM_UserJob.Value);
                        _client.CheckTNACompleted += (s, sv) =>
                        {
                            if (sv.Result == true)
                            {
                                Client.UpdateOrInsertTblApprovalAsync(saveRow, MainRowList.IndexOf(row), Perm);
                                TempStyleViewModel.Loading = true;
                                Loading = true;
                            }
                            else
                            { MessageBox.Show("Kindly Check TNA"); }
                        };  
                    }
                    else
                    {
                        Client.UpdateOrInsertTblApprovalAsync(saveRow, MainRowList.IndexOf(row), Perm);
                        TempStyleViewModel.Loading = true;
                        Loading = true;
                    }
                }
            }
        }

        public void AddNewMainRow(GenericTable row)
        {
            var newRow = new TblApprovalViewModel
            {
                UserPerRow =
                {
                    Iserial = LoggedUserInfo.Iserial,
                    AxId = LoggedUserInfo.AxId,
                    UserName = LoggedUserInfo.WFM_UserName
                },
                TblAuthUser = LoggedUserInfo.Iserial,
                TblSalesOrder = Salesorder,
                ApprovalDate = DateTime.Now,
                ApprovalType = row.Iserial,
                ApprovalPerRow = row
            };
            MainRowList.Add(newRow);
        }

        #region Prop

        private ObservableCollection<TblApprovalViewModel> _mainRowList;

        public ObservableCollection<TblApprovalViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblApprovalViewModel> _selectedMainRows;

        public ObservableCollection<TblApprovalViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblApprovalViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<GenericTable> _approvalTypesList;

        public ObservableCollection<GenericTable> ApprovalTypesList
        {
            get { return _approvalTypesList; }
            set { _approvalTypesList = value; RaisePropertyChanged("ApprovalTypesList"); }
        }

        private TblApprovalViewModel _selectedMainRow;

        public TblApprovalViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<GenericTable> _approvalStatusList;
     
        public ObservableCollection<GenericTable> ApprovalStatusList
        {
            get { return _approvalStatusList ?? (_approvalStatusList= new ObservableCollection<GenericTable>()); }
            set { _approvalStatusList = value; RaisePropertyChanged("ApprovalStatusList"); }
        }

        #endregion Prop
    }
}