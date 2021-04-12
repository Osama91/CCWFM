using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.Views.RequestForQutation
{
    public partial class ItemsGetterChild
    {

        #region [ Events ]

        public event EventHandler<RfqItemsReturnEventArgs> ItemChosen;

        protected virtual void OnItemChosen(ItemsDto dto)
        {
            var handler = ItemChosen;
            if (handler != null) handler(this, new RfqItemsReturnEventArgs(dto));
        }

        #endregion [ Events ]
        public ItemsGetterChild()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OnItemChosen(ResultGrid.SelectedItem as ItemsDto);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitiateSearch(); 
            }
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnItemChosen(ResultGrid.SelectedItem as ItemsDto);
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitiateSearch();
        }

        private void InitiateSearch()
        {
            Progress.Visibility = Visibility.Visible;
            var itemsSearchViewModel = LayoutRoot.DataContext as ItemsSearchViewModel;
            if (itemsSearchViewModel == null) return;
                itemsSearchViewModel.SearchCommand.Execute(null);
            itemsSearchViewModel.SearchEnded += (s, e) =>
            {
                Progress.Visibility = Visibility.Collapsed;
            };
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }

    public class RfqItemsReturnEventArgs : EventArgs
    {
        public ItemsDto Itm { get; private set; }

        public RfqItemsReturnEventArgs(ItemsDto dto)
        {
            Itm = dto;
        }
    }
}

