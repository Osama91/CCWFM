using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using Os.Controls.DataGrid.Events;

namespace CCWFM.UserControls
{
    public partial class SearchOffset
    {
        private readonly SearchOffsetUserControl _userControl;
        private readonly SearchEntityViewModel _viewModel = new SearchEntityViewModel();

        public SearchOffset(SearchOffsetUserControl searchEntityUserControl)
        {
            InitializeComponent();
            if (searchEntityUserControl.JournalAccountType != null)
            {
                _viewModel.TblJournalAccountType = searchEntityUserControl.JournalAccountType.Iserial;
            }
            else
            {
                _viewModel.TblJournalAccountType = 0;
            }
            if (searchEntityUserControl.ScopePerRow != null)
            {
                _viewModel.Scope = searchEntityUserControl.ScopePerRow.Iserial;
            }
            else
            {
                _viewModel.getOnlyWithAccount = true;
                _viewModel.Scope = 0;
            }
            if (searchEntityUserControl.PreventPerRow != null)
            {
                if (searchEntityUserControl.PreventPerRow.Iserial > 0)
                {
                    _viewModel.PreventPerRow = true;
                }
            }
            else
            {
                _viewModel.PreventPerRow = false;
            }
            DataContext = _viewModel;
            _userControl = searchEntityUserControl;
            _viewModel.GetMaindata();
        }

        private bool _canBeClosed = false;

        protected override void OnOpened()
        {
            base.OnOpened();
            _canBeClosed = true;
        }

        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
            Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
            _canBeClosed = false;
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (_canBeClosed)
            {
                _userControl.SearchPerRow = MainGrid.SelectedItem as Entity;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_canBeClosed)
            {
                Close();
                //DialogResult = true;
                _userControl.SearchPerRow = null;
            }

        }


        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading &&
                _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.GetMaindata();
            }
        }

        private void MainGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _userControl.SearchPerRow = MainGrid.SelectedItem as Entity;
            DialogResult = true;
        }
    }
}