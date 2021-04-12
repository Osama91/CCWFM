using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class DisplayingBarcodeSetup
    {
        private CRUD_ManagerServiceClient _webservice = new CRUD_ManagerServiceClient();
        private BarcodeSettingsUiViewModel ViewModel = new BarcodeSettingsUiViewModel();
        private BarcodeSettingsHeader _viewModel;

        public DisplayingBarcodeSetup()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SubmitClicked += ViewModel_SubmitClicked;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void CmbBarcodeOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BarcodeSettingHeaderList.Add(new BarcodeSettingsHeader());
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in dgBarcodeSettingsHeader.ItemsSource)
                {
                    ViewModel.SaveBarcodeDisplaySettingsHeader(item as BarcodeSettingsHeader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteBarcodeDisplaySettingsHeader(_viewModel);
            ViewModel.BarcodeSettingHeaderList.Remove(_viewModel);
        }

        private void dgBarcodeSettingsHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel = dgBarcodeSettingsHeader.SelectedItem as BarcodeSettingsHeader;
        }

        private void btnChangeDesign_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetDetails(_viewModel);
        }

        private void ViewModel_SubmitClicked(object sender, EventArgs e)
        {
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            var childWindows = new UiBarcodeSetup(ViewModel, _viewModel);
            if (currentUi.DisplayName == "العربية")
            {
                childWindows.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                childWindows.FlowDirection = FlowDirection.LeftToRight;
            }

            childWindows.Show();
        }
    }
}