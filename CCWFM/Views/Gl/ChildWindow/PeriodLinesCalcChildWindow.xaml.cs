using System.Windows;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class PeriodLinesCalcChildWindow
    {
        public PeriodLinesCalcChildWindow(PeriodGlViewModel viewModel)
        {
            
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}