using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class DyeingPLanServicesChildWindows
    {
        readonly int _iserial = 0;
        public DyeingPLanServicesChildWindows(DyeingPlanViewModel viewModel,int iserial)
        {
            InitializeComponent();
            DataContext = viewModel;
            _iserial = iserial;
        }
        public DyeingPLanServicesChildWindows(DyeingOrderViewModel viewModel, int iserial)
        {
            InitializeComponent();
            DataContext = viewModel;
            _iserial = iserial;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
         var viewModel = DataContext as DyeingPlanViewModel;
            if (viewModel != null)
            {
            
            viewModel.SaveServices(_iserial);
            }
            else
            {
              var   viewModelq = DataContext as DyeingOrderViewModel;
             viewModelq.SaveServices(_iserial);
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

