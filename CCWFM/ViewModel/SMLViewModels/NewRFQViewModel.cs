using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class NewRFQViewModel : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler VendorPopulatingCompleted;

        protected virtual void OnVendorPopulatingCompleted()
        {
            var handler = VendorPopulatingCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Fields ]

        private CommandsExecuter _addNewSubHeaderCommand;
        private List<Brand> _brands;
        private bool _isLoading;
        private CommandsExecuter _loadRfqCommand;
        private CommandsExecuter _newRfqCommand;
        private ObservableCollection<NewRFQDetailsViewModel> _rfqHeaderDeletedList;
        private CommandsExecuter _saveRfqCommand;
        private List<TblLkpSeason> _seasons;
        private NewRFQDetailsViewModel _selectedDetail;
        private string _styleCodeSearchTerm;
        private string BrandCodeField;
        private CommandsExecuter _filterVendorsCommand;
        private ObjectMode _formMode;
        private bool _isSearchingMode;
        private CommandsExecuter _toggleSearchCommand;
        private string _vendorSearchTerm;
        private string DocNumberField;
        private int IserialField;
        private string SeasonCodeField;
        private string SupplierCodeField;
        private ObservableCollection<NewRFQDetailsViewModel> tblNewRFQDetailsField;

        #endregion [ Fields ]

        #region [ Properties ]

        private ObservableCollection<Vendor> _vendors;
        private CommandsExecuter _cancelCommand;

        public ObjectMode FormMode
        {
            get { return _formMode; }
            set
            {
                _formMode = value;
                RaisePropertyChanged("FormMode");
            }
        }

        public string BrandCode
        {
            get
            {
                return BrandCodeField;
            }
            set
            {
                if ((ReferenceEquals(BrandCodeField, value))) return;
                BrandCodeField = value;
                RaisePropertyChanged("BrandCode");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Brand", Description = "Brand")]
        public List<Brand> Brands
        {
            get { return _brands ?? (_brands = new List<Brand>()); }
            set
            {
                _brands = value;
                RaisePropertyChanged("Brands");
            }
        }

        public string DocNumber
        {
            get
            {
                return DocNumberField;
            }
            set
            {
                if ((ReferenceEquals(DocNumberField, value))) return;
                DocNumberField = value;
                RaisePropertyChanged("DocNumber");
            }
        }

        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChanged("IsLoading"); }
        }

        public ObjectMode ObjectMode { get; set; }

        public ObservableCollection<NewRFQDetailsViewModel> RfqHeaderDeletedList
        {
            get { return _rfqHeaderDeletedList ?? (_rfqHeaderDeletedList = new ObservableCollection<NewRFQDetailsViewModel>()); }
            set { _rfqHeaderDeletedList = value; }
        }

        public string SeasonCode
        {
            get
            {
                return SeasonCodeField;
            }
            set
            {
                if ((ReferenceEquals(SeasonCodeField, value) != true))
                {
                    SeasonCodeField = value;
                    RaisePropertyChanged("SeasonCode");
                }
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Season", Description = "Season")]
        public List<TblLkpSeason> Seasons
        {
            get { return _seasons ?? (_seasons = new List<TblLkpSeason>()); }
            set
            {
                _seasons = value;
                RaisePropertyChanged("Seasons");
            }
        }

        public NewRFQDetailsViewModel SelectedDetail
        {
            get { return _selectedDetail; }
            set { _selectedDetail = value; RaisePropertyChanged("SelectedDetails"); }
        }

        public string StyleCodeSearchTerm
        {
            get { return _styleCodeSearchTerm; }
            set { _styleCodeSearchTerm = value; RaisePropertyChanged("StyleCodeSearchTerm"); }
        }

        public string SupplierCode
        {
            get
            {
                return SupplierCodeField;
            }
            set
            {
                if ((ReferenceEquals(SupplierCodeField, value) != true))
                {
                    SupplierCodeField = value;
                    RaisePropertyChanged("SupplierCode");
                }
            }
        }

        public ObservableCollection<NewRFQDetailsViewModel> tblNewRFQDetails
        {
            get
            {
                return tblNewRFQDetailsField ?? (tblNewRFQDetailsField = new ObservableCollection<NewRFQDetailsViewModel>());
            }
            set
            {
                if ((ReferenceEquals(tblNewRFQDetailsField, value) != true))
                {
                    tblNewRFQDetailsField = value;
                    RaisePropertyChanged("tblNewRFQDetails");
                }
            }
        }

        public ObservableCollection<Vendor> Vendros
        {
            get { return _vendors ?? (_vendors = new ObservableCollection<Vendor>()); }
            set
            {
                _vendors = value;
                RaisePropertyChanged("Vendros");
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public NewRFQViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;
            FillGenericCollection("tbl_lkp_Currency", RFQGlobalLkps.CurrenciesList);
            Client = new CRUD_ManagerServiceClient();
            Client.GetNewRfqCompleted += (s, e) =>
            {
                if (e.Error != null) return;
                MapToMe(e.Result);
            };
            Client.UpdateOrInsertNewRfqCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(strings.FailSavingMessage);
                    return;
                }
                MapToMe(e.Result);
                MessageBox.Show(strings.SavedMessage);
            };

            Client.GetAllBrandsCompleted += (s, e) =>
            {
                try
                {
                    Brands = new List<Brand>(e.Result);
                }
                catch (Exception ex)
                {
                    var err = new ErrorWindow(ex);
                    err.Show();
                }
            };
            Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);

            Client.GetAllSeasonsCompleted += (s, e) =>
            {
                Seasons = new List<TblLkpSeason>(e.Result);
            };
            Client.GetAllSeasonsAsync();

            tblNewRFQDetails.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (NewRFQDetailsViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                        var item1 = item;
                        item.DeleteDetails += (s1, e1) =>
                        {
                            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
                            if (res == MessageBoxResult.Cancel) return;
                            RfqHeaderDeletedList.Add(item1);
                            tblNewRFQDetails.Remove(item1);
                        };
                    }

                if (e.OldItems == null) return;
                foreach (NewRFQDetailsViewModel item in e.OldItems)
                {
                    item.PropertyChanged
                        -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                }
            };
            Client.SearchVendorsCompleted += (s, e) =>
            {
                if (e.Error != null) return;
                Vendros = new ObservableCollection<Vendor>(e.Result);
                OnVendorPopulatingCompleted();
            };
        }

        #endregion [ Constructors ]

        #region [ Commands ]

        public CommandsExecuter AddNewSubHeaderCommand
        {
            get
            {
                return _addNewSubHeaderCommand ??
                       (_addNewSubHeaderCommand = new CommandsExecuter(NewDetail) { IsEnabled = true });
            }
        }

        public CommandsExecuter FilterVendorsCommand
        {
            get { return _filterVendorsCommand ?? (_filterVendorsCommand = new CommandsExecuter(PupulateVendors) { IsEnabled = true }); }
        }

        public CommandsExecuter LoadNewRFQCommand
        {
            get { return _loadRfqCommand ?? (_loadRfqCommand = new CommandsExecuter(LoadRFQ) { IsEnabled = true }); }
        }

        public CommandsExecuter NewRFQCommand
        {
            get { return _newRfqCommand ?? (_newRfqCommand = new CommandsExecuter(NewRFQ) { IsEnabled = true }); }
            set { _newRfqCommand = value; RaisePropertyChanged("NewRFQCommand"); }
        }

        public CommandsExecuter SaveRFQCommand
        {
            get { return _saveRfqCommand ?? (_saveRfqCommand = new CommandsExecuter(SaveRFQ) { IsEnabled = true }); }
        }

        public CommandsExecuter ToggleSearchCommand
        {
            get { return _toggleSearchCommand ?? (_toggleSearchCommand = new CommandsExecuter(ToggleInFormSearch) { IsEnabled = true }); }
        }

        public CommandsExecuter CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new CommandsExecuter(Cancel) { IsEnabled = true }); }
        }

        public string VendorSearchTerm
        {
            get { return _vendorSearchTerm; }
            set { _vendorSearchTerm = value; RaisePropertyChanged("VendorSearchTerm"); }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void Cancel()
        {
            ChangeObjectMode(ObjectMode.StandBy);
        }

        public bool IsSearchingMode
        {
            get { return _isSearchingMode; }
            set { _isSearchingMode = value; RaisePropertyChanged("IsSearchingMode"); }
        }

        private void LoadRFQ()
        {
            Client.GetNewRfqAsync(DocNumber);
        }

        private void NewDetail()
        {
            tblNewRFQDetails.Add(new NewRFQDetailsViewModel());
        }

        private void NewRFQ()
        {
            ChangeObjectMode(ObjectMode.NewObject);
        }

        private void PupulateVendors()
        {
            Client.SearchVendorsAsync("ccm", VendorSearchTerm);
        }

        private void SaveRFQ()
        {
            Client.UpdateOrInsertNewRfqAsync(MapToModel(this));
        }

        private void ToggleInFormSearch()
        {
            if (FormMode == ObjectMode.LoadedFromDb)
                return;
            IsSearchingMode = !IsSearchingMode;
        }

        #endregion [ Commands bound methods ]

        #region [ Internal Logic ]

        private void ChangeObjectMode(ObjectMode o)
        {
            switch (o)
            {
                case ObjectMode.NewObject:
                    ResetObject();
                    break;

                case ObjectMode.LoadedFromDb:
                    break;

                case ObjectMode.MarkedForDeletion:
                    break;

                case ObjectMode.StandBy:
                    ResetObject();
                    break;
            }
        }

        private void MapToMe(tblNewRFQMainHeader tblNewRfqMainHeader)
        {
            if (tblNewRfqMainHeader == null) return;
            Iserial = tblNewRfqMainHeader.Iserial;
            SupplierCode = tblNewRfqMainHeader.SupplierCode;
            BrandCode = tblNewRfqMainHeader.BrandCode;
            DocNumber = tblNewRfqMainHeader.DocNumber;
            SeasonCode = tblNewRfqMainHeader.SeasonCode;
            tblNewRFQDetails.Clear();

            var tmp = (from x in tblNewRfqMainHeader.tblNewRFQDetails
                       select new NewRFQDetailsViewModel
                       {
                           Iserial = x.Iserial,
                           ColorCode = x.ColorCode,
                           StyleCode = x.StyleCode,
                           Cost = x.Cost,
                           Qty = x.Qty,
                           DeliveryDate = x.DeliveryDate,
                           MainHeader = x.MainHeader,
                           RFSNumber = x.RFSNumber,
                           LandedCost = x.LandedCost,
                           ExchangeRate = x.ExchangeRate,
                           RawCost = x.Cost != 0 && x.ExchangeRate != 0 ? (x.Cost / x.ExchangeRate) : 0,
                           IsSampleRequested = x.RFSNumber == null ? NewRFQDetailsViewModel.YesNoEnum.No : NewRFQDetailsViewModel.YesNoEnum.Yes,
                           tblNewRFQMainHeader = this,
                           tblNewRFQSizeDetails =
                               new ObservableCollection<NewRFQSizeDetailVM>
                                   (x.tblNewRFQSizeDetails
                                       .Select(s => new NewRFQSizeDetailVM
                                       {
                                           Iserial = s.Iserial,
                                           SizeRatio = s.SizeRatio,
                                           SizeCode = s.SizeCode,
                                           Qty = s.Qty,
                                           ParentSerial = x.Iserial
                                       }).ToList())
                       }).ToList();

            tmp.ForEach(x => tblNewRFQDetails.Add(x));
        }

        private tblNewRFQMainHeader MapToModel(NewRFQViewModel tblNewRfqMainHeader)
        {
            var ret = new tblNewRFQMainHeader
            {
                Iserial = tblNewRfqMainHeader.Iserial,
                SupplierCode = tblNewRfqMainHeader.SupplierCode,
                BrandCode = tblNewRfqMainHeader.BrandCode,
                DocNumber = tblNewRfqMainHeader.DocNumber,
                SeasonCode = tblNewRfqMainHeader.SeasonCode
            };

            var tmp = (from x in tblNewRfqMainHeader.tblNewRFQDetails
                       select new tblNewRFQDetail
                       {
                           Iserial = x.Iserial,
                           ColorCode = x.ColorCode,
                           StyleCode = x.StyleCode,
                           Cost = x.Cost,
                           Qty = x.Qty,
                           DeliveryDate = x.DeliveryDate,
                           MainHeader = x.MainHeader,
                           RFSNumber = x.RFSNumber,
                           LandedCost = x.LandedCost,
                           ExchangeRate = x.ExchangeRate,
                           CurrencyID = (x.SelectedCurrency != null ? (int?)x.SelectedCurrency.Iserial : null),
                           tblNewRFQSizeDetails =
                               new ObservableCollection<tblNewRFQSizeDetail>
                                   (x.tblNewRFQSizeDetails
                                       .Select(s => new tblNewRFQSizeDetail
                                       {
                                           Iserial = s.Iserial,
                                           SizeRatio = s.SizeRatio,
                                           SizeCode = s.SizeCode,
                                           Qty = s.Qty,
                                           ParentSerial = x.Iserial
                                       }).ToList())
                       }).ToList();
            ret.tblNewRFQDetails = new ObservableCollection<tblNewRFQDetail>();
            tmp.ForEach(x => ret.tblNewRFQDetails.Add(x));

            return ret;
        }

        private void ResetObject()
        {
            tblNewRFQDetails.Clear();
            Iserial = 0;
            BrandCode = string.Empty;
            SeasonCode = string.Empty;
            StyleCodeSearchTerm = string.Empty;
            VendorSearchTerm = string.Empty;
        }

        private static void FillGenericCollection(string tablEname, ObservableCollection<GenericViewModel> objectToFill)
        {
            if (objectToFill == null) objectToFill = new ObservableCollection<GenericViewModel>();
            var serviceClient = new CRUD_ManagerServiceClient();
            serviceClient.GetGenericAsync(tablEname, "%%", "%%", "%%", "Iserial", "ASC");

            serviceClient.GetGenericCompleted += (s, ev) =>
            {
                var i = 0;
                foreach (var item in ev.Result)
                {
                    objectToFill.Add(new GenericViewModel
                    {
                        Iserial = item.Iserial,
                        Code = item.Code,
                        Aname = item.Aname,
                        Ename = item.Ename
                    });
                    objectToFill[i].Status.IsChanged = false;
                    objectToFill[i].Status.IsNew = false;
                    objectToFill[i].Status.IsSavedDBItem = true;
                    i++;
                }
            };
            serviceClient.CloseAsync();
        }

        #endregion [ Internal Logic ]
    }
}