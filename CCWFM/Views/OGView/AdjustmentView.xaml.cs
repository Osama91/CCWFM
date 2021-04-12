using CCWFM.ViewModel.OGViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Views.OGView
{
    public partial class AdjustmentView : Page
    {
       public bool hasCost = false;
        public AdjustmentView(bool hasCost)
        {
            this.hasCost = hasCost;
            InitializeComponent();
            var vm = ((FrameworkElement)(LayoutRoot)).DataContext as AdjustmentViewModel;
            vm.isOpenningBalance = hasCost;
        }

        private void OsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = ((DataGrid)sender);
            grid.Columns.FirstOrDefault(c =>
                c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(Cost))
                .Visibility = hasCost ? Visibility.Visible : Visibility.Collapsed;
        }
        
    }
}
