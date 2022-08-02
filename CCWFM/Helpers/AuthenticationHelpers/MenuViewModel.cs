using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using CCWFM.LoginService;
using CCWFM.ViewModel;
using SilverlightCommands;
using MenuItem = SilverlightMenu.Library.MenuItem;

namespace CCWFM.Helpers.AuthenticationHelpers
{
    public class MenuTestViewModel : INotifyPropertyChanged
    {
        private MenuItem _mvvmMenuItem;
        private string _imagesPath = "../Images/";

        public MenuTestViewModel()
        {
            _mvvmMenuItem = new MenuItem
            {
                Name = "Root"
            };
            DrawMenus(LoggedUserInfo.WFM_MenuesPermissions);
        }

        private void DrawMenus(ObservableCollection<TblAuthPermission> wfmMenuesPermissions)
        {
            var menulist = new MenuItem();

            foreach (var tblAuthJobPermission in wfmMenuesPermissions.OrderBy(x => x.PermOrder).Where(x => x.ParentPerm == 0))
            {
                var subList = new MenuItem();
                foreach (var row in wfmMenuesPermissions.OrderBy(x => x.PermOrder).Where(x => x.ParentPerm == tblAuthJobPermission.Iserial))
                {
                    var newMenu = new MenuItem
                    {
                        Tag = row.ReportServer,
                        Text = LoggedUserInfo.CurrLang == 0 ? row.Aname : row.Ename,
                        PermissionTyp = row.PermissionTyp,
                        Name = row.Code,
                        IsCheckable = row.PermissionTyp != "M"
                    };

                    DrawChild(row, newMenu);
                    if (!subList.Contains(newMenu))
                    {
                        subList.Add(newMenu);
                    }
                }

                menulist.Add(new MenuItem
                {
                    Tag = tblAuthJobPermission.ReportServer,
                    Text = LoggedUserInfo.CurrLang == 0 ? tblAuthJobPermission.Aname : tblAuthJobPermission.Ename,
                    PermissionTyp = tblAuthJobPermission.PermissionTyp,
                    Name = tblAuthJobPermission.Code,
                    IsCheckable = tblAuthJobPermission.PermissionTyp != "M",
                    MenuItems = subList,
                });
            }

            _mvvmMenuItem.MenuItems = menulist;
        }

        private static void DrawChild(TblAuthPermission row, MenuItem subList)
        {
            foreach (var childrow in LoggedUserInfo.WFM_MenuesPermissions.OrderBy(x => x.PermOrder).Where(x => x.ParentPerm == row.Iserial))
            {
                var newmenu = new MenuItem
                {
                    Text = LoggedUserInfo.CurrLang == 0 ? childrow.Aname : childrow.Ename,
                    Tag = childrow.ReportServer,
                    PermissionTyp = childrow.PermissionTyp,
                    IsCheckable = childrow.PermissionTyp != "P",
                    Name = childrow.Code,
                    MenuLink = childrow.MenuLink

                };
                DrawChild(childrow, newmenu);
                if (!subList.Contains(newmenu))
                {
                    subList.Add(newmenu);
                }
            }
        }

        public MenuItem MVVMMenuItem
        {
            get
            {
                return _mvvmMenuItem;
            }
            set
            {
                _mvvmMenuItem = value;
                OnPropertyChanged("MVVMMenuItem");
            }
        }

        public string ImagesPath
        {
            get
            {
                return _imagesPath;
            }
            set
            {
                _imagesPath = value;
                OnPropertyChanged("ImagesPath");
            }
        }

        public ICommand MenuCommand
        {
            get { return new RelayCommand(DoMenuCommand); }
        }

        public void DoMenuCommand(object param)
        {
            var menuItem = (MenuItem)param;
            var para = new ObservableCollection<string>();
            if (menuItem.IsCheckable && menuItem.PermissionTyp != "M")
            {
                var item = menuItem.Name;
                if (menuItem.ParentName == "GlReport" && LoggedUserInfo.CurrLang == 0)
                {
                    item = menuItem.Name + "Ar";
                }

                if (menuItem.ParentName == "GlReport")
                {
                    para.Add(LoggedUserInfo.Ip + LoggedUserInfo.Port);
                    para.Add(LoggedUserInfo.DatabasEname);
                }
                if (menuItem.PermissionTyp == "R")
                {
                    var reportViewmodel = new GenericReportViewModel();


                    if (menuItem.Name == "Purchase Order Summary")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "Purchase Order Detail")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "Purchase Order Sizes Break Down")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "AttendanceOutPut")
                    {
                        para.Add(LoggedUserInfo.Code);
                    }
                    if (menuItem.Name == "CheckList")
                    {
                        para.Add(LoggedUserInfo.Code);
                    }
                    if (menuItem.Name == "ApprovedRetailPo")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "ApprovedStyles")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "QuickRefrence")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }


                    if (menuItem.Name == "Item List Format 1")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                   
                    if (menuItem.Name == "Styles In Production Not Retail Approved")
                    {
                        para.Add(LoggedUserInfo.Iserial.ToString(CultureInfo.InvariantCulture));
                    }
                    if (menuItem.Name == "StoreAttendance" || menuItem.Name== "StoreExcuse" || menuItem.Name == "StoreMission" || menuItem.Name== "StoreVacations")
                    {
                        para.Add(LoggedUserInfo.Code.ToString(CultureInfo.InvariantCulture));
                    }

                    if (menuItem.Name == "StorePeopleCounter")
                    {
                        para.Add(LoggedUserInfo.AllowedStores.First().ToString());
                    }

                    reportViewmodel.GenerateReport(item, para);
                    return;
                }

                GeneralFilter.NavigatToMenu(menuItem.Name, menuItem.Text, menuItem.MenuLink);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}