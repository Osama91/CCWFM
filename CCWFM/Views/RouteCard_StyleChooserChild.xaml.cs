using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using _Proxy = CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views
{
    public partial class RouteCard_StyleChooserChild
    {
        private readonly _Proxy.CRUD_ManagerServiceClient _serviceClient = new _Proxy.CRUD_ManagerServiceClient();
        private RouteCardHeaderViewModel ViewModel;
        public bool BoolCuttingQty { get; set; }
        public RouteCard_StyleChooserChild(int operation, int direction, int routeType, bool boolCuttingQty, RouteCardHeaderViewModel myViewModel)
        {
            InitializeComponent();
            RouteCardDetailOperationList = new List<RouteCardService.RouteCardDetail>();
            DataContext = this;
            _operation = operation;
            _direction = direction;
            _routeType = routeType;
            _serviceClient.GetSalesOrderColorsCompleted += _ServiceClient_GetSalesOrderColorsCompleted;
            BoolCuttingQty = boolCuttingQty;
            ReturnResult = new ObservableCollection<RouteCardViewModel>();
            ViewModel = myViewModel;

        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int _operation;
        private readonly int _direction;
        private readonly int _routeType;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public ObservableCollection<RouteCardViewModel> ReturnResult { get; set; }

        public event EventHandler SubmitAction;

        private _Proxy.TblSalesOrder _salesOrderPerRow;

        public _Proxy.TblSalesOrder SalesOrderPerRow
        {
            get { return _salesOrderPerRow; }
            set
            {
                _salesOrderPerRow = value; RaisePropertyChanged("SalesOrderPerRow");
                if (SalesOrderPerRow != null)
                    _serviceClient.GetSalesOrderColorsAsync(null, null, _routeType, SalesOrderPerRow.Iserial, _operation, _direction,BoolCuttingQty,LoggedUserInfo.Iserial);
            }
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dGridStyles.SelectedItems.Count > 0)
                {
                    if (SubmitAction != null)
                    {
                        foreach (_Proxy.TblSalesOrderColor item in dGridStyles.SelectedItems)
                        {
                            if (RouteCardDetailOperationList != null && RouteCardDetailOperationList.Any(x => x.TblColor == item.TblColor))
                            {
                                foreach (
                                    var variable in
                                        RouteCardDetailOperationList.Where(x => x.TblColor == item.TblColor)
                                            .GroupBy(x => x.Degree))
                                {
                                    var temp = new RouteCardViewModel
                                    {
                                        WareHouseDegreeList = ViewModel.WareHouseDegreeList,
                                        TblSalesOrder = item.TblSalesOrder,
                                        TblColor = item.TblColor,
                                        ObjectIndex = Guid.NewGuid().ToString("D"),
                                        Price = CostPerOperation,
                                        ColorPerRow = item.TblColor1,
                                        SalesOrderPerRow = item.TblSalesOrder1,
                                        Degree = variable.Key,
                                       
                                    };

                                    foreach (var sizeItem in item.TblSalesOrderSizeRatios)
                                    {
                                        var routeCardSize = new RoutCardSizeInfo
                                        {
                                            SizeCode = sizeItem.Size,
                                            SizeConsumption = (int)sizeItem.Ratio,
                                            IsTextBoxEnabled = true
                                        };
                                        temp.RoutCardSizes.Add(routeCardSize);
                                    }

                                    foreach (var newVariable in temp.RoutCardSizes)
                                    {
                                        if (RouteCardDetailOperationList != null)
                                        {
                                            var row =
                                                RouteCardDetailOperationList.FirstOrDefault(
                                                    x =>
                                                        x.Size == newVariable.SizeCode && x.TblColor == item.TblColor &&
                                                        x.Degree == variable.Key);
                                            if (row != null)
                                            {
                                                if (row.SizeQuantity != null)
                                                    newVariable.SizeConsumption = (int)row.SizeQuantity;
                                            }
                                            else
                                            {
                                                newVariable.SizeConsumption = 0;
                                            }
                                        }
                                        else
                                        {
                                            newVariable.SizeConsumption = 0;
                                        }
                                    }
                                    ReturnResult.Add(temp);
                                }
                            }
                            else
                            {
                               
                                    var temp = new RouteCardViewModel
                                    {
                                        WareHouseDegreeList = ViewModel.WareHouseDegreeList,
                                        TblSalesOrder = item.TblSalesOrder,
                                        TblColor = item.TblColor,
                                        ObjectIndex = Guid.NewGuid().ToString("D"),
                                        
                                        ColorPerRow = item.TblColor1,
                                        SalesOrderPerRow = item.TblSalesOrder1,
                                        Degree = "1st",
                                        Price = CostPerOperation,
                                    
                                    };

                                    foreach (var sizeItem in item.TblSalesOrderSizeRatios)
                                    {
                                        var routeCardSize = new RoutCardSizeInfo
                                        {
                                            SizeCode = sizeItem.Size,
                                            SizeConsumption = (int)sizeItem.Ratio,
                                            IsTextBoxEnabled = true
                                        };
                                        temp.RoutCardSizes.Add(routeCardSize);


                                    }
                                ReturnResult.Add(temp);
                                }

                            
                        }
                        ViewModel.RouteCardDetailOperationList = RouteCardDetailOperationList;
                        SubmitAction(this, new EventArgs());
                    }
                }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                var errw = new ErrorWindow(ex);
                errw.Show();
            }
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void _ServiceClient_GetSalesOrderColorsCompleted(object sender, _Proxy.GetSalesOrderColorsCompletedEventArgs e)
        {
            try
            {
               CostPerOperation= e.Cost;
                dGridStyles.ItemsSource = e.Result.ToList();
                RouteCardDetailOperationList.Clear();

                if (e.RouteCardDetailList!=null)
                {
                    foreach (var item in e.RouteCardDetailList.ToList())
                    {
                        RouteCardDetailOperationList.Add(new RouteCardService.RouteCardDetail().InjectFrom(item) as RouteCardService.RouteCardDetail);
                    }

                }



            }
            catch (Exception ex)
            {
                var errw = new ErrorWindow(ex);
                errw.Show();
            }
        }

        public float CostPerOperation { get; set; }

        public List<RouteCardService.RouteCardDetail> RouteCardDetailOperationList { get; set; }

        public void ResetChild()
        {
            dGridStyles.ItemsSource = null;
        }
    }
}