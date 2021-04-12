using System.Windows.Controls;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl
{
    public partial class GlPosting
    {
        private readonly GlPostingViewModel _viewModel;

        public GlPosting()
        {
            InitializeComponent();
            _viewModel = (GlPostingViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

      

        private void ImgClose_OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var gridparent = Parent  as Grid;
            var stackparent = gridparent.Parent as Grid;
            //var pageParent = stackparent.Parent as Page;
            var parent = stackparent.Parent as System.Windows.Controls.ChildWindow;

            if (parent != null) parent.DialogResult = true;
        }

        private void ImgOk_OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.Post();
        }
    }
}