using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.PromotionViewModel;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchEventNoChildWindow
    {

        public SearchEventNoChildWindow(PromHeaderViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.GetDetailData();
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            var viewModel = DataContext as PromHeaderViewModel;
            var Event = MainGrid.SelectedItem as TBLEVENTUALHEADER;
            viewModel.TransactionHeader.EventPerrow = Event;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            var viewModel = DataContext as PromHeaderViewModel;

            if (viewModel.DetailList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.DetailList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.DetailList.Count != viewModel.DetailFullCount)
            {
                viewModel.GetDetailData();
            }


        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {

            var _viewModel = DataContext as PromHeaderViewModel;
            if (_viewModel.Loading) return;
            _viewModel.DetailList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();

        }

    }
}

