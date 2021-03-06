using CCWFM.PurchasePlanService;
using System.ComponentModel;
using System.Windows;


namespace CCWFM.UserControls.Search
{
    public partial class SearchGeneratePurchase
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(TblGeneratePurchaseHeader), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public TblGeneratePurchaseHeader SearchPerRow
        {
            get { return (TblGeneratePurchaseHeader)GetValue(SearchPerRowProperty); }
            set
            {
                SetValue(SearchPerRowProperty, value);
                if (value != SearchPerRow)
                {
                    SearchPerRow = value;
                }

                RaisePropertyChanged("SearchPerRow");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchGeneratePurchase;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as PurchasePlanService.TblGeneratePurchaseHeader;
            }
        }

        public SearchGeneratePurchase()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchGeneratePurchaseChild(this);
            child.Show();
        }
    }
}