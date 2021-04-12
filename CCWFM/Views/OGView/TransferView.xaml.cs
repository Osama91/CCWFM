using System.Windows.Controls;

namespace CCWFM.Views.OGView
{
    public partial class TransferView : Page
    {
        public TransferView(bool isFrom)
        {
            this.isFrom = isFrom;
            InitializeComponent();
        }

        bool isFrom;
        public bool IsFrom
        {
            get { return isFrom; }
            set { isFrom = value; }
        }

        private void btnFPItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //MyPopup.IsOpen = true;
        }

        private void btnItemFPOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           // MyPopup.IsOpen = false;
        }

        private void btnItemFPCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
          //  MyPopup.IsOpen = false;
        }
    }
}
