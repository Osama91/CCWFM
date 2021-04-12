using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using System;

namespace CCWFM.Views.Gl
{
    public partial class GlCashTransaction
    {
        private readonly GlCashTransactionViewModel _viewModel;
        private readonly TblGlCashTypeSetting _setting;

        public GlCashTransaction(TblGlCashTypeSetting setting, ObservableCollection<Entity> entityList)
        {
            InitializeComponent();
            _setting = setting;
            _viewModel = new GlCashTransactionViewModel(setting, entityList);
            DataContext = _viewModel;

            _viewModel.PremCompleted += (s, sv) =>
            {
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "Approval") != null)
                {
                    BtnApprove.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnApprove.Visibility = Visibility.Collapsed;
                }

                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "DefaultBank") != null)
                {
                    ChaingSetupMethod.GetSettings("GlCashTransaction", _viewModel.Client);
                }

            };
            SwitchFormMode(FormMode.Add);

            //if (setting.Iserial == 1)
            //{
            DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "CostCenter").Visibility =
                   Visibility.Visible;
            //}

        }

        public FormMode FormMode { get; set; }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                    DetailGrid.BeginEdit();
                }
            }
            if (e.Key == Key.Delete && ModifierKeys.Shift == Keyboard.Modifiers)
            {
                if (!_viewModel.SelectedMainRow.Approved)
                {
                    _viewModel.SelectedDetailRows.Clear();
                    foreach (var row in DetailGrid.SelectedItems)
                    {
                        _viewModel.SelectedDetailRows.Add((TblGlCashTransactionDetailViewModel)row);
                    }
                    _viewModel.DeleteDetailRow();
                }
            }
        }

        private void ResetMode()
        {
            FormMode = FormMode.Standby;
            SwitchFormMode(FormMode);
        }

        private void ClearScreen()
        {
            _viewModel.SelectedMainRow = null;
            _viewModel.AddNewMainRow(false);
            BtnAddNewCard.IsChecked = false;
        }

        private void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.Visibility = Visibility.Collapsed;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnCancel.IsEnabled = true;
                    break;

                case FormMode.Standby:
                    BtnAddNewCard.IsEnabled = true;
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = true;
                    BtnSave.Visibility = Visibility.Collapsed;
                    BtnShowSearch.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtnAddNewCard.IsEnabled = false;
                    BtnAddNewCard.Visibility = Visibility.Collapsed;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = false;
                    break;

                case FormMode.Update:
                    BtnSave.Visibility = Visibility.Visible;
                    break;

                case FormMode.Read:
                    BtnSearch.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new GlCashTransactionChildWindow(_viewModel);
            childWindowSeach.Show();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.SelectedMainRow.Approved)
            {
                var r =
                    MessageBox
                        .Show(
                            "You are about to delete The Transaction permanently!!\nPlease note that this action cannot be undone!"
                            , "Delete", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK)
                {
                    _viewModel.DeleteMainRow();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnDeleteCard.Visibility = Visibility.Collapsed;
            BtnDeleteCard.IsEnabled = false;
            BtnShowSearch.IsChecked = false;
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            var para = new ObservableCollection<string>
            {
                _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture),
                LoggedUserInfo.Ip + LoggedUserInfo.Port,
                LoggedUserInfo.DatabasEname
            };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(_setting.Report, para);
        }

        private void BtnApprove_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow(true);
        }


        private void CostCenter_OnClick(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            var temp = btn.DataContext;

            _viewModel.SelectedDetailRow = temp as TblGlCashTransactionDetailViewModel;
            var valiationCollection = new List<ValidationResult>();

            var isvalid = Validator.TryValidateObject(_viewModel.SelectedDetailRow,
                new ValidationContext(_viewModel.SelectedDetailRow, null, null), valiationCollection, true);

            if (!isvalid)
            {
                MessageBox.Show("Data Is Not Valid");
            }
            else
            {
                var child = new GlCashCostCenterChildWindow(_viewModel);
                child.Show();
            }
        }
        private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.calcTotal();
        }

        private void DpTransDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var Dp = sender as DatePicker;
            if (_viewModel.PeriodLines == null)
            {
                return;
            }
            if (Dp.SelectedDate != null)
            {

                if (_viewModel.PeriodLines.Any())
                {
                    var selectedperiod = _viewModel.PeriodLines.FirstOrDefault(w => w.FromDate <= Dp.SelectedDate.Value && w.ToDate >= Dp.SelectedDate.Value);

                    if (selectedperiod != null)
                    {
                        if (selectedperiod.Locked)
                        {
                            MessageBox.Show("This Period is Locked ");
                            Dp.SelectedDate = DateTime.Now;
                        }
                    }
                }
            }
        }
    }
}