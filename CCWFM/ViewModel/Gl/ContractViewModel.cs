using CCWFM.ContractService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using GalaSoft.MvvmLight.Command;
using Omu.ValueInjecter.Silverlight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Data.Extensions;
using System.Windows.Media;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Helpers.LocalizationHelpers;

namespace CCWFM.ViewModel.Gl
{
    public class ContractHeader : TblContractHeader
    {
        public ContractHeader()
        {
            DocDate = DateTime.Now;
            CreatedBy = LoggedUserInfo.Iserial;
            CreationDate = DateTime.Now;
            base.Amount = 0;
            TblContractDetails.CollectionChanged += (s, e) => { RaisePropertyChanged(nameof(Amount)); };
        }

        [Range(1, long.MaxValue)]
        public new decimal Amount
        {
            //set { base.Amount = value; }
            get { return TblContractDetails.Sum(cd => cd.Total); }
        }
      
        ObservableCollection<ContractDetail> tblContractDetails;
        public new ObservableCollection<ContractDetail> TblContractDetails
        {
            get { return tblContractDetails ?? (tblContractDetails = new ObservableCollection<ContractDetail>()); }
            set { tblContractDetails = value; RaisePropertyChanged(nameof(TblContractDetails)); }
        }

        ObservableCollection<ContractPaymentByPeriod> tblContractPaymentByPeriods;
        public new ObservableCollection<ContractPaymentByPeriod> TblContractPaymentByPeriods
        {
            get { return tblContractPaymentByPeriods ?? (tblContractPaymentByPeriods = new ObservableCollection<ContractPaymentByPeriod>()); }
            set { tblContractPaymentByPeriods = value; RaisePropertyChanged(nameof(TblContractDetails)); }
        }
    }
    public class ContractDetail : TblContractDetail
    {
        public ContractDetail()
        {
          
        }
        [Range(1, long.MaxValue)]
        public new int Qty
        {
            get { return base.Qty; }
            set { base.Qty = value; RaisePropertyChanged(nameof(Total)); }
        }
        public new decimal Cost
        {
            get { return base.Cost; }
            set { base.Cost = value; RaisePropertyChanged(nameof(Total)); }
        }
        public decimal Total
        {
            get
            {
                decimal result = 0;
                switch (TblContractHeader1.TblRetailOrderProductionType)
                {
                    case 1:
                        result = Qty * Cost;
                        break;
                    case 2:
                        result = Qty * (Cost - AccCost);
                        break;
                    case 3:
                        result = Qty * (Cost - AccCost - FabricCost);
                        break;
                    default:
                        result = Qty * Cost;
                        break;
                }
                return result;
            }
        }

        public Brush Approved
        {
            get
            {
                return base.TblSalesOrderColor1.TblSalesOrder1.Status == 1 ?
                  new SolidColorBrush(Colors.Red) : null;
            }
        }
    }
    public class ContractPaymentByPeriod: TblContractPaymentByPeriod
    {
        public ContractPaymentByPeriod()
        {
            DueDate = DateTime.Now;
        }
        public new DateTime DueDate
        {
            get { return base.DueDate; }
            set { base.DueDate = value; }
        }
    }
    public class ContractViewModel : ViewModelStructuredBase
    {
        ContractServiceClient ContractClient = Helpers.Services.Instance.GetContractServiceClient();
        LkpData.LkpDataClient LkpDataClient = Helpers.Services.Instance.GetLkpDataClient();
        CRUDManagerService.CRUD_ManagerServiceClient managerClient = Helpers.Services.Instance.GetCRUD_ManagerServiceClient();
        public ContractViewModel() : base(PermissionItemName.Contracts)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                ApproveContract = new RelayCommand(() => {
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    SelectedMainRow.Approved = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    if (SaveCommand.CanExecute(null))
                        SaveCommand.Execute(null);
                    //if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                    //    if (NewCommand.CanExecute(null))
                    //        NewCommand.Execute(null);
                }, () => CheckCanApprove());
                UnApproveContract = new RelayCommand(() => {
                    //if (SaveCommand.CanExecute(null))
                    //    SaveCommand.Execute(null);
                    if (ValidData()) SaveRecord();
                    SelectedMainRow.Approved = false;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    if (ValidData()) SaveRecord();
                    //if (SaveCommand.CanExecute(null))
                    //    SaveCommand.Execute(null);
                    //if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                    //    if (NewCommand.CanExecute(null))
                    //        NewCommand.Execute(null);
                }, () => CheckCanUnApprove());
                DeleteContractDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete &&
                            Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblContractDetails.Remove(SelectedDetailRow);

                            //Check IF Have Approved Cancel Request
                            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                            _client.CheckApprovedSalesOrderColorCancelRequestAsync(SelectedDetailRow.TblSalesOrderColor);
                            _client.CheckApprovedSalesOrderColorCancelRequestCompleted += (s,sv) => { };

