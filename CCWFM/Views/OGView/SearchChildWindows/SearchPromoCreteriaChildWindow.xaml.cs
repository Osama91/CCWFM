using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.PromotionViewModel;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchPromoCreteriaChildWindow 
    {
        private PromHeaderViewModel _viewModel;

        public SearchPromoCreteriaChildWindow(PromHeaderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.MainRowListCriteria.Clear();
        }


        public void Cleartextbox()
        {
            _viewModel.GlserialCriteria = 0;
            _viewModel.FromDateCriteria = null;
            _viewModel.ToDateCriteria = null;

        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMainRowCriteria != null)
            {
                //_viewModel.TransactionHeaderCriteria.InjectFrom(_viewModel.SelectedMainRowCriteria);
                ////_viewModel.TransactionHeaderCriteria.Clear();
                ////  _viewModel.TransactionHeader.StorePerRow = _viewModel.SelectedMainRow.TblStore1;
                ////  _viewModel.GetTblPromoDetail();
                //_viewModel.GetBrandsDetial();
                //_viewModel.GetStoresDetail();
                Cleartextbox();
                DialogResult = true;
                //_viewModel.TransactionHeader.inter = true;
            }
        }

        private void IserialTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowListCriteria.Clear();
                _viewModel.GetMaindataCriteria();
            }

        }

        private void FromDatePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowListCriteria.Clear();
                _viewModel.GetMaindataCriteria();
            }

        }


        private void ToDatePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.MainRowListCriteria.Clear();
                _viewModel.GetMaindataCriteria();
            }
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}