using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class FabricReservation 
    {
        ReservationViewModel _viewModel;
        StyleHeaderViewModel styleViewModel;
            public FabricReservation(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();
            ReservationViewModel resViewModel = new ReservationViewModel(styleViewModel);
            _viewModel = resViewModel;
            this.DataContext = resViewModel;
            this.styleViewModel = styleViewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveOrder();
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox
            .Show("You are about to delete Reservation Order permanently!!\nPlease note that this action cannot be undone!"
                    , "Delete", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                _viewModel.DeleteOrder();
            }
        }

        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            var reportName = "Reservation";
            if (LoggedUserInfo.CurrLang == 0)
            { reportName = "Reservation"; }
            var para = new ObservableCollection<string> { _viewModel.TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, para);
        }

        private void BtnAddNewMainRow_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.OrderLineList.Clear();
            _viewModel.GetOrderInfo();
            var childWindows = new FabricDefectsLineCreationChildWindow(_viewModel);
            childWindows.Show();
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                                  
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.saveReservationForStyle();
        }

        private void btnDeleteRow_Click(object sender, MouseButtonEventArgs e)
        {
            var deleteBtn = sender as Image;

            if (_viewModel.SelectedMainDetails.Inspected)
            {
                MessageBox.Show("Cannot Delete Inspected Row ");
            }
            else
            {
                var rowtoDelete = (TblReservationDetailsViewModel)deleteBtn.DataContext;
                _viewModel.Client.DeleteReservationDetailsAsync(rowtoDelete.Iserial);
                _viewModel.SelectedMainDetails.ReservationDetailsViewModelList.Remove(rowtoDelete);
                _viewModel.SelectedMainDetails.RemQty = _viewModel.SelectedMainDetails.RemQty + rowtoDelete.IntialQty;
                _viewModel.SelectedMainDetails.RemQtyTemp = _viewModel.SelectedMainDetails.RemQtyTemp + rowtoDelete.IntialQty;

            }
        }

        private void btnResRec_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ReservationRec();
        }
        private void DgResMainDetails_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.TransactionHeader.TransactionMainDetails.Clear();
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.Filter = _viewModel.Filter + " and ";
                }

                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetReservationMainDetail(styleViewModel);
        }

        private void DgResMainDetails_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.TransactionHeader.TransactionMainDetails.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.TransactionHeader.TransactionMainDetails.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetOrderInfo();
            }
        }

        private void BtnGenerateFromReservation_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.LinkToPlan();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (_viewModel.styleViewModel != null)
                {
                    _viewModel.getQtyRequired();
                }
            }
        }
    }
}