                            //if (SelectedMainRow.TblContractDetails.Count == 0)
                            //{
                            //    AddNewDetailRow(false);
                            //}
                        }
                        else
                            DeleteDetailRow();
                        ApproveContract.RaiseCanExecuteChanged();
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                }, (o) => {
                    return (SelectedMainRow != null && !SelectedMainRow.Approved && !CanUnApprove) || SelectedMainRow.Iserial == 0 || SelectedDetailRow.Iserial == 0;
                });
                AddContractPaymentDetail = new RelayCommand<KeyEventArgs>((o) =>
                {
                    if (o.Key == Key.Down)
                    {
                        AddNewPaymentDetailRow(false);
                    }
                    if (o.Key == Key.Delete &&
                            Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedPaymentDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblContractPaymentByPeriods.Remove(SelectedPaymentDetailRow);
                            if (SelectedMainRow.TblContractPaymentByPeriods.Count == 0)
                            {
                                AddNewPaymentDetailRow(false);
                            }
                        }
                        else
                            DeletePaymentDetailRow();
                    }
                });
                OpenAttachments = new RelayCommand(() => {
                    if (SelectedMainRow.Iserial > 0)
                        new AttachmentChildWindow("TblContractHeader", SelectedMainRow.Iserial).Show();
                    else
                        MessageBox.Show("Contract must be saved first");
                });
                ContractClient.GetStylesCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        bool needAdd = false;
                        ContractDetail temp = null;
                        temp = SelectedMainRow.TblContractDetails.FirstOrDefault(cd =>
                            cd.TblSalesOrderColor1.TblSalesOrder1.SalesOrderCode == item.TblSalesOrder1.SalesOrderCode &&
                            cd.TblSalesOrderColor1.TblSalesOrder1.TblStyle1.StyleCode == item.TblSalesOrder1.TblStyle1.StyleCode &&
                            cd.TblColor1.Code == item.TblColor1.Code);
                        if (temp == null)
                        {
                            temp = new ContractDetail() { TblContractHeader1 = SelectedMainRow };
                            needAdd = true;
                        }
                        temp.TblContractHeader = SelectedMainRow.Iserial;
                        temp.TblLkpBrandSection = item.TblSalesOrder1.TblStyle1.TblLkpBrandSection;
                        temp.TblSalesOrderColor = item.Iserial;
                        temp.TblSalesOrderColor1 = item;
                        temp.TblColor = item.TblColor;
                        temp.TblColor1 = item.TblColor1;
                        temp.Qty = item.Total;
                        temp.ForeignCost = item.Cost ?? 0;
                        temp.Cost = item.LocalCost ?? 0;//.TblSalesOrder1.TblStyle1.TargetCostPrice
                        temp.AccCost = Convert.ToDecimal(item.TblSalesOrder1.AccCost);
                        temp.FabricCost = Convert.ToDecimal(item.TblSalesOrder1.FabricCost);
                        temp.OperationCost = Convert.ToDecimal(item.TblSalesOrder1.OperationCost);
                        temp.DeliveryDate = item.TblSalesOrder1.DeliveryDate.Value;
                        temp.Material = item.TblSalesOrder1.Notes;
                        if(needAdd)
                            SelectedMainRow.TblContractDetails.Add(temp);
                    }
                    Loading = false;
                    GetStyles.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(SelectedMainRow));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                GetStyles = new RelayCommand<object>((o) => {
                    if (ValidHeaderData())
                    {
                        ObservableCollection<int> brandsSection = new ObservableCollection<int>();
                        SelectedBrandSectionList.ForEach(r => brandsSection.Add(r.Iserial));
                        
                        ContractClient.GetStylesAsync(SelectedMainRow.SupplierIserial,
                            SelectedMainRow.BrandCode,
                            brandsSection,
                            new ObservableCollection<int> { SelectedMainRow.TblLkpSeason },
                             new ObservableCollection<int> { SelectedMainRow.TblCurrency },
                            SelectedMainRow.TblRetailOrderProductionType, DeliveryFrom, DeliveryTo);
                        Loading = true;
                        GetStyles.RaiseCanExecuteChanged();
                        ApproveContract.RaiseCanExecuteChanged();
                    }
                }, (o) => { return !Loading; });
                ContractClient.GetSinleStyleCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        bool needAdd = false;
                        ContractDetail temp = null;
                        temp = SelectedMainRow.TblContractDetails.FirstOrDefault(cd =>
                            cd.TblSalesOrderColor1.TblSalesOrder1.SalesOrderCode == item.TblSalesOrder1.SalesOrderCode &&
                            cd.TblSalesOrderColor1.TblSalesOrder1.TblStyle1.StyleCode == item.TblSalesOrder1.TblStyle1.StyleCode &&
                            cd.TblColor1.Code == item.TblColor1.Code);
                        if (temp == null)
                        {
                            temp = new ContractDetail() { TblContractHeader1 = SelectedMainRow };
                            needAdd = true;
                        }
                        temp.TblContractHeader = SelectedMainRow.Iserial;
                        temp.TblLkpBrandSection = item.TblSalesOrder1.TblStyle1.TblLkpBrandSection;
                        temp.TblSalesOrderColor = item.Iserial;
                        temp.TblSalesOrderColor1 = item;
                        temp.TblColor = item.TblColor;
                        temp.TblColor1 = item.TblColor1;
                        temp.Qty = item.Total;
                        temp.ForeignCost = item.Cost ?? 0;
                        temp.Cost = item.LocalCost ?? 0;//.TblSalesOrder1.TblStyle1.TargetCostPrice
                        temp.AccCost = Convert.ToDecimal(item.TblSalesOrder1.AccCost);
                        temp.FabricCost = Convert.ToDecimal(item.TblSalesOrder1.FabricCost);
                        temp.OperationCost = Convert.ToDecimal(item.TblSalesOrder1.OperationCost);
                        temp.DeliveryDate = item.TblSalesOrder1.DeliveryDate.Value;
                        temp.Material = item.TblSalesOrder1.Notes;
                       // if (needAdd)
                            SelectedMainRow.TblContractDetails.Add(temp);
                    }
                    Loading = false;
                    GetSingleStyle.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(SelectedMainRow));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                GetSingleStyle = new RelayCommand<object>((o) => {
                    if (ValidHeaderData())
                    {
                        ObservableCollection<int> brandsSection = new ObservableCollection<int>();
                        SelectedBrandSectionList.ForEach(r => brandsSection.Add(r.Iserial));
                        ContractClient.GetSinleStyleAsync(SelectedMainRow.SupplierIserial,
                            SelectedMainRow.BrandCode,
                            brandsSection,
                            new ObservableCollection<int> { SelectedMainRow.TblLkpSeason },
                             new ObservableCollection<int> { SelectedMainRow.TblCurrency },
                            SelectedMainRow.TblRetailOrderProductionType, DeliveryFrom, DeliveryTo,SingleStyleCode);
                        Loading = true;
                        GetSingleStyle.RaiseCanExecuteChanged();
                        ApproveContract.RaiseCanExecuteChanged();
                    }
                }, (o) => { return !Loading; });

                this.PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "ContractApprove") != null)
                    {
                        CanApprove = true;
                    }
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "ContractUnApprove") != null)
                    {
                        CanUnApprove = true;
                    }
                };
                this.GetCustomePermissions(PermissionItemName.Contracts.ToString());

                MainRowList = new ObservableCollection<ContractHeader>();
                AddNewMainRow(false);

                ContractClient.GetContractHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new ContractHeader();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    if (SearchWindow != null)
                    {
                        SearchWindow.FullCount = sv.fullCount;
                        SearchWindow.Loading = false;
                    }
                    FullCount = sv.fullCount;
                    //if (FullCount == 0 && MainRowList.Count == 0)
                    //{
                    //    AddNewMainRow(false);
                    //}
                };
                ContractClient.GetContractDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new ContractDetail();
                        newrow.InjectFrom(row);
                        //newrow.TblBankTransactionType1 = BankTransactionTypeList.FirstOrDefault(btt => btt.Iserial == newrow.TblBankTransactionType);
                        SelectedMainRow.TblContractDetails.Add(newrow);

                        //if (!SelectedBrandSectionList.Any(bs => bs.Iserial == newrow.Iserial))
                        //    SelectedBrandSectionList.Add(new ContractService.TblLkpBrandSection() { Iserial = newrow.Iserial });
                    }
                    //if (!SelectedMainRow.TblContractDetails.Any())
                    //{
                    //    AddNewDetailRow(false);
                    //}
                    Loading = false;
                    ApproveContract.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                ContractClient.GetContractPaymentDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new ContractPaymentByPeriod();
                        newrow.InjectFrom(row);
                        //newrow.TblBankTransactionType1 = BankTransactionTypeList.FirstOrDefault(btt => btt.Iserial == newrow.TblBankTransactionType);
                        SelectedMainRow.TblContractPaymentByPeriods.Add(newrow);
                    }
                    if (!SelectedMainRow.TblContractPaymentByPeriods.Any())
                    {
                        AddNewPaymentDetailRow(false);
                    }
                    Loading = false;
                    ApproveContract.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                ContractClient.UpdateOrInsertContractHeaderCompleted += (s, x) =>
                {
                    if (x.Error != null)
                    {
                        MessageBox.Show(Models.Helper.GetInnerExceptionMessage(x.Error));
                        Loading = false;
                        return;
                    }
                    try
                    {
                        ContractHeader savedRow = null;
                        if (x.outindex >= 0)
                            savedRow = MainRowList.ElementAt(x.outindex);

                        if (savedRow != null)
                        {
                            savedRow.InjectFrom(x.Result);
                            savedRow.TblLkpSeason1 = new TblLkpSeason();
                            savedRow.TblLkpSeason1 = SeasonList.FirstOrDefault(b => b.Iserial == savedRow.TblLkpSeason);
                            savedRow.TblLkpBrandSection1 = new TblLkpBrandSection();
                            savedRow.TblLkpBrandSection1 = BrandSectionList.FirstOrDefault(c => c.Iserial == savedRow.TblLkpBrandSection);
                            savedRow.TblContractDetails.Clear();
                            foreach (var item in x.Result.TblContractDetails)
                            {
                                var detailTemp = new ContractDetail();
                                detailTemp.InjectFrom(item);
                                //if (!SelectedBrandSectionList.Any(bs => bs.Iserial == item.TblLkpBrandSection))
                                //    SelectedBrandSectionList.Add(new ContractService.TblLkpBrandSection() { Iserial = item.TblLkpBrandSection ?? 0 });
                                savedRow.TblContractDetails.Add(detailTemp);
                            }
                            savedRow.TblContractPaymentByPeriods.Clear();
                            foreach (var item in x.Result.TblContractPaymentByPeriods)
                            {
                                var detailTemp = new ContractPaymentByPeriod();
                                detailTemp.InjectFrom(item);
                                //if (!SelectedBrandSectionList.Any(bs => bs.Iserial == item.TblLkpBrandSection))
                                //    SelectedBrandSectionList.Add(new ContractService.TblLkpBrandSection() { Iserial = item.TblLkpBrandSection ?? 0 });
                                savedRow.TblContractPaymentByPeriods.Add(detailTemp);
                            }
                        }
                        RaisePropertyChanged(nameof(IsHeaderHasDetails));
                        DeleteCommand.RaiseCanExecuteChanged();
                        ApproveContract.RaiseCanExecuteChanged();
                        UnApproveContract.RaiseCanExecuteChanged();
                        IsNewChanged();
                        RaisePropertyChanged(nameof(IsReadOnly));
                        MessageBox.Show(strings.SavedMessage);
                    }
                    finally
                    {
                        Loading = false;
                    }
                };
                //ContractClient.UpdateOrInsertContractDetailCompleted += (s, x) =>
                //{
                //    var savedRow = SelectedMainRow.TblContractDetails.ElementAt(x.outindex);
                //    if (savedRow != null)
                //    {
                //        savedRow.InjectFrom(x.Result);
                //        //savedRow.TblBankTransactionType1 = BankTransactionTypeList.FirstOrDefault(bt => bt.Iserial == savedRow.TblBankTransactionType);
                //    }
                //    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                //};
                ContractClient.DeleteContractHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };
                ContractClient.DeleteContractDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblContractDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblContractDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };
                ContractClient.DeleteContractPaymentDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblContractPaymentByPeriods.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblContractPaymentByPeriods.Remove(oldrow);
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };

                Client.GetAllBrandsCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        BrandList.Add(item.Brand_Code);
                    }
                };
                Client.GetAllSeasonsCompleted += (s, e) =>
                {
                    SeasonList.Clear();
                    foreach (var row in e.Result)
                    {
                        TblLkpSeason temp = new TblLkpSeason();
                        temp.InjectFrom(row);
                        SeasonList.Add(temp);
                    }
                };
                LkpDataClient.GetTblBrandSectionLinkCompleted += (s, e) =>
                {
                    BrandSectionList.Clear();
                    foreach (var row in e.Result)
                    {
                        TblLkpBrandSection temp = new TblLkpBrandSection();
                        temp.InjectFrom(row.TblLkpBrandSection1);
                        BrandSectionList.Add(temp);
                    }
                    SelectedMainRow.TblLkpBrandSection1 = BrandSectionList.FirstOrDefault(bs =>
                        bs.Iserial == SelectedMainRow.TblLkpBrandSection);
                };
                ContractClient.GetLookUpCurrencyCompleted += (s, e) =>
                {
                    CurrenciesList = e.Result;
                };
                BrandChanged = new RelayCommand<object>((o) => {
                    LkpDataClient.GetTblBrandSectionLinkAsync(SelectedMainRow.BrandCode, LoggedUserInfo.Iserial);
                });
                managerClient.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (RetailOrderProductionTypeList.All(x => x.Iserial != row.Iserial))
                        {
                            RetailOrderProductionTypeList.Add(
                                new CRUDManagerService.GenericTable().InjectFrom(row)
                                as CRUDManagerService.GenericTable);
                        }
                    }
                };
                Client.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (SubContractorList.All(x => x.Iserial != row.Iserial))
                        {
                            SubContractorList.Add(
                                new CRUDManagerService.GenericTable().InjectFrom(row)
                                as CRUDManagerService.GenericTable);
                        }
                    }
                };

                GetComboData();
                GetMaindata();
                ContractClient.GetBrandContractReportsCompleted += (s, e) =>
                {
                    BrandContractReportList = e.Result;
                    var report = BrandContractReportList.FirstOrDefault(r => r.BrandCode == SelectedMainRow.BrandCode);
                    if (report != null && !string.IsNullOrWhiteSpace(report.ReportName))
                        SelectedMainRow.ContractReport = report.ReportName;
                };
                ContractClient.GetBrandContractReportsAsync();
            }
        }

        #region Methods

        private void ValidateDetailRow(ContractDetail contractDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblContractDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblContractDetails.Add(contractDetail);
            }
            else
            {
                SelectedMainRow.TblContractDetails.Add(contractDetail);
            }
            SelectedDetailRow = contractDetail;
            RaisePropertyChanged(nameof(Total));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }
        private bool CheckCanApprove()
        {
            //return (CanApprove && !SelectedMainRow.Approved && SelectedMainRow.ApproveDate == null &&
            //    SelectedMainRow.TblContractDetails.Any()) || (CanUnApprove && !SelectedMainRow.Approved && SelectedMainRow.ApproveDate != null &&
            //    SelectedMainRow.TblContractDetails.Any());


            return (CanApprove && !SelectedMainRow.Approved  &&
          SelectedMainRow.TblContractDetails.Any()) || (CanUnApprove && !SelectedMainRow.Approved  &&
          SelectedMainRow.TblContractDetails.Any());
        }
        private bool CheckCanUnApprove()
        {
            return CanUnApprove && SelectedMainRow.Approved;
        }
        public bool ValidHeaderData()
        {
            if (SelectedMainRow.TblLkpSeason <= 0)
            {
                MessageBox.Show(strings.ReqBrand);
                return false;
            }
            if (SelectedMainRow.Currency == null)
            {
                MessageBox.Show(strings.ReqCurrency);
                return false;
            }
            if (SelectedMainRow.Supplier == null ||
                string.IsNullOrWhiteSpace(SelectedMainRow.Supplier.Code))
            {
                MessageBox.Show(strings.ReqSupplier);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedMainRow.BrandCode))
            {
                MessageBox.Show(strings.ReqBrand);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedMainRow.CompanyRepresentative))
            {
                MessageBox.Show(strings.ReqCompanyRepresentative);
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.Approved && SelectedMainRow.TblContractDetails.Any(td => 0 == td.Qty))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            if (SelectedMainRow.Approved && SelectedMainRow.TblContractPaymentByPeriods.Any(td => (td.AmountPercent ?? 0M) <= 0))
            {
                MessageBox.Show(strings.CheckPayments);
                return false;
            }
            if (SelectedMainRow.Approved && SelectedMainRow.TblContractPaymentByPeriods.Sum(td => td.AmountPercent) != 100)
            {
                MessageBox.Show(strings.CheckPayments);
                return false;
            }
            if ((SelectedMainRow.Approved && SelectedMainRow.Iserial <= 0 &&
                SelectedMainRow.TblContractDetails.Any(td => td.Qty <= 0)))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            if (SelectedMainRow.TblRetailOrderProductionType == 2 &&
                SelectedMainRow.TblContractDetails.Any(cd => cd.AccCost <= 0))
            {
                MessageBox.Show(strings.ReqCost);
                return false;
            }
            if (SelectedMainRow.TblRetailOrderProductionType == 3 &&
                SelectedMainRow.TblContractDetails.Any(cd => cd.FabricCost <= 0))
            {
                MessageBox.Show(strings.ReqCost);
                return false;
            }
            if (SelectedMainRow.TblRetailOrderProductionType == 4 &&
                SelectedMainRow.TblContractDetails.Any(cd => cd.OperationCost <= 0))
            {
                MessageBox.Show(strings.ReqCost);
                return false;
            }
            if (SelectedMainRow.TblContractDetails.Any(cd => cd.Total <= 0 || cd.Cost <= 0))
            {
                MessageBox.Show(strings.ReqCost);
                return false;
            }
            return true;
        }

        #endregion

        #region Operations

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            ContractClient.GetContractHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    ContractClient.DeleteContractHeaderAsync((TblContractHeader)new
                        TblContractHeader().InjectFrom(SelectedMainRow),
                        MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }
        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow && SelectedMainRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                    if (!isvalid) { return; }
                }
                SelectedMainRow = new ContractHeader();
                SupplierPerRow = new CRUDManagerService.TBLsupplier();
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                //AddNewDetailRow(false);
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
              
                if (isvalid)
                {
                    var saveRow = new TblContractHeader();                   
                    saveRow.InjectFrom(SelectedMainRow);
                    if (string.IsNullOrWhiteSpace(SelectedMainRow.Code))
                        saveRow.Code = "";
                    saveRow.TblContractDetails = new ObservableCollection<TblContractDetail>();
                    foreach (var item in SelectedMainRow.TblContractDetails)
                    {
                        var detailTemp = new TblContractDetail();
                        detailTemp.InjectFrom(item);
                        saveRow.TblContractDetails.Add(detailTemp);
                        detailTemp.TblContractHeader1 = null;
                        detailTemp.TblContractHeader1Reference = null;
                        detailTemp.TblColor1 = null;
                        detailTemp.TblColor1Reference = null;
                        detailTemp.TblSalesOrderColor1 = null;
                        detailTemp.TblSalesOrderColor1Reference = null;
                    }
                    saveRow.TblContractPaymentByPeriods = new ObservableCollection<TblContractPaymentByPeriod>();
                    foreach (var item in SelectedMainRow.TblContractPaymentByPeriods)
                    {
                        var detailTemp = new TblContractPaymentByPeriod();
                        detailTemp.InjectFrom(item);
                        saveRow.TblContractPaymentByPeriods.Add(detailTemp);
                        detailTemp.TblContractHeader1 = null;
                        detailTemp.TblContractHeader1Reference = null;
                    }
                    var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    if (mainRowIndex < 0)
                    {
                        MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    }
                    MessageBox.Show("Kindly Save Contracts On Sticth");
                    //ContractClient.UpdateOrInsertContractHeaderAsync(saveRow, mainRowIndex,
                    //    LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
                    Loading = true;
                }
            }
        }
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
                ContractClient.GetContractDetailAsync(SelectedMainRow.TblContractDetails.Count, PageSize, SelectedMainRow.Iserial);
        }
        public void GetPaymentDetailData()
        {
            if (SelectedMainRow != null)
                ContractClient.GetContractPaymentDetailAsync(
                    SelectedMainRow.TblContractPaymentByPeriods.Count, PageSize,
                    SelectedMainRow.Iserial);
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    var contractDetail = (TblContractDetail)new TblContractDetail().InjectFrom(SelectedDetailRow);
                    ContractClient.DeleteContractDetailAsync(contractDetail, LoggedUserInfo.Iserial);
                }
            }
        }
        public void DeletePaymentDetailRow()
        {
            if (SelectedPaymentDetailRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    ContractClient.DeleteContractPaymentDetailAsync(
                        (TblContractPaymentByPeriod)new TblContractPaymentByPeriod().InjectFrom(SelectedPaymentDetailRow),
                        SelectedMainRow.TblContractPaymentByPeriods.IndexOf(SelectedPaymentDetailRow));
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblContractDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblContractDetails.Count - 1))
            {
                if (checkLastRow && SelectedDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(
                        SelectedDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.TblContractDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new ContractDetail
                {
                    TblContractHeader = SelectedMainRow.Iserial
                });
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void AddNewPaymentDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblContractPaymentByPeriods.IndexOf(SelectedPaymentDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblContractPaymentByPeriods.Count - 1))
            {
                if (checkLastRow && SelectedPaymentDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedPaymentDetailRow, new ValidationContext(
                        SelectedPaymentDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.TblContractPaymentByPeriods.Insert(currentRowIndex + 1, SelectedPaymentDetailRow = new ContractPaymentByPeriod
                {
                    TblContractHeader = SelectedMainRow.Iserial
                });
            }
        }
        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();
                
                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var rowToSave = new TblContractDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    MessageBox.Show("kindly Save Contract on Stitch");
                    //ContractClient.UpdateOrInsertContractDetailAsync(rowToSave, SelectedMainRow.TblContractDetails.IndexOf(SelectedDetailRow));
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
            Client.GetAllSeasonsByUserAsync(LoggedUserInfo.Iserial);
            ContractClient.GetLookUpCurrencyAsync(LoggedUserInfo.DatabasEname);
            Client.GetGenericAsync("TblSubContractor", "%%", "%%", "%%", "Iserial", "ASC");
            managerClient.GetGenericAsync("TblRetailOrderProductionType", "%%", "%%", "%%", "Iserial", "ASC");
        }

        #endregion

        #region Properties

        private ObservableCollection<ContractHeader> _mainRowList;
        public ObservableCollection<ContractHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }
        private ObservableCollection<ContractHeader> _selectedMainRows;
        public ObservableCollection<ContractHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<ContractHeader>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private ContractHeader _selectedMainRow;
        public ContractHeader SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new ContractHeader());
            }
            set
            {
                _selectedMainRow = value;
                SelectedBrandSectionList.Clear();
                var report = BrandContractReportList.FirstOrDefault(r => r.BrandCode == SelectedMainRow.BrandCode);
                if (report != null && !string.IsNullOrWhiteSpace(report.ReportName))
                    _selectedMainRow.ContractReport = report.ReportName;
                if (SelectedMainRow.TblContractPaymentByPeriods == null)
                    SelectedMainRow.TblContractPaymentByPeriods =
                        new ObservableCollection<ContractPaymentByPeriod>();
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                ApproveContract.RaiseCanExecuteChanged();
                UnApproveContract.RaiseCanExecuteChanged();
                IsNewChanged();
                GetDetailData(); GetPaymentDetailData();
            }
        }
        private ContractDetail _selectedDetailRow;
        public ContractDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new ContractDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }
        private ContractPaymentByPeriod _selectedPaymentDetailRow;
        public ContractPaymentByPeriod SelectedPaymentDetailRow
        {
            get { return _selectedPaymentDetailRow ?? (_selectedPaymentDetailRow = new ContractPaymentByPeriod() { DueDate = DateTime.Now }); }
            set { _selectedPaymentDetailRow = value; RaisePropertyChanged(nameof(SelectedPaymentDetailRow)); }
        }
        private ObservableCollection<TblBrandContractReport> _detailRows;
        public ObservableCollection<TblBrandContractReport> DetailRows
        {
            get { return _detailRows; }
            set { _detailRows = new ObservableCollection<TblBrandContractReport>(); }
        }

        CRUDManagerService.TBLsupplier supplierRec;
        public CRUDManagerService.TBLsupplier SupplierPerRow
        {
            get { return supplierRec; }
            set
            {
                supplierRec = value;
                var result = new TBLsupplier();
                result.InjectFrom(value);
                SelectedMainRow.Supplier = result;
                SelectedMainRow.SupplierIserial = result.Iserial;
                RaisePropertyChanged(nameof(SupplierPerRow));
            }
        }

        public CRUDManagerService.GenericTable TblSubContractorRec
        {
            get { return SubContractorList.FirstOrDefault(sc => sc.Iserial == SelectedMainRow.TblLkpBrandSection); }
            set { if (value != null) SelectedMainRow.TblSubContractor = value.Iserial; }
        }
        public CRUDManagerService.GenericTable TblRetailOrderProductionTypeRec
        {
            get { return RetailOrderProductionTypeList.FirstOrDefault(sc => sc.Iserial == SelectedMainRow.TblRetailOrderProductionType); }
            set { if (value != null) SelectedMainRow.TblRetailOrderProductionType = value.Iserial; }
        }

        private ObservableCollection<TblBrandContractReport> brandContractReportList;
        public ObservableCollection<TblBrandContractReport> BrandContractReportList
        {
            get { return brandContractReportList ?? (BrandContractReportList = new ObservableCollection<TblBrandContractReport>()); }
            set { brandContractReportList = value; }
        }

        #region Combo Data

        ObservableCollection<string> _brandList = new ObservableCollection<string>();
        public ObservableCollection<string> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; RaisePropertyChanged(nameof(BrandList)); }
        }

        ObservableCollection<TblLkpBrandSection> _brandSectionList = new ObservableCollection<TblLkpBrandSection>();
        public ObservableCollection<TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList; }
            set { _brandSectionList = value; RaisePropertyChanged(nameof(BrandSectionList)); }
        }

        ObservableCollection<TblLkpBrandSection> _selectedBrandSectionList = new ObservableCollection<TblLkpBrandSection>();
        public ObservableCollection<TblLkpBrandSection> SelectedBrandSectionList
        {
            get { return _selectedBrandSectionList; }
            set { _selectedBrandSectionList = value; RaisePropertyChanged(nameof(SelectedBrandSectionList)); }
        }


        ObservableCollection<TblCurrencyTest> _currenciesList = new ObservableCollection<TblCurrencyTest>();
        public ObservableCollection<TblCurrencyTest> CurrenciesList
        {
            get { return _currenciesList; }
            set { _currenciesList = value; RaisePropertyChanged(nameof(CurrenciesList)); }
        }

        TblCurrency _selectedCurrency = new TblCurrency();
        public TblCurrency SelectedCurrency
        {
            get { return _selectedCurrency; }
            set { _selectedCurrency = value; RaisePropertyChanged(nameof(SelectedCurrency)); }
        }

        ObservableCollection<TblLkpSeason> _seasonList = new ObservableCollection<TblLkpSeason>();
        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get { return _seasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _seasonList = value; RaisePropertyChanged(nameof(SeasonList)); }
        }

        ObservableCollection<TblLkpSeason> _selectedSeasonList = new ObservableCollection<TblLkpSeason>();
        public ObservableCollection<TblLkpSeason> SelectedSeasonList
        {
            get { return _selectedSeasonList ?? (_seasonList = new ObservableCollection<TblLkpSeason>()); }
            set { _selectedSeasonList = value; RaisePropertyChanged(nameof(SelectedSeasonList)); }
        }

        ObservableCollection<CRUDManagerService.GenericTable> _subContractorList = new ObservableCollection<CRUDManagerService.GenericTable>();
        public ObservableCollection<CRUDManagerService.GenericTable> SubContractorList
        {
            get { return _subContractorList ?? (_subContractorList = new ObservableCollection<CRUDManagerService.GenericTable>()); }
            set { _subContractorList = value; RaisePropertyChanged(nameof(SubContractorList)); }
        }

        ObservableCollection<CRUDManagerService.GenericTable> _retailOrderProductionTypeList = new ObservableCollection<CRUDManagerService.GenericTable>();
        public ObservableCollection<CRUDManagerService.GenericTable> RetailOrderProductionTypeList
        {
            get { return _retailOrderProductionTypeList ?? (_retailOrderProductionTypeList = new ObservableCollection<CRUDManagerService.GenericTable>()); }
            set { _retailOrderProductionTypeList = value; RaisePropertyChanged(nameof(RetailOrderProductionTypeList)); }
        }

        #endregion

        public virtual bool IsReadOnly
        {
            get
            {
                return (SelectedMainRow != null &&
                 ((SelectedMainRow.Iserial > 0 && SelectedMainRow.Approved)) || CanUnApprove);//
            }
        }
        public virtual bool IsHeaderHasDetails
        {
            get { return (SelectedMainRow.TblContractDetails.Any() || IsReadOnly); }
        }
        private bool canApprove;
        public bool CanApprove
        {
            get { return canApprove; }
            set { canApprove = value; RaisePropertyChanged(nameof(CanApprove)); ApproveContract.RaiseCanExecuteChanged(); }
        }
        private bool canUnApprove;
        public bool CanUnApprove
        {
            get { return canUnApprove; }
            set
            {
                canUnApprove = value; RaisePropertyChanged(nameof(CanUnApprove));
                RaisePropertyChanged(nameof(IsReadOnly)); RaisePropertyChanged(nameof(IsHeaderHasDetails));
                UnApproveContract.RaiseCanExecuteChanged(); NewCommand.RaiseCanExecuteChanged();
            }
        }
        public override bool IsNew
        {
            get { return SelectedMainRow.Iserial <= 0; }//base.IsNew && 
            set { base.IsNew = value; }
        }
        public int Total
        {
            get { return SelectedMainRow.TblContractDetails.Sum(td => td.Qty); }
        }

        DateTime? deliveryFrom, deliveryTo;
        public DateTime? DeliveryFrom
        {
            get { return deliveryFrom; }
            set { deliveryFrom = value; RaisePropertyChanged(nameof(DeliveryFrom)); }
        }
        public DateTime? DeliveryTo
        {
            get { return deliveryTo; }
            set { deliveryTo = value; RaisePropertyChanged(nameof(DeliveryTo)); }
        }

        string _singleStyleCode;
        public string SingleStyleCode
        {
            get
            {
                if (!String.IsNullOrEmpty(_singleStyleCode))
                { return _singleStyleCode.Trim(); }
                else return _singleStyleCode;
            }
            set { _singleStyleCode = value; RaisePropertyChanged(nameof(_singleStyleCode)); }
        }
        #endregion

        #region Commands

        RelayCommand openAttachments;
        public RelayCommand OpenAttachments
        {
            get { return openAttachments; }
            set { openAttachments = value; RaisePropertyChanged(nameof(OpenAttachments)); }
        }
        RelayCommand approveContract;
        public RelayCommand ApproveContract
        {
            get { return approveContract; }
            set { approveContract = value; RaisePropertyChanged(nameof(ApproveContract)); }
        }

        RelayCommand unApproveContract;
        public RelayCommand UnApproveContract
        {
            get { return unApproveContract; }
            set { unApproveContract = value; RaisePropertyChanged(nameof(UnApproveContract)); }
        }
        
        RelayCommand<object> brandChanged;
        public RelayCommand<object> BrandChanged
        {
            get { return brandChanged; }
            set { brandChanged = value; RaisePropertyChanged(nameof(BrandChanged)); }
        }


        RelayCommand<object> deleteContractDetail;
        public RelayCommand<object> DeleteContractDetail
        {
            get { return deleteContractDetail; }
            set { deleteContractDetail = value; RaisePropertyChanged(nameof(DeleteContractDetail)); }
        }

        RelayCommand<object> getStyles;
        public RelayCommand<object> GetStyles
        {
            get { return getStyles; }
            set { getStyles = value; RaisePropertyChanged(nameof(GetStyles)); }
        }

        RelayCommand<object> getSingleStyle;
        public RelayCommand<object> GetSingleStyle
        {
            get { return getSingleStyle; }
            set { getSingleStyle = value; RaisePropertyChanged(nameof(GetStyles)); }
        }


        RelayCommand<KeyEventArgs> addContractPaymentDetail;
        public RelayCommand<KeyEventArgs> AddContractPaymentDetail
        {
            get { return addContractPaymentDetail; }
            set { addContractPaymentDetail = value; RaisePropertyChanged(nameof(AddContractPaymentDetail)); }
        }
        
        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
            RaisePropertyChanged(nameof(IsReadOnly));
        }
        public override bool CanAddRecord()
        {
            return base.CanAddRecord() && !CanUnApprove;
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData() && ValidDetailData();
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<ContractHeader> vm =
                new GenericSearchViewModel<ContractHeader>() { Title = "Contracts Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                    var result = new CRUDManagerService.TBLsupplier();
                    result.InjectFrom(SelectedMainRow.Supplier);
                    SupplierPerRow = result;
                }
                RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                if (ValuesObjects != null && ValuesObjects.Any(r => r.Key.StartsWith("Supplier_Ename")))
                {
                    MessageBox.Show("Not Valied search parameter.");
                    return;
                }
                GetMaindata();
            },
            (o) => {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                new SearchColumnModel()
                {
                    Header = strings.StyleCode,
                    PropertyPath = string.Format("{0}.{1}.{2}.{3}.{4}", nameof(ContractHeader.TblContractDetails),
                    nameof(TblContractDetail.TblSalesOrderColor1),nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1),nameof(TblStyle.StyleCode)),
                    FilterPropertyPath =string.Format("{0}.{1}.{2}.{3}.{4}", nameof(ContractHeader.TblContractDetails),
                    nameof(TblContractDetail.TblSalesOrderColor1),nameof(TblSalesOrderColor.TblSalesOrder1),
                    nameof(TblSalesOrder.TblStyle1),nameof(TblStyle.StyleCode)),
                },
                new SearchColumnModel()
                {
                    Header = strings.Code,
                    PropertyPath = nameof(ContractHeader.Code),
                    FilterPropertyPath =nameof(ContractHeader.Code),
                }, new SearchColumnModel()
                {
                    Header = strings.Supplier,
                    PropertyPath = string.Format("{0}.{1}", nameof(ContractHeader.Supplier),nameof(TBLsupplier.Ename)),
                    //FilterPropertyPath = string.Format("{0}.{1}", nameof(ContractHeader.Supplier),nameof(TBLsupplier.Ename)),
                },
                new SearchColumnModel()
                {
                    Header = strings.Brand,
                    PropertyPath =  nameof(ContractHeader.BrandCode),
                    FilterPropertyPath =nameof(ContractHeader.BrandCode),
                },
                new SearchColumnModel()
                {
                    Header = strings.Season,
                    PropertyPath = string.Format("{0}.{1}", nameof(ContractHeader.TblLkpSeason1),nameof(TblLkpSeason.Ename)),
                    FilterPropertyPath =string.Format("{0}.{1}",nameof(ContractHeader.TblLkpSeason1),nameof(TblLkpSeason.Ename)),
                },
                new SearchColumnModel()
                {
                    Header = strings.BrandSection,
                    PropertyPath = string.Format("{0}.{1}", nameof(ContractHeader.TblLkpBrandSection1),nameof(TblLkpBrandSection.Ename)),
                    FilterPropertyPath =string.Format("{0}.{1}",nameof(ContractHeader.TblLkpBrandSection1),nameof(TblLkpBrandSection.Ename)),
                },
                new SearchColumnModel()
                {
                    Header = strings.SubContractor,
                    PropertyPath = string.Format("{0}.{1}", nameof(ContractHeader.TblSubContractor1),nameof(TblSubContractor.Ename)),
                    FilterPropertyPath =string.Format("{0}.{1}",nameof(ContractHeader.TblSubContractor1),nameof(TblSubContractor.Ename)),
                },
                new SearchColumnModel()
                {
                    Header =strings.Date,
                    PropertyPath =nameof(ContractHeader.DocDate),
                    StringFormat ="{0:dd/MM/yyyy h:mm tt}",
                },
                new SearchColumnModel()
                {
                    Header =strings.Approved,
                    PropertyPath =nameof(ContractHeader.Approved),
                },
                new SearchColumnModel()
                {
                    Header =strings.ApproveDate,
                    PropertyPath =nameof(ContractHeader.ApproveDate),
                    StringFormat ="{0:dd/MM/yyyy h:mm tt}",
                },
            };
        }
        public override void DeleteRecord()
        {
            this.DeleteMainRow();
            AddNewMainRow(false);
            base.DeleteRecord();
        }
        public override bool CanDeleteRecord()
        {
            return SelectedMainRow.Iserial > 0 &&
            SelectedMainRow.Approved == false && base.CanDeleteRecord();
        }
        public override void Cancel()
        {
            MainRowList.Clear();
            SelectedMainRows.Clear();
            AddNewMainRow(false);
            RaisePropertyChanged(nameof(IsReadOnly));
            base.Cancel();
        }
        public override void Print()
        {
            base.Print();
            var rVM = new GenericReportViewModel();

            string reportName = SelectedMainRow.ContractReport;
            //var report = BrandContractReportList.FirstOrDefault(r => r.BrandCode == SelectedMainRow.BrandCode);
            //if (report != null && !string.IsNullOrWhiteSpace(report.ReportName))
            //    reportName = report.ReportName;
            rVM.GenerateReport(reportName, new ObservableCollection<string>() { SelectedMainRow.Iserial.ToString() });
        }

        #endregion
    }
}