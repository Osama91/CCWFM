using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;

namespace CCWFM.UserControls
{
    public partial class LookupEdit
    {
        public LookupEdit()
        {
            InitializeComponent();
        }

        public PermissionItemName FormName { get; set; }

        public string Title { get; set; }

        private void BtnLookup_OnClick(object sender, RoutedEventArgs e)
        {
            GeneralFilter.NavigatToMenu(FormName.ToString(),Title,null);
        }
    }
}