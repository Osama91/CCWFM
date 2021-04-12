using System.Linq;
using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class DyeingPlanFabricLotDetailsChildWindow
    {
        private readonly DyeingPlanViewModel ViewModel;
        private readonly DyeingPlanAccViewModel _ViewModel;

        public DyeingPlanFabricLotDetailsChildWindow(DyeingPlanViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
        }

        public DyeingPlanFabricLotDetailsChildWindow(DyeingPlanAccViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _ViewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (ViewModel != null)
            {
                if (ViewModel.SelectedLotsMasterRow != null)
                {
                    ViewModel.SelectedLotsMasterRow.RequiredQuantity =
                        ViewModel.SelectedLotsMasterRow.LotsDetailsList.Where(x => x.Saved).Sum(x => x.RequiredQuantity);
                    ViewModel.CalcfabricAgain(ViewModel.SelectedLotsMasterRow.FabricCode);
                }
            }
            else
            {
                if (_ViewModel.SelectedLotsMasterRow != null)
                {
                    _ViewModel.SelectedLotsMasterRow.RequiredQuantity =
                        _ViewModel.SelectedLotsMasterRow.LotsDetailsList.Where(x => x.Saved).Sum(x => x.RequiredQuantity);
                    _ViewModel.CalcfabricAgain(_ViewModel.SelectedLotsMasterRow.FabricCode);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}