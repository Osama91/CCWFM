using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchGroups
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(TblGROUP1), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty TableNameProperty =
                                                                  DependencyProperty.Register("TableName", typeof(string), typeof(System.Windows.Controls.UserControl),
                                                                  new PropertyMetadata(null, OnTableNameChanged));

        private static void OnTableNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fil = d as SearchGroups;
            if (e.NewValue != null)
            {
                fil.TableName = e.NewValue as string;
            }
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public TblGROUP1 SearchPerRow
        {
            get { return (TblGROUP1)GetValue(SearchPerRowProperty); }
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

        public string TableName { get; set; }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchGroups;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblGROUP1;
            }
        }

        public SearchGroups()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchGroupsChild(this);
            child.Show();
        }
    }
}