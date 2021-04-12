using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class CheckListMail
    {
        private readonly CheckListMailViewModel _viewModel;

        public CheckListMail()
        {
            InitializeComponent();
            _viewModel = (CheckListMailViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add((TblCheckListMailViewModel)row);
            }
            _viewModel.DeleteMainRow();
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
            if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblCheckListMailViewModel);
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