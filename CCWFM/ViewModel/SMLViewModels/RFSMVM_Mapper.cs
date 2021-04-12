using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.RFQViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public static class RFSMVM_Mapper
    {
        public static TblRFSHeader MapToModel(RFSViewModel objectToBemapped, bool isFullObjectMapper)
        {
            var temp = new TblRFSHeader
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
            temp.TblRFSDetails = new ObservableCollection<TblRFSDetail>();
            foreach (var item in objectToBemapped.RFQHeaderList)
            {
                var stemp = new TblRFSDetail
                {
                    Description = item.Desc,
                    Image = item.HeaderImage
                };
                if (item.MainHeaderTransID != null)
                    stemp.tblRFSHeader = item.MainHeaderTransID;
                stemp.StyleCode = item.Style;
                stemp.ColorCode = item.ColorCode;
                stemp.SizeCode = item.SizeCode;
                if (item.SubHeaderSerial != null)
                {
                    stemp.Iserial = (int)item.SubHeaderSerial;
                }
                stemp.TblRFSDetailServices = new ObservableCollection<TblRFSDetailService>();
                foreach (var sItem in item.HeaderServices)
                {
                    var hsTemp = new TblRFSDetailService { Image = sItem.Image };

                    if (sItem.Iserial != null)
                    {
                        hsTemp.Iserial = (int)sItem.Iserial;
                    }

                    hsTemp.Notes = sItem.Notes;
                    if (sItem.ServiceCode != null)
                        hsTemp.ServiceCode = sItem.ServiceCode;

                    if (sItem.ParentID != null)
                        hsTemp.Iserial = (int)sItem.ParentID;

                    stemp.TblRFSDetailServices.Add(hsTemp);
                }
                stemp.TblRFSDetailItems = new ObservableCollection<TblRFSDetailItem>();
                foreach (var iItems in item.HeaderItems)
                {
                    var iTemp = new TblRFSDetailItem
                        {
                            ItemCode = iItems.SelectedRFQItem.Code,
                            Description = iItems.Description,
                            Image = iItems.Image,
                            Size = iItems.SelectedRFQItem.Size,
                            Configuration = iItems.SelectedRFQItem.Config,
                            Batch = iItems.SelectedRFQItem.Batch,
                            Name = iItems.SelectedRFQItem.Name,
                            Source = iItems.SelectedRFQItem.ItemGroup
                        };
                    if (iItems.Iserial != null)
                    {
                        iTemp.Iserial = (int)iItems.Iserial;
                    }

                    iTemp.ItemCode = iItems.ItemCode;
                    if (iItems.ParentID != null)
                        iTemp.tblRFSDetail = iItems.ParentID;

                    stemp.TblRFSDetailItems.Add(iTemp);
                }

                temp.TblRFSDetails.Add(stemp);
            }
            return temp;
        }

        public static TblRFSDetail MapToModel(RFSSubHeader objectToBeMapped)
        {
            var stemp = new TblRFSDetail
            {
                Description = objectToBeMapped.Desc,
                Image = objectToBeMapped.HeaderImage,
                StyleCode = objectToBeMapped.Style
            };

            if (objectToBeMapped.SubHeaderSerial != null)
            {
                stemp.Iserial = (int)objectToBeMapped.SubHeaderSerial;
            }
            if (objectToBeMapped.MainHeaderTransID != null)
                stemp.tblRFSHeader = objectToBeMapped.MainHeaderTransID;
            stemp.StyleCode = objectToBeMapped.Style;
            stemp.ColorCode = objectToBeMapped.ColorCode;
            stemp.SizeCode = objectToBeMapped.SizeCode;

            stemp.TblRFSDetailServices = new ObservableCollection<TblRFSDetailService>();
            foreach (var sItem in objectToBeMapped.HeaderServices)
            {
                var hsTemp = new TblRFSDetailService { Image = sItem.Image };

                if (sItem.Iserial != null)
                {
                    hsTemp.Iserial = (int)sItem.Iserial;
                }

                hsTemp.Notes = sItem.Notes;
                if (sItem.ServiceCode != null)
                    hsTemp.ServiceCode = sItem.ServiceCode;

                if (sItem.ParentID != null)
                    hsTemp.tblRFSDetail = sItem.ParentID;

                stemp.TblRFSDetailServices.Add(hsTemp);
            }
            stemp.TblRFSDetailItems = new ObservableCollection<TblRFSDetailItem>();
            foreach (var iItems in objectToBeMapped.HeaderItems)
            {
                var iTemp = new TblRFSDetailItem { Description = iItems.Description, Image = iItems.Image };

                if (iItems.Iserial != null)
                {
                    iTemp.Iserial = (int)iItems.Iserial;
                }

                iTemp.ItemCode = iItems.ItemCode;
                if (iItems.ParentID != null)
                    iTemp.tblRFSDetail = iItems.ParentID;

                stemp.TblRFSDetailItems.Add(iTemp);
            }
            return stemp;
        }

        public static TblRFSDetailItem MapToModel(RFSHeaderItem objectToBeMapped)
        {
            var temp = new TblRFSDetailItem();
            temp.ItemCode = objectToBeMapped.SelectedRFQItem.Code;
            temp.Description = objectToBeMapped.Description;
            temp.Image = objectToBeMapped.SelectedRFQItem.Image;
            temp.Size = objectToBeMapped.SelectedRFQItem.Size;
            temp.Configuration = objectToBeMapped.SelectedRFQItem.Config;
            temp.Batch = objectToBeMapped.SelectedRFQItem.Batch;
            temp.Name = objectToBeMapped.SelectedRFQItem.Name;
            temp.Source = objectToBeMapped.SelectedRFQItem.ItemGroup;
            temp.tblRFSDetail = objectToBeMapped.ParentID;
            if (objectToBeMapped.Iserial != null)
            {
                temp.Iserial = (int)objectToBeMapped.Iserial;
            }

            return temp;
        }

        public static TblRFSDetailService MapToModel(RFSHeaderServices objectToBeMapped)
        {
            var temp = new TblRFSDetailService
            {
                Image = objectToBeMapped.Image,
                Notes = objectToBeMapped.Notes,
                ServiceCode = objectToBeMapped.ServiceCode,
                tblRFSDetail = objectToBeMapped.ParentID
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

        public static tbl_PurchaseOrderHeader MapToModel(PurchaseOrderHeaderViewModel objectToBeMapped, int parentID)
        {
            var purchHeader = objectToBeMapped;
            var purchHeaderTemp = new tbl_PurchaseOrderHeader
            {
                CreationDate = purchHeader.CreationDate,
                RecieveDate = purchHeader.DocDate,
                Vendor = purchHeader.VendorCode,
                WareHouseID = purchHeader.WarHouseCode,
                IsPosted = purchHeader.IsPosted,
                PurchaseID = purchHeader.PurchId
            };
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

        public static tblNewRFQPurchaseOrderHeader MapToModel(NewRfqPurchaseOrderHeaderViewModel objectToBeMapped)
        {
            var purchHeader = objectToBeMapped;
            var purchHeaderTemp = new tblNewRFQPurchaseOrderHeader
            {
                CreationDate = purchHeader.CreationDate,
                Vendor = purchHeader.VendorCode,
                IsPosted = purchHeader.IsPosted,
                WareHouseID = purchHeader.WarHouseCode
            };
            if (purchHeader.TransID != null) purchHeaderTemp.TransID = (int)purchHeader.TransID;
            purchHeaderTemp.NewRFQID = purchHeader.ParentId;
            purchHeaderTemp.PurchaseID = purchHeader.PurchId;
            purchHeaderTemp.tblNewRFQPurchLines = new ObservableCollection<tblNewRFQPurchLine>();
            foreach (var purchline in
                from detail in purchHeader.PurchaseOrderDetails
                let detail1 = detail
                select new tblNewRFQPurchLine
                {
                    ColorCode = detail1.StyleColor ?? "",
                    PurchasePrice = detail1.Price,
                    StyleCode = detail1.StyleHeader,
                    DeliveryDate = detail1.DelivaryDate,
                    tblNewRFQPurchLineSizes = new ObservableCollection<tblNewRFQPurchLineSize>
                        (
                        detail.PurchaseOrderSizes
                            .Where(x => x.IsTextBoxEnabled)
                            .Select(sizeDetail => new tblNewRFQPurchLineSize
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
                        )
                })
            {
                purchHeaderTemp.tblNewRFQPurchLines
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

        public static RFSViewModel MapToViewModel(TblRFSHeader objectToBeMapped, RFSViewModel objectToBeFilled)
        {
            if (objectToBeFilled == null)
            {
                objectToBeFilled = new RFSViewModel();
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

            foreach (var item in objectToBeMapped.TblRFSDetails)
            {
                objectToBeFilled.RFQHeaderList.Add(new RFSSubHeader(objectToBeFilled.RFQItems, objectToBeFilled.RFQServices));
                var temp2 = objectToBeFilled.RFQHeaderList[objectToBeFilled.RFQHeaderList.Count - 1];
                temp2.HeaderImage = item.Image;
                temp2.Desc = item.Description;
                temp2.MainHeaderTransID = objectToBeFilled.TransID;
                temp2.Style = item.StyleCode;
                temp2.ColorCode = item.ColorCode;
                temp2.SizeCode = item.SizeCode;
                temp2.ObjStatus = new ObjectStatus { IsNew = false, IsSavedDBItem = true, IsEmpty = false };

                foreach (var itemTemp in item.TblRFSDetailItems.Select(iItems => new RFSHeaderItem(temp2.RFQItems)
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

                foreach (var servtemp in item.TblRFSDetailServices.Select(sItem => new RFSHeaderServices(temp2.RFQServices)
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
            }
            objectToBeFilled.FormMode = ObjectMode.LoadedFromDb;
            objectToBeFilled.ObjStatus.IsNew = false;
            objectToBeFilled.ObjStatus.IsSavedDBItem = true;
            return objectToBeFilled;
        }
    }
}