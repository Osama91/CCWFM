using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.AccessoriesViewModel;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class AccSubGroupSizes
    {
        public AccSubGroupSizes(AccGroupViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AccSubGroup_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AccGroupViewModel;
            var check = sender as CheckBox;
            if (check != null)
            {
                var brandSectionLinkRow = check.DataContext as TblAccessorySizeViewModel;
                if (brandSectionLinkRow != null)
                {
                    brandSectionLinkRow.UpdatedAllowed = true;
                    brandSectionLinkRow.TblAccSize = brandSectionLinkRow.SizePerRow.Iserial;
                    if (viewModel != null)
                        brandSectionLinkRow.tbl_AccessoriesSubGroup = viewModel.SelectedDetailRow.Iserial;
                    if (check.IsChecked != null) brandSectionLinkRow.Checked = (bool)check.IsChecked;
                }
            }
        }

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AccGroupViewModel;
            var check = sender as CheckBox;
            if (viewModel != null)
                foreach (var brandSectionLinkRow in viewModel.SelectedDetailRow.DetailsList)
                {
                    brandSectionLinkRow.UpdatedAllowed = true;
                    brandSectionLinkRow.TblAccSize = brandSectionLinkRow.SizePerRow.Iserial;
                    brandSectionLinkRow.tbl_AccessoriesSubGroup = viewModel.SelectedDetailRow.Iserial;
                    if (check != null)
                        if (check.IsChecked != null) brandSectionLinkRow.Checked = (bool)check.IsChecked;
                }
        }
    }
}