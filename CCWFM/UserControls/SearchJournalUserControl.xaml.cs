using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls
{
    public partial class SearchJournalUserControl
    {
        private GlService.GlServiceClient Glclient = new GlServiceClient();

        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblJournal),
                typeof(System.Windows.Controls.UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public SearchJournalUserControl()
        {
            InitializeComponent();
            Glclient.FindJournalByCodeCompleted += (s, sv) =>
            {
                SearchPerRow = sv.Result;
            };
        }

        public TblJournal SearchPerRow
        {
            get { return (TblJournal)GetValue(SearchPerRowProperty); }
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
            var fil = sender as SearchJournalUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblJournal;
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchJournal(this);
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

            if (!string.IsNullOrEmpty(txt.Text))
                Glclient.FindJournalByCodeAsync(txt.Text, LoggedUserInfo.DatabasEname);
        }
    }
}