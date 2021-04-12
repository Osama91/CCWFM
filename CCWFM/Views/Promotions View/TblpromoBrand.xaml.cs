//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Net;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
//using System.Windows.Navigation;
//using CCWFM.CRUDManagerService;
//using CCWFM.ViewModel.PromotionViewModel;
//using CCWFM.Views.OGView.ChildWindows;
//using CCWFM.Views.OGView.SearchChildWindows;
//using Os.Controls.DataGrid;
//using Os.Controls.DataGrid.Events;

//namespace CCWFM.Views.Promotions_View
//{
//    public partial class TblpromoBrand : Page
//    {
//        //private PromoCriteriaViewModel _viewModel = new PromoCriteriaViewModel();

//        public TblpromoBrand()

//        {

//            InitializeComponent();
//            switchformmmode(FormMode.Standby);
//            DataContext = _viewModel;
                
//            /*  _viewModel.PremCompleted += (s, sv) =>
//            {
//                if (_viewModel. CustomePermissions.SingleOrDefault(x => x.Code == "PromotionPinVisible") != null)
//                {
//                    MainGrid.Columns.SingleOrDefault(x=>x.SortMemberPath=="PIN").Visibility= Visibility.Visible;
                    
//                }
//                else
//                {
//                    MainGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "PIN").Visibility = Visibility.Collapsed;
//                }
//            */
//        }






//        public enum FormMode
//        {
//            Save,
//            Delete,
//            Serach,
//            Standby,
//            Update,
//            Add,
//        }

//        private void clear()
//        {
//            //string serial;
//            //TblStore tempStore = _viewModel.TransactionHeaderCriteria.StorePerRow;

//            //serial = _viewModel.TransactionHeaderCriteria.Iserial;
//            //int user;
//            //user = _viewModel.TransactionHeaderCriteria.TblUser;

//            _viewModel.TransactionHeaderCriteria = new TblPromoCriteriaViewModel();


//            foreach (var Item in _viewModel.Brands)
//            {
//                Item.Chek = false;
//            }
//            foreach (var Item in _viewModel.Stores)
//            {
//                Item.Chek = false;
//            }




//            //foreach (var item in _viewModel.Brands)
//            //{
//            //    _viewModel.Brands=n

//            //}
          
//            //  if (TblUserDepositViewModel.ActiveStore != null)
//            //   {

//            //    _viewModel.TransactionHeaderCriteria.Iserial = serial;
//            //    _viewModel.TransactionHeaderCriteria.StorePerRow = tempStore;
//            //}


//            //if (TblUserDepositViewModel.Iserial != null)
//            //{

//            //    _viewModel.TransactionHeaderCriteria.TblUser = user;
//            //}

//            addtext.IsChecked = false;
//            buttonsave.IsChecked = false;
//        }

//        public void switchformmmode(FormMode formMode)
//        {
//            switch (formMode)
//            {

//                case FormMode.Add:
//                    clear();
//                    buttonsave.IsEnabled = true;
//                    bttndelete.IsEnabled = false;
//                    buttonsearch.IsEnabled = false;
//                    addtext.IsEnabled = true;
//                    _viewModel.TransactionHeaderCriteria.Enabled = true;
//                    //buttonChildwindow.IsEnabled = true;
                          

//                    break;
//                case FormMode.Save:
//                    buttonreport.IsEnabled = true;
//                    buttonsave.IsEnabled = true;
//                    bttndelete.IsEnabled = false;
//                    buttonsearch.IsEnabled = false;
//                    addtext.IsEnabled = true;
//                  //  buttonChildwindow.IsEnabled = true;
//                    //button1.IsEnabled = true;
//                   // _viewModel.TransactionHeaderCriteria.Enabled = true;
//                    // clear();
//                    break;

//                case FormMode.Delete:
//                    buttonsave.IsEnabled = false;
//                    bttndelete.IsEnabled = true;
//                    buttonsearch.IsEnabled = false;
//                    _viewModel.TransactionHeaderCriteria.Enabled = false;
//                    clear();
//                    break;

//                case FormMode.Standby:
//                    clear();
//                    addtext.IsEnabled = true;
//                    buttonsave.IsEnabled = false;
//                    bttndelete.IsEnabled = false;
//                    buttonsearch.IsEnabled = true;
//                    buttonreport.IsEnabled = false;
//                   // buttonChildwindow.IsEnabled = false;
////button1.IsEnabled = false;
//                    cancel.Visibility = Visibility.Collapsed;
//                    _viewModel.TransactionHeaderCriteria.Enabled = false;
//                    break;
//                case FormMode.Serach:
                          
