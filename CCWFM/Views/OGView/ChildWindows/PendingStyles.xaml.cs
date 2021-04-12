using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PendingStyles 
    {
        public PendingStyles(StyleHeaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

