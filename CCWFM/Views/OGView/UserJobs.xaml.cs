using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;
using Syncfusion.Windows.Controls;

namespace CCWFM.Views.OGView
{
    public partial class UserJobs
    {        
        private readonly UserJobsViewModel _viewModel = new UserJobsViewModel();

        private ImportUserChildWindow _importChild;

        public UserJobs()
        {
            InitializeComponent();
            DataContext = _viewModel;
            MainGrid.DataContext = _viewModel;
        }

        private void ImportFromAx_Click(object sender, RoutedEventArgs e)
        {
            _importChild = new ImportUserChildWindow();
            _importChild.Show();
            _importChild.DgAxUsers.ItemsSource = _viewModel.AxUsers;
            _importChild.SubmitClicked += Import_SubmitClicked;
        }

        private void Import_SubmitClicked(object sender, EventArgs e)
        {
            var axUser = _importChild.DgAxUsers.SelectedItem as CCWFM.AuthService.User;
            _viewModel.ApplyUser(axUser);
        }

        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(false);
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.Filter = _viewModel.Filter + " and ";
                }

                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetMaindata();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewMainRow(true);
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblUserViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
            //_viewModel.SaveMainRow();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblUserViewModel);
            }
            _viewModel.DeleteMainRow();
        }

        private void BtnImportEmployeeFromAx_OnClick(object sender, RoutedEventArgs e)
        {
            var childWindow = new EmployeeChildWindow(_viewModel);
            childWindow.Show();
        }

        private void BtnAllowedStores_OnClick(object sender, RoutedEventArgs e)
        {
            var childWindow = new SearchForStore(_viewModel);
            childWindow.Show();
        }

        private void BtnCheckList_OnClick(object sender, RoutedEventArgs e)
        {
            //var childWindow = new UserCheckListChildWindow(_viewModel.SelectedMainRow.Iserial);
            //childWindow.Show();
        }
    }
}