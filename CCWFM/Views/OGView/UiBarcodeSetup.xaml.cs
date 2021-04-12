using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.InputValidators;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.PrintPreviews;

namespace CCWFM.Views.OGView
{
    public partial class UiBarcodeSetup
    {
        private CRUD_ManagerServiceClient webService = new CRUD_ManagerServiceClient();

        private IList<FontFamily> fontFamiliesList = new List<string>
        {
        "Arial",
        "Calibri",
        "Cambria",
        "Candara",
        "Consolas",
        "Corbel",
        "Courier New",
        "Georgia",
        "Gigi",
        "Jokerman",
        "Lucida Sans Unicode",
        "Magneto",
        "Precursor Alphabet",
        "Quartz MS",
        "Segoe UI",
        "Times New Roman",
        "Trebuchet MS",
        }.Select(s => new FontFamily(s)).ToList<FontFamily>();

        public BarcodeSettingsHeader ViewModel;

        public BarcodeSettingsUiViewModel ViewModelProperty { get; set; }

        public UiBarcodeSetup(BarcodeSettingsUiViewModel ViewModelOld, BarcodeSettingsHeader _ViewModelOld)
        {
            InitializeComponent();

            CultureInfo currentUi = Thread.CurrentThread.CurrentUICulture;

            if (currentUi.DisplayName == "العربية")
            {
                FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                FlowDirection = FlowDirection.LeftToRight;
            }

            webService.PrintingFabricDefectsAllCompleted += webService_PrintingFabricDefectsAllCompleted;
            CmbFontFamily.ItemsSource = fontFamiliesList;
            ViewModel = _ViewModelOld;
            ViewModelProperty = ViewModelOld;
            LayoutRoot.DataContext = ViewModelProperty;
            panel.DataContext = _ViewModelOld;
            Load();
        }

        private void webService_PrintingFabricDefectsAllCompleted(object sender, PrintingFabricDefectsAllCompletedEventArgs e)
        {
            var printingPage = new BarcodePrintPreview(e.DefectsList.ToList(), new ObservableCollection<PrintingFabricDefect> { e.Result }, 1, (LoggedUserInfo.BarcodeSettingHeader.Code), false);

            var currentUi = Thread.CurrentThread.CurrentUICulture;

            printingPage.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            printingPage.Show();
        }

        private void Load()
        {
            // panel.Children.Clear();

            foreach (var item in ViewModelProperty.GenericBarcodeTemplate)
            {
                if (!panel.Children.Contains(item))
                {
                    panel.Children.Add(item);
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModelProperty.SaveDetails(ViewModel);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumValidations.validateTextDouble(sender, e);
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void PropertyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            var row = new GenericBarcodeTemplate(ViewModelProperty)
                   {
                       PropertyType = "Label",
                       CanvasLeftPropperty = 0,
                       CanvasTopPropperty = 0,
                       PropertyName = "Label",
                       PropertyNameArabic = "طابع"
                   };

            ViewModelProperty.GenericBarcodeTemplate.Add(row);
            panel.Children.Add(row);
        }

        private void btnPerview_Click(object sender, RoutedEventArgs e)
        {
            webService.PrintingFabricDefectsAllAsync();
        }
    }
}