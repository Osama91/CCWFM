using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class MarkupTransProdChildWindow
    {
        private readonly RecInvProductionViewModel _viewModel;
        private readonly RouteCardInvoiceViewModel _viewModel2;
        private readonly DyeingOrderInvoiceViewModel _viewModel3;
        private readonly SalesOrderRequestInvoiceViewModel _viewModel4;
        private readonly ProductionInvoiceViewModel _viewModel5;
        public MarkupTransProdChildWindow(RouteCardInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel2 = viewModel;
            HeaderGrid.Visibility=Visibility.Collapsed;
            
            _viewModel2.GetMarkUpdata(true);
        }

    
        public MarkupTransProdChildWindow(RecInvProductionViewModel viewModel, bool header)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            HeaderGrid.Visibility = Visibility.Collapsed;
            if (header)
            {
                _viewModel.GetMarkUpdata(true);
            }
            else
            {
                _viewModel.GetMarkUpdata(false);
            }
        }

        public MarkupTransProdChildWindow(DyeingOrderInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel3 = viewModel;
            HeaderGrid.Visibility = Visibility.Collapsed;

            _viewModel3.GetMarkUpdata(true);
        }

        public MarkupTransProdChildWindow(SalesOrderRequestInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel4 = viewModel;
            HeaderGrid.Visibility = Visibility.Collapsed;

            _viewModel4.GetMarkUpdata(true);
        }

        public MarkupTransProdChildWindow(ProductionInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel5 = viewModel;
            HeaderGrid.Visibility = Visibility.Collapsed;

            _viewModel5.GetMarkUpdata(true);
        }

        private void HeaderGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (_viewModel != null)
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel.SelectedMainRow.MarkUpTransList.IndexOf(_viewModel.SelectedMarkupRow));
                    if (currentRowIndex == (_viewModel.SelectedMainRow.MarkUpTransList.Count - 1))
                    {
                        _viewModel.AddNewMarkUpRow(true, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {

                    _viewModel.SelectedMarkupRows.Clear();
                    foreach (var row in HeaderGrid1.SelectedItems)
                    {
                        _viewModel.SelectedMarkupRows.Add(row as TblMarkupTranProdViewModel);
                    }


                    _viewModel.DeleteMarkupRow(true);
                }
            }
            else if (_viewModel2 != null)
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel2.SelectedMainRow.MarkUpTransList.IndexOf(_viewModel2.SelectedMarkupRow));
                    if (currentRowIndex == (_viewModel2.SelectedMainRow.MarkUpTransList.Count - 1))
                    {
                        _viewModel2.AddNewMarkUpRow(true, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {
                    _viewModel2.DeleteMarkupRow(true);
                }
            }

            else if (_viewModel3 != null)
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel3.SelectedMainRow.MarkUpTransList.IndexOf(_viewModel3.SelectedMarkupRow));
                    if (currentRowIndex == (_viewModel3.SelectedMainRow.MarkUpTransList.Count - 1))
                    {
                        _viewModel3.AddNewMarkUpRow(true, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {
                    _viewModel3.DeleteMarkupRow(true);
                }
            }

            else if (_viewModel4 != null)
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel4.TransactionHeader.MarkUpTransList.IndexOf(_viewModel4.SelectedMarkupRow));
                    if (currentRowIndex == (_viewModel4.TransactionHeader.MarkUpTransList.Count - 1))
                    {
                        _viewModel4.AddNewMarkUpRow(true, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {
                    _viewModel4.DeleteMarkupRow(true);
                }
            }
            else if (_viewModel5 != null)
            {
                if (e.Key == Key.Down)
                {
                    var currentRowIndex = (_viewModel5.TransactionHeader.MarkUpTransList.IndexOf(_viewModel5.SelectedMarkupRow));
                    if (currentRowIndex == (_viewModel5.TransactionHeader.MarkUpTransList.Count - 1))
                    {
                        _viewModel5.AddNewMarkUpRow(true, true);
                    }
                }
                else if (e.Key == Key.Delete)
                {
                    _viewModel5.DeleteMarkupRow(true);
                }
            }
        }

        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.SaveMarkupRow();
            }
            else if (_viewModel2 != null)
            {
                _viewModel2.SaveMarkupRow();
            }
            else if (_viewModel3 != null)
            {
                _viewModel3.SaveMarkupRow();
            }
            else if (_viewModel4 != null)
            {
                _viewModel4.SaveMarkupRow();
            }
            else if (_viewModel5 != null)
            {
                _viewModel5.SaveMarkupRow();
            }
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                foreach (var variable in e.RemovedItems)
                {
                    _viewModel.SaveMarkupRowOldRow((TblMarkupTranProdViewModel)variable);
                }
            }
            else if (_viewModel2 != null)
            {
                foreach (var variable in e.RemovedItems)
                {
                    _viewModel2.SaveMarkupRowOldRow((TblMarkupTranProdViewModel)variable);
                }
            }
            else if (_viewModel3 != null)
            {
                foreach (var variable in e.RemovedItems)
                {
                    _viewModel3.SaveMarkupRowOldRow((TblMarkupTranProdViewModel)variable);
                }
            }
            else if (_viewModel4 != null)
            {
                foreach (var variable in e.RemovedItems)
                {
                    _viewModel4.SaveMarkupRowOldRow((TblMarkupTranProdViewModel)variable);
                }
            }
            else if (_viewModel5 != null)
            {
                foreach (var variable in e.RemovedItems)
                {
                    _viewModel5.SaveMarkupRowOldRow((TblMarkupTranProdViewModel)variable);
                }
            }
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }
    }
}