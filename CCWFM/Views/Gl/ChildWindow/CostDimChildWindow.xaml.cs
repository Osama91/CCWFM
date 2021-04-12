using System.Windows.Input;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class CostDimChildWindow
    {
        private readonly CostDimChildViewModel _viewModel;
        public CostDimChildWindow(LedgerHeaderViewModel TempViewModel)
        {
            InitializeComponent();
            var viewModel = new CostDimChildViewModel(TempViewModel);
            DataContext = viewModel;
            _viewModel = viewModel;
            
        }


        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }

            }
            else if (e.Key == Key.Delete)
            {

                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add((TblCostDimDetailViewModel)row);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void BtnAddNewMainRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(false);
        }

        private void BtnDeleteMainRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.DeleteMainRow();
        }

        private void BtnSave_Onclick(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

       
   }
}