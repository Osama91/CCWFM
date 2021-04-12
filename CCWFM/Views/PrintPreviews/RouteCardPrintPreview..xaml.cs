using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Printing;
using CCWFM.ViewModel;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.Views.PrintPreviews
{
    public partial class RouteCardPrintPreview
    {
        private DetailsMainMapper _routeCardObject;

        public DetailsMainMapper RouteCardObject
        {
            get { return _routeCardObject; }
            set { _routeCardObject = value; }
        }

        private PrintDocument pd;

        public RouteCardPrintPreview(RouteCardHeaderViewModel _RouteCardObject)
        {
            InitializeComponent();
            var temp = new DetailsMainMapper(_RouteCardObject);
            PrintingDateTextBlock.Text = "Print Date : " + DateTime.Now.ToString("D");
            RouteCardObject = temp;
            grdRouteCardReportPreview.DataContext = RouteCardObject;
            pd = new PrintDocument();
            pd.PrintPage += pd_PrintPage;
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = grdRouteCardReportPreview;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            wTextBoxTitle.Visibility = Visibility.Collapsed;
            TitleTextBlock.Visibility = Visibility.Visible;
            pd.Print("Route Card Report " + DateTime.Now.ToShortDateString());

            pd.EndPrint += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    new Thread(() =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("There was an error! Route card was not printed");
                        });
                    }).Start();
                }
                else
                {
                    DialogResult = true;
                }
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

    public class DetailsMainMapper : Web.DataLayer.PropertiesViewModelBase
    {
        private ObservableCollection<DetailsSubMapper> _mainDetailsList;

        public ObservableCollection<DetailsSubMapper> MainDetailsList
        {
            get { return _mainDetailsList; }
            set { _mainDetailsList = value; RaisePropertyChanged("MainDetailsList"); }
        }

        #region[ Data Members ]
        private int? _transId;

        public int? TransID
        {
            get { return _transId; }
            set
            {
                _transId = value;
                RaisePropertyChanged("TransID");
            }
        }

        private string _vendorCode;

        public string VendorCode
        {
            get { return _vendorCode; }
            set
            {
                _vendorCode = value;
                RaisePropertyChanged("VendorCode");
            }
        }

        private _Proxy.Vendor _vendor;

        public _Proxy.Vendor Vendor
        {
            get { return _vendor; }
            set
            {
                _vendor = value;
                VendorCode = value.vendor_code;
                RaisePropertyChanged("Vendor");
            }
        }

        private DateTime? _docDate;

        public DateTime? DocDate
        {
            get { return _docDate; }
            set
            {
                _docDate = value;
                RaisePropertyChanged("DocDate");
            }
        }

        private int _processId;

        public int ProcessID
        {
            get { return _processId; }
            set
            { _processId = value; RaisePropertyChanged("_ProcessID"); }
        }

        private int _routId;

        public int RoutID
        {
            get { return _routId; }
            set { _routId = value; RaisePropertyChanged("RoutID"); }
        }

        private RouteCardService.TblRoute _routeItem;

        public RouteCardService.TblRoute RoutItem
        {
            get { return _routeItem; }
            set { _routeItem = value; RaisePropertyChanged("RoutItem"); }
        }

        private DateTime? _delivaryDate;

        public DateTime? DelivaryDate
        {
            get { return _delivaryDate; }
            set { _delivaryDate = value; RaisePropertyChanged("DelivaryDate"); }
        }

        private int _direction;

        public int Direction
        {
            get { return _direction; }
            set { _direction = value; RaisePropertyChanged("Direction"); }
        }

        private int _routGroupId;

        public int RoutGroupID
        {
            get { return _routGroupId; }
            set
            {
                _routGroupId = value;
                RaisePropertyChanged("RoutGroupID");
            }
        }

        private RouteCardService.TblRouteGroup _routGroupItem;

        public RouteCardService.TblRouteGroup RoutGroupItem
        {
            get { return _routGroupItem; }
            set { _routGroupItem = value; RaisePropertyChanged("RoutGroupItem"); }
        }

        private bool? _isPosted;

        public bool? IsPosted
        {
            get { return _isPosted; }
            set
            {
                _isPosted = value;
                RaisePropertyChanged("IsPosted");
            }
        }

        private DateTime _creationDate;

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { _creationDate = value; RaisePropertyChanged("CreationDate"); }
        }

        private DateTime _lastUpdateDate;

        public DateTime LastUpdateDate
        {
            get { return _lastUpdateDate; }
            set { _lastUpdateDate = value; RaisePropertyChanged("LastUpdateDate"); }
        }

        private ObservableCollection<RouteCardViewModel> _cardDetails;

        public ObservableCollection<RouteCardViewModel> RouteCardDetails
        {
            get { return _cardDetails; }
            set { _cardDetails = value; RaisePropertyChanged("RouteCardDetails"); }
        }

        private int _grandTotal;

        public int GrandTotal
        {
            get { return _grandTotal; }
            set { _grandTotal = value; RaisePropertyChanged("GrandTotal"); }
        }

        #endregion

        public DetailsMainMapper(RouteCardHeaderViewModel detailsList)
        {
            DocDate = detailsList.DocDate;
            DelivaryDate = detailsList.DelivaryDate;
            Direction = detailsList.Direction;
            GrandTotal = detailsList.GrandTotal;
            IsPosted = detailsList.IsPosted;
            LastUpdateDate = detailsList.LastUpdateDate;
            ProcessID = detailsList.ProcessID;
            RouteCardDetails = detailsList.RouteCardDetails;
            RoutGroupID = detailsList.RoutGroupID;
            RoutGroupItem = detailsList.RoutGroupItem;
            RoutID = detailsList.RoutID;
            RoutItem = detailsList.RoutItem;
            TransID = detailsList.TransID;
            Vendor = detailsList.VendorPerRow;
            VendorCode = detailsList.VendorCode;           
            MainDetailsList = new ObservableCollection<DetailsSubMapper>();
            foreach (var item in detailsList.RouteCardDetails.Select(x => x.TblSalesOrder).Distinct().ToList())
            {
                MainDetailsList.Add
                    (
                        new DetailsSubMapper
                                (
                                    new ObservableCollection<RouteCardViewModel>(detailsList.RouteCardDetails.Where(x => x.TblSalesOrder == item))
                                )
                    );
            }
        }
    }

    public class DetailsSubMapper : ViewModelBase
    {
        private ObservableCollection<RouteCardViewModel> detailsList;

        public ObservableCollection<RouteCardViewModel> DetailsList
        {
            get { return detailsList; }
            set { detailsList = value; RaisePropertyChanged("DetailsList"); }
        }

        private int styleTotal;

        public int StyleTotal
        {
            get { return styleTotal; }
            set { styleTotal = value; RaisePropertyChanged("StyleTotal"); }
        }

        private List<MapperSizeInfo> columnTotal;

        public List<MapperSizeInfo> ColumnTotals
        {
            get { return columnTotal; }
            set { columnTotal = value; RaisePropertyChanged("ColumnTotals"); }
        }

        public DetailsSubMapper(ObservableCollection<RouteCardViewModel> _DetailsList)
        {
            DetailsList = new ObservableCollection<RouteCardViewModel>(_DetailsList);
            StyleTotal = DetailsList.Sum(x => x.RowTotal);
            ColumnTotals = new List<MapperSizeInfo>();
            foreach (var item in DetailsList[0].RoutCardSizes.Select(x => x.SizeCode).Distinct())
            {
                ColumnTotals.Add
                    (
                        new MapperSizeInfo()
                        {
                            KeyValue = DetailsList.Select(x => x.RoutCardSizes.Where(s => s.SizeCode == item).Sum(c => c.SizeConsumption)).Sum(),
                            Key = item
                        }
                    );
            }
        }
    }

    public class MapperSizeInfo : ViewModelBase
    {
        private string key;

        public string Key
        {
            get { return key; }
            set { key = value; RaisePropertyChanged("Key"); }
        }

        private int keyValue;

        public int KeyValue
        {
            get { return keyValue; }
            set { keyValue = value; RaisePropertyChanged("KeyValue"); }
        }
    }
}