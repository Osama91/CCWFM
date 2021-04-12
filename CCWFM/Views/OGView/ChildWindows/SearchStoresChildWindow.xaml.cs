using System;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.PromotionViewModel;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SearchStoresChildWindow
    {
        private readonly PromoHeaderGifts _viewModel;

        public SearchStoresChildWindow(PromoHeaderGifts viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.StoreList.Clear();
        }

        public void Cleartextboxs()
        {
            _viewModel.Code = "";
            _viewModel.StorEname = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.StoreList.Clear();
                _viewModel.SearchForStorEname();
            }
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                _viewModel.TransactionHeader.StorePerRow = (TblStore)MainGrid.SelectedItem;
                Cleartextboxs();
                DialogResult = true;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            _viewModel.StoreList.Clear();
            _viewModel.SearchForStorEname();
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
        }
    }
}