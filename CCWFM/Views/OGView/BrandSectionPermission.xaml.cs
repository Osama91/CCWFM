using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class BrandSectionPermission
    {
        private readonly BrandSectionPermissionViewModel _viewModel;

        public BrandSectionPermission(int userIserial)
        {
            InitializeComponent();
            _viewModel = (BrandSectionPermissionViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add((TblBrandSectionPermissionViewModel)row);
            }
            _viewModel.DeleteMainRow();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblBrandSectionPermissionViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }
    }
}