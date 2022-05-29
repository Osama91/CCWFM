using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl
{
    public partial class RecInv
    {
        private RecInvViewModel _viewModel;

        public RecInv()
        {
            InitializeComponent();
            _viewModel = (RecInvViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
            

            //AccountControl
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            _viewModel = (RecInvViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            BtnEditOrder.IsChecked = false;

            _viewModel.MainRowList.Clear();
            _viewModel.SelectedMainRow = new TblRecInvHeaderViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            BtnDeleteOrder.IsEnabled = false;
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    break;

                case FormMode.Standby:

                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    ClearScreen();
                    break;

                case FormMode.Search:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnDeleteOrder.Visibility = Visibility.Visible;
                    BtnDeleteOrder.IsEnabled = true;

                    break;

                case FormMode.Update:

                    _viewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    break;

                case FormMode.Read:

                    _viewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;

                    break;
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "VendorInvoice";

            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "VendorInvoice"; }

            var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);

            //   BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteMainRow();

            ResetMode();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnAddNewOrder.IsChecked = false;
            BtnAddNewOrder.IsEnabled = true;

            BtnAddNewOrder.Visibility = Visibility.Visible;
        }

        private void btnInvoice_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Invoice();
      
        }
        
        

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new RecHeaderChildWindow(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RecieveHeaderList.Clear();
            _viewModel.RecieveHeaderChoosedList.Clear();
            _viewModel.GetRecieveHeaderListData();
            var child = new RecieveHeaderChildWindow(_viewModel);
            child.Show();
        }

        #endregion FormModesSettings

        private void BtnMisc_OnClick(object sender, RoutedEventArgs e)
        {
            new MarkupTransChildWindow(_viewModel, true).Show();
        }

        private void BtnMiscDetail_OnClick(object sender, RoutedEventArgs e)
        {
            new MarkupTransChildWindow(_viewModel, false).Show();
        }

        private void TxtCostStyleColor_LostFocus(object sender, RoutedEventArgs e)
        {
            var rowOld = StyleGrid.SelectedItem as TblRecInvMainDetailViewModel;
            if (rowOld != null)
            {
                var oldCost = rowOld.Cost;

                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding.UpdateSource();
                }

                var row = StyleColorGrid.SelectedItem as TblRecInvMainDetailViewModel;
                _viewModel.TotalCost = _viewModel.TotalCost - (oldCost * rowOld.Qty) + (row.Cost * rowOld.Qty);
                if (row != null)
                {
                    var newrow = new RecInvStyleColor
                    {
                        TblRecInvHeader = _viewModel.SelectedMainRow.Iserial,
                        Style = row.TBLITEMprice.Style,
                        Cost = row.Cost,
                        ColorCode = row.TBLITEMprice.TblColor1.Code,
                        Quantity = row.Qty,
                        Misc=row.Misc,
                    };

                    if (newrow.Cost != null && oldCost != newrow.Cost)
                    {
                        _viewModel.UpdateCostInRecInv(newrow);
                        var list = _viewModel.SelectedMainRow.DetailsList.Where(x => row != null && (x.TBLITEMprice.Style == row.TBLITEMprice.Style && x.TBLITEMprice.TblColor1.Code == row.TBLITEMprice.TblColor1.Code));

                        foreach (var tblRecInvMainDetailViewModel in list)
                        {
                            
                                tblRecInvMainDetailViewModel.Cost = row.Cost;
                            tblRecInvMainDetailViewModel.Misc = row.Misc;
                        }
                    }
                }
            }
        }

        private void TxtCostStyle_LostFocus(object sender, RoutedEventArgs e)
        {
            var rowOld = StyleGrid.SelectedItem as TblRecInvMainDetailViewModel;
            var oldCost = rowOld.Cost;

            var binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
            var row = StyleGrid.SelectedItem as TblRecInvMainDetailViewModel;
            _viewModel.TotalCost = _viewModel.TotalCost - (oldCost * rowOld.Qty) + (row.Cost * rowOld.Qty);
            if (row != null)
            {
                var newrow = new RecInvStyle
                {
                    TblRecInvHeader = _viewModel.SelectedMainRow.Iserial,
                    Style = row.TBLITEMprice.Style,
                    Cost = row.Cost,
                    Misc=row.Misc,
                    Quantity = row.Qty
                };

            //    if (newrow.Cost != null && oldCost != newrow.Cost)
             //   {
                    _viewModel.UpdateCostInRecInv(newrow);
                    var list = _viewModel.SelectedMainRow.DetailsList.Where(x => row != null && x.TBLITEMprice.Style == row.TBLITEMprice.Style);

                    foreach (var tblRecInvMainDetailViewModel in list)
                    {
                        if (row != null)
                        {
                            tblRecInvMainDetailViewModel.Cost = row.Cost;
                            tblRecInvMainDetailViewModel.Misc = row.Misc;
                        }
                    }
              //  }
            }
        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailsValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.SelectedMainRow.DetailsList.Count < _viewModel.DetailFullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetDetailData();
            }
        }

        private void DetailSubGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.SelectedMainRow.StyleDetailList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailSubFilter = filter;
            _viewModel.DetailSubValuesObjects = valueObjecttemp;
            _viewModel.GetRecInvStyle();
        }

        private void DetailSubGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.StyleDetailList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.StyleDetailList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetRecInvStyle();
            }
        }

        private void DetailSubSubGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.StyleColorDetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.StyleColorDetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetRecInvStyleColor();
            }
        }

        private void BtnPost_OnClick(object sender, RoutedEventArgs e)
        {

          
                _viewModel.Post();
          



            
        }

        private void MainGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveDetailRow();
        }



        private void TabRec_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabRec != null)
            {
                if (TabRec.SelectedItem == Color)
                {
                    _viewModel.SelectedMainRow.StyleColorDetailsList.Clear();
                    _viewModel.GetRecInvStyleColor();
                }
                else if (TabRec.SelectedItem == Style)
                {
                    _viewModel.SelectedMainRow.StyleDetailList.Clear();
                    _viewModel.GetRecInvStyle();
                }
            }
        }

        private void TxtCostStyle_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Focus();
            }
        }

    }
}