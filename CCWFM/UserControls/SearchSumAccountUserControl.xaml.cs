using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls
{
    public partial class SearchSumAccountUserControl
    {
        public DependencyProperty ChildOnlyPerRowProperty =
            DependencyProperty.Register("ChildOnlyPerRow", typeof(bool),
                typeof(System.Windows.Controls.UserControl),
                new PropertyMetadata(false, OnChildOnlyChanged));

        public bool ChildOnlyPerRow
        {
            get { return (bool)GetValue(ChildOnlyPerRowProperty); }
            set
            {
                SetValue(ChildOnlyPerRowProperty, value);
                if (value != ChildOnlyPerRow)
                {
                    ChildOnlyPerRow = value;
                }

                RaisePropertyChanged("ChildOnlyPerRow");
            }
        }

        public static void OnChildOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchSumAccountUserControl;
            if (args.NewValue != null)
            {
                fil.ChildOnlyPerRow = (bool)args.NewValue;
            }
        }

        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblAccount),
                typeof(System.Windows.Controls.UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public TblAccount SearchPerRow
        {
            get { return (TblAccount)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchSumAccountUserControl;
            if (args.NewValue != null)
            {
                if (fil != null) fil.SearchPerRow = args.NewValue as TblAccount;
            }
        }

        public SearchSumAccountUserControl()
        {
            InitializeComponent();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchSumAccount(this);
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

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;

            var currencyClient = new GlServiceClient();
            currencyClient.GetTblAccountbyCodeCompleted += (s, sv) =>
            { if (sv.Result != null) SearchPerRow = sv.Result; };
            if (txt.Text != null)
                currencyClient.GetTblAccountbyCodeAsync(txt.Text, LoggedUserInfo.DatabasEname, ChildOnlyPerRow);
        }
    }
}