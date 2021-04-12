using System.ComponentModel;
using System.Windows;
using CCWFM.GlService;

namespace CCWFM.UserControls
{
    public partial class SearchSequenceUserControl
    {
        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblSequence),
                typeof(System.Windows.Controls.UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public SearchSequenceUserControl()
        {
            InitializeComponent();
        }

        public TblSequence SearchPerRow
        {
            get { return (TblSequence)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchSequenceUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblSequence;
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchSequence(this);
            child.Show();
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged
    }
}