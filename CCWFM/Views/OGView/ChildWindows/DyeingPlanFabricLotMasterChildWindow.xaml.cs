using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class DyeingPlanFabricLotMasterChildWindow
    {
        public DyeingPlanViewModel ViewModel;
        public DyeingPlanAccViewModel _ViewModel;
        public DyeingPlanFabricLotMasterChildWindow(DyeingPlanViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
        }

        public DyeingPlanFabricLotMasterChildWindow(DyeingPlanAccViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _ViewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnFabricLotDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                var childWindow = new DyeingPlanFabricLotDetailsChildWindow(_ViewModel);
                childWindow.Show();
            }
            else
            {
                var childWindow = new DyeingPlanFabricLotDetailsChildWindow(ViewModel);
                childWindow.Show();
            }          
        }

        private void CmbFabricLot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.SelectedItem != null)
                {
                    var fabricStorage = cb.SelectedItem as FabricStorage_Result;
                    var row = Dg.SelectedItem as TblDyeingPlanLotsMasterViewModel;
                    if (fabricStorage != null)
                    {
                        row.FromColor = fabricStorage.CONFIGID;
                        row.AvaliableQuantityOrg = (double) fabricStorage.QuantityPerMeter;
                    }
                    if (ViewModel == null)
                    {
                        _ViewModel.CalcfabricAgain(row.FabricCode);
                    }
                    else
                    {
                        ViewModel.CalcfabricAgain(row.FabricCode);
                    }
                }
            }
        }
    }
}