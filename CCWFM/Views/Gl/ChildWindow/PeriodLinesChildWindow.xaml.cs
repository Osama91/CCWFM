using CCWFM.ViewModel.Gl;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class PeriodLinesChildWindow
    {
        ViewModel.Gl.PeriodGlViewModel _ViewModel;
        public PeriodLinesChildWindow(ViewModel.Gl.PeriodGlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _ViewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {

             _ViewModel.SaveDetailRow();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var Selectedrow = btn.DataContext as TblPeriodLineViewModel;
            _ViewModel.ClosePeriodLine(Selectedrow);
        }
    }
}