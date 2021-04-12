using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class RecieveDyedFabric
    {
        private readonly DyeingOrderViewModel _viewModel;

        public RecieveDyedFabric(DyeingOrderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            var orderDetails = new ObservableCollection<TblDyeingOrderDetailsViewModel>();
            foreach (var item in _viewModel.DyeingOrderMainDetailList.Where(x => x.TransactionType == 0).ToList())
            {
                foreach (var items in item.TblDyeingOrderDetails)
                {
                    var t = items;
                    orderDetails.Add(items);
                }
            }

            var s = from d in orderDetails
                    group d by new
                    {
                        d.BatchNo,
                        d.FabricCode,
                        d.DyeingClass,
                        d.EstimatedDeliveryDate,
                        d.DyedFabric,
                        d.Color,
                        d.DyeingProductionOrder,
                        d.Unit
                    }
                        into g
                        select new TblDyeingOrderDetailsViewModel
                        {
                            BatchNo = g.Key.BatchNo,
                            CalculatedTotalQty = g.Sum(x => x.CalculatedTotalQty),
                            Color = g.Key.Color,
                            DyedFabric = g.Key.DyedFabric,
                            DyeingClass = g.Key.DyeingClass,
                            DyeingProductionOrder = g.Key.DyeingProductionOrder,
                            EstimatedDeliveryDate = g.Key.EstimatedDeliveryDate,
                            FabricCode = g.Key.FabricCode,
                            TransactionType = g.FirstOrDefault().TransactionType,
                            Unit = g.Key.Unit,
                        };

            //ObservableCollection<TblDyeingOrderDetailsViewModel> Test = new ObservableCollection<TblDyeingOrderDetailsViewModel>();
            //foreach (var item in s)
            //{
            //    Test.Add(new TblDyeingOrderDetailsViewModel
            //    {
            //        BatchNo = item.BatchNo,
            //        CalculatedTotalQty = item.CalculatedTotalQty,
            //        Color = item.Color,
            //        DyedFabric = item.DyedFabric,
            //        DyeingClass = item.DyeingClass,
            //        DyeingProductionOrder = item.DyeingProductionOrder,
            //        EstimatedDeliveryDate = item.EstimatedDeliveryDate,
            //        FabricCode = item.FabricCode,
            //        Unit = item.Unit,
            //    });
            //}

            dgDyeingOrderDetails.ItemsSource = s;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TblDyeingOrderDetailsViewModel item in dgDyeingOrderDetails.SelectedItems)
            {
                item.TransactionType = 1;
                item.TransId = _viewModel.SelectedMainDetails.TransId;
                _viewModel.SelectedMainDetails.TblDyeingOrderDetails.Add(item);
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnDeleteOrderDetails_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnDyeingOrderServices_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}