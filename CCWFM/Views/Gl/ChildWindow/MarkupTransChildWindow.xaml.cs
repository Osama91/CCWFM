using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class MarkupTransChildWindow
    {
        private readonly RecInvViewModel _viewModel;

        public MarkupTransChildWindow(RecInvViewModel viewModel, bool header)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            if (header)
            {
                //DetailGrid.Visibility = Visibility.Collapsed;
                _viewModel.GetMarkUpdata(true);
            }
            else
            {
                HeaderGrid.Visibility = Visibility.Collapsed;

                _viewModel.GetMarkUpdata(false);
            }
        }

        private void HeaderGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.MarkUpTransList.IndexOf(_viewModel.SelectedMarkupRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.MarkUpTransList.Count - 1))
                {
                    _viewModel.AddNewMarkUpRow(true, true,0);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.DeleteMarkupRow(true);
            }
        }

        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMarkupRow();
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var variable in e.RemovedItems)
            {
                _viewModel.SaveMarkupRowOldRow((TblMarkupTransViewModel) variable);
            }
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void HeaderGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (_viewModel.SelectedMarkupRow.Disabled)
            //{
            //    e.Cancel = true;
            //}

        }
    }
}