using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class FamilyLink
    {
        private readonly FamilyLinkViewModel _viewModel;

        public FamilyLink()
        {
            InitializeComponent();
            _viewModel = (FamilyLinkViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                _viewModel.SelectedMainRow.DetailsList.Clear();

                _viewModel.GetDetailData();
            }
        }

        private void BrandSection_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var brandSectionLinkRow = check.DataContext as TblLkpBrandSectionLinkViewModel;
            if (brandSectionLinkRow != null)
            {
                brandSectionLinkRow.UpdatedAllowed = true;
                brandSectionLinkRow.TblBrand = _viewModel.BrandCode;
                brandSectionLinkRow.TblLkpBrandSection = brandSectionLinkRow.Iserial;
                brandSectionLinkRow.Checked = (bool)check.IsChecked;
            }
        }

        private void MainCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var familyLinkRow = check.DataContext as TblFamilyLinkViewModel;
            if (familyLinkRow != null)
            {
                familyLinkRow.UpdatedAllowed = true;
                familyLinkRow.TblBrand = _viewModel.BrandCode;
                familyLinkRow.TblLkpBrandSection = _viewModel.SelectedBrandSection.Iserial;
                familyLinkRow.TblFamily = familyLinkRow.Iserial;
                familyLinkRow.Checked = (bool)check.IsChecked;
            }
        }

        private void SubCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var subLinkRow = check.DataContext as TblSubFamilyLinkViewModel;
            if (subLinkRow != null)
            {
                subLinkRow.UpdatedAllowed = true;
                subLinkRow.TblBrand = _viewModel.BrandCode;
                subLinkRow.TblLkpBrandSection = _viewModel.SelectedBrandSection.Iserial;
                subLinkRow.TblFamily = _viewModel.SelectedMainRow.Iserial;
                subLinkRow.TblSubFamily = subLinkRow.Iserial;
                if (check.IsChecked != null) subLinkRow.Checked = (bool)check.IsChecked;
            }
        }

        private void BrandSectionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandSectionGrid.SelectedItem != null)
            {
                _viewModel.MainSelectedRows.Clear();
                _viewModel.SectionChanged();
            }
        }

        private void StoresComplete_Loaded(object sender, RoutedEventArgs e)
        {

            var autoComplete = sender as AutoCompleteBox;

            if (_viewModel != null)
                _viewModel.StoreCompleted += (s, r) => autoComplete.PopulateComplete();
            autoComplete.ItemFilter += Filter;

            
        }

        private void StoresComplete_Populating(object sender, PopulatingEventArgs e)
        {
            var autoComplete = sender as AutoCompleteBox;
            e.Cancel = true;
            _viewModel.SearchForStore(autoComplete.Text);

        }
        private bool Filter(string search, object item)
        {
            var myItem = item as TblStore;
            if (myItem == null)
                return false;

            // you would obviously check if you can parse search to number
            if (myItem.ENAME.ToLower().StartsWith(search.ToLower()) || myItem.aname.ToLower().StartsWith(search.ToLower()) 
                || myItem.code.ToLower().StartsWith(search.ToLower()))
            {
                return true;
            }

            return false;
        }



    }
}