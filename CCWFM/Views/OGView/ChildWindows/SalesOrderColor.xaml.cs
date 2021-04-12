using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SalesOrderColor
    {
        public StyleHeaderViewModel ViewModel;
        private readonly SalesOrderType _salesOrderType;

        public SalesOrderColor(StyleHeaderViewModel styleViewModel, SalesOrderType salesOrderType)
        {
            InitializeComponent();
            _salesOrderType = salesOrderType;
            DataContext = styleViewModel;
            ViewModel = styleViewModel;
            if (_salesOrderType != SalesOrderType.SalesOrderPo)
            {
                SalesOrderColorsGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                RfqColorsGrid.Visibility = Visibility.Collapsed;
            }
            foreach (var row in SalesOrderColorsGrid.Columns.Where(x => x.DisplayIndex >= 0 && x.DisplayIndex < 9))
            {
                const int pageWidth = 650;
                row.Width = new DataGridLength(pageWidth / 9);
            }

            //if (salesOrderType == SalesOrderType.SalesOrderPo)
            //{
            //    foreach (var row in SalesOrderColorsGrid.Columns.Where(x => x.DisplayIndex > 3 && x.DisplayIndex < 5))
            //    {
            //        row.Visibility = Visibility.Collapsed;
            //    }
            //}

            ViewModel.SelectedDetailRow.SalesOrderColorList.Clear();
            if (ViewModel.SelectedDetailRow != null && ViewModel.SelectedDetailRow.IsPlannedOrder)
            {
                ViewModel.GetSeasonalMasterList();
            }

            ViewModel.GetSalesOrderColors();
        }

        private void SalesOrderColorsGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.SelectedDetailRow.SalesOrderColorList.IndexOf(ViewModel.SelectedSalesOrderColorRow));
                if (currentRowIndex == (ViewModel.SelectedDetailRow.SalesOrderColorList.Count - 1))
                {
                    ViewModel.AddNewSalesOrderColor(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                ViewModel.SelectedSalesOrderColorRows.Clear();

                if (_salesOrderType != SalesOrderType.SalesOrderPo)
                {
                    foreach (var row in SalesOrderColorsGrid.SelectedItems)
                    {
                        ViewModel.SelectedSalesOrderColorRows.Add(row as TblSalesOrderColorViewModel);
                    }
                }
                else
                {
                    foreach (var row in RfqColorsGrid.SelectedItems)
                    {
                        ViewModel.SelectedSalesOrderColorRows.Add(row as TblSalesOrderColorViewModel);
                    }
                }

                ViewModel.DeleteSalesOrderColor();
            }
        }

        private void SalesOrderColorsGridEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            //    ViewModel.SalesOrderColors();
        }

        private void CalcImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.CalcRatio();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }

        private void BtnPrintBarcode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                var row = img.DataContext as TblSalesOrderColorViewModel;
                ViewModel.PrintBarcode(row);
            }
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {

            //if (Debugger.IsAttached)
            //{
            //}
            //else if (ViewModel.SelectedDetailRow.Status == 1)
            //{
            //    MessageBox.Show("Cannot edit because this order is approved");
            //    return;
            //}

            if (_salesOrderType != SalesOrderType.SalesOrderPo)
            {
                if (ViewModel.SelectedDetailRow != null && ViewModel.SelectedDetailRow.IsPlannedOrder)
                {
                    ViewModel.GetSeasonalMasterList();
                }
                else
                {
                    ViewModel.AddNewSalesOrderColor(RfqColorsGrid.SelectedIndex != -1);
                }
            }
            else
            {
                if (ViewModel.SelectedDetailRow != null && ViewModel.SelectedDetailRow.IsPlannedOrder)
                {
                    ViewModel.GetSeasonalMasterList();
                }
                else
                {
                    ViewModel.AddNewSalesOrderColor(SalesOrderColorsGrid.SelectedIndex != -1);
                }
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            //if (Debugger.IsAttached)
            //{
            //}
            //else if (ViewModel.SelectedDetailRow.Status == 1)
            //{
            //    //MessageBox.Show("Cannot edit because this order is approved");
            //    //return;
            //}

            ViewModel.SaveMainList();
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_salesOrderType != SalesOrderType.SalesOrderPo)
            {
                RfqColorsGrid.ExportExcel("Order Colors");
            }
            else
            {
                SalesOrderColorsGrid.ExportExcel("Order Colors");
            }
        }

        private void BntProduction_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SalesOrderColorProduction(ViewModel, _salesOrderType);
            child.Show();
        }


        private void SalesOrderColorsGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (Debugger.IsAttached)
            //{
            //}
            //else if (ViewModel.SelectedDetailRow.Status == 1)
            //{
            //    e.Cancel = true;
            //}
        }

        private void ChangeCancelRequestCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool salesOrderColor = ViewModel.SelectedSalesOrderColorRow.RequestForCancelApprove;
            if (salesOrderColor)
            {
                 LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                _client.ApproveSalesOrderColorCancelRequestAsync(ViewModel.SelectedSalesOrderColorRow.Iserial);
                _client.ApproveSalesOrderColorCancelRequestCompleted += (s, sv) => {
                    ViewModel.SelectedSalesOrderColorRow.RequestForCancelApprove = false;
                };
            }
        }
    }
}