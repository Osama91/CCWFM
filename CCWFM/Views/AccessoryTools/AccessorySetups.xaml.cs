    using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.AccessoriesViewModel;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.AccessoryTools
{
    public partial class AccessorySetups
    {
        private AccessoriesViewModel _accObj;

        public AccessoriesViewModel AccObj
        {
            get { return _accObj; }
            set { _accObj = value; }
        }

        public AccessorySetups()
        {
            InitializeComponent();
            AccObj = new AccessoriesViewModel();
            LayoutRoot.DataContext = AccObj;
            AccObj.SubmitSearchAction += ViewModel_SubmitSearchAction;
            SwitchFormMode(FormMode.Standby);
        }

        private void ViewModel_SubmitSearchAction(object sender, EventArgs e)
        {
            SwitchFormMode(FormMode.Read);
        }

        #region FormModesSettings

        public enum FormMode
        {
            Standby,
            Search,
            Add,
            Update,
            Read
        }

        private void ClearScreen()
        {
            BtnEditOrder.IsChecked = false;
            BtnAddNewOrder.IsChecked = false;
           AccObj.SelectedMainRow = new AccessoryAttributesViewModel();
            AccObj.AccDetailsList = new SortableCollectionView<AccessoryAttributesDetailsViewModel>();
            
            //_viewModel.TransactionDetails.Clear();
            //  AccObj =new AccessoriesViewModel();
        }

        public void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = true;
                    BtnShowSearchOrder.Visibility = Visibility.Collapsed;
                    AccObj.SelectedMainRow.ObjStatus.IsNew = true;
                    AccObj.Enabled = true;

                    break;

                case FormMode.Standby:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    AccObj.Enabled = false;
                    BtnAddNewOrder.IsEnabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;
                    BtnSearchOrder.IsEnabled = true;
                    BtnSaveOrder.IsEnabled = false;
                    //   BtnEditOrder.IsEnabled = false;
                    BtnShowSearchOrder.Visibility = Visibility.Visible;
                    BtnShowSearchOrder.IsEnabled = true;                  
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtntnAddNewMainOrderDetails.IsEnabled = false;
                    AccObj.Enabled = true;
                    BtnAddNewOrder.IsEnabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSaveOrder.IsEnabled = false;
                    BtnShowSearchOrder.IsEnabled = false;
                   
                    break;

                case FormMode.Update:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    AccObj.Enabled = true;
                    BtnAddNewOrder.Visibility = Visibility.Visible;                    
                    BtnSaveOrder.IsEnabled = true;
                  
                    break;

                case FormMode.Read:
                    BtntnAddNewMainOrderDetails.IsEnabled = true;
                    AccObj.Enabled = false;
                    BtnAddNewOrder.Visibility = Visibility.Collapsed;
                    BtnSearchOrder.IsEnabled = false;
                    BtnSaveOrder.IsEnabled = true;
                    //   BtnEditOrder.IsEnabled = true;
                    
                    break;
            }
        }

        private void btnEditOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Update;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            //         BtnDeleteOrder.Visibility = Visibility.Visible;
            //         BtnDeleteOrder.IsEnabled = true;
            //   BtnEditOrder.IsEnabled = false;
        }

        private void btnAddNewOrder_Checked(object sender, RoutedEventArgs e)
        {
            _FormMode = FormMode.Add;
            SwitchFormMode(_FormMode);
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
        }    

        private void btnShowSearchOrder_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancelOrder.Visibility = Visibility.Visible;
            BtnCancelOrder.IsEnabled = true;
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }      

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancelOrder.IsEnabled = false;
            BtnCancelOrder.Visibility = Visibility.Collapsed;
            //       BtnDeleteOrder.Visibility = Visibility.Collapsed;
            //      BtnDeleteOrder.IsEnabled = false;
            BtnShowSearchOrder.IsChecked = false;
        }

        public FormMode _FormMode { get; set; }

        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            SwitchFormMode(_FormMode);
        }

        private void btnSearchOrder_Click(object sender, RoutedEventArgs e)
        {
            AccObj.SearchHeader();
            _FormMode = FormMode.Search;
            SwitchFormMode(_FormMode);
        }
     
        #endregion FormModesSettings

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            AccObj.AccDetailsList.Clear();
            var counter = 0;
            AccObj.DetailFilter = null;

            AccObj.DetailValuesObjects = new Dictionary<string, object>();

            foreach (var f in e.FiltersPredicate)
            {
                var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
                var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Enums.FilterOperation.EndsWith:
                        myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
                        break;

                    case Enums.FilterOperation.StartsWith:
                        myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                        break;

                    case Enums.FilterOperation.Contains:
                        myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
                        break;
                }

                AccObj.DetailValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                if (counter > 0)
                {
                    AccObj.DetailFilter = AccObj.DetailFilter + " and ";
                }

                AccObj.DetailFilter = AccObj.DetailFilter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            AccObj.LoadAccessoryDetail();
        }

        private void DetailGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (AccObj.AccDetailsList.Count < AccObj.PageSize)
            {
                return;
            }
            int x = e.Row.GetIndex();
            if (AccObj.AccDetailsList.Count - 2 < x && !AccObj.Loading)
            {
                AccObj.Loading = true;
                AccObj.LoadAccessoryDetail();
            }
        }

        private void BtnDetailsDeleted_OnClick(object sender, RoutedEventArgs e)
        {
            AccObj.DeletedAccDetail();
        }
    }
}