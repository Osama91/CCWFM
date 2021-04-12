using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class GlobalBudget
    {
        public GlobalRetailBusinessBudgetViewModel ViewModel;
        public int TransactionType = 0;

        public GlobalBudget(int transactionType)
        {
            InitializeComponent();
            ViewModel = LayoutRoot.DataContext as GlobalRetailBusinessBudgetViewModel;
            TransactionType = ViewModel.TransactionType = transactionType;
            DataContext = ViewModel;
            SwitchFormMode(FormMode.Add);
            ViewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            
            ViewModel.TransactionHeader.InjectFrom(new TblGlobalRetailBusinessBudgetViewModel());
            ViewModel.TransactionHeader.TransactionType = TransactionType;
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    ViewModel.TransactionHeader.Enabled = true;
                    BtnCancelOrder.Visibility = Visibility.Visible;
                    BtnCancelOrder.IsEnabled = true;
                    break;

                case FormMode.Standby:

                    ViewModel.TransactionHeader.Enabled = false;
                    //BtnAddNewOrder.IsEnabled = true;
                    //BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:

                    ViewModel.TransactionHeader.Enabled = true;
                    //BtnAddNewOrder.IsEnabled = false;
                    //BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                    break;

                case FormMode.Update:

                    ViewModel.TransactionHeader.Enabled = true;
                   // BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = true;

                    break;

                case FormMode.Read:
                    ViewModel.TransactionHeader.Enabled = false;
                   // BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = true;

                    break;
            }
        }
        private void BtnAddNewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -2);
        }
        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
           // if (_FormMode== FormMode.Add)
          //  {
                
          //  }
          //  else
          //  {
        //        _FormMode = FormMode.Add;
        //        SwitchFormMode(_FormMode);
          //  }
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteMainRow();
        }

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancelOrder.IsEnabled = false;
            BtnCancelOrder.Visibility = Visibility.Collapsed;
            BtnShowSearchOrder.IsChecked = false;
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainRowList.Clear();
            var childWindowSeach = new GlobalBudgetHeaderChildWindow(ViewModel);
            childWindowSeach.Show();
            ViewModel.GetMaindata();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveDetailRow();
        }

        #endregion FormModesSettings

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (ViewModel.TransactionHeader.DetailsList.Count < ViewModel.PageSize)
            {
                return;
            }
            if (ViewModel.TransactionHeader.DetailsList.Count - 2 < e.Row.GetIndex() && !ViewModel.Loading && ViewModel.TransactionHeader.DetailsList.Count < ViewModel.DetailFullCount)
            {
                ViewModel.Loading = true;
                ViewModel.GetDetailData();
            }
        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            ViewModel.TransactionHeader.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            ViewModel.DetailFilter = filter;
            ViewModel.DetailValuesObjects = valueObjecttemp;
            ViewModel.GetDetailData();
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.TransactionHeader.DetailsList.IndexOf(ViewModel.SelectedDetailRow));
                if (currentRowIndex == (ViewModel.TransactionHeader.DetailsList.Count(x=>x.Iserial!=-1)))
                {
                    ViewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                ViewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    ViewModel.SelectedDetailRows.Add(row as TblGlobalRetailBusinessBudgetMainDetailViewModel);
                }

                ViewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ViewModel.SaveDetailRow();
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "GlobalBrandBudget";

            var para = new ObservableCollection<string> { ViewModel.TransactionHeader.Iserial.ToString() };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void TxtAmount_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            var month = (int)textbox.Tag;

            var binding = textbox.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();

            foreach (var item in ViewModel.MonthTotallist.Where(x => x.Month == month))
            {
                var total = 0;
                foreach (var detail in ViewModel.TransactionHeader.DetailsList.Where(x => x.Iserial != -1))
                {
                    foreach (var subDetail in detail.DetailsList.Where(x => x.Month == month))
                    {
                        total = (int)(total + subDetail.Amount);
                    }
                }
                item.Amount = total;

                ViewModel.TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == -1)
                    .DetailsList.FirstOrDefault(x => x.Month == month)
                    .Amount = item.Amount;
            }
            ViewModel.SelectedDetailRow.TotalAmount = ViewModel.SelectedDetailRow.DetailsList.Sum(x => x.Amount);

            ViewModel.TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == -1).TotalAmount =
                ViewModel.TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.TotalAmount);
        }

        private void DetailGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (ViewModel.SelectedDetailRow.Iserial == -1)
            {
                e.Cancel = true;
            }
        }

       
    }
}