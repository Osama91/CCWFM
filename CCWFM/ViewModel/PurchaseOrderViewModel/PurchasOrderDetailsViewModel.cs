using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace CCWFM.ViewModel.PurchaseOrderViewModel
{
    public class PurchasOrderDetailsViewModel : ViewModelBase
    {
        private string _objectIndex;

        public string ObjectIndex
        {
            get { return _objectIndex; }
            set
            {
                _objectIndex = value;
                RaisePropertyChanged("ObjectIndex");
            }
        }

        private string _styleHeader;

        public string StyleHeader
        {
            get { return _styleHeader; }
            set
            {
                _styleHeader = value;
                RaisePropertyChanged("StyleHeader");
            }
        }

        public string SalesOrder { get; set; }

        public string SalesOrderColor { get; set; }

        private int _direction;

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; RaisePropertyChanged("Direction"); }
        }

        private int _parentTransId;

        public int ParentTransID
        {
            get { return _parentTransId; }
            set { _parentTransId = value; RaisePropertyChanged("StyleHeader"); }
        }

        private decimal _price;

        public decimal Price
        {
            get { return _price; }
            set { _price = value; RaisePropertyChanged("Price"); }
        }

        public string _SizeGroup { get; set; }

        public ObservableCollection<string> _Degrees { get; set; }

        private ObservableCollection<PurchaseOrderSizeInfo> _purchaseOrderSizes;

        public ObservableCollection<PurchaseOrderSizeInfo> PurchaseOrderSizes
        {
            get { return _purchaseOrderSizes; }
            set
            {
                _purchaseOrderSizes = value;
                RaisePropertyChanged("PurchaseOrderSizes");
            }
        }

        private int _rowTotal;

        public int RowTotal
        {
            get { return _rowTotal; }
            private set
            {
                _rowTotal = value;
                RaisePropertyChanged("RowTotal");
            }
        }

        public PurchasOrderDetailsViewModel()
        {
            _Degrees = new ObservableCollection<string> { "1st", "2nd", "3rd" };

            PurchaseOrderSizes = new ObservableCollection<PurchaseOrderSizeInfo>();
            PurchaseOrderSizes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PurchaseOrderSizes_CollectionChanged);

            for (int i = 0; i < 15; i++)
            {
                PurchaseOrderSizes.Add(new PurchaseOrderSizeInfo() { SizeCode = "", SizeConsumption = 0, IsTextBoxEnabled = false });
            }
        }

        private void PurchaseOrderSizes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PurchaseOrderSizeInfo item in e.NewItems)
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);

            if (e.OldItems != null)
                foreach (PurchaseOrderSizeInfo item in e.OldItems)
                    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RowTotal = PurchaseOrderSizes.Sum(x => x.SizeConsumption);
        }
    }

    public class PurchaseOrderSizeInfo : ViewModelBase
    {
        private string _sizeCode;

        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                _sizeCode = value;
                RaisePropertyChanged("SizeCode");
            }
        }

        private decimal _sizeRatio;

        public decimal SizeRatio
        {
            get { return _sizeRatio; }
            set { _sizeRatio = value; }
        }

        private int _sizeConsumption;

        public int SizeConsumption
        {
            get { return _sizeConsumption; }
            set
            {
                _sizeConsumption = value;
                RaisePropertyChanged("SizeConsumption");
            }
        }

        private bool _isTextBoxEnabled;

        public bool IsTextBoxEnabled
        {
            get { return _isTextBoxEnabled; }
            set
            {
                _isTextBoxEnabled = value;
                RaisePropertyChanged("IsTextBoxEnabled");
            }
        }
    }
}