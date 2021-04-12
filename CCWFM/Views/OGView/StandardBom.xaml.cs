using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView
{
    public partial class StandardBom
    {
        private readonly StandardBomViewModel _viewModel;

        public StandardBom()
        {
            InitializeComponent();

            _viewModel = new StandardBomViewModel();
            DataContext = _viewModel;

            if (_viewModel != null)
                _viewModel.ExportCompleted += (s, r) =>
                {
                    if (_viewModel.ExportGrid == MainGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("Style");
                    }

                    if (_viewModel.ExportGrid == BomGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("Bom");
                    }
                };

            BomGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "IsSupplierMaterial").Visibility = Visibility.Collapsed;

        }

        private void Bomcolor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedBomRow.ColorChangedStandardBom();
        }
        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case (int)0:
                        _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
                        break;

                    case 1:
                        _viewModel.AddBom(BomGrid.SelectedIndex != -1);
                        break;
                }
            }
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:
                        MainGrid.CommitEdit();
                        //_viewModel.SaveMainRow();
                        break;

                    case 1:

                        _viewModel.SaveMainRow();

                        break;
                }
            }
        }

        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
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
                    _viewModel.SelectedMainRows.Add(row as TblStandardBomHeaderViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void TabStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case 0:

                        //    _viewModel.SelectedMainRow = (TblStyleViewModel)DataFormStyle.CurrentItem;
                        break;

                    case 1:

                        _viewModel.GetSalesOrderBom();
                        break;
                }
            }
        }

        private void BomGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.BomList.IndexOf(_viewModel.SelectedBomRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.BomList.Count - 1))
                {
                    _viewModel.AddBom(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedBomRows.Clear();
                foreach (var row in BomGrid.SelectedItems)
                {
                    _viewModel.SelectedBomRows.Add(row as BomViewModel);
                }

                _viewModel.DeleteBom();
            }
        }

        private void BomGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
           _viewModel.SaveBom();
        }
    }
}