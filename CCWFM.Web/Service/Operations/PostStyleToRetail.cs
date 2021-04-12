using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        public void PostStyleToPo(int iserial, int salesOrder, bool postPo, int tblAuthUser, out decimal? tempprice, out float? tempCost)
        {
            tempprice = 0;
            tempCost = 0;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var poiserial = 0;
                var style =
                    context.TblStyles.Include("TblStyleCategory1")
                        .Include("TblLkpBrandSection1.TblLkpBrandSectionLinks")
                        .Include("TblLkpSeason1")
                        .Include("TblStyleStatu")
                        .Include("TblSizeGroup1.TblSizes")
                        .Include("tbl_lkp_FabricDesignes1")
                        .Include("TblLkpDirection1")
                        .Include("TblSupplierFabric1")
                        .Include("TblFamily1")
                        .Include("TblSubFamily1")
                        .FirstOrDefault(x => x.Iserial == iserial);

                var brandSectionLink =
                    context.TblLkpBrandSectionLinks.FirstOrDefault(
                        x => x.TblBrand == style.Brand && x.TblLkpBrandSection == style.TblLkpBrandSection);
                var SalesOrder =
                    context.TblSalesOrders.Include("TblSalesOrderColors.Tblcolor1")
                        .Include("TblSalesOrderColors.TblSalesOrderColorTheme1").Include("TblSalesOrderColors.TblSalesOrderSizeRatios")
                        .FirstOrDefault(x => x.Iserial == salesOrder);

                var group5 = FindOrCreate("TblGroup5",
           new GenericTable
           {
               Iserial = 0,
               Aname = style.TblLkpDirection1.Aname,
               Code = style.TblLkpDirection1.Code,
               Ename = style.TblLkpDirection1.Ename
           });

                var fabricAtt = context.tbl_FabricAttriputes.FirstOrDefault(x => x.Iserial == style.tbl_FabricAttriputes);
                var group2 = FindOrCreate("TblGroup2",
                       new GenericTable
                       {
                           Iserial = 0,
                           Aname = fabricAtt.FabricID,
                           Code = fabricAtt.FabricID,
                           Ename = fabricAtt.FabricID
                       });

                var group3 = FindOrCreate("TblGroup3",
                      new GenericTable
                      {
                          Iserial = 0,
                          Aname = style.tbl_lkp_FabricDesignes1.Aname,
                          Code = style.tbl_lkp_FabricDesignes1.Code,
                          Ename = style.tbl_lkp_FabricDesignes1.Ename
                      });
                var group4 = FindOrCreate("TblGroup4",
                    new GenericTable
                    {
                        Iserial = 0,
                        Aname = style.TblLkpSeason1.Aname,
                        Code = style.TblLkpSeason1.Code,
                        Ename = style.TblLkpSeason1.Ename
                    });
                var group8 = 0;
                if (style.TblStyleCategory1 != null)
                {
                    group8 = FindOrCreate("TblGroup8",
                new GenericTable
                {
                    Iserial = 0,
                    Aname = style.TblStyleCategory1.Aname,
                    Code = style.TblStyleCategory1.Code,
                    Ename = style.TblStyleCategory1.Ename
                });
                }

                var brand = context.Brands.FirstOrDefault(x => x.Brand_Code == style.Brand);

                var class4 = CreateClasses(brand, style.TblLkpBrandSection1, style.TblFamily1, style.TblSubFamily1);
                var sizeGroup = FindOrCreate("tblsizegroup",
                    new GenericTable
                    {
                        Iserial = 0,
                        Aname = style.TblSizeGroup1.Aname,
                        Code = style.TblSizeGroup1.Code,
                        Ename = style.TblSizeGroup1.Ename
                    });

                using (var ccnewcontext = new ccnewEntities())
                {
                    foreach (var color in SalesOrder.TblSalesOrderColors)
                    {
                        try
                        {
                            var retailcolor = FindOrCreate("tblcolor",
                            new GenericTable
                            {
                                Iserial = 0,
                                Aname = color.TblColor1.Aname,
                                Code = color.TblColor1.Code,
                                Ename = color.TblColor1.Ename
                            });

                            var tblitemdownloadDef =
                                ccnewcontext.TblItemDownLoadDefs.FirstOrDefault(
                                    x => x.Code == brandSectionLink.TblItemDownLoadDef);

                            var tblstylecolorgroup1 =
                            ccnewcontext.TblStyleColorGroup1.FirstOrDefault(
                                x =>
                                    x.ISERIAL == color.TblSalesOrderColorTheme);
                            if (tblstylecolorgroup1 == null)
                            {
                                tblstylecolorgroup1 = new TblStyleColorGroup1
                                {
                                    ISERIAL = color.TblSalesOrderColorTheme1.Iserial,
                                    CODE = color.TblSalesOrderColorTheme1.Code,
                                    ANAME = color.TblSalesOrderColorTheme1.Aname,
                                    ENAME = color.TblSalesOrderColorTheme1.Ename,
                                    TblGroup4 = group4,
                                    TblItemDownloadDef = tblitemdownloadDef.iserial,
                                };
                                ccnewcontext.TblStyleColorGroup1.AddObject(tblstylecolorgroup1);
                                ccnewcontext.SaveChanges();
                            }
                            else
                            {
                                tblstylecolorgroup1.CODE = color.TblSalesOrderColorTheme1.Code;
                                tblstylecolorgroup1.ANAME = color.TblSalesOrderColorTheme1.Aname;
                                tblstylecolorgroup1.ENAME = color.TblSalesOrderColorTheme1.Ename;
                                ccnewcontext.SaveChanges();
                            }

                            var tblStyleColorGroupLinks = ccnewcontext.TblStyleColorGroupLinks.FirstOrDefault(x => x.Style == style.RefStyleCode && x.TblColor == retailcolor);

                            if (tblStyleColorGroupLinks == null)
                            {
                                tblStyleColorGroupLinks = new TblStyleColorGroupLink
                                {
                                    Style = style.RefStyleCode,
                                    TblColor = retailcolor,
                                    TblStyleColorGroup1 = tblstylecolorgroup1.ISERIAL,
                                    Notes = color.Notes
                                };
                                ccnewcontext.TblStyleColorGroupLinks.AddObject(tblStyleColorGroupLinks);
                                ccnewcontext.SaveChanges();
                            }
                            else
                            {
                                tblStyleColorGroupLinks.TblStyleColorGroup1 = tblstylecolorgroup1.ISERIAL;
                                ccnewcontext.SaveChanges();
                            }


                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                    var subSeasonRow = ccnewcontext.TblSubSeasons.FirstOrDefault(x => x.Code == style.TblLkpSeason1.Code);
                    var seasonRow = ccnewcontext.TblSeasons.FirstOrDefault(x => x.Code == style.TblLkpSeason1.ShortCode);

                    if (subSeasonRow == null)
                    {
                        subSeasonRow = new TblSubSeason
                        {
                            Iserial = GetMaxIserial("TblSubSeason"),
                            Code = style.TblLkpSeason1.Code,
                            Ename = style.TblLkpSeason1.Ename,
                            Aname = style.TblLkpSeason1.Aname,
                            TblSeason = seasonRow.ISerial,
                        };
                        ccnewcontext.TblSubSeasons.AddObject(subSeasonRow);
                        ccnewcontext.SaveChanges();
                    }

                    var retailbrand =
                        ccnewcontext.TblItemDownLoadDefs.FirstOrDefault(
                            x => x.Code == brandSectionLink.TblItemDownLoadDef);

                    if (postPo)
                    {
                        poiserial = PostRetailPo(SalesOrder, (int)brandSectionLink.RetailWarehouse);
                    }
                    var dserial = 0;
                    foreach (var size in style.TblSizeGroup1.TblSizes.OrderBy(x => x.Id))
                    {
                        var Size = FindOrCreate("tblsize",
                            new GenericTable
                            {
                                Iserial = 0,
                                Aname = size.SizeCode,
                                Code = size.SizeCode,
                                Ename = size.SizeCode
                            });

                        var sizeLink =
                            ccnewcontext.TblSizeLinks.FirstOrDefault(
                                x => x.TblSize1.Code == size.SizeCode && x.TblSizeGroup == sizeGroup);

                        if (sizeLink == null)
                        {
                            sizeLink = new TblSizeLink
                            {
                                TblSize = Size,
                                TblSizeGroup = sizeGroup,
                                iserial = size.Id
                            };
                            ccnewcontext.TblSizeLinks.AddObject(sizeLink);
                            ccnewcontext.SaveChanges();
                        }

                        foreach (var color in SalesOrder.TblSalesOrderColors)
                        {
                            var transactionexist = false;
                            dserial++;
                            var retailcolor = ccnewcontext.TblColorTests.FirstOrDefault(q => q.Code == color.TblColor1.Code);
                            var styleIserial = CreateRetailStyle(tblAuthUser, style, size, color.TblColor1, group2, group3, group4, group5, group8, class4,
                                (int)SalesOrder.TblSupplier, retailcolor.ISERIAL, sizeLink.TblSize, sizeLink.TblSizeGroup,
                                seasonRow.ISerial, retailbrand.iserial, subSeasonRow.Iserial.ToString(), postPo, color, out tempprice, out tempCost, out transactionexist);
                            var salesOrderSize = color.TblSalesOrderSizeRatios.FirstOrDefault(x => x.Size == size.SizeCode);

                            if (postPo)
                            {
                                if (salesOrderSize != null)
                                {

                                    PostRetailPoDetail(style, SalesOrder, retailbrand.iserial, styleIserial, salesOrderSize, poiserial, dserial, color.DeliveryDate, color);
                                }

                            }
                        }
                    }
                }

                if (postPo)
                {
                    using (var ccnewcontext = new ccnewEntities())
                    {
                        var testingSelect = ccnewcontext.TBLITEMprices.FirstOrDefault(x => x.Style == style.StyleCode);
                        if (testingSelect == null)
                        {
                            throw new FaultException("Something Wrong Please Contact The Administrator");
                        }
                        var poheaderRow =
                            ccnewcontext.TblPO1Header.FirstOrDefault(x => x.glserial == poiserial);
                        poheaderRow.creationdate = DateTime.Now;
                        ccnewcontext.SaveChanges();                        

                        var body = "Order Code :" + SalesOrder.SalesOrderCode;

                        var subject = SalesOrder.SalesOrderCode;
                        try
                        {
                            SendMailReportPo("Po", subject, body, tblAuthUser, SalesOrder);
                        }
                        catch (Exception)
                        {

                            
                        }
                       
                    }
                }
                context.SaveChanges();
            }
        }
        [OperationContract]
        public void SendMailReportPo(string reportName, string subject, string body, int tblAuthUser, TblSalesOrder salesOrder)
        {
            string deviceInfo = null;
            var extension = String.Empty;
            var mimeType = String.Empty;
            var encoding = String.Empty;
            Warning[] warnings = null;
            string[] streamIDs = null;
            string historyId = null;

            // Create a Report Execution object
            var rsExec = new ReportExecutionService();
            rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            using (var context = new WorkFlowManagerDBEntities())
            {
                if (string.IsNullOrEmpty(ReportServer))
                {
                    ReportServer = context.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "ReportServer").sSetupValue;
                }
            }

            rsExec.Url = ReportServer + "/ReportExecution2005.asmx";

            // Load the report
            var execInfo = rsExec.LoadReport("/Report/" + reportName, historyId);
            var para = new ObservableCollection<string> { salesOrder.Iserial.ToString() };
            var parameters = new ParameterValue[para.Count];
            foreach (var row in para)
            {
                var index = para.IndexOf(row);
                parameters[0] = new ParameterValue();
                parameters[index].Value = row;
                parameters[index].Name = execInfo.Parameters[index].Name;

                // paramters) { Name = , Value = row } }, "en-us");
            }
            rsExec.SetExecutionParameters(parameters, "en-us");

            // get pdf of report
            var results = rsExec.Render("PDF", deviceInfo,
            out extension, out encoding,
            out mimeType, out warnings, out streamIDs);

            //Walla...almost no code, it's easy to manage and your done.

            //Take the bytes and add as an attachment to a MailMessage(SMTP):

            var attach = new Attachment(new MemoryStream(results),
                String.Format("{0}.pdf", reportName));

            string emailFrom;
            var emailTo = new List<string>();
            using (var model = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var code = model.TblAuthUsers.FirstOrDefault(w => w.Iserial == tblAuthUser).Code;
                    //emailFrom = "osama.gamal@ccasual.loc";//
                    emailFrom = model.Employees.FirstOrDefault(x => x.EMPLID == code).Email;
                    var style = model.TblStyles.FirstOrDefault(x => x.Iserial == salesOrder.TblStyle);

                    var brandsectionMail = model.TblBrandSectionMails.Where(
                        x => x.TblBrand == style.Brand && x.TblLkpBrandSection == style.TblLkpBrandSection);

                    foreach (var variable in brandsectionMail.Select(x => x.Emp))
                    {
                        //emailTo.Add("osama.gamal@ccasual.loc");//
                        emailTo.Add(model.Employees.FirstOrDefault(x => x.EMPLID == variable).Email);
                    }
                }
                catch (Exception)
                {
                    emailFrom = model.Employees.FirstOrDefault(x => x.EMPLID == "0158").Email;
                }
            }

            //string emailTo;
            //using (var Model = new WorkFlowManagerDBEntities())
            //{
            //    emailTo = "osama.gamal@ccasual.loc";
            //    // emailFrom = Model.Employees.FirstOrDefault(x => x.EMPLID == EmpCode).Email;
            //}
            SendEmail(attach, emailFrom, emailTo, subject, body);
        }
        public int PostRetailPo(TblSalesOrder salesOrder, int retailbrand)
        {
            using (var ccnewcontext = new ccnewEntities())
            {
                var poheader =
                    ccnewcontext.TblPO1Header.FirstOrDefault(x => x.supplierInvno == salesOrder.SalesOrderCode);
                if (poheader == null)
                {
                    poheader = (new TblPO1Header
                    {
                        Code = "0",
                        tblstore = retailbrand,
                        supplierInvno = salesOrder.SalesOrderCode,
                        tblsupplier = salesOrder.TblSupplier,
                        votdate = DateTime.Now,
                        Ddate = salesOrder.DeliveryDate,
                        TblUser = 10,
                        contactperson = "0",
                        totblstore = 0,
                        chequedate = "0",
                        totvochnotax = 0,
                        totvat = 0,
                        totpricewvat = 0,
                        commericaldisc = 0,
                        grosscost = 0,
                        additionaltax = 0,
                        cashdisc = 0,
                        miscdisc = 0,
                        miscexp = 0,
                        cashinvototal = 0,
                        period = 0,
                        Votype = 0,
                        tblseason = 0,
                        INVOICE = 0,
                        BeforeInvoice = 0,
                        CashNo = 0,
                        PoStatus = 0,
                    });
                    ccnewcontext.TblPO1Header.AddObject(poheader);
                    ccnewcontext.SaveChanges();
                }
                return poheader.glserial;
            }
        }
        public void PostRetailPoDetail(TblStyle style, TblSalesOrder salesOrder, int retailbrand, int styleIserial, TblSalesOrderSizeRatio size, int poheader, int Dserial, DateTime? deliveryDate, TblSalesOrderColor color)
        {
            using (var ccnewcontext = new ccnewEntities())
            {
                var poheaderMaindetail =
                    ccnewcontext.TblPO1MainDetail.FirstOrDefault(
                        x => x.glserial == poheader && x.Dserial == Dserial);
                if (poheaderMaindetail == null)
                {
                    poheaderMaindetail = (new TblPO1MainDetail
                    {
                        glserial = poheader,
                        Dserial = Dserial,
                        tblitem = styleIserial,
                        Quantity = size.ProductionPerSize,
                        ucostwot = color.LocalCost,
                        totwot = 0,
                        unitvat = 0,
                        ucostwvat = 0,
                        totvat = 0,
                        totpricewvat = 0,
                        AddExp = 0,
                        ItmDis = 0,
                        barcode = "0",
                        QtyRec = 0,
                        ItmCommericalDisc = 0,
                        ItemSalesTax = 0,
                        DeliveryDate = deliveryDate,
                    }
                        );
                    ccnewcontext.TblPO1MainDetail.AddObject(poheaderMaindetail);
                    ccnewcontext.SaveChanges();
                }
                var poDetail =
                   ccnewcontext.TblPO1Detail.FirstOrDefault(
                       x => x.glserial == poheader && x.Dserial == Dserial);
                if (poDetail == null)
                {
                    poDetail = (new TblPO1Detail
                    {
                        glserial = poheader,
                        Dserial = Dserial,
                        Dserial1 = Dserial,
                        tblitem = styleIserial,
                        Quantity = size.ProductionPerSize,
                        ucostwot = color.LocalCost,
                        totwot = 0,
                        unitvat = 0,
                        ucostwvat = 0,
                        totvat = 0,
                        totpricewvat = 0,
                        AddExp = 0,
                        ItmDis = 0,
                        barcode = "0",
                        QtyRec = 0,
                        ItmCommericalDisc = 0,
                        ItemSalesTax = 0,
                        ItmAdditionalTax = 0,
                        ItmMiscDisc = 0,
                        ItmMiscExp = 0,
                        CashDiscOnVouch = 0,
                    }
                        );
                    ccnewcontext.TblPO1Detail.AddObject(poDetail);
                    ccnewcontext.SaveChanges();
                }
            }
        }
        public int CreateRetailStyle(int userIserial, TblStyle style, TblSize size, TblColor color, int group2, int group3, int group4, int group5, int group8, int class4, int tblSupplier, int tblcolor, int tblsize, int tblsizegroup, int tblSeason, int retailBrand, string subSeason, bool post, TblSalesOrderColor tblSalesOrderColor, out decimal? price1, out float? Cost, out bool TransactionExist)
        {
            TransactionExist = false;
            price1 = 0;
            Cost = 0;
            const char addedChar = '0';
            var maxiserial = GetMaxIserial("tblitemprice");
            using (var ccnewcontext = new ccnewEntities())
            {
                ccnewcontext.CommandTimeout = 0;
                var barcodeprice = style.BarcodePrice;

               
                var code = style.RefStyleCode + size.SizeCode.PadLeft(4, addedChar) +
                              color.Code.PadLeft(4, addedChar);
                var row = ccnewcontext.TBLITEMprices.FirstOrDefault(x => x.Code == code);
                float cost = 0;
                // 8/8/2017
                if (tblSalesOrderColor.LocalCost != null && tblSalesOrderColor.LocalCost != 0)
                {
                    cost = (float)tblSalesOrderColor.LocalCost;
                }
                else
                {
                    if (style.TargetCostPrice != 0)
                    {
                        cost = (float)style.TargetCostPrice;
                        tblSalesOrderColor.LocalCost = style.TargetCostPrice as decimal?;
                    }
                }
                if (barcodeprice == 0)
                {
                    barcodeprice = style.RetailTargetCostPrice;
                }
                //if (post)
                //{
                //    if (!GetExistPermByUser(userIserial, "UpdateTargetPriceForRetailPo"))
                //    {                        
                //        cost = 0;
                //    }          
                //}
                //else
                //{
                //    if (!GetExistPermByUser(userIserial, "UpdateTargetPriceForCCPo"))
                //    {
                //        cost = 0;
                //    }
                //}
                if (row == null)
                {
                    try
                    {
                        var date = DateTime.Now.ToString("MM/dd/yyyy");
                        if (style.CreationDate != null) date = style.CreationDate.Value.ToString("MM/dd/yyyy");
                        if (style.LastUpdatedDate != null)
                        {
                            date = style.LastUpdatedDate.Value.ToString("MM/dd/yyyy");
                        }
                        string referance = style.Brand + style.TblLkpBrandSection1.Code + style.TblLkpSeason1.Code +
                                           style.TblLkpDirection1.Code + (style.TblSubFamily1.Code.PadLeft(4, addedChar)) + style.SerialNo;

                        string specialfield2 = "";
                        if (style.TblFamily1.IncludeSub)
                        {
                            specialfield2 = style.TblSubFamily1.Code.PadLeft(4, addedChar) + style.TblLkpSeason1.ShortCode + style.SerialNo.PadLeft(3, addedChar) + style.TblLkpBrandSection1.TblLkpBrandSectionLinks.FirstOrDefault(w => w.TblBrand == style.Brand && w.TblLkpBrandSection == style.TblLkpBrandSection).ShortCode;
                            //replicate('0',4-len(tblsubfamily.code))+tblsubfamily.code+tbllkpseason.ShortCode+replicate('0',3-len(serialno))+serialno+TblLkpBrandSectionLink.ShortCode
                        }
                        else
                        {
                            specialfield2 = style.TblFamily1.Code.PadLeft(2, addedChar) + style.TblLkpSeason1.ShortCode + style.SerialNo.PadLeft(3, addedChar) + style.TblLkpBrandSection1.TblLkpBrandSectionLinks.FirstOrDefault(w => w.TblBrand == style.Brand && w.TblLkpBrandSection == style.TblLkpBrandSection).ShortCode;
                        }

                        var query =
                                "insert into tblitem (ISerial,Code, AName,EName, SName" +
                                ", TblClass4, TblGroup1, TblGroup2, TblGroup3, TblGroup4, TblGroup5, TblGroup6, TblGroup7" +
                                ", TblUnit, TblGroup8, TblSupplier ,tblcolor,tblsize,tblsizegroup,itemtype,actType,salesprice," +
                                " style,lastChangeDate,tblSeason ,price1," +
                                "price2,price3,price4,ItemStoreGroup,itemdiscgroup,itemcommgroup," +
                                "packcapacity,packname,hasexdate,activeflg,hassets,SpecialFld1,refrance,ItemCost,SpecialFld2) Values(" +
                                " " + maxiserial + ",'" + code + "','" + style.Description + "','" + style.Description + "','" + style.Description + "'," + class4 + "," +
                                " " + style.TblGroup1 + "," + group2 + "," + group3 + "," + group4 + "," + group5 + "," + style.TblGroup6 + ","
                                + style.TblGroup7 + "," + 1 + "," + group8 + "," + tblSupplier + "," + tblcolor + "," + tblsize + "," + tblsizegroup + "," + 4 + "," + 1 + ","
                                + 0 + ",'" + style.RefStyleCode + "'," + date + "," + tblSeason + "," + barcodeprice + "," + style.RetailTargetCostPrice + "," + style.RetailTargetCostPrice + "," + style.RetailTargetCostPrice + ","
                                + retailBrand + "," + 0 + "," + 0 + "," + 1 + "," + "''" + "," + 0 + "," + 0 + "," + 0 + ",'" + subSeason + "' ,'" + referance + "' ," + cost + ",'" + specialfield2 + "')";
                        ccnewcontext.ExecuteStoreCommand(query);

                        ccnewcontext.TblMultipleBcs.AddObject(new TblMultipleBc
                        {
                            tblitem = maxiserial,
                            TblItemCode = style.RefStyleCode,
                            barcode = maxiserial.ToString(),
                            DefaultBarCode = 1
                        });
                        ccnewcontext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    var storetransrow = ccnewcontext.TBLSTORETRANS.FirstOrDefault(w => w.Tblitem == row.ISerial);
                    if (storetransrow == null)
                    {
                        if (cost != 0)
                        {
                            var query = "UPDATE  tblitem set ItemCost='" + (float?)cost + "' where iserial= '" + row.ISerial + "'";
                            ccnewcontext.ExecuteStoreCommand(query);
                        }
                     
                        row.Price2 = row.Price3 = row.Price4 = (decimal?)style.RetailTargetCostPrice;
                        row.Price1 = (decimal?)barcodeprice;

                        Cost = cost;
                    }
                    else
                    {
                        TransactionExist = true;
                    }
                    row.TblGroup8 = group8;
                    maxiserial = row.ISerial;
                    ccnewcontext.SaveChanges();
                }
            }
            return maxiserial;
        }
        private int CreateClasses(Brand brand, TblLkpBrandSection tblLkpBrandSection1, TblFamily family, TblSubFamily subFamily)
        {
            var class1Iserial = FindOrCreate("TblClass1", new GenericTable { Iserial = 0, Ename = brand.Brand_Ename, Code = brand.Brand_Code, Aname = brand.Brand_Ename });
            using (var context = new ccnewEntities())
            {
                var class2Code = brand.Brand_Code + tblLkpBrandSection1.Code;
                var class3Code = class2Code + family.Code;
                var class4Code = class3Code + subFamily.Code;

                var class2MaxIserial = GetMaxIserial("TblCLASS2");
                var class3MaxIserial = GetMaxIserial("TblCLASS3");
                var class4MaxIserial = GetMaxIserial("TblCLASS4");

                var class2Row = context.TblCLASS2.FirstOrDefault(x => x.Code == class2Code);

                if (class2Row == null)
                {
                    context.TblCLASS2.AddObject(new TblCLASS2
                    {
                        ISerial = class2MaxIserial,
                        Code = class2Code,
                        EName = tblLkpBrandSection1.Ename,
                        AName = tblLkpBrandSection1.Aname,
                        TblClass1 = class1Iserial,
                    });
                }
                else
                {
                    class2MaxIserial = class2Row.ISerial;
                }

                var class3Row = context.TblCLASS3.FirstOrDefault(x => x.Code == class3Code);

                if (class3Row == null)
                {
                    context.TblCLASS3.AddObject(new TblCLASS3
                    {
                        ISerial = class3MaxIserial,
                        Code = class3Code,
                        EName = family.Ename,
                        AName = family.Aname,
                        TblClass2 = class2MaxIserial,
                    });
                }
                else
                {
                    class3MaxIserial = class3Row.ISerial;
                }

                var class4Row = context.TblCLASS4.FirstOrDefault(x => x.Code == class4Code);

                if (class4Row == null)
                {
                    class4Row = (new TblCLASS4
                    {
                        ISerial = class4MaxIserial,
                        Code = class4Code,
                        Ename = subFamily.Ename,
                        Aname = subFamily.Aname,
                        TblClass3 = class3MaxIserial,
                    });
                    context.TblCLASS4.AddObject(class4Row);
                }

                context.SaveChanges();

                return class4Row.ISerial;
            }
        }
        public int GetMaxIserial(string tableName)
        {
            using (var context = new ccnewEntities())
            {
                var query = "SELECT max(Iserial) FROM dbo." + tableName;
                var maxIserial = context.ExecuteStoreQuery<int>(query);
                return maxIserial.FirstOrDefault() + 1;
            }
        }
        public int FindOrCreate(string tableName, GenericTable tableToInsert)
        {
            using (var context = new ccnewEntities())
            {
                try {
                    var query = "SELECT Code FROM dbo." + tableName + " Where Code ={0}";

                    var result = context.ExecuteStoreQuery<string>(query, tableToInsert.Code);  //<>("");
                    if(result != null)
                    foreach (var variable in result.ToList())
                    {
                        var newquery = "SELECT Max(Iserial) FROM dbo." + tableName + " Where Code ={0}";
                        return context.ExecuteStoreQuery<int>(newquery, tableToInsert.Code).FirstOrDefault();
                    }
                    var maxIserial = GetMaxIserial(tableName);

                    var insertQuery = "insert into dbo." + tableName +
                                      "(iserial,code,aname,ename) VALUES ({0},{1},{2},{3})";

                    context.ExecuteStoreCommand(insertQuery, maxIserial, tableToInsert.Code, tableToInsert.Aname, tableToInsert.Ename);
                    return maxIserial;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
               
            }
        }
    }
}