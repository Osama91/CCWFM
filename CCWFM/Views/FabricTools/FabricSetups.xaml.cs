using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CCWFM.Helpers.InputValidators;
using CCWFM.ViewModel.FabricToolsViewModels;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.Views.FabricTools
{
    public partial class FabricSetups
    {
        private FabTool_SearchResultChild _searchResultChild;

        public FabTool_SearchResultChild SearchResultChild
        {
            get { return _searchResultChild; }
            set { _searchResultChild = value; }
        }

        private FabricSetupsViewModel _fabAttrsViewModelObj;

        public FabricSetupsViewModel FabAttrsViewModelObj
        {
            get { return _fabAttrsViewModelObj; }
            set { _fabAttrsViewModelObj = value; }
        }

        public FabricSetups()
        {
            InitializeComponent();
            FabAttrsViewModelObj = new FabricSetupsViewModel();
            LayoutRoot.DataContext = FabAttrsViewModelObj;
            SearchResultChild = new FabTool_SearchResultChild();
            SearchResultChild.SubmitAction += SearchResultChild_SubmitAction;
        }

        private void SearchResultChild_SubmitAction(object sender, EventArgs e)
        {
            if (SearchResultChild.DialogResult == true)
            {
                if (SearchResultChild.FabricCode != null)
                {
                    FabAttrsViewModelObj =
                        new FabricSetupsViewModel
                            (
                                  SearchResultChild.FabricCode
                                , SearchResultChild.IsSearchingToDye
                                ?
                                    (int)FabCategoryCombo.SelectedValue == 4 ? 2
                                    :
                                    3
                                : int.Parse(FabCategoryCombo.SelectedValue.ToString())
                                , SearchResultChild.IsSearchingToDye
                            );
                    LayoutRoot.DataContext = FabAttrsViewModelObj;
                    if (SearchResultChild.IsSearchingToDye)
                    {
                        BtnCancel.Visibility = Visibility.Visible;
                        BtnCancel.IsEnabled = true;
                        BtnAddNewCard.Visibility = Visibility.Collapsed;
                        BtnSearch.Visibility = Visibility.Collapsed;
                        BtnSave.Visibility = Visibility.Visible;
                        BtnSave.IsEnabled = true;
                        BtnShowSearch.Visibility = Visibility.Collapsed;
                        FabAttrsViewModelObj.CanPostToAX = false;
                        FabAttrsViewModelObj.IsLoadingForDyedFlag = true;
                    }
                    BtnSave.Visibility = Visibility.Visible;
                    BtnSave.IsEnabled = true;
                }
            }
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FabAttrsViewModelObj.ContentCompositionList.Add(new ContentCompositionViewModel());
            ContentListBox.ScrollIntoView(ContentListBox.Items[ContentListBox.Items.Count - 1]);
            var button = sender as Button;
            if (button != null) button.IsEnabled = ContentListBox.Items.Count <= 4;
        }

        private void ContentPercentageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumValidations.validateTextDouble(sender, e);
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (FabAttrsViewModelObj.ObjStatus.IsNew)
            {
                if (FabAttrsViewModelObj.AllowAdd)
                    FabAttrsViewModelObj.InsertFabricAttr();
                else
                    MessageBox.Show("You do not have the permission to add a new item!");
            }
            else if (FabAttrsViewModelObj.ObjStatus.IsSavedDBItem)
            {
                if (FabAttrsViewModelObj.AllowUpdate)
                    FabAttrsViewModelObj.UpdateFabricAttr();
                else
                    MessageBox.Show("You do not have the update permission");
            }
            //btnCancel_Click(null, null);
        }

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            FabAttrsViewModelObj = new FabricSetupsViewModel();
            LayoutRoot.DataContext = FabAttrsViewModelObj;
            BtnCancel.IsEnabled = true;
            BtnCancel.Visibility = Visibility.Visible;
            BtnSave.Visibility = Visibility.Visible;
            FabCategoryCombo.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FabAttrsViewModelObj = new FabricSetupsViewModel();
            LayoutRoot.DataContext = FabAttrsViewModelObj;
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnAddNewCard.IsChecked = false;
            BtnAddNewCard.Visibility = Visibility.Visible;
            BtnShowSearch.Visibility = Visibility.Visible;
            FabCategoryCombo.IsEnabled = false;
            BtnShowSearch.IsChecked = false;
            BtnSave.Visibility = Visibility.Collapsed;
            BtnPostToAx.Visibility = Visibility.Collapsed;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (FabCategoryCombo.SelectedValue != null)
            {
                SearchResultChild.Show();
                SearchResultChild.IsSearchingToDye = false;
                SearchResultChild.SearchFabricAttrs(FabAttrsViewModelObj);
            }
            else
            {
                MessageBox.Show("Not enough data for search!, you should at least chose (Fabric Category)", "Product Development Manager", MessageBoxButton.OK);
            }
        }

        private void btnLoadFabricForDying_Click(object sender, RoutedEventArgs e)
        {
            FabAttrsViewModelObj = new FabricSetupsViewModel
            {
                FabricCategoryID = (int) FabCategoryCombo.SelectedValue == 4
                    ? 2
                    : (int) FabCategoryCombo.SelectedValue == 5
                        ? 3
                        : 0
            };
            SearchResultChild.Show();
            SearchResultChild.IsSearchingToDye = true;
            SearchResultChild.SearchFabricAttrs(FabAttrsViewModelObj);
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            FabCategoryCombo.IsEnabled = true;
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            BtnAddNewCard.Visibility = Visibility.Collapsed;
            BtnSearch.Visibility = Visibility.Visible;
        }

        private void btnPostToAx_Click(object sender, RoutedEventArgs e)
        {
            FabAttrsViewModelObj.InserToAx();
        }

        private void txtSupplierRef_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {          
                FabAttrsViewModelObj
                .ContentCompositionList
                .Remove(ContentListBox.SelectedItem as ContentCompositionViewModel);         
        }

        private void btnBom_Click(object sender, RoutedEventArgs e)
        {
            var child = new FabricBom(FabAttrsViewModelObj);
            child.Show();
        }
    }
}