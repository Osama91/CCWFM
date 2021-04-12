using System.Windows;
using CCWFM.ViewModel.SMLViewModels;

namespace CCWFM.Views.StylePages
{
    public partial class NewRFQCreatePurchaseOrderChild
    {
        public NewRFQCreatePurchaseOrderChild(NewRFQ_PurchaseOrderCreationChildViewModel childDataContext)
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

