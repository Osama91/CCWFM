using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.RFQViewModels
{
    public static class RFQMVM_Mapper
    {
        public static tbl_RFQHeader MapToModel(RFQViewModel objectToBemapped, bool isFullObjectMapper)
        {
            var temp = new tbl_RFQHeader
            {
                BrandCode = objectToBemapped.BrandCode,
                SeasonCode = objectToBemapped.SeasonCode,
                SupplierCode = objectToBemapped.SupplierID
            };

            if (objectToBemapped.TransID != null)
            {
                temp.TransID = (int)objectToBemapped.TransID;
            }
            temp.DocNumber = objectToBemapped.DocNum ?? " ";
            temp.tbl_RFQDetail = new ObservableCollection<tbl_RFQDetail>();
            foreach (var item in objectToBemapped.RFQHeaderList)
            {
                var stemp = new tbl_RFQDetail
                {
                    DelivaryDate = item.DelivaryDate,
                    Descreption = item.Desc,
                    Image = item.HeaderImage,
                    IsSampleAttached = item.IsSampleAttached,
                    MainFabricDesc = item.MainFabDesc
                };
                if (item.MainHeaderTransID != null)
                    stemp.MainHeaderTransID = item.MainHeaderTransID;
                stemp.Notes = item.Notes;
                stemp.Qty = item.Qty;
                stemp.StyleCode = item.Style;
                stemp.SubFabricDesc = item.SubFabDesc;

                if (item.SubHeaderSerial != null)
                {
                    stemp.SubHeaderID = (int)item.SubHeaderSerial;
                }
                stemp.tbl_RFQDetailService = new ObservableCollection<tbl_RFQDetailService>();
                foreach (var sItem in item.HeaderServices)
                {
                    var hsTemp = new tbl_RFQDetailService { Image = sItem.Image };

                    if (sItem.Iserial != null)
                    {
                        hsTemp.Iserial = (int)sItem.Iserial;
                    }

                    hsTemp.Notes = sItem.Notes;
                    if (sItem.ServiceCode != null)
                        hsTemp.ServiceCode = sItem.ServiceCode;

                    if (sItem.ParentID != null)
                        hsTemp.SupHeaderID = sItem.ParentID;

                    stemp.tbl_RFQDetailService.Add(hsTemp);
                }
                stemp.tbl_RFQDetailItem = new ObservableCollection<tbl_RFQDetailItem>();
                foreach (var iItems in item.HeaderItems)
                {
                    var iTemp = new tbl_RFQDetailItem();
                    iTemp.ItemCode = iItems.SelectedRFQItem.Code;
                    iTemp.Description = iItems.Description;
                    iTemp.Image = iItems.Image;
                    iTemp.Size = iItems.SelectedRFQItem.Size;
                    iTemp.Configuration = iItems.SelectedRFQItem.Config;
                    iTemp.Batch = iItems.SelectedRFQItem.Batch;
                    iTemp.Name = iItems.SelectedRFQItem.Name;
                    iTemp.Source = iItems.SelectedRFQItem.ItemGroup;
                    if (iItems.Iserial != null)
                    {
                        iTemp.Iserial = (int)iItems.Iserial;
                    }

                    iTemp.ItemCode = iItems.ItemCode;
                    if (iItems.ParentID != null)
                        iTemp.SupHeaderID = iItems.ParentID;

                    stemp.tbl_RFQDetailItem.Add(iTemp);
                }
                stemp.tbl_RFQFollowup = new ObservableCollection<tbl_RFQFollowup>();
                foreach (var fItem in item.FollowUpList)
                {
                    var fTemp = new tbl_RFQFollowup
                    {
                        ActualDelivaryDate = fItem.ActualDelivaryDate,
                        EstDelivaryDate = fItem.EstimatedDelivaryDate,
                        FollowupType = 1
                    };

                    if (fItem.Iserial != null)
                    {
                        fTemp.Iserial = (int)fItem.Iserial;
                    }

                    fTemp.Notes = fItem.Notes;
                    fTemp.SizeCode = fItem.SizeCode;
                    fTemp.StatusID = fItem.StatusID;
                    fTemp.StyleCode = fItem.Style;
                    if (fItem.ParentID != null)
                        fTemp.SubHeaderID = fItem.ParentID;

                    stemp.tbl_RFQFollowup.Add(fTemp);
                }
                foreach (var fcItem in item.CostFollowUpList)
                {
                    var fcTemp = new tbl_RFQFollowup
                    {
                        ActualDelivaryDate = fcItem.ActualDelivaryDate,
                        Cost = fcItem.Cost,
                        EstDelivaryDate = fcItem.EstimatedDelivaryDate,
                        FollowupType = 0,
                        Currency = fcItem.Currency,
                        ExchangeRate = fcItem.ExchangeRate,
                        LocalCost = fcItem.LocalCost,
                        DCost = fcItem.AdditionalCost,
                        ColorCode = fcItem.StyleColor
                    };

                    if (fcItem.Iserial != null)
                    {
                        fcTemp.Iserial = (int)fcItem.Iserial;
                    }

                    fcTemp.Notes = fcItem.Notes;
                    fcTemp.StatusID = fcItem.Status;
                    fcTemp.StyleCode = fcItem.Style;
                    if (fcItem.ParentID != null)
                        fcTemp.SubHeaderID = fcItem.ParentID;

                    fcTemp.tbl_RFQ_AdditionalCost = fcItem.AdditionalCostList;
                    stemp.tbl_RFQFollowup.Add(fcTemp);
                }

                temp.tbl_RFQDetail.Add(stemp);
            }
            temp.tbl_PurchaseOrderHeader = new ObservableCollection<tbl_PurchaseOrderHeader>();
            foreach (var purchHeader in objectToBemapped.PurchHeaders)
            {
                var purchHeaderTemp = new tbl_PurchaseOrderHeader
                {
                    CreationDate = purchHeader.CreationDate,
                    RecieveDate = purchHeader.DocDate,
                    Vendor = purchHeader.VendorCode,
                    WareHouseID = purchHeader.WarHouseCode,
                    PurchaseID = purchHeader.PurchId,
                    tbl_PurchaseOrderDetails = new ObservableCollection<tbl_PurchaseOrderDetails>()
                };
                foreach (var detail in purchHeader.PurchaseOrderDetails)
                {
                    var detail1 = detail;
                    var purchline = new tbl_PurchaseOrderDetails
                        {
                            Color = detail1.StyleColor ?? "",
                            PurchasePrice = detail1.Price,
                            StyleCode = detail1.StyleHeader,
                            DelivaryDate = detail1.DelivaryDate
                        };
                    purchline.tbl_PurchaseOrderSizeDetails =
                        new ObservableCollection<tbl_PurchaseOrderSizeDetails>
                            (
                            detail.PurchaseOrderSizes
                                .Where(x => x.IsTextBoxEnabled).Select(sizeDetail => new tbl_PurchaseOrderSizeDetails
                                {
                                    Size = sizeDetail.SizeCode,
                                    Qty = int.Parse(sizeDetail.SizeConsumption.ToString(CultureInfo.InvariantCulture))
                                    ,
                                    Ratio = double.Parse(sizeDetail.SizeRatio.ToString(CultureInfo.InvariantCulture))
                                })
                            );
                    purchline.tbl_PurchaseOrder_AdditionalCost =
                        new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>
                            (detail.AdditionalCostList);
                    purchHeaderTemp.tbl_PurchaseOrderDetails
                            .Add
                            (
                                purchline
                            );
                }
                temp.tbl_PurchaseOrderHeader.Add(purchHeaderTemp);
            }
            return temp;
        }

        public static tbl_RFQDetail MapToModel(RFQSubHeader objectToBeMapped)
        {
            var stemp = new tbl_RFQDetail
            {
                DelivaryDate = objectToBeMapped.DelivaryDate,
                Descreption = objectToBeMapped.Desc,
                Image = objectToBeMapped.HeaderImage,
                IsSampleAttached = objectToBeMapped.IsSampleAttached,
                MainFabricDesc = objectToBeMapped.MainFabDesc,
                MainHeaderTransID = objectToBeMapped.MainHeaderTransID,
                Notes = objectToBeMapped.Notes,
                Qty = objectToBeMapped.Qty,
                StyleCode = objectToBeMapped.Style,
                SubFabricDesc = objectToBeMapped.SubFabDesc
            };

            if (objectToBeMapped.SubHeaderSerial != null)
            {
                stemp.SubHeaderID = (int)objectToBeMapped.SubHeaderSerial;
            }
            if (objectToBeMapped.MainHeaderTransID != null)
                stemp.MainHeaderTransID = objectToBeMapped.MainHeaderTransID;
            stemp.Notes = objectToBeMapped.Notes;
            stemp.Qty = objectToBeMapped.Qty;
            stemp.StyleCode = objectToBeMapped.Style;
            stemp.SubFabricDesc = objectToBeMapped.SubFabDesc;

            if (objectToBeMapped.SubHeaderSerial != null)
            {
                stemp.SubHeaderID = (int)objectToBeMapped.SubHeaderSerial;
            }
            stemp.tbl_RFQDetailService = new ObservableCollection<tbl_RFQDetailService>();
            foreach (var sItem in objectToBeMapped.HeaderServices)
            {
                var hsTemp = new tbl_RFQDetailService { Image = sItem.Image };

                if (sItem.Iserial != null)
                {
                    hsTemp.Iserial = (int)sItem.Iserial;
                }

                hsTemp.Notes = sItem.Notes;
                if (sItem.ServiceCode != null)
                    hsTemp.ServiceCode = sItem.ServiceCode;

                if (sItem.ParentID != null)
                    hsTemp.SupHeaderID = sItem.ParentID;

                stemp.tbl_RFQDetailService.Add(hsTemp);
            }
            stemp.tbl_RFQDetailItem = new ObservableCollection<tbl_RFQDetailItem>();
            foreach (var iItems in objectToBeMapped.HeaderItems)
            {
                var iTemp = new tbl_RFQDetailItem { Description = iItems.Description, Image = iItems.Image };

                if (iItems.Iserial != null)
                {
                    iTemp.Iserial = (int)iItems.Iserial;
                }

                iTemp.ItemCode = iItems.ItemCode;
                if (iItems.ParentID != null)
                    iTemp.SupHeaderID = iItems.ParentID;

                stemp.tbl_RFQDetailItem.Add(iTemp);
            }
            stemp.tbl_RFQFollowup = new ObservableCollection<tbl_RFQFollowup>();
            foreach (var fItem in objectToBeMapped.FollowUpList)
            {
                var fTemp = new tbl_RFQFollowup
                {
                    ActualDelivaryDate = fItem.ActualDelivaryDate,
                    EstDelivaryDate = fItem.EstimatedDelivaryDate,
                    FollowupType = 1
                };

                if (fItem.Iserial != null)
                {
                    fTemp.Iserial = (int)fItem.Iserial;
                }

                fTemp.Notes = fItem.Notes;
                fTemp.SizeCode = fItem.SizeCode;
                fTemp.StatusID = fItem.StatusID;
                fTemp.StyleCode = fItem.Style;
                if (fItem.ParentID != null)
                    fTemp.SubHeaderID = fItem.ParentID;

                stemp.tbl_RFQFollowup.Add(fTemp);
            }
            foreach (var fcItem in objectToBeMapped.CostFollowUpList)
            {
                var fcTemp = new tbl_RFQFollowup
                {
                    ActualDelivaryDate = fcItem.ActualDelivaryDate,
                    Cost = fcItem.Cost,
                    EstDelivaryDate = fcItem.EstimatedDelivaryDate,
                    FollowupType = 0
                };

                if (fcItem.Iserial != null)
                {
                    fcTemp.Iserial = (int)fcItem.Iserial;
                }

                fcTemp.Notes = fcItem.Notes;
                fcTemp.StatusID = fcItem.Status;
                fcTemp.StyleCode = fcItem.Style;
                if (fcItem.ParentID != null)
                    fcTemp.SubHeaderID = fcItem.ParentID;

                stemp.tbl_RFQFollowup.Add(fcTemp);
            }
            return stemp;
        }

        public static tbl_RFQDetailItem MapToModel(RFQHeaderItem objectToBeMapped)
        {
            var temp = new tbl_RFQDetailItem();
            temp.ItemCode = objectToBeMapped.SelectedRFQItem.Code;
            temp.Description = objectToBeMapped.Description;
            temp.Image = objectToBeMapped.SelectedRFQItem.Image;
            temp.Size = objectToBeMapped.SelectedRFQItem.Size;
            temp.Configuration = objectToBeMapped.SelectedRFQItem.Config;
            temp.Batch = objectToBeMapped.SelectedRFQItem.Batch;
            temp.Name = objectToBeMapped.SelectedRFQItem.Name;
            temp.Source = objectToBeMapped.SelectedRFQItem.ItemGroup;
            temp.SupHeaderID = objectToBeMapped.ParentID;
            if (objectToBeMapped.Iserial != null)
            {
                temp.Iserial = (int)objectToBeMapped.Iserial;
            }

            return temp;
        }

        public static tbl_RFQDetailService MapToModel(RFQHeaderServices objectToBeMapped)
        {
            var temp = new tbl_RFQDetailService
            {
                Image = objectToBeMapped.Image,
                Notes = objectToBeMapped.Notes,
                ServiceCode = objectToBeMapped.ServiceCode,
                SupHeaderID = objectToBeMapped.ParentID
            };

            if (objectToBeMapped.Iserial != null)
            {
                temp.Iserial = (int)objectToBeMapped.Iserial;
            }

            return temp;
        }

        public static tbl_RFQFollowup MapToModel(RFQFollowUpViewModel objectToBeMapped)
        {
            var temp = new tbl_RFQFollowup
            {
                ActualDelivaryDate = objectToBeMapped.ActualDelivaryDate,
                Notes = objectToBeMapped.Notes,
                EstDelivaryDate = objectToBeMapped.EstimatedDelivaryDate,
                StyleCode = objectToBeMapped.Style,
                SubHeaderID = objectToBeMapped.ParentID,
                StatusID = objectToBeMapped.StatusID,
                SizeCode = objectToBeMapped.SizeCode,
                FollowupType = 1
            };

            if (objectToBeMapped.Iserial != null)
            {
                temp.Iserial = (int)objectToBeMapped.Iserial;
            }

            return temp;
        }

        public static tbl_RFQFollowup MapToModel(RFQCostFollowUp objectToBeMapped)
        {
            var temp = new tbl_RFQFollowup
            {
                ActualDelivaryDate = objectToBeMapped.ActualDelivaryDate,
                Notes = objectToBeMapped.Notes,
                EstDelivaryDate = objectToBeMapped.EstimatedDelivaryDate,
                StyleCode = objectToBeMapped.Style,
                ColorCode = objectToBeMapped.StyleColor,
                SubHeaderID = objectToBeMapped.ParentID,
                StatusID = objectToBeMapped.Status,
                Cost = objectToBeMapped.Cost,
                Currency = objectToBeMapped.Currency,
                ExchangeRate = objectToBeMapped.ExchangeRate,
                LocalCost = objectToBeMapped.LocalCost,
                DCost = objectToBeMapped.AdditionalCost,
                FollowupType = 0
            };

            if (objectToBeMapped.Iserial != null)
            {
                temp.Iserial = (int)objectToBeMapped.Iserial;
            }

            return temp;
        }

        public static tbl_PurchaseOrderHeader MapToModel(PurchaseOrderHeaderViewModel objectToBeMapped, int parentID)
        {
            var purchHeader = objectToBeMapped;
            var purchHeaderTemp = new tbl_PurchaseOrderHeader();
            purchHeaderTemp.CreationDate = purchHeader.CreationDate;
            purchHeaderTemp.RecieveDate = purchHeader.DocDate;
            purchHeaderTemp.Vendor = purchHeader.VendorCode;
            purchHeaderTemp.WareHouseID = purchHeader.WarHouseCode;
            purchHeaderTemp.IsPosted = purchHeader.IsPosted;
            purchHeaderTemp.PurchaseID = purchHeader.PurchId;
            if (purchHeader.TransID != null) purchHeaderTemp.TransID = (int)purchHeader.TransID;
            purchHeaderTemp.RFQTransID = parentID;
            purchHeaderTemp.tbl_PurchaseOrderDetails = new ObservableCollection<tbl_PurchaseOrderDetails>();
            foreach (var purchline in
                from detail in purchHeader.PurchaseOrderDetails
                let detail1 = detail
                select new tbl_PurchaseOrderDetails
                    {
                        Color = detail1.StyleColor ?? "",
                        PurchasePrice = detail1.Price,
                        StyleCode = detail1.StyleHeader,
                        DelivaryDate = detail1.DelivaryDate,
                        tbl_PurchaseOrderSizeDetails = new ObservableCollection<tbl_PurchaseOrderSizeDetails>
                            (
                            detail.PurchaseOrderSizes
                                .Where(x => x.IsTextBoxEnabled)
                                .Select(sizeDetail => new tbl_PurchaseOrderSizeDetails
                                    {
                                        Size = sizeDetail.SizeCode,
                                        Qty =
                                            int.Parse(
                                                sizeDetail.SizeConsumption.ToString(CultureInfo.InvariantCulture))
                                        ,
                                        Ratio =
                                            double.Parse(
                                                sizeDetail.SizeRatio.ToString(CultureInfo.InvariantCulture))
                                    })
                            ),
                        tbl_PurchaseOrder_AdditionalCost =
                            new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>(detail.AdditionalCostList)
                    })
            {
                purchHeaderTemp.tbl_PurchaseOrderDetails
                               .Add
                    (
                        purchline
                    );
            }
            return purchHeaderTemp;
        }

        public static tbl_PurchaseOrderHeader MapToModel(PurchaseOrderHeaderViewModel objectToBeMapped)
        {
            var purchHeader = objectToBeMapped;
            var purchHeaderTemp = new tbl_PurchaseOrderHeader();
            purchHeaderTemp.CreationDate = purchHeader.CreationDate;
            purchHeaderTemp.RecieveDate = purchHeader.DocDate;
            purchHeaderTemp.Vendor = purchHeader.VendorCode;
            purchHeaderTemp.IsPosted = purchHeader.IsPosted;
            purchHeaderTemp.WareHouseID = purchHeader.WarHouseCode;
            if (purchHeader.TransID != null) purchHeaderTemp.TransID = (int)purchHeader.TransID;
            purchHeaderTemp.RFQTransID = purchHeader.ParentId;
            purchHeaderTemp.PurchaseID = purchHeader.PurchId;
            purchHeaderTemp.tbl_PurchaseOrderDetails = new ObservableCollection<tbl_PurchaseOrderDetails>();
            foreach (var purchline in
                from detail in purchHeader.PurchaseOrderDetails
                let detail1 = detail
                select new tbl_PurchaseOrderDetails
                {
                    Color = detail1.StyleColor ?? "",
                    PurchasePrice = detail1.Price,
                    StyleCode = detail1.StyleHeader,
                    DelivaryDate = detail1.DelivaryDate,
                    tbl_PurchaseOrderSizeDetails = new ObservableCollection<tbl_PurchaseOrderSizeDetails>
                        (
                        detail.PurchaseOrderSizes
                            .Where(x => x.IsTextBoxEnabled)
                            .Select(sizeDetail => new tbl_PurchaseOrderSizeDetails
                            {
                                Size = sizeDetail.SizeCode,
                                Qty =
                                    int.Parse(
                                        sizeDetail.SizeConsumption.ToString(CultureInfo.InvariantCulture))
                                ,
                                Ratio =
                                    double.Parse(
                                        sizeDetail.SizeRatio.ToString(CultureInfo.InvariantCulture))
                            })
                        ),
                    tbl_PurchaseOrder_AdditionalCost =
                        new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>(detail.AdditionalCostList)
                })
            {
                purchHeaderTemp.tbl_PurchaseOrderDetails
                               .Add
                    (
                        purchline
                    );
            }
            return purchHeaderTemp;
        }

        public static tbl_PurchaseOrderDetails MapToModel(PurchasOrderDetailsViewModel objectToBeMapped)
        {
            var detail1 = objectToBeMapped;
            var retVal = detail1.Iserial == null ? new tbl_PurchaseOrderDetails
                {
                    Color = detail1.StyleColor ?? "",
                    PurchasePrice = detail1.Price,
                    StyleCode = detail1.StyleHeader,
                    DelivaryDate = detail1.DelivaryDate,
                    Trans_TransactionHeader = detail1.ParentTransID,
                    tbl_PurchaseOrder_AdditionalCost =
                        new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>(detail1.AdditionalCostList)
                }
                :
                new tbl_PurchaseOrderDetails
                {
                    Iserial = (int)detail1.Iserial,
                    Color = detail1.StyleColor ?? "",
                    PurchasePrice = detail1.Price,
                    StyleCode = detail1.StyleHeader,
                    DelivaryDate = detail1.DelivaryDate,
                    Trans_TransactionHeader = detail1.ParentTransID,
                    tbl_PurchaseOrder_AdditionalCost =
                        new ObservableCollection<tbl_PurchaseOrder_AdditionalCost>(detail1.AdditionalCostList)
                }
                ;
            retVal.tbl_PurchaseOrderSizeDetails = new ObservableCollection<tbl_PurchaseOrderSizeDetails>
                        (
                        detail1.PurchaseOrderSizes
                               .Where(x => x.IsTextBoxEnabled)
                               .Select(sizeDetail => sizeDetail.Iserial != null ? new tbl_PurchaseOrderSizeDetails
                                   {
                                       Iserial = (int)sizeDetail.Iserial,
                                       Size = sizeDetail.SizeCode,
                                       Qty =
                                           int.Parse(
                                               sizeDetail.SizeConsumption.ToString(CultureInfo.InvariantCulture))
                                       ,
                                       Ratio =
                                           double.Parse(
                                               sizeDetail.SizeRatio.ToString(CultureInfo.InvariantCulture))
                                   } : new tbl_PurchaseOrderSizeDetails
                                   {
                                       Size = sizeDetail.SizeCode,
                                       Qty =
                                           int.Parse(
                                               sizeDetail.SizeConsumption.ToString(CultureInfo.InvariantCulture))
                                       ,
                                       Ratio =
                                           double.Parse(
                                               sizeDetail.SizeRatio.ToString(CultureInfo.InvariantCulture))
                                   })
                        );

            return retVal;
        }

        public static RFQViewModel MapToViewModel(tbl_RFQHeader objectToBeMapped, RFQViewModel objectToBeFilled)
        {
            if (objectToBeFilled == null)
            {
                objectToBeFilled = new RFQViewModel();
            }
            objectToBeFilled.TransID = objectToBeMapped.TransID;
            objectToBeFilled.BrandCode = objectToBeMapped.BrandCode;
            objectToBeFilled.BrandProp = objectToBeFilled.Brands.FirstOrDefault(x => x.Brand_Code == objectToBeFilled.BrandCode);
            objectToBeFilled.SeasonCode = objectToBeMapped.SeasonCode;
            objectToBeFilled.SeasonProp = objectToBeFilled.Seasons.FirstOrDefault(x => x.Code == objectToBeFilled.SeasonCode);

            objectToBeFilled.SupplierID = objectToBeMapped.SupplierCode;
            objectToBeFilled.SupplierProp = objectToBeFilled.Vendors.FirstOrDefault(x => x.vendor_code == objectToBeFilled.SupplierID);
            objectToBeFilled.DocNum = objectToBeMapped.DocNumber;

            objectToBeFilled.ObjStatus = new ObjectStatus();

            foreach (var item in objectToBeMapped.tbl_RFQDetail)
            {
                objectToBeFilled.RFQHeaderList.Add(new RFQSubHeader(objectToBeFilled.RFQItems, objectToBeFilled.RFQServices));
                var temp2 = objectToBeFilled.RFQHeaderList[objectToBeFilled.RFQHeaderList.Count - 1];
                temp2.HeaderImage = item.Image;
                if (item.DelivaryDate != null) temp2.DelivaryDate = (DateTime)item.DelivaryDate;
                temp2.Desc = item.Descreption;
                temp2.IsSampleAttached = item.IsSampleAttached;
                temp2.MainFabDesc = item.MainFabricDesc;
                temp2.MainHeaderTransID = objectToBeFilled.TransID;
                temp2.Notes = item.Notes;
                if (item.Qty != null) temp2.Qty = (int)item.Qty;
                temp2.Style = item.StyleCode;
                temp2.SubFabDesc = item.SubFabricDesc;
                temp2.SubHeaderSerial = item.SubHeaderID;
                temp2.ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false };

                foreach (var itemTemp in item.tbl_RFQDetailItem.Select(iItems => new RFQHeaderItem(temp2.RFQItems)
                {
                    Description = iItems.Description,
                    Image = iItems.Image,
                    Iserial = iItems.Iserial,
                    ItemCode = iItems.ItemCode,
                    ParentID = temp2.SubHeaderSerial,
                    ItemGroup = iItems.Source,
                    Batch = iItems.Batch,
                    Config = iItems.Configuration,
                    Size = iItems.Size,
                    Name = iItems.Name
                }))
                {
                    itemTemp.SelectedRFQItem = new ItemsDto
                    {
                        Code = itemTemp.ItemCode,
                        Desc = itemTemp.Description,
                        Image = itemTemp.Image,
                        ItemGroup = itemTemp.ItemGroup,
                        Size = itemTemp.Size,
                        Config = itemTemp.Config,
                        Batch = itemTemp.Batch,
                        Name = itemTemp.Name
                    };
                    itemTemp.ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false };

                    temp2.HeaderItems.Add(itemTemp);
                }

                foreach (var servtemp in item.tbl_RFQDetailService.Select(sItem => new RFQHeaderServices(temp2.RFQServices)
                {
                    Image = sItem.Image,
                    Iserial = sItem.Iserial,
                    Notes = sItem.Notes,
                    ServiceCode = sItem.ServiceCode,
                    ParentID = temp2.SubHeaderSerial
                }))
                {
                    servtemp.SelectedRFQService = servtemp.RFQServices.FirstOrDefault(x => x.Code == servtemp.ServiceCode);
                    servtemp.ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false };

                    temp2.HeaderServices.Add(servtemp);
                }

                foreach (var ftemp in item.tbl_RFQFollowup.Where(x => x.FollowupType == 1)
                    .Select(fItem => new RFQFollowUpViewModel
                {
                    ActualDelivaryDate = fItem.ActualDelivaryDate,
                    EstimatedDelivaryDate = fItem.EstDelivaryDate,
                    Notes = fItem.Notes,
                    SizeCode = fItem.SizeCode,
                    StatusID = fItem.StatusID,
                    Style = fItem.StyleCode,
                    ParentID = temp2.SubHeaderSerial,
                    IsApproved = fItem.StatusID == 2,
                    Iserial = fItem.Iserial,
                    ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false }
                }))
                {
                    temp2.FollowUpList.Add(ftemp);
                }

                foreach (var fctemp in item.tbl_RFQFollowup.Where(x => x.FollowupType == 0)
                    .Select(fcItem => new RFQCostFollowUp
                {
                    ActualDelivaryDate = fcItem.ActualDelivaryDate,
                    Cost = fcItem.Cost,
                    EstimatedDelivaryDate = fcItem.EstDelivaryDate,
                    Notes = fcItem.Notes,
                    Status = fcItem.StatusID,
                    IsApproved = fcItem.StatusID == 2,
                    Style = fcItem.StyleCode,
                    Currency = fcItem.Currency,
                    ExchangeRate = fcItem.ExchangeRate,
                    LocalCost = fcItem.LocalCost,
                    ParentID = temp2.SubHeaderSerial,
                    AdditionalCost = fcItem.DCost,
                    Iserial = fcItem.Iserial,
                    AdditionalCostList = fcItem.tbl_RFQ_AdditionalCost,
                    Parent = temp2,
                    StyleColor = fcItem.ColorCode,
                }))
                {
                    temp2.CostFollowUpList.Add(fctemp);
                }
            }
            foreach (var purchHeader in objectToBeMapped.tbl_PurchaseOrderHeader)
            {
                var purchHeaderTemp = new PurchaseOrderHeaderViewModel
                {
                    CreationDate = (DateTime)purchHeader.CreationDate,
                    TransID = purchHeader.TransID,
                    DocDate = purchHeader.RecieveDate,
                    VendorCode = purchHeader.Vendor,
                    WarHouseCode = purchHeader.WareHouseID,
                    ParentId = purchHeader.RFQTransID,
                    PurchId = purchHeader.PurchaseID,
                    IsPosted = purchHeader.IsPosted,
                    ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true }
                };

                #region [ Adding Purch Lines ]

                var detailHeaders = (from tblPurchaseOrderDetails in purchHeader.tbl_PurchaseOrderDetails
                                     let purchasePrice = tblPurchaseOrderDetails.PurchasePrice
                                     where purchasePrice != null
                                     let rowTotal = tblPurchaseOrderDetails.tbl_PurchaseOrderSizeDetails.Sum(x => x.Qty)
                                     let tmp = tblPurchaseOrderDetails.tbl_PurchaseOrder_AdditionalCost.Sum(x => x.LocalValue)
                                     let addCost = tmp == 0 ? tmp : (tmp / rowTotal)
                                     select new PurchasOrderDetailsViewModel
                                     {
                                         StyleColor = tblPurchaseOrderDetails.Color,
                                         Price = (decimal)purchasePrice,
                                         StyleHeader = tblPurchaseOrderDetails.StyleCode,
                                         DelivaryDate = tblPurchaseOrderDetails.DelivaryDate,
                                         ParentTransID = tblPurchaseOrderDetails.Trans_TransactionHeader,
                                         ParentRfqSub = objectToBeFilled.RFQHeaderList.FirstOrDefault(x => x.Style == tblPurchaseOrderDetails.StyleCode),
                                         Iserial = tblPurchaseOrderDetails.Iserial,
                                         RowTotal = rowTotal ?? 0,
                                         AdditionalCost = addCost ?? 0,
                                         ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false, IsReadyForSaving = true, IsLoading = true },
                                         AdditionalCostList = tblPurchaseOrderDetails.tbl_PurchaseOrder_AdditionalCost
                                     }).ToList();
                foreach (var detail in purchHeader.tbl_PurchaseOrderDetails)
                {
                    var counter = 0;
                    var detail1 = detail.tbl_PurchaseOrderSizeDetails;
                    var purchline = detailHeaders.FirstOrDefault(x => x.Iserial == detail.Iserial);
                    foreach (var sizeDetail in detail1)
                    {
                        purchline.PurchaseOrderSizes[counter].SizeCode = sizeDetail.Size;
                        purchline.PurchaseOrderSizes[counter].SizeConsumption = (int)sizeDetail.Qty;
                        purchline.PurchaseOrderSizes[counter].IsTextBoxEnabled = true;
                        purchline.PurchaseOrderSizes[counter].Iserial = sizeDetail.Iserial;
                        purchline.PurchaseOrderSizes[counter].SizeRatio = (decimal)sizeDetail.Ratio;
                        counter++;
                    }
                }

                detailHeaders.ForEach(x => purchHeaderTemp.PurchaseOrderDetails.Add(x));
                purchHeaderTemp.GrandTotal =
                    int.Parse
                    (
                        purchHeaderTemp
                        .PurchaseOrderDetails
                        .Select
                            (x =>
                                x.PurchaseOrderSizes
                                .Sum(z => z.SizeConsumption)
                            ).Sum().ToString(CultureInfo.InvariantCulture)
                    );

                #endregion [ Adding Purch Lines ]

                purchHeaderTemp.SelectedPurchDetail = purchHeaderTemp.PurchaseOrderDetails.FirstOrDefault();
                detailHeaders.ToList().ForEach(x => x.ObjStatus.IsLoading = false);
                objectToBeFilled.PurchHeaders.Add(purchHeaderTemp);
            }
            if (objectToBeFilled.PurchHeaders != null && objectToBeFilled.PurchHeaders.Any())
                objectToBeFilled.PurchaseHeaderSelectedItem = objectToBeFilled.PurchHeaders[0];

            objectToBeFilled.AllFollowupApproved =
                objectToBeFilled.RFQHeaderList
                .Any(x => (x.FollowupsApproved = x.FollowUpList
                    .Any(y => y.IsApproved) && x.CostFollowUpList
                       .Any(z => z.IsApproved)));
            objectToBeFilled.FormMode = ObjectMode.LoadedFromDb;
            objectToBeFilled.ObjStatus.IsNew = false;
            objectToBeFilled.ObjStatus.IsSavedDBItem = true;
            return objectToBeFilled;
        }
    }
}