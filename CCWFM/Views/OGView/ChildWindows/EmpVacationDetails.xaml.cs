using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class EmpVacationChild
    {
        public EmpVacationChild(EmployeeShiftViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
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

