using System;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.SearchChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class BankDeposit : UserControl
    {
        private DepositViewModel _viewModle = new DepositViewModel();

        public BankDeposit()
        {
            InitializeComponent();
            switchformmmode(FormMode.Standby);
            DataContext = _viewModle;
            if (LoggedUserInfo.ActiveStore != 0)
            {
                addtext.IsChecked = true;
            }
            else
            {
                addtext.IsChecked = false;
            }
        }

        public enum FormMode
        {
            Save,
            Delete,
            Serach,
            Standby,
            Update,
            Add,
        }

        private void Clear()
        {
            string serial;
            var tempStore = _viewModle.TransactionHeader.StorePerRow;

            serial = _viewModle.TransactionHeader.Iserial;
            int user;
            user = _viewModle.TransactionHeader.TblUser;

            _viewModle.TransactionHeader = new TblDepositViewModel();
            if (LoggedUserInfo.ActiveStore != null)
            {
                _viewModle.TransactionHeader.Iserial = serial;
                _viewModle.TransactionHeader.StorePerRow = tempStore;
            }

            if (LoggedUserInfo.Iserial != 0)
            {
                _viewModle.TransactionHeader.TblUser = user;
            }

            addtext.IsChecked = false;
            buttonsave.IsChecked = false;
        }

        public void switchformmmode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    Clear();
                    buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = false;
                    buttonsearch.IsEnabled = false;
                    addtext.IsEnabled = true;

                    break;

                case FormMode.Save:
                    buttonreport.IsEnabled = true;
                    buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = false;
                    buttonsearch.IsEnabled = false;
                    addtext.IsEnabled = true;
                    // clear();
                    break;

                case FormMode.Delete:
                    buttonsave.IsEnabled = false;
                    bttndelete.IsEnabled = true;
                    buttonsearch.IsEnabled = false;
                    _viewModle.TransactionHeader.inter = false;
                    Clear();
                    break;

                case FormMode.Standby:
                    Clear();
                    addtext.IsEnabled = true;
                    buttonsave.IsEnabled = false;
                    bttndelete.IsEnabled = false;
                    buttonsearch.IsEnabled = true;
                    buttonreport.IsEnabled = false;

                    cancel.Visibility = Visibility.Collapsed;
                    _viewModle.TransactionHeader.inter = false;
                    break;

                case FormMode.Serach:
                    buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = true;
                    buttonsearch.IsEnabled = true;
                    buttonreport.IsEnabled = true;
                    //_viewModle.TransactionHeader.inter = true;

                    break;
            }
        }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            switchformmmode(_FormMode);
        }

        public FormMode _FormMode { get; set; }

        private void buttonsearch_Click(object sender, RoutedEventArgs e)
        {
            var child = new SearchBankDeposit(_viewModle);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            child.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            child.Show();
            _FormMode = FormMode.Serach;
            switchformmmode(_FormMode);
            cancel.Visibility = Visibility.Visible;
            cancel.IsEnabled = true;
            _viewModle.TransactionHeader.inter = true;
            // _viewModle.GetMaindata();
        }

        private void buttonsave_Click(object sender, RoutedEventArgs e)
        {
            _viewModle.UpdateAndInsert();

            _FormMode = FormMode.Save;
            switchformmmode(_FormMode);
            cancel.IsEnabled = true;

            cancel.Visibility = Visibility.Visible;
        }

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModle.Delete();
            _FormMode = FormMode.Delete;
            switchformmmode(_FormMode);
        }

        private void ItemAutoComplete_Populating(object sender, PopulatingEventArgs e)
        {
            var autoComplete = sender as AutoCompleteBox;
            // _viewModle.SearchForStorEname(autoComplete.Text);
        }

        private void ItemAutoComplete_Loaded(object sender, RoutedEventArgs e)
        {
            var autoComplete = sender as AutoCompleteBox;
            if (_viewModle != null)
                _viewModle.SupplierItemCompleted += (s, r) => autoComplete.PopulateComplete();
        }

        private void cancel_Checked(object sender, RoutedEventArgs e)
        {
            ResetMode();
            cancel.IsEnabled = false;
            cancel.Visibility = Visibility.Collapsed;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void searchbycode_Click(object sender, RoutedEventArgs e)
        {
            var Child2 = new SearchForStore(_viewModle);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            Child2.FlowDirection = currentUi.DisplayName == "العربية" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            Child2.Show();
            //SearchChilWindow child = new SearchChilWindow(_viewModle);
            //child.Show();

            _FormMode = FormMode.Serach;
            switchformmmode(_FormMode);
            cancel.Visibility = Visibility.Visible;
            cancel.IsEnabled = true;
            _viewModle.TransactionHeader.inter = true;
        }

        private void textBlock1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModle.SearchByStoreCode();

                _FormMode = FormMode.Serach;
                switchformmmode(_FormMode);
                cancel.Visibility = Visibility.Visible;
                cancel.IsEnabled = true;
            }
        }

        private void addtext_Checked(object sender, RoutedEventArgs e)
        {
            if (addtext.IsChecked == true)
            {
                _viewModle.Bank();
                _viewModle.TransactionHeader.date();
                _FormMode = FormMode.Add;
                switchformmmode(_FormMode);
                cancel.Visibility = Visibility.Visible;
                cancel.IsEnabled = true;
                _viewModle.TransactionHeader.inter = true;
            }
        }

        private void buttonreport_Click(object sender, RoutedEventArgs e)
        {
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            if (LoggedUserInfo.DatabasEname.ToLower() == "ccnew")
            {
                if (currentUi.DisplayName != "العربية")
                {
                    var myUri = new Uri(HtmlPage.Document.DocumentUri, string.Format("CcnewBankDepositReportAr.aspx?Iserial=" + _viewModle.TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri, "_Blank");
                }
                else
                {
                    var myUri2 = new Uri(HtmlPage.Document.DocumentUri, string.Format("CcnewBankDepositReport.aspx?Iserial=" + _viewModle.TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri2, "_Blank");
                }
            }
            else
            {
                if (currentUi.DisplayName != "العربية")
                {
                    var myUri3 = new Uri(HtmlPage.Document.DocumentUri, string.Format("SwBankDepositReportAr.aspx?Iserial=" + _viewModle.TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri3, "_Blank");
                }

                else
                {
                    var myUri4 = new Uri(HtmlPage.Document.DocumentUri, string.Format("SwBankDepositReport.aspx?Iserial=" + _viewModle.TransactionHeader.Iserial));

                    HtmlPage.Window.Navigate(myUri4, "_Blank");
                }
            }
        }
    }
}