//                  //  buttonChildwindow.IsEnabled = true;
//                    buttonsave.IsEnabled = true;
//                    bttndelete.IsEnabled = true;
//                    buttonsearch.IsEnabled = true;
//                    buttonreport.IsEnabled = true;
//                    _viewModel.TransactionHeaderCriteriaEnabled = true;

//                    break;
//            }
//        }

//        private void ResetMode()
//        {
//            _FormMode = FormMode.Standby;
//            switchformmmode(_FormMode);
//        }

//        public FormMode _FormMode { get; set; }

//        private void buttonsearch_Click2(object sender, RoutedEventArgs e)
//        {

//            var child = new SearchPromoCreteriaChildWindow(_viewModel);
//            var currentUi = Thread.CurrentThread.CurrentUICulture;
//            child.FlowDirection = currentUi.DisplayName == "العربية"
//                ? FlowDirection.RightToLeft
//                : FlowDirection.LeftToRight;
//            child.Show();
//            _FormMode = FormMode.Serach;
//            switchformmmode(_FormMode);
//            cancel.Visibility = Visibility.Visible;
//            cancel.IsEnabled = true;
//            _viewModel.TransactionHeaderCriteria.Enabled = true;
//            _viewModel.GetMaindataCriteria();


//        }

//        private void buttonsave_Click2(object sender, RoutedEventArgs e)
//        {
//            _viewModel.UpdateAndInsertCriteria();


//            _FormMode = FormMode.Save;
//            switchformmmode(_FormMode);
//            cancel.IsEnabled = true;

//            cancel.Visibility = Visibility.Visible;


//        }



//        private void cancel_Checked2(object sender, RoutedEventArgs e)
//        {
//            ResetMode();
//            cancel.IsEnabled = false;
//            cancel.Visibility = Visibility.Collapsed;
//        }

//        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
//        {

//        }

//        private void addtext_Checked2(object sender, RoutedEventArgs e)
//        {
//            if (addtext.IsChecked == true)
//            {

//                _FormMode = FormMode.Add;
//                switchformmmode(_FormMode);
//                cancel.Visibility = Visibility.Visible;
//                cancel.IsEnabled = true;
//                //   _viewModel.TransactionHeaderCriteria.inter = true;
//            }

//        }

//        private void buttonreport_Click(object sender, RoutedEventArgs e)


//        {

//            //_viewModel.ExcelSheet();
//            //var currentUi = Thread.CurrentThread.CurrentUICulture;

//            //if (currentUi.DisplayName != "العربية")
//            //{
//            //    var myUri = new Uri(HtmlPage.Document.DocumentUri, string.Format("report.aspx?Iserial=" + _viewModel.TransactionHeaderCriteria.Iserial));

//            //    HtmlPage.Window.Navigate(myUri, "_Blank");
//            //}

//            //else
//            //{

//            //    var myUri2 = new Uri(HtmlPage.Document.DocumentUri, string.Format("report2.aspx?Iserial=" + _viewModel.TransactionHeaderCriteria.Iserial));

//            //    HtmlPage.Window.Navigate(myUri2, "_Blank");
//            //}
//        }

//        private void UserControl_Loaded(object sender, RoutedEventArgs e)
//        {

//        }

//        //private void buttonChildwindow_Click(object sender, RoutedEventArgs e)
//        //{
//        //    var Child2 = new SearchEventNoChildWindow(_viewModel);
//        //    var currentUi = Thread.CurrentThread.CurrentUICulture;
//        //    Child2.FlowDirection = currentUi.DisplayName == "العربية"
//        //        ? FlowDirection.RightToLeft
//        //        : FlowDirection.LeftToRight;

//        //    Child2.Show();
//        //    //SearchChilWindow child = new SearchChilWindow(_viewModel);
//        //    //child.Show();
//        //    _viewModel.TransactionHeaderCriteria.Enabled = true;
//        //    _FormMode = PromoCriteriaViewModel.FormMode.Serach;
//        //    switchformmmode(_FormMode);
//        //    cancel.Visibility = Visibility.Visible;
//        //    cancel.IsEnabled = true;


//        //}

//        private void MainGrid_OnFilter(object sender, FilterEvent e)
//        {
//            _viewModel.Brands.Clear();
//            var counter = 0;
//            _viewModel.Filter = null;

