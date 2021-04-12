using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System;

namespace CCWFM.Views.RequestForQutation
{
    public partial class RFQPage
    {
        public RFQPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void TextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RfqTabs.SelectedIndex = 0;
            if (!(bool) e.NewValue) return;
            var textBox = sender as TextBox;
            if (textBox != null) textBox.Focus();
        }

        private void DataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;
            var rfqFollowUpViewModel = dataGrid.SelectedItem as ViewModel.RFQViewModels.RFQFollowUpViewModel;
            if (rfqFollowUpViewModel == null || (!rfqFollowUpViewModel.IsRejected)) return;
            e.Row.Background = new SolidColorBrush(Colors.Red);
            e.Row.Foreground = new SolidColorBrush(Colors.Red);
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
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) == null) return;
            var temp = (sender as DataGrid);
            Dispatcher
                .BeginInvoke
                (
                    ()=> 
                    {
                        var itmsCtrl = (GetChildControl(temp, "IcSizes") as ItemsControl);
                        if (itmsCtrl == null) return;
                        if (temp.SelectedItem == null) return;
                        var src = (temp.SelectedItem as ViewModel.RFQViewModels.PurchasOrderDetailsViewModel);
                        if (src == null) return;
                        itmsCtrl.ItemsSource = src.PurchaseOrderSizes;
                    });
            
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }
    }
}