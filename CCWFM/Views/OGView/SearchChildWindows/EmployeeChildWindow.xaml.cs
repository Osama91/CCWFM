using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.AttViewModel;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class EmployeeChildWindow
    {
        public enum FormsEnum
        {
            User,
            Manager,
            Operator
        }

        public FormsEnum formsEnum = new FormsEnum();

        public EmployeeChildWindow(UserJobsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.GetDetailData();
            formsEnum = FormsEnum.User;
        }

        public EmployeeChildWindow(EmployeeBehalfViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.GetDetailData();
        }

        public EmployeeChildWindow(EmployeeShiftViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.GetDetailData();
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (formsEnum == FormsEnum.User)
            {
                var viewModel = DataContext as UserJobsViewModel;
                viewModel.SelectedDetailRow = MainGrid.SelectedItem as AuthService.EmployeesView;
                viewModel.ApplyUser();
            }
            else if (formsEnum == FormsEnum.Manager)
            {
                var viewModel = DataContext as EmployeeBehalfViewModel;
                var manager = MainGrid.SelectedItem as EmployeesView;

                viewModel.SelectedMainRow.ManagerPerRow = manager;
                viewModel.SelectedMainRow.ManagerId = manager.Emplid;
            }
            else if (formsEnum == FormsEnum.Operator)
            {
                var viewModel = DataContext as EmployeeBehalfViewModel;
                var manager = MainGrid.SelectedItem as EmployeesView;

                viewModel.SelectedMainRow.OperatorPerRow = manager;
                viewModel.SelectedMainRow.AttOperatorId = manager.Emplid;
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (formsEnum == FormsEnum.Manager || formsEnum == FormsEnum.Operator)
            {
                var viewModel = DataContext as EmployeeBehalfViewModel;

                if (viewModel.DetailList.Count < viewModel.PageSize)
                {
                    return;
                }
                if (viewModel.DetailList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.DetailList.Count != viewModel.DetailFullCount)
                {
                    viewModel.GetDetailData();
                }
            }
            else
            {
                var viewModel = DataContext as UserJobsViewModel;

                if (viewModel.DetailList.Count < viewModel.PageSize)
                {
                    return;
                }
                if (viewModel.DetailList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.DetailList.Count != viewModel.DetailFullCount)
                {
                    viewModel.GetDetailData();
                }
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            if (formsEnum == FormsEnum.User)
            {
                var viewModel = DataContext as UserJobsViewModel;
                viewModel.DetailList.Clear();
                var counter = 0;
                viewModel.DetailFilter = null;

                viewModel.DetailValuesObjects = new Dictionary<string, object>();

                foreach (var f in e.FiltersPredicate)
                {
                    var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
                    var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                    switch (f.SelectedFilterOperation.FilterOption)
                    {
                        case Enums.FilterOperation.EndsWith:
                            myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
                            break;

                        case Enums.FilterOperation.StartsWith:
                            myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                            break;

                        case Enums.FilterOperation.Contains:
                            myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                            break;
                    }

                    viewModel.DetailValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                    if (counter > 0)
                    {
                        viewModel.DetailFilter = viewModel.DetailFilter + " and ";
                    }

                    viewModel.DetailFilter = viewModel.DetailFilter + "it." + f.FilterColumnInfo.PropertyPath +
                                        f.SelectedFilterOperation.LinqUse + paramter;

                    counter++;
                }
                viewModel.GetDetailData();
            }
            else
            {
                var viewModel = DataContext as EmployeeBehalfViewModel;
                viewModel.DetailList.Clear();
                var counter = 0;
                viewModel.DetailFilter = null;

                viewModel.DetailValuesObjects = new Dictionary<string, object>();

                foreach (var f in e.FiltersPredicate)
                {
                    var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
                    var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                    switch (f.SelectedFilterOperation.FilterOption)
                    {
                        case Enums.FilterOperation.EndsWith:
                            myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
                            break;

                        case Enums.FilterOperation.StartsWith:
                            myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                            break;

                        case Enums.FilterOperation.Contains:
                            myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                            break;
                    }

                    viewModel.DetailValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                    if (counter > 0)
                    {
                        viewModel.DetailFilter = viewModel.DetailFilter + " and ";
                    }

                    viewModel.DetailFilter = viewModel.DetailFilter + "it." + f.FilterColumnInfo.PropertyPath +
                                        f.SelectedFilterOperation.LinqUse + paramter;

                    counter++;
                }
                viewModel.GetDetailData();
            }
        }
    }
}