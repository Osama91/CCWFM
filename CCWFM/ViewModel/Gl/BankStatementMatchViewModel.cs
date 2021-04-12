using CCWFM.BankStatService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Models.Gl;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.Views.Gl;
using GalaSoft.MvvmLight.Command;
using Omu.ValueInjecter.Silverlight;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.ViewModel.Gl
{
    public class BankStatementMatchViewModel : ViewModelStructuredBase
    {
        decimal acceptedAmountDifferance = 0.1M;
        
        BankStatServiceClient BankStatClient = Helpers.Services.Instance.GetBankStatServiceClient();
        public BankStatementMatchViewModel() : base(PermissionItemName.BankStatement)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                LoadingDetailRows = new RelayCommand<object>((o) =>
                {
                    var e = o as DataGridRowEventArgs;
                    if (HeaderRow.TblBankStatDetails.Count < PageSize)
                    {
                        return;
                    }
                    if (HeaderRow.TblBankStatDetails.Count - 2 < e.Row.GetIndex() && !Loading)
                    {
                        GetMatchedData();
                    }
                });
                BankStatClient.InsertMatchedListCompleted += (s, e) =>
                {
                    if (!e.lastInsert)
                    {
                        requests--;
                        if (requests == 0)
                        {
                            GetUnMatcheddata();
                            GetMatchedData();
                        }
                    }
                    else
                    {
                        GetUnMatcheddata();
                        GetMatchedData();
                    }
                };
                BankStatClient.RemoveMatchedListCompleted += (s, e) =>
                {
                    GetUnMatcheddata();
                    GetMatchedData();
                };
                SelectionChanged = new RelayCommand<object>((o) => { MatchSelected.RaiseCanExecuteChanged(); });

                BankStatClient.GetBankStatDetailForMatchingCompleted += (s, e) => {
                    BankStatDetailList = e.Result;
                    ApproveAsMatched.RaiseCanExecuteChanged();
                };
                BankStatClient.GetLedgerDetailForMatchingCompleted += (s, e) => {
                    LedgerDetailList = e.Result;
                    ApproveAsMatched.RaiseCanExecuteChanged();
                };
                BankStatClient.GetBankStatDetailMatchedCompleted += (s, e) => {
                    BankStatDetailMatchedList = e.Result;
                    ApproveAsMatched.RaiseCanExecuteChanged();
                };
                BankStatClient.GetLedgerDetailMatchedCompleted += (s, e) => {
                    LedgerDetailMatchedList = e.Result;
                    ApproveAsMatched.RaiseCanExecuteChanged();
                };

                BankStatClient.GetLedgerDetailMatchedByBankStatDetailIdCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        LedgerDetailMatchedList.FirstOrDefault(ldm => ldm.Iserial == item.Iserial).IsChecked = item.IsChecked;
                    }
                };
                BankStatClient.GetBankStatDetailMatchedByLedgerDetailIdCompleted += (s, e) =>
                {
                    foreach (var item in e.Result)
                    {
                        BankStatDetailMatchedList.FirstOrDefault(ldm => ldm.Iserial == item.Iserial).IsChecked = item.IsChecked;
                    }
                };

                BankStatClient.UpdateOrInsertBankStatHeaderCompleted += (s, x) =>
                {
                    BankStatHeader savedRow = new BankStatHeader();

                    if (savedRow != null)
                    {
                        savedRow.InjectFrom(x.Result);
                        savedRow.TblBankStatDetails.Clear();
                        foreach (var item in x.Result.TblBankStatDetails)
                        {
                            var detailTemp = new BankStatDetail();
                            detailTemp.InjectFrom(item);
                            savedRow.TblBankStatDetails.Add(detailTemp);
                        }
                    }
                    HeaderRow = savedRow;
                    IsNewChanged();
                };

                BankStatChecked = new RelayCommand<object>((o) => {
                    var row = ((CheckBox)((RoutedEventArgs)o).OriginalSource).DataContext as BankStatMatchingModel;
                    // كده هو اختارها
                    BankStatClient.GetLedgerDetailMatchedByBankStatDetailIdAsync(
                        HeaderRow.Iserial, row.Iserial, true, LoggedUserInfo.DatabasEname);
                });
                BankStatUnchecked = new RelayCommand<object>((o) => {
                    var row = ((CheckBox)((RoutedEventArgs)o).OriginalSource).DataContext as BankStatMatchingModel;
                    // كده هو لغاها
                    BankStatClient.GetLedgerDetailMatchedByBankStatDetailIdAsync(
                        HeaderRow.Iserial, row.Iserial, false, LoggedUserInfo.DatabasEname);
                });
                LedgerMainDetailChecked = new RelayCommand<object>((o) => {
                    var row = ((CheckBox)((RoutedEventArgs)o).OriginalSource).DataContext as BankStatMatchingModel;
                    // كده هو اختارها
                    BankStatClient.GetBankStatDetailMatchedByLedgerDetailIdAsync(
                        HeaderRow.Iserial, row.Iserial, true, LoggedUserInfo.DatabasEname);
                });
                LedgerMainDetailUnchecked = new RelayCommand<object>((o) => {
                    var row = ((CheckBox)((RoutedEventArgs)o).OriginalSource).DataContext as BankStatMatchingModel;
                    // كده هو لغاها
                    BankStatClient.GetBankStatDetailMatchedByLedgerDetailIdAsync(
                        HeaderRow.Iserial, row.Iserial, false, LoggedUserInfo.DatabasEname);
                });

                MatchSelected = new RelayCommand<object>((o) => {
                    var selectedBankStatDetailList = new ObservableCollection<BankStatMatchingModel>();
                    BankStatDetailList.ForEach(bs => { if (bs.IsChecked) selectedBankStatDetailList.Add(bs); });
                    var selectedLedgerDetailList = new ObservableCollection<BankStatMatchingModel>();
                    LedgerDetailList.ForEach(bs => { if (bs.IsChecked) selectedLedgerDetailList.Add(bs); });
                    if (selectedBankStatDetailList.Count() <= 0 || selectedLedgerDetailList.Count() <= 0)
                    {
                        MessageBox.Show("You must select from both parties");
                        return;
                    }
                    if (!(selectedBankStatDetailList.Count() == 1 || selectedLedgerDetailList.Count() == 1))
                    {
                        MessageBox.Show("You must select one record from one of the parties");
                        return;
                    }
                    if (Math.Abs(selectedBankStatDetailList.Sum(s => s.Amount) - selectedLedgerDetailList.Sum(s => s.Amount)) > acceptedAmountDifferance)
                    {
                        MessageBox.Show("Amounts must be equals");
                        return;
                    }
                    BankStatClient.InsertMatchedListAsync(selectedBankStatDetailList,
                        selectedLedgerDetailList, true, LoggedUserInfo.DatabasEname);
                    //هتعمل ريكويس ياخد الليستيتين ويضيف ريكوردات فى الداتابيز ويرجع هنا لما يرجع تجيب الريكوردات دى وتضيفها تحت وهتدور بالسريال الى معاك
                    //والحذف نفس الكلام هروح احذفهم وارجع احذف من عندى
                    //وفى الحالتين الى بينضاف تحت يتحذف من فوق والى يتحذف من تحت ينضاف فوق او تعمل ريفريش لكله

                });
                UnMatchSelected = new RelayCommand<object>((o) => {
                    var selectedBankStatDetailList = new ObservableCollection<BankStatMatchingModel>();
                    BankStatDetailMatchedList.ForEach(bs => { if (bs.IsChecked) selectedBankStatDetailList.Add(bs); });
                    var selectedLedgerDetailList = new ObservableCollection<BankStatMatchingModel>();
                    LedgerDetailMatchedList.ForEach(bs => { if (bs.IsChecked) selectedLedgerDetailList.Add(bs); });
                    if (selectedBankStatDetailList.Count() <= 0 || selectedLedgerDetailList.Count() <= 0)
                    {
                        MessageBox.Show("You must select from both parties");
                        return;
                    }
                    if (!(selectedBankStatDetailList.Count() == 1 || selectedLedgerDetailList.Count() == 1))
                    {
                        MessageBox.Show("You must select one record from one of the parties");
                        return;
                    }
                    if (selectedBankStatDetailList.Sum(s => s.Amount) != selectedLedgerDetailList.Sum(s => s.Amount))
                    {
                        MessageBox.Show("Amounts must be equals");
                        return;
                    }
                    BankStatClient.RemoveMatchedListAsync(selectedBankStatDetailList,
                        selectedLedgerDetailList, true, LoggedUserInfo.DatabasEname);
                });
                
                AutoMatch = new RelayCommand<object>((o) => {
                    var w = new BankStatementAutoMatchTypeView();
                    w.Closing += (s, e) => {
                        if (w.DialogResult ?? false)//كده هو اختار حاجة
                        {
                            requests = 0;
                            bool transType = w.TransactionType.IsChecked ?? false,
                            chequeNo = w.ChequeNo.IsChecked ?? false,
                            depositNo = w.DepositNo.IsChecked ?? false,
                            amount = w.Amount.IsChecked ?? false;
                            int count = 0;
                            var bankStatMinDate = BankStatDetailList.Min(r => r.DocDate);
                            // كده معاك الى اختاره
                            BankStatDetailList.ForEach(bsd => bsd.IsChecked = false);
                            LedgerDetailList.ForEach(ld => ld.IsChecked = false);
                            if (transType) { }/////احنا اصلا مش بنسجله
                            if (chequeNo) {
                                BankStatDetailList.Where(bsd =>
                                bsd.ChequeNo != null /*&& !bsd.IsChecked*/).ForEach(bsd => {
                                    var chequeRows = new ObservableCollection<BankStatMatchingModel>();
                                    LedgerDetailList.Where(ld => ld.ChequeNo != null &&
                                    ld.ChequeNo == bsd.ChequeNo && !ld.IsChecked)
                                    .ForEach(ld => { chequeRows.Add(ld); });
                                    if (chequeRows != null && Math.Abs(chequeRows.Sum(c => c.Amount) - bsd.Amount) <= acceptedAmountDifferance)
                                    {
                                        bsd.IsChecked = true; chequeRows.ForEach(a => a.IsChecked = true);
                                        BankStatClient.InsertMatchedListAsync(
                                            new ObservableCollection<BankStatMatchingModel>() { bsd },
                                            chequeRows, false, LoggedUserInfo.DatabasEname);
                                        requests++; count++;
                                        return;
                                    }
                                });
                            }
                            if (depositNo)
                            {
                                //BankStatDetailList.Where(bsd =>
                                //   !string.IsNullOrEmpty(bsd.DepositNo)/* && !bsd.IsChecked*/).ForEach(bsd => {
                                //       var depositRows = new ObservableCollection<BankStatMatchingModel>();
                                //       LedgerDetailList.Where(ld => !string.IsNullOrEmpty(ld.DepositNo) &&
                                //       ld.DepositNo == bsd.DepositNo && !ld.IsChecked)
                                //       .ForEach(ld => { depositRows.Add(ld); });
                                //       if (depositRows != null && Math.Abs(depositRows.Sum(c => c.Amount) - bsd.Amount) <= acceptedAmountDifferance)
                                //       {
                                //           bsd.IsChecked = true; depositRows.ForEach(a => a.IsChecked = true);
                                //           BankStatClient.InsertMatchedListAsync(
                                //               new ObservableCollection<BankStatMatchingModel>() { bsd },
                                //               depositRows, false);
                                //           requests++; count++;
                                //           return;
                                //       }
                                //   });

                                BankStatDetailList.Where(bsd =>
                                     !string.IsNullOrEmpty(bsd.DepositNo)/* && !bsd.IsChecked*/).GroupBy(bsd =>
                                     new { bsd.DepositNo, bsd.IsChecked }).ForEach(bsd => {
                                         var depositRows = new ObservableCollection<BankStatMatchingModel>();
                                         LedgerDetailList.Where(ld => !string.IsNullOrEmpty(ld.DepositNo) &&
                                         ld.DepositNo == bsd.Key.DepositNo && !ld.IsChecked)
                                         .ForEach(ld => { depositRows.Add(ld); });
                                         if (depositRows != null && Math.Abs(depositRows.Sum(c => c.Amount) - bsd.Sum(r => r.Amount)) <= acceptedAmountDifferance)
                                         {
                                             bsd.ForEach(r => r.IsChecked = true); depositRows.ForEach(a => a.IsChecked = true);
                                             BankStatClient.InsertMatchedListAsync(
                                                  bsd.ToObservableCollection(),
                                                 depositRows, false, LoggedUserInfo.DatabasEname);
                                             requests++; count++;
                                             return;
                                         }
                                         else if(depositRows.Count > 1)
                                         {
                                             depositRows = new ObservableCollection<BankStatMatchingModel>();
                                             LedgerDetailList.Where(ld => !string.IsNullOrEmpty(ld.DepositNo) &&
                                             ld.DocDate > bankStatMinDate && !ld.IsChecked)
                                             .Where(ld => ld.DepositNo == bsd.Key.DepositNo)
                                             .ForEach(ld => { depositRows.Add(ld); });
                                             if (depositRows != null && Math.Abs(depositRows.Sum(c => c.Amount) - bsd.Sum(r => r.Amount)) <= acceptedAmountDifferance)
                                             {
                                                 bsd.ForEach(r => r.IsChecked = true); depositRows.ForEach(a => a.IsChecked = true);
                                                 BankStatClient.InsertMatchedListAsync(
                                                      bsd.ToObservableCollection(),
                                                     depositRows, false, LoggedUserInfo.DatabasEname);
                                                 requests++; count++;
                                                 return;
                                             }
                                         }
                                     });
                            }
                            if (amount)
                            {
                                //هتعمل ايه لو اكتر من سجل له نفس المبلغ
                                BankStatDetailList/*.Where(bsd => !bsd.IsChecked)*/.ForEach(bsd =>
                                {
                                    //var amountRows = new ObservableCollection<BankStatMatchingModel>();
                                    var firstMatch = LedgerDetailList.FirstOrDefault(ld => (Math.Abs(ld.Amount - bsd.Amount) <= acceptedAmountDifferance) &&
                                      ld.DocDate == bsd.DocDate &&
                                      !ld.IsChecked);
                                    if (firstMatch != null)
                                    {
                                        bsd.IsChecked = true; firstMatch.IsChecked = true;
                                        BankStatClient.InsertMatchedListAsync(
                                            new ObservableCollection<BankStatMatchingModel>() { bsd },
                                            new ObservableCollection<BankStatMatchingModel>() { firstMatch }, false, LoggedUserInfo.DatabasEname);
                                        requests++; count++;
                                    }
                                    //.ForEach(ld => { amountRows.Add(ld); });
                                    //if (amountRows != null && amountRows.Sum(c => c.Amount) == bsd.Amount &&
                                    //    !amountRows.Any(c => c.DocDate.Value.Date != bsd.DocDate.Value.Date))
                                    ////طيب لو الحركة عبارة عن جزء من الحركات يعنى فيه كذا حركة بالف انا هماتش مع واحدة مش كله
                                    //{
                                    //    BankStatClient.InsertMatchedListAsync(
                                    //        new ObservableCollection<BankStatMatchingModel>() { bsd },
                                    //        amountRows, false);
                                    //    requests++; count++;
                                    //    bsd.IsChecked = true; amountRows.ForEach(a => a.IsChecked = true);
                                    //    return;
                                    //}
                                });
                            }
                            // يا تعرف ان ده اخر واحد يا تبعت واحد فاضى فى الاخر بعدهم
                            if (count == 0) MessageBox.Show("No Match Found");
                        }
                    };
                    w.Show();
                });
                UnMatchAll = new RelayCommand<object>((o) => {
                    BankStatDetailMatchedList.ForEach(bs => { if (!bs.IsChecked) bs.IsChecked=true; });
                    LedgerDetailMatchedList.ForEach(bs => { if (!bs.IsChecked) bs.IsChecked = true; });
                    if (Math.Abs(BankStatDetailMatchedList.Sum(s => s.Amount) - LedgerDetailMatchedList.Sum(s => s.Amount)) > acceptedAmountDifferance)
                    {
                        MessageBox.Show("Amounts must be equals");
                        return;
                    }
                    BankStatClient.RemoveMatchedListAsync(BankStatDetailMatchedList,
                        LedgerDetailMatchedList, true, LoggedUserInfo.DatabasEname);
                });

                ApproveAsMatched = new RelayCommand<object>((o) => {
                    HeaderRow.MatchApproved = true;
                    HeaderRow.MatchApproveDate = DateTime.Now;
                    HeaderRow.MatchApprovedBy = LoggedUserInfo.Iserial;
                    BankStatClient.UpdateOrInsertBankStatHeaderAsync((TblBankStatHeader)
                        new TblBankStatHeader().InjectFrom(HeaderRow), 0,
                        LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
                },
                    (o) => {
                        return BankStatDetailList.Count == 0 &&
                            BankStatDetailMatchedList.Count > 0;
                    });
            }
        }

        int requests = 0;

        #region Methods
        
        public bool ValidHeaderData()
        {
            if (HeaderRow.TblBank <= 0)
            {
                MessageBox.Show(strings.ReqBankAccountNo);
                return false;
            }
            if (HeaderRow.TblCurrency < 0)
            {
                MessageBox.Show(strings.ReqCurrency);
                return false;
            }
            return true;
        }
        public bool ValidDetailData()
        {
            if (HeaderRow.Approved && HeaderRow.TblBankStatDetails.Any(td => 0 == td.Amount))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
    
        #endregion

        #region Operations

        public void GetUnMatcheddata()
        {
            if (HeaderRow != null)
            {
                BankStatClient.GetBankStatDetailForMatchingAsync(HeaderRow.Iserial, LoggedUserInfo.DatabasEname);
                BankStatClient.GetLedgerDetailForMatchingAsync(HeaderRow.Iserial, LoggedUserInfo.DatabasEname);
            }
        }
        public void GetMatchedData()
        {
            if (HeaderRow != null)
            {
                BankStatClient.GetBankStatDetailMatchedAsync(HeaderRow.Iserial, LoggedUserInfo.DatabasEname);
                BankStatClient.GetLedgerDetailMatchedAsync(HeaderRow.Iserial, LoggedUserInfo.DatabasEname);
            }
        }

        #endregion

        #region Properties

        private BankStatHeader _headerRow;
        public BankStatHeader HeaderRow
        {
            get
            {
                return _headerRow;
            }
            set
            {
                _headerRow = value;
                RaisePropertyChanged(nameof(HeaderRow));
                DeleteCommand.RaiseCanExecuteChanged();
                IsNewChanged();
                GetUnMatcheddata();
                GetMatchedData();
            }
        }

        #region Combo Data

        ObservableCollection<BankStatMatchingModel> _bankStatDetailList = new ObservableCollection<BankStatMatchingModel>();
        public ObservableCollection<BankStatMatchingModel> BankStatDetailList
        {
            get { return _bankStatDetailList; }
            set { _bankStatDetailList = value; RaisePropertyChanged(nameof(BankStatDetailList)); }
        }

        ObservableCollection<BankStatMatchingModel> _ledgerDetailList = new ObservableCollection<BankStatMatchingModel>();
        public ObservableCollection<BankStatMatchingModel> LedgerDetailList
        {
            get { return _ledgerDetailList; }
            set { _ledgerDetailList = value; RaisePropertyChanged(nameof(LedgerDetailList)); }
        }


        ObservableCollection<BankStatMatchingModel> _bankStatDetailMatchedList = new ObservableCollection<BankStatMatchingModel>();
        public ObservableCollection<BankStatMatchingModel> BankStatDetailMatchedList
        {
            get { return _bankStatDetailMatchedList; }
            set { _bankStatDetailMatchedList = value; RaisePropertyChanged(nameof(BankStatDetailMatchedList)); }
        }

        ObservableCollection<BankStatMatchingModel> _ledgerDetailMatchedList = new ObservableCollection<BankStatMatchingModel>();
        public ObservableCollection<BankStatMatchingModel> LedgerDetailMatchedList
        {
            get { return _ledgerDetailMatchedList; }
            set { _ledgerDetailMatchedList = value; RaisePropertyChanged(nameof(LedgerDetailMatchedList)); }
        }
      
        #endregion

        #endregion

        #region Commands

        RelayCommand<object> loadingDetailRows;      
        public RelayCommand<object> LoadingDetailRows
        {
            get { return loadingDetailRows; }
            set { loadingDetailRows = value; RaisePropertyChanged(nameof(LoadingDetailRows)); }
        }
        
        RelayCommand<object> selectionChanged;
        public RelayCommand<object> SelectionChanged
        {
            get { return selectionChanged; }
            set { selectionChanged = value; RaisePropertyChanged(nameof(SelectionChanged)); }
        }

        RelayCommand<object> matchSelected;
        public RelayCommand<object> MatchSelected
        {
            get { return matchSelected; }
            set { matchSelected = value; RaisePropertyChanged(nameof(MatchSelected)); }
        }

        RelayCommand<object> autoMatch;
        public RelayCommand<object> AutoMatch
        {
            get { return autoMatch; }
            set { autoMatch = value; RaisePropertyChanged(nameof(AutoMatch)); }
        }

        RelayCommand<object> unMatchAll;
        public RelayCommand<object> UnMatchAll
        {
            get { return unMatchAll; }
            set { unMatchAll = value; RaisePropertyChanged(nameof(UnMatchAll)); }
        }

        RelayCommand<object> unMatchSelected;
        public RelayCommand<object> UnMatchSelected
        {
            get { return unMatchSelected; }
            set { unMatchSelected = value; RaisePropertyChanged(nameof(UnMatchSelected)); }
        }
      
        RelayCommand<object> approveAsMatched;
        public RelayCommand<object> ApproveAsMatched
        {
            get { return approveAsMatched; }
            set { approveAsMatched = value; RaisePropertyChanged(nameof(ApproveAsMatched)); }
        }

        RelayCommand<object> bankStatChecked;
        public RelayCommand<object> BankStatChecked
        {
            get { return bankStatChecked; }
            set { bankStatChecked = value; RaisePropertyChanged(nameof(BankStatChecked)); }
        }

        RelayCommand<object> ledgerMainDetailChecked;
        public RelayCommand<object> LedgerMainDetailChecked
        {
            get { return ledgerMainDetailChecked; }
            set { ledgerMainDetailChecked = value; RaisePropertyChanged(nameof(LedgerMainDetailChecked)); }
        }

        RelayCommand<object> bankStatUnchecked;
        public RelayCommand<object> BankStatUnchecked
        {
            get { return bankStatUnchecked; }
            set { bankStatUnchecked = value; RaisePropertyChanged(nameof(BankStatUnchecked)); }
        }

        RelayCommand<object> ledgerMainDetailUnchecked;
        public RelayCommand<object> LedgerMainDetailUnchecked
        {
            get { return ledgerMainDetailUnchecked; }
            set { ledgerMainDetailUnchecked = value; RaisePropertyChanged(nameof(LedgerMainDetailUnchecked)); }
        }
       
        #endregion

        #region override

        public override void SaveRecord()
        {
            //SaveMainRow();
            base.SaveRecord();
        }
        public override bool ValidData()
        {
            if ((HeaderRow.Approved && HeaderRow.Iserial <= 0 &&
                HeaderRow.TblBankStatDetails.Any(td => td.Amount <= 0)))
            {
                MessageBox.Show(strings.CheckQuantities);
                return false;
            }
            return true;
        }
        public override void Cancel()
        {
            //MainRowList.Clear();
            //SelectedMainRows.Clear();
            //AddNewMainRow(false);
            //RaisePropertyChanged(nameof(IsReadOnly));
            base.Cancel();
        }
      
        #endregion
    }
}