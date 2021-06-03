using CCWFM.BankDepositService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.InputValidators;
using CCWFM.Models;
using CCWFM.Models.Gl;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.Gl;
using CCWFM.Views.OGView.SearchChildWindows;
using GalaSoft.MvvmLight.Command;
using Omu.ValueInjecter.Silverlight;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.ViewModel.Gl
{
    public class CashDepositHeader : TblCashDepositHeader
    {
        public CashDepositHeader()
        {
            DocDate = DateTime.Now;
            CreatedBy = LoggedUserInfo.Iserial;
            CreationDate = DateTime.Now;
        }

        public new int Iserial
        {
            set { base.Iserial = value; RaisePropertyChanged(nameof(NoBank)); }
            get { return base.Iserial; }
        }

        [Range(1, int.MaxValue)]
        public new int TblStore
        {
            set { base.TblStore = value; }
            get { TblCashDepositDetails.ForEach(cd => cd.GenerateLedgerDescription(true)); return base.TblStore; }
        }

        public new int? TblBank
        {
            set { base.TblBank = value; TblCashDepositDetails.ForEach(cd => cd.GenerateLedgerDescription(true)); }
            get { return base.TblBank; }
        }


   
        public new bool IsSubSafe
        {
            get { return base.IsSubSafe; }
            set { base.IsSubSafe = value; RaisePropertyChanged(nameof(NoBank)); if (base.IsSubSafe) { TblBank = null; TblBank1 = null; } }
        }

        [NotEqualNumber(0)]
        public new decimal Amount
        {
            set { base.Amount = value; }
            get
            {
                return this.TblCashDepositAmountDetails.Count > 0 ?
                  (base.Amount = this.TblCashDepositAmountDetails.Distinct().Sum(d => d.Amount)) :
                  base.Amount;
            }
        }

        int tblCashDepositType;
        [Range(1, int.MaxValue)]
        public int TblCashDepositType
        {
            set
            {
                tblCashDepositType = value; RaisePropertyChanged(nameof(NoBank));
                if (value == 4 || value == 5) { TblBank = null; TblBank1 = null; }
            }
            get { return tblCashDepositType; }
        }

        //int depositeTypeGroup;
        //[Range(1, int.MaxValue)]
        //public int DepositeTypeGroup
        //{
        //    set
        //    {
        //        depositeTypeGroup = value; RaisePropertyChanged(nameof(NoBank));
        //        if (value == 4 || value == 5) { TblBank = null; TblBank1 = null; }
        //    }
        //    get { return depositeTypeGroup; }
        //}

        string _reverseSequence;
        public string ReverseSequance
        {
            set
            {
                _reverseSequence = value; RaisePropertyChanged(nameof(ReverseSequance));
            }
            get { return _reverseSequence; }
        }

        public new string Sequance
        {
            get { return base.Sequance; }
            set { base.Sequance = value; TblCashDepositDetails.ForEach(cd => cd.GenerateLedgerDescription(true)); }
        }


        public new DateTime? DocDate
        {
            get { return base.DocDate; }
            set
            {
                base.DocDate = value; GenerateLedgerDescription();
                TblCashDepositDetails.ForEach(cd => cd.GenerateLedgerDescription(true));
            }
        }

        public new TblTenderType TblTenderType1
        {
            set
            {
                base.TblTenderType1 = value;
                if (value != null && value.TblCashDepositTypes.Count > 0)
                { tblCashDepositType = value.TblCashDepositTypes.FirstOrDefault().Iserial; }
            }
            get { return base.TblTenderType1; }
        }

        public bool NoBank
        {
            get { return TblCashDepositType == 4 || TblCashDepositType == 5 || TblCashDepositType == 6 || TblCashDepositType == 7 || TblCashDepositType == 8 || TblCashDepositType == 10 || TblCashDepositType == 11 || IsSubSafe || TblCashDepositType == 12; }
        }

        public new string LedgerDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(base.LedgerDescription))
                    GenerateLedgerDescription();
                return base.LedgerDescription;
            }
            set { base.LedgerDescription = value; RaisePropertyChanged(nameof(LedgerDescription)); }
        }

        private void GenerateLedgerDescription()
        {
            if (DocDate.HasValue && TblCashDepositTypeRec != null &&
                !string.IsNullOrWhiteSpace(TblCashDepositTypeRec.LedgerDescriptionHeader))
            {
                base.LedgerDescription = string.Format(
                 TblCashDepositTypeRec.LedgerDescriptionHeader, DocDate.Value.ToString("MM-yyyy"));
                RaisePropertyChanged(nameof(LedgerDescription));
            }
        }

        TblCashDepositType tblCashDepositTypeRec;
        public TblCashDepositType TblCashDepositTypeRec
        {
            set
            {
                tblCashDepositTypeRec = value; RaisePropertyChanged(nameof(LedgerDescription));
                RaisePropertyChanged(nameof(TblCashDepositTypeRec)); GenerateLedgerDescription();
            }
            get { return tblCashDepositTypeRec; }
        }

        ObservableCollection<CashDepositDetail> cashDepositDetails;
        public new ObservableCollection<CashDepositDetail> TblCashDepositDetails
        {
            get { return cashDepositDetails ?? (cashDepositDetails = new ObservableCollection<CashDepositDetail>()); }
            set { cashDepositDetails = value; RaisePropertyChanged(nameof(TblCashDepositDetails)); }
        }
        ObservableCollection<CashDepositAmountDetail> cashDepositAmountDetails;
        public new ObservableCollection<CashDepositAmountDetail> TblCashDepositAmountDetails
        {
            get { return cashDepositAmountDetails ?? (cashDepositAmountDetails = new ObservableCollection<CashDepositAmountDetail>()); }
            set { cashDepositAmountDetails = value; RaisePropertyChanged(nameof(TblCashDepositAmountDetails)); }
        }
    }
    public class CashDepositDetail : TblCashDepositDetail
    {
        public CashDepositDetail()
        {
            DueDate = null;
            BatchDate = null;
        }

        [NotEqualNumber(0)]
        public new decimal Amount
        {
            get { return base.Amount; }
            set { base.Amount = value; RaisePropertyChanged(nameof(CashDepositHeader.Amount)); }
        }

        public new string BatchNo
        {
            get { return base.BatchNo; }
            set { base.BatchNo = value; GenerateLedgerDescription(true); }
        }

        public new TblBank TblBank1
        {
            get { return base.TblBank1; }
            set
            {
                base.TblBank1 = value;
                if (value != null && value.Iserial > 0)
                    JournalAccountTypePerRow = null;
            }
        }

        public new string LedgerDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(base.LedgerDescription))
                {
                    return GenerateLedgerDescription(false);
                }
                return base.LedgerDescription;
            }
            set { base.LedgerDescription = value; RaisePropertyChanged(nameof(LedgerDescription)); }
        }

        internal string GenerateLedgerDescription(bool overrite)
        {
            var header = TblCashDepositHeader1 as CashDepositHeader;
            if (header != null && header.DocDate.HasValue &&
                (string.IsNullOrWhiteSpace(base.LedgerDescription) || overrite))
            {
                var typeRec = header.TblCashDepositTypeRec;
                if (typeRec == null) return base.LedgerDescription;
                var type = (CashDepositType)typeRec.Iserial;
                base.LedgerDescription = string.Format(typeRec.LedgerDescriptionDetail,
                  header.Sequance ?? "",//0
                  header.DocDate.Value.ToString("MM-yyyy"),//1
                  header.TblStore1 == null ? "" : header.TblStore1.aname,//2
                  EntityPerRow.Aname,//3
                  header.TblBank1 == null ? "" : header.TblBank1.Ename,//4
                  BatchNo//5
                  );
            }
            RaisePropertyChanged(nameof(LedgerDescription));
            return base.LedgerDescription;
        }

        GlService.Entity _entityAccount = new GlService.Entity();
        public GlService.Entity EntityPerRow
        {
            get { return _entityAccount ?? (_entityAccount = new GlService.Entity()); }
            set
            {
                _entityAccount = value;
                RaiseEntityChanged();
                if (value != null)
                {
                    EntityAccount = value.Iserial;
                    TblJournalAccountType = value.TblJournalAccountType;
                }
                GenerateLedgerDescription(true);
            }
        }

        internal void RaiseEntityChanged()
        {
            RaisePropertyChanged(nameof(EntityPerRow));
        }

        GlService.GenericTable _journalAccountTypePerRow;
        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if (_journalAccountTypePerRow == null || !_journalAccountTypePerRow.Equals(value))
                    EntityPerRow = null;
                _journalAccountTypePerRow = value;
                RaisePropertyChanged(nameof(JournalAccountTypePerRow));
                if (JournalAccountTypePerRow != null && JournalAccountTypePerRow.Iserial > 0)
                { RaiseEntityChanged(); TblBank1 = null; TblBank = null; }
            }
        }
    }
    public class CashDepositAmountDetail : TblCashDepositAmountDetail
    {
        [Required]
        //[RegularExpression("/.(?!0)/g")]
        [Range(long.MinValue, long.MaxValue)]
        public new decimal Amount
        {
            get { return base.Amount; }
            set { base.Amount = value; if (TblCashDepositHeader1 != null) TblCashDepositHeader1.Amount = -3; }
        }
        public bool IsSaved { get { return Iserial > 0; } }

        private bool isQuantityFocused = true;
        public bool IsQuantityFocused
        {
            get { return isQuantityFocused; }
            set { isQuantityFocused = value; RaisePropertyChanged(nameof(IsQuantityFocused)); }
        }
    }
    public class CashDepositViewModel : ViewModelStructuredBase
    {
        bool Approving = false;
        int PremiumBankIserial = 94;
        int PremiumBank2030Iserial = 115;

        int TFKDiscountBankIserial = 94;

        BankDepositServiceClient BankDepositClient = Helpers.Services.Instance.GetBankDepositServiceClient();
        CashDepositDetailView detailView = null;
        public CashDepositViewModel() : base(PermissionItemName.CashDeposit)
        {




            //BankDepositClient.GetDailySalesCommisionAsync(0, new DateTime(2018, 3, 25), new DateTime(2018, 3, 30), LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
            if (!DesignerProperties.IsInDesignTool)
            {
                this.PremCompleted += (s, sv) =>
                {
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositApprove") != null)
                    {
                        CanApprove = true;
                    }
                    // مش شغالة مش هتحذف من الكاستومر ولا الليدجر
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositUnApprove123") != null)//اسم الصلاحية متغير فمش هتشتغل
                    {
                        CanUnApprove = true;
                    }
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositCancel") != null)
                    {
                        CanCancel = true;
                    }
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositDetail") != null)
                    {
                        CanAddDetail = true;
                    }
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositDetailVisa") != null)
                    {
                        CanAddDetailVisa = true;
                    }
                    
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositDetailPremium") != null)
                    {
                        CanAddDetailPremium = true;
                    }
                    if (this.CustomePermissions.SingleOrDefault(x => x.Code == "CashDepositDetailTFKDiscount15") != null)
                    {
                        CanAddDetailTFKDiscount15 = true;
                    }
                };
                this.GetCustomePermissions(PermissionItemName.CashDeposit.ToString());
                BankDepositClient.GetPremiumBankIserialCompleted += (s, e) =>
                {
                    PremiumBankIserial = e.Result;
                };
                BankDepositClient.GetPremiumBankIserialAsync(LoggedUserInfo.DatabasEname);

                BankDepositClient.GetPremium2030BankIserialCompleted  += (s, e) =>
                {
                    PremiumBank2030Iserial = e.Result;
                };
                BankDepositClient.GetPremium2030BankIserialAsync(LoggedUserInfo.DatabasEname);

                BankDepositClient.GetCashDepositSettingAsync(LoggedUserInfo.DatabasEname);

                BankDepositClient.GetCashDepositSettingCompleted += (s, e) => {
                    EntityList=  e.entityList;
                    CashDepositSetting=e.Result;
                };

                BankDepositClient.GetTFKDiscountBankIserialCompleted += (s, e) =>
                {
                    TFKDiscountBankIserial  = e.Result;
                };
                BankDepositClient.GetTFKDiscountBankIserialAsync(LoggedUserInfo.DatabasEname);

                BankDepositClient.GetCashDepositSettingAsync(LoggedUserInfo.DatabasEname);

                BankDepositClient.GetCashDepositSettingCompleted += (s, e) => {
                    EntityList = e.entityList;
                    CashDepositSetting = e.Result;
                };

                ApproveCashDeposit = new RelayCommand(() =>
                {
                    Approving = true;
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    //SaveRecord();
                    SelectedMainRow.Approved = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    //SaveCommand.Execute(null);
                    SaveRecord();
                    Approving = false;
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                        NewCommand.Execute(null);
                }, () => CheckCanApprove() && !Approving);


                ReverseCashDeposit = new RelayCommand(() =>
                {
                     ReveseRecord();
                     ReverseCashDeposit.RaiseCanExecuteChanged();
                 });


                UnApproveCashDeposit = new RelayCommand(() =>
                {
                    //SaveCommand.Execute(null);
                    SaveRecord();
                    SelectedMainRow.Approved = false;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    //SaveCommand.Execute(null);
                    SaveRecord();
                    if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                        NewCommand.Execute(null);
                }, () => CheckCanUnApprove());
                CancelCashDeposit = new RelayCommand(() =>
                {
                    SelectedMainRow.Canceled = true;
                    SelectedMainRow.ApproveDate = DateTime.Now;
                    SelectedMainRow.ApprovedBy = LoggedUserInfo.Iserial;
                    //SaveCommand.Execute(null);
                    SaveRecord();
                    if (SelectedMainRow.Approved)//كده نفذ فهعمل جديد
                        NewCommand.Execute(null);
                }, () => CheckCanCancel());

                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    if (SelectedMainRow.TblCashDepositDetails.Count < PageSize)
                    {
                        return;
                    }
                    if (SelectedMainRow.TblCashDepositDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    {
                        GetDetailData();
                    }
                });

                NewDetail = new RelayCommand<object>((o) =>
                {
                    if (o == null || ((KeyEventArgs)(o)).Key == Key.Down &&
                    !SelectedMainRow.Approved)
                    {
                        var lastRow = SelectedMainRow.TblCashDepositDetails.LastOrDefault();
                        AddNewDetailRow(true);
                        if (lastRow != null)
                        {
                            SelectedDetailRow.TblBank = lastRow.TblBank;
                            SelectedDetailRow.TblBank1 = lastRow.TblBank1;
                            if (lastRow.BatchDate.HasValue && !SelectedDetailRow.BatchDate.HasValue)
                                if (AutoIncrement)
                                    SelectedDetailRow.BatchDate = lastRow.BatchDate.Value.AddDays(1);
                                else
                                    SelectedDetailRow.BatchDate = lastRow.BatchDate.Value;
                            CashDepositType type = (CashDepositType)Enum.ToObject(typeof(CashDepositType), SelectedMainRow.TblCashDepositType);
                            switch (type)
                            {
                                case CashDepositType.Cheque:
                                    // if selectedType.addDueDate ==true
                                    //{ }
                                    SelectedDetailRow.DueDate = lastRow.DueDate;
                                    break;
                                case CashDepositType.Visa:
                                    SelectedDetailRow.MachineId = lastRow.MachineId;
                                    break;
                                case CashDepositType.Cash:
                                    break;
                                case CashDepositType.Expences:
                                case CashDepositType.Discount:
                                    SelectedDetailRow.TblBank = null;
                                    SelectedDetailRow.TblBank1 = null;
                                    break;

                                case CashDepositType.DSquaresCIB:
                                    SelectedDetailRow.TblBank = null;
                                    SelectedDetailRow.TblBank1 = null;
                                    SelectedDetailRow.TblJournalAccountType = lastRow.TblJournalAccountType;
                                    SelectedDetailRow.JournalAccountTypePerRow = lastRow.JournalAccountTypePerRow;
                                    SelectedDetailRow.EntityAccount = lastRow.EntityAccount;
                                    SelectedDetailRow.EntityPerRow = lastRow.EntityPerRow;
                                    break;

                                case CashDepositType.DsquaresLuckyWallet:
                                    SelectedDetailRow.TblBank = null;
                                    SelectedDetailRow.TblBank1 = null;
                                    SelectedDetailRow.TblJournalAccountType = lastRow.TblJournalAccountType;
                                    SelectedDetailRow.JournalAccountTypePerRow = lastRow.JournalAccountTypePerRow;
                                    SelectedDetailRow.EntityAccount = lastRow.EntityAccount;
                                    SelectedDetailRow.EntityPerRow = lastRow.EntityPerRow;
                                    break;
                                case CashDepositType.TFKCourier:

                                    SelectedDetailRow.TblBank = null;
                                    SelectedDetailRow.TblBank1 = null;

                                    //if (LoggedUserInfo.DatabasEname.ToLower() == "ccnew")
                                    //{
                                    //    SelectedDetailRow.TblBank = 113;
                                    //    SelectedDetailRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                                    //}
                                    //else if (LoggedUserInfo.DatabasEname.ToLower() == "sw")
                                    //{
                                    //    SelectedDetailRow.TblBank = 116;
                                    //    SelectedDetailRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                                    //}


                                    SelectedDetailRow.TblJournalAccountType = lastRow.TblJournalAccountType;
                                    SelectedDetailRow.JournalAccountTypePerRow = lastRow.JournalAccountTypePerRow;
                                    SelectedDetailRow.EntityAccount = lastRow.EntityAccount;
                                    SelectedDetailRow.EntityPerRow = lastRow.EntityPerRow;
                                    break;

                                case CashDepositType.TFKCash:
                                    break;
                            }
                        }
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(TotalDifference));
                }, (o) =>
                {
                    return !SelectedMainRow.Approved && !SelectedMainRow.Canceled;
                });
                NewAmountDetail = new RelayCommand<object>((o) =>
                {
                    var e = o as SelectionChangedEventArgs;
                    if (((KeyEventArgs)(o)).Key == Key.Down &&
                    !SelectedMainRow.Approved && !SelectedMainRow.Canceled)
                    {
                        var lastRow = SelectedMainRow.TblCashDepositAmountDetails.LastOrDefault();
                        AddNewAmountDetailRow(true);
                        if (lastRow != null)
                        {
                            SelectedAmountDetailRow.Notes = lastRow.Notes;
                        }
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    //SelectedMainRow.Amount = SelectedMainRow.Amount;
                    RaisePropertyChanged(nameof(AmountTotal));
                    RaisePropertyChanged(nameof(TotalDifference));
                });

                OpenDetail = new RelayCommand<object>((o) =>
                {
                    detailView = new CashDepositDetailView();
                    detailView.DataContext = this;
                    detailView.CashDepositType = (CashDepositType)Enum.ToObject(
                        typeof(CashDepositType), SelectedMainRow.TblCashDepositType);
                    detailView.Closing += (s, e) =>
                    {
                        ApproveCashDeposit.RaiseCanExecuteChanged();
                        CancelCashDeposit.RaiseCanExecuteChanged();
                    };
                    if (detailView.CashDepositType == CashDepositType.TFKCourier)
                    {
                        if(SelectedMainRow.TblCashDepositDetails.Count == 0)
                        {
                            AddNewDetailRow(false);
                        }
                    }
                   
                    //NewDetail.Execute(null);
                    detailView.Show();
                }, (o) =>
                {
                    var type = (CashDepositType)Enum.ToObject(
                          typeof(CashDepositType), SelectedMainRow.TblCashDepositType);

                    //var DepositeTypeGroup = (CashDepositType)Enum.ToObject(
                    //     typeof(CashDepositType), SelectedMainRow.TblCashDepositTypeRec.DepositeTypeGroup);

                    bool hasCashPermission = CanAddDetail && (type == CashDepositType.Cash || type == CashDepositType.TFKCash ||
                        type == CashDepositType.Cheque ||
                        type == CashDepositType.Discount ||
                        type == CashDepositType.Expences);
                    //PremiumCard2030
                    bool hasPremiumPermission = CanAddDetailPremium && (type == CashDepositType.PremiumCard) ;
                    //bool hasPremiumPermission = CanAddDetailPremium && DepositeTypeGroup == CashDepositType.PremiumCard;
                    bool hasTFKDiscountPermission = CanAddDetailTFKDiscount15 && type == CashDepositType.TFKDiscount15;
                    bool hasVisaPermission = CanAddDetailVisa && type == CashDepositType.Visa;



                    return SelectedMainRow.Iserial > 0 && (hasCashPermission ||
                    hasPremiumPermission || hasVisaPermission || CanOpenDetail || hasTFKDiscountPermission);
                });
                SaveDetails = new RelayCommand<object>((o) =>
                {
                    // if (TotalDifference != 0 || MessageBox.Show("Difference is not zero are you sure to save?", "Save",
                    //MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    //     return;
                    // لو هتشغله لازم تشوف الفرق هيتغير امتى وتحدث الكوماند والا هيفضل شايف الفرق زى اول ما الشاشة اتفتحت
                    if (ValidDetailData() && !SelectedMainRow.Approved && !SelectedMainRow.Canceled)
                    {
                        var toSaveList = new ObservableCollection<TblCashDepositDetail>();
                        foreach (var item in SelectedMainRow.TblCashDepositDetails)
                        {
                            var valiationCollection = new List<ValidationResult>();

                            var isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null),
                                valiationCollection, true);
                            if (isvalid)
                            {
                                var rowToSave = new TblCashDepositDetail();
                                rowToSave.InjectFrom(item);
                                rowToSave.TblCashDepositHeader1 = null;
                                toSaveList.Add(rowToSave);
                            }
                        }
                        Loading = true;
                        //////////
                        BankDepositClient.UpdateOrInsertCashDepositDetailsAsync(
                            toSaveList, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
                    }
                }, (o) =>
                {
                    return (CanAddDetail || CanAddDetailPremium || CanAddDetailVisa || CanAddDetailTFKDiscount15) &&
                        SelectedMainRow.Iserial > 0 && !SelectedMainRow.Approved && !Loading && !SelectedMainRow.Canceled;
                });
                SaveAmountDetails = new RelayCommand<object>((o) =>
                {
                    if (ValidAmountDetailData() && !SelectedMainRow.Approved)
                    {
                        foreach (var item in SelectedMainRow.TblCashDepositAmountDetails)
                        {
                            var valiationCollection = new List<ValidationResult>();

                            var isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);

                            if (isvalid)
                            {
                                var rowToSave = new TblCashDepositAmountDetail();
                                rowToSave.InjectFrom(item);
                                BankDepositClient.UpdateOrInsertCashDepositAmountDetailAsync(rowToSave,
                                    SelectedMainRow.TblCashDepositAmountDetails.IndexOf(item), LoggedUserInfo.Iserial,
                                    LoggedUserInfo.DatabasEname);
                            }
                        }
                    }
                }, (o) =>
                {
                    return SelectedMainRow.Iserial > 0 && !SelectedMainRow.Approved && !SelectedMainRow.Canceled;
                });

                MainRowList = new ObservableCollection<CashDepositHeader>();

        
                BankDepositClient.GetCashDepositHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new CashDepositHeader()
                        {
                            TblCashDepositType = 3,
                        };

                        newrow.InjectFrom(row);
                        newrow.TblCashDepositTypeRec = CashDepositTypeList.FirstOrDefault(r => r.Iserial == newrow.TblCashDepositType);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (SearchWindow != null)
                    {
                        SearchWindow.FullCount = sv.fullCount;
                        SearchWindow.Loading = false;
                    }
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };
                BankDepositClient.UpdateOrInsertCashDepositHeaderCompleted += (s, x) => SaveCompleted(x.Result, x.outindex, x.Error);
                //BankDepositClient.ApproveCashDepositHeaderCompleted += (s, x) => SaveCompleted(x.Result, x.outindex, x.Error);
                 
                BankDepositClient.GetCashDepositAmountDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new CashDepositAmountDetail();
                        newrow.InjectFrom(row);
                        SelectedMainRow.TblCashDepositAmountDetails.Add(newrow);
                        newrow.TblCashDepositHeader1 = SelectedMainRow;
                    }
                    if (!SelectedMainRow.TblCashDepositAmountDetails.Any())
                    {
                        AddNewAmountDetailRow(false);
                    }
                    Loading = false;
                    RaisePropertyChanged(nameof(AmountTotal));
                    RaisePropertyChanged(nameof(TotalDifference));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    CancelCashDeposit.RaiseCanExecuteChanged();
                };
                BankDepositClient.UpdateOrInsertCashDepositAmountDetailCompleted += (s, x) =>
                {
                    CashDepositAmountDetail savedRow = new CashDepositAmountDetail();
                    if (x.outindex >= 0)
                        savedRow = SelectedMainRow.TblCashDepositAmountDetails.ElementAt(x.outindex);

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblCashDepositHeader1 = SelectedMainRow;
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    CancelCashDeposit.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(AmountTotal));
                    RaisePropertyChanged(nameof(TotalDifference));
                };

                BankDepositClient.GetCashDepositDetailCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new CashDepositDetail();
                        newrow.InjectFrom(row);
                        newrow.TblBank = row.TblBank;
                        newrow.TblBank1 = BankList.FirstOrDefault(btt => btt.Iserial == newrow.TblBank);

                        newrow.JournalAccountTypePerRow = new GlService.GenericTable();
                        if (newrow.TblJournalAccountType.HasValue)
                        {
                            newrow.JournalAccountTypePerRow =
                                JournalAccountTypeList.FirstOrDefault(jAT => jAT.Iserial == newrow.TblJournalAccountType.Value);
                        }

                        var entity = sv.EntityAccounts.FirstOrDefault(x => x.TblJournalAccountType == newrow.TblJournalAccountType &&
                                                         x.Iserial == row.EntityAccount);
                        if (entity != null)
                        {
                            newrow.EntityPerRow.InjectFrom(entity); newrow.RaiseEntityChanged();
                        }

                        SelectedMainRow.TblCashDepositDetails.Add(newrow);
                        newrow.TblCashDepositHeader1 = SelectedMainRow;
                    }
                    if (!SelectedMainRow.TblCashDepositDetails.Any())
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(TotalDifference));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    CancelCashDeposit.RaiseCanExecuteChanged();
                };
                BankDepositClient.UpdateOrInsertCashDepositDetailsCompleted += (s, x) =>
                {
                    SelectedMainRow.TblCashDepositDetails.Clear();
                    foreach (var item in x.Result)
                    {
                        var savedRow = new CashDepositDetail();

                        savedRow.InjectFrom(item);
                        savedRow.TblBank = item.TblBank;
                        savedRow.TblBank1 = BankList.FirstOrDefault(bt => bt.Iserial == savedRow.TblBank);
                        SelectedMainRow.TblCashDepositDetails.Add(savedRow);
                        savedRow.TblCashDepositHeader1 = SelectedMainRow;
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    CancelCashDeposit.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(TotalDifference));
                    if (x.Result.Count > 0)
                        if (CloseAfterSave)
                        { if (detailView != null) detailView.Close(); }
                        else
                            MessageBox.Show(strings.SavedMessage);
                    Loading = false;
                };

                #region Delete

                DeleteCashDepositDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete && !SelectedMainRow.Approved && !SelectedMainRow.Canceled)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblCashDepositDetails.Remove(SelectedDetailRow);
                            if (SelectedMainRow.TblCashDepositDetails.Count == 0)
                            {
                                AddNewDetailRow(false);
                            }
                        }
                        else
                            DeleteDetailRow();
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(TotalDifference));
                }, (o) =>
                {
                    return SelectedMainRow != null && !SelectedMainRow.Approved && !SelectedMainRow.Canceled;
                });
                DeleteCashDepositAmountDetail = new RelayCommand<object>((o) =>
                {
                    if (((KeyEventArgs)(o)).Key == Key.Delete && !SelectedMainRow.Approved && !SelectedMainRow.Canceled &&
                            Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (SelectedMainRow.Iserial <= 0 || SelectedAmountDetailRow.Iserial <= 0)
                        {
                            SelectedMainRow.TblCashDepositAmountDetails.Remove(SelectedAmountDetailRow);
                            if (SelectedMainRow.TblCashDepositAmountDetails.Count == 0)
                            {
                                AddNewAmountDetailRow(false);
                            }
                        }
                    }
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                    RaisePropertyChanged(nameof(AmountTotal));
                    RaisePropertyChanged(nameof(TotalDifference));
                }, (o) =>
                {
                    return SelectedMainRow != null && !SelectedMainRow.Approved && !SelectedMainRow.Canceled;
                });
                //BankDepositClient.DeleteCashDepositHeaderCompleted += (s, ev) =>
                //{
                //    if (ev.Error != null)
                //    {
                //        throw ev.Error;
                //    }

                //    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                //    if (oldrow != null) MainRowList.Remove(oldrow);
                //};
                BankDepositClient.DeleteCashDepositDetailCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.TblCashDepositDetails.FirstOrDefault(x => x.Iserial == ev.Result.Iserial);
                    if (oldrow != null) SelectedMainRow.TblCashDepositDetails.Remove(oldrow);
                    RaisePropertyChanged(nameof(Total));
                    RaisePropertyChanged(nameof(TotalDifference));
                    RaisePropertyChanged(nameof(IsHeaderHasDetails));
                };

                #endregion

                BankDepositClient.GetLookUpStoreCompleted += (s, e) =>
                {
                    StoreList = e.Result;
                };
                BankDepositClient.GetLookUpBankCompleted += (s, e) =>
                {
                    BankList = e.Result;
                };
                BankDepositClient.GetLookUpCashDepositTypeCompleted += (s, e) =>
                {
                    CashDepositTypeList = e.Result;
                };
                BankDepositClient.GetLookUpTenderTypesCompleted += (s, e) =>
                {
                    try
                    {
                        TenderTypeList = e.Result;
                        var tenderType = TenderTypeList.FirstOrDefault(t => t.ISerial == SelectedMainRow.TblTenderType);
                        if (tenderType == null)
                            tenderType = TenderTypeList.FirstOrDefault();
                        SelectedMainRow.TblTenderType1 = tenderType;
                        SelectedMainRow.TblTenderType = tenderType.ISerial;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "-----------------" + ex.StackTrace);
                    }
                };

                BankDepositClient.GetCashDepositHeaderByIserialCompleted += (s, e) =>
                {
                    SelectedMainRow.InjectFrom(e.Result);
                    SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                    TblCashDepositTypeRec = CashDepositTypeList.FirstOrDefault(c => c.Iserial == SelectedMainRow.TblCashDepositType);
                    if (SelectedMainRow.TblCashDepositDetails != null)
                        SelectedMainRow.TblCashDepositDetails = new ObservableCollection<CashDepositDetail>();
                    if (SelectedMainRow.TblCashDepositAmountDetails != null)
                        SelectedMainRow.TblCashDepositAmountDetails = new ObservableCollection<CashDepositAmountDetail>();
                    RaisePropertyChanged(nameof(SelectedMainRow));
                    OpenDetail.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                    ApproveCashDeposit.RaiseCanExecuteChanged();
                    UnApproveCashDeposit.RaiseCanExecuteChanged();
                    CancelCashDeposit.RaiseCanExecuteChanged();
                    //DeleteCashDepositDetail.RaiseCanExecuteChanged();
                    IsNewChanged();
                    GetDetailData();
                };

                BankDepositClient.GetMachineIdCompleted += (s, e) =>
                {
                    //كمل تخزين نسب العمولة علشان التقارير ولو اتغيرت تكون عارف
                    //if (e.Result.TblBank == SelectedDetailRow.TblBank)
                    SelectedDetailRow.MachineId = e.Result.MachineId;
                    SelectedDetailRow.DiscountPercent = e.Result.DiscountPercent;
                    //else
                    //    SelectedDetailRow.MachineId = string.Empty;
                };
                BankChanged = new RelayCommand<SelectionChangedEventArgs>((o) =>
                {
                    TblBank tblBank = null;
                    if (o.AddedItems.Count > 0)
                        tblBank = o.AddedItems[0] as TblBank;
                    if (tblBank != null && (CashDepositType)Enum.ToObject(
                        typeof(CashDepositType), SelectedMainRow.TblCashDepositType) == CashDepositType.Visa)
                    {
                        BankDepositClient.GetMachineIdAsync(SelectedMainRow.TblStore,
                            tblBank.Iserial, LoggedUserInfo.DatabasEname);    //SelectedDetailRow.TblBank.Value);
                    }
                });
                PrintAll = new RelayCommand<object>((o) =>
                {
                    //BankDepositClient.testStoreMailAsync();
                    var rVM = new GenericReportViewModel();
                    var para = new ObservableCollection<string>();
                    para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
                    para.Add(LoggedUserInfo.DatabasEname);
                    rVM.GenerateReport("CashDepositeMultiDocument", para);
                });

                //BankDepositClient.testStoreMailCompleted += (s, e) => {
                //    if (e.Error != null) MessageBox.Show(e.Error.Message);
                //    else MessageBox.Show("sssssssssssss");
                //};

                Glclient.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (JournalAccountTypeList.All(x => x.Iserial != row.Iserial))
                        {
                            JournalAccountTypeList.Add(row);
                        }
                    }
                };

                GetComboData();
                GetMaindata();
                AddNewMainRow(false);
            }
        }

        private void ReveseRecord()
        {
           // (int TblCashDepositHeader, string no, string month, string year, string User, string company)
            Glclient.ReveseTBLCashDepoisteHeaderAsync(SelectedMainRow.Iserial, "", "", "",LoggedUserInfo.WFM_UserID.ToString(), LoggedUserInfo.DatabasEname);
            Glclient.ReveseTBLCashDepoisteHeaderCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    SelectedMainRow.ReverseSequance = sv.Result.Sequance;
                }
            };
        }

        private void SaveCompleted(TblCashDepositHeader Result, int outindex, Exception Error)
        {
            if (Error != null)
            {
                MessageBox.Show(Helper.GetInnerExceptionMessage(Error));
            }
            else
                MessageBox.Show(strings.SavedMessage);

            CashDepositHeader savedRow = new CashDepositHeader();
            if (outindex >= 0 && MainRowList.Count > outindex)
                savedRow = MainRowList.ElementAt(outindex);

            if (savedRow != null)
            {
                savedRow.InjectFrom(Result);
                savedRow.TblBank = Result.TblBank;
                savedRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == savedRow.TblBank);
                savedRow.TblStore1 = StoreList.FirstOrDefault(b => b.iserial == savedRow.TblStore);
                TblCashDepositTypeRec = CashDepositTypeList.FirstOrDefault(c => c.Iserial == savedRow.TblCashDepositType);

                savedRow.TblCashDepositDetails.Clear();
                foreach (var item in Result.TblCashDepositDetails)
                {
                    var detailTemp = new CashDepositDetail();
                    detailTemp.InjectFrom(item);
                    savedRow.TblCashDepositDetails.Add(detailTemp);
                }
                savedRow.TblCashDepositAmountDetails.Clear();
                foreach (var item in Result.TblCashDepositAmountDetails)
                {
                    var detailTemp = new CashDepositAmountDetail();
                    detailTemp.InjectFrom(item);
                    savedRow.TblCashDepositAmountDetails.Add(detailTemp);
                }
            }
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
            RaisePropertyChanged(nameof(IsReadOnly));
            OpenDetail.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            ApproveCashDeposit.RaiseCanExecuteChanged();
            UnApproveCashDeposit.RaiseCanExecuteChanged();
            CancelCashDeposit.RaiseCanExecuteChanged();
            //DeleteCashDepositDetail.RaiseCanExecuteChanged();
            IsNewChanged();
            if (savedRow.Approved)
            {
                BankDepositClient.SendApproveMailAsync(savedRow.Iserial,
                    savedRow.TblStore, savedRow.DocDate.Value, LoggedUserInfo.DatabasEname,
                    savedRow.Sequance, LoggedUserInfo.Iserial);
            }
            Loading = false;
        }

        #region Methods

        private void ValidateDetailRow(CashDepositDetail cashDepositDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblCashDepositDetails.Remove(SelectedDetailRow);
                SelectedMainRow.TblCashDepositDetails.Add(cashDepositDetail);
            }
            else
            {
                SelectedMainRow.TblCashDepositDetails.Add(cashDepositDetail);
            }
            SelectedDetailRow = cashDepositDetail;
            RaisePropertyChanged(nameof(Total));
            RaisePropertyChanged(nameof(TotalDifference));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }
        private void ValidateAmountDetailRow(CashDepositAmountDetail cashDepositAmountDetail)
        {
            var valiationCollection = new List<ValidationResult>();
            var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                SelectedMainRow.TblCashDepositAmountDetails.Remove(SelectedAmountDetailRow);
                SelectedMainRow.TblCashDepositAmountDetails.Add(cashDepositAmountDetail);
            }
            else
            {
                SelectedMainRow.TblCashDepositAmountDetails.Add(cashDepositAmountDetail);
            }
            SelectedAmountDetailRow = cashDepositAmountDetail;
            RaisePropertyChanged(nameof(AmountTotal));
            RaisePropertyChanged(nameof(TotalDifference));
            RaisePropertyChanged(nameof(IsHeaderHasDetails));
        }
        private bool CheckCanApprove()
        {
            return CanApprove && !SelectedMainRow.Approved && !SelectedMainRow.Canceled &&
                SelectedMainRow.Iserial > 0 &&
                SelectedMainRow.TblCashDepositAmountDetails.Sum(d => d.Amount) ==
                SelectedMainRow.TblCashDepositDetails.Sum(d => d.Amount);
        }
        private bool CheckCanUnApprove()
        {
            return CanUnApprove && SelectedMainRow.Approved && !SelectedMainRow.Canceled && SelectedMainRow.Iserial > 0;
        }
        private bool CheckCanCancel()
        {
            return CanCancel && !SelectedMainRow.Approved && !SelectedMainRow.Canceled &&
                SelectedMainRow.Iserial > 0;
        }
        public bool ValidHeaderData()
        {
            if (SelectedMainRow.TblCashDepositType < 0)
            {
                MessageBox.Show(strings.ReqCashDepositType);
                return false;
            }
            if (!SelectedMainRow.DocDate.HasValue)
            {
                MessageBox.Show(strings.ReqDate);
                return false;
            }
            // To do Add RequireBank Field In TblCashDepositType
            if ((SelectedMainRow.TblBank == null || SelectedMainRow.TblBank <= 0) &&//bank Not selected
                !SelectedMainRow.IsSubSafe && // Not SubSafe salah 5tab
                SelectedMainRow.TblCashDepositType != 4 && 
                SelectedMainRow.TblCashDepositType != 5 &&
                SelectedMainRow.TblCashDepositType != 6 &&
                SelectedMainRow.TblCashDepositType != 7 &&
                SelectedMainRow.TblCashDepositType != 12)
            {
                MessageBox.Show(strings.ReqBankAccountNo);
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (SelectedMainRow.TblCashDepositDetails.Any(td => 0 == td.Amount))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            if (SelectedMainRow.TblCashDepositDetails.Any(td => string.IsNullOrWhiteSpace(td.BatchNo)))
            {
                MessageBox.Show(strings.ReqBatchNo);
                return false;
            }
            if (SelectedMainRow.TblCashDepositDetails.Any(td => td.BatchDate == null))
            {
                MessageBox.Show(strings.ReqDate);
                return false;
            }
            if ((SelectedMainRow.TblCashDepositType != (int)CashDepositType.Expences &&
                SelectedMainRow.TblCashDepositType != (int)CashDepositType.Discount &&
                SelectedMainRow.TblCashDepositDetails.Any(td => td.TblBank == null &&
                td.JournalAccountTypePerRow == null && td.EntityAccount == null)))
            {
                MessageBox.Show(strings.ReqBankAccountNo);
                return false;
            }
            if ((SelectedMainRow.TblCashDepositType == (int)CashDepositType.Visa &&
                SelectedMainRow.TblCashDepositDetails.Any(td => string.IsNullOrWhiteSpace(td.MachineId))))
            {
                MessageBox.Show(strings.ReqMachineId);
                return false;
            }
            if ((SelectedMainRow.TblCashDepositType == (int)CashDepositType.Cheque &&
                SelectedMainRow.TblCashDepositDetails.Any(td => string.IsNullOrWhiteSpace(td.ChequeNo))))
            {
                MessageBox.Show(strings.ReqChequeNo);
                return false;
            }
            if ((SelectedMainRow.TblCashDepositType == (int)CashDepositType.Cheque &&
                SelectedMainRow.TblCashDepositDetails.Any(td => td.DueDate == null)))
            {
                MessageBox.Show(strings.ReqDate);
                return false;
            }
            return true;
        }
        public bool ValidAmountDetailData()
        {
            if (SelectedMainRow.TblCashDepositAmountDetails.Any(td => 0 == td.Amount))
            {
                MessageBox.Show(strings.CheckQuantities);
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
            BankDepositClient.GetCashDepositHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }
        public void DeleteMainRow()
        {
            if (SelectedMainRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    //BankDepositClient.DeleteCashDepositHeaderAsync((TblCashDepositHeader)new
                    //    TblCashDepositHeader().InjectFrom(SelectedMainRow),
                    //    MainRowList.IndexOf(SelectedMainRow));
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
                SelectedMainRow = new CashDepositHeader()
                {
                    TblCashDepositType = 3,
                    TblCashDepositDetails = new ObservableCollection<CashDepositDetail>(),
                    TblCashDepositAmountDetails = new ObservableCollection<CashDepositAmountDetail>()
                };
                TblCashDepositTypeRec = CashDepositTypeList.FirstOrDefault(t => t.Iserial == 3);
                //MainRowList.Insert(currentRowIndex + 1, SelectedMainRow);
                AddNewDetailRow(false);
                AddNewAmountDetailRow(false);
                OpenDetail.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(AmountTotal));
                RaisePropertyChanged(nameof(TotalDifference));
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
               // if (true)
                {
                    var saveRow = new TblCashDepositHeader()
                    {
                        DocDate = DateTime.Now,
                        CreationDate = DateTime.Now,
                    };
                    saveRow.InjectFrom(SelectedMainRow);
                    if (string.IsNullOrWhiteSpace(SelectedMainRow.Sequance))
                        saveRow.Sequance = "";
                    saveRow.TblCashDepositDetails = new ObservableCollection<TblCashDepositDetail>();
                    foreach (var item in SelectedMainRow.TblCashDepositDetails.Where(d => d.Amount != 0))
                    {
                        var detailTemp = new TblCashDepositDetail();
                        detailTemp.InjectFrom(item);
                        detailTemp.TblCashDepositHeader1 = saveRow;
                        saveRow.TblCashDepositDetails.Add(detailTemp);
                    }
                    saveRow.TblCashDepositAmountDetails = new ObservableCollection<TblCashDepositAmountDetail>();
                    foreach (var item in SelectedMainRow.TblCashDepositAmountDetails.Where(d => d.Amount != 0))
                    {
                        var detailTemp = new TblCashDepositAmountDetail();
                        detailTemp.InjectFrom(item);
                        detailTemp.TblCashDepositHeader1 = saveRow;
                        saveRow.TblCashDepositAmountDetails.Add(detailTemp);
                    }

                    var mainRowIndex = MainRowList.IndexOf(SelectedMainRow);
                    if (mainRowIndex < 0)
                    {
                        MainRowList.Insert(mainRowIndex + 1, SelectedMainRow); mainRowIndex++;
                    }
                    //if (SelectedMainRow.Approved)
                    //    BankDepositClient.ApproveCashDepositHeaderAsync(saveRow,
                    //    LoggedUserInfo.DatabasEname, mainRowIndex, LoggedUserInfo.Iserial);
                    //else
                        BankDepositClient.UpdateOrInsertCashDepositHeaderAsync(saveRow,
                            LoggedUserInfo.DatabasEname, mainRowIndex, LoggedUserInfo.Iserial);
                    Loading = true;
                }
            }
        }
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                BankDepositClient.GetCashDepositDetailAsync(0,
                    //SelectedMainRow.TblCashDepositDetails.Where(d => d.TblBank > 0).Count(),PageSize
                    int.MaxValue, SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
                BankDepositClient.GetCashDepositAmountDetailAsync(0,
                    //SelectedMainRow.TblCashDepositAmountDetails.Where(d => d.Amount != 0).Count(), PageSize
                    int.MaxValue, SelectedMainRow.Iserial, LoggedUserInfo.DatabasEname);
            }
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    var temp = new TblCashDepositDetail();
                    temp.InjectFrom(SelectedDetailRow);
                    temp.TblCashDepositHeader1 = null;
                    BankDepositClient.DeleteCashDepositDetailAsync(temp, LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.TblCashDepositDetails.IndexOf(SelectedDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblCashDepositDetails.Count - 1))
            {
                if (checkLastRow && SelectedDetailRow != null &&
                    SelectedMainRow.TblCashDepositDetails.IndexOf(SelectedDetailRow) >= 0)
                {
                    var valiationCollection = new List<ValidationResult>();
                    var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(
                        SelectedDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedMainRow.TblCashDepositDetails.Insert(currentRowIndex + 1, SelectedDetailRow = new CashDepositDetail
                {
                    TblCashDepositHeader = SelectedMainRow.Iserial,
                    TblCashDepositHeader1 = SelectedMainRow,
                });
                if (SelectedMainRow.TblCashDepositType == (int)CashDepositType.Expences)
                {
                    SelectedDetailRow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(t => t.Iserial == 15);
                }
                else if (SelectedMainRow.TblCashDepositType == (int)CashDepositType.DSquaresCIB)
                {
                    var CashDepositRow = CashDepositSetting.FirstOrDefault(w => w.TblTenderTypes == SelectedMainRow.TblTenderType && w.TblCashDepositType.Value == SelectedMainRow.TblCashDepositType);
                    if (CashDepositRow!=null)
                    {
                        SelectedDetailRow.TblJournalAccountType = CashDepositRow.TblJournalAccountType;
                        SelectedDetailRow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType);
                            //new gene().InjectFrom(
                            // as TblJournalAccountType;
                            //SelectedDetailRow.JournalAccountTypePerRow = new TblJournalAccountType().InjectFrom(JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType)) as TblJournalAccountType;
                        SelectedDetailRow.EntityAccount = CashDepositRow.EntityAccount;
                        SelectedDetailRow.EntityPerRow = new GlService.Entity().InjectFrom( EntityList.FirstOrDefault(w => w.Iserial == CashDepositRow.EntityAccount && w.TblJournalAccountType == CashDepositRow.TblJournalAccountType)) as GlService.Entity;
                        SelectedDetailRow.DiscountPercent = CashDepositRow.DiscountPercent;
                    }                 
                }
                else if (SelectedMainRow.TblCashDepositType == (int)CashDepositType.DsquaresLuckyWallet)
                {
                    var CashDepositRow = CashDepositSetting.FirstOrDefault(w => w.TblTenderTypes == SelectedMainRow.TblTenderType && w.TblCashDepositType.Value == SelectedMainRow.TblCashDepositType);
                    if (CashDepositRow != null)
                    {
                        SelectedDetailRow.TblJournalAccountType = CashDepositRow.TblJournalAccountType;
                        SelectedDetailRow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType);
                        //new gene().InjectFrom(
                        // as TblJournalAccountType;
                        //SelectedDetailRow.JournalAccountTypePerRow = new TblJournalAccountType().InjectFrom(JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType)) as TblJournalAccountType;
                        SelectedDetailRow.EntityAccount = CashDepositRow.EntityAccount;
                        SelectedDetailRow.EntityPerRow = new GlService.Entity().InjectFrom(EntityList.FirstOrDefault(w => w.Iserial == CashDepositRow.EntityAccount && w.TblJournalAccountType == CashDepositRow.TblJournalAccountType)) as GlService.Entity;
                        SelectedDetailRow.DiscountPercent = CashDepositRow.DiscountPercent;
                    }
                }
                else if (SelectedMainRow.TblCashDepositType == (int)CashDepositType.TFKCourier)
                {
                    var CashDepositRow = CashDepositSetting.FirstOrDefault(w => w.TblTenderTypes == SelectedMainRow.TblTenderType && w.TblCashDepositType.Value == SelectedMainRow.TblCashDepositType);
                    if (CashDepositRow != null)
                    {
                        SelectedDetailRow.TblJournalAccountType = CashDepositRow.TblJournalAccountType;
                        SelectedDetailRow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType);
                        //new gene().InjectFrom(
                        // as TblJournalAccountType;
                        //SelectedDetailRow.JournalAccountTypePerRow = new TblJournalAccountType().InjectFrom(JournalAccountTypeList.FirstOrDefault(w => w.Iserial == CashDepositRow.TblJournalAccountType)) as TblJournalAccountType;
                        SelectedDetailRow.EntityAccount = CashDepositRow.EntityAccount;
                        SelectedDetailRow.EntityPerRow = new GlService.Entity().InjectFrom(EntityList.FirstOrDefault(w => w.Iserial == CashDepositRow.EntityAccount && w.TblJournalAccountType == CashDepositRow.TblJournalAccountType)) as GlService.Entity;
                        SelectedDetailRow.DiscountPercent = CashDepositRow.DiscountPercent;
                    }
                }
               
                RaisePropertyChanged(nameof(Total));
                RaisePropertyChanged(nameof(TotalDifference));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
            }
        }
        public void AddNewAmountDetailRow(bool checkLastRow)
        {
            if (!AllowUpdate && !IsNew)
                return;
            var currentRowIndex = (SelectedMainRow.TblCashDepositAmountDetails.IndexOf(SelectedAmountDetailRow));
            if (!checkLastRow || currentRowIndex == (SelectedMainRow.TblCashDepositAmountDetails.Count - 1))
            {
                if (checkLastRow && SelectedAmountDetailRow != null)
                {
                    var valiationCollection = new List<ValidationResult>();
                    bool isvalid = (SelectedAmountDetailRow.Amount != 0);//Validator.TryValidateObject(SelectedAmountDetailRow, new ValidationContext(
                                                                         //SelectedAmountDetailRow, null, null), valiationCollection, true);
                    if (!isvalid)
                    {
                        return;
                    }
                }
                SelectedAmountDetailRow = new CashDepositAmountDetail
                {
                    TblCashDepositHeader = SelectedMainRow.Iserial,
                    TblCashDepositHeader1 = SelectedMainRow,
                };
                SelectedMainRow.TblCashDepositAmountDetails.ForEach(d =>
                    d.IsQuantityFocused = false);
                SelectedMainRow.TblCashDepositAmountDetails.Insert(currentRowIndex + 1,
                    SelectedAmountDetailRow);
                SelectedAmountDetailRow.IsQuantityFocused = true;
                SaveCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(AmountTotal));
                RaisePropertyChanged(nameof(TotalDifference));
                RaisePropertyChanged(nameof(IsHeaderHasDetails));
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
                    var rowToSave = new TblCashDepositDetail();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    BankDepositClient.UpdateOrInsertCashDepositDetailAsync(rowToSave,
                        SelectedMainRow.TblCashDepositDetails.IndexOf(SelectedDetailRow),
                        LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
                }
            }
        }
        public void GetComboData()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            BankDepositClient.GetLookUpCashDepositTypeAsync(LoggedUserInfo.DatabasEname);
            BankDepositClient.GetLookUpBankAsync(LoggedUserInfo.DatabasEname);
            BankDepositClient.GetLookUpStoreAsync(LoggedUserInfo.DatabasEname, true);
            Glclient.GetGenericAsync(nameof(TblJournalAccountType), "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
        }

        #endregion

        #region Properties

        private ObservableCollection<CashDepositHeader> _mainRowList;
        public ObservableCollection<CashDepositHeader> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }

        private ObservableCollection<TblCashDepositSetting> _CashDepositSetting;

        public ObservableCollection<TblCashDepositSetting> CashDepositSetting
        {
            get { return _CashDepositSetting; }
            set { _CashDepositSetting = value; RaisePropertyChanged(nameof(CashDepositSetting)); }
        }
        private ObservableCollection<Entity> _EntityList;

        public ObservableCollection<Entity> EntityList
        {
            get { return _EntityList; }
            set { _EntityList = value; RaisePropertyChanged(nameof(EntityList)); }
        }


        



        private ObservableCollection<CashDepositHeader> _selectedMainRows;
        public ObservableCollection<CashDepositHeader> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<CashDepositHeader>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }
        private CashDepositHeader _selectedMainRow;
        public CashDepositHeader SelectedMainRow
        {
            get
            {
                return _selectedMainRow ?? (_selectedMainRow = new CashDepositHeader()
                {
                    TblCashDepositDetails = new ObservableCollection<CashDepositDetail>()
                });
            }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged(nameof(SelectedMainRow));
                DeleteCommand.RaiseCanExecuteChanged();
                ApproveCashDeposit.RaiseCanExecuteChanged();
                UnApproveCashDeposit.RaiseCanExecuteChanged();
                CancelCashDeposit.RaiseCanExecuteChanged();
                OpenDetail.RaiseCanExecuteChanged();
                IsNewChanged();
                GetDetailData();
                RaisePropertyChanged(nameof(HasBank));
            }
        }

        private CashDepositDetail _selectedDetailRow;
        public CashDepositDetail SelectedDetailRow
        {
            get { return _selectedDetailRow ?? (_selectedDetailRow = new CashDepositDetail()); }
            set { _selectedDetailRow = value; RaisePropertyChanged(nameof(SelectedDetailRow)); }
        }

        private CashDepositAmountDetail _selectedAmountDetailRow;
        public CashDepositAmountDetail SelectedAmountDetailRow
        {
            get { return _selectedAmountDetailRow ?? (_selectedAmountDetailRow = new CashDepositAmountDetail()); }
            set { _selectedAmountDetailRow = value; RaisePropertyChanged(nameof(SelectedAmountDetailRow)); }
        }

        #region Combo Data

        ObservableCollection<TblStore> _storeList = new ObservableCollection<TblStore>();
        public ObservableCollection<TblStore> StoreList
        {
            get { return _storeList; }
            set { _storeList = value; RaisePropertyChanged(nameof(StoreList)); }
        }
        ObservableCollection<TblCashDepositType> _cashDepositTypeList = new ObservableCollection<TblCashDepositType>();
        public ObservableCollection<TblCashDepositType> CashDepositTypeList
        {
            get { return _cashDepositTypeList; }
            set { _cashDepositTypeList = value; RaisePropertyChanged(nameof(CashDepositTypeList)); }
        }

        ObservableCollection<TblBank> _bankList = new ObservableCollection<TblBank>();
        public ObservableCollection<TblBank> BankList
        {
            get { return _bankList ?? (_bankList = new ObservableCollection<TblBank>()); }
            set { _bankList = value; RaisePropertyChanged(nameof(BankList)); }
        }

        ObservableCollection<GlService.GenericTable> _journalAccountTypeList = null;
        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get
            {
                return _journalAccountTypeList ?? (_journalAccountTypeList =
                  new ObservableCollection<GlService.GenericTable>() {
                      new GlService.GenericTable(){
                        Iserial = 14,
                        Code = "دائنون",
                        Ename = "Payable",
                        Aname = "دائنون",
                    },
                      new GlService.GenericTable(){
                        Iserial = 15,
                        Code = "Expenses",
                        Ename = "Expenses",
                        Aname = "Expenses",
                    }});
            }
            set { _journalAccountTypeList = value; RaisePropertyChanged(nameof(JournalAccountTypeList)); }
        }

        ObservableCollection<TblTenderType> _tenderTypeList = new ObservableCollection<TblTenderType>();
        public ObservableCollection<TblTenderType> TenderTypeList
        {
            get { return _tenderTypeList ?? (_tenderTypeList = new ObservableCollection<TblTenderType>()); }
            set { _tenderTypeList = value; RaisePropertyChanged(nameof(TenderTypeList)); }
        }

        public TblCashDepositType TblCashDepositTypeRec
        {
            set
            {
                try
                {
                    SelectedMainRow.TblCashDepositTypeRec = value;
                    RaisePropertyChanged(nameof(TblCashDepositTypeRec));
                    if (value != null)
                    {
                        SelectedMainRow.TblCashDepositType = value.Iserial;
                        BankDepositClient.GetLookUpTenderTypesAsync(value.Iserial, LoggedUserInfo.DatabasEname);
                        //if (value.Iserial == (int)CashDepositType.PremiumCard)
                        if (value.DepositeTypeGroup == (int)CashDepositType.PremiumCard)
                        {
                            SelectedMainRow.TblBank = value.Iserial == 6 ? PremiumBankIserial : PremiumBank2030Iserial;
                            SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                        }

                        if (value.Iserial == (int)CashDepositType.TFKDiscount15)
                        {
                            SelectedMainRow.TblBank = TFKDiscountBankIserial;
                            SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                        }


                        if (value.Iserial == (int)CashDepositType.TFKCourier)
                        {
                            if(LoggedUserInfo.DatabasEname.ToLower() == "ccnew")
                            {
                                SelectedMainRow.TblBank = 113;
                                SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                            }
                            else if(LoggedUserInfo.DatabasEname.ToLower() == "sw") {
                                SelectedMainRow.TblBank = 116;
                                SelectedMainRow.TblBank1 = BankList.FirstOrDefault(b => b.Iserial == SelectedMainRow.TblBank);
                            }
                            
                        }
                       
                    }
                    RaisePropertyChanged(nameof(HasBank));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "-----------------" + ex.StackTrace);
                }
            }
            get { return SelectedMainRow.TblCashDepositTypeRec; }
        }
        #endregion

        public virtual bool IsReadOnly
        {
            get
            {
                RaisePropertyChanged(nameof(CanChangeData));
                RaisePropertyChanged(nameof(HasBank));
                return (SelectedMainRow != null && SelectedMainRow.Iserial > 0);
            }
        }
        public virtual bool IsHeaderHasDetails
        {
            get { return SelectedMainRow.TblCashDepositDetails.Any(d => d.TblBank > 0) || IsReadOnly; }
        }
        private bool canApprove, canUnApprove, canCancel;
        private bool canAddDetail, canAddDetailVisa, canAddDetailPremium, canAddDetailTFKDiscount15;
        public bool CanAddDetail
        {
            get { return canAddDetail; }
            set
            {
                canAddDetail = value;
                RaisePropertyChanged(nameof(CanAddDetail));
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
            }
        }
        public bool CanOpenDetail
        {
            get { return CanAddDetail || CanAddDetailVisa || CanAddDetailPremium || CanAddDetailTFKDiscount15 || CanApprove; }
        }
        public bool CanEditDetail
        {
            get { return (CanAddDetail || CanAddDetailVisa || CanAddDetailPremium || CanAddDetailTFKDiscount15) && !SelectedMainRow.Approved && !SelectedMainRow.Canceled; }
        }
        public bool CanAddDetailVisa
        {
            get { return canAddDetailVisa; }
            set
            {
                canAddDetailVisa = value;
                RaisePropertyChanged(nameof(CanAddDetailVisa));
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
            }
        }
        public bool CanAddDetailPremium
        {
            get { return canAddDetailPremium; }
            set
            {
                canAddDetailPremium = value;
                RaisePropertyChanged(nameof(CanAddDetailPremium));
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
            }
        }

        public bool CanAddDetailTFKDiscount15
        {
            get { return canAddDetailTFKDiscount15; }
            set
            {
                canAddDetailTFKDiscount15 = value;
                RaisePropertyChanged(nameof(canAddDetailTFKDiscount15));
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
            }
        }
        public bool CanApprove
        {
            get { return canApprove; }
            set
            {
                canApprove = value;
                RaisePropertyChanged(nameof(CanApprove));
                if (ApproveCashDeposit != null) ApproveCashDeposit.RaiseCanExecuteChanged();
                if (CancelCashDeposit != null) CancelCashDeposit.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
                RaisePropertyChanged(nameof(CanChangeData));
                RaisePropertyChanged(nameof(HasBank));
            }
        }
        public bool CanUnApprove
        {
            get { return canUnApprove; }
            set { canUnApprove = value; RaisePropertyChanged(nameof(CanUnApprove)); UnApproveCashDeposit.RaiseCanExecuteChanged(); }
        }
        public bool CanCancel
        {
            get { return canCancel; }
            set
            {
                canCancel = value;
                RaisePropertyChanged(nameof(CanCancel));
                if (ApproveCashDeposit != null) ApproveCashDeposit.RaiseCanExecuteChanged();
                if (CancelCashDeposit != null) CancelCashDeposit.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(CanOpenDetail));
                RaisePropertyChanged(nameof(CanEditDetail));
                RaisePropertyChanged(nameof(CanChangeData));
                RaisePropertyChanged(nameof(HasBank));
            }
        }
        public override bool IsNew
        {
            get { return SelectedMainRow.Iserial <= 0; }//base.IsNew && 
            set { base.IsNew = value; }
        }
        public decimal Total
        {
            get { return SelectedMainRow.TblCashDepositDetails.Sum(td => td.Amount); }
        }
        public decimal AmountTotal
        {
            get { return SelectedMainRow.TblCashDepositAmountDetails.Sum(td => td.Amount); }
        }
        public decimal TotalDifference
        {
            get { return AmountTotal - Total; }
        }
        public bool IsSaved { get { return SelectedMainRow.Iserial > 0; } }

        private bool autoIncrement = false;
        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; RaisePropertyChanged(nameof(AutoIncrement)); }
        }

        private bool closeAfterSave = true;
        public bool CloseAfterSave
        {
            get { return closeAfterSave; }
            set { closeAfterSave = value; RaisePropertyChanged(nameof(CloseAfterSave)); }
        }
        public bool CanChangeData
        {
            get { return (CanApprove || SelectedMainRow.Iserial == 0) && !SelectedMainRow.Approved && !SelectedMainRow.Canceled; }
        }
        public bool HasBank
        {
            get
            {
                return (!SelectedMainRow.NoBank && SelectedMainRow.Iserial == 0 && !SelectedMainRow.Approved && !SelectedMainRow.Canceled) ||
                (CanApprove && !SelectedMainRow.NoBank && SelectedMainRow.Iserial > 0 && !SelectedMainRow.Approved && !SelectedMainRow.Canceled);
                //( && SelectedMainRow.Iserial == 0))
                //&& !SelectedMainRow.Approved;
            }
        }

        #endregion

        #region Commands

        RelayCommand approveCashDeposit;
        public RelayCommand ApproveCashDeposit
        {
            get { return approveCashDeposit; }
            set { approveCashDeposit = value; RaisePropertyChanged(nameof(ApproveCashDeposit)); }
        }
        RelayCommand unApproveCashDeposit;
        public RelayCommand UnApproveCashDeposit
        {
            get { return unApproveCashDeposit; }
            set { unApproveCashDeposit = value; RaisePropertyChanged(nameof(UnApproveCashDeposit)); }
        }

        RelayCommand reverseCashDeposit;
        public RelayCommand ReverseCashDeposit
        {
            get { return reverseCashDeposit; }
            set { reverseCashDeposit = value; RaisePropertyChanged(nameof(ReverseCashDeposit)); }
        }

        RelayCommand cancelCashDeposit;
        public RelayCommand CancelCashDeposit
        {
            get { return cancelCashDeposit; }
            set { cancelCashDeposit = value; RaisePropertyChanged(nameof(CancelCashDeposit)); }
        }
        RelayCommand<object> deleteCashDepositDetail;
        public RelayCommand<object> DeleteCashDepositDetail
        {
            get { return deleteCashDepositDetail; }
            set { deleteCashDepositDetail = value; RaisePropertyChanged(nameof(DeleteCashDepositDetail)); }
        }

        RelayCommand<object> deleteCashDepositAmountDetail;
        public RelayCommand<object> DeleteCashDepositAmountDetail
        {
            get { return deleteCashDepositAmountDetail; }
            set { deleteCashDepositAmountDetail = value; RaisePropertyChanged(nameof(DeleteCashDepositAmountDetail)); }
        }

        RelayCommand<object> newDetail;
        public RelayCommand<object> NewDetail
        {
            get { return newDetail; }
            set { newDetail = value; RaisePropertyChanged(nameof(NewDetail)); }
        }

        RelayCommand<object> newAmountDetail;
        public RelayCommand<object> NewAmountDetail
        {
            get { return newAmountDetail; }
            set { newAmountDetail = value; RaisePropertyChanged(nameof(NewAmountDetail)); }
        }

        RelayCommand<object> loadingDetailRows;
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }

        RelayCommand<object> openDetail;
        public RelayCommand<object> OpenDetail
        {
            get { return openDetail; }
            set { openDetail = value; RaisePropertyChanged(nameof(OpenDetail)); }
        }

        RelayCommand<object> saveDetails;
        public RelayCommand<object> SaveDetails
        {
            get { return saveDetails; }
            set { saveDetails = value; RaisePropertyChanged(nameof(SaveDetails)); }
        }
        RelayCommand<object> saveAmountDetails;
        public RelayCommand<object> SaveAmountDetails
        {
            get { return saveAmountDetails; }
            set { saveAmountDetails = value; RaisePropertyChanged(nameof(SaveAmountDetails)); }
        }

        RelayCommand<SelectionChangedEventArgs> bankChanged;
        public RelayCommand<SelectionChangedEventArgs> BankChanged
        {
            get { return bankChanged; }
            set { bankChanged = value; RaisePropertyChanged(nameof(BankChanged)); }
        }

        RelayCommand<object> printAll;
        public RelayCommand<object> PrintAll
        {
            get { return printAll; }
            set { printAll = value; RaisePropertyChanged(nameof(PrintAll)); }
        }

        #endregion

        #region override

        public override void NewRecord()
        {
            AddNewMainRow(false);
            base.NewRecord();
            RaisePropertyChanged(nameof(IsReadOnly));
        }
        public override bool CanSaveRecord()
        {
            var result = base.CanSaveRecord() && (SelectedMainRow.Iserial <= 0 ||
                SelectedMainRow.TblCashDepositAmountDetails.Any(d => d.Iserial <= 0) ||
                CanApprove) && !SelectedMainRow.Approved;
            return result;
        }
        public override void SaveRecord()
        {
            SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            return ValidHeaderData();// && ValidDetailData();
        }
        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<CashDepositHeader> vm =
                new GenericSearchViewModel<CashDepositHeader>() { Title = "Cash Deposit Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
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
                        Header = strings.Sequence,
                        PropertyPath= nameof(CashDepositHeader.Sequance),
                        FilterPropertyPath=nameof(CashDepositHeader.Sequance),
                        //SelectedFilterOperation= new FilterOperationItem(
                        //    Enums.FilterOperation.Contains, "Contains", " LIKE",
                        //    "/Os.Controls;component/Images/Contains.png"),
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.BankAccount,
                        PropertyPath= string.Format("{0}.{1}", nameof(CashDepositHeader.TblBank1),nameof(TblBank.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(CashDepositHeader.TblBank1),nameof(TblBank.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.StoreCode,
                        PropertyPath= string.Format("{0}.{1}", nameof(CashDepositHeader.TblStore1),nameof(TblStore.code)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(CashDepositHeader.TblStore1),nameof(TblStore.code)),
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.StoreEname,
                        PropertyPath= string.Format("{0}.{1}", nameof(CashDepositHeader.TblStore1),nameof(TblStore.ENAME)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(CashDepositHeader.TblStore1),nameof(TblStore.ENAME)),
                    },
                    new SearchColumnModel()
                    {
                        Header = strings.Payment,
                        PropertyPath= string.Format("{0}.{1}", nameof(CashDepositHeader.TblTenderType1),nameof(TblTenderType.ename)),
                        FilterPropertyPath=string.Format("{0}.{1}", nameof(CashDepositHeader.TblTenderType1),nameof(TblTenderType.ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Date,
                        PropertyPath=nameof(CashDepositHeader.DocDate),
                        StringFormat="{0:dd/MM/yyyy}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Total,
                        PropertyPath=nameof(CashDepositHeader.Amount),
                        StringFormat="0.#",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Approved,
                        PropertyPath=nameof(CashDepositHeader.Approved),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.ApproveDate,
                        PropertyPath=nameof(CashDepositHeader.ApproveDate),
                        StringFormat="{0:dd/MM/yyyy}",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Cancel,
                        PropertyPath=nameof(CashDepositHeader.Canceled),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.CanceledDate,
                        PropertyPath=nameof(CashDepositHeader.CanceledDate),
                        StringFormat="{0:dd/MM/yyyy h:mm tt}",
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
            return false;
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
            var para = new ObservableCollection<string>() { SelectedMainRow.Iserial.ToString() };
            para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
            para.Add(LoggedUserInfo.DatabasEname);
            rVM.GenerateReport("CashDepositeDocument", para);
        }

        #endregion

    }
}