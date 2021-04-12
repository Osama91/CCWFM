using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls
{
    public partial class SearchCostCenterUserControl
    {
        private GlServiceClient Glclient = new GlServiceClient();

        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblCostCenter),
                typeof(UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty JournalAccountTypePerRowProperty =
         DependencyProperty.Register("JournalAccountType", typeof(GenericTable),
             typeof(UserControl),
             new PropertyMetadata(null, OnJournalAccountTypeChanged));

        public DependencyProperty EntityAccountPerRowProperty =
       DependencyProperty.Register("EntityAccount", typeof(Entity),
           typeof(UserControl),
           new PropertyMetadata(null, OnEntityAccountChanged));

        public DependencyProperty EntityAccountTypePerRowProperty =
    DependencyProperty.Register("EntityAccountType", typeof(GenericTable),
        typeof(UserControl),
        new PropertyMetadata(null, OnEntityAccountTypeChanged));

        public SearchCostCenterUserControl()
        {
            InitializeComponent();
            Glclient.GetTblCostCenterByCodeCompleted += (s, sv) =>
            {
                SearchPerRow = sv.Result;
            };
        }

        public TblCostCenter SearchPerRow
        {
            get { return (TblCostCenter)GetValue(SearchPerRowProperty); }
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

        public GenericTable JournalAccountType
        {
            get { return (GenericTable)GetValue(JournalAccountTypePerRowProperty); }
            set
            {
                SetValue(JournalAccountTypePerRowProperty, value);
                if (value != JournalAccountType)
                {
                    JournalAccountType = value;
                }

                RaisePropertyChanged("JournalAccountType");
            }
        }

        public Entity EntityAccount
        {
            get { return (Entity)GetValue(EntityAccountPerRowProperty); }
            set
            {
                SetValue(EntityAccountPerRowProperty, value);
                if (value != EntityAccount)
                {
                    EntityAccount = value;
                }

                RaisePropertyChanged("EntityAccount");
            }
        }

        public GenericTable EntityAccountType
        {
            get { return (GenericTable)GetValue(EntityAccountTypePerRowProperty); }
            set
            {
                SetValue(EntityAccountTypePerRowProperty, value);
                if (value != EntityAccountType)
                {
                    EntityAccountType = value;
                }

                RaisePropertyChanged("EntityAccountType");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchCostCenterUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblCostCenter;
            }
        }

        public static void OnEntityAccountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchCostCenterUserControl;
            if (args.NewValue != null)
            {
                fil.EntityAccount = args.NewValue as Entity;
            }
        }

        public static void OnEntityAccountTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchCostCenterUserControl;
            if (args.NewValue != null)
            {
                fil.EntityAccountType = args.NewValue as GenericTable;
            }
        }

        public static void OnJournalAccountTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchCostCenterUserControl;
            if (args.NewValue != null)
            {
                fil.JournalAccountType = args.NewValue as GenericTable;
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchCostCenter(this);
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
                if (JournalAccountType != null)
                    Glclient.GetTblCostCenterByCodeAsync(txt.Text,JournalAccountType.Iserial,
                        LoggedUserInfo.DatabasEname, LoggedUserInfo.Iserial);

            //  Glclient.FindEntityByCodeAsync(JournalAccountType.Iserial, txt.Text, ScopePerRow.Iserial, LoggedUserInfo.DatabasEname, tempPre);
        }
    }
}