using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchVendor
    {
        public SearchVendor()
        {
            if (IsDesignTime == false)
            {
                InitializeComponent();
            }
        }

         bool IsDesignTime
        {
            get
            {
                return (Application.Current == null) ||
                       (Application.Current.GetType() == typeof(Application));
            }
        }

        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(Vendor), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty SeasonPerRowProperty =
                                                                DependencyProperty.Register("SeasonPerRow", typeof(string), typeof(System.Windows.Controls.UserControl),
                                                                new PropertyMetadata(null, OnSeasonPerRowChanged));

        public DependencyProperty ItemPerRowProperty =
                                                              DependencyProperty.Register("ItemPerRow", typeof(ItemsDto), typeof(System.Windows.Controls.UserControl),
                                                              new PropertyMetadata(null, OnItemPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public Vendor SearchPerRow
        {
            get { return (Vendor)GetValue(SearchPerRowProperty); }
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

        public string SeasonPerRow
        {
            get { return (string)GetValue(SeasonPerRowProperty); }
            set
            {
                SetValue(SeasonPerRowProperty, value);
                if (value != SeasonPerRow)
                {
                    SeasonPerRow = value;
                }

                RaisePropertyChanged("SeasonPerRow");
            }
        }


        public ItemsDto ItemPerRow
        {
            get { return (ItemsDto)GetValue(ItemPerRowProperty); }
            set
            {
                SetValue(ItemPerRowProperty, value);
                if (value != ItemPerRow)
                {
                    ItemPerRow = value;
                }

                RaisePropertyChanged("ItemPerRow");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchVendor;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as Vendor;
            }
        }

        
  public static void OnSeasonPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchVendor;
            if (args.NewValue != null)
            {
                fil.SeasonPerRow = args.NewValue as string;
            }
        }
        public static void OnItemPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchVendor;
            if (args.NewValue != null)
            {
                fil.ItemPerRow = args.NewValue as ItemsDto;
            }
        }

    

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchVendorChild(this);
            child.Show();
        }

    
    }
}