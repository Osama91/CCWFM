using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using CCWFM.ViewModel.SMLViewModels;

namespace CCWFM.Views.StylePages
{
    public partial class NewRFQPage
    {
        public NewRFQPage()
        {
            InitializeComponent();
            var rfqSearchViewModel = LayoutRoot.DataContext as NewRFQViewModel;
            if (rfqSearchViewModel != null)
                rfqSearchViewModel.VendorPopulatingCompleted += (s, e) => VendAutoComplete.PopulateComplete();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
        }
        /// <summary>
        /// Searches for a specific control by name, in a given Parent control
        /// </summary>
        /// <param name="parent">Parent Control</param>
        /// <param name="controlName">Child control name to search for</param>
        /// <returns></returns>
        private object GetChildControl(DependencyObject parent, string controlName)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var counter = 0; counter < count; counter++)
            {
                //Get The Child Control based on Index
                Object tempObj = VisualTreeHelper.GetChild(parent, counter);

                //If Control's name Property matches with the argument control
                //name supplied then Return Control
                var dependencyObject = tempObj as DependencyObject;
                if (dependencyObject != null && dependencyObject.GetValue(NameProperty).ToString() == controlName)
                    return tempObj;
                tempObj = GetChildControl(tempObj as DependencyObject, controlName);
                if (tempObj != null)
                    return tempObj;
            }
            return null;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) == null) return;
            var temp = (sender as DataGrid);
            Dispatcher
                .BeginInvoke
                (
                    () =>
                    {
                        var itmsCtrl = (GetChildControl(temp, "IcSizes") as ItemsControl);
                        if (itmsCtrl == null) return;
                        if (temp.SelectedItem == null) return;
                        var src = (temp.SelectedItem as NewRFQDetailsViewModel);
                        if (src == null) return;
                        itmsCtrl.ItemsSource = src.tblNewRFQSizeDetails;
                    });
        }
    }
}
