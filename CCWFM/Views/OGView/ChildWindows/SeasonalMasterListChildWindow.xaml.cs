using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SeasonalMasterListChildWindow
    {
        private readonly SeasonalMasterListViewModel _viewModel;
        private StyleHeaderViewModel StyleviewModel;
        public SeasonalMasterListChildWindow(StyleHeaderViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = new SeasonalMasterListViewModel(viewModel);
            DataContext = _viewModel;
            StyleviewModel = viewModel;
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add(row as TblSeasonalMasterListViewModel);
            }
            _viewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();

            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }


        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 >= e.Row.GetIndex() || _viewModel.Loading) return;
            _viewModel.Loading = true;
            _viewModel.GetMaindata();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblSeasonalMasterListViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }
        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {

            _viewModel.SaveMainList();
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PreviewReport();
        }

        private void TextBox_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                e.Handled = true;
                var cmb = sender as ComboBox;
                if (cmb.Items.Count > 0)
                {

                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }

        private void CalcImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.CalcRatio();
        }

        private void MainGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (StyleviewModel.SelectedMainRow.TransactionExists && _viewModel.SelectedMainRow.Iserial != 0)
            {
                e.Cancel = true;
            }
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                e.Handled = true;
                var cmb = sender as ComboBox;
                if (cmb.Items.Count > 0)
                {
                    //         cmb.SelectedItem = cmb.Items[cmb.Items.IndexOf(cmb.SelectedIndex) - 1];
                }
            }
        }


        private void BtnStoreIntial_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new StoreIntialOrderChildWindow(_viewModel);
            child.Show();
        }
    }
}