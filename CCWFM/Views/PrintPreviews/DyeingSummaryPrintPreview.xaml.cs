using System.Windows;

namespace CCWFM.Views.PrintPreviews
{
    public partial class DyeingSummaryPrintPreview
    {
        public DyeingSummaryPrintPreview()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

