using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;
using CCWFM.AttService;

namespace CCWFM.Views.OGView
{
    public partial class EmployeeShift
    {
        private readonly EmployeeShiftViewModel _viewModel;

        public int? Fromtime { get; set; }

        public int? Totime { get; set; }

        AttService.AttServiceClient AttService = new AttService.AttServiceClient();

        public EmployeeShift()
        {
            InitializeComponent();
            GridAttendance.Visibility = Visibility.Collapsed;
            _viewModel = (EmployeeShiftViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            _viewModel.Code = LoggedUserInfo.Code;
            _viewModel.GetPoisition(LoggedUserInfo.Code);
            _viewModel.GetEmpTransportationLine(LoggedUserInfo.Code);
            _viewModel.PremCompleted += (s, sv) =>
            {
                
                //if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "DisableManualAttendance") != null)
                //{
                //    DisableManualAttendance = false;
                //}
                //else
                //{
                //  DisableManualAttendance = true;

                //}

                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "NoDayLimit") != null)
                {
                    _viewModel.NoDayLimit = true;
                }

                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "NoDayLimitApproval") != null)
                {
                    _viewModel.NoDayLimitApproval = true;
                }

                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "AttFilePost") != null)
                {
                    AttendanceFileGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "Status").Visibility =
                        Visibility.Visible;
                    _viewModel.AttFileStatusVisibility = Visibility.Visible;
                }
                else
                {
                    AttendanceFileGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "Status").Visibility =
                        Visibility.Collapsed;
                }
                var employeeShiftTabForm =
                    _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "EmployeeShiftTabForm");

                if (employeeShiftTabForm != null)
                {
                    _viewModel.EmpShiftAllowAdd =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == employeeShiftTabForm.Iserial).AllowNew ?? false;
                    _viewModel.EmpShiftAllowUpdate =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == employeeShiftTabForm.Iserial).AllowUpdate ?? false;
                    _viewModel.EmpShiftAllowDelete =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == employeeShiftTabForm.Iserial).AllowDelete ?? false
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
                var vacationTabForm = _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "VacationTabForm");

                if (vacationTabForm != null)
                {
                    _viewModel.VacationAllowAdd =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == vacationTabForm.Iserial).AllowNew ?? false;
                    _viewModel.VacationAllowUpdate =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == vacationTabForm.Iserial).AllowUpdate ?? false;
                    _viewModel.VacationAllowDelete =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == vacationTabForm.Iserial).AllowDelete ?? false
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
                var excuseTabForm = _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "ExcuseTabForm");

                if (excuseTabForm != null)
                {
                    _viewModel.ExcuseAllowAdd =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == excuseTabForm.Iserial).AllowNew ?? false;
                    _viewModel.ExcuseAllowUpdate =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == excuseTabForm.Iserial).AllowUpdate ?? false;
                    _viewModel.ExcuseAllowDelete =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == excuseTabForm.Iserial).AllowDelete ?? false
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
                var missionTabForm = _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "MissionTabForm");

                if (missionTabForm != null)
                {
                    _viewModel.MissionAllowAdd =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == missionTabForm.Iserial).AllowNew ?? false;
                    _viewModel.MissionAllowUpdate =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == missionTabForm.Iserial).AllowUpdate ?? false;
                    _viewModel.MissionAllowDelete =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == missionTabForm.Iserial).AllowDelete ?? false
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
                var attFileTabForm = _viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "AttFileTabForm");

                if (attFileTabForm != null)
                {
                    _viewModel.AttFileAllowAdd =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == attFileTabForm.Iserial).AllowNew ?? false;
                    _viewModel.AttFileAllowUpdate =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == attFileTabForm.Iserial).AllowUpdate ?? false;
                    _viewModel.AttFileAllowDelete =
                        LoggedUserInfo.WFM_UserJobPermissions.SingleOrDefault(
                            x => x.TblPermission == attFileTabForm.Iserial).AllowDelete ?? false
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
            };
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetEmpByStore();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_viewModel.DayDate != null)
                {
                    var week = GetIso8601WeekOfYear((DateTime)_viewModel.DayDate);
                    if (_viewModel.WeekList.Contains(week))
                    {
                        _viewModel.Week = week;
                    }
                }
            }
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        //  private int _lastSelectedTab = -1;

        private void TabControlAtt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlAtt != null)
            {
                var item = TabControlAtt.SelectedItem as TabItem;
                //if (_lastSelectedTab != TabControlAtt.SelectedIndex)
                //{
                //
                //    if (_viewModel.Changed)
                //    {
                //        var res = MessageBox.Show("Changes Has Been Made Can Be Lost After Changing The Tab ! SaveChanges ?", "SaveChanges",
                //   MessageBoxButton.OKCancel);
                //        if (res == MessageBoxResult.OK)
                //        {
                //            TabControlAtt.SelectedIndex = _lastSelectedTab;
                //            BtnSave_Onclick(null, null);
                //        }
                //    }
                //    _lastSelectedTab = TabControlAtt.SelectedIndex;
                //}
                _viewModel.SelectedTab = item.Name;
                TxtExcDesc.Visibility = Visibility.Collapsed;
                TxtMissionDesc.Visibility = Visibility.Collapsed;
                TxtVacationDesc.Visibility = Visibility.Collapsed;
                GridAttendance.Visibility = Visibility.Collapsed;
                switch (item.Name)
                {
                    case "Shift":

                        break;

                    case "Vacation":
                        TxtVacationDesc.Visibility = Visibility.Visible;
                        _viewModel.IsVactionWindowClosed = false;
                        break;

                    case "Excuse":

                        TxtExcDesc.Visibility = Visibility.Visible;
                        break;

                    case "Mission":

                        TxtMissionDesc.Visibility = Visibility.Visible;
                        break;

                    case "Attendance":
                        if (_viewModel.TransportationAllowed)
                        {
                            GridAttendance.Visibility = Visibility.Visible;
                        }

                        break;
                }
                _viewModel.GetEmpByStore();
            }
        }

        private void BtnDeleteExcuse_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblExcuseViewModel;
                _viewModel.DeleteExcuse(row);
            }
        }

        private void btnDeleteEmployeeShifte_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblEmployeeshiftViewModel;
                _viewModel.DeleteEmployeeShift(row);
            }
        }

        private void BtnDeleteVacationOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblVacationViewModel;
                _viewModel.DeleteVacation(row);
            }
        }

        private void BtnDeleteMissionOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblMissionViewModel;
                _viewModel.DeleteMission(row);
            }
        }

        private void OsGrid_OnOnFilter(object sender, FilterEvent e)
        {
            var emp = "";
            var name = "";
            foreach (var f in e.FiltersPredicate)
            {
                if (f.Column.SortMemberPath == "EmpId")
                {
                    emp = f.FilterText;
                }
                else
                {
                    name = f.FilterText;
                }

                //     var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
            }

            _viewModel.SelectedMainRow =
                _viewModel.TransactionHeader.SelectedMainRows.FirstOrDefault(
                    x =>
                        (x.EmpId.ToLowerInvariant().StartsWith(emp.ToLowerInvariant()) || emp == "") &&
                        (x.Name.ToLowerInvariant().StartsWith(name.ToLowerInvariant()) || name == ""));

            //switch (f.SelectedFilterOperation.FilterOption)
            //    {
            //        case Enums.FilterOperation.EndsWith:
            //             break;

            //        //case Enums.FilterOperation.StartsWith:
            //        //    myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
            //        //    break;

            //        //case Enums.FilterOperation.Contains:
            //        //    myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
            //        //    break;
            //    }

            //    _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

            //}
            //_viewModel.GetMaindata();
        }

        private void ComboBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void ComboBox_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void BtnDeleteAttFileOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var row = img.DataContext as TblAttendanceFileViewModel;
            _viewModel.DeleteAttFile(row);
        }

        private void AttendanceFileGrid_OnOnFilter(object sender, FilterEvent e)
        {
            var emp = "";
            var name = "";
            foreach (var f in e.FiltersPredicate)
            {
                if (f.Column.SortMemberPath == "Emplid")
                {
                    emp = f.FilterText;
                }
                else
                {
                    name = f.FilterText;
                }

                //     var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
            }
            _viewModel.SelectedAttRule =
                _viewModel.TransactionHeader.SelectedAttendanceFile.FirstOrDefault(
                    x =>
                        (x.Emplid.ToLowerInvariant().StartsWith(emp.ToLowerInvariant()) || emp == "") &&
                        (x.Name.ToLowerInvariant().StartsWith(name.ToLowerInvariant()) || name == ""));
        }

        private void VacaionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = TabControlAtt.SelectedItem as TabItem;
            if (item.Name == "Vacation")
            {
                if (_viewModel.SelectedMainRow != null) _viewModel.GetEmpVacation();

                //    var temp = new ObservableCollection<CSPEMPLOYEEVACATION>();

                //    if (_viewModel.SelectedMainRowCopy != _viewModel.SelectedMainRow)
                //    {
                //        if (_viewModel.SelectedMainRowCopy == null)
                //        {
                //            _viewModel.SelectedMainRowCopy = _viewModel.SelectedMainRow;
                //        }

                //        if (_viewModel.SelectedMainRowCopy.RemainingVacations != null)
                //        {
                //            GenericMapper.InjectFromObCollection(temp, _viewModel.SelectedMainRowCopy.RemainingVacations);

                //            foreach (var oldRow in _viewModel.SelectedMainRowCopy.SelectedCopyVacations.OrderBy(x => x.FromDate))
                //            {
                //                var newrow = _viewModel.SelectedMainRowCopy.SelectedVacations.FirstOrDefault(x => x.FromDate == oldRow.FromDate);
                //                if (oldRow.CSPVACATIONID == null || newrow.CSPVACATIONID != null && oldRow.CSPVACATIONID.ToLower() != newrow.CSPVACATIONID.ToLower())
                //                {
                //                    var oldRemVac = temp.SingleOrDefault(
                //                        x => x.EMPLID == oldRow.Emplid && x.CSPVACATIONID.ToLower() == oldRow.CSPVACATIONID.ToLower());
                //                    if (oldRemVac != null) oldRemVac.REMAINING++;

                //                    var newRemVac = temp.SingleOrDefault(
                //                        x => x.EMPLID == newrow.Emplid && x.CSPVACATIONID.ToLower() == newrow.CSPVACATIONID.ToLower());
                //                    if (newRemVac != null) newRemVac.REMAINING--;
                //                    if (newRemVac == null && oldRemVac == null)
                //                    {
                //                        newrow.CSPVACATIONID = "";
                //                    }
                //                }
                //            }

                //            if (_viewModel.SelectedMainRowCopy != null && _viewModel.SelectedMainRowCopy.RemainingVacations != null)
                //            {
                //                var vacationStr = _viewModel.SelectedMainRowCopy.CcVacation.Select(x => x.CSPVACATIONID);
                //                var vacations = temp.Where(
                //                    x => x.EMPLID == _viewModel.SelectedMainRowCopy.EmpId && vacationStr.Contains(x.CSPVACATIONID)).ToList();
                //                foreach (var row in vacations)
                //                {
                //                    if (row.REMAINING < 0)
                //                    {
                //                        MessageBox.Show("Vacations Exceed Limit");
                //                        VacaionGrid.SelectedItem = _viewModel.SelectedMainRowCopy;
                //                        return;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    _viewModel.SelectedMainRowCopy = _viewModel.SelectedMainRow;
            }
        }

        private void ExcuseGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = TabControlAtt.SelectedItem as TabItem;
            if (item.Name == "Excuse")
            {



                if (_viewModel.SelectedMainRowCopy == null)
                {
                    _viewModel.SelectedMainRowCopy = _viewModel.SelectedMainRow;
                }

                //if (_viewModel.SelectedMainRowCopy != _viewModel.SelectedMainRow)
                //{
                //    foreach (var variable in _viewModel.SelectedMainRowCopy.ExcuseCount)
                //    {
                //        var excusecount = variable.Value;
                //        var count = _viewModel.SelectedMainRowCopy.SelectedExcuses.Count(
                //            x =>
                //                variable.Key == x.TransDate.Value.Month && x.CSPEXCUSEID != null && x.Iserial == 0 &&
                //                !string.IsNullOrWhiteSpace(x.CSPEXCUSEID));
                //        excusecount = excusecount + count;

                //        if (excusecount >
                //            _viewModel.ExcuseRuleList.FirstOrDefault(
                //                x => x.PeriodId == _viewModel.SelectedMainRowCopy.PeriodId).CounterPerPeriodLine)
                //        {
                //            MessageBox.Show("Excuses exceed Limit");
                //            ExcuseGrid.SelectedItem = _viewModel.SelectedMainRowCopy;
                //            return;
                //        }
                //    }
                //}

               _viewModel.SelectedMainRowCopy = _viewModel.SelectedMainRow;
            //    if (_viewModel.SelectedMainRowCopy != null)
            //        foreach (
            //            var variable in
            //                _viewModel.SelectedMainRowCopy.SelectedExcuses.Where(x => x.TransDate != null)
            //                    .Select(x => x.TransDate.Value.Month)
            //                    .Distinct())
            //        {
            //            _viewModel.SelectedMainRowCopy.ExcuseCount = new Dictionary<decimal, int>();
            //            _viewModel.GetExcuseCount(variable);
            //        }
            }
        }

        private void StackPanelExcuses_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var stack = sender as StackPanel;
            if (stack != null)
            {
                var excuse = stack.DataContext as TblExcuseViewModel;
                AttService.AttServiceClient AttService1 = new AttService.AttServiceClient();
                GetCalendarTime(excuse.Emplid, excuse.TransDate, AttService1);

                var tempstack = stack.Children.LastOrDefault() as StackPanel;
                var fromPicker = tempstack.FindName("FromPicker") as TimePicker;
                var toPicker = tempstack.FindName("ToPicker") as TimePicker;


                if (string.IsNullOrEmpty(excuse.CSPEXCUSEID))
                {

                    fromPicker.IsEnabled = false;

                    toPicker.IsEnabled = false;
                }
                else {

                    fromPicker.IsEnabled = true;

                    toPicker.IsEnabled = true;
                }

                AttService1.GetCalendarTimeCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        Fromtime = sv.Result.FROMTIME;
                        Totime = sv.Result.TOTIME;
                    }
                    else
                    {
                        Fromtime = null;
                        Totime = null;

                        fromPicker.IsEnabled = toPicker.IsEnabled = false;
                    }

                    // DateTime from = new DateTime();
                    // from= DateTime.fr

                    if (Fromtime != null)
                    {
                        var fromSpan = TimeSpan.FromSeconds((double)Fromtime);
                        var toSpan = TimeSpan.FromSeconds((double)Totime);
                        var fromDatetime = DateTime.Today.Add(fromSpan);
                        var toDatetime = DateTime.Today.Add(toSpan);

                        fromPicker.Minimum = toPicker.Minimum = fromDatetime;
                        fromPicker.Maximum = toPicker.Maximum = toDatetime;
                    }
                };

                /*****************************/
                /*Apply Excuse Max Limit*/
                if (excuse.CSPEXCUSEID != "Deduction" && excuse.CSPEXCUSEID != "ChristianFeast")
                { 
                    AttService1.GetExcuseMaxTimeLimitAsync(excuse.Emplid);
                }
                AttService1.GetExcuseMaxTimeLimitCompleted += (s, sv) =>
                {
                    if (sv.Result > 0)
                    {
                        if (excuse.FromTime != null)
                        {
                            Totime = excuse.FromTime + (int)(sv.Result * 60);
                            excuse.ToTime = Totime;
                            toPicker.IsEnabled = false;
                        }
                    }
                };

                /*****************************/

                //AttService1
                if (excuse != null)
                {
                    _viewModel.selectedExcuse = excuse;
                    TxtExcDesc.IsEnabled = true;
                }
            }
        }

        private void StackPanelExcuses_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Excuse);
        }

        private void StackPanelShift_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var stack = sender as StackPanel;

            if (stack != null)
            {
                var excuse = stack.DataContext as TblEmployeeshiftViewModel;

                if (excuse != null)
                {
                    _viewModel.selectedEmployeeShift = excuse;
                }
            }
        }

        private void StackPanelShift_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var row =
            _viewModel.TransactionHeader.SelectedMainRows.FirstOrDefault(
                x => x.EmpId == _viewModel.selectedEmployeeShift.EmpId);
            if (row.MaxdayOffPerWeek != null)
            {
                if (_viewModel.selectedEmployeeShift.TblEmployeeShiftLookup == 6)
                {
                    if (row.SelectedMainRows.Count(x => x.TblEmployeeShiftLookup == 6) > row.MaxdayOffPerWeek)
                    {
                        MessageBox.Show("You Cannot Have More Than Your Limit In DayOff Per Week ");
                        _viewModel.selectedEmployeeShift.TblEmployeeShiftLookup = null;

                        return;
                    }
                }
            }
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Shift);
        }

        private void StackPanelVacation_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var stack = sender as StackPanel;

            if (stack != null)
            {
                var vacation = stack.DataContext as TblVacationViewModel;

                if (vacation != null)
                {
                    _viewModel.selectedVacation = vacation;
                    TxtVacationDesc.IsEnabled = true;
                }
            }
        }

        private void StackPanelVacation_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Vacation);
        }

        public void GetCalendarTime(string emplid, DateTime? fromDate, AttService.AttServiceClient client)
        {
            if (fromDate != null)
            {
                client.GetCalendarTimeAsync(emplid, (DateTime)fromDate);
            }
        }

        private void StackPanelMission_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var stack = sender as StackPanel;

            if (stack != null)
            {
                var mission = stack.DataContext as TblMissionViewModel;
                AttService.AttServiceClient AttService1 = new AttService.AttServiceClient();
                GetCalendarTime(mission.Emplid, mission.FromDate, AttService1);

                AttService1.GetCalendarTimeCompleted += (s, sv) =>
                {
                    var tempstack = stack.Children.LastOrDefault() as StackPanel;
                    var fromPicker = tempstack.FindName("FromPicker") as TimePicker;
                    var toPicker = tempstack.FindName("ToPicker") as TimePicker;
                    if (sv.Result != null)
                    {
                        Fromtime = sv.Result.FROMTIME;
                        Totime = sv.Result.TOTIME;
                    }
                    else
                    {
                        //Fromtime = null;
                        //Totime = null;
                        if (fromPicker != null) if (toPicker != null) fromPicker.IsEnabled = toPicker.IsEnabled = true;
                    }

                    // DateTime from = new DateTime();
                    // from= DateTime.fr

                    if (Fromtime != null)
                    {
                        var fromSpan = TimeSpan.FromSeconds((double)Fromtime);
                        var toSpan = TimeSpan.FromSeconds((double)Totime);
                        var fromDatetime = DateTime.Today.Add(fromSpan);
                        var toDatetime = DateTime.Today.Add(toSpan);

                        if (fromPicker != null)
                        {
                            if (toPicker != null)
                            {
                                fromPicker.Minimum = toPicker.Minimum = fromDatetime;
                                fromPicker.Maximum = toPicker.Maximum = toDatetime;
                            }
                        }
                    }
                };
                if (mission != null)
                {
                    _viewModel.selectedMission = mission;
                    TxtMissionDesc.IsEnabled = true;
                }
            }
        }

        private void StackPanelMission_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.Mission);
        }

        private void AttendanceFileGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "DisableManualAttendance") != null)
            {
                _viewModel.SelectedAttRule.FromTime = null;
                _viewModel.SelectedAttRule.ToTime = null;
                MessageBox.Show("You don't have permission to edit ");
                
            }
            else
            {
                _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.AttFile);
            }
        }

        private void BtnEmpVacation_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GetEmpVacation();
            var child = new EmpVacationChild(DataContext as EmployeeShiftViewModel);
            child.Show();
        }

        private void BtnEmpVacationType_OnClick (object sender, RoutedEventArgs e)
        {
            //_viewModel.GetEmpVacation();
            //EmployeeVactionTypesPopUp.IsOpen = true;


            var btn = sender as Button;
            if (btn != null)
            {
                //_viewModel.GetEmpVacation();
                AttServiceClient _client = new AttServiceClient();

                if (_viewModel.SelectedMainRow != null)
                    _client.GetEmpVacationAsync(_viewModel.SelectedMainRow.EmpId);
                _client.GetEmpVacationCompleted += (s, sv) =>
                {
                    var vacationsToAdd = _viewModel.CcVacation.Select(x => x.CSPVACATIONID.ToLower());
                    var row = sv.Result.Where(x => vacationsToAdd.Contains(x.CSPVACATIONID.ToLower()));
                    if (row != null)
                        if (_viewModel.SelectedMainRow != null)
                            _viewModel.SelectedMainRow.RemainingVacations = new ObservableCollection<TblVacationDetail>(row);

                        var SelectedVacRow = btn.DataContext as TblVacationViewModel;
                        var child = new EmpVacationDetails(DataContext as EmployeeShiftViewModel, SelectedVacRow);
                        child.Show();
             
                
                };
               
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "Vacation";
            switch (_viewModel.SelectedTab)
            {
                case "Shift":
                    break;

                case "Vacation":
                    reportName = "Vacation";

                    break;

                case "Excuse":
                    reportName = "Excuse";

                    break;

                case "Mission":
                    reportName = "Mission";

                    break;

                case "Attendance":
                    if (LoggedUserInfo.Company.Ename == "HQ")
                    {
                        reportName = "Absence";
                    }
                    else
                    {
                        reportName = "OutLetAttendanceManagerReview";
                    }

                    break;
            }
            var para = new ObservableCollection<string> { LoggedUserInfo.Code.ToString(CultureInfo.InvariantCulture) };
            if (reportName == "OutLetAttendanceManagerReview")
            {
                para = new ObservableCollection<string>
                {
                    _viewModel.TransactionHeader.StorePerRow.code,
                    LoggedUserInfo.Company.Code.ToString(CultureInfo.InvariantCulture)
                };
            }

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void BtnAddExcuse_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var child = new ExcusesChild(_viewModel);
            child.Show();
        }

        private void BtnAddMission_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var child = new MissionsChild(_viewModel);
            child.Show();
        }

        private void BtnEmpVacationApprovalDetails_Onclick(object sender, RoutedEventArgs e)
        {
            switch (_viewModel.SelectedTab)
            {
                case "Shift":
                    break;

                case "Vacation":
                    if (_viewModel.VacationStatusSelfVisibility == Visibility.Visible || _viewModel.SelectedMainRow.EmpId != LoggedUserInfo.Code)
                    {
                        var childv = new VacationsChild(_viewModel);
                        childv.Show();
                    }
                    break;

                case "Excuse":
                    if (_viewModel.ExcuseStatusSelfVisibility == Visibility.Visible || _viewModel.SelectedMainRow.EmpId != LoggedUserInfo.Code)
                    {
                        var childe = new ExcuseChild(_viewModel);
                        childe.Show();
                    }
                    break;

                case "Mission":
                    if (_viewModel.MissionStatusSelfVisibility == Visibility.Visible || _viewModel.SelectedMainRow.EmpId != LoggedUserInfo.Code)
                    {
                        var childm = new MissionsChild(_viewModel);
                        childm.Show();
                    }
                    break;
            }
        }

        private void BtnApplyAttendance_Onclick(object sender, RoutedEventArgs e)
        {
            foreach (var variable in _viewModel.TransactionHeader.SelectedAttendanceFile.Where(x => x.Transportation == _viewModel.Transportation && x.TransDate == _viewModel.DayDate && (x.OrginalFromTime != null || x.OrginalInTime != null)))
            {
                if (_viewModel.ToTime != 0)
                {
                    variable.ToTime = _viewModel.ToTime;
                }

                if (_viewModel.FromTime != 0)
                {
                    variable.FromTime = _viewModel.FromTime;
                }
                var save = variable.Iserial == 0;

                var saveRow = new AttService.TblAttendanceFile();

                saveRow.InjectFrom(variable);
                AttService.UpdateAndInsertTblAttendanceFileAsync(saveRow, save, _viewModel.TransactionHeader.SelectedAttendanceFile.IndexOf(variable), LoggedUserInfo.Iserial);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var row = _viewModel.ExcuseRuleList.FirstOrDefault(
                    x =>
                        x.PeriodId == _viewModel.SelectedMainRow.PeriodId &&
                        x.ExcuseId == _viewModel.selectedExcuse.CSPEXCUSEID);
                if (_viewModel.ExcuseRuleList.FirstOrDefault(
                                         x => x.PeriodId == _viewModel.SelectedMainRow.PeriodId && x.ExcuseId == _viewModel.selectedExcuse.CSPEXCUSEID).DefaultFromTime != null)
                {
                    _viewModel.selectedExcuse.FromTime = row.DefaultFromTime;
                    _viewModel.selectedExcuse.ToTime = row.DefaultToTime;
                }
            }
            catch (Exception)
            {
            }

        }

        private void BtnFullDay_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var row = btn.DataContext as TblAttendanceFileViewModel;
            if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "EditAttendanceForEmptyDay") == null)
            {
                if (row.OrginalFromTime != null && row.OrginalFromTime != 0 && row.OrginalInTime != null && row.OrginalInTime != 0)
                {
                }
                else
                {
                    return;
                }
            }
            AttService.AttServiceClient AttService1 = new AttService.AttServiceClient();
            GetCalendarTime(row.Emplid, row.TransDate, AttService1);
            AttService1.GetCalendarTimeCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "DisableManualAttendance") != null)
                    {
                        _viewModel.SelectedAttRule.FromTime = null;
                        _viewModel.SelectedAttRule.ToTime = null;
                        MessageBox.Show("You don't have permission to edit ");

                    }
                    else
                    {
                        row.FromTime = sv.Result.FROMTIME;
                        row.ToTime = sv.Result.TOTIME;
                        _viewModel.SaveRow(EmployeeShiftViewModel.AttSaveTypes.AttFile);
                    }
                }
                else
                {
                    row.FromTime = null;
                    row.ToTime = null;

                }

            };
        }

        private void AttendanceFileGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var grid = sender as DataGrid;
            var selectedRow = grid.SelectedItem as TblAttendanceFileViewModel;
            if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "EditAttendanceForEmptyDay") == null)
            {
                if ((selectedRow.OrginalFromTime != null && selectedRow.OrginalFromTime != 0) || (selectedRow.OrginalInTime != null && selectedRow.OrginalInTime != 0))
                {
                }
                else
                {
                    e.Cancel = true;
                }
            }
        } 
        private void CmbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetShiftLookup();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void TimePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            //var stack = sender as TimePicker;
            //if (stack != null)
            //{
                //if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "DisableManualAttendance") != null)
                //{
                //    stack.IsEnabled = false;
                //}
                //else
                //{
                //    stack.IsEnabled = true;
                //}
           //}
        }
    }
}