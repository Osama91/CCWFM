using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Views.PrintPreviews;

namespace CCWFM.UserControls
{
    public partial class DisplayingBarcodeToPrintUserControl
    {
        static public BarcodeLayoutSettings BarcodeSettings;

        public DisplayingBarcodeToPrintUserControl(string Value, BarcodeLayoutSettings LayoutSetting, ObservableCollection<BarcodeLayoutSettings> LayoutSettingsList, List<DefectsData> DefectsDataList)
        {
            InitializeComponent();

            var currentUi = Thread.CurrentThread.CurrentUICulture;

            FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            DataContext = LayoutSetting;
            BarcodeSettings = LayoutSetting;
            var Content = new TextBlock();

            var styleResource = Resources["TextBlockUserResource"] as Style;

            Content.Style = styleResource;

            if (LayoutSetting.PropertyType == "Field")
            {
                var maxPageWidth = ConvertToPixels(LayoutSetting.PageWidthSizeUnitProperty) * .7;
                var textboxWidth = GetTextWidth(Value, Convert.ToDouble(LayoutSetting.FontSize), LayoutSetting.FontFamily);
                if (textboxWidth > maxPageWidth)
                {
                    textboxWidth = maxPageWidth;
                }

                Content.Width = textboxWidth;
                Content.Text = Value;
                MyCanvas.Children.Add(Content);
            }

            else if (LayoutSetting.PropertyType == "Barcode")
            {
                var barcodeStack = new StackPanel { Orientation = Orientation.Vertical };
                Canvas.SetLeft(barcodeStack, LayoutSetting.CanvasLeft);
                Canvas.SetTop(barcodeStack, LayoutSetting.CanvasTop);
                var barcodeContentControl = new Image();
                var barcodeLabel = new TextBlock { Style = Resources["BarcodeLabel"] as Style };

                var styleResourceBarcode = Resources["BarcodeContentControlResouces"] as Style;
                barcodeContentControl.Style = styleResourceBarcode;
                barcodeContentControl.Stretch = Stretch.UniformToFill;
                barcodeContentControl.Width = LayoutSetting.BarcodeWidth;
                barcodeContentControl.Height = LayoutSetting.BarcodeHeight;
                barcodeLabel.HorizontalAlignment = HorizontalAlignment.Center;
                LayoutSetting.PropertyValue = Value;

                barcodeLabel.Text = Value;
                BarcodeSettings.PropertyValue = Value;
                barcodeStack.Children.Add(barcodeContentControl);
                barcodeStack.Children.Add(barcodeLabel);

                MyCanvas.Children.Add(barcodeStack);
            }

            else if (LayoutSetting.PropertyType == "Label")
            {
                if (currentUi.DisplayName == "العربية")
                {
                    Content.Text = LayoutSetting.PropertyNameArabic;
                    Content.Width = GetTextWidth(Content.Text, Convert.ToDouble(LayoutSetting.FontSize), LayoutSetting.FontFamily); ;
                }
                else
                {
                    Content.Text = LayoutSetting.PropertyName;
                }

                MyCanvas.Children.Add(Content);
            }

            var defects = LayoutSettingsList.SingleOrDefault(s => s.printingPropertiesIserial == 6);

            var defectsStack = new StackPanel();

            double maxlenght = 0;
            if (DefectsDataList != null)
            {
                foreach (var item in DefectsDataList.OrderByDescending(x => x.Aname.Length))
                {
                    var gridDefects = new StackPanel { Orientation = Orientation.Horizontal };

                    var contentDefects = new TextBlock();
                    var contentDefectsLabel = new TextBlock();
                    Canvas.SetLeft(defectsStack, defects.CanvasLeft);
                    Canvas.SetTop(defectsStack, defects.CanvasTop);

                    contentDefects.Style = styleResource;
                    contentDefectsLabel.Style = styleResource;

                    contentDefects.Text = item.DefectValue.ToString();

                    if (currentUi.DisplayName == "العربية")
                    {
                        if (maxlenght == 0)
                        {
                            maxlenght = GetTextWidth(item.Aname, Convert.ToDouble(defects.FontSize), defects.FontFamily);
                        }

                        contentDefectsLabel.Text = item.Aname;
                    }
                    else
                    {
                        if (maxlenght == 0)
                        {
                            maxlenght = GetTextWidth(item.Ename, Convert.ToDouble(defects.FontSize), defects.FontFamily);
                        }
                        contentDefectsLabel.Text = item.Ename;
                    }

                    contentDefectsLabel.MinWidth = maxlenght;
                    defectsStack.Orientation = Orientation.Vertical;

                    gridDefects.Children.Add(contentDefectsLabel);
                    gridDefects.Children.Add(contentDefects);

                    defectsStack.Children.Add(gridDefects);
                }

                MyCanvas.Children.Add(defectsStack);
            }
        }

        public double ConvertToPixels(string value)
        {
            const double inch = 96.0;
            const double cm = 37.7952755905512;

            var strValue = value;

            var values = strValue.Split(" ".ToCharArray());
            var unit = values[1];
            var size = values[0];

            if (unit == "CM")
            {
                return (Convert.ToDouble(size) * cm * .9);
            }
            if (unit == "Inch")
            {
                return (Convert.ToDouble(size) * inch * .9);
            }
            return (Convert.ToDouble(size) * .9);
        }

        private double GetTextWidth(string text, double fontSize, string FontFamily)
        {
            var txtMeasure = new TextBlock
            {
                FontSize = fontSize,
                Text = text,
                FontFamily = new FontFamily(FontFamily)
            };
            var width = txtMeasure.ActualWidth;
            return width;
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged
    }
}