using System;
using System.Collections.ObjectModel;
using System.Windows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView
{
    public partial class AssetsTransaction
    {
        public AssetsTransactionViewModel ViewModel;

        public AssetsTransaction()
        {
            InitializeComponent();

            ViewModel = LayoutRoot.DataContext as AssetsTransactionViewModel;
            DataContext = ViewModel;
            SwitchFormMode(FormMode.Add);
            ViewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            ViewModel.SelectedMainRow.InjectFrom(new TblAssetsTransactionViewModel());
        }

        public void SwitchFormMode(FormMode formMode)
        {
            BtnDeleteOrder.Visibility = Visibility.Collapsed;
            BtnDeleteOrder.IsEnabled = false;
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    ViewModel.SelectedMainRow.Enabled = true;
                    BtnCancelOrder.Visibility = Visibility.Visible;
                    BtnCancelOrder.IsEnabled = true;
                    break;

                case FormMode.Standby:

                    ViewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:

                    ViewModel.SelectedMainRow.Enabled = false;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                    break;

                case FormMode.Update:

                    ViewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Read:
                    ViewModel.SelectedMainRow.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    BtnSaveOrder.IsEnabled = true;
                    BtnDeleteOrder.Visibility = Visibility.Visible;
                    BtnDeleteOrder.IsEnabled = true;
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
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
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

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SearchAssetsTransaction();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }

        #endregion FormModesSettings

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {

            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            var reportName = "AssetsTransaction";

            var para = new ObservableCollection<string> { ViewModel.SelectedMainRow.Iserial.ToString() };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }
    }
}