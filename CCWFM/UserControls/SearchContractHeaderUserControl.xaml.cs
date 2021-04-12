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
    public partial class SearchContractHeaderUserControl
    {
        private GlServiceClient Glclient = new GlServiceClient();
        public DependencyProperty SearchPerRowProperty =
            DependencyProperty.Register("SearchPerRow", typeof(TblContractHeader),
                typeof(UserControl),
                new PropertyMetadata(null, OnEmpPerRowChanged));

        public DependencyProperty EntityPerRowProperty =
          DependencyProperty.Register("EntityPerRow", typeof(Entity),
              typeof(UserControl),
              new PropertyMetadata(null, OnEntityPerRowChanged));

   

        public SearchContractHeaderUserControl()
        {
            InitializeComponent();
            //Glclient.GetTblContractHeaderCompleted += (s, sv) =>
            //{ if (sv.Result != null) SearchPerRow = sv.Result.FirstOrDefault(); };
        }

        public TblContractHeader SearchPerRow
        {
            get { return (TblContractHeader)GetValue(SearchPerRowProperty); }
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



        public static void OnEmpPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchContractHeaderUserControl;
            if (args.NewValue != null)
            {
                fil.SearchPerRow = args.NewValue as TblContractHeader;
            }
        }

        public static void OnEntityPerRowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var fil = sender as SearchContractHeaderUserControl;
            if (args.NewValue != null)
            {
                fil.EntityPerRow = args.NewValue as Entity;
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //var txt = sender as TextBox;

            //const string sortBy = "it.Iserial";
            //if (txt.Text != null)
            //{
            //    if (!string.IsNullOrWhiteSpace(txt.Text))
            //    {
            //        string temp = txt.Text;
            //        var valuesObjects = new Dictionary<string, object>();
            //        const string filter = "it.ContractHeader ==(@tt)";
            //        valuesObjects.Add("tt", Convert.ToInt32(temp));
            //        Glclient.GetTblContractHeaderForChequeAsync(0, 1, 0, sortBy, filter, valuesObjects,
            //            LoggedUserInfo.DatabasEname, ContractHeaderStatusPerRow.Iserial);
            //    }
            //}
        }


        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SearchContractHeader(this);
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