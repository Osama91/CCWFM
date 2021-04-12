using System.Windows.Navigation;

namespace CCWFM.Views.FabricTools
{
    public partial class FabImgs
    {
        public FabImgs()
        {
            InitializeComponent();
            LayoutRoot.Children.Add(new UserControls.FabricImageGallary());
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}