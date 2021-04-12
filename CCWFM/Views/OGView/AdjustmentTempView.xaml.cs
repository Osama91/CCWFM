using CCWFM.ViewModel.OGViewModels.ControlsOverride;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Views.OGView
{
    public partial class AdjustmentTempView : ChildWindowsOverride
    {
        public bool hasCost = false;
        public AdjustmentTempView(bool hasCost)
        {
            this.hasCost = hasCost;
            InitializeComponent();
        }

        private void osGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = ((DataGrid)sender);
            grid.Columns.FirstOrDefault(c =>
                c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(Cost)).Visibility = hasCost ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
