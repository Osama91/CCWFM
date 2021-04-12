using System.Windows;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.AttService;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class EmpVacationDetails
    {
        TblVacationViewModel _selectedVactionDay;
        EmployeeShiftViewModel _viewmodel;
        public EmpVacationDetails(EmployeeShiftViewModel viewmodel, TblVacationViewModel row)
        {
             InitializeComponent();
            _viewmodel = viewmodel;
             DataContext = _viewmodel;
            _selectedVactionDay = row;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in cSPEMPLOYEEVACATIONDataGrid.SelectedItems)
            {
                var vac = row as TblVacationDetail;
                //_selectedVactionDay.CSPVACATIONID = vac.CSPVACATIONID;
                _viewmodel.selectedVacation.CSPVACATIONID = vac.CSPVACATIONID;
                _viewmodel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Vacation);
            }
            DialogResult = true;
            //_viewmodel.IsVactionWindowClosed = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

