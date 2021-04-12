using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.CRUDManagerService;

namespace CCWFM.Views.StylePages
{
    

    public partial class RFSGetStyleChild
    {
        readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
        public string BrandCode { get; set; }
        public string SeasonCode { get; set; }
        public RFSGetStyleChild()
        {
            InitializeComponent();
        //    _client.SearchSMLsCompleted += (s, ee) =>
        //    {
        //        try
        //        {
        //            SearchResultDataGrid.ItemsSource = ee.Result;
        //        }
        //        finally
        //        {
        //            lgnProgress.Visibility = Visibility.Collapsed;
        //        }
        //    };
        }

        public event EventHandler<RFSChildReturnEventArgs> SubmitActions;

        private void btnSearchItems_Click(object sender, RoutedEventArgs e)
        {
            SearchStyles();
        }

        private void SearchStyles()
        {
            lgnProgress.Visibility = Visibility.Visible;
         // _client.SearchSMLsAsync(StyleCodeTextBox.Text.Trim(),SeasonCode,BrandCode);
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
                var tmp = SearchResultDataGrid.SelectedItem as 
                   TblSeasonalMasterList;
                
                //if (tmp != null)
                //    SubmitActions(this
                //        , new RFSChildReturnEventArgs(tmp.StyleCode
                //            , tmp.Description, tmp.ColorCode,tmp.tblSeasonalMasterListDetails.Select(x=>x.Size)));
                //DialogResult = true;
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

    public class RFSChildReturnEventArgs : EventArgs
    {
        public string Description { get; private set; }

        public string Style { get; private set; }

        public string ColorCode { get; private set; }

        public List<string> Sizes { get; private set; }

        public RFSChildReturnEventArgs(string style, string description, string clrs,
            IEnumerable<string> tblSeasonalMasterListDetails)
        {
            Style = style;
            Description = description;
            ColorCode = clrs;
            Sizes = new List<string>(tblSeasonalMasterListDetails);
        }
    }
}