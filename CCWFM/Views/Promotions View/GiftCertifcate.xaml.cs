using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel;
using CCWFM.ViewModel.PromotionViewModel;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.OGView.SearchChildWindows;
using Os.Controls.DataGrid;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Promotions_View
{
    public partial class GiftCertifcate : UserControl
    {
        PromoHeaderGifts _viewModel=new PromoHeaderGifts();

        public GiftCertifcate()

        {            

            InitializeComponent();
            switchformmmode(             FormMode.Standby);
            DataContext = _viewModel;
           _viewModel.PremCompleted += (s, sv) =>
            {
                //if (_viewModel. CustomePermissions.SingleOrDefault(x => x.Code == "PromotionPinVisible") != null)
                //{
                //    MainGrid.Columns.SingleOrDefault(x=>x.SortMemberPath=="PIN").Visibility= Visibility.Visible;
                    
                //}
                //else
                //{
                //    MainGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "PIN").Visibility = Visibility.Collapsed;
                //
            };
            _viewModel.TransactionHeader.Type = 0;
        }


        public enum FormMode
        {
            Save,
            Delete,
            Serach,          
            Standby,
            Update,
            Add,
        }

        private void clear()
        {
            //string serial;
            //TblStore tempStore = _viewModel.TransactionHeader.StorePerRow;

            //serial = _viewModel.TransactionHeader.Iserial;
            //int user;
            //user = _viewModel.TransactionHeader.TblUser;

            _viewModel.TransactionHeader = new TblPromoHeaderGifts();

        
       //  if (TblUserDepositViewModel.ActiveStore != null)
       //   {

            //    _viewModel.TransactionHeader.Iserial = serial;
            //    _viewModel.TransactionHeader.StorePerRow = tempStore;
            //}


            //if (TblUserDepositViewModel.Iserial != null)
            //{

            //    _viewModel.TransactionHeader.TblUser = user;
            //}

            addtext.IsChecked = false;
        buttonsave.IsChecked = false;  
        }

        public void switchformmmode(FormMode formMode)
        {
             switch (formMode)
            {
               
                case              FormMode.Add:
                    clear();
                     buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = false;
                   buttonsearch.IsEnabled = false;
                    addtext.IsEnabled = true;
                    _viewModel.TransactionHeader.Enabled = true;
                    button1.IsEnabled = true;
               
                     
                    break;
                case              FormMode.Save:
                    buttonreport.IsEnabled = true;
                    buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = false;
                   buttonsearch.IsEnabled = false;
                    addtext.IsEnabled = true;
                    button1.IsEnabled = true;
                    _viewModel.TransactionHeader.Enabled = true;
              
                  // clear();
                    break;

                case              FormMode.Delete:
                    buttonsave.IsEnabled = false;
                    bttndelete.IsEnabled = true;
                    buttonsearch.IsEnabled = false;
                    _viewModel.TransactionHeader.Enabled = false;
                            
                    clear();
                    break;

                case              FormMode.Standby:  
                    clear();
                    addtext.IsEnabled = true;
                    buttonsave.IsEnabled = false;
                    bttndelete.IsEnabled = false;
                    buttonsearch.IsEnabled = true;
                    buttonreport.IsEnabled = false;                     
                    button1.IsEnabled = false;
                    cancel.Visibility = Visibility.Collapsed;
                    _viewModel.TransactionHeader.Enabled = false;
                            
                    break;
                case              FormMode.Serach:
                    button1.IsEnabled = true;
                    buttonsave.IsEnabled = true;
                    bttndelete.IsEnabled = true;
                    buttonsearch.IsEnabled = true;
                    buttonreport.IsEnabled = true;
                    _viewModel.TransactionHeader.Enabled = true;
            
                  
                    break;
            }
        }
        private void ResetMode()
        {
            _FormMode = FormMode.Standby;
            switchformmmode(_FormMode);
        }

        public   FormMode _FormMode { get; set; }

        private void buttonsearch_Click(object sender, RoutedEventArgs e)
        {

            var child = new SearchGiftsChildWindow(_viewModel);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            child.FlowDirection = currentUi.DisplayName == "العربية"
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
            child.Show();
            _FormMode =FormMode.Serach;
            switchformmmode(_FormMode);
            cancel.Visibility = Visibility.Visible;
            cancel.IsEnabled = true;
            _viewModel.TransactionHeader.Enabled = true;
            _viewModel.GetMaindata();

        }

                
         
        

        private void buttonsave_Click(object sender, RoutedEventArgs e)
        {
           
                _viewModel.UpdateAndInsert();
                
                _FormMode =FormMode.Save;
                switchformmmode(_FormMode);
                cancel.IsEnabled = true;
                cancel.Visibility = Visibility.Visible;
           
        }
        private void cancel_Checked(object sender, RoutedEventArgs e)
        {
            ResetMode();
            cancel.IsEnabled = false;
            cancel.Visibility = Visibility.Collapsed;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
   
        private void addtext_Checked(object sender, RoutedEventArgs e)
        {
            if (addtext.IsChecked==true)
            {
              
                _FormMode =              FormMode.Add;
                switchformmmode(_FormMode);
                cancel.Visibility = Visibility.Visible;
                cancel.IsEnabled = true;
             //   _viewModel.TransactionHeader.inter = true;
            }
           
        }

        private void buttonreport_Click(object sender, RoutedEventArgs e)

        {
            var reportName = "PromotionGifts.rdl"; 
            var reportNameModified = reportName.Replace(" ", "+");
            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportNameModified, null);
             
        }


        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.TransactionHeader.DetailsList.Clear();
            var counter = 0;
            _viewModel.Filter = null;

            _viewModel.ValuesObjects = new Dictionary<string, object>();

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

                _viewModel.ValuesObjects.Add(f.FilterColumnInfo.PropertyPath + counter, myObject);

                if (counter > 0)
                {
                    _viewModel.Filter = _viewModel.Filter + " and ";
                }

                _viewModel.Filter = _viewModel.Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                    f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
            _viewModel.GetTblPromoDetail();
        }




        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.TransactionHeader.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.TransactionHeader.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetTblPromoDetail();
            }
        }

        //private void AddBttn_Click(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.AddNewMainRow(MainGrid.SelectedItem != null);
        //}

        //private void SaveBttn_Click(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.SaveMainRowExc();
        //}

        private void bttndelete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedDetailRows.Clear();
            foreach (var row in MainGrid.SelectedItems)
            {
                _viewModel.SelectedDetailRows.Add(row as TblPromoDetailViewModel);
            }
            _viewModel.Delete();
            _viewModel.DeleteMainRow();
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewMainRow(true);
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblPromoDetailViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetPromoRange();
            
            ////for (int i = 0; i < UPPER; i++)
            ////{
                
            ////}
        }

        private void buttonsearchcode(object sender, RoutedEventArgs e)
        {
            var child = new SearchStoresChildWindow(_viewModel);
            var currentUi = Thread.CurrentThread.CurrentUICulture;
            child.FlowDirection = currentUi.DisplayName == "العربية"
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
            child.Show();
            _FormMode =              FormMode.Serach;
            switchformmmode(_FormMode);
            cancel.Visibility = Visibility.Visible;
            cancel.IsEnabled = true;
            _viewModel.TransactionHeader.Enabled = true;
            _viewModel.GetStores();

        }


    }
}
