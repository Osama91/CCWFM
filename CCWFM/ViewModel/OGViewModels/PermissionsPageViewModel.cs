using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblAuthPermissionViewModel : GenericViewModel
    {
        public bool IsParanlCall { get; set; }

        public event EventHandler NotifyMyParent;

        protected virtual void OnNotifyMyParent()
        {
            var handler = NotifyMyParent;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public TblAuthPermissionViewModel()
        {
            SubPermissionsList.CollectionChanged += SubPermissionsList_CollectionChanged;
        }

        private void SubPermissionsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblAuthPermissionViewModel item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                    item.NotifyMyParent += item_NotifyMyParent;
                }

            if (e.OldItems == null) return;
            foreach (TblAuthPermissionViewModel item in e.OldItems)
            {
                item.PropertyChanged -= item_PropertyChanged;
                item.NotifyMyParent -= item_NotifyMyParent;
            }
        }

        private void item_NotifyMyParent(object sender, EventArgs e)
        {
            if (!SubPermissionsList.Any()) return;
            IsParanlCall = true;
            if (SubPermissionsList.All(x => x.IsSelected == true))
            {
                IsSelected = true;
                return;
            }
            if (SubPermissionsList.All(x => x.IsSelected == false))
            {
                IsSelected = false;
                return;
            }
            IsSelected = null;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private bool? _isSelected;

        public bool? IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                OnNotifyMyParent();

                if (!IsParanlCall)
                {
                    if (SubPermissionsList.Any())
                    {
                        SubPermissionsList.ToList().ForEach(x => x.IsSelected = value);
                    }
                }
                else
                {
                    IsParanlCall = false;
                }
            }
        }

        private string _menuLinkField;

        private int? _parentPermField;

        private int? _permOrderField;

        private string _permissionTypField;

        private int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        public string MenuLink
        {
            get
            {
                return _menuLinkField;
            }
            set
            {
                if ((ReferenceEquals(_menuLinkField, value) != true))
                {
                    _menuLinkField = value;
                    RaisePropertyChanged("MenuLink");
                }
            }
        }

        public int? ParentPerm
        {
            get
            {
                return _parentPermField;
            }
            set
            {
                if ((_parentPermField.Equals(value) != true))
                {
                    _parentPermField = value;
                    RaisePropertyChanged("ParentPerm");
                }
            }
        }

        public int? PermOrder
        {
            get
            {
                return _permOrderField;
            }
            set
            {
                if ((_permOrderField.Equals(value) != true))
                {
                    _permOrderField = value;
                    RaisePropertyChanged("PermOrder");
                }
            }
        }

        public string PermissionTyp
        {
            get
            {
                return _permissionTypField;
            }
            set
            {
                if ((ReferenceEquals(_permissionTypField, value) != true))
                {
                    _permissionTypField = value;
                    RaisePropertyChanged("PermissionTyp");
                }
            }
        }

        private string _imageKey;

        public string ImageKey
        {
            get { return _imageKey; }
            set { _imageKey = value; RaisePropertyChanged("ImageKey"); }
        }

        private ImageSource _selectedImage;

        public ImageSource SelectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; RaisePropertyChanged("SelectedImage"); }
        }

        private ObservableCollection<TblAuthPermissionViewModel> _subPermissionsList;

        public ObservableCollection<TblAuthPermissionViewModel> SubPermissionsList
        {
            get { return _subPermissionsList ?? (_subPermissionsList = new ObservableCollection<TblAuthPermissionViewModel>()); }
            set { _subPermissionsList = value; RaisePropertyChanged("SubPermissionsList"); }
        }

        private bool _allowAdd;

        private bool _allowDelete;

        private bool _allowUpdate;

        public bool AllowNew
        {
            get { return _allowAdd; }
            set { _allowAdd = value; RaisePropertyChanged("AllowNew"); }
        }

        public bool AllowDelete
        {
            get { return _allowDelete; }
            set { _allowDelete = value; RaisePropertyChanged("AllowDelete"); }
        }

        public bool AllowUpdate
        {
            get { return _allowUpdate; }
            set { _allowUpdate = value; RaisePropertyChanged("AllowUpdate"); }
        }
    }

    public class PermissionsPageViewModel : ViewModelBase
    {
        public PermissionsPageViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                TblAuthPermissionsList = new ObservableCollection<TblAuthPermissionViewModel>();
                TblAuthPermissionsList.CollectionChanged += TblAuthPermissionsList_CollectionChanged;
                Jobs = new ObservableCollection<TblAuthJob>();
                Client.GetAllJobsAsync();
                Client.GetAllJobsCompleted += (s, j) =>
                {
                    Jobs = j.Result;
                };
                Loading = true;
                Client.GetAllPermissionsAsync();
                Client.GetUserJobPermissionsCompleted += (s, sv) => CheckAll(sv.Result);
                Client.GetAllPermissionsCompleted += (s, sv) =>
                    DisplayPermtions(sv.Result.ToList());

                Client.SaveJobPermCompleted += (s, sv) => MessageBox.Show("SavedSuccefully");
            }
        }

        private void CheckAll(ObservableCollection<TblAuthJobPermission> jobPermissions)
        {
            for (var i = 0; i < TreeView.Items.Count; i++)
            {
                CheckSomeTimes((TreeViewItem)TreeView.ItemContainerGenerator.ContainerFromIndex(i), jobPermissions);
            }
            Loading = false;
        }

        private void CheckSomeTimes(TreeViewItem currentTreeViewItem, ObservableCollection<TblAuthJobPermission> jobper)
        {
            var currentPerm = currentTreeViewItem.DataContext as TblAuthPermissionViewModel;



            if (currentPerm.Iserial == 569)
            {

            }
            if (currentPerm.Ename != "Add" && currentPerm.Ename != "Update" && currentPerm.Ename != "Delete")
            {
                var formPrem = jobper.SingleOrDefault(x => x.TblPermission == currentPerm.Iserial);
                currentTreeViewItem.IsSelected = jobper.Any(x => x.TblPermission == currentPerm.Iserial);

                for (var i = 0; i < currentTreeViewItem.Items.Count; i++)
                {
                    var child = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
                    if (child != null && child.DataContext != null)
                    {
                        var checkbox = (TblAuthPermissionViewModel)child.DataContext;

                        checkbox.IsSelected = jobper.Any(x => x.TblPermission == checkbox.Iserial);

                        if (!string.IsNullOrEmpty(checkbox.ImageKey) && formPrem != null)
                        {
                            switch (checkbox.Ename)
                            {
                                case "Add":
                                    checkbox.IsSelected = formPrem.AllowNew;
                                    break;

                                case "Update":
                                    checkbox.IsSelected = formPrem.AllowUpdate;
                                    break;

                                case "Delete":
                                    checkbox.IsSelected = formPrem.AllowDelete;
                                    break;
                            }
                        }
                        else
                        {
                            CheckSomeTimes(child, jobper);
                        }
                    }
                }
            }
        }

        private void TblAuthPermissionsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TblAuthPermissionViewModel item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (TblAuthPermissionViewModel item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void DisplayPermtions(List<TblAuthPermission> result)
        {
            GenericMapper.InjectFromObCollection(TblAuthPermissionsList, result.Where(x => x.ParentPerm == 0));

            foreach (var tblAuthPermission in TblAuthPermissionsList)
            {
               
                //  GetImageListKey(tblAuthPermission, tblAuthPermission.PermissionTyp);
                FillMyChild(tblAuthPermission, result.Where(x => x.ParentPerm != 0).ToList());
            }
            Loading = false;
        }

        private void FillMyChild(TblAuthPermissionViewModel tblAuthPermission, List<TblAuthPermission> result)
        {
            var query = (from x in result
                         where x.ParentPerm == tblAuthPermission.Iserial
                         select x).OrderBy(x => x.PermOrder).ToList();
         
            if (query.Count > 0)
            {
                GenericMapper.InjectFromObCollection(tblAuthPermission.SubPermissionsList, query);
                foreach (var authPermission in tblAuthPermission.SubPermissionsList)
                {

                    FillMyChild(new TblAuthPermissionViewModel().InjectFrom(authPermission) as TblAuthPermissionViewModel, result);
                }
            }

            GetImageListKey(tblAuthPermission, tblAuthPermission.PermissionTyp);

            // var a = TblAuthPermissionsList.Where(x=>x.SubPermissionsList.Where(s=>x.Ename.Contains("fabric Insp"));
        }

        private void GetImageListKey(TblAuthPermissionViewModel tblAuthPermission, string permissionType)
        {



            if (tblAuthPermission.Iserial == 569)
            {

            }
            const string src = "/CCWFM;component/Images/ImageListIcons/";
            var converter = new ImageSourceConverter();

            var iconKey = "";
            switch (permissionType.ToUpper().Trim())
            {
                case "F":
                    iconKey = "FormIcon";
                    break;

                case "FU":
                    iconKey = "DetailsIcon";
                    break;

                case "FD":
                    iconKey = "TransIcon";
                    break;

                case "C":
                    iconKey = "ChildIcon";
                    break;

                case "M":
                    iconKey = "MenuIcon";
                    break;

                case "R":
                    iconKey = "ReportIcon";
                    break;
            }
            tblAuthPermission.ImageKey = iconKey;

            switch (iconKey)
            {
                case "MenuIcon":
                    tblAuthPermission.SelectedImage = (ImageSource)converter.ConvertFromString(src + "Menu.png");

                    break;

                case "FormIcon":
                    tblAuthPermission.SelectedImage = (ImageSource)converter.ConvertFromString(src + "Form.png");

                    //CCWFM;component/Images/ImageListIcons/Form.png
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Add",
                        ImageKey = "Add",

                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Add.png")
                    });
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Update",
                        ImageKey = "Update",
                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Update.png")
                    });
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Delete",
                        ImageKey = "Delete",
                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Delete.png")
                    });
                    break;

                case "DetailsIcon":
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Update",
                        ImageKey = "Update",
                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Update.png")
                    });
                    break;

                case "TransIcon":
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Add",
                        ImageKey = "Add",
                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Add.png")
                    });
                    tblAuthPermission.SubPermissionsList.Add(new TblAuthPermissionViewModel
                    {
                        Ename = "Update",
                        ImageKey = "Update",
                        SelectedImage = (ImageSource)converter.ConvertFromString(src + "Update.png")
                    });
                    break;
            }
        }

        #region Props

        private ObservableCollection<TblAuthJob> _jobs;

        public ObservableCollection<TblAuthJob> Jobs
        {
            get { return _jobs ?? (_jobs = new ObservableCollection<TblAuthJob>()); }
            set
            {
                _jobs = value;
                RaisePropertyChanged("Jobs");
            }
        }

        private TblAuthJob _selectedJob;

        public TblAuthJob SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                RaisePropertyChanged("SelectedJob");
            }
        }

        private ObservableCollection<TblAuthPermissionViewModel> _tblAuthPermissionsList;

        public ObservableCollection<TblAuthPermissionViewModel> TblAuthPermissionsList
        {
            get { return _tblAuthPermissionsList; }
            set
            {
                _tblAuthPermissionsList = value;
                RaisePropertyChanged("TblAuthPermissionsList");
            }
        }

        public TreeView TreeView { get; set; }

        #endregion Props

        internal void GetPermissionForJob(int userId, TreeView treeView)
        {
            Loading = true;
            Client.GetUserJobPermissionsAsync(userId);
            TreeView = treeView;

            // Client.GetUserJobAsync();
        }

        internal void SavePerm()
        {
            var temp = TblAuthPermissionsList.Where(x => x.IsSelected == true || x.IsSelected == null);
            var tempTodelete = TblAuthPermissionsList.Where(x => x.IsSelected == false);

            if (temp != null || tempTodelete != null)
            {
                var listToSaveTemp = new ObservableCollection<TblAuthPermissionViewModel>(temp);
                var listToDeleteTemp = new ObservableCollection<TblAuthPermissionViewModel>(tempTodelete);
                var listToSave = new ObservableCollection<TblAuthJobPermission>();
                foreach (var row in listToSaveTemp)
                {
                    listToSave.Add(new TblAuthJobPermission
                    {
                        TblPermission = row.Iserial,
                        Tbljob = SelectedJob.Iserial,
                    });
                }

                var listToDelete = new ObservableCollection<TblAuthJobPermission>();

                foreach (var row in listToDeleteTemp)
                {
                    listToDelete.Add(new TblAuthJobPermission
                    {
                        TblPermission = row.Iserial,
                        Tbljob = SelectedJob.Iserial,
                    });
                }
                foreach (var row in temp)
                {
                    FillToSave(row, listToSave, listToDelete);
                }

                foreach (var row in tempTodelete)
                {
                    FillToSave(row, listToSave, listToDelete);
                }
                var temptosave = listToSave.Distinct();
                if (temptosave.Any(x => x.TblPermission == 304))
                {
                }
                var tempToDelete = listToDelete.Distinct();
                if (tempToDelete.Any(x => x.TblPermission == 304))
                {
                }
                Client.SaveJobPermAsync(new ObservableCollection<TblAuthJobPermission>(temptosave), new ObservableCollection<TblAuthJobPermission>(tempToDelete), SelectedJob.Iserial);
            }
        }

        private void FillToSave(TblAuthPermissionViewModel row, ObservableCollection<TblAuthJobPermission> listToSave, ObservableCollection<TblAuthJobPermission> listToDelete)
        {
            if (row.Iserial == 304)
            {
            }

            foreach (var currentPerm in row.SubPermissionsList)
            {
                if (row.PermissionTyp == "F")
                {
                    switch (currentPerm.Ename)
                    {
                        case "Add":
                            row.AllowNew = currentPerm.IsSelected != null && (bool)currentPerm.IsSelected;

                            break;

                        case "Update":
                            row.AllowUpdate = currentPerm.IsSelected != null && (bool)currentPerm.IsSelected;

                            break;

                        case "Delete":
                            row.AllowDelete = currentPerm.IsSelected != null && (bool)currentPerm.IsSelected;
                            break;
                    }
                }

                //if (row.PermissionTyp == "F" && currentPerm.PermissionTyp == null)
                //{
                //    if (currentPerm.IsSelected == false)
                //    {
                //        listToDelete.Add(new TblAuthJobPermission
                //        {
                //            TblPermission = row.Iserial,
                //            Tbljob = SelectedJob.Iserial,
                //        });
                //    }
                //    else
                //    {
                //        listToSave.Add(new TblAuthJobPermission
                //        {
                //            TblPermission = row.Iserial,
                //            Tbljob = SelectedJob.Iserial,
                //            AllowDelete = row.AllowDelete,
                //            AllowNew = row.AllowNew,
                //            AllowUpdate = row.AllowUpdate,
                //        });
                //    }
                //}
                if (currentPerm.PermissionTyp != null)
                {
                    FillToSave(currentPerm, listToSave, listToDelete);
                }
            }

            if (row.PermissionTyp != null)
            {
                if (row.IsSelected == false)
                {
                    listToDelete.Add(new TblAuthJobPermission
                    {
                        TblPermission = row.Iserial,
                        Tbljob = SelectedJob.Iserial,
                    });
                }
                else
                {
                    listToSave.Add(new TblAuthJobPermission
                    {
                        TblPermission = row.Iserial,
                        Tbljob = SelectedJob.Iserial,
                        AllowDelete = row.AllowDelete,
                        AllowUpdate = row.AllowUpdate,
                        AllowNew = row.AllowNew
                    });
                }
            }
        }

        internal void CopyPerm(int Job)
        {
            Client.CopyPermissionAsync(Job);
        }
    }
}