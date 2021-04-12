using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StyleTNAChildWindow
    {
        private readonly StyleTNAHeaderViewModel _viewModel;
        private StyleHeaderViewModel StyleviewModel;
        public StyleTNAChildWindow(StyleHeaderViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = new StyleTNAHeaderViewModel(viewModel);
            DataContext = _viewModel;
            StyleviewModel = viewModel;
            //TxtCreatedBy.Text = viewModel.SelectedMainRow.CreatedBy;
            //TxtCreationDate.Text = viewModel.SelectedMainRow.CreationDate.Value.ToShortDateString();
        }   

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PreviewReport();
        }
    }
}