using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class PurchaseOrderDetailChild
    {
        private readonly int _temp;
        public PurchaseOrderDetailChild(GeneratePurchaseViewModel viewModel, int i)
        {
            InitializeComponent();
            DataContext = viewModel;
            _temp = i;
            if (i==1)
            {
                DetailGrid2.Visibility= Visibility.Collapsed;
                viewModel.GetDetailData();
        
            }
            else
            {
                DetailGrid.Visibility = Visibility.Collapsed;
                viewModel.GetDetail2Data();
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;
            if (_temp == 1)
            {
                viewModel.SelectedPurchaseRow.DetailsList.Clear();
                string filter;
                Dictionary<string, object> valueObjecttemp;
                GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
                viewModel.DetailFilter = filter;
                viewModel.DetailValuesObjects = valueObjecttemp;
                viewModel.GetDetailData();
            }
            else
            {
                viewModel.SelectedPurchaseRow.DetailsList.Clear();
                string filter;
                Dictionary<string, object> valueObjecttemp;
                GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
                viewModel.DetailFilter = filter;
                viewModel.DetailValuesObjects = valueObjecttemp;
                viewModel.GetDetail2Data();
            }
        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;


            if (_temp == 1)
            {
                if (viewModel.SelectedMainRow.PurchaseOrderList.Count < viewModel.PageSize)
                {
                    return;
                }
                if (viewModel.SelectedMainRow.PurchaseOrderList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
                {
                    viewModel.Loading = true;


                    viewModel.GetDetailData();
                }
            }
            else
            {
                if (viewModel.SelectedPurchaseRow.DetailsList.Count < viewModel.PageSize)
                {
                    return;
                }
                if (viewModel.SelectedPurchaseRow.DetailsList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
                {
                    viewModel.Loading = true;


                    viewModel.GetDetail2Data();
                }
            }

          
        }

        private void DgResults_OnKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;

            if (e.Key == Key.Delete)
            {
                viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    viewModel.SelectedDetailRows.Add(row as TblPurchaseOrderDetailViewModel);
                }

                viewModel.DeleteDetailRow();
            }
            else
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (viewModel.SelectedPurchaseRow.DetailsList.IndexOf(viewModel.SelectedDetailRow));
                    if (currentRowIndex == (viewModel.SelectedPurchaseRow.DetailsList.Count - 1))
                    {
                        viewModel.AddNewPurchaseDetail(true);
                    }
                }
        }

        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;
            if (_temp==1)
            {
                viewModel.SaveDetailRow();
                
            }
            else
            {
                viewModel.SaveDetail2Row();
            }
          
        }

        private void BtnLink_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new PurchaseOrderBomChild(DataContext as GeneratePurchaseViewModel);
            child.Show();
        }

        private void DetailGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var viewModel = (GeneratePurchaseViewModel)DataContext;
            if (viewModel.SelectedPurchaseRow.Status)
            {
                e.Cancel = true;
            }
        }
    }
}