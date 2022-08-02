using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using System;

namespace CCWFM.UserControls.Search
{
    public partial class SearchFabricAcc
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(ItemsDto), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));


        public DependencyProperty SalesOrderPerRowProperty =
                                                               DependencyProperty.Register("SalesOrderPerRow", typeof(int), typeof(System.Windows.Controls.UserControl),
                                                               new PropertyMetadata(0, OnSalesOrderPerRowChanged));
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public ItemsDto SearchPerRow
        {
            get { return (ItemsDto)GetValue(SearchPerRowProperty); }
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

        public int SalesOrderPerRow
        {
            get { return (int)GetValue(SalesOrderPerRowProperty); }
            set
            {
                SetValue(SalesOrderPerRowProperty, value);
                if (value != SalesOrderPerRow)
                {
                    SalesOrderPerRow = value;
                }

                RaisePropertyChanged("SalesOrderPerRow");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchFabricAcc;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as ItemsDto;
            }
        }

        public static void OnSalesOrderPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchFabricAcc;
            if (args.NewValue != null)
            {
                    fil.SalesOrderPerRow = Convert.ToInt32( args.NewValue.ToString());
                
               
            }
        }
        

        public SearchFabricAcc()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchFabricAccChild(this);
            child.Show();
        }
    }
}