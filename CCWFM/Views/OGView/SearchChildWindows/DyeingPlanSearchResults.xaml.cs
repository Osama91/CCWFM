using System.Collections.Generic;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class DyeingPlanSearchResults
    {
        public DyeingPlanViewModel ViewModel;
        public DyeingPlanAccViewModel _ViewModel;

        public DyeingPlanSearchResults(DyeingPlanViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
        }

        public DyeingPlanSearchResults(DyeingPlanAccViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _ViewModel = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.HeaderViewModel.InjectFrom(DgDyeingOrderResults.SelectedItem);
                ViewModel.GetDyeingPlanOrder();
            }
            else
            {
                _ViewModel.HeaderViewModel.InjectFrom(DgDyeingOrderResults.SelectedItem);
                _ViewModel.GetDyeingPlanOrder();
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgDyeingOrderResults_OnOnFilter(object sender, FilterEvent e)
        {
            ViewModel.DyeingPlanHeaderViewModelList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            ViewModel.Filter = filter;
            ViewModel.ValuesObjects = valueObjecttemp;
            ViewModel.SearchHeader();
        }
    }
}