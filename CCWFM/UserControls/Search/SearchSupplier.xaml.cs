using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using TBLsupplier = CCWFM.CRUDManagerService.TBLsupplier;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls.Search
{
    public partial class SearchSupplier
    {
        public DependencyProperty SearchPerRowProperty =
                                                                   DependencyProperty.Register("SearchPerRow", typeof(TBLsupplier), typeof(UserControl),
                                                                   new PropertyMetadata(null, OnEmpPerRowChanged));

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public TBLsupplier SearchPerRow
        {
            get { return (TBLsupplier)GetValue(SearchPerRowProperty); }
            set
            {
                SetValue(SearchPerRowProperty, value);
                if (SearchPerRow != null && value != SearchPerRow)
                {
                    SearchPerRow = value;
                }

                RaisePropertyChanged("SearchPerRow");
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchSupplier;
            if (args.NewValue != null)
            {
                if (fil != null) fil.SearchPerRow = args.NewValue as TBLsupplier;
            }
        }
        public bool IsDesignTime
        {
            get
            {
                return (Application.Current == null) ||
                       (Application.Current.GetType() == typeof(Application));
            }
        }
        public SearchSupplier()
        {
            if (!IsDesignTime)
            {
                InitializeComponent();
                Client.GetTblRetailSupplierCompleted += (s, sv) =>
                { if (sv.Result != null) SearchPerRow = sv.Result.FirstOrDefault(); };
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchSupplierChild(this);
            child.Show();
        }
        private CRUD_ManagerServiceClient _client;

        public CRUD_ManagerServiceClient Client
        {
            get { return _client ?? (_client = new CRUD_ManagerServiceClient()); }
            set { _client = value; RaisePropertyChanged("Client"); }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;

            const string sortBy = "it.Iserial";
            if (txt != null && txt.Text != null)
            {
                if (!string.IsNullOrWhiteSpace(txt.Text))
                {
                    var temp = txt.Text;
                    var valuesObjects = new Dictionary<string, object>();
                    const string filter = "it.Code ==(@tt)";
                    valuesObjects.Add("tt", temp);
                    Client.GetTblRetailSupplierAsync(0, 1, sortBy, filter, valuesObjects, LoggedUserInfo.DatabasEname);
                }
            }


            //  Glclient.FindEntityByCodeAsync(JournalAccountType.Iserial, txt.Text, ScopePerRow.Iserial, LoggedUserInfo.DatabasEname, tempPre);
        }
    }
}