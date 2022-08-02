using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class TradeAgreementFabricView
    {
        public TradeAgreementFabricView()
        {
            InitializeComponent();
            var context =
            this.LayoutRoot.DataContext as TradeAgreementViewModel;
            context.ExportGrid = DetailGrid;
        }
    }
}