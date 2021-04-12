using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchBankDeposit
    {
        private DepositViewModel _viewModel;

        public SearchBankDeposit(DepositViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.MainRowList.Clear();
        }

        public void Cleartextbox()
        {
            _viewModel.serial = "";
            _viewModel.Code = "";
            _viewModel.ddate = null;
            _viewModel.StorEname = "";
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMainRow != null)
            {
                _viewModel.TransactionHeader.InjectFrom(_viewModel.SelectedMainRow);
                _viewModel.TransactionHeader.StorePerRow = _viewModel.SelectedMainRow.TblStore1;
                Cleartextbox();
                DialogResult = true;
                _viewModel.TransactionHeader.inter = true;
            }
        }

        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }

        //private void Date_KeyDown(object sender, KeyEventArgs e)
        //{
        //}

        private void Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
        }

        private void txtcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }
    }
}