using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class GeneratePurchaseChild
    {
        private bool collect;
        public GeneratePurchaseChild(GeneratePurchaseViewModel viewModel,bool Collect=false)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.GetMaindata();
            collect = Collect;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            var viewModel = (GeneratePurchaseViewModel)DataContext;

            if (collect)
            {
                var newrow=DgResults.SelectedItem as TblGeneratePurchaseHeaderModel;
                if (newrow != null) viewModel.ApplyMergePlans(newrow.Iserial);
                //viewModel.Collect
            }
            else
            {
                viewModel.SelectedMainRow = DgResults.SelectedItem as TblGeneratePurchaseHeaderModel;
                viewModel.GetPurchaseHeader();    
            }
            
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;
            viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.Filter = filter;
            viewModel.ValuesObjects = valueObjecttemp;
            viewModel.GetMaindata();
        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;
 
            if (viewModel.MainRowList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.GetMaindata();
            }
        }
    }
}