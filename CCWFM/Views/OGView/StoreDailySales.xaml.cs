using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCWFM.Views.OGView
{
    public partial class StoreDailySales 
    {
     
        private readonly StoreDailySalesViewModel _viewModel;

        public StoreDailySales()
        {
            InitializeComponent();
            _viewModel = LayoutRoot.DataContext as StoreDailySalesViewModel;
            DataContext = _viewModel;

            LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
            lkpClient.GetUserDefaultStoreAsync(LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname, LoggedUserInfo.AllowedStores.First());
            lkpClient.GetUserDefaultStoreCompleted += (s, sv) =>
            {
                lblStoreName.Text = sv.Result.ENAME.ToString();
            };

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

    }
}
