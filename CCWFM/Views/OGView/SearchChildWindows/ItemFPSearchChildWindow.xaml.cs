using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class ItemFPSearchChildWindow
    {
        private ItemFPSearchViewModel _viewModel;
        public ItemFPSearchChildWindow(ItemFPSearchViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        //bool isTransfer = true;
        //public bool IsTransfer
        //{
        //    get { return isTransfer; }
        //    set
        //    {
        //        isTransfer = value;
        //        SelectedItems.Columns.FirstOrDefault(c =>
        //        c.SortMemberPath == "To").Visibility
        //        = value ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        //string quantityTitle;
        //public string QuantityTitle
        //{
        //    get { return quantityTitle; }
        //    set { quantityTitle = value; 
        //        SelectedItems.Columns.FirstOrDefault(c =>
        //        c.SortMemberPath == "Quantity").Header = value; }
        //}

      
    }
}

