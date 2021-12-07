using CCWFM.Web.Model;
using CCWFM.Web.Service.AssistanceOp;
using CCWFM.Web.Service.Operations.GlOperations;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace CCWFM.Web.Service.Operations
{
    static internal class SharedOperation
    {
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public static Process GetExcelProcess(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            return Process.GetProcessById(id);
        }
        internal static string ReportServer = "http://WebSrv/ReportServer/";
        static internal T Clone<T>(this T Entity) where T:EntityObject
        {
            var Type = Entity.GetType();
            var Clone = Activator.CreateInstance(Type);

            foreach (var Property in Type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty))
            {
                if (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)) continue;
                if (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)) continue;
                if (Property.PropertyType.IsSubclassOf(typeof(EntityObject))) continue;

                if (Property.CanWrite)
                {
                    Property.SetValue(Clone, Property.GetValue(Entity, null), null);
                }
            }

            return (T)Clone;
        }
        static internal float GetTradeAgrementPrice(int Season, int item, string itemType, string vendor, int color, DateTime fromDate, out string Currency, out string Vendor, out int leadtime, out float BasicPrice)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var ShortSeasonCode = entities.TblLkpSeasons.FirstOrDefault(w => w.Iserial == Season).ShortCode;

                var tradeAgreement =
                    entities.TblTradeAgreementDetails.Include("TblTradeAgreementHeader1").Where(
                        x =>
                        x.TblTradeAgreementHeader1.TblTradeAgreementTransaction1.TblLkpSeason1.ShortCode== ShortSeasonCode&&
                            x.ItemCode == item && x.ItemType == itemType &&
                            (x.TblTradeAgreementHeader1.Vendor == vendor || vendor == null)
                            && x.TblTradeAgreementHeader1.TblTradeAgreementTransaction1.FromDate <= fromDate && x.TblTradeAgreementHeader1.TblTradeAgreementTransaction1.ToDate >= fromDate
                            ).OrderByDescending(x => x.Iserial).ThenBy(x => x.Price);
                if (tradeAgreement.Any())
                {
                    var tradeAgreementColor = tradeAgreement.FirstOrDefault(x => x.TblColor == color);

                    if (tradeAgreementColor != null)
                    {
                        Currency = tradeAgreementColor.CurrencyCode;
                        Vendor = tradeAgreementColor.TblTradeAgreementHeader1.Vendor;
                        leadtime = tradeAgreementColor.Days ?? 0;
                        BasicPrice = tradeAgreementColor.Price;
                        return (float)(tradeAgreementColor.Price + (tradeAgreementColor.Price * ((tradeAgreementColor.CustomsPercentage + tradeAgreementColor.ShippingPercentage + tradeAgreementColor.SalesPercentage) / 100)));
                    }
                    else
                    {
                        var row = tradeAgreement.FirstOrDefault();
                        if (row.TblColor ==null|| row.TblColor==0)
                        {
                            Currency = row.CurrencyCode;
                            leadtime = row.Days ?? 0;
                            Vendor = row.TblTradeAgreementHeader1.Vendor;
                            BasicPrice = row.Price;
                            return (float)(row.Price + (row.Price * ((row.CustomsPercentage + row.ShippingPercentage + row.SalesPercentage) / 100)));
                        }                   
                    }
                }
                leadtime = 0;
                Vendor = "";
                Currency = "EGP";
                BasicPrice = 0;
                return 0;
            }
        }
        static internal string HandelSequence(TblSequenceProduction journal)
        {
            var temp = "";
            var tempFormat = "";
            const char aa = '0';
            using (var entity = new WorkFlowManagerDBEntities())
            {
                for (var i = 0; i < journal.NumberOfInt; i++)
                {
                    tempFormat = tempFormat + aa;
                }
                var tempno = journal.NextRec;
                temp = tempno.ToString(tempFormat) + journal.Format;
                var seq = entity.TblSequenceProductions.FirstOrDefault(x => x.Iserial == journal.Iserial);
                if (seq != null) seq.NextRec = seq.NextRec + 1;
                entity.SaveChanges();
            }
            return temp;
        }
        static internal string HandelSequence(DataLayer.TblSequenceProduction journal)
        {
            var temp = "";
            var tempFormat = "";
            const char aa = '0';
            using (var entity = new WorkFlowManagerDBEntities())
            {
                for (var i = 0; i < journal.NumberOfInt; i++)
                {
                    tempFormat = tempFormat + aa;
                }
                var tempno = journal.NextRec;
                temp = tempno.ToString(tempFormat) + journal.Format;
                var seq = entity.TblSequenceProductions.FirstOrDefault(x => x.Iserial == journal.Iserial);
                if (seq != null) seq.NextRec = seq.NextRec + 1;
                entity.SaveChanges();
            }
            return temp;
        }
        static internal string HandelSequence(TblSequence journal, string company, int no, int month, int year)
        {
            int seqq = 0;
            return HandelSequence(string.Empty, journal, "", company, no, month, year, out seqq);
        }
        static internal string HandelSequence(string code, TblSequence journal, string table, string company, int no, int month, int year, out int seqq)
        {
            seqq = 0;
            var temp = "";
            var tempFormat = "";
            const char aa = '0';
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                for (var i = 0; i < journal.NumberOfInt; i++)
                {
                    tempFormat = tempFormat + aa;
                }
                var tempno = journal.NextRec + no;
                temp = tempno.ToString(tempFormat) + journal.Format;
                var seq = entity.TblSequences.FirstOrDefault(x => x.Iserial == journal.Iserial);

                if (seq != null) seq.NextRec = seq.NextRec + 1 + no;

                entity.SaveChanges();
            }
            return temp;
        }

        static internal List<ObjectParameter> ConvertToParamters(Dictionary<string, object> valuesObjects)
        {
            return valuesObjects.Select(valuesObject => new ObjectParameter(valuesObject.Key, valuesObject.Value)).ToList();
        }
        static internal IEnumerable<string> GenericUpdate<T>(T oldValues, T newValues, ObjectContext entities)
        {
            var orgRow = entities.ObjectStateManager.GetObjectStateEntry(oldValues);
            orgRow.ApplyCurrentValues(newValues);
            var modifiedProperties = orgRow.GetModifiedProperties();

            return modifiedProperties;
        }
        static internal bool isSample(string SalesOrderCode)
        {
            if (SalesOrderCode.Contains("-S-"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static List<tblChainSetup> ChainSetupList = new List<tblChainSetup>();
        static internal string GetChainSetup(string Code)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {

                    return context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == Code).sSetupValue;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        static internal bool UseAx()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var Return = true;
                var query = context.tblChainSetups.Where(X => X.sGlobalSettingCode == "UseAx");

                if (query.Any())
                {
                    if (query.FirstOrDefault().sSetupValue == "0")
                    {
                        Return = false;
                    }
                }
                return Return;
            }
        }

        static internal string GetSqlConnectionString(string dbName)
        {
            var sqlBuilder = new SqlConnectionStringBuilder();

            TblCompany company;
            using (var context = new WorkFlowManagerDBEntities())
            {
                company = context.TblCompanies.FirstOrDefault(x => x.DbName == dbName);
            }
            // Set the properties for the data source.
              if (company != null) sqlBuilder.DataSource = company.Ip + company.Port;
            //if (company != null) sqlBuilder.DataSource = "192.168.11.15" + company.Port;
            sqlBuilder.InitialCatalog = company.DbName;
            sqlBuilder.UserID = "Pts";
            sqlBuilder.Password = "2583094";
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ConnectTimeout = 0;
            // Build the SqlConnection connection string.
            var providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            var entityBuilder = new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = "System.Data.SqlClient";

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/Model.CCNewEntities.csdl|res://*/Model.CCNewEntities.ssdl|res://*/Model.CCNewEntities.msl";

            return entityBuilder.ToString();
        }
        static internal void CalcEstimatedBom(TblSalesOrder row)
        {
            using (var model = new WorkFlowManagerDBEntities())
            {
                var bom = model.BOMs.Include("TblBOMSizes").Include("TblBOMStyleColors").Where(w => w.TblSalesOrder == row.Iserial);
                foreach (var bomLine in bom.ToList())
                {
                    float price = 0;
                    var temp = model.TblTradeAgreementDetails.Where(x => x.ItemCode == bomLine.BOM_Fabric);
                    if (temp.Any())
                    {
                        price = temp.FirstOrDefault().Price;
                    }
                    foreach (var bomsize in bomLine.TblBOMSizes.ToList())
                    {
                        foreach (var bomColor in bomLine.TblBOMStyleColors.ToList())
                        {
                            var tblsalesordercolor =
                                model.TblSalesOrderColors.Include("TblSalesOrderSizeRatios").FirstOrDefault(
                                    x => x.TblSalesOrder == row.Iserial && x.TblColor == bomColor.StyleColor);
                            if (temp.Any(x => x.TblColor == bomColor.FabricColor))
                            {
                                price = temp.FirstOrDefault().Price;
                            }
                            if (temp.Any(x => x.TblColor == bomColor.FabricColor && x.AccSize == bomsize.FabricSize))
                            {
                                price = temp.FirstOrDefault().Price;
                            }
                            var bomprice = new BomCost
                            {
                                Bom = bomLine.Iserial,
                                BOM_Fabric = (int)bomLine.BOM_Fabric,
                                BOM_FabricType = bomLine.BOM_FabricType,
                                StyleSize = bomsize.StyleSize,
                                FabricSize = bomsize.FabricSize,
                                MaterialUsage = bomsize.MaterialUsage,
                                StyleColor = bomColor.StyleColor,
                                FabricColor = bomColor.FabricColor,
                                ProductionPerSize = tblsalesordercolor.TblSalesOrderSizeRatios.FirstOrDefault(x => x.Size == bomsize.StyleSize).ProductionPerSize,
                                Price = price,
                            };
                            model.BomCosts.AddObject(bomprice);
                        }
                    }
                }
                model.SaveChanges();
            }
        }
        static internal int GetUserJob(int _UserID, string _UserName)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return (from x in context.TblAuthJobs
                        where x.Iserial == (from z in context.TblAuthUsers
                                            where z.Iserial == _UserID
                                            select z.TblJob).FirstOrDefault()
                        select x.Iserial).FirstOrDefault();
            }
        }
        static internal int GetMaxRouteCardTransactionID(int routeGroupId, int direction, int tblTransactionType)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var x = 0;
                if (context.sp_GetMaxRouteCardTransID(direction, routeGroupId, tblTransactionType).Any())
                {
                    x = (int)context.sp_GetMaxRouteCardTransID(direction, routeGroupId, tblTransactionType).ToList()[0];
                }
                return x;
            }
        }
        static internal void ClearAxTable(string tableName, Microsoft.Dynamics.BusinessConnectorNet.Axapta axapta, string transactionGuid)
        {
            var axRecord = axapta.CreateAxaptaRecord(tableName);
            axRecord.Clear();
            axRecord.InitValue();

            using (axRecord = axapta.CreateAxaptaRecord(tableName))
            {
                // Execute a query to retrieve an editable record where the StatGroupName is "High Priority Customer”.

                var query = "select forupdate * from %1 where %1.TransactionGuid == '" + transactionGuid + "'";
                //    " %1.WORKFLOWJOURID ==  " + iserial + "";
                axRecord.ExecuteStmt(query);
                // If the record is found then delete the record.
                if (axRecord.Found)
                {
                    // Start a transaction that can be committed.
                    axapta.TTSBegin();
                    axRecord.Delete();
                    // Commit the transaction.
                    axapta.TTSCommit();
                    axRecord.Next();
                }
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        static List<TblCompany> Companies = new List<TblCompany>();

        private static TblCompany GetCompany(string dbName)
        {

            TblCompany company;
            if (Companies.Count > 0)
            {
                company = Companies.FirstOrDefault(x => x.DbName == dbName);
                return company;
            }

            using (var context = new WorkFlowManagerDBEntities())
            {
                company = context.TblCompanies.FirstOrDefault(x => x.DbName == dbName);
                Companies = context.TblCompanies.ToList();
            }

            return company;
        }


        public static void SendMailReport(string reportName, string reportTitle,
                  List<ParameterValue> parameters, string company,
                  string emailTo, string Subject, string body)
        {
            string deviceInfo = null;
            string extension = string.Empty;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            Warning[] warnings = null;
            string[] streamIDs = null;
            string historyId = null;

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

            // Create a Report Execution object
            var rsExec = new ReportExecutionService()
            {
                ExecutionHeaderValue = new ExecutionHeader(),
                Timeout = Timeout.Infinite
            };
            rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;
            rsExec.Url = ReportServer + "/ReportExecution2005.asmx";

            // Load the report
            ExecutionInfo execInfo = rsExec.LoadReport("/Report/" + reportName, historyId);
            TblCompany companyRow=GetCompany(company);

            //TblCompany companyRow;
            //using (var context = new WorkFlowManagerDBEntities())
            //{
            //       companyRow = context.TblCompanies.FirstOrDefault(x => x.DbName == company);
            //}
            string Ip = companyRow.Ip+ companyRow.Port;
            foreach (var item in execInfo.Parameters)
            {
                try
                {
                    if (item.Name == "Ip" && !string.IsNullOrWhiteSpace(Ip))
                    {
                        parameters.Add(new ParameterValue() { Name = item.Name, Value = Ip });
                        continue;
                    }
                    if (item.Name == "Database" && !string.IsNullOrWhiteSpace(company))
                    {
                        parameters.Add(new ParameterValue() { Name = item.Name, Value = company });
                        continue;
                    }
                    //parameters.Add(new Microsoft.Reporting.WebForms.ReportParameter(item.Name, para[count]));
                }
                catch (Exception) { }
            }

            rsExec.SetExecutionParameters(parameters.ToArray(), "en-us");

            // get pdf of report
            byte[] results = rsExec.Render("PDF", deviceInfo,
            out extension, out encoding,
            out mimeType, out warnings, out streamIDs);

            //Walla...almost no code, it's easy to manage and your done.

            //Take the bytes and add as an attachment to a MailMessage(SMTP):

            var attach = new Attachment(new MemoryStream(results), string.Format("{0}.pdf", reportTitle));

            string emailFrom;
            var service = new GlOperations.GlService();

            try
            {
                emailFrom = service.GetRetailChainSetupByCode("CashDepositeFromMail", company).sSetupValue;
            }
            catch (Exception ex)
            {
                new AssistanceService().SaveLog(JsonConvert.SerializeObject(ex), 0);
                emailFrom = "Retail@ccausal.loc";
            }

            SendEmail(attach, emailFrom, emailTo.Split(';').ToList(), Subject, body);
        }


        //********************************************************************************
        public static bool SendEmail(Attachment msg, string emailfrom, List<string> emailTo, string subject, string body)
        {
            //********************************************************************************
            try
            {
                var msga = new MailMessage { Subject = subject, Body = body };
                if (msg != null) msga.Attachments.Add(msg);
                foreach (var variable in emailTo)
                {
                    msga.To.Add(variable);
                }
                msga.From = new MailAddress(emailfrom);
                msga.IsBodyHtml = true;
                var smtp = new SmtpClient("192.168.1.5");
                smtp.Send(msga);
            }
            catch (Exception ex)
            {
                new AssistanceService().SaveLog(JsonConvert.SerializeObject(ex), 0);
                return false;
            }
            return true;
        }

        public static string GetBrandPerUser(string filter, Dictionary<string, object> valuesObjects, int userIserial, WorkFlowManagerDBEntities context)
        {
            var brandTempQuery = context.TblUserBrandSections.Where(x => x.TblAuthUser == userIserial).ToList();
            var brandSectionQuery = brandTempQuery.Select(x => x.TblLkpBrandSection).Distinct();
            var brandQuery = brandTempQuery.Select(x => x.BrandCode).Distinct();

            if (brandQuery.Any())
            {
                filter = filter + " and (";
            }

            int brandcount = 0;
            foreach (var brand in brandQuery)
            {
                brandcount++;
                if (brand == brandQuery.FirstOrDefault())
                {
                    filter = filter + " it.Brand ==(@b)";
                    valuesObjects.Add("b", brand);
                }
                if (brandcount == 1)
                {
                    if (brand != brandQuery.FirstOrDefault() && brand != brandQuery.LastOrDefault())
                    {
                        filter = filter + " or it.Brand ==(@bw)";
                        valuesObjects.Add("bw", brand);
                    }
                }

                if (brandcount == 2)
                {
                    if (brand != brandQuery.FirstOrDefault() && brand != brandQuery.LastOrDefault())
                    {
                        filter = filter + " or it.Brand ==(@bzz)";
                        valuesObjects.Add("bzz", brand);
                    }
                }
                if (brandcount == 3)
                {
                    if (brand != brandQuery.FirstOrDefault() && brand != brandQuery.LastOrDefault())
                    {
                        filter = filter + " or it.Brand ==(@bwz)";
                        valuesObjects.Add("bwz", brand);
                    }
                }
                if (brandcount == 4)
                {
                    if (brand != brandQuery.FirstOrDefault() && brand != brandQuery.LastOrDefault())
                    {
                        filter = filter + " or it.Brand ==(@bzzzqweqeqeq)";
                        valuesObjects.Add("bzzzqweqeqeq", brand);
                    }
                }
                if (brandcount == 5)
                {
                    if (brand == brandQuery.LastOrDefault() && brand != brandQuery.FirstOrDefault())
                    {
                        filter = filter + " or it.Brand ==(@bl)";
                        valuesObjects.Add("bl", brand);
                    }
                }
                if (brand == brandQuery.LastOrDefault() && brand != brandQuery.FirstOrDefault())
                {
                    filter = filter + " or it.Brand ==(@blllllll)";
                    valuesObjects.Add("blllllll", brand);
                }
                if (brand == brandQuery.LastOrDefault())
                {
                    filter = filter + ")";
                }
            }

            if (brandSectionQuery.Any())
            {
                filter = filter + " and (";
            }
            int brandSectioncount = 0;
            foreach (var brandSection in brandSectionQuery)
            {
                brandSectioncount++;
                if (brandSection == brandSectionQuery.FirstOrDefault())
                {
                    filter = filter + " it.TblLkpBrandSection ==(@bs)";
                    valuesObjects.Add("bs", brandSection);
                }
                if (brandSectioncount == 1)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@br)";
                        valuesObjects.Add("br", brandSection);
                    }
                }

                if (brandSectioncount == 2)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bm)";
                        valuesObjects.Add("bm", brandSection);
                    }
                }

                if (brandSectioncount == 3)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bo)";
                        valuesObjects.Add("bo", brandSection);
                    }
                }

                if (brandSectioncount == 4)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@ba)";
                        valuesObjects.Add("ba", brandSection);
                    }
                }

                if (brandSectioncount == 5)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bz)";
                        valuesObjects.Add("bz", brandSection);
                    }
                }

                if (brandSectioncount == 6)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bzzeerrr)";
                        valuesObjects.Add("bzzeerrr", brandSection);
                    }
                }

                if (brandSectioncount == 7)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bzzz)";
                        valuesObjects.Add("bzzz", brandSection);
                    }
                }

                if (brandSectioncount == 8)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bzzzz)";
                        valuesObjects.Add("bzzzz", brandSection);
                    }
                }

                if (brandSectioncount == 9)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bzzzzaaaa)";
                        valuesObjects.Add("bzzzzaaaa", brandSection);
                    }
                }

                if (brandSectioncount == 10)
                {
                    if (brandSection != brandSectionQuery.FirstOrDefault() &&
                        brandSection != brandSectionQuery.LastOrDefault())
                    {
                        filter = filter + " or it.TblLkpBrandSection ==(@bzzzzaaaadd)";
                        valuesObjects.Add("bzzzzaaaadd", brandSection);
                    }
                }

                if (brandSection == brandSectionQuery.LastOrDefault() &&
                    brandSection != brandSectionQuery.FirstOrDefault())
                {
                    filter = filter + " or it.TblLkpBrandSection ==(@bsl)";
                    valuesObjects.Add("bsl", brandSection);
                }
                if (brandSection == brandSectionQuery.LastOrDefault())
                {
                    filter = filter + ")";
                }
            }

            return filter;
        }

    }
}