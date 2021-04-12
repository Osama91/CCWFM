using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;

namespace CCWFM.UserControls
{
    public partial class SearchChequeUserControl
    {
        private GlServiceClient Glclient = new GlServiceClient();
        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblBankCheque),
                typeof(UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty EntityPerRowProperty =
          DependencyProperty.Register("EntityPerRow", typeof(Entity),
              typeof(UserControl),
              new PropertyMetadata(null, OnEntityPerRowChanged));

        public DependencyProperty ChequeStatusPerRowProperty =
        DependencyProperty.Register("ChequeStatusPerRow", typeof(GenericTable),
            typeof(UserControl),
            new PropertyMetadata(null, OnChequeStatusPerRowChanged));

        public DependencyProperty DefaultBankPerRowProperty =
       DependencyProperty.Register("DefaultBankPerRow", typeof(GenericTable),
           typeof(UserControl),
           new PropertyMetadata(null, OnDefaultBankPerRowChanged));

        public SearchChequeUserControl()
        {
            InitializeComponent();
            Glclient.GetTblBankChequeCompleted += (s, sv) =>
            { if (sv.Result != null) SearchPerRow = sv.Result.FirstOrDefault(); };
        }

        public TblBankCheque SearchPerRow
        {
            get { return (TblBankCheque)GetValue(SearchPerRowProperty); }
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

        public Entity EntityPerRow
        {
            get { return (Entity)GetValue(EntityPerRowProperty); }
            set
            {
                SetValue(EntityPerRowProperty, value);
                if (value != EntityPerRow)
                {
                    EntityPerRow = value;
                }

                RaisePropertyChanged("EntityPerRow");
            }
        }

        public GenericTable ChequeStatusPerRow
        {
            get { return (GenericTable)GetValue(ChequeStatusPerRowProperty); }
            set
            {
                if (value != null)
                {


                    SetValue(ChequeStatusPerRowProperty, value);

                    if (value != ChequeStatusPerRow)
                    {
                        ChequeStatusPerRow = value;
                    }

                    RaisePropertyChanged("ChequeStatusPerRow");
                }
            }

        }
        public GenericTable DefaultBankPerRow
        {
            get { return (GenericTable)GetValue(DefaultBankPerRowProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(DefaultBankPerRowProperty, value);

                    if (value != DefaultBankPerRow)
                    {
                        DefaultBankPerRow = value;
                    }

                    RaisePropertyChanged("DefaultBankPerRow");
                }
            }
        }

        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchChequeUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblBankCheque;
            }
        }

        public static void OnEntityPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchChequeUserControl;
            if (args.NewValue != null)
            {
                fil.EntityPerRow = args.NewValue as Entity;
            }
        }

        public static void OnChequeStatusPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchChequeUserControl;
            if (args.NewValue != null)
            {
                fil.ChequeStatusPerRow = args.NewValue as GenericTable;
            }
        }
        public static void OnDefaultBankPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchChequeUserControl;
            if (args.NewValue != null)
            {
                fil.DefaultBankPerRow = args.NewValue as GenericTable;
            }
        }
        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;

            const string sortBy = "it.Iserial";
            if (txt.Text != null)
            {
                if (!string.IsNullOrWhiteSpace(txt.Text))
                {
                    string temp = txt.Text;
                    var valuesObjects = new Dictionary<string, object>();
                    const string filter = "it.Cheque ==(@tt)";
                    valuesObjects.Add("tt", Convert.ToInt32(temp));
                    Glclient.GetTblBankChequeAsync(0, 1, 0, sortBy, filter, valuesObjects,
                        LoggedUserInfo.DatabasEname, ChequeStatusPerRow.Iserial);
                }
            }
        }


        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchBankCheque(this);
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