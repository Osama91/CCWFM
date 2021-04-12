using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ExcusesChild
    {
        private readonly EmployeeShiftViewModel _viewModel;

        public ExcusesChild(EmployeeShiftViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;

            DataContext = viewModel;
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                //var currentRowIndex = (_viewModel.SelectedMainRow.SelectedExcuses.IndexOf(_viewModel.selectedExcuse));
                //if (currentRowIndex == (_viewModel.SelectedMainRow.SelectedExcuses.Count - 1))
                //{
                //    var newexe = new TblExcuseViewModel();

                //    if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
                //    {
                //        newexe.Status = _viewModel.ExcuseStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.ExcuseStatusVisibility = _viewModel.ExcuseStatusSelfVisibility;
                //    }
                //    else
                //    {
                //        newexe.Status = _viewModel.ExcuseStatusVisibility == Visibility.Visible ? 1 : 0;
                //        newexe.ExcuseStatusVisibility = _viewModel.ExcuseStatusVisibility;
                //    }
                //    newexe.Emplid = _viewModel.SelectedMainRow.EmpId;

                //    _viewModel.SelectedMainRow.SelectedExcuses.Add(newexe);

                //}
            }
            else if (e.Key == Key.Delete)
            {
                if (_viewModel.selectedExcuse.Iserial != 0)
                {
                    _viewModel.DeleteExcuse(_viewModel.selectedExcuse);
                }
                else
                {
                    if (_viewModel.SelectedMainRow.SelectedExcuses.Count(x => x.TransDate == _viewModel.selectedExcuse.TransDate) > 1)
                    {
                        _viewModel.SelectedMainRow.SelectedExcuses.Remove(_viewModel.selectedExcuse);
                    }
                }
            }
        }

        private void AddBttn_Click(object sender, RoutedEventArgs e)
        {
            var newexe = new TblExcuseViewModel();

            var row = MainGrid.SelectedItem as TblExcuseViewModel;

            if (_viewModel.SelectedMainRow.EmpId == LoggedUserInfo.Code)
            {
                newexe.Status = _viewModel.ExcuseStatusSelfVisibility == Visibility.Visible ? 1 : 0;
                newexe.ExcuseStatusVisibility = _viewModel.ExcuseStatusSelfVisibility;
            }
            else
            {
                newexe.Status = _viewModel.ExcuseStatusVisibility == Visibility.Visible ? 1 : 0;
                newexe.ExcuseStatusVisibility = _viewModel.ExcuseStatusVisibility;
            }
            newexe.Emplid = _viewModel.SelectedMainRow.EmpId;
            if (row != null) newexe.TransDate = row.TransDate;
            newexe.IsNotExtraExcuse = Visibility.Collapsed;
            _viewModel.SelectedMainRow.SelectedExcuses.Insert(_viewModel.SelectedMainRow.SelectedExcuses.IndexOf(_viewModel.selectedExcuse), newexe);
        }

        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Excuse);
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteExcuse(_viewModel.selectedExcuse);
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Excuse);
        }
    }
}