using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class CheckListLink
    {
        private readonly CheckListLinkViewModel _viewModel;

        public CheckListLink()
        {
            InitializeComponent();
            _viewModel = (CheckListLinkViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            
        }      
        private void MainCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var checkListLinkRow = check.DataContext as TblCheckListLinkViewModel;
            checkListLinkRow.UpdatedAllowed = true;
            checkListLinkRow.TblCheckListGroup = _viewModel.SelectedMainRow.Iserial;
            checkListLinkRow.Checked = (bool)check.IsChecked;
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandSectionGrid.SelectedItem != null)
            {                    
                _viewModel.SectionChanged();
            }
        }    
    }
}