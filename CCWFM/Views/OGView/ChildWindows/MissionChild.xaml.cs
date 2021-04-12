using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class MissionsChild
    {
        private readonly EmployeeShiftViewModel _viewModel;
        public MissionsChild(EmployeeShiftViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            DataContext = viewModel;
            try
            {
                if (viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
                {
                    MainGrid.Columns.LastOrDefault().Visibility = Visibility.Collapsed;
                }
            }
            catch (System.Exception)
            {

                MainGrid.Columns.LastOrDefault().Visibility = Visibility.Collapsed;
            }
           
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                //var currentRowIndex = (_viewModel.SelectedMainRow.SelectedMissions.IndexOf(_viewModel.selectedMission));
                //if (currentRowIndex == (_viewModel.SelectedMainRow.SelectedMissions.Count - 1))
                //{

                //    var newexe = new TblMissionViewModel();

                //    if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
                //    {
                //        newexe.Status = _viewModel.MissionStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.MissionStatusVisibility = _viewModel.MissionStatusSelfVisibility;
                //    }
                //    else
                //    {
                //        newexe.Status = _viewModel.MissionStatusVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.MissionStatusVisibility = _viewModel.MissionStatusVisibility;
                //    }
                //    newexe.Emplid = _viewModel.SelectedMainRow.EmpId;

                //    _viewModel.SelectedMainRow.SelectedMissions.Add(newexe);
                    
                //}
            }
            else if (e.Key == Key.Delete)
            {
                if (_viewModel.selectedMission.Iserial!=0)
                {
                    
                
                _viewModel.DeleteMission(_viewModel.selectedMission);
                }
                else
                {
                    if (_viewModel.SelectedMainRow.SelectedMissions.Count(x => x.FromDate == _viewModel.selectedMission.FromDate) > 1)
                    {
                        _viewModel.SelectedMainRow.SelectedMissions.Remove(_viewModel.selectedMission);
                    }
                }
                
            }
        }
     
        private void AddBttn_Click(object sender, RoutedEventArgs e)
        {
            var newexe = new TblMissionViewModel();

            var row= MainGrid.SelectedItem as TblMissionViewModel;
            

            if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
            {
                newexe.Status = _viewModel.MissionStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                newexe.MissionStatusVisibility = _viewModel.MissionStatusSelfVisibility;
            }
            else
            {
                newexe.Status = _viewModel.MissionStatusVisibility == Visibility.Visible ? 1 : 0;
                newexe.MissionStatusVisibility = _viewModel.MissionStatusVisibility;
            }
            newexe.Emplid = _viewModel.SelectedMainRow.EmpId;
            newexe.FromDate = row.FromDate;
            newexe.ToDate = row.FromDate;
            newexe.IsNotExtraMission = Visibility.Collapsed;
            _viewModel.SelectedMainRow.SelectedMissions.Insert(_viewModel.SelectedMainRow.SelectedMissions.IndexOf(_viewModel.selectedMission), newexe);           
        }

        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Mission);
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteMission(_viewModel.selectedMission);
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Mission);
        }
    }
}