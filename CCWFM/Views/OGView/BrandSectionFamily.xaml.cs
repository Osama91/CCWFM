using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class BrandSectionFamily
    {
        private readonly BrandSectionFamilySizeGroupViewModelViewModel _viewModel;

        public BrandSectionFamily()
        {
            InitializeComponent();
            _viewModel = (BrandSectionFamilySizeGroupViewModelViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

    }
}