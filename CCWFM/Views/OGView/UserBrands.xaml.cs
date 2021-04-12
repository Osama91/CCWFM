using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
  
    public partial class UserBrands
    {
        private readonly UserBrandViewModel _viewModel;

        public UserBrands()
        {
            InitializeComponent();
            _viewModel = (UserBrandViewModel)LayoutRoot.DataContext;
                
        }

        private void Cmb_Users_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUsers.SelectedValue != null) _viewModel.GetBrandSectionPerUser();
        }

        private void BrandSection_Click(object sender, RoutedEventArgs e)
        {
            
            var check = sender as CheckBox;
            var brandSectionLinkRow = check.DataContext as TblUserBrandSectionViewModel;
            brandSectionLinkRow.UpdatedAllowed = true;
            brandSectionLinkRow.BrandCode = _viewModel.SelectedBrand.Brand_Code;
            brandSectionLinkRow.TblAuthUser = _viewModel.SelectedUser.Iserial;
            brandSectionLinkRow.Checked = (bool)check.IsChecked;
        }

        private void Permission_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var brandSectionLinkRow = check.DataContext as TblUserBrandSectionPermissionViewModel;
            brandSectionLinkRow.UpdatedAllowed = true;
            
            brandSectionLinkRow.TblUserBrandSection = _viewModel.SelectedUserBrandSection.Iserial;
            
            brandSectionLinkRow.Checked = (bool)check.IsChecked;
        }
    }
}