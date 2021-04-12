using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;
using CCWFM.Views.RequestForQutation;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class NewRfqPurchasOrderDetailsViewModel : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler DeletePurchLine;

        public virtual void OnDeletePurchLine()
        {
            var handler = DeletePurchLine;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Private Fields ]

        private ObservableCollection<TblColor> _colorsList;
        private string _objectIndex;

        private int _parentTransId;

        private decimal _price;

        private ObservableCollection<PurchaseOrderSizeInfo> _purchaseOrderSizes;

        private int _rowTotal;

        private string _sizeGroup;
        private string _styleColor;
        private string _styleHeader;
        private DateTime? _delivaryDate;

        #endregion [ Private Fields ]

        #region [ Public Properties ]

        private decimal _additionalCost;
        private int? _iserial;

        private ObjectStatus _objStatus;

        private RFQSubHeader _parentRfq;
        private bool _isCalculatingQts;

        public bool IsCalculatingQts
        {
            get { return _isCalculatingQts; }
            set
            {
                _isCalculatingQts = value;
                RaisePropertyChanged("IsCalculatingQts");
            }
        }

        public decimal AdditionalCost
        {
            get { return _additionalCost; }
            set { _additionalCost = value; RaisePropertyChanged("AdditionalCost"); }
        }

        public ObservableCollection<tbl_PurchaseOrder_AdditionalCost> AdditionalCostList
        {
            get { return _additionalCostList ?? (_additionalCostList = new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>()); }
            set { _additionalCostList = value; RaisePropertyChanged("AdditionalCostList"); }
        }

        public ObservableCollection<TblColor> ColorsList
        {
            get { return _colorsList; }
            set
            {
                _colorsList = value;
                RaisePropertyChanged("ColorsList");
            }
        }

        public DateTime? DelivaryDate
        {
            get { return _delivaryDate; }
            set
            {
                _delivaryDate = value;
                RaisePropertyChanged("DelivaryDate");
            }
        }

        public int? Iserial
        {
            get { return _iserial; }
            set
            {
                _iserial = value;
                RaisePropertyChanged("Iserial");
            }
        }

        public string ObjectIndex
        {
            get { return _objectIndex; }
            set
            {
                _objectIndex = value;
                RaisePropertyChanged("ObjectIndex");
            }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus ?? (_objStatus = new ObjectStatus { IsNew = true }); }
            set
            {
                _objStatus = value;
                RaisePropertyChanged("ObjStatus");
            }
        }

        public RFQSubHeader ParentRfqSub
        {
            get { return _parentRfq; }
            set { _parentRfq = value; RaisePropertyChanged("ParentRFQ"); }
        }

        public int ParentTransID
        {
            get { return _parentTransId; }
            set { _parentTransId = value; RaisePropertyChanged("ParentTransID"); }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; RaisePropertyChanged("Price"); }
        }

        public ObservableCollection<PurchaseOrderSizeInfo> PurchaseOrderSizes
        {
            get { return _purchaseOrderSizes; }
            set
            {
                _purchaseOrderSizes = value;
                RaisePropertyChanged("PurchaseOrderSizes");
            }
        }

        private Brush _textBoxBackColor;

        public Brush TextBoxBackColor
        {
            get { return _textBoxBackColor; }
            set
            {
                _textBoxBackColor = value;
                RaisePropertyChanged("TextBoxBackColor");
            }
        }

        public int RowTotal
        {
            get { return _rowTotal; }
            set
            {
                if (IsCalculatingQts) return;
                //if (_rowTotal == value) return;
                _rowTotal = value;
                RaisePropertyChanged("RowTotal");
            }
        }

        public string SizeGroup
        {
            get { return _sizeGroup; }
            set
            {
                _sizeGroup = value;
                RaisePropertyChanged("SizeGroup");
            }
        }

        public string StyleColor
        {
            get { return _styleColor; }
            set
            {
                _styleColor = value;
                RaisePropertyChanged("SalesOrderColor");
            }
        }

        public string StyleHeader
        {
            get { return _styleHeader; }
            set
            {
                _styleHeader = value;
                RaisePropertyChanged("StyleHeader");
            }
        }

        #endregion [ Public Properties ]

        #region [ Commands ]

        private ObservableCollection<tbl_PurchaseOrder_AdditionalCost> _additionalCostList;
        private CommandsExecuter _defineAdditionalCostCommand;
        private CommandsExecuter _deleteLineCommand;

        public CommandsExecuter DefineAdditionalCostsCommand
        {
            get { return _defineAdditionalCostCommand ?? (_defineAdditionalCostCommand = new CommandsExecuter(DefineAdditionalCosts) { IsEnabled = true }); }
        }

        public CommandsExecuter DeleteLineCommand
        {
            get
            {
                return _deleteLineCommand ??
                       (_deleteLineCommand = new CommandsExecuter(OnDeletePurchLine) { IsEnabled = true });
            }
        }

        #endregion [ Commands ]

        #region [ Constructor(s) ]

        public NewRfqPurchasOrderDetailsViewModel()
        {
            PurchaseOrderSizes = new ObservableCollection<PurchaseOrderSizeInfo>();
            PurchaseOrderSizes.CollectionChanged += PurchaseOrderSizes_CollectionChanged;

            for (var i = 0; i < 15; i++)
            {
                PurchaseOrderSizes.Add(new PurchaseOrderSizeInfo { SizeCode = "", SizeConsumption = 0, IsTextBoxEnabled = false });
            }

            ColorsList = new ObservableCollection<TblColor>();
            ColorsList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (TblColor item in e.NewItems)
                    {
                        item.PropertyChanged
                            += OnItemOnPropertyChanged;
                    }

                if (e.OldItems != null)
                    foreach (TblColor item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= OnItemOnPropertyChanged;
                    }
            };

            foreach (var item in RFQGlobalLkps.ColorsList)
            {
                ColorsList.Add(item);
            }
            ObjStatus.IsReadyForSaving = true;
        }

        private void OnItemOnPropertyChanged(object s1, PropertyChangedEventArgs e1)
        {
            RaisePropertyChanged(e1.PropertyName);
            if (e1.PropertyName != "SizeConsumption") return;
            if (IsCalculatingQts) return;
            if (ObjStatus.IsLoading) return;
            var tmp = (int)PurchaseOrderSizes.Sum(x => x.SizeConsumption);
            if (IsCalculatingQts) return;
            if (tmp <= ParentRfqSub.Qty)
            {
                TextBoxBackColor = new SolidColorBrush(Colors.White);
                RowTotal = tmp;
                ObjStatus.IsReadyForSaving = true;
            }
            else
            {
                TextBoxBackColor
                    = new SolidColorBrush(Color.FromArgb(255, 255, 22, 22));
                RowTotal = ParentRfqSub.Qty;
                ObjStatus.IsReadyForSaving = false;
            }
        }

        #endregion [ Constructor(s) ]

        #region [ Internal Logic ]

        private void PurchaseOrderSizes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PurchaseOrderSizeInfo item in e.NewItems)
                {
                    item.PropertyChanged += OnItemOnPropertyChanged;

                    item.RatioChangedHandler += (ss, ee) => CalculateBasedOnRatio();
                }

            if (e.OldItems == null) return;
            foreach (PurchaseOrderSizeInfo item in e.OldItems)
            {
                item.PropertyChanged -= OnItemOnPropertyChanged;
            }
        }

        #endregion [ Internal Logic ]

        #region [ Commands bound method ]

        private void DefineAdditionalCosts()
        {
            var temp = new RFQCostingChild(AdditionalCostList ?? (new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>()));
            temp.SubmitCosts += (s, e) =>
            {
                AdditionalCostList = new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>
                    (
                        e.AdditionalCostList.Select
                        (poCosts => Iserial != null ? new tbl_PurchaseOrder_AdditionalCost
                            {
                                CostType = poCosts.CostType,
                                CostValue = poCosts.CostValue,
                                Currency = poCosts.Currency,
                                ExchangeRate = float.Parse(poCosts.ExchangeRate.ToString()),
                                Iserial = poCosts.Iserial,
                                LocalValue = poCosts.LocalValue,
                                ParentPurchLineSerial = (int)Iserial
                            } : new tbl_PurchaseOrder_AdditionalCost
                            {
                                CostType = poCosts.CostType,
                                CostValue = poCosts.CostValue,
                                Currency = poCosts.Currency,
                                ExchangeRate = float.Parse(poCosts.ExchangeRate.ToString()),
                                Iserial = poCosts.Iserial,
                                LocalValue = poCosts.LocalValue
                            }
                        )
                    );
                AdditionalCost = ParentRfqSub.Qty != 0 ? (e.TotalocalValue / ParentRfqSub.Qty) : 0;
            };
            temp.Show();
        }

        #endregion [ Commands bound method ]

        #region [ Internal Logic ]

        private void CalculateBasedOnRatio()
        {
            IsCalculatingQts = true;
            double floatingNumbers = 0;
            var sum = double.Parse(PurchaseOrderSizes.Where(x => x.IsTextBoxEnabled).Sum(x => x.SizeRatio).ToString(CultureInfo.InvariantCulture));
            var num1 = RowTotal;
            foreach (var s in PurchaseOrderSizes.Where(x => x.IsTextBoxEnabled))
            {
                s.SizeConsumption = (num1 * (double.Parse((s.SizeRatio.ToString(CultureInfo.InvariantCulture))) / sum));
            }
            foreach (var s in PurchaseOrderSizes.Where(x => x.IsTextBoxEnabled))
            {
                int dummyValueHolder;
                if (int.TryParse(s.SizeConsumption.ToString(CultureInfo.InvariantCulture), out dummyValueHolder)) continue;
                var x = (Double.Parse(s.SizeConsumption.ToString(CultureInfo.InvariantCulture).Substring(s.SizeConsumption.ToString(CultureInfo.InvariantCulture).IndexOf('.'))));
                floatingNumbers += x;
                s.SizeConsumption = int.Parse(s.SizeConsumption.ToString(CultureInfo.InvariantCulture).Split('.')[0]);
            }
            var topNearInt = (int)(Math.Ceiling(double.Parse(floatingNumbers.ToString(CultureInfo.InvariantCulture))));
            var purchaseOrderSizeInfo =
                PurchaseOrderSizes
                .FirstOrDefault(x => Math.Abs(x.SizeConsumption - PurchaseOrderSizes.Max(m => m.SizeConsumption)) < double.Epsilon);
            if (purchaseOrderSizeInfo != null)
                purchaseOrderSizeInfo
                    .SizeConsumption = (purchaseOrderSizeInfo
                    .SizeConsumption + topNearInt);

            IsCalculatingQts = false;
        }

        #endregion [ Internal Logic ]
    }
}