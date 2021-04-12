using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.RouteCardViewModelClasses;

namespace CCWFM.Views
{
    public partial class RouteCardFabricRemChildWindow : ChildWindow
    {
        private readonly RouteCardService.RouteCardServiceClient _client = new RouteCardService.RouteCardServiceClient();

        public RouteCardFabricRemChildWindow(RouteCardHeaderViewModel viewModel, int SearchCriteria = 0)
        {
            InitializeComponent();
            DataContext = viewModel;

            if (SearchCriteria == 1)
            {
                viewModel.RouteCardFabricRemViewModelList = new ObservableCollection<RouteCardFabricViewModel>();
                viewModel.SearchPagedCollection = new PagedCollectionView(viewModel.RouteCardFabricRemViewModelList);
                SearchCriteria = 0;
            }

            _client.GetRemRouteQuantityCompleted += (s, v) =>
            {
                foreach (var item in v.Result)
                {
                    viewModel.RouteCardFabricRemViewModelList.Add(RouteCardMappers.MapToViewModel(item));
                }
                viewModel.SearchPagedCollection = new PagedCollectionView(viewModel.RouteCardFabricRemViewModelList);
                viewModel.SearchPagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("TransID"));
                DgfabricIssue.SelectedIndex = viewModel.SearchPagedCollection == null ? -1 : viewModel.SearchPagedCollection.Count - 1;
            };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as RouteCardHeaderViewModel;

            foreach (var item in DgfabricIssue.SelectedItems)
            {
                if (viewModel != null) viewModel.RouteCardFabricViewModelList.Add((RouteCardFabricViewModel)item);
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TxtSearch_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Enter)
            {
                var viewModel = (RouteCardHeaderViewModel)DataContext;
                //viewModel.SearchPagedCollection = ;
                LoadData();
            }
         
        }

        private void LoadData()
        {
            var viewModel = (RouteCardHeaderViewModel)DataContext;
            var requiredTransactionTYpe = 0;
            switch (viewModel.TransactionType)
            {
                case 2:
                    requiredTransactionTYpe = 1;
                    break;

                case 3:
                    requiredTransactionTYpe = 1;
                    break;

                case 4:
                    requiredTransactionTYpe = 3;
                    break;

                case 6:
                    requiredTransactionTYpe = 5;
                    break;

                default:
                    break;
            }

            var transid = 0;
            if (!string.IsNullOrWhiteSpace(TxtSalesOrder.Text))
                if (!string.IsNullOrWhiteSpace(TxtItemIt.Text))
                {
                    _client.GetRemRouteQuantityAsync(viewModel.VendorCode, viewModel.RoutGroupID, viewModel.RoutID
                                , requiredTransactionTYpe, transid, TxtSalesOrder.Text, TxtItemIt.Text, viewModel.SearchPagedCollection== null? 0: viewModel.SearchPagedCollection.Count, 20);
                }
        }

        private void DgfabricIssue_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (RouteCardHeaderViewModel)DataContext;
            if (viewModel.SearchPagedCollection.Count < 20)
            {
                return;
            }
            if (viewModel.SearchPagedCollection.Count - 2 < e.Row.GetIndex())
            {
                viewModel.Loading = true;
                LoadData();
            }
        }
    }
}