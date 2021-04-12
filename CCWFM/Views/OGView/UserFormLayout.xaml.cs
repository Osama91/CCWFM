using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections;
using System.Collections.ObjectModel;


namespace CCWFM.Views.OGView
{
    public partial class UserFormLayout 
    {
        ObservableCollection<LkpData.tblFormLayout> MainItems = new ObservableCollection<LkpData.tblFormLayout>();
        public UserFormLayout()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            ////LoadDefaultGrid
            //LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
            //_client.GettblFormLayoutAsync("StyleHeaderMainGrid");
            //_client.GettblFormLayoutCompleted += (s, sv) =>
            //{
            //    if (sv.Result != null)
            //    {
            //        MainItems = sv.Result;
            //        grdDefault.ItemsSource = MainItems;
            //    }
            //};

        }
    }
}
