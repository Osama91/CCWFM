using System.Windows;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StyleTNAColor
    {
        public StyleHeaderViewModel ViewModel;

        public StyleTNAColor(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();
            DataContext = styleViewModel;
            ViewModel = styleViewModel;
            ViewModel.AddNewStyleTNAColorDetailRow(true);
        }

        private void SalesOrderColorsGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.SelectedTnaRow.StyleTNAColorDetailList.IndexOf(ViewModel.SelectedStyleTNAColorDetailRow));
                if (currentRowIndex == (ViewModel.SelectedTnaRow.StyleTNAColorDetailList.Count - 1))
                {
                    ViewModel.AddNewStyleTNAColorDetailRow(true);
                }
            }
       }
        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewStyleTNAColorDetailRow(SalesOrderColorsGrid.SelectedIndex != -1);
        }
        private void BtnTnaApprovals_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new StyleTNAStatus(ViewModel);
            child.Show();
        }
    }
}