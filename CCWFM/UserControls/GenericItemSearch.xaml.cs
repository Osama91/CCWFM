using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel;

namespace CCWFM.UserControls
{
    public partial class GenericItemSearch
    {
        private readonly GenericItemSearchViewModel _viewModel = new GenericItemSearchViewModel();

        public GenericItemSearch()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void SearchItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                (!string.IsNullOrWhiteSpace(_viewModel.ItemDescription) || !string.IsNullOrWhiteSpace(_viewModel.ItemId)))
            {
                _viewModel.Search();
            }
        }

        private void DgMainItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = DgMainItem.SelectedItem as CRUD_ManagerServiceAxItem;
            if (selectedItem != null)
            {
                _viewModel.SearchedAxItemsDetails(selectedItem);
            }
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}