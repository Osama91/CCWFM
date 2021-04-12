using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class AdvanceSampleRequestChildWindow
    {
        private readonly StyleHeaderViewModel _viewModel;

        public AdvanceSampleRequestChildWindow(StyleHeaderViewModel style)
        {
            InitializeComponent();
            DataContext = style;
            _viewModel = style;
       //     _RequestforSampleViewModel = requestForSampleViewModel;
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:

                        _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
                        //_viewModel.SelectedDetailRow.TblSupplier = (int)_RequestforSampleViewModel.SelectedMainRow.TblSupplier;
                        //_viewModel.SelectedDetailRow.SupplierPerRow = _RequestforSampleViewModel.SelectedMainRow.SupplierPerRow;
                        //_viewModel.SelectedDetailRow.TblRequestForSample = _RequestforSampleViewModel.SelectedMainRow.Iserial;
                        //_viewModel.SelectedDetailRow.RequestforSamplePerRow = new TblRequestForSample().InjectFrom(_RequestforSampleViewModel.SelectedMainRow) as TblRequestForSample;
                        break;

                    case 1:
                        _viewModel.AddNewSalesOrderOperation(SalesOrderOperationGrid.SelectedIndex != -1);
                        break;
                    case 2:
                        _viewModel.AddNewSubEventRow(SubEventGrid.SelectedIndex != -1);
                        break;
                    case 3:
                        _viewModel.AddBom(BomGrid.SelectedIndex != -1);
                        break;
                }
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:
                        DetailGrid.CommitEdit();
                     //   _viewModel.SaveDetailRow();
                        break;

                    case 1:
                        if (!_viewModel.Loading)
                        {
                            _viewModel.SaveSalesOrderOperationsList();
                        }
                   //     _viewModel.SaveSalesOrderOperations();
                        break;
                    case 2:
                        SubEventGrid.CommitEdit();
                        break;
                    case 3:
                        _viewModel.SaveBom();
                        break;
                }
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:
                        //_viewModel.GetDetaildataFull(DetailGrid);
                        DetailGrid.ExportExcel("Po");
                        break;

                    case 1:
                        SalesOrderOperationGrid.ExportExcel("Operations Colors");
                        break;
                    case 2:
                        SubEventGrid.ExportExcel("Sample Event");
                        break;
                    case 3:
                        BomGrid.ExportExcel("Bom");
                        break;
                }
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Report();
        }

        private void TabStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:
                        _viewModel.GetSalesOrderLookups();
                        break;

                    case 1:
                        _viewModel.GetSalesOrderOperations();
                        break;
                    case 2:
                        _viewModel.GetRequestForSampleEventData();
                        break;
                    case 3:
                        _viewModel.GetSalesOrderBom();
                        break;
                }
            }
        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;

            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();
     
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetDetailData();
            }
        }

        private void DetailGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                    //_viewModel.SelectedDetailRow.TblSupplier = (int)_RequestforSampleViewModel.SelectedMainRow.TblSupplier;
                    //_viewModel.SelectedDetailRow.SupplierPerRow = _RequestforSampleViewModel.SelectedMainRow.SupplierPerRow;
                    //_viewModel.SelectedDetailRow.TblRequestForSample = _RequestforSampleViewModel.SelectedMainRow.Iserial;
                    //_viewModel.SelectedDetailRow.RequestforSamplePerRow = new TblRequestForSample().InjectFrom(_RequestforSampleViewModel.SelectedMainRow) as TblRequestForSample;
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblSalesOrderViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }

        private void BtnColors_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SalesOrderColor(_viewModel, SalesOrderType.AdvancedSampleRequest);
            child.Show();
        }

        private void BtnPrintSalesOrder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ReportSalesOrder();
        }

        private void SalesOrderOperationGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.SalesOrderOperationList.IndexOf(_viewModel.SelectedSalesOrderOperation));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.SalesOrderOperationList.Count - 1))
                {
                    _viewModel.AddNewSalesOrderOperation(true);
                }
                _viewModel.AddNewSalesOrderOperation(true);
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedSalesOrderOperations.Clear();
                foreach (var row in SalesOrderOperationGrid.SelectedItems)
                {
                    _viewModel.SelectedSalesOrderOperations.Add(row as TblSalesOrderOperationViewModel);
                }

                _viewModel.DeleteSalesOrderOperations();
            }
        }

        private void SalesOrderOperationGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSalesOrderOperations();
        }

        private void BomGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.BomList.IndexOf(_viewModel.SelectedBomRow));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.BomList.Count - 1))
                {
                    _viewModel.AddBom(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedBomRows.Clear();
                foreach (var row in BomGrid.SelectedItems)
                {
                    _viewModel.SelectedBomRows.Add(row as BomViewModel);
                }

                _viewModel.DeleteBom();
            }
        }

        private void BomGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (var row in _viewModel.SelectedBomRow.BomStyleColors)
            //{
            //    MessageBox.Show(row.FabricColor.ToString());
            //}
        }

        private void BtnGenerateFromStandard_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));

            myPopup.HorizontalOffset = p.X;
            myPopup.VerticalOffset = p.Y;

            myPopup.IsOpen = true;

            _viewModel.GetStandardBom();
        }

        private void GetBomBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StandardBomGrid.SelectedItem != null)
            {
                var row = StandardBomGrid.SelectedItem as TblStandardBomHeaderViewModel;
                _viewModel.GenerateBomFromStandard(row);
                myPopup.IsOpen = false;
            }
        }

        private void StandardBomGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.StandardBomList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.StandardBomList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetStandardBom();
            }
        }

        private void StandardBomGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;
            _viewModel.StandardBomList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetStandardBom();

      
        }

        private void BtnGenerateSalesOrder_OnClick(object sender, RoutedEventArgs e)
        {
      //      _viewModel.SelectedPoToLink = _viewModel.SelectedDetailRow;
          _viewModel.GenerateSalesOrderFromSample();
        }

        private void Bomcolor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedBomRow.ColorChanged();
        }

        private void BtnAddnewOperationDetail_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedSalesOrderOperation.TblSalesOrderOperationDetailList.Add(new TblSalesOrderOperationDetail());
        }

        private void BtnDeleteSalesOrderOperationDetail_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblSalesOrderOperationDetail;
                _viewModel.SalesOperationDetail(row);
            }
        }

        private void BomGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveBomRow();
        }

        private void btnClearColors_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            if (btn != null)
            {
                var row = btn.DataContext as BomStyleColorViewModel;
                if (row != null)
                {
                    row.TblColor = null;

                    row.FabricColor = null;
                    row.DummyColor = null;
                    row.DyedColor = null;
                    row.TblColor2 = null;
                }
            }
        }

        private void SubEventGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.SubEventList.IndexOf(_viewModel.SelectedRequestForSampleEvent));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.SubEventList.Count - 1))
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

        private void SubEventGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
           // if (_viewModel.RequestForSampleOpen != true)
          //  {
                if (_viewModel.SelectedDetailRow.SubEventList.Any(x => x.Iserial != 0))
                {
                    var lastRow = _viewModel.SelectedDetailRow.SubEventList.OrderBy(x => x.Iserial).LastOrDefault();
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
             //   }
            }
        }


        private void SubEventGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveSubEventRow();
        }

        private void BtnSendMailSample_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var link = sender as Image;
            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }
            if (link != null)
            {
                var row = link.DataContext as TblRequestForSampleEventViewModel;
                var para = new ObservableCollection<string> { row.TblSalesOrder.ToString(CultureInfo.InvariantCulture) };
                string body = "Status :" + row.RequestForSampleStatusPerRow.Ename;
                body = body + "Made by :" + row.UserPerRow.Ename + "<br>";
                body = body + "Requirements :" + row.Notes + "<br>";
                body = body + "Start Date :" + row.RequestDate + "<br>";
                body = body + "Finish Date :" + row.DeliveryDate;
                string subject = _viewModel.SelectedDetailRow.SalesOrderCode + " Sample Request";
                _viewModel.SendMail(para, subject, body);
            }
        }
    }
}