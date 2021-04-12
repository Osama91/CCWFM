using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class NewRFQDetailsViewModel : ViewModelBase
    {
        #region [ Enums ]

        public enum YesNoEnum
        {
            Yes,
            No
        }

        #endregion [ Enums ]

        #region [ Events ]

        public event EventHandler DeleteDetails;

        protected virtual void OnDeleteDetails()
        {
            var handler = DeleteDetails;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Fields ]

        private string ColorCodeField;

        private decimal? CostField;

        private DateTime? DeliveryDateField;

        private int IserialField;

        private int? MainHeaderField;

        private int? QtyField;

        private int? RFSNumberField;

        private string StyleCodeField;

        private NewRFQViewModel tblNewRFQMainHeaderField;

        private ObservableCollection<NewRFQSizeDetailVM> tblNewRFQSizeDetailsField;
        private CommandsExecuter _deleteLineCommand;
        private YesNoEnum _isSampleRequested;
        private string _styleSearchTerm;
        private CommandsExecuter _filterStyleCommand;
        private List<SMLDTO> _smlStyles;
        private SMLDTO _selectedSmldto;
        private List<GenericViewModel> _currencies;
        private decimal? _exchangeRate;
        private decimal? _landedCost;
        private GenericViewModel _selectedCurrency;
        private decimal? _rawCost;

        #endregion [ Fields ]

        #region [ Properties ]

        public SMLDTO SelectedSmldto
        {
            get { return _selectedSmldto; }
            set
            {
                if (value == null || ReferenceEquals(_selectedSmldto, value)) return;
                _selectedSmldto = value;
                RaisePropertyChanged("SelectedSmldto");
                StyleCode = value.StyleCode;
                ColorCode = value.ColorCode;
                Qty = value.Qty;
                IsSampleRequested = value.IsSampleRequested ? YesNoEnum.Yes : YesNoEnum.No;
                tblNewRFQSizeDetails = new ObservableCollection<NewRFQSizeDetailVM>();
                tblNewRFQSizeDetails.Clear();
                foreach (var size in value.SmlSizes)
                {
                    tblNewRFQSizeDetails.Add(new NewRFQSizeDetailVM
                    {
                        Qty = size.Qty,
                        SizeCode = size.SizeCode,
                        SizeRatio = size.Ratio
                    });
                }
            }
        }

        public List<SMLDTO> SMLStyles
        {
            get { return _smlStyles; }
            set { _smlStyles = value; RaisePropertyChanged("SMLStyles"); }
        }

        public string StyleSearchTerm
        {
            get { return _styleSearchTerm; }
            set { _styleSearchTerm = value; RaisePropertyChanged("StyleSearchTerm"); }
        }

        public string ColorCode
        {
            get
            {
                return ColorCodeField;
            }
            set
            {
                if ((ReferenceEquals(ColorCodeField, value) != true))
                {
                    ColorCodeField = value;
                    RaisePropertyChanged("ColorCode");
                }
            }
        }

        [Display(ResourceType = typeof(strings), Name = "localCost")]
        public decimal? Cost
        {
            get
            {
                return CostField;
            }
            set
            {
                if ((CostField.Equals(value) != true))
                {
                    CostField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }

        public DateTime? DeliveryDate
        {
            get
            {
                return DeliveryDateField;
            }
            set
            {
                if ((DeliveryDateField.Equals(value) != true))
                {
                    DeliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
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

        public int? MainHeader
        {
            get
            {
                return MainHeaderField;
            }
            set
            {
                if ((MainHeaderField.Equals(value) != true))
                {
                    MainHeaderField = value;
                    RaisePropertyChanged("MainHeader");
                }
            }
        }

        public int? Qty
        {
            get
            {
                return QtyField;
            }
            set
            {
                if ((QtyField.Equals(value) != true))
                {
                    QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        public int? RFSNumber
        {
            get
            {
                return RFSNumberField;
            }
            set
            {
                if ((RFSNumberField.Equals(value) != true))
                {
                    RFSNumberField = value;
                    RaisePropertyChanged("RFSNumber");
                }
            }
        }

        public string StyleCode
        {
            get
            {
                return StyleCodeField;
            }
            set
            {
                if ((ReferenceEquals(StyleCodeField, value) != true))
                {
                    StyleCodeField = value;
                    RaisePropertyChanged("StyleCode");
                }
            }
        }

        public NewRFQViewModel tblNewRFQMainHeader
        {
            get
            {
                return tblNewRFQMainHeaderField;
            }
            set
            {
                if ((ReferenceEquals(tblNewRFQMainHeaderField, value) != true))
                {
                    tblNewRFQMainHeaderField = value;
                    RaisePropertyChanged("tblNewRFQMainHeader");
                }
            }
        }

        public ObservableCollection<NewRFQSizeDetailVM> tblNewRFQSizeDetails
        {
            get
            {
                return tblNewRFQSizeDetailsField;
            }
            set
            {
                if ((ReferenceEquals(tblNewRFQSizeDetailsField, value) != true))
                {
                    tblNewRFQSizeDetailsField = value;
                    RaisePropertyChanged("tblNewRFQSizeDetails");
                }
            }
        }

        [Display(Name = "Sample Requseted")]
        public YesNoEnum IsSampleRequested
        {
            get { return _isSampleRequested; }
            set { _isSampleRequested = value; RaisePropertyChanged("IsSampleRequested"); }
        }

        [Display(ResourceType = typeof(strings), Name = "Currency")]
        public GenericViewModel SelectedCurrency
        {
            get { return _selectedCurrency; }
            set { _selectedCurrency = value; RaisePropertyChanged("SelectedCurrency"); }
        }

        public List<GenericViewModel> Currencies
        {
            get { return _currencies ?? (_currencies = new List<GenericViewModel>(RFQGlobalLkps.CurrenciesList)); }
            set { _currencies = value; }
        }

        [Display(ResourceType = typeof(strings), Name = "Cost")]
        public decimal? RawCost
        {
            get { return _rawCost; }
            set
            {
                _rawCost = value;
                Cost = ExchangeRate * value;
            }
        }

        [Display(ResourceType = typeof(strings), Name = "ExchRate")]
        public decimal? ExchangeRate
        {
            get { return _exchangeRate; }
            set
            {
                _exchangeRate = value;
                RaisePropertyChanged("ExchangeRate");
                Cost = RawCost * value;
            }
        }

        [Display(ResourceType = typeof(strings), Name = "AdditionalCost")]
        public decimal? LandedCost
        {
            get { return _landedCost; }
            set { _landedCost = value; RaisePropertyChanged("LandedCost"); }
        }

        #endregion [ Properties ]

        #region [ Constructor(s) ]

        public NewRFQDetailsViewModel()
        {
        }

        #endregion [ Constructor(s) ]

        #region [ Commands ]

        public CommandsExecuter DeleteLineCommand
        {
            get { return _deleteLineCommand ?? (_deleteLineCommand = new CommandsExecuter(DeleteMe) { IsEnabled = true }); }
        }

        public CommandsExecuter FilterStyleCommand
        {
            get { return _filterStyleCommand ?? (_filterStyleCommand = new CommandsExecuter(FilterStyles, FilterStyleArgs) { IsEnabled = true }); }
        }

        #endregion [ Commands ]

        #region [ Command's bound methods ]

        private void DeleteMe()
        {
            OnDeleteDetails();
        }

        private void FilterStyles(object sender)
        {
            var client = new CRUD_ManagerServiceClient();
            var autobox = (sender as AutoCompleteBox);
            if (autobox == null) return;
            client.SearchSMLStylesAsync(autobox.Text);
            client.SearchSMLStylesCompleted += (s, e) =>
            {
                if (e.Error != null) return;
                SMLStyles = new List<SMLDTO>(e.Result);
                autobox.PopulateComplete();
            };
        }

        private bool FilterStyleArgs(object sender)
        {
            return true;
        }

        #endregion [ Command's bound methods ]

        #region [ Internal Logic ]

        #endregion [ Internal Logic ]
    }
}