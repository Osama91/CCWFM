using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class VacationsChild
    {
        private readonly EmployeeShiftViewModel _viewModel;
        public VacationsChild(EmployeeShiftViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            DataContext = viewModel;
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                //var currentRowIndex = (_viewModel.SelectedMainRow.SelectedVacations.IndexOf(_viewModel.selectedVacation));
                //if (currentRowIndex == (_viewModel.SelectedMainRow.SelectedVacations.Count - 1))
                //{

                //    var newexe = new TblVacationViewModel();

                //    if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
                //    {
                //        newexe.Status = _viewModel.VacationStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.VacationStatusVisibility = _viewModel.VacationStatusSelfVisibility;
                //    }
                //    else
                //    {
                //        newexe.Status = _viewModel.VacationStatusVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.VacationStatusVisibility = _viewModel.VacationStatusVisibility;
                //    }
                //    newexe.Emplid = _viewModel.SelectedMainRow.EmpId;

                //    _viewModel.SelectedMainRow.SelectedVacations.Add(newexe);
                    
                //}
            }
            else if (e.Key == Key.Delete)
            {
                if (_viewModel.selectedVacation.Iserial!=0)
                {
                    
                
                _viewModel.DeleteVacation(_viewModel.selectedVacation);
                }
                else
                {
                    if (_viewModel.SelectedMainRow.SelectedVacations.Count(x => x.FromDate == _viewModel.selectedVacation.FromDate) > 1)
                    {
                        _viewModel.SelectedMainRow.SelectedVacations.Remove(_viewModel.selectedVacation);
                    }
                }
                
            }
        }
     
        private void AddBttn_Click(object sender, RoutedEventArgs e)
        {
            var newexe = new TblVacationViewModel();

            var row= MainGrid.SelectedItem as TblVacationViewModel;
            

            if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
            {
                newexe.Status = _viewModel.VacationStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                newexe.VacationStatusVisibility = _viewModel.VacationStatusSelfVisibility;
            }
            else
            {
                newexe.Status = _viewModel.VacationStatusVisibility == Visibility.Visible ? 1 : 0;
                newexe.VacationStatusVisibility = _viewModel.VacationStatusVisibility;
            }
            newexe.Emplid = _viewModel.SelectedMainRow.EmpId;
            newexe.FromDate = row.FromDate;
            newexe.ToDate = row.FromDate;
            _viewModel.SelectedMainRow.SelectedVacations.Insert(_viewModel.SelectedMainRow.SelectedVacations.IndexOf(_viewModel.selectedVacation), newexe);           
        }

        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Vacation);
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteVacation(_viewModel.selectedVacation);
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Vacation);
        }
    }
}