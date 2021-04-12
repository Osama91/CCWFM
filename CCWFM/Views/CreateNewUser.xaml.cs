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
using System.Windows.Navigation;
using CCWFM.ViewModel.OGViewModels;


namespace CCWFM.Views.OGView
{
    public partial class CreateNewUser
    {
        public CreateNewUserViewModel ViewModel;
        public bool IsValidUserName { get; set; }
        public CreateNewUser()
        {
            InitializeComponent();
            try
            {
                ViewModel = LayoutRoot.DataContext as CreateNewUserViewModel;
                DataContext = ViewModel;
            }
            catch { }
            ClearControls();
            LoadData();
            IsValidUserName = false;

            AuthService.AuthServiceClient c = new AuthService.AuthServiceClient();


        }
        private void chkIsSalesPerson_Checked(object sender, RoutedEventArgs e)
        {
            if (chkIsSalesPerson.IsChecked == true)
                chkIsRetailPerson.IsChecked = true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            lblSaveMessage.Content = "";
            lblSaveMessage.Visibility = Visibility.Collapsed;

            string LikeEmpid = ViewModel.SelectedMainRow.EmpPerRow.Emplid;
            string NewEmpid = ViewModel.SelectedMainRow.SecondEmpPerRow.Emplid;
            bool IsValidUser = true;
            try
            {
                lblUserNameMessage.Visibility = Visibility.Collapsed;
                if (string.IsNullOrEmpty(LikeEmpid))
                {
                    IsValidUser = false;
                    lblLikeEmployeeUserMessage.Visibility = Visibility.Visible;
                    lblLikeEmployeeUserMessage.Content = "Please select Employee";
                }
                if (string.IsNullOrEmpty(NewEmpid))
                {
                    IsValidUser = false;
                    lblNewEmployeeUserMessage.Visibility = Visibility.Visible;
                    lblNewEmployeeUserMessage.Content = "Please select Employee";
                }
                if (string.IsNullOrEmpty(radCompany.SelectedValue.ToString()))
                {
                    IsValidUser = false;
                    lblCompanyMessage.Visibility = Visibility.Visible;
                    lblCompanyMessage.Content = "Please select Company";
                }
                if (IsValidUserName == false)
                {
                    IsValidUser = false;
                    lblUserNameMessage.Visibility = Visibility.Visible;
                    lblUserNameMessage.Content = "*";
                }
                if (IsValidUser)
                {
                     LoginService.LoginServiceClient  _client = new LoginService.LoginServiceClient();
                    _client.SaveNewUserRequestCompleted += _client_SaveNewUserRequestCompleted;
                    _client.SaveNewUserRequestAsync(NewEmpid,
                      radCompany.SelectedValue.ToString(),
                      LikeEmpid,
                      radCompany.SelectedValue.ToString(),
                      chkIsSalesPerson.IsChecked == true ? "1" : "0",
                      chkIsRetailPerson.IsChecked == true ? "1" : "0",
                      txtUserName.Text.Trim(),
                      txtReason.Text,
                      Helpers.AuthenticationHelpers.LoggedUserInfo.Iserial.ToString());
                }
            }
            catch { }

        }

        private void _client_SaveNewUserRequestCompleted(object sender, LoginService.SaveNewUserRequestCompletedEventArgs e)
        {
            try
            {
                validateUserName();
                if (e.Result.ToString() == "1")
                {
                    //MessageBoxResult result = MessageBox.Show("Request Saved",
                    //   "Confirm", MessageBoxButton.OKCancel);
                    lblSaveMessage.Visibility = Visibility.Visible;
                    lblSaveMessage.Foreground = new SolidColorBrush(Colors.Green);
                    lblSaveMessage.Content = "Request Saved Successfuly";
                    ClearControls();
                }
                else
                {
                    //MessageBoxResult result = MessageBox.Show("Request Failed",
                    //                  "Failed", MessageBoxButton.OKCancel);
                    lblSaveMessage.Visibility = Visibility.Visible;
                    lblSaveMessage.Foreground = new SolidColorBrush(Colors.Red);
                    lblSaveMessage.Content = "Request Failed";
                }
            }
            catch
            {
                lblSaveMessage.Visibility = Visibility.Visible;
                lblSaveMessage.Foreground = new SolidColorBrush(Colors.Red);
                lblSaveMessage.Content = "Request Failed";
            }
        }

        private void LoadData()
        {
            BankStatService.BankStatServiceClient _Client = new BankStatService.BankStatServiceClient();

            _Client.GetCompaniesCompleted += _Client_GetCompaniesCompleted ;
            _Client.GetCompaniesAsync();

        }


        private void _Client_GetCompaniesCompleted(object sender, BankStatService.GetCompaniesCompletedEventArgs e)
        {
            try
            {
                radCompany.ItemsSource = e.Result.ToList();
            }
            catch
            { }
        }
        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {


        }

        private void validateUserName()
        {
            string UserName = txtUserName.Text;

            if (!UserName.Contains('.'))
            {
                lblUserNameMessage.Visibility = Visibility.Visible;
                lblUserNameMessage.Content = "Invalid, Must Contain '.' Character";
                IsValidUserName = false;
            }
            else if (UserName.Substring(0, 1) == ".")
            {
                lblUserNameMessage.Visibility = Visibility.Visible;
                lblUserNameMessage.Content = "Invalid,'.' not in the begining";
                IsValidUserName = false;
            }
            else if (UserName.Substring(UserName.Length - 1) == ".")
            {
                lblUserNameMessage.Visibility = Visibility.Visible;
                lblUserNameMessage.Content = "Invalid,'.' not in the end";
                IsValidUserName = false;
            }
            else
            {
                
                UserDomains.UserDomainsClient _Client = new UserDomains.UserDomainsClient();
                _Client.GetADUsersCompleted += _Client_GetADUsersCompleted;
                _Client.GetADUsersAsync();
            }
        }

        private void _Client_GetADUsersCompleted(object sender, UserDomains.GetADUsersCompletedEventArgs e)
        {
            lblUserNameMessage.Visibility = Visibility.Collapsed;
            lblUserNameMessage.Content = "";
            var DomianUsers = e.Result.ToList();
            int Count = DomianUsers.Where(x => x.UserName == txtUserName.Text).Count();
            if (Count >= 1)
            {
                IsValidUserName = false;
                lblUserNameMessage.Visibility = Visibility.Visible;
                lblUserNameMessage.Content = "User Already Exists";
            }
            else
            {
                IsValidUserName = true;
                lblUserNameMessage.Visibility = Visibility.Collapsed;
                lblUserNameMessage.Content = "";
            }
        }
        private void ClearControls()
        {
            lblCompanyMessage.Content =
            lblNewEmployeeUserMessage.Content =
            lblLikeEmployeeUserMessage.Content =
            lblUserNameMessage.Content =
            txtReason.Text = txtUserName.Text = "";
            chkIsSalesPerson.IsChecked = chkIsRetailPerson.IsChecked = false;
            LoadData();
        }
        private void chkIsRetailPerson_Unchecked(object sender, RoutedEventArgs e)
        {
            chkIsSalesPerson.IsChecked = false;
        }
    }
}
