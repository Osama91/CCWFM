using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.Views.RequestForQutation
{
    

    public partial class RFQGetStyleChild : ChildWindow
    {
        readonly _Proxy.CRUD_ManagerServiceClient _client = new _Proxy.CRUD_ManagerServiceClient();

        public RFQGetStyleChild()
        {
            InitializeComponent();
            _client.SearchAXStylesCompleted += (s, ee) =>
            {
                try
                {
                    SearchResultDataGrid.ItemsSource = ee.Result;
                }
                finally
                {
                    lgnProgress.Visibility = Visibility.Collapsed;
                }
            };
        }

        public event EventHandler<RFQChildReturnEventArgs> SubmitActions;

        private void btnSearchItems_Click(object sender, RoutedEventArgs e)
        {
            SearchStyles();
        }

        private void SearchStyles()
        {
            lgnProgress.Visibility = Visibility.Visible;
            _client.SearchAXStylesAsync(StyleCodeTextBox.Text.Trim());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReturnStyle();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnStyle();
        }

        private void ReturnStyle()
        {
            if (SubmitActions != null)
            {
                var vAxStyles = SearchResultDataGrid.SelectedItem as _Proxy.V_AXStyles;
                if (vAxStyles != null)
                    SubmitActions(this
                        , new RFQChildReturnEventArgs(vAxStyles.StyleCode
                            , vAxStyles.StyleName));
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }

        private void StyleCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //(sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void StyleCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchStyles();
            }
        }
    }

    public class RFQChildReturnEventArgs : EventArgs
    {
        public string Description { get; private set; }

        public string Style { get; private set; }

        public RFQChildReturnEventArgs(string style, string description)
        {
            Style = style;
            Description = description;
        }
    }
}