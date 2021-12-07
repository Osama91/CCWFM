using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls
{
    public partial class SearchBanks
    {
        public DependencyProperty BankPerRowProperty = DependencyProperty.Register("SearchPerRow", typeof(GlService.TblBank), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnBankPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public GlService.TblBank SearchPerRow
        {
            get { return (GlService.TblBank)GetValue(BankPerRowProperty); }
            set
            {
                SetValue(BankPerRowProperty, value);
                if (value != SearchPerRow)
                {
                    SearchPerRow = value;
                }

                RaisePropertyChanged("SearchPerRow");
            }
        }

        public static void OnBankPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchBanks;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as GlService.TblBank;
            }
        }

        public SearchBanks()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchBanksChild(this);
            child.Show();
        }
    }
}