using System;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.Views.RequestForQutation
{
    public partial class RFQSearchChild : ChildWindow
    {
        public event EventHandler<RFQSearchReturnEventArgs> SubmitSearchResult;

        public RFQSearchChild()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubmitSearchResult != null)
            {
                SubmitSearchResult(this, new RFQSearchReturnEventArgs(null));
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
        }
    }

    public class RFQSearchReturnEventArgs : EventArgs
    {
        private RFQViewModel _rfqObject;

        public RFQViewModel RFQObject
        {
            get { return _rfqObject; }
            set { _rfqObject = value; }
        }

        public RFQSearchReturnEventArgs(RFQViewModel _RFQObject)
        {
            RFQObject = _RFQObject;
        }
    }
}