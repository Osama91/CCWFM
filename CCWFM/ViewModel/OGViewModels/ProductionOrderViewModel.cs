using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using GalaSoft.MvvmLight.Command;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using System.Collections.Specialized;
using CCWFM.ProductionService;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.ViewModel.OGViewModels
{
    #region Model

    public class TblProductionOrderHeaderModel : CRUDManagerService.PropertiesViewModelBase
    {
        private string _DocCode;

        public string DocCode
        {
            get { return _DocCode; }
            set { _DocCode = value; RaisePropertyChanged("DocCode"); }
        }

        int? TblWarehouseField;

        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public int? TblWarehouse
        {
            get
            {
                return TblWarehouseField;
            }
            set
            {
                if ((TblWarehouseField.Equals(value) != true))
                {
                    TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }

        CRUDManagerService.TblWarehouse WareHousePerRowField;
        //[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public CRUDManagerService.TblWarehouse WareHousePerRow
        {
            get
            {
                return WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(WareHousePerRowField, value) != true))
                {
                    WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }

        private Web.DataLayer.ItemDimensionSearchModel itemTransfer;

        public virtual Web.DataLayer.ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new Web.DataLayer.ItemDimensionSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemTransfer, value) != true))
                {
                    itemTransfer = value;
                  //  TblItemDim = value.ItemDimFromIserial;
                    RaisePropertyChanged("ItemTransfer");
                }
            }
        }

        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; RaisePropertyChanged("Enabled"); }
        }
        ObservableCollection<TblProductionOrderTransactionModel> DetailsListField;
        public ObservableCollection<TblProductionOrderTransactionModel> DetailsList
        {
            get
            {
                return DetailsListField ?? (DetailsListField = new ObservableCollection<TblProductionOrderTransactionModel>());
            }
            set
            {
                if ((ReferenceEquals(DetailsListField, value) != true))
                {
                    DetailsListField = value;
                    RaisePropertyChanged("DetailsList");
                }
            }
        }

        private int CreatedByField;

        private DateTime? CreationDateField;

        private string DescriptionField;

        private DateTime? DocDateField;

        private int IserialField;

        private int? LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private decimal? QtyField;

        //private int? TblItemDimField;



        public int CreatedBy
        {
            get
            {
                return CreatedByField;
            }
            set
            {
                if ((CreatedByField.Equals(value) != true))
                {
                    CreatedByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }


        public DateTime? CreationDate
        {
            get
            {
                return CreationDateField;
            }
            set
            {
                if ((CreationDateField.Equals(value) != true))
                {
                    CreationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }


        public string Description
        {
            get
            {
                return DescriptionField;
            }
            set
            {
                if ((ReferenceEquals(DescriptionField, value) != true))
                {
                    DescriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        public DateTime? DocDate
        {
            get
            {
                return DocDateField;
            }
            set
            {
                if ((DocDateField.Equals(value) != true))
                {
                    DocDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public int? LastUpdatedBy
        {
            get
            {
                return LastUpdatedByField;
            }
            set
            {
                if ((LastUpdatedByField.Equals(value) != true))
                {
                    LastUpdatedByField = value;
                    RaisePropertyChanged("LastUpdatedBy");
                }
            }
        }


        public DateTime? LastUpdatedDate
        {
            get
            {
                return LastUpdatedDateField;
            }
            set
            {
                if ((LastUpdatedDateField.Equals(value) != true))
                {
                    LastUpdatedDateField = value;
                    RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }


        public decimal? Qty
        {
            get
            {
                return QtyField;
            }
            set
            {
                if ((QtyField.Equals(value) != true))
                {
                    QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }


        //public int? TblItemDim
        //{
        //    get
        //    {
        //        return TblItemDimField;
        //    }
        //    set
        //    {
        //        if ((TblItemDimField.Equals(value) != true))
        //        {
        //            TblItemDimField = value;
        //            RaisePropertyChanged("TblItemDim");
        //        }
        //    }
        //}
    }

    public class TblProductionOrderTransactionModel : CRUDManagerService.PropertiesViewModelBase
    {


        int? TblWarehouseField;

        public int? TblWarehouse
        {
            get
            {
                return TblWarehouseField;
            }
            set
            {
                if ((TblWarehouseField.Equals(value) != true))
                {
                    TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }
        CRUDManagerService.TblWarehouse WareHousePerRowField;
        public CRUDManagerService.TblWarehouse WareHousePerRow
        {
            get
            {
                return WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(WareHousePerRowField, value) != true))
                {
                    WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }


        private string _DocCode;

        public string DocCode
        {
            get { return _DocCode; }
            set { _DocCode = value; RaisePropertyChanged("DocCode"); }
        }


        bool PostedField;
        public bool Posted
        {
            get
            {
                return PostedField;
            }
            set
            {
                if ((PostedField.Equals(value) != true))
                {
                    PostedField = value;
                    RaisePropertyChanged("Posted");
                }
            }
        }

        int? PostedByField;
        public int? PostedBy
        {
            get
            {
                return this.PostedByField;
            }
            set
            {
                if ((this.PostedByField.Equals(value) != true))
                {
                    this.PostedByField = value;
                    this.RaisePropertyChanged("PostedBy");
                }
            }
        }
        DateTime? PostedDateField;
        public DateTime? PostedDate
        {
            get
            {
                return PostedDateField;
            }
            set
            {
                if ((this.PostedDateField.Equals(value) != true))
                {
                    this.PostedDateField = value;
                    this.RaisePropertyChanged("PostedDate");
                }
            }
        }

        private int CreatedByField;

        private DateTime? CreationDateField;

        private string DescriptionField;

        private DateTime? DocDateField;

        private int IserialField;

        private int? LastUpdatedByField;

        private DateTime? LastUpdatedDateField;

        private ObservableCollection<TblProductionOrderFabricModel> TblProductionOrderFabricsField;

        private int? TblProductionOrderHeaderField;


        private ObservableCollection<TblProductionOrderServiceModel> TblProductionOrderServicesField;

        private int? TblProductionOrderTransactionTypeField;




        public int CreatedBy
        {
            get
            {
                return CreatedByField;
            }
            set
            {
                if ((CreatedByField.Equals(value) != true))
                {
                    CreatedByField = value;
                    RaisePropertyChanged("CreatedBy");
                }
            }
        }


        public DateTime? CreationDate
        {
            get
            {
                return CreationDateField;
            }
            set
            {
                if ((CreationDateField.Equals(value) != true))
                {
                    CreationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }


        public string Description
        {
            get
            {
                return DescriptionField;
            }
            set
            {
                if ((object.ReferenceEquals(DescriptionField, value) != true))
                {
                    DescriptionField = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        public DateTime? DocDate
        {
            get
            {
                return DocDateField;
            }
            set
            {
                if ((DocDateField.Equals(value) != true))
                {
                    DocDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public int? LastUpdatedBy
        {
            get
            {
                return LastUpdatedByField;
            }
            set
            {
                if ((LastUpdatedByField.Equals(value) != true))
                {
                    LastUpdatedByField = value;
                    RaisePropertyChanged("LastUpdatedBy");
                }
            }
        }


        public DateTime? LastUpdatedDate
        {
            get
            {
                return LastUpdatedDateField;
            }
            set
            {
                if ((LastUpdatedDateField.Equals(value) != true))
                {
                    LastUpdatedDateField = value;
                    RaisePropertyChanged("LastUpdatedDate");
                }
            }
        }


        public ObservableCollection<TblProductionOrderFabricModel> TblProductionOrderFabrics
        {
            get
            {
                return TblProductionOrderFabricsField ?? (TblProductionOrderFabricsField = new ObservableCollection<TblProductionOrderFabricModel>());
            }
            set
            {
                if ((ReferenceEquals(TblProductionOrderFabricsField, value) != true))
                {
                    TblProductionOrderFabricsField = value;
                    RaisePropertyChanged("TblProductionOrderFabrics");
                }
            }
        }


        public int? TblProductionOrderHeader
        {
            get
            {
                return TblProductionOrderHeaderField;
            }
            set
            {
                if ((TblProductionOrderHeaderField.Equals(value) != true))
                {
                    TblProductionOrderHeaderField = value;
                    RaisePropertyChanged("TblProductionOrderHeader");
                }
            }
        }

        private CRUDManagerService.GenericTable _ProductionOrderTransactionTypePerRow;

        public CRUDManagerService.GenericTable ProductionOrderTransactionTypePerRow
        {
            get { return _ProductionOrderTransactionTypePerRow; }
            set
            {
                _ProductionOrderTransactionTypePerRow = value; RaisePropertyChanged("ProductionOrderTransactionTypePerRow");
                if (ProductionOrderTransactionTypePerRow != null && ProductionOrderTransactionTypePerRow.Iserial != 0)
                {
                    TblProductionOrderTransactionType = ProductionOrderTransactionTypePerRow.Iserial;
                }
            }
        }



        public ObservableCollection<TblProductionOrderServiceModel> TblProductionOrderServices
        {
            get
            {
                return TblProductionOrderServicesField ?? (TblProductionOrderServicesField = new ObservableCollection<TblProductionOrderServiceModel>());
            }
            set
            {
                if ((ReferenceEquals(TblProductionOrderServicesField, value) != true))
                {
                    TblProductionOrderServicesField = value;
                    RaisePropertyChanged("TblProductionOrderServices");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqTransactionType")]
        public int? TblProductionOrderTransactionType
        {
            get
            {
                return TblProductionOrderTransactionTypeField;
            }
            set
            {
                if ((TblProductionOrderTransactionTypeField.Equals(value) != true))
                {
                    TblProductionOrderTransactionTypeField = value;
                    RaisePropertyChanged("TblProductionOrderTransactionType");
                }
            }
        }
    }

    public class TblProductionOrderFabricModel : CRUDManagerService.PropertiesViewModelBase
    {

        private string _Notes;

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; RaisePropertyChanged("Notes"); }
        }

        private Web.DataLayer.ItemDimensionSearchModel itemTransfer;

        public virtual Web.DataLayer.ItemDimensionSearchModel ItemTransfer
        {
            get { return itemTransfer ?? (itemTransfer = new Web.DataLayer.ItemDimensionSearchModel()); }
            set
            {
                if ((ReferenceEquals(itemTransfer, value) != true))
                {
                    itemTransfer = value;
                    TblItemDim = value.ItemDimFromIserial;
                    RaisePropertyChanged("ItemTransfer");
                }
            }
        }

        int? TblWarehouseField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public int? TblWarehouse
        {
            get
            {
                return TblWarehouseField;
            }
            set
            {
                if ((TblWarehouseField.Equals(value) != true))
                {
                    TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }

        CRUDManagerService.TblWarehouse WareHousePerRowField;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public CRUDManagerService.TblWarehouse WareHousePerRow
        {
            get
            {
                return WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(WareHousePerRowField, value) != true))
                {
                    WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }

        private decimal? CostField;

        private int IserialField;

        private decimal? QtyField;

        private int? TblItemDimField;

        private TblItemDim TblItemDim1Field;

        private int? TblProductionOrderTransactionField;

        public decimal? Cost
        {
            get
            {
                return CostField;
            }
            set
            {
                if ((CostField.Equals(value) != true))
                {
                    CostField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public decimal? Qty
        {
            get
            {
                return QtyField;
            }
            set
            {
                if ((QtyField.Equals(value) != true))
                {
                    QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }


        public int? TblItemDim
        {
            get
            {
                return TblItemDimField;
            }
            set
            {
                if ((TblItemDimField.Equals(value) != true))
                {
                    TblItemDimField = value;
                    RaisePropertyChanged("TblItemDim");
                }
            }
        }


        public TblItemDim TblItemDim1
        {
            get
            {
                return TblItemDim1Field;
            }
            set
            {
                if ((ReferenceEquals(TblItemDim1Field, value) != true))
                {
                    TblItemDim1Field = value;
                    RaisePropertyChanged("TblItemDim1");
                }
            }
        }

        public int? TblProductionOrderTransaction
        {
            get
            {
                return TblProductionOrderTransactionField;
            }
            set
            {
                if ((TblProductionOrderTransactionField.Equals(value) != true))
                {
                    TblProductionOrderTransactionField = value;
                    RaisePropertyChanged("TblProductionOrderTransaction");
                }
            }
        }


    }

    public class TblProductionOrderServiceModel : CRUDManagerService.PropertiesViewModelBase
    {
        private string _Notes;

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; RaisePropertyChanged("Notes"); }
        }

        private decimal? CostField;

        private int? EntityAccountField;

        private int IserialField;

        private decimal? QtyField;

        private int? TblColorField;

        private CRUDManagerService.TblColor TblColor1Field;


        private int? TblJournalAccountTypeField;

        private int? TblProductionOrderTransactionField;


        private int? TblServiceField;

        private CCWFM.CRUDManagerService.TblService TblService1Field;




        public decimal? Cost
        {
            get
            {
                return CostField;
            }
            set
            {
                if ((CostField.Equals(value) != true))
                {
                    CostField = value;
                    RaisePropertyChanged("Cost");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAccount")]
        public int? EntityAccount
        {
            get
            {
                return EntityAccountField;
            }
            set
            {
                if ((EntityAccountField.Equals(value) != true))
                {
                    EntityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }


        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }


        public decimal? Qty
        {
            get
            {
                return QtyField;
            }
            set
            {
                if ((QtyField.Equals(value) != true))
                {
                    QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqColor")]
        public int? TblColor
        {
            get
            {
                return TblColorField;
            }
            set
            {
                if ((TblColorField.Equals(value) != true))
                {
                    TblColorField = value;
                    RaisePropertyChanged("TblColor");
                }
            }
        }


        public CRUDManagerService.TblColor ColorPerRow
        {
            get
            {
                return TblColor1Field;
            }
            set
            {
                if ((ReferenceEquals(TblColor1Field, value) != true))
                {
                    TblColor1Field = value;
                    TblColor = value.Iserial;
                    RaisePropertyChanged("ColorPerRow");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqJournalAccountType")]
        public int? TblJournalAccountType
        {
            get
            {
                return TblJournalAccountTypeField;
            }
            set
            {
                if ((TblJournalAccountTypeField.Equals(value) != true))
                {
                    TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }


        public int? TblProductionOrderTransaction
        {
            get
            {
                return TblProductionOrderTransactionField;
            }
            set
            {
                if ((TblProductionOrderTransactionField.Equals(value) != true))
                {
                    TblProductionOrderTransactionField = value;
                    RaisePropertyChanged("TblProductionOrderTransaction");
                }
            }
        }


        public int? TblService
        {
            get
            {
                return TblServiceField;
            }
            set
            {
                if ((TblServiceField.Equals(value) != true))
                {
                    TblServiceField = value;
                    RaisePropertyChanged("TblService");
                }
            }
        }


        public CCWFM.CRUDManagerService.TblService ServicePerRow
        {
            get
            {
                return TblService1Field;
            }
            set
            {
                if ((ReferenceEquals(TblService1Field, value) != true))
                {
                    TblService1Field = value;
                    TblService = ServicePerRow.Iserial;
                    RaisePropertyChanged("ServicePerRow");
                }
            }
        }
        private GlService.GenericTable _journalAccountTypePerRow;
        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _journalAccountTypePerRow; }
            set
            {
                if ((ReferenceEquals(_journalAccountTypePerRow, value) != true))
                {
                    _journalAccountTypePerRow = value;
                    if (JournalAccountTypePerRow != null) TblJournalAccountType = JournalAccountTypePerRow.Iserial;
                    RaisePropertyChanged("JournalAccountTypePerRow");
                }
            }
        }
        private GlService.Entity _entityPerRow;
        public GlService.Entity EntityPerRow
        {
            get { return _entityPerRow; }
            set
            {
                if ((ReferenceEquals(_entityPerRow, value) != true))
                {
                    _entityPerRow = value;
                    RaisePropertyChanged("EntityPerRow");
                    if (EntityPerRow != null)
                    {
                        EntityAccount = EntityPerRow.Iserial;
                    }
                }
            }
        }
    }

    #endregion

    public class ProductionOrderViewModel : ViewModelStructuredBase
    {
        public ProductionOrderViewModel() : base(PermissionItemName.ProductionOrder)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Approve = new RelayCommand(() =>
                {
                    SelectedDetailRow.Posted = true;
                    SaveDetailRow();
                });

                OpenItemSearch = new RelayCommand(() =>
            {
                try
                {

                    if (!string.IsNullOrWhiteSpace(SelectedMainRow.WareHousePerRow.Code))
                    {
                        var vm = new ItemDimensionAdjustmentSearchViewModel();
                        vm.WarehouseCode = SelectedMainRow.WareHousePerRow.Code;
                        vm.SiteIserial = SelectedMainRow.WareHousePerRow.TblSite;
                        vm.AppliedSearchResultList.CollectionChanged += (s, e) =>
                        {
                            foreach (var item in vm.AppliedSearchResultList)
                            {
                                //SelectedMainRow.ItemTransfer = item;
                                SelectedMainRow.Qty = item.AvailableQuantity;
                            }
                        };
                        var childWindowSeach = new ItemDimensionAdjustmentSearchChildWindow(vm);
                        childWindowSeach.Show();
                        vm.Title = strings.Production;
                        _FormMode = FormMode.Search;
                    }
                    else MessageBox.Show(strings.PleaseSelectWarehouseFrom);
                }
                catch (Exception ex) { throw ex; }
            });


                OpenItemSearchDetail = new RelayCommand(() =>
                {
                    try
                    {

                        if (!string.IsNullOrWhiteSpace(WareHousePerRow.Code))
                        {
                            //Issue
                            if (SelectedDetailRow.TblProductionOrderTransactionType == 1)
                            {
                                var vm = new ItemDimensionSearchViewModel();
                                vm.WarehouseCode = WareHousePerRow.Code;
                                vm.SiteIserial = WareHousePerRow.TblSite;
                                vm.AppliedSearchResultList.CollectionChanged += (s, e) =>
                                {
                                    foreach (var item in vm.AppliedSearchResultList)
                                    {
                                        var temp = SelectedDetailRow.TblProductionOrderFabrics.FirstOrDefault(td => td.TblItemDim == item.ItemDimFromIserial);
                                        if (item.AvailableQuantity < item.TransferredQuantity)
                                            continue;
                                        if (temp == null)// مش موجود
                                        {
                                            var transferDetail = new TblProductionOrderFabricModel()
                                            {
                                                TblProductionOrderTransaction = SelectedDetailRow.Iserial,
                                                TblItemDim = item.ItemDimFromIserial,
                                                ItemTransfer = item,
                                                Qty = item.TransferredQuantity,
                                                WareHousePerRow = WareHousePerRow,
                                                TblWarehouse = WareHousePerRow.Iserial,

                                            };
                                            SelectedDetailRow.TblProductionOrderFabrics.Add(transferDetail);
                                            SelectedProductionOrderFabric = transferDetail;
                                        }
                                        else// لو موجود هحدث الكمية
                                        {
                                            temp.Qty = item.TransferredQuantity;
                                        }
                                    }

                                };
                                var childWindowSeach = new ItemDimensionSearchChildWindow(vm);
                                childWindowSeach.Show();

                                childWindowSeach.IsTransfer = false;
                                childWindowSeach.QuantityTitle = strings.Qty;
                                vm.Title = strings.Production;
                            }
                            else
                            {
                                var vm = new ItemDimensionAdjustmentSearchViewModel();
                                vm.WarehouseCode = WareHousePerRow.Code;
                                vm.SiteIserial = WareHousePerRow.TblSite;
                                vm.AppliedSearchResultList.CollectionChanged += (s, e) =>
                                {
                                    foreach (var item in vm.AppliedSearchResultList)
                                    {
                                        var temp = SelectedDetailRow.TblProductionOrderFabrics.FirstOrDefault(td => td.TblItemDim == item.ItemDimFromIserial);
                                        if (item.AvailableQuantity < item.TransferredQuantity)
                                            continue;
                                        if (temp == null)// مش موجود
                                        {
                                            var transferDetail = new TblProductionOrderFabricModel()
                                            {
                                                TblProductionOrderTransaction = SelectedDetailRow.Iserial,
                                                TblItemDim = item.ItemDimFromIserial,
                                                ItemTransfer = item,
                                                Qty = item.TransferredQuantity,
                                                WareHousePerRow = WareHousePerRow,
                                                TblWarehouse = WareHousePerRow.Iserial,

                                            };
                                            SelectedDetailRow.TblProductionOrderFabrics.Add(transferDetail);
                                            SelectedProductionOrderFabric = transferDetail;
                                        }
                                        else// لو موجود هحدث الكمية
                                        {
                                            temp.Qty = item.TransferredQuantity;
                                        }
                                    }

                                };
                                var childWindowSeach = new ItemDimensionAdjustmentSearchChildWindow(vm);
                                childWindowSeach.Show();
                                vm.Title = strings.Production;
                            }

                            _FormMode = FormMode.Search;
                        }
                        else MessageBox.Show(strings.PleaseSelectWarehouseFrom);
                    }
                    catch (Exception ex) { throw ex; }
                });
                DetailChild = new RelayCommand(() =>
                {
                    GetProductionFabric();
                    GetProductionService();
                    if (SelectedDetailRow.WareHousePerRow != null)
                    {
                        WareHousePerRow = SelectedDetailRow.WareHousePerRow;
                        TblWarehouse = SelectedDetailRow.TblWarehouse;
                    }

                    var child = new ProductionOrderChildWindows(this);
                    child.Show();
                });

                MainRowList = new ObservableCollection<TblProductionOrderHeaderModel>();
                SelectedMainRow = new TblProductionOrderHeaderModel();
                SelectedMainRow.DetailsList = new ObservableCollection<TblProductionOrderTransactionModel>();
                SelectedMainRow.DetailsList.Add(new TblProductionOrderTransactionModel());
                Client.GetTblServiceAsync(0, int.MaxValue, "It.Iserial", null, null);

                Client.GetTblServiceCompleted += (s, sv) =>
                {
                    ServiceList = sv.Result;
                };

                var calculationClient = new CRUDManagerService.CRUD_ManagerServiceClient();
                calculationClient.GetGenericCompleted += (s, sv) =>
                {
                    ProductionOrderTransactionTypeList = sv.Result;
                };
                calculationClient.GetGenericAsync("TblProductionOrderTransactionType", "%%", "%%", "%%", "Iserial", "ASC");


                var journalAccountTypeClient = new GlService.GlServiceClient();
                journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
                {
                    JournalAccountTypeList = sv.Result;
                };
                journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

                var currencyClient = new GlService.GlServiceClient();

                currencyClient.GetGenericCompleted += (s, sv) =>
                {
                    CurrencyList = new ObservableCollection<CRUDManagerService.GenericTable>();
                    foreach (var item in sv.Result)
                    {
                        CurrencyList.Add(new CRUDManagerService.GenericTable().InjectFrom(item) as CRUDManagerService.GenericTable);
                    }

                };
                currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);
                Client.GetTblWarehouseAsync(0, int.MaxValue, "it.Iserial", null, null);

                Client.GetTblWarehouseCompleted += (s, sv) =>
                {
                    WareHouseList = sv.Result;
                };

                Client.GetAllWarehousesByCompanyNameAsync("CCm");
                ProductionClient.GetTblProductionOrderHeaderCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblProductionOrderHeaderModel();


                        newrow.InjectFrom(row);
                      
                        newrow.ItemTransfer = new Web.DataLayer.ItemDimensionSearchModel().InjectFrom( row.ItemTransfer) as Web.DataLayer.ItemDimensionSearchModel;

                        newrow.ItemTransfer.ColorPerRow = new Web.DataLayer.TblColor().InjectFrom( row.ItemTransfer.ColorPerRow) as Web.DataLayer.TblColor;
                        //if (row.TblProductionOrderTransactions.Any(w => w.TblProductionOrderTransactionType == 1))
                        //{ 
                        //    var prodTransaction = row.TblProductionOrderTransactions.FirstOrDefault(w => w.TblProductionOrderTransactionType == 1);

                        //    if (prodTransaction.TblProductionOrderFabrics.Any())
                        //    {
                        //        newrow.ItemTransfer = new Web.DataLayer.ItemDimensionSearchModel();

                        //        var itemtransfer = prodTransaction.TblProductionOrderFabrics.FirstOrDefault().TblItemDim1;
                        //        //newrow.ItemTransfer.ItemCode = itemtransfer.;
                        //        //newrow.ItemTransfer.ItemName = itemtransfer.ItemName;
                        //        newrow.ItemTransfer.ItemType = itemtransfer.ItemType;
                        //        newrow.ItemTransfer.ColorFromCode = itemtransfer.TblColor1.Code;
                        //        newrow.ItemTransfer.BatchNoFrom = itemtransfer.BatchNo;
                        //        newrow.ItemTransfer.SizeFrom = itemtransfer.Size;


                        //        //.InjectFrom(prodTransaction.TblProductionOrderFabrics.FirstOrDefault().ItemTransfer) as Web.DataLayer.ItemDimensionSearchModel;




                        //    }
                        //}
                        //newrow.WareHousePerRow = new CRUDManagerService.TblWarehouse().InjectFrom(row.TblWarehouse1) as CRUDManagerService.TblWarehouse;
                        //newrow.ItemTransfer.InjectFrom( row.ItemTransfer);
                        MainRowList.Add(newrow);
                        FullCount = sv.fullCount;
                        if (SearchWindow != null) {
                            SearchWindow.FullCount = FullCount;
                        }
                    }
                    Loading = false;
                };
                ProductionClient.GetTblProductionOrderTransactionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblProductionOrderTransactionModel();
                        newrow.InjectFrom(row);
                        newrow.ProductionOrderTransactionTypePerRow = new CRUDManagerService.GenericTable();
                        newrow.ProductionOrderTransactionTypePerRow.InjectFrom(row.TblProductionOrderTransactionType1);
                        if (row.TblWarehouse1 != null)
                        {
                            newrow.WareHousePerRow = new CRUDManagerService.TblWarehouse().InjectFrom(row.TblWarehouse1) as CRUDManagerService.TblWarehouse;
                        }

                        //  newrow.WareHousePerRow = row.TblWarehouse1;
                        SelectedMainRow.DetailsList.Add(newrow);
                    }
                    if (DetailFullCount == 0 && SelectedMainRow.DetailsList.Count == 0)
                    {
                        AddNewDetailRow(false);
                    }
                    Loading = false;
                };
                ProductionClient.GetTblProductionOrderFabricCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblProductionOrderFabricModel();
                        newrow.InjectFrom(row);
                        newrow.WareHousePerRow = new CRUDManagerService.TblWarehouse();
                        newrow.WareHousePerRow.InjectFrom(row.TblWarehouse1);
                        newrow.InjectFrom(row);
                        SelectedDetailRow.TblProductionOrderFabrics.Add(newrow);
                    }


                    Loading = false;
                };
                ProductionClient.GetTblProductionOrderServiceCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblProductionOrderServiceModel();

                        var servicetemp = new CRUDManagerService.TblService();
                        servicetemp.InjectFrom(row.TblService1);

                        newrow.ServicePerRow = servicetemp;
                        
                        
                        //= row.TblService1;
                        if (row.TblColor1 != null)
                        {
                            newrow.ColorPerRow = new CRUDManagerService.TblColor();
                            newrow.ColorPerRow.InjectFrom(row.TblColor1);

                        }
                        newrow.JournalAccountTypePerRow = new GlService.GenericTable();


                        newrow.JournalAccountTypePerRow.InjectFrom(JournalAccountTypeList.FirstOrDefault(w => w.Iserial == row.TblJournalAccountType));
                        newrow.EntityPerRow = new GlService.Entity();
                        newrow.EntityPerRow.InjectFrom(sv.entityList.FirstOrDefault(x => x.TblJournalAccountType == row.TblJournalAccountType &&
                                                    x.Iserial == row.EntityAccount));
                        newrow.InjectFrom(row);
                        SelectedDetailRow.TblProductionOrderServices.Add(newrow);
                    }
                    if (DetailSubFullCount == 0 && SelectedDetailRow.TblProductionOrderServices.Count == 0)
                    {
                        AddNewServiceRow(false);
                    }
                    Loading = false;
                };
                ProductionClient.DeleteTblProductionOrderTransactionCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedMainRow.DetailsList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedMainRow.DetailsList.Remove(oldrow);
                };

                ProductionClient.DeleteTblProductionOrderFabricCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedDetailRow.TblProductionOrderServices.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.TblProductionOrderServices.Remove(oldrow);
                };
                ProductionClient.UpdateOrInsertTblProductionOrderHeaderCompleted += (s, x) =>
                {
                    if (SelectedMainRow != null)
                    {
                        SelectedMainRow.Iserial = x.Result.Iserial;
                        SelectedMainRow.DocCode = x.Result.DocCode;
                    }
                    if (!SelectedMainRow.DetailsList.Any())
                    {
                        AddNewDetailRow(false);
                    }
                };

                ProductionClient.UpdateOrInsertTblProductionOrderTransactionCompleted += (s, x) =>
                {
                    Loading = false;
                    try
                    {
                        var savedRow = SelectedMainRow.DetailsList.ElementAt(x.outindex);
                        if (savedRow != null) savedRow.InjectFrom(x.Result);
                    }
                    catch (Exception)
                    {
                    }
                };

                ProductionClient.UpdateOrInsertTblProductionOrderFabricCompleted += (s, x) =>
                {
                    try
                    {
                        Loading = false;
                        var savedRow = SelectedDetailRow.TblProductionOrderFabrics.ElementAt(x.outindex);
                        if (savedRow != null) savedRow.Iserial = x.Result.Iserial;
                    }
                    catch (Exception)
                    {
                    }

                };
                ProductionClient.UpdateOrInsertTblProductionOrderServiceCompleted += (s, x) =>
                {
                    try
                    {
                        Loading = false;
                        var savedRow = SelectedDetailRow.TblProductionOrderServices.ElementAt(x.outindex);
                        if (savedRow != null) savedRow.InjectFrom(x.Result);
                    }
                    catch (Exception)
                    {
                    }

                };

                ProductionClient.DeleteTblProductionOrderHeaderCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                ProductionClient.DeleteTblProductionOrderServiceCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = SelectedDetailRow.TblProductionOrderServices.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) SelectedDetailRow.TblProductionOrderServices.Remove(oldrow);
                };
            }
        }


        #region Properties


        int? TblWarehouseField;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public int? TblWarehouse
        {
            get
            {
                return TblWarehouseField;
            }
            set
            {
                if ((TblWarehouseField.Equals(value) != true))
                {
                    TblWarehouseField = value;
                    RaisePropertyChanged("TblWarehouse");
                }
            }
        }

        CRUDManagerService.TblWarehouse WareHousePerRowField;
        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqWarehouse")]
        public CRUDManagerService.TblWarehouse WareHousePerRow
        {
            get
            {
                return WareHousePerRowField;
            }
            set
            {
                if ((ReferenceEquals(WareHousePerRowField, value) != true))
                {
                    WareHousePerRowField = value;
                    RaisePropertyChanged("WareHousePerRow");
                }
            }
        }
        ProductionServiceClient ProductionClient = new ProductionServiceClient();

        private CRUDManagerService.ItemsDto _itemPerRow;

        public CRUDManagerService.ItemsDto ItemPerRow
        {
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
            }
        }

        private DateTime? _From;

        public DateTime? FromDate
        {
            get { return _From; }
            set { _From = value; RaisePropertyChanged("FromDate"); }
        }

        private DateTime? _to;

        public DateTime? ToDate
        {
            get { return _to; }
            set { _to = value; RaisePropertyChanged("ToDate"); }
        }


        #endregion

        #region Commands

        RelayCommand openItemSearch;
        public RelayCommand OpenItemSearch
        {
            get { return openItemSearch; }
            set { openItemSearch = value; RaisePropertyChanged(nameof(OpenItemSearch)); }
        }
        RelayCommand _OpenItemSearchDetail;
        public RelayCommand OpenItemSearchDetail
        {
            get { return _OpenItemSearchDetail; }
            set { _OpenItemSearchDetail = value; RaisePropertyChanged(nameof(OpenItemSearchDetail)); }
        }

        RelayCommand<object> _ProductionFabricKeyDown;
        public RelayCommand<object> ProductionFabricKeyDown
        {
            get { return _ProductionFabricKeyDown; }
            set { _ProductionFabricKeyDown = value; RaisePropertyChanged(nameof(ProductionFabricKeyDown)); }
        }

        RelayCommand _FromTransfer;
        public RelayCommand FromTransfer
        {
            get { return _FromTransfer; }
            set { _FromTransfer = value; RaisePropertyChanged(nameof(FromTransfer)); }
        }



        RelayCommand approveTransfer;
        public RelayCommand ApproveTransfer
        {
            get { return approveTransfer; }
            set { approveTransfer = value; RaisePropertyChanged(nameof(ApproveTransfer)); }
        }

        RelayCommand<object> deleteTransferDetail;
        public RelayCommand<object> DeleteTransferDetail
        {
            get { return deleteTransferDetail; }
            set { deleteTransferDetail = value; RaisePropertyChanged(nameof(DeleteTransferDetail)); }
        }

        RelayCommand _DetailChild;
        public RelayCommand DetailChild
        {
            get { return _DetailChild; }
            set { _DetailChild = value; RaisePropertyChanged(nameof(DetailChild)); }
        }
        RelayCommand _Approve;
        public RelayCommand Approve
        {
            get { return _Approve; }
            set { _Approve = value; RaisePropertyChanged(nameof(Approve)); }
        }




        #endregion
        #region Methods

        public void AddNewDetailRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow));

            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblProductionOrderTransactionModel
            {
                TblProductionOrderHeader = SelectedMainRow.Iserial,

            };
            SelectedMainRow.DetailsList.Insert(currentRowIndex + 1, newrow);

            SelectedDetailRow = newrow;
        }
        //public void AddNewFabricRow(bool checkLastRow)
        //{
        //    var currentRowIndex = (SelectedDetailRow.TblProductionOrderFabrics.IndexOf(SelectedProductionOrderFabric));

        //    if (AllowAdd != true)
        //    {
        //        MessageBox.Show(strings.AllowAddMsg);
        //        return;
        //    }

        //    if (checkLastRow)
        //    {
        //        var valiationCollection = new List<ValidationResult>();

        //        var isvalid = Validator.TryValidateObject(SelectedDetailRow.TblProductionOrderFabrics, new ValidationContext(SelectedDetailRow.TblProductionOrderFabrics, null, null), valiationCollection, true);

        //        if (!isvalid)
        //        {
        //            return;
        //        }
        //    }
        //    var newrow = new TblProductionOrderFabricModel
        //    {
        //        TblProductionOrderTransaction = SelectedDetailRow.Iserial,

        //    };
        //    SelectedDetailRow.TblProductionOrderFabrics.Insert(currentRowIndex + 1, newrow);

        //    SelectedProductionOrderFabric = newrow;
        //}

        public void AddNewServiceRow(bool checkLastRow)
        {
            var currentRowIndex = (SelectedDetailRow.TblProductionOrderServices.IndexOf(SelectedProductionOrderService));

            if (AllowAdd != true)
            {
                MessageBox.Show(strings.AllowAddMsg);
                return;
            }

            if (checkLastRow)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow.TblProductionOrderServices, new ValidationContext(SelectedDetailRow.TblProductionOrderServices, null, null), valiationCollection, true);

                if (!isvalid)
                {
                    return;
                }
            }
            var newrow = new TblProductionOrderServiceModel
            {
                TblProductionOrderTransaction = SelectedDetailRow.Iserial,

            };
            SelectedDetailRow.TblProductionOrderServices.Insert(currentRowIndex + 1, newrow);

            SelectedProductionOrderService = newrow;
        }

        public void GetMaindata()
        {
                SortBy = "it.Iserial desc";
            ProductionClient.GetTblProductionOrderHeaderAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.DatabasEname);
        }
        public void GetDetailData()
        {
            if (SelectedMainRow != null)
            {
                if (DetailSortBy == null)
                    DetailSortBy = "it.Iserial";
                ProductionClient.GetTblProductionOrderTransactionAsync(SelectedMainRow.DetailsList.Count, int.MaxValue,
                    SelectedMainRow.Iserial, DetailSortBy, DetailFilter, DetailValuesObjects);
            }
        }

        public void GetProductionFabric()
        {
            if (SelectedDetailRow != null)
            {
                SelectedDetailRow.TblProductionOrderFabrics.Clear();
                if (DetailSubSortBy == null)
                    DetailSubSortBy = "it.Iserial";
                ProductionClient.GetTblProductionOrderFabricAsync(SelectedDetailRow.TblProductionOrderFabrics.Count, int.MaxValue,
                    SelectedDetailRow.Iserial, DetailSubSortBy, DetailSubFilter, DetailSubValuesObjects);
            }
        }

        public void GetProductionService()
        {
            if (SelectedDetailRow != null)
            {
                SelectedDetailRow.TblProductionOrderServices.Clear();
                if (ServiceSortBy == null)
                    ServiceSortBy = "it.Iserial";
                ProductionClient.GetTblProductionOrderServiceAsync(SelectedDetailRow.TblProductionOrderServices.Count, int.MaxValue,
                    SelectedDetailRow.Iserial, ServiceSortBy, ServiceFilter, ServiceValuesObjects, LoggedUserInfo.DatabasEname);
            }
        }



        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        ProductionClient.DeleteTblProductionOrderHeaderAsync((TblProductionOrderHeader)new TblProductionOrderHeader().InjectFrom(row), MainRowList.IndexOf(row));
                    }
                }
            }
        }





        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblProductionOrderHeader();
                    saveRow.InjectFrom(SelectedMainRow);
                    saveRow.ItemTransfer = null;
                    ProductionClient.UpdateOrInsertTblProductionOrderHeaderAsync(saveRow, save, 0, LoggedUserInfo.Iserial);
                }
            }
        }


        public void SaveDetailRow()
        {
            if (SelectedDetailRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedDetailRow, new ValidationContext(SelectedDetailRow, null, null), valiationCollection, true);

                if (isvalid && Loading == false)
                {
                    Loading = true;
                    var save = SelectedDetailRow.Iserial == 0;
                    var rowToSave = new TblProductionOrderTransaction();
                    rowToSave.InjectFrom(SelectedDetailRow);
                    ProductionClient.UpdateOrInsertTblProductionOrderTransactionAsync(rowToSave, save, SelectedMainRow.DetailsList.IndexOf(SelectedDetailRow), LoggedUserInfo.Iserial);
                }
            }
        }
        public void DeleteDetailRow()
        {
            if (SelectedDetailRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                  MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {

                    foreach (var row in SelectedDetailRows)
                    {
                        if (row.Iserial != 0)
                        {
                            ProductionClient.DeleteTblProductionOrderTransactionAsync((TblProductionOrderTransaction)new TblProductionOrderTransaction().InjectFrom(row), SelectedMainRow.DetailsList.IndexOf(row));
                        }
                        else
                        {
                            SelectedMainRow.DetailsList.Remove(row);
                        }
                    }
                }
            }
        }

        public void SaveProductionOrderFabrics()
        {
            foreach (var item in SelectedDetailRow.TblProductionOrderFabrics)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = item.Iserial == 0;
                    var rowToSave = new TblProductionOrderFabric();
                    if (SelectedDetailRow != null)
                    {
                        item.TblProductionOrderTransaction = SelectedDetailRow.Iserial;
                    }

                    rowToSave.InjectFrom(item);
                    rowToSave.ItemTransfer = null;
                    ProductionClient.UpdateOrInsertTblProductionOrderFabricAsync(rowToSave, save, SelectedDetailRow.TblProductionOrderFabrics.IndexOf(item));
                }

            }
        }

        public void SaveProductionOrderServices()
        {
            foreach (var item in SelectedDetailRow.TblProductionOrderServices)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = item.Iserial == 0;
                    var rowToSave = new TblProductionOrderService();
                    if (SelectedDetailRow != null)
                    {
                        item.TblProductionOrderTransaction = SelectedDetailRow.Iserial;
                    }
                    rowToSave.InjectFrom(item);
                    ProductionClient.UpdateOrInsertTblProductionOrderServiceAsync(rowToSave, save, SelectedDetailRow.TblProductionOrderServices.IndexOf(item));
                }

            }
        }


        public void DeleteProductionOrderFabric()
        {
            if (SelectedProductionOrderFabrics != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedProductionOrderFabrics)
                    {
                        if (row.Iserial == 0)
                        {
                            SelectedDetailRow.TblProductionOrderFabrics.Remove(row);
                        }
                        else
                        {
                            ProductionClient.DeleteTblProductionOrderFabricAsync((TblProductionOrderFabric)new TblProductionOrderFabric().InjectFrom(row));
                        }


                    }
                }
            }
        }

        public void DeleteProductionOrderService()
        {
            if (SelectedProductionOrderServices != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedProductionOrderServices)
                    {
                        if (row.Iserial == 0)
                        {
                            SelectedDetailRow.TblProductionOrderServices.Remove(row);
                        }
                        else
                        {
                            ProductionClient.DeleteTblProductionOrderServiceAsync((TblProductionOrderService)new TblProductionOrderService().InjectFrom(row));
                        }


                    }
                }
            }
        }


        public void DeleteOrder()
        {
            ProductionClient.DeleteTblProductionOrderHeaderAsync((TblProductionOrderHeader)new TblProductionOrderHeader().InjectFrom(SelectedMainRow), MainRowList.IndexOf(SelectedMainRow));
        }


        public override void Search()
        {
            MainRowList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TblProductionOrderHeaderModel> vm =
                new GenericSearchViewModel<TblProductionOrderHeaderModel>() { Title = "Production Order Search" };
            vm.FilteredItemsList = MainRowList;
            vm.ItemsList = MainRowList;
            vm.FullCount = FullCount;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    SelectedMainRow = vm.ResultItemsList[e.NewStartingIndex];
                GetDetailData();
                // RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) =>
            {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) =>
            {
                return true;
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }

        #endregion
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                   new SearchColumnModel()
                    {
                        Header=strings.Item,
                        PropertyPath="ItemTransfer.ItemCode",
                    },
                 new SearchColumnModel()
                    {
                        Header=strings.Color,
                        PropertyPath="ItemTransfer.ColorFromCode",
                    },
                   new SearchColumnModel()
                    {
                        Header=strings.Color,
                        PropertyPath="ItemTransfer.ColorPerRow.Code",
                    },

               
                   new SearchColumnModel()
                    {
                        Header=strings.Size,
                           PropertyPath="ItemTransfer.SizeFrom",
                    },
                         new SearchColumnModel()
                    {
                        Header=strings.ItemType,
                        PropertyPath="ItemTransfer.ItemType",
                    },
                                   new SearchColumnModel()
                    {
                        Header=strings.BatchNo,
                        PropertyPath="ItemTransfer.BatchNoFrom",
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Qty,
                        PropertyPath=nameof(TblProductionOrderHeaderModel.Qty),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.TransDate,
                        PropertyPath=nameof(TblProductionOrderHeaderModel.DocDate),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Code,
                        PropertyPath=nameof(TblProductionOrderHeaderModel.DocCode),
                    },
                    
                  
                         

                };
        }



        #region Prop     
        public string ServiceSortBy { get; set; }
        public string ServiceFilter { get; set; }
        public Dictionary<string, object> ServiceValuesObjects { get; set; }
        private ObservableCollection<CRUDManagerService.GenericTable> _CurrencyList;

        public ObservableCollection<CRUDManagerService.GenericTable> CurrencyList
        {
            get { return _CurrencyList; }
            set { _CurrencyList = value; RaisePropertyChanged("CurrencyList"); }
        }

        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        private ObservableCollection<CRUDManagerService.GenericTable> _ProductionOrderTransactionTypeList;

        public ObservableCollection<CRUDManagerService.GenericTable> ProductionOrderTransactionTypeList
        {
            get { return _ProductionOrderTransactionTypeList; }
            set { _ProductionOrderTransactionTypeList = value; RaisePropertyChanged("ProductionOrderTransactionTypeList"); }
        }

        private ObservableCollection<CRUDManagerService.TblWarehouse> _wareHouseList;

        public ObservableCollection<CRUDManagerService.TblWarehouse> WareHouseList
        {
            get { return _wareHouseList ?? (_wareHouseList = new ObservableCollection<CRUDManagerService.TblWarehouse>()); }
            set { _wareHouseList = value; RaisePropertyChanged("WareHouseList"); }
        }


        private ObservableCollection<CRUDManagerService.TblService> _ServiceList;

        public ObservableCollection<CRUDManagerService.TblService> ServiceList
        {
            get { return _ServiceList; }
            set { _ServiceList = value; RaisePropertyChanged("ServiceList"); }
        }

        private ObservableCollection<TblProductionOrderHeaderModel> _mainRowList;

        public ObservableCollection<TblProductionOrderHeaderModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblProductionOrderHeaderModel> _selectedMainRows;

        public ObservableCollection<TblProductionOrderHeaderModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblProductionOrderHeaderModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblProductionOrderHeaderModel _selectedMainRow;

        public TblProductionOrderHeaderModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblProductionOrderHeaderModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private TblProductionOrderTransactionModel _selectedDetailRow;

        public TblProductionOrderTransactionModel SelectedDetailRow
        {
            get { return _selectedDetailRow; }
            set { _selectedDetailRow = value; RaisePropertyChanged("SelectedDetailRow"); }
        }

        private ObservableCollection<TblProductionOrderTransactionModel> _selectedDetailRows;

        public ObservableCollection<TblProductionOrderTransactionModel> SelectedDetailRows
        {
            get
            {
                return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblProductionOrderTransactionModel>());
            }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        public string RecDetailFilter { get; set; }

        public Dictionary<string, object> RecDetailValuesObjects { get; set; }


        private TblProductionOrderFabricModel _selectedProductionOrderFabric;

        public TblProductionOrderFabricModel SelectedProductionOrderFabric
        {
            get { return _selectedProductionOrderFabric; }
            set { _selectedProductionOrderFabric = value; RaisePropertyChanged("SelectedProductionOrderFabric"); }
        }

        private TblProductionOrderServiceModel _SelectedProductionOrderService;

        public TblProductionOrderServiceModel SelectedProductionOrderService
        {
            get { return _SelectedProductionOrderService ?? (_SelectedProductionOrderService = new TblProductionOrderServiceModel()); }
            set { _SelectedProductionOrderService = value; RaisePropertyChanged("SelectedProductionOrderService"); }
        }


        private ObservableCollection<TblProductionOrderServiceModel> _selectedProductionOrderServices;

        public ObservableCollection<TblProductionOrderServiceModel> SelectedProductionOrderServices
        {
            get { return _selectedProductionOrderServices ?? (_selectedProductionOrderServices = new ObservableCollection<TblProductionOrderServiceModel>()); }
            set { _selectedProductionOrderServices = value; RaisePropertyChanged("SelectedProductionOrderServices"); }
        }

        private ObservableCollection<TblProductionOrderFabricModel> _selectedProductionOrderFabrics;

        public ObservableCollection<TblProductionOrderFabricModel> SelectedProductionOrderFabrics
        {
            get { return _selectedProductionOrderFabrics ?? (_selectedProductionOrderFabrics = new ObservableCollection<TblProductionOrderFabricModel>()); }
            set { _selectedProductionOrderFabrics = value; RaisePropertyChanged("SelectedProductionOrderFabrics"); }
        }


        private TblMarkupTranProdViewModel _selectedMarkup;

        public TblMarkupTranProdViewModel SelectedMarkupRow
        {
            get { return _selectedMarkup; }
            set
            {
                _selectedMarkup = value;
                RaisePropertyChanged("SelectedMarkupRow");
            }
        }

        private ObservableCollection<TransferHeader> _TransferList;
        public ObservableCollection<TransferHeader> TransferList
        {
            get { return _TransferList ?? (_TransferList = new ObservableCollection<TransferHeader>()); }
            set { _TransferList = value; RaisePropertyChanged(nameof(TransferList)); }
        }

        public string TransferFilter { get; private set; }
        public Dictionary<string, object> TransferValuesObjects { get; private set; }

        #endregion Prop    
        //public void SearchTransfer()
        //{
        //    TransferList.Clear();
        //    GetTransfer();
        //    if (SearchWindow == null)
        //        SearchWindow = new GenericSearchWindow(GetSearchTransferModel());
        //    GenericSearchViewModel<TransferHeader> vm =
        //        new GenericSearchViewModel<TransferHeader>() { Title = "Transfer Search" };
        //    vm.FilteredItemsList = TransferList;
        //    vm.ItemsList = TransferList;
        //    vm.ResultItemsList.CollectionChanged += (s, e) => {
        //        if (e.Action == NotifyCollectionChangedAction.Add)
        //            SelectedTransferRow = vm.ResultItemsList[e.NewStartingIndex];
        //        // RaisePropertyChanged(nameof(IsReadOnly));
        //    };
        //    vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) => {
        //        TransferFilter = vm.Filter;
        //        TransferValuesObjects = vm.ValuesObjects;
        //        GetTransfer();
        //    },
        //    (o) => {
        //        return true;//هنا الصلاحيات
        //    });
        //    SearchWindow.DataContext = vm;
        //    base.Search();
        //}

        //private static ObservableCollection<SearchColumnModel> GetSearchTransferModel()
        //{
        //    return new ObservableCollection<SearchColumnModel>()
        //        {
        //            new SearchColumnModel()
        //            {
        //                Header="Code From",
        //                PropertyPath=nameof(TransferHeader.CodeFrom),
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header=strings.WarehouseFrom,
        //                PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
        //                FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseFrom),nameof(TblWarehouse.Ename)),
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header="Code To",
        //                PropertyPath=nameof(TransferHeader.CodeTo),
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header=strings.WarehouseTo,
        //                PropertyPath= string.Format("{0}.{1}", nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
        //                FilterPropertyPath=string.Format("{0}.{1}",nameof(TransferHeader.TblWarehouseTo),nameof(TblWarehouse.Ename)),
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header=strings.Date,
        //                PropertyPath=nameof(TransferHeader.DocDate),
        //                StringFormat="{0:dd/MM/yyyy h:mm tt}",
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header=strings.Approved,
        //                PropertyPath=nameof(TransferHeader.Approved),
        //            },
        //            new SearchColumnModel()
        //            {
        //                Header=strings.ApproveDate,
        //                PropertyPath=nameof(TransferHeader.ApproveDate),
        //                StringFormat="{0:dd/MM/yyyy h:mm tt}",
        //            },
        //        };
        //}
    }
}