using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class BrandBudgetHeaderChildWindow
    {
        public BrandBudgetHeaderChildWindow(BrandBudgetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as BrandBudgetViewModel;
            viewModel.TransactionHeader.InjectFrom(MainGrid.SelectedItem);
            if (viewModel.Brandsection.All(x => x.Iserial != viewModel.TransactionHeader.TblLkpBrandSection))
            {
                viewModel.Brandsection.Add(new LkpData.TblLkpBrandSection().InjectFrom( viewModel.TransactionHeader.BrandSectionPerRow) as LkpData.TblLkpBrandSection);
                viewModel.TransactionHeader.BrandSectionPerRow =
                new CRUDManagerService.TblLkpBrandSection().InjectFrom(viewModel.Brandsection.FirstOrDefault(x => x.Iserial == viewModel.TransactionHeader.TblLkpBrandSection)) as CRUDManagerService.TblLkpBrandSection;
            }
                
            viewModel.TransactionHeader.DetailsList.Clear();
            viewModel.GetDetailData();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = DataContext as BrandBudgetViewModel;
            if (viewModel.MainRowList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading && viewModel.MainRowList.Count < viewModel.FullCount)
            {
                viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            var viewModel = DataContext as BrandBudgetViewModel;
            viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.Filter = filter;
            viewModel.ValuesObjects = valueObjecttemp;
            viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null,null);
        }
    }
}