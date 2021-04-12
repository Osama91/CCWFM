using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class MarkersView
    {
        private readonly MarkerViewModel _viewModel;

        public MarkersView()
        {
            InitializeComponent();
            
            _viewModel = (MarkerViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
            _viewModel.SubmitSearchAction += ViewModel_SubmitSearchAction;
            SwitchFormMode(FormMode.Add);
        }

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        #region FormModesSettings

        public enum FormMode
        {
            Standby,
            Search,
            Add,
            Update,
            Read
        }

        private void ClearScreen()
        {
            BtnEdit.IsChecked = false;
            BtnAddNewCard.IsChecked = false;
            _viewModel.MarkerHeader = new MarkerHeaderListViewModel { TransDate = DateTime.Now };
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();

                    BtnSave.IsEnabled = true;
                    BtnShowSearch.Visibility = Visibility.Collapsed;
                    BtnPrintPreview.IsEnabled = false;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnCancel.IsEnabled = true;

                    break;

                case FormMode.Standby:
                    BtnAddNewCard.IsEnabled = true;
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = true;
                    BtnSave.IsEnabled = false;
                    BtnEdit.IsEnabled = false;
                    BtnShowSearch.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = true;
                    BtnPrintPreview.IsEnabled = false;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtnAddNewCard.IsEnabled = false;
                    BtnAddNewCard.Visibility = Visibility.Collapsed;
                    BtnSave.IsEnabled = false;
                    BtnShowSearch.IsEnabled = false;
                    BtnPrintPreview.IsEnabled = false;
                    break;

                case FormMode.Update:              
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSave.IsEnabled = false;
                    BtnPrintPreview.IsEnabled = false;
                    break;

                case FormMode.Read:                  
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = false;
                    BtnSave.IsEnabled = true;
                    BtnEdit.IsEnabled = true;
                    BtnPrintPreview.IsEnabled = true;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnCancel.IsEnabled = true;
                  
                    break;
            }
        }     

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;         
            BtnShowSearch.IsChecked = false;
        }

        private FormMode _formMode;

        public FormMode _FormMode
        {
            get { return _formMode; }
            set { _formMode = value; }
        }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            var childWindowSeach = new MarkerHeaderListSearchResultsChild(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        #endregion FormModesSettings
      
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SavingHeader();
        }     

        private CuttingOrderChildWindow _cuttingView;

        private void btnCuttingOrder_Click(object sender, RoutedEventArgs e)
        {
            _cuttingView = new CuttingOrderChildWindow(_viewModel);
            _viewModel.FillCuttingOrder();

            _cuttingView.Show();

            _cuttingView.SubmitClicked += CuttingView_SubmitClicked;
        }

        private void CuttingView_SubmitClicked(object sender, EventArgs e)
        {
            foreach (var variable in _cuttingView.CuttingOrderListChildWindows)
            {
                variable.RollAssignedQty = variable.StoreRollQty;
                _viewModel.SelectedMarker.SavedCuttingOrderlist.Add(variable);
            }          
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            var reportChild = new ReportsChildWindow("Marker", _viewModel.MarkerHeader.Iserial);
            reportChild.Show();

        }

        private void btnInsertPicingList_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PickingList();
        }

        private void btnDelete_Click(object sender, MouseButtonEventArgs e)
        {
            var deleteImage = sender as Image;

            _viewModel.DeleteMarkerDetails((MarkerListViewModel)deleteImage.DataContext);
        }

        private void BtnSizes_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new MarkerSizeChildWindow(_viewModel);
            child.Show();
        }

        private void DgCuttingDetails_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveCuttingOrder(DgCuttingDetails.SelectedItem as CuttingOrderViewModel);
        }

        private void DgMarkers_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.MarkerHeader.MarkerListViewModelList.Add(new MarkerListViewModel());
            }
        }

        private void DgCuttingDetails_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var row = DgCuttingDetails.SelectedItem as CuttingOrderViewModel;

                _viewModel.SelectedMarker.SavedCuttingOrderlist.Remove(row);
                _viewModel.DeleteCutting(row);
            }
        }

        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            var child = new MarkerCalculationsChildWindow(_viewModel);
            child.Show();
        }
    }
}