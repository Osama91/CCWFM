using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.PrintPreviews;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView
{
    public partial class Assets
    {
        public AssetsViewModel ViewModel;

        public Assets()
        {
            InitializeComponent();

            ViewModel = LayoutRoot.DataContext as AssetsViewModel;
            DataContext = ViewModel;
            SwitchFormMode(FormMode.Add);
            if (ViewModel != null) ViewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
        }

        #region FormModesSettings

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
            ViewModel.SelectedMainRow.InjectFrom(new TblAssetsViewModel());
        
            //ViewModel.SelectedMainRow.AssetTypePerRow = ViewModel.AssetsTypeList.FirstOrDefault(x => x.Code == "N/a");
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    ViewModel.SelectedMainRow.Enabled = true;
                    BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
                    ViewModel.GetMaxAsset();
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
                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Update;
            SwitchFormMode(FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            //         BtnDeleteOrder.Visibility = Visibility.Visible;
            //         BtnDeleteOrder.IsEnabled = true;
            //   BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
            
        }


        internal void PrintBarcode()
        {
            var printingPage = new BarcodePrintPreview(ViewModel.SelectedMainRow, 3, (LoggedUserInfo.BarcodeSettingHeader.Code), true);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            printingPage.Show();
        }
        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteMainRow();
        }

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancelOrder.IsEnabled = false;
            BtnCancelOrder.Visibility = Visibility.Collapsed;            
            BtnShowSearchOrder.IsChecked = false;
        }

        public FormMode FormMode { get; set; }

        private void ResetMode()
        {
            FormMode = FormMode.Standby;
            SwitchFormMode(FormMode);
        }
        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }
        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SearchAssets();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }

        #endregion FormModesSettings

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            PrintBarcode();
        }

        private void btnCopyOrder_Click(object sender, RoutedEventArgs e)
        {
            var child = new CopyAsset(DataContext as AssetsViewModel);
            child.Show();
        }

        private void FrameworkElement_OnBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action== ValidationErrorEventAction.Added)
            {
                var control = e.OriginalSource as Control;
                if (control != null)
                    control.Background= new SolidColorBrush(Colors.Yellow);
                ToolTipService.SetToolTip((e.OriginalSource as TextBox),e.Error.Exception.Message);
            }
            else if (e.Action== ValidationErrorEventAction.Removed)
            {
                var control = e.OriginalSource as Control;
                if (control != null)
                    control.Background= new SolidColorBrush(Colors.White);
                ToolTipService.SetToolTip((e.OriginalSource as TextBox),null);
            }
        }
    }
}