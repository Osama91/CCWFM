using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchSalesOrder
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(TblSalesOrder), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty StatusPerRowProperty =
                                                                 DependencyProperty.Register("StatusPerRow", typeof(GenericTable), typeof(System.Windows.Controls.UserControl),
                                                                 new PropertyMetadata(null, OnStatusPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public TblSalesOrder SearchPerRow
        {
            get { return (TblSalesOrder)GetValue(SearchPerRowProperty); }
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

        public GenericTable StatusPerRow
        {
            get { return (GenericTable)GetValue(StatusPerRowProperty); }
            set
            {
                SetValue(StatusPerRowProperty, value);
                if (value != StatusPerRow)
                {
                    StatusPerRow = value;
                }

                RaisePropertyChanged("StatusPerRow");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchSalesOrder;
            if (args.NewValue != null)
            {
                if (fil != null) fil.SearchPerRow = args.NewValue as TblSalesOrder;
            }
        }

        public static void OnStatusPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchSalesOrder;
            if (args.NewValue != null)
            {
                if (fil != null) fil.StatusPerRow = args.NewValue as GenericTable;
            }
        }

        public SearchSalesOrder()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchSalesOrderChild(this);
            child.Show();
        }
    }
}