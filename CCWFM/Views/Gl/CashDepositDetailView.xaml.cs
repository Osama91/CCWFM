using CCWFM.Models.Gl;
using CCWFM.ViewModel.OGViewModels.ControlsOverride;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.Views.Gl
{
    public partial class CashDepositDetailView : ChildWindowsOverride
    {
        CashDepositType cashDepositType;
        public CashDepositDetailView()
        {
            InitializeComponent();
        }

        public CashDepositType CashDepositType
        {
            get { return cashDepositType; }
            set { cashDepositType = value; }
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = ((DataGrid)sender);
            if (cashDepositType == CashDepositType.Cheque)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(ChequeNo)).Visibility = Visibility.Visible;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(DueDate)).Visibility = Visibility.Visible;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(Notes)).Visibility = Visibility.Visible;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(PayTo)).Visibility = Visibility.Visible;

                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityAccount)).Visibility = Visibility.Collapsed;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityType)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.Visa)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(MachineId)).Visibility = Visibility.Visible;

                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityAccount)).Visibility = Visibility.Collapsed;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityType)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.Expences)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityAccount)).Visibility = Visibility.Visible;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityType)).Visibility = Visibility.Visible;
            }
            else if (cashDepositType == CashDepositType.Discount)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(LedgerDescription)).Visibility = Visibility.Collapsed;

                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityAccount)).Visibility = Visibility.Collapsed;
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(EntityType)).Visibility = Visibility.Collapsed;
            }

            else if (cashDepositType == CashDepositType.DSquaresCIB)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;   
            }
            else if (cashDepositType == CashDepositType.VALU)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.FORSA)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.AmazonMarket)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.DsquaresLuckyWallet)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            }
            else if (cashDepositType == CashDepositType.TFKCourier)
            {
                grid.Columns.FirstOrDefault(c =>
                    c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            }
            //else if (cashDepositType == CashDepositType.TFKCash)
            //{
            //    grid.Columns.FirstOrDefault(c =>
            //        c.GetValue(FrameworkElement.NameProperty).ToString() == nameof(BankAccount)).Visibility = Visibility.Collapsed;
            //}
        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Down)
            //{
            //    var vm = this.DataContext as CashDepositViewModel;
            //    if (vm.NewDetail.CanExecute(null)) vm.NewDetail.Execute(null);
            //}
        }        
    }
}

