using System;
using System.Threading;
using System.Windows;
using System.Windows.Printing;
using CCWFM.ViewModel.Gl;
using System.Windows.Controls;
using Lite.ExcelLibrary.SpreadSheet;
using System.Collections.Generic;

using System.Linq;

namespace CCWFM.Views.PrintPreviews
{
    public partial class IncomeStatmentPrintPreview
    {
        private IncomeStatmentViewModel _ViewModel;

        public IncomeStatmentViewModel ViewModel
        {
            get { return _ViewModel; }
            set { _ViewModel = value; }
        }

        private PrintDocument pd;

        public IncomeStatmentPrintPreview(IncomeStatmentViewModel viewModel)
        {
            InitializeComponent();
           // PrintingDateTextBlock.Text = "Print Date : " + DateTime.Now.ToString("D");
            ViewModel = viewModel;
            grdRouteCardReportPreview.DataContext = ViewModel;
            pd = new PrintDocument();
            pd.PrintPage += pd_PrintPage;
            DataContext = ViewModel;
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = PrintArea;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            wTextBoxTitle.Visibility = Visibility.Collapsed;

            pd.Print("Income Statment Report " + DateTime.Now.ToShortDateString());

            pd.EndPrint += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    new Thread(() =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show("There was an error! Income Statment was not printed"));
                    }).Start();
                }
                else
                {
                    DialogResult = true;
                }
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }



        public void ExportExcel()
        {
            var sDialog = new SaveFileDialog { Filter = "Excel Files(*.xls)|*.xls" };
            if (sDialog.ShowDialog() == true)
            {
                // create an instance of excel workbook
                var workbook = new Workbook();
                // create a worksheet object
                var worksheet = new Worksheet("Income Statment");
                // write data in worksheet cells
                //  var a = grid.GetRows();
                //var list = grid.ItemsSource.Cast<object>().ToList();
                var data = _ViewModel.SelectedMainRow.DetailsList;

                var columns = new List<string>();
                columns.Add("Description");
                columns.Add("Dr");
                columns.Add("CR");

                worksheet.Cells[0, 0] = new Cell("Income Statment");

                if (_ViewModel.SelectedMainRow.CostCenterPerRow!=null)
                {
                    if (!string.IsNullOrWhiteSpace(_ViewModel.SelectedMainRow.CostCenterPerRow.Ename))
                    {
                        worksheet.Cells[1, 0] = new Cell("Cost Center :" + _ViewModel.SelectedMainRow.CostCenterPerRow.Ename);
                    }
            
                }

              
                worksheet.Cells[1, 1] = new Cell("From :" +_ViewModel.SelectedMainRow.FromDate);
                worksheet.Cells[1, 2] = new Cell("To :" + _ViewModel.SelectedMainRow.ToDate);

                worksheet.Cells[2, 0] = new Cell("Description");
                worksheet.Cells[2, 1] = new Cell("Dr");
                worksheet.Cells[2, 2] = new Cell("Cr");

                for (var columnIndex = 0; columnIndex < columns.Count(); columnIndex++)
                {
                    //var gridColumn = columns.ElementAt(columnIndex);

                    //data[0, columnIndex] = GetHeader(gridColumn);
                    var cell = new Cell(columns.ElementAt(columnIndex));

                    worksheet.Cells[0, columnIndex] = cell;

                    //var path = GetPath(gridColumn);

                    //var formatForExport = GetFormatForExport(gridColumn);

                    //        if (path != null)
                    //       {
                    // Fill data with values
                    for (var rowIndex = 0; rowIndex < data.Count; rowIndex++)
                    {
                        //var source = data[rowIndex - 1];
                        //   data[rowIndex, columnIndex] = GetValue(path, source, formatForExport);
                        //    worksheet.Cells[rowIndex, columnIndex] = new Cell(data[rowIndex, columnIndex]);
                        worksheet.Cells[rowIndex+3, 0] = new Cell(data[rowIndex].Description);
                        worksheet.Cells[rowIndex + 3, 1] = new Cell(data[rowIndex].DrAmount??0);
                        worksheet.Cells[rowIndex + 3, 2] = new Cell(data[rowIndex].CrAmount??0);


                        //               }
                        //             }
                    }

          
                }
                workbook.Worksheets.Add(worksheet);
                var sFile = sDialog.OpenFile();

                // save method needs a stream object to write an excel file.
                workbook.Save(sFile);
            }
        }



        private void BtnPreview_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.GetMaindata();
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {

            ViewModel.Clear();


        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportExcel();
        }
    }
}