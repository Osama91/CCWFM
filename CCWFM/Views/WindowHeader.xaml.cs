using CCWFM.ViewModel;
using System.Windows.Controls;

namespace CCWFM.Views
{
    public partial class WindowHeader : StackPanel
    {
        ViewModelStructuredBase viewModel;
        public WindowHeader()
        {
            InitializeComponent();
        }

        public ViewModelStructuredBase ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; DataContext = ViewModel; }
        }
    }
}
