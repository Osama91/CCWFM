using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CCWFM.ViewModel.RFQViewModels;
using CCWFM.CRUDManagerService;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace CCWFM.Views.RequestForQutation
{
    public partial class RFQCostingChild
    {
        public event EventHandler<CostSubmittingEventArgs> SubmitCosts;
        public virtual void OnCostSubmitted()
        {
            var handler = SubmitCosts;
            if (handler == null) return;
            var vm = (LayoutRoot.DataContext as RFQCostingViewModel);
            if (vm == null) return;
            if (vm.AdditionalCostList.Count <= 0) return;
            var cost = vm.FotterDumpSource[0].Cost;
            if (cost == null) return;
            var args = new CostSubmittingEventArgs((decimal)cost, vm.AdditionalCostList);
            handler(this, args);
        }
        public RFQCostingChild(IEnumerable<tbl_RFQ_AdditionalCost> childList)
        {
            InitializeComponent();
            var vm = (LayoutRoot.DataContext as RFQCostingViewModel);
            if (vm == null) return;
            vm.AdditionalCostList.Clear();
            
            foreach (var item in childList)
            {
                vm.AdditionalCostList.Add(item);
            }
            vm.FotterDumpSource[0].Cost = vm.AdditionalCostList.Sum(x => x.LocalValue);
        }

        public RFQCostingChild(IEnumerable<tbl_PurchaseOrder_AdditionalCost> childList)
        {
            InitializeComponent();
            var vm = (LayoutRoot.DataContext as RFQCostingViewModel);
            if (vm == null) return;
            vm.AdditionalCostList.Clear();

            foreach (var item in childList)
            {
                vm.AdditionalCostList.Add(new tbl_RFQ_AdditionalCost
                    {
                        CostType = item.CostType,
                        CostValue = item.CostValue,
                        Currency = item.Currency,
                        ExchangeRate = decimal.Parse(item.ExchangeRate.ToString()),
                        Iserial = item.Iserial,
                        LocalValue = item.LocalValue
                    });
            }
            vm.FotterDumpSource[0].Cost = vm.AdditionalCostList.Sum(x => x.LocalValue);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OnCostSubmitted();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            var vm = (LayoutRoot.DataContext as RFQCostingViewModel);
            if (e.Key != Key.Down) return;
            if (vm.AdditionalCostList.Count > 0)
            {
                if (vm.AdditionalCostList.IndexOf((CostingGrid.SelectedItem as tbl_RFQ_AdditionalCost)) == (vm.AdditionalCostList.Count - 1))
                    vm.AdditionalCostList.Add(new tbl_RFQ_AdditionalCost());
            }
            else
                vm.AdditionalCostList.Add(new tbl_RFQ_AdditionalCost());
        }
    }

    public class CostSubmittingEventArgs : EventArgs
    {
        private ObservableCollection<tbl_RFQ_AdditionalCost> _additionalCostList;
        public ObservableCollection<tbl_RFQ_AdditionalCost> AdditionalCostList
        {
            get { return _additionalCostList ?? (_additionalCostList = new ObservableCollection<CRUDManagerService.tbl_RFQ_AdditionalCost>()); }
            set { _additionalCostList = value; }
        }

        private decimal _totalLocalValue;

        public decimal TotalocalValue
        {
            get { return _totalLocalValue; }
            set { _totalLocalValue = value; }
        }
        public CostSubmittingEventArgs(decimal totalValue, ObservableCollection<tbl_RFQ_AdditionalCost> costList)
        {
            AdditionalCostList = costList;
            TotalocalValue = totalValue;
        }
    }
}