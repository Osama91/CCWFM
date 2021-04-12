using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class IssueJournalRecSearch
    {
        public IssueJournalRecSearch(IssueJournalViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (IssueJournalViewModel)DataContext;
            viewModel.SelectedRecRow = DgResults.SelectedItem as TblIssueJournalReceiveHeaderViewModel;
            viewModel.GetSubDetaildata();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DgResults_OnOnFilter(object sender, FilterEvent e)
        {
            var viewModel = (IssueJournalViewModel)DataContext;
            viewModel.SelectedMainRow.SubDetailList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            viewModel.DetailSubFilter = filter;
            viewModel.DetailSubValuesObjects = valueObjecttemp;
            viewModel.GetSubDetaildata();
        }

        private void DgResults_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var viewModel = (IssueJournalViewModel)DataContext;
 
            if (viewModel.SelectedMainRow.SubDetailList.Count < viewModel.PageSize)
            {
                return;
            }
            if (viewModel.SelectedMainRow.SubDetailList.Count - 2 < e.Row.GetIndex() && !viewModel.Loading)
            {
                viewModel.Loading = true;
                viewModel.GetSubDetaildata();
            }
        }
    }
}