using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;
using Lite.ExcelLibrary.SpreadSheet;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid.Events;
using BomFabricBom = CCWFM.Views.OGView.ChildWindows.BomFabricBom;
using Image = System.Windows.Controls.Image;
using System.IO;
using System.Windows.Media.Imaging;
using CCWFM.Views.OGView;
using System.Collections;
using System.Windows.Browser;
using CCWFM.ViewModel;
using System.Globalization;

namespace CCWFM.Views.OGView
{
    public partial class StyleHeader
    {
        private readonly StyleHeaderViewModel _viewModel;

        public SalesOrderType Ordertype;
        public SalesOrderType OrgType;
        public List<TNAStyleRouteViewModel> TNAStyleRouteList = new List<TNAStyleRouteViewModel>();
        public int AddMode = 0;
        public int TNAApprove = 0;
        public bool showHideTNA_Approve = false;

        public string Stcode = "";
        string startupPath = string.Empty;

        public StyleHeader(SalesOrderType salesOrderType, bool styleOnly)
        {
            InitializeComponent();
            OrgType = salesOrderType;
            Ordertype = salesOrderType;
            _viewModel = new StyleHeaderViewModel(salesOrderType);
            DataContext = _viewModel;
           
            //chkRepatedStyle.IsEnabled = false;

            if (_viewModel != null)
                _viewModel.ExportCompleted += (s, r) =>
                {
                    if (_viewModel.ExportGrid == MainGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("Style");
                    }
                    if (_viewModel.ExportGrid == DetailGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("PO");
                    }
                    if (_viewModel.ExportGrid == RfqGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("RFQ");
                    }
                    if (_viewModel.ExportGrid == SalesOrderOperationGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("Operations");
                    }
                    if (_viewModel.ExportGrid == BomGrid)
                    {
                        _viewModel.ExportGrid.ExportExcel("Bom");
                    }
                };

            if (styleOnly)
            {
                foreach (TabItem item in TabStyle.Items)
                {
                    if (item.Tag != null)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if (salesOrderType == SalesOrderType.SalesOrderPo || salesOrderType == SalesOrderType.AdvancedSampleRequest)
                {
                    TabRfqItem.Visibility = Visibility.Collapsed;
                    BtnGeneratePo.Visibility = Visibility.Collapsed;

                    DetailGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "TblSupplier").Visibility = Visibility.Collapsed;
                    BomGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "IsSupplierMaterial").Visibility = Visibility.Collapsed;
                    TechPackBomGrid.Columns.SingleOrDefault(x => x.SortMemberPath == "IsSupplierMaterial").Visibility = Visibility.Collapsed;

                    foreach (TabItem item in TabStyle.Items)
                    {
                        if (item.Tag != null && item.Tag.ToString() == "RetailTab")
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else if (salesOrderType == SalesOrderType.RetailPo)
                {
                    TabSalesItem.Header = strings.RetailPo;
                    BtnGenerateSalesOrder.Visibility = Visibility.Collapsed;
                    BtnPostToAx.Visibility = Visibility.Collapsed;

                    foreach (TabItem item in TabStyle.Items)
                    {
                        if (item.Tag != null && item.Tag.ToString() == "NotRetailTab")
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                }


            }
            _viewModel.PremCompleted += (w, sv) =>
            {
                foreach (TabItem item in TabStyle.Items)
                {
                    if (_viewModel.CustomePermissions.FirstOrDefault(e => e.Code == "Hide" + item.Name) != null)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                    if (item.Name == "TabTechPackItem")
                    {
                        if (_viewModel.CustomePermissions.FirstOrDefault(e => e.Code == "HideTabTechPackItem" + item.Name) == null)
                        {
                            //EnableOrDisableTechPackControls();
                            //ShowHideTechPackTabs();
                        }
                        else
                        {
                        }
                    }
                }

                //if (_viewModel.CustomePermissions.Any(r => r.Code == "LimitedAddStyleTNA"))
                //{
                //    _viewModel.LimitedAddStyleTNA = true;
                //}

                if (_viewModel.CustomePermissions.Any(r => r.Code == "RemoveSalesOrderApproval"))
                {
                    BtnRemoveSaleOrderApprove.Visibility = Visibility.Visible;
                }
                
                if (_viewModel.CustomePermissions.Any(r => r.Code == "LimitedAddStyleTNA")
                || _viewModel.CustomePermissions.Any(r => r.Code == "AddStyleTNA")
                || _viewModel.CustomePermissions.Any(r => r.Code == "UpdateStyleTNA")
                || _viewModel.CustomePermissions.Any(r => r.Code == "LimitedUpdateStyleTNA"))

                {
                    GridTna.Visibility = Visibility.Visible;
                    var Itemss = _viewModel.CustomePermissions.Where(yt => yt.Code.StartsWith("Btn")).ToList();
                    foreach (var item in _viewModel.CustomePermissions.Where(yt => yt.Code.StartsWith("Btn")))
                    {
                        var ts = GetChildren<Button>(GridTna);
                        ts.ForEach(a =>
                        {
                            if (a.Name.ToLower() == item.Code.ToLower())
                            {
                                a.Visibility = Visibility.Visible;
                            }
                        });
                    }
                    _viewModel.GetMaindata();
                    _viewModel.StyleTnaCount();
                }
                else
                {
                    _viewModel.GetMaindata();
                }
                ReOrderColumns();
            };
            GetStyleTNARoute();

            startupPath = App.Current.Host.Source.Scheme + @"://" +
                        App.Current.Host.Source.Host + ":" +
                        App.Current.Host.Source.Port.ToString() + @"/CCWFMTestPage.aspx";
        }

        private void EnableOrDisableTechPackControls()
        {
            if(_viewModel.CustomePermissions.Any(r => r.Code == "OperationsTechPack"))
            {
                TabTechPackBom.Visibility = Visibility.Collapsed;
                TabTechPackDesignSpec.Visibility = Visibility.Collapsed;
                TabTechPackStyleTnaRoute.Visibility = Visibility.Collapsed;
                TabTechPackStyleTNA.IsSelected = true;
                Dispatcher.BeginInvoke((Action)(() => TabTechPackItems.SelectedIndex = 3));

            }

            #region OLdHideShow
                try
                {
                    //if (_viewModel.CustomePermissions.Any(r => r.Code == "DisableTabTechPackBom"))
                    //{

                    //       TabTechPackBom.Visibility = Visibility.Collapsed;
                    //       txtTeckPackBOMDescription.IsReadOnly = true;
                    //       BtnCopyTechPackBomLine.IsEnabled = false;
                    //       BtnCopyTechPackBom.IsEnabled = false;
                    //       TechPackBomGrid.IsReadOnly = true;

                    //        //foreach (Control ctrl in TechPackBomGrid.ColumnOptionControls)
                    //        //{
                    //        //    if (ctrl.GetType() == typeof(Button))
                    //        //    {
                    //        //        Button btn = (Button)ctrl;
                    //        //        btn.IsEnabled = false;
                    //        //    }
                    //        //}
                    //}
                    //if (_viewModel.CustomePermissions.Any(r => r.Code == "DisableTabTechPackDesignSpec"))
                    //{
                    //        TabTechPackDesignSpec.Visibility = Visibility.Collapsed;
                    //        StyleTechPackStatus.IsEnabled = false;
                    //        txtTeckPacKFIT.IsReadOnly = true;
                    //        grdTechPackDesignDetail.IsReadOnly = true;
                    //}
                    //if (_viewModel.CustomePermissions.Any(r => r.Code == "DisableTabTechPackStyleTnaRoute"))
                    //{
                    //    TabTechPackStyleTnaRoute.Visibility = Visibility.Collapsed;
                    //    TabTechPackStyleTNA.IsSelected = true;
                    //    grdTNARoute.IsReadOnly = true;
                    //    Dispatcher.BeginInvoke((Action)(() => TabTechPackItems.SelectedIndex = 3));
                    //}
                    //if (_viewModel.CustomePermissions.Any(r => r.Code == "DisableTabTechPackStyleTNA"))
                    //{
                    //    TabTechPackStyleTNA.Visibility = Visibility.Collapsed;
                    //    grdStyleTechPackTNA.IsReadOnly = true;
                    //}
                }
                catch { }
            #endregion
        }

        private void ShowHideTechPackTabs()
        {
            try
            {
                foreach (TabItem item in TabTechPackItems.Items)
                {
                    if (_viewModel.CustomePermissions.FirstOrDefault(e => e.Code == "Hide" + item.Name) != null)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }

                var SelectedTabItem = (from m in _viewModel.CustomePermissions.OrderBy(x=>x.PermOrder)
                                       where m.Code.Contains("HideTabTechPack")
                                       select m.PermOrder).ToList();
                List<int?> Tabs = new List<int?>() { 0,1,2,3,};
                var result = Tabs.Except(SelectedTabItem).ToList();
                if (result.Count > 0) { TabTechPackItems.SelectedIndex = result.FirstOrDefault().Value; }
                else { TabTechPackItems.SelectedIndex = 0; }
            }
            catch { }

        }
        private void BtnGetPending_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.PenStatus = 0;
            _viewModel.GetMainTempData();
        }
        private void BtnGetApproved_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.PenStatus = 1;
            _viewModel.GetMainTempData();
        }
        private void BtnCopyProduct_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedMainRow.Iserial == 0)
            {
                MessageBox.Show("You have to save the style first in order to copy it");
            }

