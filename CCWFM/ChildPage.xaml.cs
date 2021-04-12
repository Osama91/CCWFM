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
using System.Collections.ObjectModel;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.LkpData;

namespace CCWFM
{
    public partial class ChildPage : Page
    {
        ObservableCollection<LkpData.tblFormLayout> AllDefaultItems = new ObservableCollection<LkpData.tblFormLayout>();
        ObservableCollection<LkpData.tblFormLayout> RemainingDefaultItems = new ObservableCollection<LkpData.tblFormLayout>();
        ObservableCollection<LkpData.tblFormLayout> UserCustomItems = new ObservableCollection<LkpData.tblFormLayout>();
        int tblUser;
        string UserFormID;
        public ChildPage(int User,string FormID = "")
        {
            InitializeComponent();
            tblUser = User;
            UserFormID = "StyleHeaderMainGrid";
            LoadData();
        }

        private void LoadData()
        {
            //LoadDefaultGrid
            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
            _client.GettblFormLayoutDefaultAsync(UserFormID);
            _client.GettblFormLayoutDefaultCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    RemainingDefaultItems = sv.Result;
                    AllDefaultItems  = sv.Result;

                    //LoadUserItems
                    _client.GettblFormLayoutByUserAsync(UserFormID, tblUser);
                    _client.GettblFormLayoutByUserCompleted += _client_GettblFormLayoutByUserCompleted;     
                }
            };

          
        }

        private void _client_GettblFormLayoutByUserCompleted(object sender, GettblFormLayoutByUserCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                UserCustomItems.Clear();
                ObservableCollection<LkpData.tblFormLayout> dItems = new ObservableCollection<LkpData.tblFormLayout>(AllDefaultItems);
                foreach (var item in e.Result)
                {
                    LkpData.tblFormLayout fl = new LkpData.tblFormLayout();
                    fl = dItems.FirstOrDefault(x => x.Iserial == item.tblFormLayout1.Iserial);
                    RemainingDefaultItems = new ObservableCollection<LkpData.tblFormLayout>(RemainingDefaultItems.Where(x => x.Iserial != fl.Iserial).ToList());
                    UserCustomItems.Add(fl);
                }
                grdDefault.ItemsSource = RemainingDefaultItems;
                grdUserCustom.ItemsSource = UserCustomItems;
            }
        }

        private void btnLoadDefault_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grdDefault.ItemsSource = null;
                grdUserCustom.ItemsSource = null;
                UserCustomItems.Clear();
                UserCustomItems = new ObservableCollection<LkpData.tblFormLayout>(AllDefaultItems);
                grdUserCustom.ItemsSource = UserCustomItems;
            }
            catch
            {
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Order = 0;
                ObservableCollection<LkpData.tblFormLayoutUser> SelectedItems = new ObservableCollection<tblFormLayoutUser>();
                foreach (var row in grdUserCustom.Items)
                {
                    var currentItem = row as tblFormLayout;
                    tblFormLayoutUser item = new tblFormLayoutUser();
                    item.tblUser = tblUser;
                    item.tblFormLayout = currentItem.Iserial;
                    item.ColOrder = Order;
                    Order++;
                    SelectedItems.Add(item);
                }
                LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                _client.UpdateOrDeletetblFormLayoutUsersAsync(tblUser, UserFormID, SelectedItems);
                _client.UpdateOrDeletetblFormLayoutUsersCompleted += (s, sv) => { };

            } catch { }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grdUserCustom.ItemsSource = null;
                UserCustomItems.Clear();
            }
            catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            { System.Windows.Browser.HtmlPage.Window.Invoke("close"); }
            catch { }
        }
    }
}
