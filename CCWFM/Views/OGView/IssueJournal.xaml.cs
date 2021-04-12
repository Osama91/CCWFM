using System.Windows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.SearchChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class IssueJournal
    {
        private readonly IssueJournalViewModel _viewModel = new IssueJournalViewModel();

        #region FormModesSettings

        private void ClearScreen()
        {            
            BtnAddNewOrder.IsChecked = false;
            _viewModel.MainRowList.Clear();
            _viewModel.SelectedMainRow = new TblIssueJournalHeaderViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtntnAddNewMainOrderDetails.IsEnabled = true;

                    break;

                case FormMode.Standby:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;

                    BtnSaveOrder.IsEnabled = false;

                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;

                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Update:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;

                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSaveOrder.IsEnabled = false;

                    break;

                case FormMode.Read:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;

                    BtnAddNewOrder.Visibility = Visibility.Collapsed;

                    BtnSaveOrder.IsEnabled = false;

                    break;
            }
        }



        private void btnSearchOrderRec_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedMainRow.SubDetailList.Clear();
            _viewModel.GetSubDetaildata();
            var childWindowSeach = new IssueJournalRecSearch(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnAddNewOrderRec_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnCancelOrderRec_Click(object sender, RoutedEventArgs e)
        {
         //   ResetModeRec();
        }    

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }
     

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new IssueJournalSearch(_viewModel);
            childWindowSeach.Show();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedIndex==0)
            {
                _viewModel.SaveMainRow(); 
            }
            else
            {
                _viewModel.SaveRecRow(); 
            }
       
        }

        #endregion FormModesSettings

        public IssueJournal()
        {
            InitializeComponent();
            DataContext = _viewModel;

            SwitchFormMode(FormMode.Add);
        }

        private void btnAddNewMainOrderDetails_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
        }

        private void btnAddNewMainOrderDetailsRec_Clicked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewRecRow();
        }    
    }
}