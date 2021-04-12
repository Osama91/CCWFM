using System;
using System.Linq;
using System.Windows;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.ViewModel.RouteCardViewModelClasses;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;
using _proxy = CCWFM.CRUDManagerService;

namespace CCWFM.Views
{
    public partial class RouteCardSearchResultChild
    {
        private readonly RouteHeaderSearchViewModel _viewModel = new RouteHeaderSearchViewModel();

        private readonly RouteCardHeaderViewModel _routeCardHeader;

       public event EventHandler SubmitSearchAction;
        private readonly bool _inspection;

        public RouteCardSearchResultChild(RouteCardHeaderViewModel myViewModel, bool Inspections)
        {
            InitializeComponent();
            DataContext = _viewModel;
            _routeCardHeader = myViewModel;
            _inspection = Inspections;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (_inspection)
            {
                try
                {
                    var selectedrow = MainGrid.SelectedItem as _proxy.RouteCardHeader;

                    _routeCardHeader.RouteCardDetails.Clear();
                    _routeCardHeader.LinkIserial = selectedrow.Iserial;
                    var objectindexlist = selectedrow.RouteCardDetails.Select(x => x.ObjectIndex);

                    foreach (var variable in objectindexlist)
                    {
                        var savedrow = selectedrow.RouteCardDetails.FirstOrDefault(x => x.ObjectIndex == variable);

                        var temp = (RouteCardViewModel)new RouteCardViewModel().InjectFrom(savedrow);

                        if (savedrow != null)
                        {
                            temp.ColorPerRow = savedrow.TblColor1;
                            temp.SalesOrderPerRow = savedrow.TblSalesOrder1;
                        }

                        foreach (var VARIABLE in _viewModel.Sizes.Where((x => x.TblSizeGroup == savedrow.TblSalesOrder1.TblStyle1.TblSizeGroup)).OrderBy(x => x.Id).Distinct())
                        {
                            foreach (var item in selectedrow.RouteCardDetails.OrderBy(x => x.Iserial).Where(x => x.ObjectIndex == variable && x.Size == VARIABLE.SizeCode).ToList())
                            {
                                if (item.SizeQuantity != null)
                                {
                                    var newSize = new RoutCardSizeInfo
                                    {
                                        SizeCode = item.Size,
                                        SizeConsumption = (int)item.SizeQuantity,
                                        IsTextBoxEnabled = true
                                    };
                                    temp.RoutCardSizes.Add(newSize);
                                }
                            }
                        }

                        temp.RowTotal = temp.RoutCardSizes.Sum(x => x.SizeConsumption);
                        if (_routeCardHeader.RouteCardDetails.All(x => x.ObjectIndex != variable))
                        {
                            _routeCardHeader.RouteCardDetails.Add(temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            }
            else
            {


                RouteCardMappers.MapToViewModel(MainGrid.SelectedItem as RouteCardService.RouteCardHeader, _routeCardHeader);
                if (_viewModel.VendorList!=null)
                {
                    if (_viewModel.VendorList.Any(w => w.vendor_code == _routeCardHeader.VendorCode))
                    {
                        _routeCardHeader.VendorPerRow = new _proxy.Vendor().InjectFrom(_viewModel.VendorList.FirstOrDefault(w => w.vendor_code == _routeCardHeader.VendorCode)) as _proxy.Vendor;

                    }
                }
              
                _routeCardHeader.LoadRouteDetails();
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OKButton_Click(null, null);
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata(_inspection);
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
        }

        private void MainGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetMaindata(_inspection);
            }
        }
    }
}