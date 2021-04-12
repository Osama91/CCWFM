using System.Collections.ObjectModel;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.RFQViewModels
{
    public static class RFQGlobalLkps
    {
        public static ObservableCollection<TblColor> ColorsList { get; set; }

        public static ObservableCollection<GenericViewModel> CostTypeList { get; set; }

        public static ObservableCollection<GenericViewModel> CurrenciesList { get; set; }

        public static ObservableCollection<GenericViewModel> FollowupStatusesList { get; set; }

        public static ObservableCollection<SizesWithGroups> StatisSizesCollection { get; set; }
    }
}