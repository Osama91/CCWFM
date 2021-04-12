using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchFabricService
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(ItemsDto), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

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

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchFabricService;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as ItemsDto;
            }
        }

        public SearchFabricService()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchFabricServiceChild(this);
            child.Show();
        }
    }
}