using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class BomOperations
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        private int _routeGroup;

        public int RouteGroup
        {
            get { return _routeGroup; }
            set
            {
                _routeGroup = value; RaisePropertyChanged("RouteGroup");
                GetRoute();
            }
        }

        private void GetRoute()
        {
            if (RouteGroup != 0 && Vendor != null)
            {
                RouteCardService.GetRoutesAsync(null, null, RouteGroup, Vendor);
            }
        }

        private string _vendor;

        public string Vendor
        {
            get { return _vendor; }
            set
            {
                _vendor = value; RaisePropertyChanged("Vendor");
                GetRoute();
            }
        }

        public bool MainOperation { get; set; }

        private readonly StyleHeaderViewModel _viewModel;


        RouteCardService.RouteCardServiceClient RouteCardService = new RouteCardService.RouteCardServiceClient();


        public BomOperations(StyleHeaderViewModel viewModel, bool mainOperation)
        {
            InitializeComponent();
            DataContext = this;
            MainOperation = mainOperation;
            _viewModel = viewModel;
            _client.SearchVendorsCompleted += (s, e) =>
          {
              if (e.Error != null) return;
              VendAutoComplete.ItemsSource = e.Result;
              VendAutoComplete.PopulateComplete();
          };
            RouteCardService.GetRoutesCompleted += (s, sv) =>
            {
                DgResults.ItemsSource = sv.Result;
            };
            RouteCardService.GetRoutGroupsAsync(null, null);
            RouteCardService.GetRoutGroupsCompleted += (s, sv) =>
            {
                CmbRouteGroup.ItemsSource = sv.Result;
            };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var row = DgResults.SelectedItem as TblRoute;
            if (MainOperation)
            {
                //_viewModel.SelectedSalesOrderOperation.RouteGroupPerRow = row;
                if (row != null) _viewModel.SelectedSalesOrderOperation.TblOperation = row.Iserial;
            }
       

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            _client.SearchVendorsAsync( "ccm", VendAutoComplete.Text);
        }
    }
}