using System;
using System.Windows;

namespace CCWFM.Views.StylePages
{
    public partial class SMLRatioEditor
    {
        public event EventHandler SubmitRatios;

        protected virtual void OnSubmitRatios()
        {
            var handler = SubmitRatios;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public SMLRatioEditor()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OnSubmitRatios();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}