            _viewModel.CopyProduct();
        }
        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
             AddMode = 1;
            _viewModel.ViewModelAddMode = 1;
            //_viewModel.SizeGroupList.Clear();
            //_viewModel.StyleCategoryList.Clear();
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case (int)StyleTabes.Style:
                    case (int)StyleTabes.StyleDetails:
                    case (int)StyleTabes.RetailSpecDetails:
                        if( _viewModel.CustomePermissions.FirstOrDefault(x => x.Code == "DisableStyleCodingOnPDM") !=null)
                        {
                            MessageBox.Show("Save Data on Stitch");
                            break;
                        }
                        _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
                        if (TabStyle.SelectedIndex == 0) { TabStyle.SelectedIndex = 1; }
                        if (AddMode == 1)
                        {
                            if (TabStyle.SelectedIndex == 0 || TabStyle.SelectedIndex == 1) { StyleDetailsClickedAddMode(); }
                        }
                        break;
                    case (int)StyleTabes.SalesOrder:
                        _viewModel.AddNewDetailRow(DetailGrid.SelectedIndex != -1);
                        break;

                    case (int)StyleTabes.Rfq:
                        _viewModel.AddNewDetailRow(RfqGrid.SelectedIndex != -1);
                        break;

                    case (int)StyleTabes.Operation:
                        _viewModel.AddNewSalesOrderOperation(SalesOrderOperationGrid.SelectedIndex != -1);
                        break;

                    case (int)StyleTabes.Bom:
                        _viewModel.AddBom(BomGrid.SelectedIndex != -1);
                        break;
                    case (int)StyleTabes.TeckPack:
                        AddTechPackBomTabsRow();
                        break;
                }
            }
        }

        private void AddTechPackBomTabsRow()
        {
            switch (TabTechPackItems.SelectedIndex)
            {
                case 3:
                    _viewModel.AddNewTNADetailRow();
                    break;
                default:
                    _viewModel.AddTechPackBom(TechPackBomGrid.SelectedIndex != -1);
                    break;

            }
           
        }

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case (int)StyleTabes.Style:
                    case (int)StyleTabes.StyleDetails:
                    case (int)StyleTabes.RetailSpecDetails:
                        if (_viewModel.CustomePermissions.FirstOrDefault(x => x.Code == "DisableStyleCodingOnPDM") != null)
                        {
                            MessageBox.Show("Save Data on Stitch");
                            break;
                        }
                        //if(_viewModel.SelectedMainRow.StyleImage == null)
                        //{
                        //    MessageBox.Show("Kindly Insert Style Image");
                        //}
                        //if( !string.IsNullOrWhiteSpace( _viewModel.SelectedMainRow.ExternalStyleCode))
                        //{
                        //    if(_viewModel.SelectedMainRow.TblStyleStatus == null)
                        //    {
                        //        MessageBox.Show("Kindly Select Style Design");
                        //    }
                        //    else
                        //    {
                        //        if (RepeatedStylesUpdate.SelectedItem != null && chkRepeatedStyles.IsChecked == true)
                        //            _viewModel.SaveMainRow(RepeatedStylesUpdate.SelectedItem.ToString());
                        //        else
                        //            _viewModel.SaveMainRow();
                        //        AddMode = 0;
                        //    }
                        //}
                        //else
                        {
                            if (RepeatedStylesUpdate.SelectedItem != null && chkRepeatedStyles.IsChecked == true)
                                _viewModel.SaveMainRow(RepeatedStylesUpdate.SelectedItem.ToString());
                            else
                                _viewModel.SaveMainRow();
                            AddMode = 0;
                        }
                        break;
                    case (int)StyleTabes.SalesOrder:

                        if (!_viewModel.Loading)
                        {
                            DetailGrid.CommitEdit();
                        }
                        break;

                    case (int)StyleTabes.Rfq:
                        if (!_viewModel.Loading)
                        {
                            RfqGrid.CommitEdit();
                        }
                        break;

                    case (int)StyleTabes.Operation:
                        if (!_viewModel.Loading)
                        {
                            _viewModel.SaveSalesOrderOperationsList();
                        }
                        break;

                    case (int)StyleTabes.Bom:
                        _viewModel.SaveBom();
                        break;
                    //case (int)StyleTabes.Tna:
                    //    _viewModel.SaveStyleTNARow();
                    //    break;

                    case (int)StyleTabes.TeckPack:
                        SaveTechPack();
                        break;
                    case (int)StyleTabes.StyleSpec:
                        _viewModel.SaveMainRow();
                        _viewModel.SaveStyleSpecDetails();
                        break;

                 
                }
            }
        }
        private void SaveTechPack()
        {
            switch (TabTechPackItems.SelectedIndex)
            {
                case 0:
                    _viewModel.SaveTechPackBom();
                    break;
                case 1:
                    try
                    {
                        var query = _viewModel.SelectedTnaRow.TechPackDesignDetailList.GroupBy(x => x.TechPackPartPerRow.Iserial)
                                                                          .Where(g => g.Count() > 1)
                                                                          .Select(y => y.Key)
                                                                          .ToList();
                        if (query.Count() >= 1)
                        {
                            MessageBox.Show("Repeated Techpack Parts, kindly Check");
                        }
                        else
                        {
                            int? TechStaus = null;
                            if (!string.IsNullOrEmpty(StyleTechPackStatus.SelectedValue.ToString()))
                            { TechStaus = (int)StyleTechPackStatus.SelectedValue; }
                            _viewModel.SaveTechPackDesignDetailRow(TechStaus, txtTeckPacKFIT.Text, txtTeckPackorgFabric.Text);
                        }
                    }
                    catch { _viewModel.SaveTechPackDesignDetailRow(null,"",""); }
                    break;
                case 3:
                    //_viewModel.SaveStyleTNARow();
                 break;
            }

        }
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (TabStyle != null)
            {
                switch (TabStyle.SelectedIndex)
                {
                    case (int)StyleTabes.Style:
                    case (int)StyleTabes.StyleDetails:
                    case (int)StyleTabes.RetailSpecDetails:
                        MainGrid.ExportExcel("Style");
                        break;
                    case (int)StyleTabes.SalesOrder:
                        DetailGrid.ExportExcel("Po");
                        break;
                    case (int)StyleTabes.Rfq:
                        RfqGrid.ExportExcel("RFQ");
                        break;
                    case (int)StyleTabes.Operation:
                        SalesOrderOperationGrid.ExportExcel("Operations Colors");
                        break;
                    case (int)StyleTabes.Bom:
                        BomGrid.ExportExcel("Bom");
                        break;
                }
            }
        }
        private void btnPrintPreviewOrder_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Report();
        }
        private void MainGrid_OnFilter(object sender, FilterEvent e)
        {
            _viewModel.MainRowList.Clear();
            DataFormStyle.CancelEdit();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetMaindata();
        }
        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.MainRowList.IndexOf(_viewModel.SelectedMainRow));
                if (currentRowIndex == (_viewModel.MainRowList.Count - 1))
                {
                    _viewModel.AddNewMainRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblStyleViewModel);
                }

                _viewModel.DeleteMainRow();
            }
        }
        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            if (_viewModel.MainRowList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.MainRowList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading && _viewModel.MainRowList.Count < _viewModel.FullCount)
            {
                _viewModel.Loading = true;
                _viewModel.GetMaindata();
            }

           // ShowHideTNA_Approve(showHideTNA_Approve);
        }
        private void MainGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (_viewModel.SelectedMainRow.StyleImage == null)
            {
                MessageBox.Show("Kindly Insert Style Image");
            }
            else if (!string.IsNullOrWhiteSpace(_viewModel.SelectedMainRow.ExternalStyleCode))
            {
                if (_viewModel.SelectedMainRow.TblStyleStatus == null)
                {
                    MessageBox.Show("Kindly Select Style Design");
                }
                else
                {
                    if (RepeatedStylesUpdate.SelectedItem != null && chkRepeatedStyles.IsChecked == true)
                        _viewModel.SaveMainRow(RepeatedStylesUpdate.SelectedItem.ToString());
                    else
                        _viewModel.SaveMainRow();
                    AddMode = 0;
                }
            }
            else
            _viewModel.SaveMainRow();
        }
        private void DataFormStyle_AddingNewItem(object sender, DataFormAddingNewItemEventArgs e)
        {
            e.Cancel = true;
            _viewModel.AddNewMainRow(true);
        }
        public List<T> GetChildren<T>(UIElement parent) where T : UIElement
        {
            var list = new List<T>();
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if (child != null)
                {
                    if (child is T)
                        list.Add(child as T);

                    var l1 = GetChildren<T>(child);
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var u in l1)
                        list.Add(u);
                }
            }
            return list;
        }
        private void TabStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabStyle != null)
            {
               //  _viewModel.CheckTnaChanged();
                switch (TabStyle.SelectedIndex)
                {
                    case (int)StyleTabes.StyleDetails:
                        try
                        {
                            if (_viewModel.SelectedMainRow.SeasonPerRow != null)
                            {
                                var selectedSeason = _viewModel.SelectedMainRow.SeasonPerRow;
                                _viewModel.SeasonListPerRow =
                               new ObservableCollection<TblLkpSeason>
                               (
                                _viewModel.SeasonList
                               //.Where(x => x.ShortCode == _viewModel.SelectedMainRow.SeasonPerRow.ShortCode)
                               );
                                _viewModel.SelectedMainRow.TblLkpSeason = selectedSeason.Iserial;
                                _viewModel.SelectedMainRow.SeasonPerRow = selectedSeason;
                            }
                            else
                            {
                                _viewModel.SeasonListPerRow = _viewModel.SeasonList;
                            }
                        }
                        catch (Exception)
                        {
                            _viewModel.SeasonListPerRow.Clear();
                        }

                        if (_viewModel.SelectedMainRow.RetialPoTransactionExist)
                        {
                            var p = DataFormStyle as FrameworkElement;
                            var ts = GetChildren<TextBox>(p);
                            ts.ForEach(a =>
                            {
                                if (a.Name != "DescriptionUpdate" || !_viewModel.CustomePermissions.Any(x => x.Code == "DescriptionUpdate"))
                                {
                                    a.IsReadOnly = true;
                                }
                                else
                                {
                                    a.IsReadOnly = false;
                                    a.IsEnabled = true;
                                }
                            });
                            var tsc = GetChildren<ComboBox>(p);
                            tsc.ForEach(a =>
                            {
                                if (a.Name != "SeasonApprovedUpdate")
                                {
                                    a.IsEnabled = false;
                                }
                            });
                        }
                        else
                        {
                            var p = DataFormStyle as FrameworkElement;
                            var ts = GetChildren<TextBox>(p);
                            ts.ForEach(a => { a.IsReadOnly = false; });

                            var tsc = GetChildren<ComboBox>(p);
                            tsc.ForEach(a => { a.IsEnabled = true; });

                            foreach (var customPerm in _viewModel.CustomePermissions)
                            {
                                try
                                {
                                    if (customPerm.Code == "FamilyUpdate")
                                    {
                                        _viewModel.SelectedMainRow.familyUpdate = true;
                                    }
                                    if (customPerm.Code == "BrandUpdate")
                                    {
                                        _viewModel.SelectedMainRow.brandUpdate = true;
                                    }
                                    if (customPerm.Code == "BrandSectionUpdate")
                                    {
                                        _viewModel.SelectedMainRow.brandSectionUpdate = true;
                                    }
                                    if (customPerm.Code == "SeasonUpdate")
                                    {
                                        _viewModel.SelectedMainRow.seasonUpdate = true;
                                    }
                                    var item = DataFormStyle.FindName(customPerm.Code) as TextBox;
                                    item.IsEnabled = true;
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        var item = DataFormStyle.FindName(customPerm.Code) as ComboBox;
                                        item.IsEnabled = true;
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            var item = DataFormStyle.FindName(customPerm.Code) as AutoCompleteBox;
                                            item.IsEnabled = true;
                                        }
                                        catch (Exception)
                                        {
                                            var item = DataFormStyle.FindName(customPerm.Code) as UserControl;
                                            if (item != null)
                                            {
                                                item.IsEnabled = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (_viewModel.SelectedMainRow.Iserial != 0)
                            {
                                _viewModel.SelectedMainRow.familyUpdate = false;
                                _viewModel.SelectedMainRow.brandUpdate = false;
                                _viewModel.SelectedMainRow.brandSectionUpdate = false;
                                _viewModel.SelectedMainRow.seasonUpdate = false;
                            }
                        }
                        //if (_viewModel.SelectedMainRow.TargetCostPrice > 0)
                        //{
                        //    TargetCostPriceUpdate.IsReadOnly = true;
                            
                        //}
                        break;

                    case (int)StyleTabes.RetailSpecDetails:
                        foreach (var customPerm in _viewModel.CustomePermissions)
                        {
                            try
                            {
                                var item = RetailSpecsTab.FindName(customPerm.Code) as TextBox;
                                if (item != null) item.IsEnabled = true;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    var item = RetailSpecsTab.FindName(customPerm.Code) as ComboBox;
                                    if (item != null) item.IsEnabled = true;
                                }
                                catch (Exception)
                                {
                                    var item = RetailSpecsTab.FindName(customPerm.Code) as AutoCompleteBox;
                                    if (item != null)
                                    {
                                        item.IsEnabled = true;
                                    }
                                }
                            }
                        }

                        break;

                    case (int)StyleTabes.SalesOrder:
                        _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
                        if (OrgType == SalesOrderType.SalesOrderPo)
                        {
                            _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
                        }
                        else if (OrgType == SalesOrderType.RetailPo)
                        {
                            _viewModel.SalesOrderType = SalesOrderType.RetailPo;
                        }

                        _viewModel.SelectedMainRow.DetailsList.Clear();
                        _viewModel.SelectedDetailRow = null;
                        _viewModel.GetSalesOrderLookups();
                        break;

                    case (int)StyleTabes.Rfq:
                        _viewModel.SalesOrderType = SalesOrderType.Rfq;
                        _viewModel.SelectedMainRow.DetailsList.Clear();
                        _viewModel.SelectedDetailRow = null;
                        _viewModel.GetSalesOrderLookups();
                        break;

                    case (int)StyleTabes.Operation:
                        _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
                        _viewModel.GetSalesOrderOperations();
                        break;

                    case (int)StyleTabes.Bom:
                        _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
                        _viewModel.GetSalesOrderBom();
                        break;
                    //case (int)StyleTabes.Tna:
                    //    _viewModel.GetStyleTNALockup();
                    //    _viewModel.SeasonPerRow = _viewModel.SelectedMainRow.SeasonPerRow;
                    //    _viewModel.GetStyleTNAdata();
                    //    //if (true)
                    //    //{

                    //    //}
                    //    //_viewModel.SelectedTnaRow = null;
                    //    //_viewModel.AddNewStyleTNARow(false);
                    //    //_viewModel.AddNewTNADetailRow();
                    //    _viewModel.getSml();
                    //    break;
                    //case (int)StyleTabes.TnaRoute:
                    //      GetStyleTNARouteByStyle();
                    //      break;
                    //case (int)StyleTabes.TechPack:
                    //      _viewModel.GetStyleTechPackParts();
                    //     // _viewModel.GetTechPackdeDetaildata();
                    //      LoadStyleTechPack();
                    //      break;
                    case (int)StyleTabes.TeckPack:
                        //For Now Will Be Edited//
                        //_viewModel.GetStyleTNALockup();
                        //_viewModel.SeasonPerRow = _viewModel.SelectedMainRow.SeasonPerRow;
                        //_viewModel.GetStyleTNAdata();
                        ////////////////////////
                        GetTeckPackData();
                        EnableOrDisableTechPackControls();
                        break;
                    case (int)StyleTabes.StyleSpec:
                        _viewModel.GetStyleSpecTypes();
                        //_viewModel.GetStyleSpecDetail();
                        break;
                } 
            }
        }

        private void GetTeckPackData()
        {
            //ShowHideTechPackTabs();
            if (TabTechPackItems != null)
            {
                switch (TabTechPackItems.SelectedIndex)
                {
                    case 0:
                        
                        CRUDManagerService.CRUD_ManagerServiceClient _client = new CRUDManagerService.CRUD_ManagerServiceClient();
                        _client.GetTBLTechPackSeasonalMaterListColorsAsync(_viewModel.SelectedMainRow.Iserial);
                        _client.GetTBLTechPackSeasonalMaterListColorsCompleted += (s, sv) =>
                        {
                            _viewModel.SelectedMainRow.SelectedStyleSeasonalMaterColors = sv.Result;
                            _viewModel.GetTechPackBom();
                            _viewModel.GetTechPackBomComments();
                        };
                        break;
                    case 1:
                        _viewModel.GetTechPackDetailDesignData();
                        break;
                }
            }
        }

        private void LoadStyleTechPack()
        {
            try
            {
                LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
                lkpClient.GetTBLTechPackStatusAsync();
                lkpClient.GetTBLTechPackStatusCompleted += (s, sv) =>
               {
                   //if (sv.Result != null)
                   //    comTeckPackStaus.ItemsSource = sv.Result;
               };
            }
            catch { }

        }

        private void DetailGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;
            _viewModel.SelectedMainRow.DetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.Filter = filter;
            _viewModel.ValuesObjects = valueObjecttemp;
            _viewModel.GetDetailData();
        }

        private void DetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.SelectedMainRow.DetailsList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.SelectedMainRow.DetailsList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.GetDetailData();
            }
        }

        private void DetailGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedMainRow.DetailsList.IndexOf(_viewModel.SelectedDetailRow));
                if (currentRowIndex == (_viewModel.SelectedMainRow.DetailsList.Count - 1))
                {
                    _viewModel.AddNewDetailRow(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    _viewModel.SelectedDetailRows.Add(row as TblSalesOrderViewModel);
                }

                _viewModel.DeleteDetailRow();
            }
        }

        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (!_viewModel.Loading)
            {
                _viewModel.SaveDetailRow();
            }
        }

        private void SalesOrderOperationGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.SalesOrderOperationList.IndexOf(_viewModel.SelectedSalesOrderOperation));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.SalesOrderOperationList.Count - 1))
                {
                    _viewModel.AddNewSalesOrderOperation(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedSalesOrderOperations.Clear();
                foreach (var row in SalesOrderOperationGrid.SelectedItems)
                {
                    _viewModel.SelectedSalesOrderOperations.Add(row as TblSalesOrderOperationViewModel);
                }

                _viewModel.DeleteSalesOrderOperations();
            }
        }

        private void SalesOrderOperationGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (!_viewModel.Loading)
            {
                _viewModel.SaveSalesOrderOperations();
            }
        }

        private void BomGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.BomList.IndexOf(_viewModel.SelectedBomRow));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.BomList.Count - 1))
                {
                    _viewModel.AddBom(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedBomRows.Clear();
                foreach (var row in BomGrid.SelectedItems)
                {
                    _viewModel.SelectedBomRows.Add(row as BomViewModel);
                }

                _viewModel.DeleteBom();
            }
        }

        private void BtnGenerateSalesOrder_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateSalesOrder();
        }

        private void BtnGeneratePo_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.GeneratePo();
        }

        private void BtnPostToAx_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.PostToAx();
        }

        private void BomGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetTechPackLookUp();
            _viewModel.GetStyleImage();
        }

        public void GetImageStyleFromFile()
        {

        }

        private void BtnShowImages_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;

            if (btn != null)
            {
                var childWindow = new StyleImagesChildWindow(btn.DataContext as TblStyleViewModel);
                childWindow.Show();
            }
        }

        private void BtnApprove_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedDetailRow.ContainColors)
            {
                if ((_viewModel.SelectedMainRow.TblGroup1 != null && _viewModel.SelectedMainRow.TblGroup1 != 0))
                {
                    if (_viewModel.SelectedDetailRow.SalesOrderType != 2)
                    {
                        var childWindow = new ApprovalChildWindow(_viewModel);
                        childWindow.Show();
                    }
                    else
                    {
                        _viewModel.ValidateBom();
                    }
                }
                else
                {
                    MessageBox.Show("Source Must BE Specified !");
                }
            }
            else
            {
                MessageBox.Show("The po Doesn't Contain Color");
            }
        }

        private void BtnAttachment_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            if (btn != null)
            {
                var childWindow = new StyleAttachmentChildWindow(btn.DataContext as TblStyleViewModel);
                childWindow.Show();
            }
        }

        private void BtnSalesOrderAttachment_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            if (btn != null)
            {
                var childWindow = new SalesOrderAttachmentChildWindow(btn.DataContext as TblSalesOrderViewModel);
                childWindow.Show();
            }
        }

        private void BtnRequestForSample_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SalesOrderType = SalesOrderType.AdvancedSampleRequest;
            _viewModel.SelectedMainRow.DetailsList.Clear();
            _viewModel.SelectedDetailRow = null;
            var childWindow = new AdvanceSampleRequestChildWindow(_viewModel);
            _viewModel.GetSalesOrderLookups();
            childWindow.Show();
        }

        private void BtnPrintSalesOrder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ReportSalesOrder();
        }

        private void btnSeasonalMasterList_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) _viewModel.SelectedMainRow = btn.DataContext as TblStyleViewModel;
            _viewModel.SeasonalMasterListCompleted = true;
            _viewModel.GetSalesOrderLookups();
        }

        private void btnStyleTna_OnClick(object sender, RoutedEventArgs e)
        {
            /*
            var btn = sender as Button;
            if (btn != null) _viewModel.SelectedMainRow = btn.DataContext as TblStyleViewModel;

            var childWindow = new StyleTNAChildWindow(_viewModel);

            childWindow.Show();
            */
        }

        private void BtnColors_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SalesOrderColor(_viewModel, Ordertype);
            child.Show();
        }

        private void BtnTnaColors_OnClick(object sender, RoutedEventArgs e)
        {
            /*
            var child = new StyleTNAColor(_viewModel);
            child.Show();
            */
        }

        private void BtnNotes_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new SalesOrderNotes(_viewModel);
            child.Show();
        }

        private void LookupCombo_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null) return;
            switch (cmb.Tag.ToString())
            {
                case "SizeGroup":
                    if (AddMode == 0)
                    {
                        _viewModel.GetSizeGroup();
                    }
                    break;

                case "StyleStatus":
                    if (AddMode == 0)
                    {
                        _viewModel.GetTblStyleStatus(0);
                    }
                    //else if (AddMode == 1)
                    //{
                    //    _viewModel.GetTblStyleStatus(1);
                    //}
                    break;

                case "StyleCategory":
                    if (AddMode == 0)
                    {
                        _viewModel.GetTblStyleCategory();
                    }
                    //_viewModel.GetTblStyleCategoryLink(DirectionUpdate.SelectedValue.ToString());
                    break;

                case "Family":
                    if (AddMode == 0)
                    { _viewModel.GetFamilyLink(); }

                    break;

                case "Design":
                    _viewModel.GetDesign();
                    break;

                case "Direction":
                    // if (AddMode == 0)
                    _viewModel.GetDirection();
                    break;

                case "SubFamily":
                    if (AddMode == 0)
                        _viewModel.GetSubFamily();
                    break;

                case "StyleTheme":
                    //if (AddMode == 0)
                    //{
                    //    _viewModel.GetTblStyleTheme();
                    //}
                    //break;
                    //if (AddMode == 0)
                    //{
                    //}

                    _viewModel.GetStyleTheme();
                    break;
                case "GenricFabric":
                    if (AddMode == 0)
                        _viewModel.GetTblStyleSpecGenericFabric();
                    break;
            }
        }

        private void StandardBomGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.StandardBomList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.StandardBomList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetStandardBom();
            }
        }

        private void StandardBomGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;

            _viewModel.StandardBomList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetStandardBom();
        }

        private void GetBomBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StandardBomGrid.SelectedItem != null)
            {
                var row = StandardBomGrid.SelectedItem as TblStandardBomHeaderViewModel;
                _viewModel.GenerateBomFromStandard(row);
                MyPopup.IsOpen = false;
            }
        }
        
        private void CopyBomGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CopyBomGrid.SelectedItem != null)
            {
                var row = CopyBomGrid.SelectedItem as TblSalesOrderViewModel;
                if (row != null)
                {
                    foreach (var newRow in row.SalesOrderOperationList)
                    {
                        _viewModel.SelectedDetailRow.SalesOrderOperationList.Add(newRow);
                    }
                    foreach (var newRow in row.BomList)
                    {
                        // newRow.GotPlan = false;
                        _viewModel.SelectedDetailRow.BomList.Add(newRow);
                    }
                }

                CopyBomPopup.IsOpen = false;
            }
        }

        private void BtnGenerateFromStandard_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));
            MyPopup.HorizontalOffset = p.X;
            MyPopup.VerticalOffset = p.Y;
            MyPopup.IsOpen = true;
            _viewModel.GetStandardBom();
        }

        private void Bomcolor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedBomRow.ColorChanged();
        }

        private void btnCopyBom_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));
            CopyBomPopup.HorizontalOffset = p.X;
            CopyBomPopup.VerticalOffset = p.Y;
            CopyBomPopup.IsOpen = true;
            _viewModel.CopybomList.Clear();
            _viewModel.CopyBom();
        }

        private void CopyBomGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.CopybomList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.CopybomList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.CopyBom();
            }
        }

        private void CopyBomGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;

            _viewModel.CopybomList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailSubFilter = filter;
            _viewModel.DetailSubValuesObjects = valueObjecttemp;
            _viewModel.CopyBom();
        }

        private void BomGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveBomRow();
        }

        private void btnClearColors_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            if (btn != null)
            {
                var row = btn.DataContext as BomStyleColorViewModel;
                if (row != null)
                {
                    row.TblColor = null;

                    row.FabricColor = null;
                    row.DummyColor = null;
                    row.DyedColor = null;
                    row.TblColor2 = null;
                }
            }
        }

        private void BomGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedBomRow.GotPurchaseRequest)
            {
                //   e.Cancel = true;
            }
        }

        private void btnBom_Click(object sender, RoutedEventArgs e)
        {
            var child = new BomFabricBom(_viewModel.SelectedBomRow);

            child.Show();
        }

        private void MaterialUsage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
            {
                var be = txt.GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }

            if (_viewModel.SelectedBomRow.BomSizes != null)
                foreach (var row in _viewModel.SelectedBomRow.BomSizes)
                {
                    var firstOrDefault = _viewModel.SelectedBomRow.BomSizes.FirstOrDefault();
                    if (firstOrDefault != null)
                        if (txt != null && firstOrDefault == txt.DataContext)
                        {
                            row.MaterialUsage = firstOrDefault.MaterialUsage;
                        }
                }
        }

        private void DetailGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetSalesOrderColors();
            _viewModel.GetSalesOrderOperations();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var stylelist = new ObservableCollection<TblStyle>();
            var salesOrderlist = new ObservableCollection<TblSalesOrder>();
            var oFile = new OpenFileDialog { Filter = "Excel (*.xls)|*.xls" };

            if (oFile.ShowDialog() == true)
            {
                var fs = oFile.File.OpenRead();

                var book = Workbook.Open(fs);
                var sheet = book.Worksheets[0];

                var description = 0;
                var brand = 0;
                var section = 0;
                var season = 0;
                var family = 0;
                var subfamily = 0;
                var sizeRange = 0;
                var design = 0;
                var direction = 0;
                var targetCostPrice = 0;
                var price = 0;
                var note = 0;
                var category = 0;
                var styleStatus = 0;
                var theme = 0;
                var size = 0;
                var colorcode = 0;
                var qty = 0;
                var styleCode = 0;
                var source = 0;
                var fabricnumber = 0;
                var deliverydate = 0;
                var supplier = 0;
                for (int j = sheet.Cells.FirstColIndex; j < sheet.Cells.LastColIndex + 1; j++)
                {
                    switch (sheet.Cells[0, j].StringValue.ToLower())
                    {
                        case "style code":
                            styleCode = j;
                            break;

                        case "source":
                            source = j;
                            break;

                        case "description":
                            description = j;
                            break;

                        case "size":
                            size = j;
                            break;

                        case "brand":
                            brand = j;
                            break;

                        case "section":
                            section = j;
                            break;

                        case "season":
                            season = j;
                            break;

                        case "family":
                            family = j;
                            break;

                        case "subfamily":
                            subfamily = j;
                            break;

                        case "size range":
                            sizeRange = j;
                            break;

                        case "design":
                            design = j;
                            break;

                        case "direction":
                            direction = j;
                            break;

                        case "target cost price":
                            targetCostPrice = j;
                            break;

                        case "price":
                            price = j;
                            break;

                        case "note":
                            note = j;
                            break;

                        case "category":
                            category = j;
                            break;

                        case "style status":
                            styleStatus = j;
                            break;

                        case "theme":
                            theme = j;
                            break;

                        case "color code":
                            colorcode = j;
                            break;

                        case "qty":
                            qty = j;
                            break;

                        case "fabric number":
                            fabricnumber = j;
                            break;

                        case "supplier":
                            supplier = j;
                            break;

                        case "deliverydate":
                            deliverydate = j;
                            break;
                    }
                }

                switch (TabStyle.SelectedIndex)
                {
                    case (int)StyleTabes.Style:
                    case (int)StyleTabes.StyleDetails:
                    case (int)StyleTabes.RetailSpecDetails:

                        #region Styles

                        for (int i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex + 1; i++)
                        {
                            if (string.IsNullOrEmpty(sheet.Cells[i, note].StringValue.ToUpper().Trim()))
                            {

                                MessageBox.Show("Imported Till Line: " + i);
                                break;

                            }
                            var newemp = new TblStyle();
                            var newseasonalmasterlist = new TblSeasonalMasterList();
                            if (stylelist.All(x => x.Notes.ToUpper().Trim() != sheet.Cells[i, note].StringValue.ToUpper().Trim()))
                            {
                                newemp.TblSeasonalMasterLists = new ObservableCollection<TblSeasonalMasterList>();
                                newemp.StyleCode = (sheet.Cells[i, styleCode].StringValue.ToUpper().Trim() ?? "");
                                newemp.RefStyleCode = (sheet.Cells[i, styleCode].StringValue.ToUpper().Trim() ?? "");
                                newemp.AdditionalDescription = ToTitleCase(sheet.Cells[i, description].StringValue.ToUpper().Trim());
                                newemp.Brand = sheet.Cells[i, brand].StringValue;
                                newemp.TblLkpBrandSection =
                                    _viewModel.BrandSectionList.FirstOrDefault(
                                        w => w.Ename.ToUpper().Trim() == sheet.Cells[i, section].StringValue.ToUpper().Trim())
                                        .Iserial;
                                newemp.TargetCostPrice =
                                    Convert.ToDouble(sheet.Cells[i, targetCostPrice].StringValue.ToUpper().Trim());
                                newemp.CreatedBy = LoggedUserInfo.WFM_UserName;
                                newemp.RetailTargetCostPrice =
                                    Convert.ToDouble(sheet.Cells[i, price].StringValue.ToUpper().Trim());
                                newemp.Notes = sheet.Cells[i, note].StringValue.ToUpper().Trim();

                                newemp.TblStyleCategory1 =
                                    new TblStyleCategory
                                    {
                                        Iserial = 0,
                                        Code = "",
                                        Ename = sheet.Cells[i, category].StringValue.ToUpper().Trim(),
                                        Aname = sheet.Cells[i, source].StringValue.ToUpper().Trim(),
                                    };

                                if (!string.IsNullOrEmpty(sheet.Cells[i, styleStatus].StringValue))
                                {
                                    newemp.TblStyleStatus =

                                        _viewModel.StyleStatusList.SingleOrDefault(
                                            x =>
                                                x.Ename.ToUpper().Trim() ==
                                                sheet.Cells[i, styleStatus].StringValue.ToUpper().Trim()).Iserial;
                                }

                                try
                                {
                                    newemp.TblLkpSeason =
                                                                     _viewModel.SeasonList.FirstOrDefault(
                                                                         w => w.Ename.ToUpper().Trim() == sheet.Cells[i, season].StringValue.ToUpper().Trim())
                                                                         .Iserial;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Season  Doesn't Exist :" + sheet.Cells[i, season].StringValue.ToUpper().Trim());
                                    throw;
                                }


                                try
                                {

                                    //newemp.TblSizeGroup =
                                    //  _viewModel.SizeGroupList.FirstOrDefault(
                                    //      w => w.Ename.ToUpper().Trim() == sheet.Cells[i, sizeRange].StringValue.ToUpper().Trim())
                                    //      .Iserial;

                                    newemp.TblSizeGroup =
                                    _viewModel.SizeGroupList.FirstOrDefault(
                                        w => w.Code.ToUpper().Trim() == sheet.Cells[i, sizeRange].StringValue.ToUpper().Trim())
                                        .Iserial;
                                }

                                catch (Exception)
                                {
                                    MessageBox.Show("SizeGroup  Doesn't Exist :" + sheet.Cells[i, sizeRange].StringValue.ToUpper().Trim());
                                    throw;
                                }

                                try
                                {
                                    newemp.tbl_lkp_FabricDesignes =
                                                                       _viewModel.DesignList.SingleOrDefault(
                                                                           x => x.Ename.ToUpper().Trim() == sheet.Cells[i, design].StringValue.ToUpper().Trim())
                                                                           .Iserial;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Design Doesn't Exist :" + sheet.Cells[i, design].StringValue.ToUpper().Trim());
                                    throw;
                                }


                                newemp.ExternalStyleCode = sheet.Cells[i, fabricnumber].StringValue.ToUpper().Trim();

                                newemp.TblLkpDirection1 = new TblLkpDirection
                                {
                                    Iserial = 0,
                                    Code = "",
                                    Aname = "",
                                    Ename = sheet.Cells[i, direction].StringValue.ToUpper().Trim(),
                                };

                                newemp.TblFamily1 = new TblFamily
                                {
                                    Iserial = 0,
                                    Code = "",
                                    Aname = "",
                                    Ename = sheet.Cells[i, family].StringValue.ToUpper().Trim()
                                };

                                newemp.TblSubFamily1 = new TblSubFamily
                                {
                                    Iserial = 0,
                                    Code = "",
                                    Aname = "",
                                    TblFamily = 0,
                                    Ename = sheet.Cells[i, subfamily].StringValue.ToUpper().Trim()
                                };

                                newemp.Description = sheet.Cells[i, direction].StringValue.ToUpper().Trim() + " " +
                                                     sheet.Cells[i, family].StringValue.ToUpper().Trim() + " " +
                                                     sheet.Cells[i, subfamily].StringValue.ToUpper().Trim() + " " +
                                                     sheet.Cells[i, design].StringValue.ToUpper().Trim();
                                if (!string.IsNullOrWhiteSpace(newemp.AdditionalDescription))
                                {
                                    newemp.Description = sheet.Cells[i, family].StringValue.ToUpper().Trim() + " " +
                                                         newemp.AdditionalDescription;
                                }
                                if (newemp.Description.Length > 50)
                                {
                                    newemp.Description = newemp.Description.Remove(50);
                                }
                                if (deliverydate != 0)
                                {
                                    var brandRow = _viewModel.BrandList.SingleOrDefault(x => x.Brand_Code.ToLower() == sheet.Cells[i, brand].StringValue.ToLower().Trim());

                                    var newsalesorder = new TblSalesOrder
                                    {
                                        SalesOrderCode = sheet.Cells[i, styleCode].StringValue.ToUpper().Trim() + "-001",
                                        DeliveryDate = sheet.Cells[i, deliverydate].DateTimeValue,
                                        AxMethodOfPaymentCode = "cashMethod1",
                                        AxTermOfPaymentCode = "60d",
                                        IsPlannedOrder = true,
                                        ShippingDate = sheet.Cells[i, deliverydate].DateTimeValue,
                                        CreationDate = DateTime.Now,
                                        CreatedBy = LoggedUserInfo.WFM_UserName,
                                        Customer = brandRow.CustomerCode,
                                        SalesOrderType = (int)_viewModel.SalesOrderType,
                                        SerialNo = "001",
                                        AdditionalNotes = sheet.Cells[i, supplier].StringValue.ToUpper().Trim(),
                                    };

                                    newemp.TblSalesOrders = new ObservableCollection<TblSalesOrder>();
                                    newemp.TblSalesOrders.Add(newsalesorder);
                                }

                                stylelist.Add(newemp);
                            }
                            else
                            {
                                newemp =
                                    stylelist.FirstOrDefault(
                                        x => x.Notes.ToUpper().Trim() == sheet.Cells[i, note].StringValue.ToUpper().Trim());
                            }

                            newseasonalmasterlist.Qty = Convert.ToInt32(sheet.Cells[i, qty].StringValue.ToUpper().Trim());
                            newseasonalmasterlist.TblSalesOrderColorTheme1 = new TblSalesOrderColorTheme
                            {
                                Iserial = 0,
                                Code = "",
                                Ename = sheet.Cells[i, theme].StringValue.ToUpper().Trim(),
                                Aname = sheet.Cells[i, size].StringValue.ToUpper().Trim(),
                                TblBrand = "",
                                DeliveryDate = DateTime.Now,
                                ShopDeliveryDate = DateTime.Now,
                                TblLkpBrandSection = 2,
                                TblLkpSeason = 1,
                            };
                            newseasonalmasterlist.ManualCalculation = false;
                            string ColorCode = sheet.Cells[i, colorcode].StringValue.ToUpper().Trim().ToString();
                            int Repeated = 3 - ColorCode.Length;


                            for (int K = 0 ; K < Repeated; K++)
                            {
                                ColorCode = "0" + ColorCode;
                            }
                            newseasonalmasterlist.TblColor1 = new TblColor
                            {
                                Iserial = 0,
                                Aname = "",
                                Code = ColorCode,
                                TblLkpColorGroup = 0,
                            };

                            newemp.TblSeasonalMasterLists.Add(newseasonalmasterlist);
                        }
                        _viewModel.InsertImportedStyles(stylelist);

                        #endregion Styles

                        break;

                    case (int)StyleTabes.SalesOrder:

                        for (var i = sheet.Cells.FirstRowIndex + 1; i < sheet.Cells.LastRowIndex; i++)
                        {
                            var brandRow = _viewModel.BrandList.SingleOrDefault(x => x.Brand_Code.ToLower() == sheet.Cells[i, brand].StringValue.ToLower().Trim());
                            var newemp = new TblSalesOrder();
                            newemp.TblStyle1 = new TblStyle();
                            newemp.TblStyle1.InjectFrom(_viewModel.SelectedMainRow);
                            newemp.TblStyle1.StyleCode = sheet.Cells[i, styleCode].StringValue.ToUpper().Trim();
                            newemp.SalesOrderCode = sheet.Cells[i, supplier].StringValue.ToUpper().Trim();
                            newemp.DeliveryDate = sheet.Cells[i, deliverydate].DateTimeValue;
                            newemp.AxMethodOfPaymentCode = "cashMethod1";
                            newemp.AxTermOfPaymentCode = "60d";
                            newemp.IsPlannedOrder = true;
                            newemp.ShippingDate = newemp.DeliveryDate;
                            newemp.CreationDate = DateTime.Now;
                            newemp.CreatedBy = LoggedUserInfo.WFM_UserName;
                            newemp.Customer = brandRow.CustomerCode;
                            newemp.SalesOrderType = (int)_viewModel.SalesOrderType;

                            salesOrderlist.Add(newemp);
                        }
                        _viewModel.InsertImportedSalesOrder(salesOrderlist);
                        break;
                }
            }
        }

        public string ToTitleCase(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            var result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }

        private void CopyBomPopup_Opened(object sender, EventArgs e)
        {
        }

        private void btnClearVendors_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedBomRow.VendorPerRow = new Vendor();
            _viewModel.SelectedBomRow.Vendor = null;
        }

        private void LayoutRoot_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && ModifierKeys.Control == Keyboard.Modifiers)
            {
                btnSeasonalMasterList_OnClick(null, null);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            GetBomBehavior_DoubleClick(null, null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }

        private void btnRemoveFromPlan_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Are You To Remove That Bom From the purchase plan?", "Delete",
               MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                _viewModel.RemoveFromPlan(_viewModel.SelectedBomRow);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetStyleTNAdata();
        }

        private void BtnTnaCount_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Name == "BtnRequirePo")
            {
                _viewModel.GetTblSupplier();
                showHideTNA_Approve = true;
                ShowHideTNA_Approve(true);
            }
            else
            {
                showHideTNA_Approve = false;
                ShowHideTNA_Approve(false);
            }


            foreach (var item in BtnsStackPanel.Children)
            {
                item.ClearValue(Button.BackgroundProperty);
            }
            btn.Background = new SolidColorBrush(Colors.White);
            _viewModel.StyletnaOption = btn.Tag as string;
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
        }

        private void ReOrderColumns()
        {
            try
            {
                LkpData.LkpDataClient _lkpClient = new LkpData.LkpDataClient();
                _lkpClient.GettblFormLayoutByUserAsync("StyleHeaderMainGrid", LoggedUserInfo.Iserial);
                _lkpClient.GettblFormLayoutByUserCompleted += (s, sv) =>
                {

                    int ColNumber = MainGrid.Columns.Count();
                    foreach (var item in MainGrid.Columns)
                    {
                        if (sv.Result.Count() > 0)
                        {
                            
                            var UserItem = sv.Result.FirstOrDefault(x => x.tblFormLayout1.ColName == item.Header.ToString());
                            if (UserItem != null)
                            {
                                string Name = item.Header.ToString();
                                item.Visibility = Visibility.Visible;
                                item.DisplayIndex = (int)UserItem.ColOrder;
                            }
                            else
                            {
                                item.Visibility = Visibility.Collapsed;
                                ColNumber--;
                                item.DisplayIndex = ColNumber;
                            }
                        }
                        else { item.Visibility = Visibility.Visible; }

                    }
                };
            }
            catch { }
        }

        private void ShowHideTNA_Approve(bool showTNAApprove)
        {
            var p = MainGrid as DataGrid;
            var ts = GetChildren<Button>(p);

            if (showTNAApprove == true)
            {

                ts.ForEach(a =>
                {
                    if (a.Name == "btnTNAColors" || a.Name == "btnTNApproveColor")
                    {
                        a.IsEnabled = true;
                    }
                });
            }
            else
            {
                ts.ForEach(a =>
                {
                    if (a.Name == "btnTNAColors" || a.Name == "btnTNApproveColor")
                    {
                        a.IsEnabled = false;
                    }
                });
            }
        }

        private void RequireTnaChk_Click(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;

            var data = chk.DataContext as TblStyleViewModel;
            if (chk.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(data.LastTnaStatus))
                {
                    var res =
                             MessageBox.Show("TNA Already exist, Create New TNA?",
                                 "Confirm",
                                 MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        _viewModel.RequestTna(chk.IsChecked ?? false);
                        return;
                    }
                    //chk.IsChecked = false;
                }
                else
                {
                    _viewModel.RequestTna(chk.IsChecked ?? false);
                    return;
                }
                chk.IsChecked = false;
            }

            _viewModel.RequestTna(chk.IsChecked ?? false);

        }

        private void DateTnaDeliveryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker.SelectedDate.HasValue)
            {
                foreach (var item in _viewModel.SelectedTnaRow.DetailList)
                {
                    if (!item.ExpectedDeliveryDate.HasValue || _viewModel.SelectedTnaRow.Iserial == 0)
                    {
                        item.ExpectedDeliveryDate = datepicker.SelectedDate.Value.AddDays(-item.StyleTNAPerRow.EstimatedDates ?? 0);
                    }

                }
            }
            //   datepicker.SelectedDate;
        }

        private void BtnDuplicateTna_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext as TblStyleTNADetailViewModel;
            _viewModel.AddNewTNADetailRow(data);
        }

        private void BtnDuplicateTeckPackDesign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                var data = btn.DataContext as TblTechPackDesignDetailViewModel;
                _viewModel.AddNewTechPackDetailRow(data);
            }
            catch {
                MessageBox.Show("Invalid Data");
            }
            
        }

        private void btnReservation_Click(object sender, RoutedEventArgs e)
        {
            FabricReservation child = new FabricReservation(_viewModel);
            child.Show();
        }

        private void DirectionUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {
                if (DirectionUpdate.SelectedValue != null)
                    _viewModel.GetTblStyleCategoryLink(DirectionUpdate.SelectedValue.ToString());

            }
        }

        private void StyleCategoryUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {
                if (StyleCategoryUpdate.SelectedValue != null && DirectionUpdate.SelectedValue != null)
                    _viewModel.GetTblFamilyLinkByCategory(DirectionUpdate.SelectedValue.ToString(), StyleCategoryUpdate.SelectedValue.ToString());
            }
        }

        private void FamilyUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {

                if (StyleCategoryUpdate.SelectedValue != null && DirectionUpdate.SelectedValue != null && FamilyUpdate.SelectedValue != null)
                {
                    chkRepeatedStyles.IsEnabled = true;
                    LoadRepeatedStyles();
                    _viewModel.GetTblSubFamilyLinkByCategory(DirectionUpdate.SelectedValue.ToString(), StyleCategoryUpdate.SelectedValue.ToString(), FamilyUpdate.SelectedValue.ToString());
                }

            }
        }

        private void chkRepeatedStyles_Checked(object sender, RoutedEventArgs e)
        {
            RepeatedStylesUpdate.IsEnabled = true;
            LoadRepeatedStyles();
        }

        private void LoadRepeatedStyles()
        {
            if (BrandUpdate.SelectedValue != null && BrandSectionUpdate.SelectedValue != null && FamilyUpdate.SelectedValue != null && SeasonUpdate.SelectedValue != null)
            {
                LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
                lkpClient.FamilyCategory_GetRepeatedStylesCompleted += (s, sv) =>
                {
                    RepeatedStylesUpdate.ItemsSource = null;
                    RepeatedStylesUpdate.ItemsSource = sv.Result;

                };

                lkpClient.FamilyCategory_GetRepeatedStylesAsync(BrandUpdate.SelectedValue.ToString(), BrandSectionUpdate.SelectedValue.ToString(), FamilyUpdate.SelectedValue.ToString(), SeasonUpdate.SelectedValue.ToString());
                // _viewModel.GetRepeatedStyles(BrandUpdate.SelectedValue.ToString(), BrandSectionUpdate.SelectedValue.ToString(), FamilyUpdate.SelectedValue.ToString());
            }
        }

        private void chkRepeatedStyles_Unchecked(object sender, RoutedEventArgs e)
        {
            RepeatedStylesUpdate.IsEnabled = false;
        }

        private void BrandSectionUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {
                if (BrandUpdate.SelectedValue != null && BrandSectionUpdate.SelectedValue != null)
                {
                    _viewModel.GetTblSizeGroupLinkByBrandSection(BrandUpdate.SelectedValue.ToString(), BrandSectionUpdate.SelectedValue.ToString());
                }
                _viewModel.GetTblStyleThemeLink();
            }
        }

        private void btnTNAColors_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                var MainGridSelectedItem = MainGrid.SelectedItem as TblStyleViewModel;
                if (MainGridSelectedItem.StyleTNAMaxCost.Contains("*"))
                {
                    _viewModel.CheckTnaChanged();
                    _viewModel.SeasonPerRow = _viewModel.SelectedMainRow.SeasonPerRow;
                    _viewModel.GetStyleTNAdata();
                    _viewModel.getSml();
                    _viewModel.AddNewStyleTNAColorDetailRow(true);
                    var child = new StyleTNAColor(_viewModel);
                    child.Show();
                }
                else
                {
                    TNAApprove = 0;
                    openStyleTNAStatusWindow();
                }
            }
            catch { }
            */

        }

        private void openStyleTNAStatusWindow()
        {
            try
            {
                _viewModel.CheckTnaChanged();
                _viewModel.SeasonPerRow = _viewModel.SelectedMainRow.SeasonPerRow;
                // _viewModel.GetStyleTNAdata();
                _viewModel.SelectedMainRow.StyleTnaList.Clear();
                ProductionService.ProductionServiceClient _Client = new ProductionService.ProductionServiceClient();
                _Client.GetTblStyleTNAHeaderAsync(_viewModel.SelectedMainRow.Iserial, _viewModel.SeasonPerRow.Iserial);
                _Client.GetTblStyleTNAHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStyleTNAHeaderViewModel();
                        if (sv.SupplierList != null)
                        {
                            newrow.SupplierPerRow = new TBLsupplier().InjectFrom(sv.SupplierList.FirstOrDefault(x => x.Iserial == row.TblSupplier)) as TBLsupplier;
                        }
                        newrow.SeasonPerRow = _viewModel.SeasonList.FirstOrDefault(w => w.Iserial == row.TblLkpSeason);
                        newrow.CurrencyPerRow = _viewModel.CurrencyList.FirstOrDefault(x => x.Iserial == row.TblCurrency);

                        newrow.InjectFrom(row);


                        foreach (var DetailRowrow in row.TblStyleTNAColorDetails)
                        {
                            var newrowDetail = new TblStyleTNAColorDetailModel();
                            newrowDetail.ColorPerRow = new TblColor().InjectFrom(DetailRowrow.TblColor1) as TblColor;
                            newrowDetail.CurrencyPerRow = _viewModel.CurrencyList.FirstOrDefault(x => x.Iserial == DetailRowrow.TblCurrency);
                            newrowDetail.InjectFrom(DetailRowrow);

                            newrow.StyleTNAColorDetailList.Add(newrowDetail);
                        }
                        if (_viewModel.SelectedMainRow.StyleTnaList.FirstOrDefault(w => w.Iserial == row.Iserial) == null)
                        {
                            _viewModel.SelectedMainRow.StyleTnaList.Add(newrow);
                            if (_viewModel.SelectedTnaRow != newrow)
                            {
                                _viewModel.SelectedTnaRow = newrow;
                            }

                        }
                    }

                    if (!_viewModel.SelectedMainRow.StyleTnaList.Any())
                    {
                        _viewModel.AddNewStyleTNARow(false);
                    }

                    _viewModel.AddNewStyleTNAColorDetailRow(true);
                    //if (!_viewModel.SelectedTnaRow.StyleTNAColorDetailList.Any())
                    //{
                    getPendingTnaColors();
                    // }
                };
                // _viewModel.getSml();

            }
            catch { }
        }

        private void getPendingTnaColors()
        {
            CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();
            _client.GetSeasonalMasterListNotLinkedToStyleTnaByStyleAsync(_viewModel.SelectedMainRow.Iserial);
            _client.GetSeasonalMasterListNotLinkedToStyleTnaByStyleCompleted += (s, sv) =>
            {

                if (sv.Result != null)
                {
                    //SelectedDetailRow.SeasonalMasterList = sv.Result;
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblStyleTNAColorDetailModel();
                        newrow.Cost = Convert.ToDecimal(_viewModel.SelectedTnaRow.TargetCostPrice);
                        newrow.AccCost = _viewModel.SelectedTnaRow.AccCost;
                        newrow.FabricCost = _viewModel.SelectedTnaRow.FabricCost;
                        newrow.OperationCost = _viewModel.SelectedTnaRow.OperationCost;
                        newrow.DeliveryDate = _viewModel.SelectedTnaRow.DeliveryDate;
                        newrow.TblCurrency = _viewModel.SelectedTnaRow.TblCurrency;
                        newrow.CurrencyPerRow = _viewModel.SelectedTnaRow.CurrencyPerRow;
                        newrow.ColorPerRow = row.TblColor1;
                        newrow.TblColor = row.TblColor;
                        newrow.ExchangeRate = _viewModel.SelectedTnaRow.ExchangeRate;
                        newrow.LocalCost = _viewModel.SelectedTnaRow.LocalCost;
                        var currentRowIndex = (_viewModel.SelectedTnaRow.StyleTNAColorDetailList.IndexOf(_viewModel.SelectedStyleTNAColorDetailRow));
                        _viewModel.SelectedTnaRow.StyleTNAColorDetailList.Insert(currentRowIndex + 1, newrow);
                        newrow.Qty = row.Qty;
                        _viewModel.SelectedStyleTNAColorDetailRow = newrow;
                        if (_viewModel.SelectedTnaRow.StyleTNAColorDetailList.Count(x => x.TblColor == row.TblColor) == 0)
                        {
                            _viewModel.SelectedTnaRow.StyleTNAColorDetailList.Add(newrow);
                        }
                    }
                    if (TNAApprove == 0)
                    {
                        var child = new StyleTNAStatus(_viewModel);
                        child.Show();
                    }
                    else if (TNAApprove == 1)
                    {
                        var child = new StyleTNAStatus(_viewModel);
                        child.SaveFromStyleHeader();
                    }

                }
            };
        }

        private void BtnTNARoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TNAStyleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
                var selectedItem = grdTNARoute.SelectedItem as TNAStyleRouteViewModel;
                lkpClient.FamilyCategory_UpdateOrDeleteTblStyleTNARouteAsync(_viewModel.SelectedMainRow.Iserial, selectedItem.Iserial, selectedItem.Checked);

            }
            catch { }
        }

        private void GetStyleTNARoute()
        {
            LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
            lkpClient.FamilyCategory_GetTblStyleTNACompleted += (s, sv) =>
            {
                try
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TNAStyleRouteViewModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code
                        };
                        TNAStyleRouteList.Add(newrow);

                    }
                    grdTNARoute.ItemsSource = null;
                    grdTNARoute.ItemsSource = TNAStyleRouteList;
                }
                catch { }
            };
            lkpClient.FamilyCategory_GetTblStyleTNAAsync();
        }

        private void GetStyleTNARouteByStyle()
        {
            try
            {
                LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
                lkpClient.FamilyCategory_GetTNARouteByStyleCompleted += (s, sv) =>
                {
                    try
                    {
                        foreach (var row in TNAStyleRouteList)
                        {
                            row.Checked = false;
                        }

                        foreach (var row in sv.Result)
                        {
                            var categoryRow = TNAStyleRouteList.SingleOrDefault(x => x.Iserial == row.TblStyleTNA);
                            if (categoryRow != null)
                            {
                                categoryRow.Checked = true;
                            }
                        }
                    }
                    catch { }
                    grdTNARoute.ItemsSource = null;
                    grdTNARoute.ItemsSource = TNAStyleRouteList;
                };
                lkpClient.FamilyCategory_GetTNARouteByStyleAsync(_viewModel.SelectedMainRow.Iserial);
            }
            catch { }

        }

        private void grdTNARoute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StyleDetailsClickedAddMode()
        {
            try
            {
                _viewModel.SeasonListPerRow = _viewModel.SeasonList;
                var p = DataFormStyle as FrameworkElement;
                var ts = GetChildren<TextBox>(p);
                ts.ForEach(a => { a.IsReadOnly = false; });

                var tsc = GetChildren<ComboBox>(p);
                tsc.ForEach(a => { a.IsEnabled = true; });

                foreach (var customPerm in _viewModel.CustomePermissions)
                {
                    try
                    {
                        if (customPerm.Code == "FamilyUpdate")
                        {
                            _viewModel.SelectedMainRow.familyUpdate = true;
                        }
                        if (customPerm.Code == "BrandUpdate")
                        {
                            _viewModel.SelectedMainRow.brandUpdate = true;
                        }
                        if (customPerm.Code == "BrandSectionUpdate")
                        {
                            _viewModel.SelectedMainRow.brandSectionUpdate = true;
                        }
                        if (customPerm.Code == "SeasonUpdate")
                        {
                            _viewModel.SelectedMainRow.seasonUpdate = true;
                        }
                        var item = DataFormStyle.FindName(customPerm.Code) as TextBox;
                        item.IsEnabled = true;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var item = DataFormStyle.FindName(customPerm.Code) as ComboBox;
                            item.IsEnabled = true;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                var item = DataFormStyle.FindName(customPerm.Code) as AutoCompleteBox;

                                item.IsEnabled = true;
                            }
                            catch (Exception)
                            {
                                var item = DataFormStyle.FindName(customPerm.Code) as UserControl;
                                if (item != null)
                                {
                                    item.IsEnabled = true;
                                }
                            }
                        }
                    }
                }

                if (_viewModel.SelectedMainRow.Iserial != 0)
                {
                    _viewModel.SelectedMainRow.familyUpdate = false;
                    _viewModel.SelectedMainRow.brandUpdate = false;
                    _viewModel.SelectedMainRow.brandSectionUpdate = false;
                    _viewModel.SelectedMainRow.seasonUpdate = false;
                }
            }
            catch { }
        }

        private void btnTNApproveColor_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (_viewModel.StyleTnaPerRow.RequireSo > 0)
            {
                _viewModel.StyleTnaPerRow.RequireSo = _viewModel.StyleTnaPerRow.RequireSo - 1;
                _viewModel.StyleTnaPerRow.RequireSoApproval = _viewModel.StyleTnaPerRow.RequireSoApproval + 1;
            }
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            TNAApprove = 1;
            openStyleTNAStatusWindow();
            */
        }

        private void btnGridColumnOrder_Click(object sender, RoutedEventArgs e)
        {

            HtmlPopupWindowOptions options = new HtmlPopupWindowOptions();
            options.Left = 300;
            options.Top = 150;
            options.Width = 1024;
            options.Height = 900;
            options.Directories = false;
            options.Location = false;
            options.Menubar = false;
            options.Status = false;
            options.Toolbar = false;

            Uri myUri = new Uri(startupPath + "#ChildPage.xaml," + LoggedUserInfo.Iserial);
            HtmlPage.PopupWindow(myUri, "self", options);
        }

        private void imgStylePic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StyleImagePreview child = new StyleImagePreview(_viewModel.SelectedMainRow.StyleCode);
            child.Show();
        }

        private void BrandUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {
                if (BrandUpdate.SelectedValue != null && SeasonUpdate.SelectedValue != null)
                    _viewModel.GetTblStyleThemeLink();
            }

            //Check Is ManualCodingCheckManualCodeing
            try
            {
                LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                _client.CheckManualCodeingAsync(_viewModel.SelectedMainRow.Brand);
                _client.CheckManualCodeingCompleted += (s, sv) =>
                {
                    if (sv.Result == true)
                    {
                        RefStyleCodeUpdate.IsReadOnly = false;
                    }
                    else
                    { RefStyleCodeUpdate.IsReadOnly = true; }
                };

            } catch { }
        }

        private void SeasonUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddMode == 1)
            {
                if (BrandUpdate.SelectedValue != null && SeasonUpdate.SelectedValue != null)
                    _viewModel.GetTblStyleThemeLink();
            }
        }

        private void grdTeckPackDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TechPackBomGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (_viewModel.SelectedTechPackBomRow.GotPurchaseRequest)
            {
                //   e.Cancel = true;
            }
        }

        private void TechPackBomGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            _viewModel.SaveTechPackBomRow();
        }

        private void TechPackBomGrid_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Down)
            {
                var currentRowIndex = (_viewModel.SelectedDetailRow.TechPackBomList.IndexOf(_viewModel.SelectedTechPackBomRow));
                if (currentRowIndex == (_viewModel.SelectedDetailRow.TechPackBomList.Count - 1))
                {
                    _viewModel.AddTechPackBom(true);
                }
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.SelectedTechPackBomRows.Clear();
                foreach (var row in TechPackBomGrid.SelectedItems)
                {
                    _viewModel.SelectedTechPackBomRows.Add(row as TechPackBomViewModel);
                }

                _viewModel.DeleteTechPackBom();
            }
        }

        private void TechPackBomGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabTechPackItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TabTechPackItems != null)
                {
                    if (TabTechPackItems.SelectedIndex == 0)
                    {
                        CRUDManagerService.CRUD_ManagerServiceClient _client = new CRUDManagerService.CRUD_ManagerServiceClient();
                        _client.GetTBLTechPackSeasonalMaterListColorsAsync(_viewModel.SelectedMainRow.Iserial);
                        _client.GetTBLTechPackSeasonalMaterListColorsCompleted += (s, sv) =>
                        {
                            _viewModel.SelectedMainRow.SelectedStyleSeasonalMaterColors = sv.Result;
                            _viewModel.GetTechPackBom();
                        };
                    }
                    else if (TabTechPackItems.SelectedIndex == 1)
                    {
                        _viewModel.GetTechPackDetailDesignData();
                    }
                    else if (TabTechPackItems.SelectedIndex == 2)
                    {
                        GetStyleTNARouteByStyle();
                    }
                    else if (TabTechPackItems.SelectedIndex == 3)
                    {
                        _viewModel.GetStyleTNALockup();
                        //_viewModel.SeasonPerRow = _viewModel.SelectedMainRow.SeasonPerRow;
                        //_viewModel.GetStyleTNAdata();
                    }
                }
            }
            catch { }
        }

        private void TechPackDesign_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            //_viewModel.SaveTechPackDesignDetailRow();
        }

        private void BtnTechPackShowImages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TblTechPackDesignDetailViewModel selectedRow = (TblTechPackDesignDetailViewModel)grdTechPackDesignDetail.SelectedItem;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                
                openFileDialog.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == true)
                {
                    Stream stream = (Stream)openFileDialog.File.OpenRead();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    BitmapImage bi = new BitmapImage();
                    bi.SetSource(stream);
                    string fileName = openFileDialog.File.Name;
                    selectedRow.galarylink = fileName;
                    selectedRow.ImageThumb = bytes;
                    selectedRow.ImageName = fileName;

                   
                }
            }
            catch (Exception ex) { string M = ex.Message; }
        }

        private void grdTechPackDesignDetail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                _viewModel.AddNewTechPackDetailRow(null);
            }
            else if (e.Key == Key.Delete)
            {
                List<TblTechPackDesignDetailViewModel> _selectedTechPackDesignDetails = new List<TblTechPackDesignDetailViewModel>();
                try
                {
                    foreach (var row in grdTechPackDesignDetail.SelectedItems)
                    {
                        _selectedTechPackDesignDetails.Add(row as TblTechPackDesignDetailViewModel);
                        _viewModel.SelectedTnaRow.TechPackDesignDetailList.Remove((TblTechPackDesignDetailViewModel)row);
                    }
                }
                catch
                { }

                _viewModel.DeleteTechPackDesingDetailRows(_selectedTechPackDesignDetails);
            }
        }

        private void comStyleTnaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //var SelectedItem = (sender as ComboBox).SelectedItem as ProductionService.TblStyleTNA;
                //if (SelectedItem != null)
                //{
                //    int _CurrentCount = _viewModel.SelectedTnaRow.DetailList.Where(x => x.StyleTNAPerRow.Iserial == SelectedItem.Iserial).Count();

                //    if (_CurrentCount >= SelectedItem.MaxRepeated)
                //    {
                //        var itemToRemove = _viewModel.StyleTNAListTemp.Single(r => r.Iserial == SelectedItem.Iserial);
                //        _viewModel.StyleTNAListTemp.Remove(itemToRemove);
                //        MessageBox.Show(string.Format(@"You can't Add More Than {0} {1}", SelectedItem.MaxRepeated,SelectedItem.Ename));
                //    }
                //}
            }
            catch { }
        }

        private void BtnTechPackBomTrimImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                TechPackBomViewModel selectedRow = (TechPackBomViewModel)TechPackBomGrid.SelectedItem;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == true)
                {
                    Stream stream = (Stream)openFileDialog.File.OpenRead();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    BitmapImage bi = new BitmapImage();
                    bi.SetSource(stream);
                    string fileName = openFileDialog.File.Name;
                    selectedRow.galarylink = fileName;
                    selectedRow.ImageThumb = bytes;
                    selectedRow.ImageName = fileName;

                    _viewModel.SaveTechPackBomRow();
                }
            } catch { }
        }

        private void txtTeckPackBOMDescription_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (_viewModel.SelectedMainRow != null && _viewModel.SelectedMainRow.tblTechPackBOMComment.Comment != txtTeckPackBOMDescription.Text)
            {
                _viewModel.SelectedMainRow.tblTechPackBOMComment.Comment = txtTeckPackBOMDescription.Text;
                _viewModel.UpdateOrInsertTechPackBOMComment();

            }
        }

        private void CopyTechPackBomPopup_Opened(object sender, EventArgs e)
        {

        }

        private void CopyTechPackBomGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_viewModel.CopyTechPackbomList.Count < _viewModel.PageSize)
            {
                return;
            }
            if (_viewModel.CopyTechPackbomList.Count - 2 < e.Row.GetIndex() && !_viewModel.Loading)
            {
                _viewModel.Loading = true;
                _viewModel.GetTechBackBomCopyList();
            }
        }

        private void CopyTechPackBomGrid_OnFilter(object sender, FilterEvent e)
        {
            if (_viewModel.Loading) return;
            _viewModel.CopyTechPackbomList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailSubFilter = filter;
            _viewModel.DetailSubValuesObjects = valueObjecttemp;
            _viewModel.GetTechBackBomCopyList();
           
        }

        private void BtnCopyTechPackBom_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            //var gt = b.TransformToVisual(Application.Current.RootVisual);
            //var p = gt.Transform(new Point(0, b.ActualHeight));
            //CopyTechPackBomPopup.HorizontalOffset = p.X;
            //CopyTechPackBomPopup.VerticalOffset = p.Y;
            CopyTechPackBomPopup.IsOpen = true;
            _viewModel.GetTechBackBomCopyList();
        }

        private void CopyTechPackBomGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CopyTechPackBomGrid.SelectedItem != null)
            {
                var row = CopyTechPackBomGrid.SelectedItem as LkpData.TblStyle;
                if (row != null)
                {
                    CRUDManagerService.CRUD_ManagerServiceClient _client = new CRUDManagerService.CRUD_ManagerServiceClient();
                    _client.GetTBLTechPackSeasonalMaterListColorsAsync(row.Iserial);
                    _client.GetTBLTechPackSeasonalMaterListColorsCompleted += (s, sv) =>
                    {
                       // _viewModel.SelectedMainRow.SelectedStyleSeasonalMaterColors = sv.Result;
                        GetSelectedCopiedTechBom(row.Iserial);
                    };
                    //foreach (var newRow in row.SalesOrderOperationList)
                    //{
                    //    _viewModel.SelectedDetailRow.SalesOrderOperationList.Add(newRow);
                    //}
                    //foreach (var newRow in row.BomList)
                    //{
                    //    _viewModel.SelectedDetailRow.BomList.Add(newRow);
                    //}
                }
                CopyTechPackBomPopup.IsOpen = false;
            }
        }

        private void GetSelectedCopiedTechBom(int iserial)
        {
            CRUDManagerService.CRUD_ManagerServiceClient _client = new CRUDManagerService.CRUD_ManagerServiceClient();
            _client.GetTechPackBomAsync(iserial);
            _client.GetTechPackBomCompleted += (s, sv) => 
            {
               //SelectedDetailRow.TechPackBomList.Clear();
                foreach (var row in sv.Result)
                {
                    foreach (var styleColor in row.TblTechPackBOMStyleColors)
                    {
                        styleColor.DummyColor = styleColor.FabricColor;
                    }
                    row.Iserial = 0;
                    var newrow = new TechPackBomViewModel();
                    
                    if (row.BOM_CalcMethod != null)
                        newrow.BOM_CalcMethodPerRow = new GenericTable().InjectFrom(row.BOM_CalcMethod) as GenericTable; // row.BOM_CalcMethod;
                    if (row.BOM_FabricType != null)
                        newrow.BOM_FabricTypePerRow = new GenericTable().InjectFrom(row.BOM_FabricType1) as GenericTable;
                    newrow.BomStyleColors = new ObservableCollection<TechPackBomStyleColorViewModel>();
                    newrow.BomStyleColors.Clear();
                    foreach (var item in _viewModel.SelectedMainRow.SelectedStyleSeasonalMaterColors)
                    {
                        if (item != null)
                        {
                            var newbomcolor = new TechPackBomStyleColorViewModel
                            {
                                StyleColor = item.Iserial,
                                TblColor1 = (TblColor)item,
                                Bom = _viewModel.SelectedTechPackBomRow.Iserial
                            };

                            foreach (var variable in row.TblTechPackBOMStyleColors.Where(x => x.StyleColor == item.Iserial))
                            {
                                newbomcolor.InjectFrom(variable);
                            }
                            newrow.BomStyleColors.Add(newbomcolor);
                        }
                    }

                    GenericMapper.InjectFromObCollection(newrow.BomSizes, row.TblTechPackBOMSizes);
                    var sizesToAdd = from c in _viewModel.Sizes where !(from o in newrow.BomSizes select o.StyleSize).Contains(c.SizeCode) select c.SizeCode;
                    foreach (var VARIABLE in sizesToAdd)
                    {
                        newrow.BomSizes.Add(new TblTechPackBOMSize
                        {
                            StyleSize = VARIABLE,
                            MaterialUsage = 1
                        });
                    }
                    newrow.InjectFrom(row);
                 

                    newrow.GotPurchaseRequest = false;
                    newrow.GotPlan = false;
                    newrow.ItemPerRow = sv.itemsList.SingleOrDefault(x => x.Iserial == row.BOM_Fabric && x.ItemGroup == row.BOM_FabricType);
                    newrow.VendorPerRow = sv.vendorList.FirstOrDefault(w => w.vendor_code == row.Vendor);
                    foreach (var colorRow in newrow.BomStyleColors)
                    {
                        colorRow.DummyColor = colorRow.DummyColor;
                    }
                    newrow.galarylink = null;
                    newrow.ImageName = "";
                   _viewModel.SelectedDetailRow.TechPackBomList.Add(newrow);
                }
            };
        }

        private void comBrandFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _viewModel.BrandFilter = comBrandFilter.SelectedValue.ToString();
                _viewModel.GetFilterBrandSections();
            }
            catch { }
        }

        private void comBrandSectionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetDirectionFilter();
        }

        private void comDirectionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetStyleCategoryFilter();
        }

        private void comStyleCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetFamilyFilter();
        }

        private void comFamilyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.GetSubFamilyFilter();
        }

        private void comSubFamilyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            comBrandFilter.SelectedIndex = -1;
            _viewModel.ClearStyleGridFilters();
        }

        private void BtnCopyTechPackBomLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var currentRowIndex = (_viewModel.SelectedDetailRow.TechPackBomList.IndexOf(_viewModel.SelectedTechPackBomRow));
                //if (currentRowIndex == (_viewModel.SelectedDetailRow.TechPackBomList.Count - 1))
                //{
                    _viewModel.CopyTechPackBomRow(true);
                //}

            } catch { }
        }

        private void grdTechPackDesignDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // _viewModel.StyleTechPackPartListTemp
            //= _viewModel.StyleTechPackPartList
            //  _viewModel.SelectedTnaRow.TechPackDesignDetailList)
            //_viewModel.StyleTechPackPartListTemp = _viewModel.StyleTechPackPartList.ToList();
            //foreach (var item in _viewModel.SelectedTnaRow.TechPackDesignDetailList)
            //{
            //    LkpData.tblTechPackPart selectedPart = _viewModel.StyleTechPackPartListTemp.Where(x => x.Iserial == item.TechPackPartPerRow.Iserial
            //                                                        && x.Iserial != _viewModel.SelectedTnaRow.SelectedTechPackDesignDetailRow.TechPackPartPerRow.Iserial ).FirstOrDefault();
            //    _viewModel.StyleTechPackPartListTemp.Remove(selectedPart);
            //}
        }

        private void BtnPrintStyleHeader_Click(object sender, RoutedEventArgs e)
        {
            var  para = new ObservableCollection<string>
                       {
                                _viewModel.SelectedMainRow.Iserial
                                .ToString(CultureInfo.InvariantCulture),
                                LoggedUserInfo.Ip + LoggedUserInfo.Port,
                                LoggedUserInfo.DatabasEname
                        };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport("Style Sheet", para);
        }

        private void BtnRemoveSaleOrderApprove_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_viewModel.SelectedDetailRow.SalesOrderCode))
                _viewModel.RemoveApprovedSalesOrder();
            else
                MessageBox.Show("No SalesOrder is selected");
        }

        private void btnCancelRequest_Click(object sender, RoutedEventArgs e)
        {
            GetSalesorderColorData();

        }
        private void GetSalesorderColorData()
        {
            _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
            if (OrgType == SalesOrderType.SalesOrderPo)
            {
                _viewModel.SalesOrderType = SalesOrderType.SalesOrderPo;
            }
            else if (OrgType == SalesOrderType.RetailPo)
            {
                _viewModel.SalesOrderType = SalesOrderType.RetailPo;
            }
            
            var child = new SalesOrderColorCancelRequest(_viewModel);
            child.Show();
        }

        private void comStyleTnaStatusList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void grdStyleTechPackTNA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void grdStyleSpecDetails_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {

        }

        private void grdStyleSpecDetails_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                 var data = new TblStyleSpecDetailsViewModel();
                _viewModel.AddNewStyleSpecDetailRow(data);
            }
            else if (e.Key == Key.Delete)
            {
                List<TblStyleSpecDetailsViewModel> _selectedStyleSpecDetails = new List<TblStyleSpecDetailsViewModel>();
                try
                {
                    foreach (var row in grdStyleSpecDetails.SelectedItems)
                    {
                        _selectedStyleSpecDetails.Add(row as TblStyleSpecDetailsViewModel);
                        _viewModel.SelectedMainRow.StyleSpecDetailsList.Remove((TblStyleSpecDetailsViewModel)row);
                    }
                }
                catch
                { }

                _viewModel.DeleteStyleSpecDetailsRows(_selectedStyleSpecDetails);
            }
        }

        private void BtnStyleSpecDetailsShowImages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TblStyleSpecDetailsViewModel selectedRow = (TblStyleSpecDetailsViewModel)grdStyleSpecDetails.SelectedItem;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                //openFileDialog.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                selectedRow.TblStyleSpecDetailAttachment = new ObservableCollection<LkpData.tblStyleSpecDetailAttachment>();
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var file in openFileDialog.Files)
                    {
                        Stream stream = (Stream)file.OpenRead();
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(stream);

                        LkpData.tblStyleSpecDetailAttachment _attachment = new LkpData.tblStyleSpecDetailAttachment();
                        _attachment.galaryLint = file.Name;
                        _attachment.ImageThumb = bytes;
                        _attachment.FileName = file.Name;
                        selectedRow.TblStyleSpecDetailAttachment.Add(_attachment);
                    }
                }
            }
            catch (Exception ex) { string M = ex.Message; }
        }

        private void BtnDuplicateStyleSpecDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                var data = btn.DataContext as TblStyleSpecDetailsViewModel;
                _viewModel.AddNewStyleSpecDetailRow(data);
            }
            catch
            {
                MessageBox.Show("Invalid Data");
            }
        }

        private void FabricStyleSpecUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void grdStyleSpecDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnGetFileDetails_Click(object sender, RoutedEventArgs e)
        {
            TblStyleSpecDetailsViewModel selectedRow = (TblStyleSpecDetailsViewModel)grdStyleSpecDetails.SelectedItem;
            if(selectedRow.Iserial > 0)
            {
                var child = new StyleSpecFileDetails(_viewModel.SelectedMainRow.Iserial, selectedRow.Iserial);
                child.Show();
            }
        }

        private void btnStyleImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == true)
                {
                    Stream stream = (Stream)openFileDialog.File.OpenRead();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    BitmapImage bi = new BitmapImage();
                    bi.SetSource(stream);
                    string fileName = openFileDialog.File.Name;
                    _viewModel.SelectedMainRow.galarylink = fileName;
                    _viewModel.SelectedMainRow.StyleImage = bytes;
                    _viewModel.SelectedMainRow.ImageName = fileName;
                }
            }
            catch (Exception ex) { string M = ex.Message; }
        }

        private void BtnTNACalandarShowImages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TblStyleTNADetailViewModel selectedRow = (TblStyleTNADetailViewModel)grdStyleTechPackTNA.SelectedItem;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                //openFileDialog.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                selectedRow.TblStyleTNADetailAttachment = new ObservableCollection<ProductionService.TblStyleTNADetailAttachment>();
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var file in openFileDialog.Files)
                    {
                        Stream stream = (Stream)file.OpenRead();
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(stream);

                        ProductionService.TblStyleTNADetailAttachment _attachment = new ProductionService.TblStyleTNADetailAttachment();
                        _attachment.galaryLink = file.Name;
                        _attachment.ImageThumb = bytes;
                        _attachment.FileName = file.Name;
                        selectedRow.TblStyleTNADetailAttachment.Add(_attachment);
                    }
                }
            }
            catch (Exception ex) { string M = ex.Message; }
        }

        private void btnGetFileTNACalandar_Click(object sender, RoutedEventArgs e)
        {
            TblStyleTNADetailViewModel selectedRow = (TblStyleTNADetailViewModel)grdStyleTechPackTNA.SelectedItem;
            if (selectedRow.Iserial > 0)
            {
                var child = new StyleTNADetails(selectedRow.Iserial);
                child.Show();
            }
        }
    }
}
