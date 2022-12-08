using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels.Mappers;
using CCWFM.Views.OGView.ChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class DyeingPlanViewModel : ViewModelBase
    {
        #region Properties

        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        private ObservableCollection<TblService> _serviceList;

        public ObservableCollection<TblService> ServiceList
        {
            get { return _serviceList; }
            set { _serviceList = value; RaisePropertyChanged("ServiceList"); }
        }

        private ObservableCollection<TblPurchaseOrderDetail> _purchaseDetailList;

        public ObservableCollection<TblPurchaseOrderDetail> PurchaseDetailList
        {
            get { return _purchaseDetailList; }
            set { _purchaseDetailList = value; RaisePropertyChanged("PurchaseDetailList"); }
        }

        public string DefaultDyeingVendor { get; set; }

        public string DefaultFinishedFabricWarehouse { get; set; }

        private ObservableCollection<GenericTable> _dyeingTypeList;

        public ObservableCollection<GenericTable> DyeingTypeList
        {
            get { return _dyeingTypeList; }
            set { _dyeingTypeList = value; RaisePropertyChanged("DyeingTypeList"); }
        }

        private ObservableCollection<FabricStorage_Result> _fabricStorageList;

        public ObservableCollection<FabricStorage_Result> FabricStorageList
        {
            get
            {
                return _fabricStorageList;
            }
            set
            {
                if ((ReferenceEquals(_fabricStorageList, value) != true))
                {
                    _fabricStorageList = value;
                    RaisePropertyChanged("FabricStorageList");
                }
            }
        }

        private ObservableCollection<TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<TblLkpBrandSection>()); }
            set { _brandSectionList = value; RaisePropertyChanged("BrandSectionList"); }
        }

        private ObservableCollection<TblWarehouse> _warehouse;

        public ObservableCollection<TblWarehouse> WareHouseList
        {
            get
            {
                return _warehouse;
            }
            set
            {
                if ((ReferenceEquals(_warehouse, value) != true))
                {
                    _warehouse = value;
                    RaisePropertyChanged("WareHouseList");
                }
            }
        }

        private ObservableCollection<tbl_FabricAttriputes> _fabricAttList;

        public ObservableCollection<tbl_FabricAttriputes> FabricAttList
        {
            get
            {
                return _fabricAttList;
            }
            set
            {
                if ((ReferenceEquals(_fabricAttList, value) != true))
                {
                    _fabricAttList = value;
                    RaisePropertyChanged("FabricAttList");
                }
            }
        }

        private ObservableCollection<DyeingSummeryViewModel> _dyeingSummeryViewModel;

        public ObservableCollection<DyeingSummeryViewModel> DyeingSummeryViewModelList
        {
            get
            {
                return _dyeingSummeryViewModel;
            }
            set
            {
                if ((ReferenceEquals(_dyeingSummeryViewModel, value) != true))
                {
                    _dyeingSummeryViewModel = value;
                    RaisePropertyChanged("DyeingSummeryViewModelList");
                }
            }
        }

        //private void LotsMasterList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "RequiredQuantity")
        //    {
        //        if (DyeingSummerySelectedRow != null)
        //        {
        //            var summaryRows = DyeingSummeryViewModelList.Where(l => l.FabricCode == DyeingSummerySelectedRow.FabricCode);
        //            double requiredQty = 0;
        //            //foreach (var row in summaryRows)
        //            //{
        //            //    requiredQty = requiredQty + row.LotsMasterList.FirstOrDefault().RequiredQuantity;
        //            //}
        //            requiredQty = summaryRows.Sum(x => x.CalculatedTotalQty);
        //            double orgQty = 0;
        //            foreach (var rows in summaryRows)
        //            {
        //                orgQty = orgQty + rows.LotsMasterList.FirstOrDefault().AvaliableQuantityOrg;
        //            }
        //            foreach (var rows in summaryRows)
        //            {
        //                rows.LotsMasterList.FirstOrDefault().AvaliableQuantity = orgQty - requiredQty;
        //            }
        //        }
        //        else
        //        {
        //            var fabrics = (DyeingSummeryViewModelList.Select(x => x.FabricCode).Distinct());
        //            foreach (var sumRow in fabrics)
        //            {
        //                var summaryRows = DyeingSummeryViewModelList.Where(l => l.FabricCode == sumRow);
        //                double requiredQty = 0;
        //                //foreach (var row in summaryRows)
        //                //{
        //                //    requiredQty = requiredQty + row.LotsMasterList.FirstOrDefault().RequiredQuantity;
        //                //}
        //                requiredQty = summaryRows.Sum(x => x.CalculatedTotalQty);
        //                double orgQty = 0;
        //                foreach (var rows in summaryRows)
        //                {
        //                    orgQty = orgQty + rows.LotsMasterList.FirstOrDefault().AvaliableQuantityOrg;
        //                }
        //                foreach (var rows in summaryRows)
        //                {
        //                    rows.LotsMasterList.FirstOrDefault().AvaliableQuantity = orgQty - requiredQty;
        //                }
        //            }
        //        }
        //    }
        //}

        public void CalcfabricAgain(string fabric)
        {
            //var summaryRows = DyeingSummeryViewModelList.Where(l => l.FabricCode == fabric);
            //double requiredQty = 0;
            ////foreach (var row in summaryRows)
            ////{
            ////    requiredQty = requiredQty + row.LotsMasterList.FirstOrDefault().RequiredQuantity;
            ////}
            //requiredQty = summaryRows.Sum(x => x.CalculatedTotalQty);
            //double orgQty = 0;
            //foreach (var rows in summaryRows)
            //{
            //    orgQty = orgQty + rows.LotsMasterList.FirstOrDefault().AvaliableQuantityOrg;
            //}

            //foreach (var rows in summaryRows)
            //{
            //    var tblDyeingPlanLotsMasterViewModel = rows.LotsMasterList.FirstOrDefault();
            //    if (tblDyeingPlanLotsMasterViewModel != null)
            //        tblDyeingPlanLotsMasterViewModel.AvaliableQuantity = orgQty - requiredQty;
            //}
        }

        private DyeingSummeryViewModel _dyeingSummerySelectedRow;

        public DyeingSummeryViewModel DyeingSummerySelectedRow
        {
            get
            {
                return _dyeingSummerySelectedRow;
            }
            set
            {
                if ((ReferenceEquals(_dyeingSummerySelectedRow, value) != true))
                {
                    _dyeingSummerySelectedRow = value;
                    RaisePropertyChanged("DyeingSummerySelectedRow");
                }
            }
        }

        private ObservableCollection<tbl_lkp_DyingClassification> _dyeingClass;

        public ObservableCollection<tbl_lkp_DyingClassification> DyeingClassList
        {
            get
            {
                return _dyeingClass;
            }
            set
            {
                if ((ReferenceEquals(_dyeingClass, value) != true))
                {
                    _dyeingClass = value;
                    RaisePropertyChanged("DyeingClassList");
                }
            }
        }

        private ObservableCollection<Brand> _brandsList;

        public ObservableCollection<Brand> BrandsList
        {
            get
            {
                return _brandsList;
            }
            set
            {
                if ((ReferenceEquals(_brandsList, value) != true))
                {
                    _brandsList = value;
                    RaisePropertyChanged("BrandsList");
                }
            }
        }

        private ObservableCollection<TblLkpSeason> _seasonList;

        public ObservableCollection<TblLkpSeason> SeasonList
        {
            get
            {
                return _seasonList;
            }
            set
            {
                if ((ReferenceEquals(_seasonList, value) != true))
                {
                    _seasonList = value;
                    RaisePropertyChanged("SeasonList");
                }
            }
        }

      

        private ObservableCollection<WareHouseDto> _warehouseWithOnhandList;

        public ObservableCollection<WareHouseDto> WarehouseWithOnHandList
        {
            get
            {
                return _warehouseWithOnhandList;
            }
            set
            {
                if ((ReferenceEquals(_warehouseWithOnhandList, value) != true))
                {
                    _warehouseWithOnhandList = value;
                    RaisePropertyChanged("WarehouseWithOnHandList");
                }
            }
        }

        private ObservableCollection<ColorHeader> _colorHeader;

        public ObservableCollection<ColorHeader> ColorHeader
        {
            get
            {
                return _colorHeader;
            }
            set
            {
                if ((ReferenceEquals(_colorHeader, value) != true))
                {
                    _colorHeader = value;
                    RaisePropertyChanged("ColorHeader");
                }
            }
        }

        private TblDyeingPlanHeaderViewModel _headerViewModel;

        public TblDyeingPlanHeaderViewModel HeaderViewModel
        {
            get
            {
                return _headerViewModel;
            }
            set
            {
                if ((ReferenceEquals(_headerViewModel, value) != true))
                {
                    _headerViewModel = value;
                    RaisePropertyChanged("HeaderViewModel");
                }
            }
        }

        private ObservableCollection<TblDyeingPlanHeaderViewModel> _tblDyeingPlanHeaderViewModelList;

        public ObservableCollection<TblDyeingPlanHeaderViewModel> DyeingPlanHeaderViewModelList
        {
            get
            {
                return _tblDyeingPlanHeaderViewModelList ?? (_tblDyeingPlanHeaderViewModelList = new ObservableCollection<TblDyeingPlanHeaderViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblDyeingPlanHeaderViewModelList, value) != true))
                {
                    _tblDyeingPlanHeaderViewModelList = value;
                    RaisePropertyChanged("DyeingPlanHeaderViewModelList");
                }
            }
        }

        private TblDyeingPlanViewModel _selectedRow;

        public TblDyeingPlanViewModel SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                if ((ReferenceEquals(_selectedRow, value) != true))
                {
                    _selectedRow = value;
                    RaisePropertyChanged("SelectedRow");
                }
            }
        }

        private TblDyeingPlanLotsMasterViewModel _selectedLotsMasterRow;

        public TblDyeingPlanLotsMasterViewModel SelectedLotsMasterRow
        {
            get
            {
                return _selectedLotsMasterRow;
            }
            set
            {
                if ((ReferenceEquals(_selectedLotsMasterRow, value) != true))
                {
                    _selectedLotsMasterRow = value;
                    RaisePropertyChanged("SelectedLotsMasterRow");
                }
            }
        }

        private ObservableCollection<DyeingAxService> _dyeingServiceSummaryList;

        public ObservableCollection<DyeingAxService> DyeingAxServiceSummaryList
        {
            get
            {
                return _dyeingServiceSummaryList;
            }
            set
            {
                if ((ReferenceEquals(_dyeingServiceSummaryList, value) != true))
                {
                    _dyeingServiceSummaryList = value;
                    RaisePropertyChanged("DyeingAxServiceSummaryList");
                }
            }
        }

        private ObservableCollection<DyeingSummaryServicesViewModel> _dyeingSummaryServices;

        public ObservableCollection<DyeingSummaryServicesViewModel> DyeingSummaryServices
        {
            get
            {
                return _dyeingSummaryServices;
            }
            set
            {
                if ((ReferenceEquals(_dyeingSummaryServices, value) != true))
                {
                    _dyeingSummaryServices = value;
                    RaisePropertyChanged("DyeingSummaryServices");
                }
            }
        }

        private PagedCollectionView _summaryPagedCollection;

        public PagedCollectionView SummaryPagedCollection
        {
            get
            {
                return _summaryPagedCollection;
            }
            set
            {
                _summaryPagedCollection = value;
                RaisePropertyChanged("SummaryPagedCollection");
            }
        }

        #endregion Properties

        public DyeingPlanViewModel()
        {
            DyeingSummeryViewModelList = new ObservableCollection<DyeingSummeryViewModel>();
            DyeingClassList = new ObservableCollection<tbl_lkp_DyingClassification>();
            FabricStorageList = new ObservableCollection<FabricStorage_Result>();
            Client.GetDyeingClassificaitonAsync();
            Client.GetDyeingClassificaitonCompleted += (d, s) =>
                {
                    foreach (var item in s.Result)
                    {
                        DyeingClassList.Add(item);
                    }
                };

            lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
            {
                BrandSectionList.Clear();
                foreach (var row in sv.Result)
                {
                    BrandSectionList.Add( new TblLkpBrandSection().InjectFrom( row.TblLkpBrandSection1) as TblLkpBrandSection);
                    if (row.TblLkpBrandSection == HeaderViewModel.TblLkpBrandSection)
                    {
                        HeaderViewModel.BrandSectionPerRow = new LkpData.TblLkpBrandSection().InjectFrom( row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection;
                    }
                }
                //HeaderViewModel.BrandSectionPerRow =
                //    BrandSectionList.Where(
                //        x =>
                //            x.TblLkpBrandSectionLinks.FirstOrDefault(
                //                w => w.TblLkpBrandSection == HeaderViewModel.TblLkpBrandSection)).;
            };
            Client.GetFabricWithUnitAndDyeingClassCompleted += (d, sv) =>
            {
                FabricAttList = sv.Result;
            };
            ChaingSetupMethod.GetSettings("Dyeing Order", Client);

            Client.GetChainSetupCompleted += (a, s) =>
            {
                foreach (var item in s.Result)
                {
                    if (item.sGlobalSettingCode == "DefaultDyeingVendorCode")
                    {
                        DefaultDyeingVendor = item.sSetupValue;
                    }
                    else if (item.sGlobalSettingCode == "DefaultFinishedFabricWarehouse")
                    {
                        DefaultFinishedFabricWarehouse = item.sSetupValue;
                    }
                }
            };

            WareHouseList = new ObservableCollection<TblWarehouse>();
            Client.GetTblWarehouseAsync(0,int.MaxValue,"it.Iserial",null,null);
            Client.GetTblWarehouseCompleted += (s, w) =>
            {
                foreach (var item in w.Result)
                {
                    WareHouseList.Add(item);
                }
            };

            Client.GetLocationDetailsCompleted += (s, sv) =>
            {
                FabricStorageList = sv.Result;
            };

            BrandsList = new ObservableCollection<Brand>();
            SeasonList = new ObservableCollection<TblLkpSeason>();
            

            HeaderViewModel = new TblDyeingPlanHeaderViewModel();

            SelectedRow = new TblDyeingPlanViewModel();
            DyeingAxServiceSummaryList = new ObservableCollection<DyeingAxService>();
            HeaderViewModel.FromDate = null;
            HeaderViewModel.ToDate = null;
            HeaderViewModel.TransactionDate = DateTime.Now;
            Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
            Client.SeasonsAsync();
            Client.GetWarehouseswithOnHandAsync();
            Client.GetAxSummaryServicesAsync();
            Client.DeleteDyeingPlanHeaderCompleted += (s, sv) =>
            {
                if (sv.Error != null && sv.Error.Message == null)
                {
                    MessageBox.Show("Deleted");
                }
                else
                {
                    if (sv.Error != null) MessageBox.Show(sv.Error.Message);
                }
            };
            Client.GetAxSummaryServicesCompleted += (d, s) =>
            {
                foreach (var item in s.Result)
                {
                    DyeingAxServiceSummaryList.Add(item);
                }
            };

            Client.GetSavedSummaryServicesCompleted += (d, e) =>
            {
                if (PurchaseDetailList != null)
                {
                    var purchasedetails = PurchaseDetailList.Where(x => x.ItemId == DyeingSummerySelectedRow.DyedFabric && x.TblColor.Code == DyeingSummerySelectedRow.ColorName);

                    var serviceqty = new ObservableCollection<DyeingSummaryServicesViewModel>();

                    foreach (var purchaseDetail in purchasedetails)
                    {
                        foreach (var breakdown in purchaseDetail.TblPurchaseOrderDetailBreakDowns)
                        {
                            foreach (var service in breakdown.BOM1.BomFabricBoms.Where(x => x.ItemType == "Service"))
                            {
                                var srv = ServiceList.FirstOrDefault(x => x.Iserial == service.Item);
                                var temprow = serviceqty.FirstOrDefault(x => srv != null && x.ServiceCode == srv.Code);
                                if (temprow != null)
                                {
                                    temprow.Qty = (double)(temprow.Qty + (breakdown.Qty * service.MaterialUsage));
                                }
                                else
                                {
                                    if (srv != null)
                                        serviceqty.Add(new DyeingSummaryServicesViewModel
                                        {
                                            ServiceCode = srv.Code,
                                            Qty = (double)(breakdown.Qty * service.MaterialUsage),
                                        });
                                }
                            }
                        }
                    }

                    DyeingSummaryServices = new ObservableCollection<DyeingSummaryServicesViewModel>();
                    foreach (var item in DyeingAxServiceSummaryList)
                    {
                        double qty = 0;
                        bool flagg = false;
                        if (serviceqty.Any(x => x.ServiceCode == item.ServiceCode))
                        {
                            var sum = serviceqty.Where(x => x.ServiceCode == item.ServiceCode).Sum(x => x.Qty);
                            qty = sum;
                            flagg = true;
                        }

                        DyeingSummaryServices.Add(new DyeingSummaryServicesViewModel
                        {
                            ServiceCode = item.ServiceCode,
                            ServicEname = item.ServiceName,
                            Qty = qty,
                            ItemChecked = flagg
                        });
                    }
                }

                foreach (var row in e.Result)
                {
                    var serviceRow = DyeingSummaryServices.SingleOrDefault(x => x.ServiceCode == row.ServiceCode);
                    if (serviceRow != null)
                    {
                        serviceRow.SummaryRowIserial = row.SummaryRowIserial;
                        serviceRow.Notes = row.Notes;
                        serviceRow.ItemChecked = true;
                        if (row.Qty != null) serviceRow.Qty = (double)row.Qty;
                    }
                }
            };
            Client.GetEstimatedDyeingListFromProductionCompleted += (s, sv) =>
            {
                HeaderViewModel.DyeingViewModelList = new ObservableCollection<TblDyeingPlanViewModel>();
                ColorHeader = new ObservableCollection<ColorHeader>();

                foreach (var item in sv.ColorsValues.Distinct())
                {
                    var test = new ColorHeader
                    {
                        BatchNo = sv.BatchNo,
                        ColorName = item.FabricColor
                    };

                    if (ColorHeader.All(x => x.ColorName != test.ColorName))
                    {
                        test.BatchNo = test.BatchNo;
                        ColorHeader.Add(test);
                    }
                }

                foreach (var item in sv.Result.Distinct())
                {
                    if (HeaderViewModel.DyeingViewModelList.Count(x => x.SalesOrder == item.SalesOrderID && x.FabricCode == item.FabricCode) == 0)
                    {
                        HeaderViewModel.DyeingViewModelList.Add(DyeingPlanMapper.VmMapToDyeingPLanEstimated(item, sv.ColorsValues.Where(x => x.Fabric_Code == item.DyedCode && x.SalesOrderID == item.SalesOrderID).ToList(), sv.BatchNo, ColorHeader));
                    }
                }

                PurchaseDetailList = sv.purchaseDetailList;
                ServiceList = sv.Services;
            };
            Client.GetEstimatedDyeingListCompleted += (s, sv) =>
            {
                HeaderViewModel.DyeingViewModelList = new ObservableCollection<TblDyeingPlanViewModel>();
                ColorHeader = new ObservableCollection<ColorHeader>();

                foreach (var item in sv.ColorsValues.Distinct())
                {
                    var test = new ColorHeader
                    {
                        BatchNo = sv.BatchNo,
                        ColorName = item.FabricColor
                    };

                    if (ColorHeader.All(x => x.ColorName != test.ColorName))
                    {
                        test.BatchNo = test.BatchNo;
                        ColorHeader.Add(test);
                    }
                }

                foreach (var item in sv.Result)
                {
                    HeaderViewModel.DyeingViewModelList.Add(DyeingPlanMapper.VmMapToDyeingPLanEstimated(item, sv.ColorsValues.Where(x => x.Fabric_Code == item.FabricCode && x.SalesOrderID == item.SalesOrderID).ToList(), sv.BatchNo, ColorHeader));
                }
            };

            Client.SeasonsCompleted += (d, s) =>
            {
                SeasonList = s.Result;
            };
            Client.GetAllBrandsCompleted += (a, b) =>
            {
                foreach (var item in b.Result)
                {
                    BrandsList.Add(item);
                }
            };
            Client.GetGenericCompleted += (s, sv) =>
            {
                DyeingTypeList = sv.Result;
            };
            Client.GetGenericAsync("TblDyeingType", "%%", "%%", "%%", "Iserial", "ASC");         

            Client.GetWarehouseswithOnHandCompleted += (l, c) =>
            {
                WarehouseWithOnHandList = c.Result;
            };

            Client.GetDyeingSummaryCompleted += (l, s) =>
                {
                    DyeingSummeryViewModelList.Clear();
                    foreach (var item in s.Result)
                    {
                        DyeingSummeryViewModelList.Add(DyeingPlanMapper.VmMapToDyeingSummary(item));
                    }
                    SummaryPagedCollection = new PagedCollectionView(DyeingSummeryViewModelList);
                    SummaryPagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("Batchno"));
                };
            HeaderViewModel.PropertyChanged += DyeingHeaderViewModel_PropertyChanged;

            Client.GetDyeingListCompleted += webService_GetDyeingListCompleted;

            Client.GetDyeingHeaderListCompleted += (h, ho) =>
            {
                HeaderViewModel.DyeingViewModelList = new ObservableCollection<TblDyeingPlanViewModel>();
                ColorHeader = new ObservableCollection<ColorHeader>();    
                foreach (var row in ho.Result)
                {
                    var newrow = new TblDyeingPlanHeaderViewModel();
                    newrow.InjectFrom(row);
                    if (row.TblLkpBrandSection1!=null)
                    {
                        newrow.BrandSectionPerRow = new LkpData.TblLkpBrandSection().InjectFrom(row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection;

                    }
                    newrow.SeasonPerRow = row.TblLkpSeason1;
                    if (row.TblDyeingType1 != null) newrow.DyeingTypePerRow.InjectFrom(row.TblDyeingType1);
                    DyeingPlanHeaderViewModelList.Add(newrow);
                }                
            };
        }

        internal void GetBillOfMaterials()
        {
            var season = SeasonList.FirstOrDefault(x => x.Iserial == HeaderViewModel.TblLkpSeason);

            if (HeaderViewModel.TblDyeingType == 1)
            {
                if (season != null)
                    if (HeaderViewModel.TblLkpBrandSection != null)
                        Client.GetEstimatedDyeingListAsync(HeaderViewModel.Brand, (int)HeaderViewModel.TblLkpBrandSection, season.Code, HeaderViewModel.FromDate, HeaderViewModel.ToDate);
            }
            else
            {
                Client.GetEstimatedDyeingListFromProductionAsync(HeaderViewModel.GeneratePurchaseHeaderPerRow.Iserial);
            }
        }

        private void webService_GetDyeingListCompleted(object sender, GetDyeingListCompletedEventArgs e)
        {
            foreach (var head in e.Result)
            {
                HeaderViewModel.DyeingViewModelList.Add(DyeingPlanMapper.VmMapToDyeingPLan(head));

                foreach (var item in head.TblDyeingPlanDetails.OrderBy(x => x.BatchNo))
                {
                    var test = new ColorHeader { BatchNo = item.BatchNo, ColorName = item.FabricColorName };

                    if (ColorHeader.All(x => x.ColorName != item.FabricColorName))
                    {
                        ColorHeader.Add(test);
                    }
                }
            }
        }

        public void SaveHeader()
        {
            var dyeingPlanlist = new ObservableCollection<TblDyeingPlan>();
            foreach (var item in HeaderViewModel.DyeingViewModelList)
            {
                var dyeingplanrow = new TblDyeingPlan();
                dyeingplanrow.InjectFrom(item);
                dyeingplanrow.TblDyeingPlanDetails = new ObservableCollection<TblDyeingPlanDetail>();
                GenericMapper.InjectFromObCollection(dyeingplanrow.TblDyeingPlanDetails, item.DetailsViewModel.Where(w=>w.BatchNoCreated!=0&& w.FabricColorName!=""));
                dyeingPlanlist.Add(dyeingplanrow);
            }

            var summaryList = new ObservableCollection<TblDyeingSummary>();
            foreach (var itemSummary in DyeingSummeryViewModelList)
            {
                summaryList.Add(DyeingPlanMapper.DbMapToDyeingSummary(itemSummary));
            }
            Client.SaveDyeingHeaderListAsync(new TblDyeingPlanHeader().InjectFrom(HeaderViewModel) as TblDyeingPlanHeader
              , dyeingPlanlist, summaryList);
        }

        public void SaveServices(int iserial)
        {
            foreach (var row in DyeingSummaryServices)
            {
                Client.SaveSummaryServicesAsync(iserial, row.ServiceCode, row.ServicEname, row.Notes, row.ItemChecked, row.Qty);
            }
        }

        public void DyeingServicesChildWindow(DyeingSummeryViewModel dyeingSummeryRow)
        {
            Client.GetSavedSummaryServicesAsync(dyeingSummeryRow.Iserial);
            var childServices = new DyeingPLanServicesChildWindows(this, dyeingSummeryRow.Iserial);
            childServices.Show();
        }

        private void DyeingHeaderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Location")
            {
                Client.GetLocationDetailsAsync(HeaderViewModel.Location);
            }
            if (e.PropertyName == "Brand")
            {
                lkpClient.GetTblBrandSectionLinkAsync(HeaderViewModel.Brand, LoggedUserInfo.Iserial);
            }
        }

        internal void GetDyeingPlanOrder()
        {
            Client.GetDyeingListAsync(HeaderViewModel.Iserial);
        }

        public void GenerateDyeingOrder()
        {
            Client.GenerateDyeingOrdersAsync(HeaderViewModel.Iserial);
        }

        public void GenerateSummary()
        {
            if (HeaderViewModel.Iserial != 0)
            {
                Client.GetDyeingSummaryAsync(HeaderViewModel.Iserial);
                return;
            }
            if (DyeingSummeryViewModelList.Select(x => x.DyeingHeader).FirstOrDefault() == 0)
            {
                DyeingSummeryViewModelList = new ObservableCollection<DyeingSummeryViewModel>();
                var dyeingSummaryTemp = new ObservableCollection<DyeingSummeryViewModel>();

                var firstOrDefault = ColorHeader.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var counter = firstOrDefault.BatchNo;

                    foreach (var color in ColorHeader)
                    {
                        counter++;
                        var intialDyeingClass = 0;
                        foreach (var planRow in HeaderViewModel.DyeingViewModelList.OrderBy(x => x.DyeingClass))
                        {
                            if (planRow.DetailsViewModel.Count(x => x.FabricColorName == color.ColorName) != 0)
                            {
                                var dyeingClass = planRow.DyeingClass;
                                if (intialDyeingClass != 0 && intialDyeingClass != dyeingClass)
                                {
                                    counter++;
                                }

                                var colorRow = planRow.DetailsViewModel.FirstOrDefault(x => x.FabricColorName == color.ColorName);

                                dyeingSummaryTemp.Add(new DyeingSummeryViewModel
                                {
                                    Batchno = counter,
                                    CalculatedTotalQty = colorRow.FabricColorValue,
                                    ColorName = colorRow.FabricColorName,
                                    DyedFabric = planRow.DyedFabric,
                                    DyeingClass = planRow.DyeingClass,
                                    DyeingHeader = planRow.DyeingHeader,
                                    Iserial = planRow.Iserial,
                                    FabricCode = planRow.FabricCode,
                                    Unit = planRow.Unit,
                                });
                                intialDyeingClass = planRow.DyeingClass;
                            }
                        }
                    }
                }               

                var s = from d in dyeingSummaryTemp
                        group d by new
                        {
                            d.Batchno,
                            d.ColorName,
                            d.DyedFabric,
                            d.DyeingClass,
                            d.FabricCode,
                            d.Unit,
                        }
                            into g
                            select new DyeingSummeryViewModel
                            {
                                Batchno = g.Key.Batchno,
                                CalculatedTotalQty = g.Sum(x => x.CalculatedTotalQty),
                                ColorName = g.Key.ColorName,
                                DyedFabric = g.Key.DyedFabric,
                                DyeingClass = g.Key.DyeingClass,
                                FabricCode = g.Key.FabricCode,
                                Vendor = DefaultDyeingVendor,
                                Unit = g.Key.Unit,
                            };

                foreach (var items in s)
                {
                    items.LotsMasterList.Add(new TblDyeingPlanLotsMasterViewModel

                    {
                        FabricCode = items.FabricCode,
                        Unit = items.Unit,
                        Config = items.ColorName,
                        DyeingsSummaryPlanIserial = items.Iserial,
                        RequiredQuantity = items.CalculatedTotalQty,
                        FabricStoragePerFabricList = FabricStorageList.Where(x => x.itemid == items.FabricCode
                            //&& x.CONFIGID == items.ColorName
                            ).ToList(),
                        BatchNo = items.Batchno,
                    });

                    DyeingSummeryViewModelList.Add(items);
                }
                SummaryPagedCollection = new PagedCollectionView(DyeingSummeryViewModelList);
                SummaryPagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("Batchno"));
                foreach (var summaryRow in DyeingSummeryViewModelList)
                {
                    var ad = HeaderViewModel.DyeingViewModelList.Where(x => x.FabricCode == summaryRow.FabricCode);

                    foreach (var det in ad)
                    {
                        try
                        {
                            var tblDyeingPlanDetailViewModel = det.DetailsViewModel.SingleOrDefault(r => r.FabricColorName == summaryRow.ColorName);
                            if (tblDyeingPlanDetailViewModel !=
                                null)
                                tblDyeingPlanDetailViewModel.BatchNoCreated = summaryRow.Batchno;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                foreach (var item in DyeingSummeryViewModelList)
                {
                    foreach (var masterLotRow in item.LotsMasterList)
                    {
                        if (masterLotRow.FabricStoragePerFabricList.Count() == 1)
                        {
                            var fabricStorageResult = masterLotRow.FabricStoragePerFabricList.SingleOrDefault();
                            if (fabricStorageResult != null)
                            {
                                masterLotRow.FromColor = fabricStorageResult.CONFIGID;
                                masterLotRow.FabricLot = fabricStorageResult.INVENTBATCHID;
                                masterLotRow.AvaliableQuantity = Convert.ToSingle(fabricStorageResult.QuantityPerMeter);
                                masterLotRow.AvaliableQuantityOrg = Convert.ToSingle(fabricStorageResult.QuantityPerMeter);
                            }
                        }
                    }
                }
            }
        }

        public void GenerateFabricLots()
        {
            var childWindow = new DyeingPlanFabricLotMasterChildWindow(this);
            childWindow.Show();
        }

        internal void SearchHeader()
        {
            if (SortBy == null)
            {
                SortBy = "it.Iserial";
            }
            
            Client.GetDyeingHeaderListAsync(DyeingPlanHeaderViewModelList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        internal void SearchForFabrics(string fabricToSearch)
        {
            Client.GetFabricWithUnitAndDyeingClassAsync(fabricToSearch);
        }

        internal void DeletePlan()
        {
            Client.DeleteDyeingPlanHeaderAsync(HeaderViewModel.Iserial);
        }
    }

    #region ViewModels

    public class ColorHeader : PropertiesViewModelBase
    {
        private string _colorName;

        public string ColorName
        {
            get
            {
                return _colorName;
            }
            set
            {
                if ((ReferenceEquals(_colorName, value) != true))
                {
                    _colorName = value;
                    RaisePropertyChanged("ColorName");
                }
            }
        }

        private int _batchNo;

        public int BatchNo
        {
            get
            {
                return _batchNo;
            }
            set
            {
                if ((Equals(_batchNo, value) != true))
                {
                    _batchNo = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }
    }

    public class DyeingSummeryViewModel : PropertiesViewModelBase
    {
        public DyeingSummeryViewModel()
        {
            LotsMasterList = new ObservableCollection<TblDyeingPlanLotsMasterViewModel>();
        
        }        
        private bool _valid;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; RaisePropertyChanged("Valid"); }
        }

        private ObservableCollection<TblDyeingPlanLotsMasterViewModel> _lotsMasterList;

        public ObservableCollection<TblDyeingPlanLotsMasterViewModel> LotsMasterList
        {
            get
            {
                return _lotsMasterList;
            }
            set
            {
                if ((ReferenceEquals(_lotsMasterList, value) != true))
                {
                    _lotsMasterList = value;
                    RaisePropertyChanged("LotsMasterList");
                }
            }
        }

        private string _size;

        [ReadOnly(true)]
        public string Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("Size"); }
        }

      
        
        private int _batchno;

        [ReadOnly(true)]
        public int Batchno
        {
            get
            {
                return _batchno;
            }
            set
            {
                if ((_batchno.Equals(value) != true))
                {
                    _batchno = value;
                    RaisePropertyChanged("Batchno");
                }
            }
        }

        private string _colorName;

        [ReadOnly(true)]
        public string ColorName
        {
            get
            {
                return _colorName;
            }
            set
            {
                if ((ReferenceEquals(_colorName, value) != true))
                {
                    _colorName = value;
                    RaisePropertyChanged("ColorName");
                }
            }
        }

        private string _vendor;

        public string Vendor
        {
            get
            {
                return _vendor;
            }
            set
            {
                if ((ReferenceEquals(_vendor, value) != true))
                {
                    _vendor = value;
                    RaisePropertyChanged("Vendor");
                }
            }
        }

        private Vendor _vendorPerRow;

        public Vendor VendorPerRow
        {
            get { return _vendorPerRow; }
            set
            {
                _vendorPerRow = value; RaisePropertyChanged("VendorPerRow");
                if (value != null) Vendor = value.vendor_code;
            }
        }

        private string _dyedFabric;

        [ReadOnly(true)]
        public string DyedFabric
        {
            get
            {
                return _dyedFabric;
            }
            set
            {
                if ((ReferenceEquals(_dyedFabric, value) != true))
                {
                    _dyedFabric = value;
                    RaisePropertyChanged("DyedFabric");
                }
            }
        }

        private double _calculatedTotalQtyField;

        private int _dyeingClassField;

        private int _dyeingHeaderField;

        private string _fabricCodeField;

        private int _iserialField;

        private string _unitField;

        public double CalculatedTotalQty
        {
            get
            {
                return _calculatedTotalQtyField;
            }
            set
            {
                if ((Equals(_calculatedTotalQtyField, value) != true))
                {
                    _calculatedTotalQtyField = value;
                    RaisePropertyChanged("CalculatedTotalQty");
                }
            }
        }

        [ReadOnly(true)]
        public int DyeingClass
        {
            get
            {
                return _dyeingClassField;
            }
            set
            {
                if ((Equals(_dyeingClassField, value) != true))
                {
                    _dyeingClassField = value;
                    RaisePropertyChanged("DyeingClass");
                }
            }
        }

        public int DyeingHeader
        {
            get
            {
                return _dyeingHeaderField;
            }
            set
            {
                if ((_dyeingHeaderField.Equals(value) != true))
                {
                    _dyeingHeaderField = value;
                    RaisePropertyChanged("DyeingHeader");
                }
            }
        }

        [ReadOnly(true)]
        public string FabricCode
        {
            get
            {
                return _fabricCodeField;
            }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [ReadOnly(true)]
        public string Unit
        {
            get
            {
                return _unitField;
            }
            set
            {
                if ((ReferenceEquals(_unitField, value) != true))
                {
                    _unitField = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }
    }

    public class TblDyeingPlanViewModel : PropertiesViewModelBase
    {
        private ObservableCollection<TblDyeingPlanDetailViewModel> _detailsViewModel;

        private double _calculatedTotalQtyField;

        private int _dyeingClassField;

        private int _dyeingHeaderField;

        private string _fabricCodeField;

        private int _iserialField;

        private string _styleField;

        private string _unitField;

        private string _dyedFabric;

        private string _salesOrder;

        private tbl_FabricAttriputes _fabricAttSelectedItem;

        public tbl_FabricAttriputes FabricAttSelectedItem
        {
            get { return _fabricAttSelectedItem; }
            set { _fabricAttSelectedItem = value; RaisePropertyChanged("FabricAttSelectedItem"); }
        }

        public ObservableCollection<TblDyeingPlanDetailViewModel> DetailsViewModel
        {
            get
            {
                return _detailsViewModel;
            }
            set
            {
                if ((ReferenceEquals(_detailsViewModel, value) != true))
                {
                    _detailsViewModel = value;
                    RaisePropertyChanged("DetailsViewModel");
                }
            }
        }

        public string SalesOrder
        {
            get { return _salesOrder ?? (_salesOrder = "N/A"); }
            set
            {
                if ((ReferenceEquals(_salesOrder, value) != true))
                {
                    _salesOrder = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }

        public string DyedFabric
        {
            get
            {
                return _dyedFabric;
            }
            set
            {
                if ((ReferenceEquals(_dyedFabric, value) != true))
                {
                    _dyedFabric = value;
                    RaisePropertyChanged("DyedFabric");
                }
            }
        }

        public double CalculatedTotalQty
        {
            get
            {
                return _calculatedTotalQtyField;
            }
            set
            {
                if ((_calculatedTotalQtyField.Equals(value) != true))
                {
                    _calculatedTotalQtyField = value;
                    RaisePropertyChanged("CalculatedTotalQty");
                }
            }
        }

        public int DyeingClass
        {
            get
            {
                return _dyeingClassField;
            }
            set
            {
                if ((Equals(_dyeingClassField, value) != true))
                {
                    _dyeingClassField = value;
                    RaisePropertyChanged("DyeingClass");
                }
            }
        }

        public int DyeingHeader
        {
            get
            {
                return _dyeingHeaderField;
            }
            set
            {
                if ((_dyeingHeaderField.Equals(value) != true))
                {
                    _dyeingHeaderField = value;
                    RaisePropertyChanged("DyeingHeader");
                }
            }
        }

        public string FabricCode
        {
            get
            {
                return _fabricCodeField;
            }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string Style
        {
            get { return _styleField ?? (_styleField = "N/A"); }
            set
            {
                if ((ReferenceEquals(_styleField, value) != true))
                {
                    _styleField = value;
                    RaisePropertyChanged("Style");
                }
            }
        }

        public string Unit
        {
            get
            {
                return _unitField;
            }
            set
            {
                if ((ReferenceEquals(_unitField, value) != true))
                {
                    _unitField = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }
    }

    public class TblDyeingPlanDetailViewModel : PropertiesViewModelBase
    {
        private int _batchNoField;

        private string _fabricColorNameField;

        private double _fabricColorValueField;

        private int _iserialField;

        private int _iserialHeaderField;

        public int BatchNo
        {
            get
            {
                return _batchNoField;
            }
            set
            {
                if ((_batchNoField.Equals(value) != true))
                {
                    _batchNoField = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }

        public string FabricColorName
        {
            get
            {
                return _fabricColorNameField;
            }
            set
            {
                if ((ReferenceEquals(_fabricColorNameField, value) != true))
                {
                    _fabricColorNameField = value;
                    RaisePropertyChanged("FabricColorName");
                }
            }
        }

        public double FabricColorValue
        {
            get
            {
                return _fabricColorValueField;
            }
            set
            {
                if ((_fabricColorValueField.Equals(value) != true))
                {
                    _fabricColorValueField = value;
                    RaisePropertyChanged("FabricColorValue");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int IserialHeader
        {
            get
            {
                return _iserialHeaderField;
            }
            set
            {
                if ((_iserialHeaderField.Equals(value) != true))
                {
                    _iserialHeaderField = value;
                    RaisePropertyChanged("IserialHeader");
                }
            }
        }

        private int _batchNoCreated;

        public int BatchNoCreated
        {
            get
            {
                return _batchNoCreated;
            }
            set
            {
                if ((_batchNoCreated.Equals(value) != true))
                {
                    _batchNoCreated = value;
                    RaisePropertyChanged("BatchNoCreated");
                }
            }
        }

        private string _salesOrder;

        public string SalesOrder
        {
            get
            {
                return _salesOrder;
            }
            set
            {
                if ((ReferenceEquals(_salesOrder, value) != true))
                {
                    _salesOrder = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }

        private string _size;

        [ReadOnly(true)]
        public string Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("Size"); }
        }

        private string _oldColor;

        public string OldColor
        {
            get { return _oldColor; }
            set { _oldColor = value; RaisePropertyChanged("OldColor"); }
        }
    }

    public class TblDyeingPlanHeaderViewModel : PropertiesViewModelBase
    {
        private int? _tblDyeingType;

        public int? TblDyeingType
        {
            get { return _tblDyeingType; }
            set { _tblDyeingType = value; RaisePropertyChanged("TblDyeingType"); }
        }

        private int? _tblGeneratePurchaseHeader;

        public int? TblGeneratePurchaseHeader
        {
            get { return _tblGeneratePurchaseHeader; }
            set { _tblGeneratePurchaseHeader = value; RaisePropertyChanged("TblGeneratePurchaseHeader"); }
        }

        private PurchasePlanService.TblGeneratePurchaseHeader _generatePurchaseHeaderPerRow;

        public PurchasePlanService.TblGeneratePurchaseHeader GeneratePurchaseHeaderPerRow
        {
            get { return _generatePurchaseHeaderPerRow ?? (_generatePurchaseHeaderPerRow = new PurchasePlanService.TblGeneratePurchaseHeader()); }
            set
            {
                _generatePurchaseHeaderPerRow = value; RaisePropertyChanged("GeneratePurchaseHeaderPerRow");
                TblGeneratePurchaseHeader = GeneratePurchaseHeaderPerRow.Iserial;
                DocNo = GeneratePurchaseHeaderPerRow.Code;
            }
        }

        private GenericTable _dyeingTypePerRow;

        public GenericTable DyeingTypePerRow
        {
            get { return _dyeingTypePerRow ?? (_dyeingTypePerRow = new GenericTable()); }
            set { _dyeingTypePerRow = value; RaisePropertyChanged("DyeingTypePerRow"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if ((_enabled.Equals(value) != true))
                {
                    _enabled = value;
                    RaisePropertyChanged("Enabled");
                }
            }
        }

        private string _docNoField;

        private int _iserialField;

        private string _locationField;

        public string DocNo
        {
            get
            {
                return _docNoField;
            }
            set
            {
                if ((ReferenceEquals(_docNoField, value) != true))
                {
                    _docNoField = value;
                    RaisePropertyChanged("DocNo");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string Location
        {
            get
            {
                return _locationField;
            }
            set
            {
                if ((ReferenceEquals(_locationField, value) != true))
                {
                    _locationField = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        private TblLkpSeason _seasonPerRow;

        public TblLkpSeason SeasonPerRow
        {
            get { return _seasonPerRow; }
            set { _seasonPerRow = value; RaisePropertyChanged("SeasonPerRow"); }
        }

        private LkpData.TblLkpBrandSection _brandSection;

        public LkpData.TblLkpBrandSection BrandSectionPerRow
        {
            get { return _brandSection; }
            set { _brandSection = value; RaisePropertyChanged("BrandSectionPerRow"); }
        }

        private int? _tblLkpSeason;

        public int? TblLkpSeason
        {
            get
            {
                return _tblLkpSeason;
            }
            set
            {
                if ((Equals(_tblLkpSeason, value) != true))
                {
                    _tblLkpSeason = value;
                    RaisePropertyChanged("TblLkpSeason");
                }
            }
        }

        private string _brand;

        public string Brand
        {
            get
            {
                return _brand;
            }
            set
            {
                if ((ReferenceEquals(_brand, value) != true))
                {
                    _brand = value;
                    RaisePropertyChanged("Brand");
                }
            }
        }

        private int? _tblLkpBrandSection;

        public int? TblLkpBrandSection
        {
            get { return _tblLkpBrandSection; }
            set { _tblLkpBrandSection = value; RaisePropertyChanged("TblLkpBrandSection"); }
        }

        private DateTime? _fromDate;

        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                if ((ReferenceEquals(_fromDate, value) != true))
                {
                    _fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        private DateTime? _toDate;

        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                if ((ReferenceEquals(_toDate, value) != true))
                {
                    _toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        private DateTime? _transDate;

        public DateTime? TransactionDate
        {
            get
            {
                return _transDate;
            }
            set
            {
                if ((ReferenceEquals(_transDate, value) != true))
                {
                    _transDate = value;
                    RaisePropertyChanged("TransactionDate");
                }
            }
        }

        private bool _generated;

        public bool PlanGenerated
        {
            get
            {
                return _generated;
            }
            set
            {
                if ((_generated == value != true))
                {
                    _generated = value;
                    RaisePropertyChanged("PlanGenerated");
                }
            }
        }

        private ObservableCollection<TblDyeingPlanViewModel> _tblDyeingPlanViewModel;

        public ObservableCollection<TblDyeingPlanViewModel> DyeingViewModelList
        {
            get
            {
                return _tblDyeingPlanViewModel ?? (_tblDyeingPlanViewModel = new ObservableCollection<TblDyeingPlanViewModel>());
            }
            set
            {
                if ((ReferenceEquals(_tblDyeingPlanViewModel, value) != true))
                {
                    _tblDyeingPlanViewModel = value;
                    RaisePropertyChanged("DyeingViewModelList");
                }
            }
        }
    }

    public class DyeingSummaryServicesViewModel : PropertiesViewModelBase
    {
        private int _summaryRowIserial;

        public int SummaryRowIserial
        {
            get
            {
                return _summaryRowIserial;
            }
            set
            {
                if ((_summaryRowIserial.Equals(value) != true))
                {
                    _summaryRowIserial = value;
                    RaisePropertyChanged("SummaryRowIserial");
                }
            }
        }

        private string _notesField;

        private string _serviceCodeField;

        private string _servicEnameField;

        private int _iserialField;

        private bool _itemChecked;

        public bool ItemChecked
        {
            get
            {
                return _itemChecked;
            }
            set
            {
                if ((_itemChecked == value != true))
                {
                    _itemChecked = value;
                    RaisePropertyChanged("ItemChecked");
                }
            }
        }

        public string Notes
        {
            get
            {
                return _notesField;
            }
            set
            {
                if ((ReferenceEquals(_notesField, value) != true))
                {
                    _notesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public string ServiceCode
        {
            get
            {
                return _serviceCodeField;
            }
            set
            {
                if ((ReferenceEquals(_serviceCodeField, value) != true))
                {
                    _serviceCodeField = value;
                    RaisePropertyChanged("ServiceCode");
                }
            }
        }

        public string ServicEname
        {
            get
            {
                return _servicEnameField;
            }
            set
            {
                if ((ReferenceEquals(_servicEnameField, value) != true))
                {
                    _servicEnameField = value;
                    RaisePropertyChanged("ServicEname");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        private double _qty;

        public double Qty
        {
            get { return _qty; }
            set { _qty = value; RaisePropertyChanged("Qty"); }
        }
    }

    public class TblDyeingPlanLotsDetailsViewModel : PropertiesViewModelBase
    {
        private DateTime _deliveryDateField;

        private int _fabricLotMasterIserialField;

        private int _iserialField;

        private double _requiredQuantityField;

        private string _salesOrderField;

        private bool _savedField;

        public DateTime DeliveryDate
        {
            get
            {
                return _deliveryDateField;
            }
            set
            {
                if ((_deliveryDateField.Equals(value) != true))
                {
                    _deliveryDateField = value;
                    RaisePropertyChanged("DeliveryDate");
                }
            }
        }

        public int FabricLotMasterIserial
        {
            get
            {
                return _fabricLotMasterIserialField;
            }
            set
            {
                if ((_fabricLotMasterIserialField.Equals(value) != true))
                {
                    _fabricLotMasterIserialField = value;
                    RaisePropertyChanged("FabricLotMasterIserial");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public double RequiredQuantity
        {
            get
            {
                return _requiredQuantityField;
            }
            set
            {
                if ((_requiredQuantityField.Equals(value) != true))
                {
                    _requiredQuantityField = value;
                    RaisePropertyChanged("RequiredQuantity");
                }
            }
        }

        public string SalesOrder
        {
            get
            {
                return _salesOrderField;
            }
            set
            {
                if ((ReferenceEquals(_salesOrderField, value) != true))
                {
                    _salesOrderField = value;
                    RaisePropertyChanged("SalesOrder");
                }
            }
        }

        public bool Saved
        {
            get
            {
                return _savedField;
            }
            set
            {
                if ((_savedField.Equals(value) != true))
                {
                    _savedField = value;
                    RaisePropertyChanged("Saved");
                }
            }
        }

        private string _unit;

        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                if ((ReferenceEquals(_unit, value) != true))
                {
                    _unit = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }

        private double _avaliableQuantityField;

        [ReadOnly(true)]
        public double AvaliableQuantity
        {
            get
            {
                return _avaliableQuantityField;
            }
            set
            {
                if ((_avaliableQuantityField.Equals(value) != true))
                {
                    _avaliableQuantityField = value;
                    RaisePropertyChanged("AvaliableQuantity");
                }
            }
        }
    }

    public class TblDyeingPlanLotsMasterViewModel : PropertiesViewModelBase
    {
        public TblDyeingPlanLotsMasterViewModel()
        {
            FabricStoragePerFabricList = new List<FabricStorage_Result>();
            LotsDetailsList = new ObservableCollection<TblDyeingPlanLotsDetailsViewModel>();
        }

        private string _config;

        [ReadOnly(true)]
        public string Config
        {
            get
            {
                return _config;
            }
            set
            {
                if ((ReferenceEquals(_config, value) != true))
                {
                    _config = value;
                    RaisePropertyChanged("Config");
                }
            }
        }

        private string _fromColor;

        public string FromColor
        {
            get { return _fromColor; }
            set { _fromColor = value; RaisePropertyChanged("FromColor"); }
        }

        private List<FabricStorage_Result> _fabricStoragePerFabricList;

        public List<FabricStorage_Result> FabricStoragePerFabricList
        {
            get
            {
                return _fabricStoragePerFabricList;
            }
            set
            {
                if ((ReferenceEquals(_fabricStoragePerFabricList, value) != true))
                {
                    _fabricStoragePerFabricList = value;
                    RaisePropertyChanged("FabricStoragePerFabricList");
                }
            }
        }

        private FabricStorage_Result _fabricStoragePerRow;

        public FabricStorage_Result FabricStoragePerRow
        {
            get { return _fabricStoragePerRow; }
            set
            {
                _fabricStoragePerRow = value; RaisePropertyChanged("FabricStoragePerRow");
                if (FabricStoragePerRow != null)
                {
                    if (FabricStoragePerRow.QuantityPerMeter != null)
                        AvaliableQuantityOrg = (double)FabricStoragePerRow.QuantityPerMeter;
                }
            }
        }

        private ObservableCollection<TblDyeingPlanLotsDetailsViewModel> _lotsDetailsList;

        public ObservableCollection<TblDyeingPlanLotsDetailsViewModel> LotsDetailsList
        {
            get
            {
                return _lotsDetailsList;
            }
            set
            {
                if ((ReferenceEquals(_lotsDetailsList, value) != true))
                {
                    _lotsDetailsList = value;

                    RaisePropertyChanged("LotsDetailsList");
                }
            }
        }

        private double _avaliableQuantityField;

        [ReadOnly(true)]
        public double AvaliableQuantity
        {
            get
            {
                return _avaliableQuantityField;
            }
            set
            {
                if ((_avaliableQuantityField.Equals(value) != true))
                {
                    _avaliableQuantityField = value;
                    RaisePropertyChanged("AvaliableQuantity");
                }
            }
        }

        private double _avaliableQuantityOrgField;

        [ReadOnly(true)]
        public double AvaliableQuantityOrg
        {
            get
            {
                return _avaliableQuantityOrgField;
            }
            set
            {
                if ((_avaliableQuantityOrgField.Equals(value) != true))
                {
                    _avaliableQuantityOrgField = value;
                    RaisePropertyChanged("AvaliableQuantityOrg");
                }
            }
        }

        private int _dyeingPlanIserialField;

        private string _fabricCodeField;

        private string _fabricLotField;

        private int _iserialField;

        private double _requiredQuantityField;

        public int DyeingsSummaryPlanIserial
        {
            get
            {
                return _dyeingPlanIserialField;
            }
            set
            {
                if ((_dyeingPlanIserialField.Equals(value) != true))
                {
                    _dyeingPlanIserialField = value;
                    RaisePropertyChanged("DyeingsSummaryPlanIserial");
                }
            }
        }

        public string FabricCode
        {
            get
            {
                return _fabricCodeField;
            }
            set
            {
                if ((ReferenceEquals(_fabricCodeField, value) != true))
                {
                    _fabricCodeField = value;
                    RaisePropertyChanged("FabricCode");
                }
            }
        }

        public string FabricLot
        {
            get
            {
                return _fabricLotField;
            }
            set
            {
                if ((ReferenceEquals(_fabricLotField, value) != true))
                {
                    _fabricLotField = value;
                    RaisePropertyChanged("FabricLot");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public double RequiredQuantity
        {
            get
            {
                return _requiredQuantityField;
            }
            set
            {
                if ((_requiredQuantityField.Equals(value) != true))
                {
                    _requiredQuantityField = value;
                    RaisePropertyChanged("RequiredQuantity");
                }
            }
        }

        private string _unitField;

        [ReadOnly(true)]
        public string Unit
        {
            get
            {
                return _unitField;
            }
            set
            {
                if ((ReferenceEquals(_unitField, value) != true))
                {
                    _unitField = value;
                    RaisePropertyChanged("Unit");
                }
            }
        }

        private int _batchNo;

        [ReadOnly(true)]
        public int BatchNo
        {
            get
            {
                return _batchNo;
            }
            set
            {
                if ((Equals(_batchNo, value) != true))
                {
                    _batchNo = value;
                    RaisePropertyChanged("BatchNo");
                }
            }
        }
    }

    #endregion ViewModels
}