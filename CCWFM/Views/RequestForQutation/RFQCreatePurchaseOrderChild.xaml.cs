using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.Views.RequestForQutation
{
    public partial class RFQCreatePurchaseOrderChild : ChildWindow
    {
        public RFQCreatePurchaseOrderChild(PurchaseOrderCreationChildViewModel childDataContext)
        {
            InitializeComponent();
            LayoutRoot.DataContext = childDataContext;
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

