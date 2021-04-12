using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SalesOrderColorProduction
    {
        public StyleHeaderViewModel ViewModel;

        public SalesOrderColorProduction(StyleHeaderViewModel styleViewModel, SalesOrderType salesOrderType)
        {
            InitializeComponent();
            DataContext = styleViewModel;
            ViewModel = styleViewModel;
          
                SalesOrderColorsGrid.Visibility = Visibility.Visible;
           
            foreach (var row in SalesOrderColorsGrid.Columns.Where(x => x.DisplayIndex >= 0 && x.DisplayIndex < 9))
            {
                const int pageWidth = 650;
                row.Width = new DataGridLength(pageWidth / 9);
            }

            if (salesOrderType == SalesOrderType.SalesOrderPo)
            {
                foreach (var row in SalesOrderColorsGrid.Columns.Where(x => x.DisplayIndex > 3 && x.DisplayIndex < 9))
                {
                    row.Visibility = Visibility.Collapsed;
                }
            }

        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }


        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMainList();
        }

    }
}