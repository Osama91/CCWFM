using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class RFQCostingDetail : ViewModelBase
    {
        #region [ Public Properties ]

        private decimal? _cost;

        public decimal? Cost
        {
            get { return _cost; }
            set { _cost = value; RaisePropertyChanged("Cost"); }
        }

        #endregion [ Public Properties ]
    }

    public class RFQCostingViewModel : ViewModelBase
    {
        #region [ Properties ]

        private ObservableCollection<tbl_RFQ_AdditionalCost> _additionalCostList;

        private List<GenericViewModel> _CostTypes;

        private List<GenericViewModel> _currencies;

        private List<RFQCostingDetail> _fotterDumpSource;

        private decimal _totalLocalValue;

        public ObservableCollection<tbl_RFQ_AdditionalCost> AdditionalCostList
        {
            get { return _additionalCostList ?? (_additionalCostList = new ObservableCollection<CRUDManagerService.tbl_RFQ_AdditionalCost>()); }
            set { _additionalCostList = value; RaisePropertyChanged("AdditionalCostList"); }
        }

        public List<GenericViewModel> CostTypes
        {
            get { return _CostTypes ?? (_CostTypes = new List<GenericViewModel>(RFQGlobalLkps.CostTypeList)); }
            set { _CostTypes = value; RaisePropertyChanged("CostTypes"); }
        }

        public List<GenericViewModel> Currencies
        {
            get { return _currencies ?? (_currencies = new List<GenericViewModel>(RFQGlobalLkps.CurrenciesList)); }
            set { _currencies = value; }
        }

        public List<RFQCostingDetail> FotterDumpSource
        {
            get { return _fotterDumpSource; }
            set { _fotterDumpSource = value; RaisePropertyChanged("FotterDumpSource"); }
        }

        public decimal TotalocalValue
        {
            get { return _totalLocalValue; }
            set { _totalLocalValue = value; RaisePropertyChanged("TotalocalValue"); }
        }

        #endregion [ Properties ]

        #region [ Constructor(s) ]

        public RFQCostingViewModel()
        {
            AdditionalCostList.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                        foreach (tbl_RFQ_AdditionalCost item in e.NewItems)
                        {
                            var item1 = item;
                            item1.PropertyChanged
                                += (s1, e1) =>
                                {
                                    RaisePropertyChanged(e1.PropertyName);
                                    if (e1.PropertyName == "ExchangeRate" || e1.PropertyName == "CostValue")
                                        item1.LocalValue = decimal.Parse(((item1.ExchangeRate ?? 0) * item1.CostValue).ToString());

                                    if (e1.PropertyName == "LocalValue")
                                        FotterDumpSource[0].Cost = TotalocalValue = (decimal)AdditionalCostList.Sum(x => x.LocalValue);
                                };
                        }

                    if (e.OldItems != null)
                        foreach (tbl_RFQ_AdditionalCost item in e.OldItems)
                        {
                            item.PropertyChanged
                                -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                        }
                };
            FotterDumpSource = new List<RFQCostingDetail>();
            FotterDumpSource.Add(new RFQCostingDetail());
        }

        #endregion [ Constructor(s) ]
    }
}