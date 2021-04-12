using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView
{
    public partial class BrandBudget
    {
        public BrandBudgetViewModel ViewModel;
        public int TransactionType = 0;

        public BrandBudget(int transactionType)
        {
            InitializeComponent();
            ViewModel = LayoutRoot.DataContext as BrandBudgetViewModel;
            TransactionType = ViewModel.TransactionType = transactionType;
            DataContext = ViewModel;
            SwitchFormMode(FormMode.Add);

            if (TransactionType == 1)
            {
                DetailGrid.Columns.FirstOrDefault().Visibility = Visibility.Collapsed;
            }
            else
            {
                DetailGrid.Columns.ElementAt(1).Visibility = Visibility.Collapsed;
                DetailGrid.Columns.ElementAt(2).Visibility = Visibility.Collapsed;
                DetailGrid.Columns.ElementAt(3).Visibility = Visibility.Collapsed;
            }
            ViewModel.TransactionHeader.DetailsList.RemoveAt(ViewModel.TransactionHeader.DetailsList.Count - 1);
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            ViewModel.TransactionHeader.InjectFrom(new TblBrandBudgetsHeaderViewModel());
            ViewModel.TransactionHeader.TransactionType = TransactionType;
        }

        private void LookupCombo_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var cmb = sender as ComboBox;
            switch (cmb.Tag.ToString())
            {
                case "Direction":
                    ViewModel.GetDirectionLink();
                    break;
                case "StyleCategory":
                    ViewModel.GetStyleCategoryLink();
                    break;
                case "Family":
                    ViewModel.GetFamilyLink();
                    break;
                    //case "Theme":
                    //    ViewModel.GetTheme();
                    //    break;
            }
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnCancelOrder.Visibility = Visibility.Visible;
                    BtnCancelOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    ViewModel.TransactionHeader.Enabled = true;

                    break;

                case FormMode.Standby:

                    ViewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:

                    ViewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                    break;

                case FormMode.Update:

                    ViewModel.TransactionHeader.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Read:
                    ViewModel.TransactionHeader.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    BtnSaveOrder.IsEnabled = false;
                    //   BtnEditOrder.IsEnabled = true;
                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            //         BtnDeleteOrder.Visibility = Visibility.Visible;
            //         BtnDeleteOrder.IsEnabled = true;
            //   BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            if (_FormMode == FormMode.Add)
            {
                ViewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
            }
            else
            {
                _FormMode = FormMode.Add;
                SwitchFormMode(_FormMode);
            }
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.DataGridName == "DetailGrid")
            {
                ViewModel.DeleteDetailRow();
            }
            else
            {
                ViewModel.DeleteMainRow();
            }
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
            //       BtnDeleteOrder.Visibility = Visibility.Collapsed;
            //      BtnDeleteOrder.IsEnabled = false;
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
            var childWindowSeach = new BrandBudgetHeaderChildWindow(ViewModel);

            childWindowSeach.Show();

            ViewModel.GetMaindata();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            var RowToRemove = ViewModel.TransactionHeader.DetailsList.FirstOrDefault(x => x.DirectionPerRow == null || x.StyleCategoryPerRow == null || x.FamilyPerRow == null /*|| x.ThemePerRow == null*/);// Removes empty rows
            ViewModel.TransactionHeader.DetailsList.Remove(RowToRemove);
            CalculateTotalCost();

            //for (int i = 0; i < ViewModel.TransactionHeader.DetailsList.Count; i++)
            //{
            //    if (i < ViewModel.TransactionHeader.DetailsList.Count - 1) //check duplicates
            //    {
            //        if (ViewModel.TransactionHeader.DetailsList[i].FamilyPerRow.Ename == ViewModel.TransactionHeader.DetailsList[i + 1].FamilyPerRow.Ename)
            //        {
            //            MessageBox.Show("You Cannot Duplicate Family");
            //            return;
            //        }
            //    }
            //}
            //     ViewModel.SaveDetailRow();
            ViewModel.SaveMainRow();
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

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.TransactionHeader.DetailsList.IndexOf(ViewModel.SelectedDetailRow));
                if (currentRowIndex == (ViewModel.TransactionHeader.DetailsList.Count - 2))
                {
                    ViewModel.AddNewDetailRow(true);
                }
            }
        }

        private void DetailGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ViewModel.SaveDetailRow();
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "BrandBuget";

            var para = new ObservableCollection<string> { ViewModel.TransactionHeader.Iserial.ToString() };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void DetailGrid_OnLayoutUpdated(object sender, EventArgs e)
        {
            var tblBrandBudgetDetailViewModel = ViewModel.TransactionHeader.DetailsList.FirstOrDefault(x => x.Iserial == -1);
            if (tblBrandBudgetDetailViewModel != null)
            {
                tblBrandBudgetDetailViewModel.Qty =
                        ViewModel.TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.Qty);

                tblBrandBudgetDetailViewModel.Amount =
                    ViewModel.TransactionHeader.DetailsList.Where(x => x.Iserial != -1).Sum(x => x.Amount);
            }
        }

        private void DetailGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (ViewModel.SelectedDetailRow.Iserial == -1)
            //{
            //    e.Cancel = true;
            //}
        }
        #region ColumnsKeyUpAndDowns

        private void FamilyUpdate_OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void FamilyUpdate_OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DirectionUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DirectionUpdate_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void StyleCategoryUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void StyleCategoryUpdate_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
        private void DetailGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ViewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    ViewModel.SelectedDetailRows.Add(row as TblBrandBudgetDetailViewModel);
                }

                ViewModel.DeleteDetailRow();
            }
            else if (e.Key == Key.Down)
            {
                foreach (var row in DetailGrid.SelectedItems)
                {
                    ViewModel.SelectedDetailRows.Add(row as TblBrandBudgetDetailViewModel);
                }
                // ViewModel.DeleteDetailRow();

            }

        }

        private void AddGridRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewDetailRow(true);
        }

        /// <summary>
        /// Calculate Total cost with ( Total Cost = Cost * Qty) for all rows and set the result to Amount in Header Details
        /// </summary>
        void CalculateTotalCost()
        {
            //check the total cost of the entered quantity and compare with the amount of the header
            double TotalCost = 0.0;
            for (int i = 0; i < ViewModel.TransactionHeader.DetailsList.Count; i++)
                TotalCost += ViewModel.TransactionHeader.DetailsList[i].Amount * ViewModel.TransactionHeader.DetailsList[i].Qty;
            txtTotalCost.Text = TotalCost.ToString();
        }

        private void DetailGrid_RowEditEnded_1(object sender, DataGridRowEditEndedEventArgs e)
        {
            CalculateTotalCost();
        }
    }
}