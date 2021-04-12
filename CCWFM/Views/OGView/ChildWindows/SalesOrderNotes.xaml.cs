using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SalesOrderNotes
    {
        public StyleHeaderViewModel ViewModel;


        public SalesOrderNotes(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();

            DataContext = styleViewModel;
            ViewModel = styleViewModel;
            ViewModel.GetSalesOrderNotes();
        }

        private void SalesOrderColorsGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (ViewModel.SelectedDetailRow.SalesOrderNotesList.IndexOf(ViewModel.SelectedSalesOrderNotesRow));
                if (currentRowIndex == (ViewModel.SelectedDetailRow.SalesOrderNotesList.Count - 1))
                {
                    ViewModel.AddNewSalesOrderNotes(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                ViewModel.SelectedSalesOrderNotesRows.Clear();


                foreach (var row in SalesOrderNotesGrid.SelectedItems)
                {
                    ViewModel.SelectedSalesOrderNotesRows.Add(row as TblSalesOrderNotesModel);
                }

                ViewModel.DeleteSalesOrderNotes();
            }
        }

        private void SalesOrderNotesGridEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ViewModel.SalesOrderNotes();
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }


        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewSalesOrderNotes(SalesOrderNotesGrid.SelectedIndex != -1);
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            ViewModel.SalesOrderNotes();
        }

      
    }
}