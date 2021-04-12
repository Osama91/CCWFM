using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class RequestForSample
    {
        private readonly RequestForSampleViewModel _viewModel;
        private readonly StyleHeaderViewModel _styleViewModel;

        public RequestForSample(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();
            _viewModel = new RequestForSampleViewModel(styleViewModel);
            DataContext = _viewModel;
            _styleViewModel = styleViewModel;
            _viewModel.DataGridName = "MainGrid";

            _viewModel.PremCompleted += (s, sv) =>
            {
                if (_viewModel.CustomePermissions.SingleOrDefault(x => x.Code == "RequestForSampleOpen") != null)
                {
                    _viewModel.RequestForSampleOpen = true;
                }
            };
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DataGridName == "DetailGrid")
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblRequestForSampleItemViewModel);
                }
                _viewModel.DeleteDetailRow();
            }
            else if (_viewModel.DataGridName == "SubDetailGrid")
            {
                _viewModel.SelectedSubDetailRows.Clear();
                foreach (var row in SubDetailGrid.SelectedItems)
                {
                    _viewModel.SelectedSubDetailRows.Add(row as TblRequestForSampleServiceViewModel);
                }
                _viewModel.DeleteSubDetailRow();
            }
            else if (_viewModel.DataGridName == "SubEventGrid")
            {
                _viewModel.SelectedRequestForSampleEvents.Clear();
                foreach (var row in SubEventGrid.SelectedItems)
                {
                    _viewModel.SelectedRequestForSampleEvents.Add(row as TblRequestForSampleEventViewModel);
                }
                _viewModel.DeleteRequestForSampleEventRow();
            }
            else
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblRequestForSampleViewModel);
                }
                _viewModel.DeleteMainRow();
            }
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DataGridName == "DetailGrid")
            {
                _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
            }
            else if (_viewModel.DataGridName == "SubDetailGrid")
            {
                _viewModel.AddNewSubDetailRow(SubDetailGrid.SelectedIndex != -1);
            }
            else if (_viewModel.DataGridName == "SubEventGrid")
            {
                _viewModel.AddNewSubEventRow(SubEventGrid.SelectedIndex != -1);
            }
            else
            {
                _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                _viewModel.SelectedMainRow.DetailsList.Clear();
                _viewModel.SelectedMainRow.SubDetailsList.Clear();
                _viewModel.SelectedMainRow.SubEventList.Clear();

                _viewModel.GetDetailData();
                _viewModel.GetSubDetailData();
                _viewModel.GetRequestForSampleEventData();
            }
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add((TblRequestForSampleViewModel)row);
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
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DataGridName == "DetailGrid")
            {
                DetailGrid.CommitEdit();
            }
            else if (_viewModel.DataGridName == "SubDetailGrid")
            {
                SubDetailGrid.CommitEdit();
            }
            else if (_viewModel.DataGridName == "SubEventGrid")
            {
                SubEventGrid.CommitEdit();
            }
            else
            {
                MainGrid.CommitEdit();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }

            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblRequestForSampleItemViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void SubEventGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.SubEventList.IndexOf(_viewModel.SelectedRequestForSampleEvent));
                if (currentRowIndex == (_viewModel.SelectedMainRow.SubEventList.Count - 1))
                {
                    _viewModel.AddNewSubEventRow(true);
                }
            }

            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedRequestForSampleEvents.Clear();
                foreach (var row in SubEventGrid.SelectedItems)
                {
                    _viewModel.SelectedRequestForSampleEvents.Add(row as TblRequestForSampleEventViewModel);
                }

                _viewModel.DeleteRequestForSampleEventRow();
            }
        }

        private void SubEventGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubEventRow();
        }

        private void SubDetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.SubDetailsList.IndexOf(_viewModel.SelectedSubDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.SubDetailsList.Count - 1))
                {
                    _viewModel.AddNewSubDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedSubDetailRows.Clear();
                foreach (var row in SubDetailGrid.SelectedItems)
                {
                    _viewModel.SelectedSubDetailRows.Add(row as TblRequestForSampleServiceViewModel);
                }

                _viewModel.DeleteSubDetailRow();
            }
        }

        private void SubDetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubDetailRow();
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        private void BtnShowImages_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;

            var childWindow = new RequestForSampleImagesChildWindow((TblRequestForSampleViewModel)btn.DataContext);
            childWindow.Show();
        }

        private void MainGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var datagrid = sender as DataGrid;
            _viewModel.DataGridName = datagrid.Name;
        }

        private void BtnPrintSample_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var link = sender as Image;

            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }
            var row = link.DataContext as TblRequestForSampleViewModel;
            var para = new ObservableCollection<string> { row.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport("RequestForSample", para);
        }

        private void BtnSendMailSample_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var link = sender as Image;
            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }
            var row = link.DataContext as TblRequestForSampleEventViewModel;
            var para = new ObservableCollection<string> { row.TblSalesOrder.ToString(CultureInfo.InvariantCulture) };
            string body = "Status :" + row.RequestForSampleStatusPerRow.Ename;
            body = body + "Made by :" + row.UserPerRow.Ename + "<br>";
            body = body + "Requirements :" + row.Notes + "<br>";
            body = body + "Start Date :" + row.RequestDate + "<br>";
            body = body + "Finish Date :" + row.DeliveryDate;
            string subject = _viewModel.SelectedMainRow.Code + " Sample Request";
            _viewModel.SendMail(para, subject, body);
        }

        private void SubEventGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.RequestForSampleOpen != true)
            {
                if (_viewModel.SelectedMainRow.SubEventList.Any(x => x.Iserial != 0))
                {
                    var lastRow = _viewModel.SelectedMainRow.SubEventList.OrderBy(x => x.Iserial).LastOrDefault();
                    if (sender == SubEventGrid)
                    {
                        if (lastRow != _viewModel.SelectedRequestForSampleEvent && lastRow.RequestForSampleStatusPerRow.LastEvent == true)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (LoggedUserInfo.Iserial != _viewModel.SelectedRequestForSampleEvent.ApprovedBy)
                            {
                                e.Cancel = true;
                            }
                        }
                    }
                    else
                    {
                        if (lastRow.RequestForSampleStatusPerRow.LastEvent == true)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void BtnPrepareSample_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new AdvanceSampleRequestChildWindow(_styleViewModel);
            var list = new SortableCollectionView<TblSalesOrderViewModel>();
         


            foreach (var row in _styleViewModel.SelectedMainRow.TempDetailsList.Where(
                x => x.TblRequestForSample == _viewModel.SelectedMainRow.Iserial))
            {
                list.Add(row);
            }
            _styleViewModel.SelectedMainRow.DetailsList = list;

            child.Show();
        }
    }
}