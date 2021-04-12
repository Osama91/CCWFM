using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.AttViewModel;

namespace CCWFM.Views.OGView
{
    public partial class EmployeeInfo
    {
        public EmployeeInfo()
        {
            InitializeComponent();
            var viewModel = (EmployeeInfoViewModel)LayoutRoot.DataContext;
            DataContext = viewModel;
            viewModel.Code = LoggedUserInfo.Code;
            viewModel.GetMaindata();
        }

        private void ExportExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
           MainGrid.ExportExcel("Employee Info");
            
        }
    }
}