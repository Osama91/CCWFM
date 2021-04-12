using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ChangePassword
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private bool _btnEnabled;

        public bool BtnEnabled
        {
            get { return _btnEnabled; }
            set
            {
                _btnEnabled = value;
                RaisePropertyChanged("BtnEnabled");
            }
        }

        private string _newPassword;
        private string _newPasswordConfirmation;

        [Display(Name = "New password")]
        [Required]
        [StringLength(80, ErrorMessage = "New password must be a string with maximum length of 80.")]
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                RaisePropertyChanged("NewPassword");
                BtnEnabled = false;
                //   BtnEnabled = true;
                //if (NewPassword != NewPasswordConfirmation)
                //{
                //    BtnEnabled = false;
                //    throw new ValidationException("Password confirmation not equal to password."); ;
                //}
            }
        }

        [Display(Name = "New password confirmation")]
        public string NewPasswordConfirmation
        {
            get { return _newPasswordConfirmation; }
            set
            {
                _newPasswordConfirmation = value;
                RaisePropertyChanged("NewPasswordConfirmation");
                BtnEnabled = true;
                if (NewPassword != NewPasswordConfirmation)
                {
                    BtnEnabled = false;
                    throw new ValidationException("Password confirmation not equal to password."); ;
                }
            }
        }

        private string _userName;

        [Display(Name = "User Name")]
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqUserName")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public ChangePassword(int userIserial)
        {
            InitializeComponent();
            this.userIserial = userIserial;
            _client.ChangePasswordCompleted += (s, sv) =>
            {
                if (sv.Error != null)
                {
                    MessageBox.Show(sv.Error.Message);
                }
                else
                {
                    MessageBox.Show("User Name and Password Changed");
                    DialogResult = true;
                }
            };

            DataContext = this;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(UserName))
            { _client.ChangePasswordAsync(UserName, NewPassword, userIserial); }
        }

        private int _userIserial;

        public int userIserial
        {
            get { return _userIserial; }
            set { _userIserial = value; RaisePropertyChanged("userIserial"); }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}