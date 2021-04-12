using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.UserControls.Search
{
    public partial class SearchPurchaseOrders
    {
        public DependencyProperty JournalTypePerRowProperty =
         DependencyProperty.Register("JournalType", typeof(Transactions),
             typeof(System.Windows.Controls.UserControl),
             new PropertyMetadata(null, OnJournalTypeChanged));

        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(CRUD_ManagerServicePurchaseOrderDto), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public Transactions JournalType
        {
            get { return (Transactions)GetValue(JournalTypePerRowProperty); }
            set
            {
                SetValue(JournalTypePerRowProperty, value);
                if (value != JournalType)
                {
                    JournalType = value;
                }

                RaisePropertyChanged("JournalType");
            }
        }

        public CRUD_ManagerServicePurchaseOrderDto SearchPerRow
        {
            get { return (CRUD_ManagerServicePurchaseOrderDto)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchPurchaseOrders;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as CRUD_ManagerServicePurchaseOrderDto;
            }
        }

        public static void OnJournalTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchPurchaseOrders;
            if (args.NewValue != null)
            {
                fil.JournalType = args.NewValue as Transactions;
            }
        }

        public SearchPurchaseOrders()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchPurchaseOrdersChild(this);
            child.Show();
        }
    }
}