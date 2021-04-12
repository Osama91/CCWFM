using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblStyleTNAStatusDetailModel : PropertiesViewModelBase
    {
        private string CommentField;

        private int _iserialField;

        private DateTime RequestDateField;

        private int TblAuthUserField;


        private int TblStyleTNAHeaderField;

        private GenericTable TblStyleTNAStatuField;

        private int TblStyleTnaStatusField;

        public string Comment
        {
            get
            {
                return this.CommentField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CommentField, value) != true))
                {
                    this.CommentField = value;
                    this.RaisePropertyChanged("Comment");
                }
            }
        }

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                    if (Iserial != 0)
                    {
                        Locked = true;
                    }
                }
            }
        }
        private bool _locked;

        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; RaisePropertyChanged("Locked"); }
        }

        public DateTime RequestDate
        {
            get
            {
                return this.RequestDateField;
            }
            set
            {

                this.RequestDateField = value;
                this.RaisePropertyChanged("RequestDate");

            }
        }

        public int TblAuthUser
        {
            get
            {
                return this.TblAuthUserField;
            }
            set
            {
                if ((this.TblAuthUserField.Equals(value) != true))
                {
                    this.TblAuthUserField = value;
                    this.RaisePropertyChanged("TblAuthUser");
                }
            }
        }

        private ProductionService.TblAuthUser _userPerRow;

        public ProductionService.TblAuthUser UserPerRow
        {
            get { return _userPerRow ?? (_userPerRow = new ProductionService.TblAuthUser()); }
            set
            {
                _userPerRow = value; RaisePropertyChanged("UserPerRow");
                if (UserPerRow != null) TblAuthUser = UserPerRow.Iserial;
            }
        }

        public int TblStyleTNAHeader
        {
            get
            {
                return TblStyleTNAHeaderField;
            }
            set
            {

                TblStyleTNAHeaderField = value;
                RaisePropertyChanged("TblStyleTNAHeader");

            }
        }

        public GenericTable StyleTNAStatusPerRow
        {
            get
            {
                return TblStyleTNAStatuField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblStyleTNAStatuField, value) != true))
                {
                    this.TblStyleTNAStatuField = value;
                    this.RaisePropertyChanged("StyleTNAStatusPerRow");
                }
            }
        }

        public int TblStyleTnaStatus
        {
            get
            {
                return this.TblStyleTnaStatusField;
            }
            set
            {
                if ((this.TblStyleTnaStatusField.Equals(value) != true))
                {
                    this.TblStyleTnaStatusField = value;
                    this.RaisePropertyChanged("TblStyleTnaStatus");
                }
            }
        }


    }

    public class StyleTNAStatusViewModel : ViewModelBase
    {

        internal ProductionService.ProductionServiceClient ProductionClient = new ProductionService.ProductionServiceClient();

        public StyleTNAStatusViewModel(StyleHeaderViewModel styleViewModel)
        {
            TempStyleViewModel = styleViewModel;

            if (!DesignerProperties.IsInDesignTool)
            {

                MainRowList = new ObservableCollection<TblStyleTNAStatusDetailModel>();
                SelectedMainRow = new TblStyleTNAStatusDetailModel();

                ProductionClient.GetTblStyleTNAStatusDetailCompleted += (s, sv) =>
                {
                    if (sv.Error != null)
                    {
                        MessageBox.Show(sv.Error.Message);
                    }

                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStyleTNAStatusDetailModel();
                        newrow.InjectFrom(row);
                        newrow.UserPerRow = row.TblAuthUser1;
                        newrow.StyleTNAStatusPerRow = new GenericTable();
                        if (row.TblStyleTNAStatu != null)
                        {
                            GenericTable newTempRow = new GenericTable().InjectFrom(row.TblStyleTNAStatu) as GenericTable;
                            newrow.StyleTNAStatusPerRow = newTempRow;
                        }
                        MainRowList.Add(newrow);
                    }

                    AddNewMainRow();
                    Loading = false;
                };

                ProductionClient.GetTblUsersStyleTNAStatusAsync(LoggedUserInfo.Iserial);

                ProductionClient.GetTblUsersStyleTNAStatusCompleted += (s, sv) =>
                {
                    StyleTNAStatusList.Clear();
                    FullStyleTNAStatusList.Clear();
                    foreach (var item in sv.Result.ToList())
                    {
                        FullStyleTNAStatusList.Add(new ProductionService.TblStyleTNAStatu().InjectFrom(item) as ProductionService.TblStyleTNAStatu);
                    }
                    foreach (var item in sv.Result.ToList())
                    {
                        if (!CheckIfEnableEdit(TempStyleViewModel.SelectedTnaRow.TblStyleTNAStatus))
                        {
                            if (item.DisplayAfterApprove)
                            {
                                StyleTNAStatusList.Add(new ProductionService.TblStyleTNAStatu().InjectFrom(item) as ProductionService.TblStyleTNAStatu);
                            }

                        }
                        else
                        {
                            if (!item.DisplayAfterApprove)
                            {
                                StyleTNAStatusList.Add(new ProductionService.TblStyleTNAStatu().InjectFrom(item) as ProductionService.TblStyleTNAStatu);
                            }
                        }




                    }

                };
                ProductionClient.UpdateOrInsertTblStyleTNAStatusDetailCompleted += (s, sv) =>
                {
                    TempStyleViewModel.SelectedTnaRow.TblStyleTNAStatus = sv.Result.TblStyleTnaStatus;
                };

                GetMaindata();
            }
        }

        StyleHeaderViewModel _tempStyleViewModel;
        public StyleHeaderViewModel TempStyleViewModel
        {
            get { return _tempStyleViewModel; }
            set { _tempStyleViewModel = value; RaisePropertyChanged("TempStyleViewModel"); }
        }

        public void GetMaindata()
        {
            ProductionClient.GetTblStyleTNAStatusDetailAsync(TempStyleViewModel.SelectedTnaRow.Iserial);
        }

        public void SaveMainRow()
        {
            
            foreach (var row in MainRowList.Where(x => x.Iserial == 0))
            {
                var isvalid = Validator.TryValidateObject(row,
                    new ValidationContext(row, null, null), null, true);

                if (isvalid)
                {
                    var saveRow = new ProductionService.TblStyleTNAStatusDetail();

                    saveRow.InjectFrom(row);
                    int x = MainRowList.IndexOf(row);
                    ProductionClient.UpdateOrInsertTblStyleTNAStatusDetailAsync(saveRow, MainRowList.IndexOf(row));
                    TempStyleViewModel.Loading = true;
                    Loading = true;
                }
            }
        }

        public void AddNewMainRow()
        {
           
            var newRow = new TblStyleTNAStatusDetailModel
            {
                UserPerRow =
                {
                    Iserial = LoggedUserInfo.Iserial,
                    UserName = LoggedUserInfo.WFM_UserName,
                    Ename=LoggedUserInfo.Ename
                },
                TblAuthUser = LoggedUserInfo.Iserial,
                TblStyleTNAHeader = TempStyleViewModel.SelectedTnaRow.Iserial,
                RequestDate = DateTime.Now,
            };
            MainRowList.Add(newRow);
        }

        public bool CheckIfEnableEdit(int status)
        {
            return FullStyleTNAStatusList.FirstOrDefault(w => w.Iserial == status).EnableEdit;

        }
        #region Prop

        private ObservableCollection<TblStyleTNAStatusDetailModel> _mainRowList;

        public ObservableCollection<TblStyleTNAStatusDetailModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }



        private TblStyleTNAStatusDetailModel _selectedMainRow;

        public TblStyleTNAStatusDetailModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private ObservableCollection<ProductionService.TblStyleTNAStatu> _StyleTNAStatusList;

        public ObservableCollection<ProductionService.TblStyleTNAStatu> StyleTNAStatusList
        {
            get { return _StyleTNAStatusList ?? (_StyleTNAStatusList = new ObservableCollection<ProductionService.TblStyleTNAStatu>()); }
            set { _StyleTNAStatusList = value; RaisePropertyChanged("StyleTNAStatusList"); }
        }

        private ObservableCollection<ProductionService.TblStyleTNAStatu> _FullStyleTNAStatusList;

        public ObservableCollection<ProductionService.TblStyleTNAStatu> FullStyleTNAStatusList
        {
            get { return _FullStyleTNAStatusList ?? (_FullStyleTNAStatusList = new ObservableCollection<ProductionService.TblStyleTNAStatu>()); }
            set { _FullStyleTNAStatusList = value; RaisePropertyChanged("FullStyleTNAStatusList"); }
        }

        #endregion Prop
    }
}