using CCWFM.ViewModel;
using GalaSoft.MvvmLight.Command;
using Lite.ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.Automation;
using System.Windows;
using System.Windows.Controls;
namespace CCWFM.Views.OGView
{
    public partial class FixAtt
    {
        AttService.AttServiceClient client = new AttService.AttServiceClient();
        public ObservableCollection<AttService.Transaction> ImportedList = new ObservableCollection<AttService.Transaction>();

        public FixAtt()
        {
           
            InitializeComponent();
            SaveImportedSheet.Visibility = Visibility.Visible;
            AttService.AttServiceClient Fixatt = new AttService.AttServiceClient();
            SaveImportedSheet.Visibility = Visibility.Collapsed;
         //   BtnAddNewOrder.Visibility = Visibility.Collapsed;

            client.SaveFixAttCompleted += (s, sv) =>
            {
                if (sv != null)
                {
                    dtpDate.IsEnabled = true;
                    imgImportExcel.Visibility = Visibility.Visible;
                    gv_Imported.ItemsSource = ImportedList;
                    MessageBox.Show($"{ImportedList.Count.ToString()} Row(s) imported and saved successfully.");
                }
                else
                    MessageBox.Show("Unexpected error, Nothing Saved.");
            };
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            AttService.AttServiceClient client = new AttService.AttServiceClient();
            client.FixAttAsync();
        }


        private void btnBrowse_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void imgImportExcel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (dtpDate.SelectedDate == null)
                {
                    MessageBox.Show("Please insert the date");
                    return;
                }
                if (dtpDate.SelectedDate.Value.Day < 12 || dtpDate.SelectedDate.Value.Day > 20)
                {
                    MessageBox.Show("Please select a date between 12 and 20");
                    return;
                }
                dtpDate.IsEnabled = false;
                var oFile = new OpenFileDialog();
                oFile.Filter = "Excel (*.xls)|*.xls";
                if (oFile.ShowDialog() == true)
                {
                    var fs = oFile.File.OpenRead();
                    var book = Workbook.Open(fs);
                    var sheet = book.Worksheets[0];
                    int i = sheet.Cells.FirstRowIndex;
                    var temp = sheet.Cells[i, 2].StringValue.Trim().ToLower();
                    if (sheet.Cells[i, 2].StringValue.Trim().ToLower() == "amount") //Check if the table has titles in the sheet
                        i++;

                    for (int x = i; x < sheet.Cells.LastRowIndex + 1; x++)
                    {
                        var newtransaction = new AttService.Transaction();
                        newtransaction.Code = sheet.Cells[x, 0].StringValue.Trim();
                        newtransaction.Name = sheet.Cells[x, 1].StringValue.Trim();
                        newtransaction.Amount = double.Parse(sheet.Cells[x, 2].StringValue.Trim());
                        newtransaction.SalaryTerm = sheet.Cells[x, 3].StringValue.Trim();
                        ImportedList.Add(newtransaction);
                    }
                    gv_Imported.ItemsSource = ImportedList;
                    ImportebCount.Content = $"Imported List: ({ImportedList.Count.ToString()})"; // Display count of imported records from excel sheet
                    var mydate = dtpDate.SelectedDate.Value;
                    imgImportExcel.Visibility = Visibility.Collapsed;
                    SaveImportedSheet.Visibility = Visibility.Visible;

                    //  client.SaveFixAttAsync(ImportedList, dtpDate.SelectedDate.Value);
                }
            }
            catch (Exception ex)
            {
                dtpDate.IsEnabled = true;
                imgImportExcel.Visibility = Visibility.Visible;
                MessageBox.Show("Unexpected error, Please review your excel sheet and try again.");
            }
        }

        private void SaveImportedSheet_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Imported data will be saved to database, Are you sure you want to continue ?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                client.SaveFixAttAsync(ImportedList, dtpDate.SelectedDate.Value);
                ImportebCount.Content = $"Saved: ({ImportedList.Count.ToString()})";
                SaveImportedSheet.Visibility = Visibility.Collapsed;
                BtnAddNewOrder.Visibility = Visibility.Visible;

            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Imported data will be saved to database, Are you sure you want to continue ?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                client.SaveFixAttAsync(ImportedList, dtpDate.SelectedDate.Value);
                ImportebCount.Content = $"Saved: ({ImportedList.Count.ToString()})";
                SaveImportedSheet.Visibility = Visibility.Collapsed;
                BtnAddNewOrder.Visibility = Visibility.Visible;

            }
        }
    }
}