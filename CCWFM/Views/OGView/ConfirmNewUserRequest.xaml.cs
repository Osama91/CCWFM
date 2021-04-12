using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using CCWFM.LoginService;

namespace CCWFM.Views.OGView
{
    public partial class ConfirmNewUserRequest 
    {
        public ConfirmNewUserRequest()
        {
            InitializeComponent();
            LoadUserRequests();
        }

        private void LoadUserRequests()
        {
            LoginService.LoginServiceClient _Client = new LoginService.LoginServiceClient();
            _Client.GetAllNewUsersRequestCompleted += _Client_GetAllNewUsersRequestCompleted;
            _Client.GetAllNewUsersRequestAsync();    
        }

        private void _Client_GetAllNewUsersRequestCompleted(object sender, LoginService.GetAllNewUsersRequestCompletedEventArgs e)
        {
            
            List<CCWFM.LoginService.NewUserRequests> UserRequests = new List<CCWFM.LoginService.NewUserRequests>();
            UserRequests = e.Result.ToList();
            MainGrid.ItemsSource = UserRequests;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void BtnConfirmUser_Click(object sender, RoutedEventArgs e)
        {
            CCWFM.LoginService.NewUserRequests user = (CCWFM.LoginService.NewUserRequests)MainGrid.SelectedItem;
            user.Approved = "1";
            user.Rejected = "0";
            UpdateUserRequest(user);
        }

        private void UpdateUserRequest(NewUserRequests user)
        {
            LoginService.LoginServiceClient _Client = new LoginService.LoginServiceClient();
            _Client.UpdateNewUsersRequestCompleted += _Client_UpdateNewUsersRequestCompleted;
            _Client.UpdateNewUsersRequestAsync(user);
           // ShowMessage(user.Rejected == "1" ? "1" : "2");
        }

        private void ShowMessage(string Status)
        {
            MessageBoxResult result = MessageBox.Show(Status == "1" ? "User Rejected" : "User Approved",
             "Confirm", MessageBoxButton.OKCancel);
        }
        private void _Client_UpdateNewUsersRequestCompleted(object sender, UpdateNewUsersRequestCompletedEventArgs e)
        {
            LoadUserRequests();
        }

        private void BtnRejectUser_Click(object sender, RoutedEventArgs e)
        {
            CCWFM.LoginService.NewUserRequests user = (CCWFM.LoginService.NewUserRequests)MainGrid.SelectedItem;
            user.Rejected = "1";
            user.Approved = "0";
            UpdateUserRequest(user);
        }
    }
}
