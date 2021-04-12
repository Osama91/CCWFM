using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;

namespace CCWFM.UserControls.Search
{
    public partial class SearchRetailGeneric
    {

        public DependencyProperty TableNamePerRowProperty =
           DependencyProperty.Register("TableNamePerRow", typeof(string),
               typeof(System.Windows.Controls.UserControl),
               new PropertyMetadata("", OnTableNameChanged));

        public string TableNamePerRow
        {
            get { return (string)GetValue(TableNamePerRowProperty); }
            set
            {
                SetValue(TableNamePerRowProperty, value);
                if (value != TableNamePerRow)
                {
                    TableNamePerRow = value;
                }

                RaisePropertyChanged("TableNamePerRow");
            }
        }

        public static void OnTableNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchRetailGeneric;
            if (args.NewValue != null)
            {
                fil.TableNamePerRow = (string)args.NewValue;
            }
        }




        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(GenericTable), typeof(System.Windows.Controls.UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public GenericTable SearchPerRow
        {
            get { return (GenericTable)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchRetailGeneric;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as GenericTable;
            }
        }

        public SearchRetailGeneric()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchRetailGenericChild(this);
            child.Show();
        }
    }
}