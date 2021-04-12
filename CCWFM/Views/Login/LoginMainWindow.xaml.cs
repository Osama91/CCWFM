using System.Windows;

namespace CCWFM.Views.Login
{
    public partial class LoginMainWindow
    {
        private LoginChildWindow _child;

        public LoginMainWindow()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            _child = new LoginChildWindow(this);
            _child.Show();
            _child.Closed += child_Closed;
        }

        private void child_Closed(object sender, System.EventArgs e)
        {
            Visibility = Visibility.Visible;
            if (_child.DialogResult == false)
            {
                MainPage_Loaded(null, null);
            }
        }
    }
}