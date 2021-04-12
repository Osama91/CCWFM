using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchRetailStyle
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(viewstyle), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty BrandPerRowProperty =
         DependencyProperty.Register("Brand", typeof(GenericTable),
             typeof(System.Windows.Controls.UserControl),
             new PropertyMetadata(null, OnBrandChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public CCWFM.CRUDManagerService.GenericTable Brand
        {
            get { return (CCWFM.CRUDManagerService.GenericTable)GetValue(BrandPerRowProperty); }
            set
            {
                SetValue(BrandPerRowProperty, value);
                if (value != Brand)
                {
                    Brand = value;
                }

                RaisePropertyChanged("Brand");
            }
        }

        public viewstyle SearchPerRow
        {
            get { return (viewstyle)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchRetailStyle;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as viewstyle;
            }
        }

        public static void OnBrandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchRetailStyle;
            if (args.NewValue != null)
            {
                fil.Brand = args.NewValue as GenericTable;
            }
        }

        public SearchRetailStyle()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchRetailStyleChild(this);
            child.Show();
        }
    }
}