using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.ViewModel.Gl;

namespace CCWFM.Views.Gl
{
    public partial class InventPosting
    {
        private readonly InventPostingViewModel _viewModel;

        public InventPosting()
        {
            InitializeComponent();
            _viewModel = (InventPostingViewModel) LayoutRoot.DataContext;
            DataContext = _viewModel;

            MainGrid.Columns.First(x => x.SortMemberPath == "SupCustScope").Visibility = Visibility.Visible;
            MainGrid.Columns.First(x => x.SortMemberPath == "SupCustRelation").Visibility = Visibility.Visible;
            _viewModel.JournalAccountTypePerRow.Iserial = 1;
       
        }

        private void BtnDeleteMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedMainRows.Add((TblInventPostingViewModel)row);
            }
            _viewModel.DeleteMainRow();
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add((TblInventPostingViewModel) row);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid.CommitEdit();
        }

        private void MainGrid_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportGrid = sender as DataGrid;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var
            radiobtn= sender as RadioButton;
            if (radiobtn != null)
            {
                var row =radiobtn.DataContext as TblInventAccountType;
                if (row != null) _viewModel.TblInventAccountType = row.Iserial;
            }
            _viewModel.GetMaindata();
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (Tab != null && Tab.SelectedItem!=null)
            {
                _viewModel.MainRowList.Clear();
var selectedtab = Tab.SelectedItem as TabItem;
                MainGrid.Columns.First(x => x.SortMemberPath == "SupCustScope").Visibility=Visibility.Collapsed;
                MainGrid.Columns.First(x => x.SortMemberPath == "SupCustRelation").Visibility = Visibility.Collapsed;
                if (selectedtab.Name=="SalesOrder")
                {
                    MainGrid.Columns.First(x => x.SortMemberPath == "SupCustScope").Visibility = Visibility.Visible;
                    MainGrid.Columns.First(x => x.SortMemberPath == "SupCustRelation").Visibility = Visibility.Visible;
                    _viewModel.JournalAccountTypePerRow.Iserial = 1;
                }

                else if (selectedtab.Name == "Purchase")
                {
                    MainGrid.Columns.First(x => x.SortMemberPath == "SupCustScope").Visibility = Visibility.Visible;
                    MainGrid.Columns.First(x => x.SortMemberPath == "SupCustRelation").Visibility = Visibility.Visible;
                    _viewModel.JournalAccountTypePerRow= new GenericTable(){Iserial = 2};
                }
                else
                {
                    _viewModel.JournalAccountTypePerRow.Iserial = 0;
                }
                if (_viewModel.InventPostingTypeList != null)
                    _viewModel.Items = new ObservableCollection<TblInventAccountType>(_viewModel.InventPostingTypeList.Where(x => x.TabName == selectedtab.Name));
            }
        }
    }
}