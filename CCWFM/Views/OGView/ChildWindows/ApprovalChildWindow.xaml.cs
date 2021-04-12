using System.Windows;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ApprovalChildWindow
    {
        private readonly ApprovalViewModel _viewModel;

        public ApprovalChildWindow(StyleHeaderViewModel styleViewModel)
        {

            InitializeComponent();
            _viewModel = new ApprovalViewModel(styleViewModel);
            DataContext = _viewModel;

            _viewModel.ApproveCompleted += (s, r) =>
            {
              
                DialogResult = true;
                _viewModel.Loading = false;
                styleViewModel.SelectedMainRow.DetailsList.Clear();
                styleViewModel.GetDetailData();
            };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //CheckIF
            //_viewModel.SelectedMainRow.Iserial 
            _viewModel.SaveMainRow();
            //DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Down)
            //{
            //    _viewModel.AddNewMainRow(true);
            //}
            //else if (e.Key == Key.Delete)
            //{
            //    _viewModel.SelectedMainRows.Clear();
            //    foreach (var row in MainGrid.SelectedItems)
            //    {
            //        _viewModel.SelectedMainRows.Add(row as TblApprovalViewModel);
            //    }

            //    _viewModel.DeleteMainRow();
            //}
        }
    }
}