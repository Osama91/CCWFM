using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using Lite.ExcelLibrary.SpreadSheet;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class GenratePurchaseSalesOrderChildWindow
    {
        private readonly GeneratePurchaseViewModel _viewModel;

        public GenratePurchaseSalesOrderChildWindow(GeneratePurchaseViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateSalesOrdersList.Remove(_viewModel.SelectedSalesOrder);
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewSalesOrder(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.GenerateSalesOrdersList.IndexOf(_viewModel.SelectedSalesOrder));
                if (currentRowIndex == (_viewModel.GenerateSalesOrdersList.Count - 1))
                {
                    _viewModel.AddNewSalesOrder(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.GenerateSalesOrdersList.Remove(_viewModel.SelectedSalesOrder);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.ExportExcel("Style");
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateSalesOrdersList.Clear();


            var oFile = new OpenFileDialog { Filter = "Excel (*.xls)|*.xls" };

            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();

                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];

                var description = 0;
                var iserial = 0;
                for (var j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "salesorder":
                            description = j;
                            break;
                        case "salesorderiserial":
                            iserial = j;
                            break;
                    }
                }
                for (var i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex; i++)
                {
                    if (sheet.Cells[i, iserial].StringValue.ToUpper().Trim()!="")
                    {
                        var newemp = new TblGeneratePurchaseSalesOrdersViewModel
                        {
                            SalesOrderPerRow =
                                new TblSalesOrder
                                {
                                    SalesOrderCode = sheet.Cells[i, description].StringValue.ToUpper().Trim(),
                                    Iserial = Convert.ToInt32(sheet.Cells[i, iserial].StringValue.ToUpper().Trim())
                                }
                        };

                        _viewModel.GenerateSalesOrdersList.Add(newemp); 
                    }
               
                }


            }
        }
    }
}