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
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class DyeingPlanAccViewModel : ViewModelBase
    {
        #region Properties

        public string DefaultDyeingVendor { get; set; }

        public string DefaultFinishedFabricWarehouse { get; set; }

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

        private ObservableCollection<LkpData.TblLkpBrandSection> _brandSectionList;

        public ObservableCollection<LkpData.TblLkpBrandSection> BrandSectionList
        {
            get { return _brandSectionList ?? (_brandSectionList = new ObservableCollection<LkpData.TblLkpBrandSection>()); }
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

        public void CalcfabricAgain(string fabric, string newColor = null, string oldColor = null)
        {
            //var summaryRows = DyeingSummeryViewModelList.Where(l => l.FabricCode == fabric);
           

            //if (summaryRows.Any())
            //{
            //    var requiredQty = summaryRows.Sum(x => x.CalculatedTotalQty);
            //    double orgQty = 0;

            //    orgQty = summaryRows.FirstOrDefault().LotsMasterList.FirstOrDefault().AvaliableQuantityOrg;
               

            //    foreach (var rows in summaryRows)
            //    {
            //        rows.LotsMasterList.FirstOrDefault().AvaliableQuantity = orgQty - requiredQty;
            //    }
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

        private ObservableCollection<FabricsDyedGroup> _fabricList;

        public ObservableCollection<FabricsDyedGroup> FabricList
        {
            get
            {
                return _fabricList;
            }
            set
            {
                if ((ReferenceEquals(_fabricList, value) != true))
                {
                    _fabricList = value;
                    RaisePropertyChanged("FabricList");
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

        private ObservableCollection<Vendor> _vendor;

        public ObservableCollection<Vendor> VendorList
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
                    RaisePropertyChanged("VendorList");
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

        private readonly CRUD_ManagerServiceClient _webService = new CRUD_ManagerServiceClient();
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();

        public DyeingPlanAccViewModel()
        {
            DyeingSummeryViewModelList = new ObservableCollection<DyeingSummeryViewModel>();
            FabricStorageList = new ObservableCollection<FabricStorage_Result>();

            lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
            {
                BrandSectionList.Clear();
                foreach (var row in sv.Result)
                {
                    BrandSectionList.Add(row.TblLkpBrandSection1);
                    if (row.TblLkpBrandSection == HeaderViewModel.TblLkpBrandSection)
                    {
                        HeaderViewModel.BrandSectionPerRow = row.TblLkpBrandSection1;
                    }
                }
                //HeaderViewModel.BrandSectionPerRow =
                //    BrandSectionList.Where(
                //        x =>
                //            x.TblLkpBrandSectionLinks.FirstOrDefault(
                //                w => w.TblLkpBrandSection == HeaderViewModel.TblLkpBrandSection)).;
            };

            Client.GetBomfabricDyeingServiceCompleted += (s, sv) =>
            {
            };
            _webService.GetFabricWithUnitAndDyeingClassCompleted += (d, sv) =>
            {
                FabricAttList = sv.Result;
            };
            ChaingSetupMethod.GetSettings("Dyeing Order acc", _webService);

            _webService.GetChainSetupCompleted += (a, s) =>
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
            _webService.GetTblWarehouseAsync(0,int.MaxValue,"it.Iserial",null,null);
            _webService.GetTblWarehouseCompleted += (s, w) =>
            {
                foreach (var item in w.Result)
                {
                    WareHouseList.Add(item);
                }
            };

            _webService.GetLocationDetailsCompleted += (s, sv) =>
            {
                FabricStorageList = sv.Result;
            };

            BrandsList = new ObservableCollection<Brand>();
            SeasonList = new ObservableCollection<TblLkpSeason>();
            FabricList = new ObservableCollection<FabricsDyedGroup>();
            VendorList = new ObservableCollection<Vendor>();
            HeaderViewModel = new TblDyeingPlanHeaderViewModel();

            SelectedRow = new TblDyeingPlanViewModel();
            DyeingAxServiceSummaryList = new ObservableCollection<DyeingAxService>();
            HeaderViewModel.FromDate = null;
            HeaderViewModel.ToDate = null;
            HeaderViewModel.TransactionDate = DateTime.Now;
            _webService.GetAllBrandsAsync(LoggedUserInfo.Iserial);
            _webService.SeasonsAsync();
            _webService.GetWarehouseswithOnHandAsync();
            _webService.GetAxSummaryServicesAsync();

            _webService.GetAxSummaryServicesCompleted += (d, s) =>
            {
                foreach (var item in s.Result)
                {
                    DyeingAxServiceSummaryList.Add(item);
                }
            };

            //_webService.GetSavedSummaryServicesCompleted += (d, e) =>
            //{
            //    DyeingSummaryServices = new ObservableCollection<DyeingSummaryServicesViewModel>();
            //    foreach (var item in DyeingAxServiceSummaryList)
            //    {
            //        DyeingSummaryServices.Add(new DyeingSummaryServicesViewModel
            //        {
            //            ServiceCode = item.ServiceCode,
            //            ServicEname = item.ServiceName
            //        });
            //    }

            //    foreach (var row in e.Result)
            //    {
            //        var serviceRow = DyeingSummaryServices.SingleOrDefault(x => x.ServiceCode == row.ServiceCode);
            //        if (serviceRow != null)
            //        {
            //            serviceRow.SummaryRowIserial = row.SummaryRowIserial;
            //            serviceRow.Notes = row.Notes;
            //            serviceRow.ItemChecked = true;
            //        }
            //    }
            //};

            _webService.GetEstimatedDyeingListAccCompleted += (s, sv) =>
            {
                HeaderViewModel.DyeingViewModelList = new ObservableCollection<TblDyeingPlanViewModel>();
                ColorHeader = new ObservableCollection<ColorHeader>();

                foreach (var item in sv.ColorsValues.Distinct())
                {
                    var test = new ColorHeader
                    {
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
                    HeaderViewModel.DyeingViewModelList.Add(DyeingPlanMapper.VmMapToDyeingPLanEstimated(item, sv.ColorsValues.Where(x => x.Fabric_Code == item.FabricCode && x.SalesOrderID == item.SalesOrderID).ToList(), 0, ColorHeader));
                }

                foreach (var variable in HeaderViewModel.DyeingViewModelList)
                {
                    variable.DyedFabric = variable.FabricCode + "Dy";
                }
            };
            _webService.GetVendorsCompleted += (d, s) =>
            {
                foreach (var item in s.Result)
                {
                    VendorList.Add(item);
                }
            };
            _webService.SeasonsCompleted += (d, s) =>
            {
                SeasonList = s.Result;
            };
            _webService.GetAllBrandsCompleted += (a, b) =>
            {
                foreach (var item in b.Result)
                {
                    BrandsList.Add(item);
                }
            };

            //_webService.GetFabricDyedListCompleted += (r, f) =>
            //{
            //    foreach (var item in f.Result)
            //    {
            //        FabricList.Add(item);
            //    }
            //};

            _webService.GetWarehouseswithOnHandCompleted += (l, c) =>
            {
                WarehouseWithOnHandList = c.Result;
            };

            _webService.GetDyeingSummaryAccCompleted += (l, s) =>
                {
                    DyeingSummeryViewModelList.Clear();
                    foreach (var item in s.Result)
                    {
                        DyeingSummeryViewModelList.Add(DyeingPlanMapper.VmMapToDyeingSummary(new TblDyeingSummary().InjectFrom(item) as TblDyeingSummary));
                    }
                    SummaryPagedCollection = new PagedCollectionView(DyeingSummeryViewModelList);
                    SummaryPagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("FabricCode"));
                };
            HeaderViewModel.PropertyChanged += DyeingHeaderViewModel_PropertyChanged;

            _webService.GetDyeingListAccCompleted += webService_GetDyeingListCompleted;

            _webService.GetDyeingHeaderListAccCompleted += (h, ho) =>
            {
                HeaderViewModel.DyeingViewModelList = new ObservableCollection<TblDyeingPlanViewModel>();
                ColorHeader = new ObservableCollection<ColorHeader>();

                var childWindowSeach = new DyeingPlanSearchResults(this);
                foreach (var row in ho.Result)
                {
                    var newrow = new TblDyeingPlanHeaderViewModel();
                    newrow.InjectFrom(row);
                    newrow.BrandSectionPerRow = new LkpData.TblLkpBrandSection().InjectFrom( row.TblLkpBrandSection1) as LkpData.TblLkpBrandSection;
                    newrow.SeasonPerRow = row.TblLkpSeason1;
                    DyeingPlanHeaderViewModelList.Add(newrow);
                }

                childWindowSeach.Show();

                DyeingSummeryViewModelList.Clear();
            };
        }

        internal void GetBillOfMaterials()
        {
            var season = SeasonList.FirstOrDefault(x => x.Iserial == HeaderViewModel.TblLkpSeason);
            if (HeaderViewModel.TblLkpBrandSection != null)
                _webService.GetEstimatedDyeingListAccAsync(HeaderViewModel.Brand, (int)HeaderViewModel.TblLkpBrandSection, season.Code, HeaderViewModel.FromDate, HeaderViewModel.ToDate);
        }

        private void webService_GetDyeingListCompleted(object sender, GetDyeingListAccCompletedEventArgs e)
        {
            foreach (var head in e.Result)
            {
                HeaderViewModel.DyeingViewModelList.Add(DyeingPlanMapper.VmMapToDyeingPLan(new TblDyeingPlan().InjectFrom(head) as TblDyeingPlan));

                foreach (var item in head.TblDyeingPlanDetailsAccs)
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
            var dyeingPlanlist = new ObservableCollection<TblDyeingPlanACC>();
            foreach (var item in HeaderViewModel.DyeingViewModelList)
            {
                var dyeingplanrow = new TblDyeingPlanACC();
                dyeingplanrow.InjectFrom(item);
                dyeingplanrow.TblDyeingPlanDetailsAccs = new ObservableCollection<TblDyeingPlanDetailsAcc>();
                GenericMapper.InjectFromObCollection(dyeingplanrow.TblDyeingPlanDetailsAccs, item.DetailsViewModel);
                dyeingPlanlist.Add(dyeingplanrow);
            }

            var summaryList = new ObservableCollection<TblDyeingSummaryAcc>();
            GenericMapper.InjectFromObCollection(summaryList, DyeingSummeryViewModelList);
            foreach (var row in DyeingSummeryViewModelList)
            {
                var sumRow =
                    FabricStorageList.Where(
                        x => x.CONFIGID == row.ColorName && x.INVENTSITEID == row.Size && x.itemid == row.FabricCode).Sum(w => w.QuantityPerMeter);
                row.Valid = row.CalculatedTotalQty <= (double)sumRow.Value;
            }

            if (DyeingSummeryViewModelList.Count(x => x.Valid) != 0)
            {
                _webService.SaveDyeingHeaderListAccAsync(new TblDyeingPlanHeaderAcc().InjectFrom(HeaderViewModel) as TblDyeingPlanHeaderAcc
                  , dyeingPlanlist, summaryList);
            }
            else
            {
                MessageBox.Show("Some Of The Quantities Doesn't Exists In the Warehouse");
            }
        }

        private void DyeingHeaderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Location")
            {
                _webService.GetLocationDetailsAsync(HeaderViewModel.Location);
            }
            if (e.PropertyName == "Brand")
            {
                lkpClient.GetTblBrandSectionLinkAsync(HeaderViewModel.Brand, LoggedUserInfo.Iserial);
            }
        }

        internal void GetDyeingPlanOrder()
        {
            _webService.GetDyeingListAccAsync(HeaderViewModel.Iserial);
        }

        public void GenerateDyeingOrder()
        {
            _webService.GenerateDyeingOrdersAsync(HeaderViewModel.Iserial);
        }

        public void GenerateSummary()
        {
            if (HeaderViewModel.Iserial != 0)
            {
                _webService.GetDyeingSummaryAccAsync(HeaderViewModel.Iserial);
                return;
            }
            if (DyeingSummeryViewModelList.Select(x => x.DyeingHeader).FirstOrDefault() == 0)
            {
                DyeingSummeryViewModelList = new ObservableCollection<DyeingSummeryViewModel>();
                var dyeingSummaryTemp = new ObservableCollection<DyeingSummeryViewModel>();

                foreach (var color in ColorHeader)
                {
                    foreach (var planRow in HeaderViewModel.DyeingViewModelList)
                    {
                        if (planRow.DetailsViewModel.Count(x => x.FabricColorName == color.ColorName) != 0)
                        {
                            var colorRow = planRow.DetailsViewModel.FirstOrDefault(x => x.FabricColorName == color.ColorName);

                            dyeingSummaryTemp.Add(new DyeingSummeryViewModel
                            {
                                CalculatedTotalQty = colorRow.FabricColorValue,
                                ColorName = colorRow.FabricColorName,
                                DyedFabric = planRow.DyedFabric,
                                DyeingHeader = planRow.DyeingHeader,
                                Iserial = planRow.Iserial,
                                FabricCode = planRow.FabricCode,
                                Unit = planRow.Unit,
                                Size = colorRow.Size,
                            });
                        }
                    }
                }

                var s = from d in dyeingSummaryTemp
                        group d by new
                        {
                            d.ColorName,
                            d.DyedFabric,
                            d.Size,
                            d.FabricCode,
                            d.Unit,
                        }
                            into g
                            select new DyeingSummeryViewModel
                            {
                                CalculatedTotalQty = g.Sum(x => x.CalculatedTotalQty),
                                ColorName = g.Key.ColorName,
                                DyedFabric = g.Key.DyedFabric,
                                Size = g.Key.Size,
                                FabricCode = g.Key.FabricCode,
                                Vendor = DefaultDyeingVendor,
                                Unit = g.Key.Unit,
                            };
                foreach (var items in s)
                {
                    foreach (var lotsDetailsCreation in HeaderViewModel.DyeingViewModelList.Where(x => x.FabricCode == items.FabricCode))
                    {
                        var colorInSalesorder = lotsDetailsCreation.DetailsViewModel.FirstOrDefault(w => w.Size == items.Size);

                        var lotsdetail = new List<TblDyeingPlanLotsDetailsViewModel>();
                        if (colorInSalesorder != null)
                        {
                            //  MasterLotRow.RequiredQuantity
                            // double Req = 0;
                            lotsdetail.Add(new TblDyeingPlanLotsDetailsViewModel
                            {
                                SalesOrder = colorInSalesorder.SalesOrder,
                                RequiredQuantity = colorInSalesorder.FabricColorValue,
                                Saved = true

                                //   Req= Req+  ColorInSalesorder.FabricColorValue,
                            });
                        }

                        var row = new TblDyeingPlanLotsMasterViewModel();
                        row.FabricCode = items.FabricCode;
                        row.Unit = items.Unit;
                        row.Config = colorInSalesorder.OldColor;
                        row.DyeingsSummaryPlanIserial = items.Iserial;
                        row.RequiredQuantity = items.CalculatedTotalQty;
                        row.FabricStoragePerFabricList = FabricStorageList.Where(x => x.itemid == items.FabricCode
                                                                                      && x.CONFIGID == colorInSalesorder.OldColor &&
                                                                                      x.INVENTSIZEID == items.Size).ToList();
                        row.BatchNo = items.Batchno;
                        row.AvaliableQuantity = (double)FabricStorageList.Where(x => x.itemid == items.FabricCode
                                                                                     && x.CONFIGID == colorInSalesorder.OldColor &&
                                                                                     x.INVENTSIZEID == items.Size
                            ).Sum(x => x.QuantityPerMeter);
                        row.AvaliableQuantityOrg = (double)FabricStorageList.Where(x => x.itemid == items.FabricCode
                                                                                        &&
                                                                                        x.CONFIGID == colorInSalesorder.OldColor &&
                                                                                        x.INVENTSIZEID == items.Size
                            ).Sum(x => x.QuantityPerMeter);
                        row.LotsDetailsList = new ObservableCollection<TblDyeingPlanLotsDetailsViewModel>(lotsdetail);
                        items.LotsMasterList.Add(row);
                    }

                    DyeingSummeryViewModelList.Add(items);
                    CalcfabricAgain(items.FabricCode);
                }

                SummaryPagedCollection = new PagedCollectionView(DyeingSummeryViewModelList);
                SummaryPagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("FabricCode"));
            }
        }

        //public void GenerateFabricLots()
        //{
        //    var childWindow = new DyeingPlanFabricLotMasterChildWindow(this);
        //    childWindow.Show();
        //}

        internal void SearchHeader()
        {
            if (SortBy == null)
            {
                SortBy = "it.Iserial";
            }
            DyeingPlanHeaderViewModelList = new ObservableCollection<TblDyeingPlanHeaderViewModel>();
            _webService.GetDyeingHeaderListAccAsync(DyeingPlanHeaderViewModelList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void GenerateFabricLots()
        {
            var childWindow = new DyeingPlanFabricLotMasterChildWindow(this);
            childWindow.Show();
        }
    }
}