//            _viewModel.ValuesObjects = new Dictionary<string, object>();

//            foreach (var f in e.FiltersPredicate)
//            {
//                var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
//                var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
//                switch (f.SelectedFilterOperation.FilterOption)
//                {
//                    case Enums.FilterOperation.EndsWith:
//                        myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
//                        break;

//                    case Enums.FilterOperation.StartsWith:
//                        myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
//                        break;

//                    case Enums.FilterOperation.Contains:
//                        myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
//                        break;
//                }

//                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

//                if (counter > 0)
//                {
//                    _viewModel.Filter = _viewModel.Filter + " and ";
//                }

//                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
//                                    f.SelectedFilterOperation.LinqUse + paramter;

//                counter++;
//            }
//            _viewModel.GetBrands();
//        }




//        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
//        {
//            if (_viewModel.Brands.Count < _viewModel.PageSize)
//            {
//                return;
//            }
//            if (_viewModel.Brands.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
//            {
//                _viewModel.Loading = true;
//                _viewModel.GetBrands();
//            }
//        }

//        //

//       // private void AddBttn_Click(object sender, RoutedEventArgs e)
//       // {
//       // //    _viewModel.AddNewMainRow(MainGrid.SelectedItem != null);
//       // }

//       // private void SaveBttn_Click(object sender, RoutedEventArgs e)
//       // {
//       // //    _viewModel.SaveMainRowExc();
//       //}

//        private void bttndelete_Click2(object sender, RoutedEventArgs e)
//        {
//            _viewModel.SelectedDetailRows.Clear();
//            foreach (var row in MainGrid.SelectedItems)
//            {
//                _viewModel.SelectedDetailRows.Add(row as TblItemDownLoadDef);
//            }
//            _viewModel.Delete();
//           // _viewModel.DeleteMainRow();

//        }

//        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
//        {
//        //    if (e.Key == Key.Down)
//        //    {
//        //        _viewModel.AddNewMainRow(true);
//        //    }
//        //    else if (e.Key == Key.Delete)
//        //    {
//        //        _viewModel.SelectedDetailRows.Clear();
//        //        foreach (var row in MainGrid.SelectedItems)
//        //        {
//        //            _viewModel.SelectedDetailRows.Add(row as TblPromoDetailViewModel);
//        //        }

//        //        _viewModel.DeleteMainRow();
//        //    }
//        }

//        private void ButtonreturnDateToFilterIt_Click(object sender, RoutedEventArgs e)
//        {

//          //_viewModel.DetailFilter();

//        }

//        private void MainGrid_OnFilter2(object sender, FilterEvent e)
//        {
//            _viewModel.Stores.Clear();
//            var counter = 0;
//            _viewModel.Filter = null;

//            _viewModel.ValuesObjects = new Dictionary<string, object>();

//            foreach (var f in e.FiltersPredicate)
//            {
//                var paramter = "(@" + f.FilterColumnInfo.PropertyPath + counter + ")";
//                var myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
//                switch (f.SelectedFilterOperation.FilterOption)
//                {
//                    case Enums.FilterOperation.EndsWith:
//                        myObject = Convert.ChangeType("%" + f.FilterText, f.FilterColumnInfo.PropertyType, null);
//                        break;

//                    case Enums.FilterOperation.StartsWith:
//                        myObject = Convert.ChangeType(f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
//                        break;

//                    case Enums.FilterOperation.Contains:
//                        myObject = Convert.ChangeType("%" + f.FilterText + "%", f.FilterColumnInfo.PropertyType, null);
//                        break;
//                }

//                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

//                if (counter > 0)
//                {
//                    _viewModel.Filter = _viewModel.Filter + " and ";
//                }

//                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
//                                    f.SelectedFilterOperation.LinqUse + paramter;

//                counter++;
//            }
//            _viewModel.GetStores();
//        }

//        private void MainGrid_LoadingRow2(object sender, DataGridRowEventArgs e)
//        {
//            if (_viewModel.Stores.Count < _viewModel.PageSize)
//            {
//                return;
//            }
//            if (_viewModel.Stores.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
//            {
//                _viewModel.Loading = true;
//                _viewModel.GetStores();
//            }
//        }

//        //private void button1_Click(object sender, RoutedEventArgs e)
//        //{
//        //    _viewModel.GetPromoRange();

//        //    ////for (int i = 0; i < UPPER; i++)
//        //    ////{

//        //    ////}
//        //}

//    }
//}
     
    

