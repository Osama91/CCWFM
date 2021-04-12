using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.ViewModel.OGViewModels.Mappers;
using CCWFM.ViewModel.RouteCardViewModelClasses;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class CuttingOrderChildWindow
    {
        public event EventHandler SubmitClicked;

        public List<string> RollUnitList { get; set; }

        public CuttingOrderChildWindow(MarkerViewModel markerViewModel)
        {
            RollUnitList = new List<string> { "KG", "Meter" };
            InitializeComponent();
            DataContext = markerViewModel;
            CuttingOrderListChildWindows = new ObservableCollection<CuttingOrderViewModel>();
        }

        public CuttingOrderChildWindow(RouteCardFabricViewModel markerViewModel)
        {
            RollUnitList = new List<string> { "KG", "Meter" };
            InitializeComponent();
            DataContext = markerViewModel;
            CuttingOrderListChildWindows = new ObservableCollection<CuttingOrderViewModel>();

            var client = new CRUD_ManagerServiceClient();
            if (markerViewModel.FabricColorPerRow != null)
            {
                if (markerViewModel.ItemId != null)
                {
                    client.InspectionRouteAsync(markerViewModel.ItemId, markerViewModel.FabricColorPerRow.Iserial);
                }
                else
                {

                    MessageBox.Show("You have to select Fabric First");
                }
            }
            else
            {
                MessageBox.Show("You have to select Fabric Color First");
            }

            client.InspectionRouteCompleted += (s, sv) =>
            {
                var list = new ObservableCollection<CuttingOrderViewModel>();
                foreach (var item in sv.Result)
                {
                    list.Add(MarkerMapper.MapToCuttingOrder(item, 1));
                }
                DgCuttingOrder.ItemsSource = list;
            };
        }

        private ObservableCollection<CuttingOrderViewModel> _cuttingOrderListChildWindows;

        public ObservableCollection<CuttingOrderViewModel> CuttingOrderListChildWindows
        {
            get
            {
                return _cuttingOrderListChildWindows;
            }
            set
            {
                if ((ReferenceEquals(_cuttingOrderListChildWindows, value) != true))
                {
                    _cuttingOrderListChildWindows = value;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubmitClicked != null)
            {
                foreach (CuttingOrderViewModel item in DgCuttingOrder.SelectedItems)
                {
                    CuttingOrderListChildWindows.Add(item);
                }
                SubmitClicked(this, new EventArgs());
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}