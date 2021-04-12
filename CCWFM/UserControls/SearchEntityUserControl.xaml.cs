using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls
{
    public partial class SearchEntityUserControl
    {
        private readonly GlServiceClient _glclient = new GlServiceClient();

        public DependencyProperty JournalAccountTypePerRowProperty =
            DependencyProperty.Register("JournalAccountType", typeof(GenericTable),
                typeof(UserControl),
                new PropertyMetadata(null, OnJournalAccountTypeChanged));

        public DependencyProperty ScopePerRowProperty =
         DependencyProperty.Register("ScopePerRow", typeof(GenericTable),
             typeof(UserControl),
             new PropertyMetadata(null, OnScopeTypeChanged));

        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(Entity), typeof(UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty PreventPerRowProperty =
            DependencyProperty.Register("PreventPerRow", typeof(GenericTable), typeof(UserControl),
                new PropertyMetadata(null, OnPreventPerRowChanged));

        private bool IsDesignTime
        {
            get
            {
                return (Application.Current == null) ||
                       (Application.Current.GetType() == typeof(Application));
            }
        }
        public SearchEntityUserControl()
        {
            if (!IsDesignTime)
            {
                InitializeComponent();
                _glclient.FindEntityByCodeCompleted += (s, sv) =>
                {
                    if (sv.Result != null)
                    {
                        SearchPerRow = sv.Result;
                    }

                };
            }
        }

        public Entity SearchPerRow
        {
            get { return (Entity)GetValue(SearchPerRowProperty); }
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

        public GenericTable ScopePerRow
        {
            get { return (GenericTable)GetValue(ScopePerRowProperty); }
            set
            {
                SetValue(ScopePerRowProperty, value);
                if (value != ScopePerRow)
                {
                    ScopePerRow = value;
                }

                RaisePropertyChanged("ScopePerRow");
            }
        }

        public GenericTable PreventPerRow
        {
            get { return (GenericTable)GetValue(PreventPerRowProperty); }
            set
            {
                SetValue(PreventPerRowProperty, value);
                if (value != PreventPerRow)
                {
                    PreventPerRow = value;
                }

                RaisePropertyChanged("PreventPerRow");
            }
        }

        public static void OnPreventPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchEntityUserControl;
            if (args.NewValue != null)
            {
                fil.PreventPerRow = args.NewValue as GenericTable;
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchEntityUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as Entity;
            }
        }

        public static void OnJournalAccountTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchEntityUserControl;
            if (args.NewValue != null)
            {
                fil.JournalAccountType = args.NewValue as GenericTable;
            }
        }

        public static void OnScopeTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchEntityUserControl;
            if (args.NewValue != null)
            {
                fil.ScopePerRow = args.NewValue as GenericTable;
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchEntity(this);
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
            if (ScopePerRow == null)
            {
                ScopePerRow = new GenericTable();
            }
            bool tempPre = false;
            if (PreventPerRow != null)
            {
                tempPre = PreventPerRow.Iserial > 0;
            }

            if (!string.IsNullOrEmpty(txt.Text))                
                _glclient.FindEntityByCodeAsync(JournalAccountType.Iserial, txt.Text, ScopePerRow.Iserial, LoggedUserInfo.DatabasEname, tempPre);
            //Glclient.GetEntityAsync(0, 1, SearchPerRow.TblJournalAccountType, SearchPerRow.scope, "it.Iserial", Filter,
            //        ValuesObjects, LoggedUserInfo.DatabasEname);
        }
    }
}