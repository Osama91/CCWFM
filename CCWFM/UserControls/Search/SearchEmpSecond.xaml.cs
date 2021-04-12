using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchEmpSecond
    {
        public DependencyProperty SearchPerRowProperty =
                                   DependencyProperty.Register("SearchPerRow", typeof(EmployeesView), typeof(System.Windows.Controls.UserControl),
                                    new PropertyMetadata(null, OnSecondEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public EmployeesView SearchPerRow
        {
            get { return (EmployeesView)GetValue(SearchPerRowProperty); }
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

        public static void OnSecondEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchEmpSecond;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as EmployeesView;
            }
        }

        public SearchEmpSecond()
        {
            InitializeComponent();
            
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchEmpSecondChild(this);
            child.Show();
        }
    }
}