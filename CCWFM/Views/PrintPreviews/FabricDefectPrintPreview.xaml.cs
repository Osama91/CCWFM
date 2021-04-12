using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Printing;
using System.Windows.Shapes;

namespace CCWFM.Views.PrintPreviews
{
    public partial class FabricDefectPrintPreview : ChildWindow
    {
        public FabricDefectPrintPreview()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();

            printDocument.BeginPrint += new EventHandler<BeginPrintEventArgs>(printDocument_BeginPrint);
            //  printDocument.EndPrint += new EventHandler<EndPrintEventArgs>(printDocument_EndPrint);
            printDocument.PrintPage += new EventHandler<PrintPageEventArgs>(printDocument_PrintPage);
            printDocument.Print("SLPrintDemo document");

            this.DialogResult = true;
        }

        private void printDocument_BeginPrint(object sender, BeginPrintEventArgs e)
        {
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = ListPrint;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void MyCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas MyCanvas = sender as Canvas;
            Me.BarcodeSoftware.Barcode.Barcodes barcode = new Me.BarcodeSoftware.Barcode.Barcodes();
            barcode.BarcodeType = Me.BarcodeSoftware.Barcode.Barcodes.BarcodeEnum.Code39;
            barcode.Data = MyCanvas.Tag.ToString();
            barcode.encode();
            string encodedData = barcode.EncodedData;

            int encodedLength = 0;
            for (int x = 0; x < encodedData.Length; x++)
            {
                if (encodedData[x] == 't')
                    encodedLength++;
                else if (encodedData[x] == 'w')
                    encodedLength = encodedLength + 3;
            }

            float barWidth = (float)(MyCanvas.Width / encodedLength);

            if (barWidth < 1)
                barWidth = 1;
            float thickWidth = barWidth * 3;
            double incrementWidth = 0;

            int swing = 0;
            for (int x = 0; x < encodedData.Length; x++)
            {
                Brush brush;
                if (swing == 0)
                    brush = new SolidColorBrush(System.Windows.Media.Colors.Black);
                else
                    brush = new SolidColorBrush(System.Windows.Media.Colors.White);

                if (encodedData[x] == 't')
                {
                    Rectangle r = new Rectangle();
                    r.Fill = brush;
                    r.Width = barWidth;
                    r.Height = MyCanvas.Height;
                    r.SetValue(Canvas.LeftProperty, incrementWidth);
                    r.SetValue(Canvas.TopProperty, 0.0);
                    MyCanvas.Children.Add(r);
                    incrementWidth = incrementWidth + ((barWidth));
                }
                else if (encodedData[x] == 'w')
                {
                    Rectangle r = new Rectangle();
                    r.Fill = brush;
                    r.Width = 3 * barWidth;
                    r.Height = MyCanvas.Height;
                    r.SetValue(Canvas.LeftProperty, incrementWidth);
                    r.SetValue(Canvas.TopProperty, 0.0);
                    MyCanvas.Children.Add(r);
                    incrementWidth = incrementWidth + (3 * (barWidth));
                }

                if (swing == 0)
                    swing = 1;
                else
                    swing = 0;
            }
        }
    }
}