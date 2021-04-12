using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.PromotionViewModel;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SearchPromHeaderAll
    {
        private PromHeaderViewModel _viewModel;

        public SearchPromHeaderAll(PromHeaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.MainRowList.Clear();
        }

        public void Cleartextbox()
        {
            _viewModel.GlSerial = 0;
            _viewModel.FromCode = 0;
            _viewModel.ToCode = 0;
            _viewModel.Date = null;
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMainRow != null)
            {
                _viewModel.TransactionHeader.InjectFrom(_viewModel.SelectedMainRow);
                _viewModel.TransactionHeader.DetailsList.Clear();
                //  _viewModel.TransactionHeader.StorePerRow = _viewModel.SelectedMainRow.TblStore1;
                _viewModel.GetTblPromoDetail();
                Cleartextbox();
                DialogResult = true;

                //_viewModel.TransactionHeader.inter = true;
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

        private void Date_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }

        private void datePicker2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }

        private void ToCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }

        private void txtFromCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowList.Clear();
                _viewModel.GetMaindata();
            }
        }
    }
}