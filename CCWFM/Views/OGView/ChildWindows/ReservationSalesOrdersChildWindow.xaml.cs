using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ReservationSalesOrdersChildWindow
    {
        public ReservationSalesOrdersChildWindow(ReservationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SearchSalesOrder();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (ReservationViewModel)DataContext;

            if (viewModel.CheckFabricLineQty())
            {
                viewModel.SelectedMainDetails.RemQty = viewModel.SelectedMainDetails.RemQtyTemp;
                GenericMapper.InjectFromObCollection(viewModel.SelectedMainDetails.ReservationDetailsViewModelList, viewModel.SalesorderList);
                OkButton.IsEnabled = false;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Remaning Quanitity is Not enought");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            var viewModel = (ReservationViewModel)DataContext;
            viewModel.SelectedMainDetails.RemQtyTemp = viewModel.SelectedMainDetails.RemQtyTemp + viewModel.SalesorderList.Sum(x => x.IntialQty);
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Escape:
                    DialogResult = false;
                    break;

                case Key.Enter:

                    break;
            }
        }

        private void btnDeleteRow_Click(object sender, MouseButtonEventArgs e)
        {
            var deleteBtn = sender as Image;

            var viewModel = (ReservationViewModel)DataContext;
            viewModel.SalesorderList.Remove((CRUD_ManagerServiceSalesOrderDto)deleteBtn.DataContext);
        }
      
        private void BtnSearchSalesOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (ReservationViewModel)DataContext;
            viewModel.SearchSalesOrder();
        }
    }
}