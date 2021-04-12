using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.PromotionViewModel;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchGiftsChildWindow 
    {
        private PromoHeaderGifts _viewModel;
        public SearchGiftsChildWindow(PromoHeaderGifts viewModel)
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


        }




        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedMainRow == null)
            {
            _viewModel.SelectedMainRow=    MainGrid.SelectedItem as TblPromoHeaderGifts;
            }
            if (_viewModel.SelectedMainRow != null)
            {
                _viewModel.TransactionHeader.InjectFrom(_viewModel.SelectedMainRow);
                _viewModel.TransactionHeader.Enabled = true;
                _viewModel.TransactionHeader.DetailsList.Clear();
                //  _viewModel.TransactionHeader.StorePerRow = _viewModel.SelectedMainRow.TblStore1;
                _viewModel.GetTblPromoDetail();
                _viewModel.GetStores();
                Cleartextbox();
                DialogResult = true;
                
                //_viewModel.TransactionHeader.inter = true;
            }
        }

    

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
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

