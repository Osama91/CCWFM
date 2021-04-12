using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.Gl;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class CostCenterShopArea
    {
        private readonly ShopAreaViewModel _viewModel;

        public CostCenterShopArea(ShopAreaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void DetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailsValuesObjects = valueObjecttemp;

            _viewModel.GetDetailData();
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetDetailData();
            }
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 2))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in TblPeriodLineDataGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblCostCenterShopAreaModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }
    }
}