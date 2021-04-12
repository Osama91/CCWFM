using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class DirectionLink
    {
        private readonly DirectionLinkViewModel _viewModel;

        public DirectionLink()
        {
            InitializeComponent();
            _viewModel = (DirectionLinkViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        
        }       

        private void Direction_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var directionLinkRow = check.DataContext as TblLkpDirectionLinkViewModel;
            directionLinkRow.UpdatedAllowed = true;
            directionLinkRow.TblBrand = _viewModel.BrandCode;
            directionLinkRow.TblLkpBrandSection = _viewModel.BrandSection;
            directionLinkRow.TblLkpDirection = directionLinkRow.Iserial;
            directionLinkRow.Checked = (bool)check.IsChecked;
        }

     

    }
}