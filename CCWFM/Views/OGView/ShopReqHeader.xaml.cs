using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class ShopReqHeader
    {
        private readonly ShopReqHeaderViewModel _viewModel;

        public ShopReqHeader()
        {
            
            InitializeComponent();
            _viewModel = (ShopReqHeaderViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            
        }

        private void CbBrand_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.Brand = new CCWFM.CRUDManagerService.GenericTable() { Iserial = _viewModel.TblItemDownLoadDef };
            _viewModel.GetMaindata();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void MainComment_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
           _viewModel.SaveComment();
        }

        private void MainWareHouse_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveWarehouse();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainWareHouse.SelectedIndex != -1);
        }

        private void MainGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.TblShopReqInvs.IndexOf(_viewModel.SelectedShopReqInvRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.TblShopReqInvs.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainWareHouse.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblShopReqInvViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }
    }
}