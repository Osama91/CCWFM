using System;
using System.Windows;

namespace CCWFM.Views.OGView
{
    public partial class ImportUserChildWindow
    {
        public event EventHandler SubmitClicked;

        public ImportUserChildWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubmitClicked != null)
            {
                SubmitClicked(this, new EventArgs());
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}