using System.ComponentModel;
using System.Windows;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.UserControls.Search
{
    public partial class SearchAsset
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(TblAssetsViewModel), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public TblAssetsViewModel SearchPerRow
        {
            get { return (TblAssetsViewModel)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchAsset;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblAssetsViewModel;
            }
        }

        public SearchAsset()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchAssetChild(this);
            child.Show();
        }
    }
}