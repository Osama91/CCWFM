using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.FabricToolsViewModels;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class FabricBom
    {
        public FabricBomViewModel ViewModel;

        public FabricBom(FabricSetupsViewModel fabricViewModel)
        {
            InitializeComponent();
            ViewModel = new FabricBomViewModel(fabricViewModel);
            DataContext = ViewModel;

            ViewModel.GetMaindata();
        }

        public FabricBom(FabricSetupsWFViewModel fabricViewModel)
        {
            InitializeComponent();
            ViewModel = new FabricBomViewModel(fabricViewModel);
            DataContext = ViewModel;

            ViewModel.GetMaindata();
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                ViewModel.SelectedMainRows.Add(row as TblFabricBomViewModel);
            }
            ViewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            ViewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            ViewModel.Filter = filter;
            ViewModel.ValuesObjects = valueObjecttemp;
            ViewModel.GetMaindata();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.MainRowList.IndexOf(ViewModel.SelectedMainRow));
                if (currentRowIndex == (ViewModel.MainRowList.Count - 1))
                {
                    ViewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                ViewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    ViewModel.SelectedMainRows.Add(row as TblFabricBomViewModel);
                }

                ViewModel.DeleteMainRow();
            }
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (ViewModel.MainRowList.Count < ViewModel.PageSize)
            {
                return;
            }
            if (ViewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !ViewModel.Loading)
            {
                ViewModel.Loading = true;
                ViewModel.GetMaindata();
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }
    }
}