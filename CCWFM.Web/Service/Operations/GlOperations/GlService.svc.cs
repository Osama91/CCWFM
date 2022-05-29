using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CCWFM.Web.Model;
using System.Transactions;
using System.Timers;
using System;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode =ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public partial class GlService
    {

        [OperationContract]
        private void TestingMethod(string Company)
        {
            //using (var scope = new TransactionScope())
            //{
            //    using (var entity = new ccnewEntities(GetSqlConnectionString(Company)))
            //    {
            //        Timer NewTimer = new Timer();
            //        NewTimer.Enabled = true;
            //        NewTimer.Interval = 100;
            //        NewTimer.Start();
            //        int i = 0;
            //        NewTimer.Elapsed += (s, e) =>
            //        {
            //            {
            //                if (DateTime.Now.Hour >=   15  )
            //                {
            //                    NewTimer.Close();
            //                }

            //                i++;
            //                var newrow = new TableTest();
            //                newrow.Count = i;
            //                entity.SaveChanges();
            //            };

            //        };               
            //    }
            //    scope.Complete();
            //}
        }
        
        

        private List<ObjectParameter> ConvertToParamters(Dictionary<string, object> valuesObjects)
        {
            return valuesObjects.Select(valuesObject => new ObjectParameter(valuesObject.Key, valuesObject.Value)).ToList();
        }

        public string GetSqlConnectionString(string dbName)
        {
            var sqlBuilder = new SqlConnectionStringBuilder();

            TblCompany company = GetCompany(dbName);
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

       static List<TblCompany> Companies = new List<TblCompany>();

        //private static TblCompany GetCompany(string dbName)
        //{

        //    TblCompany company;
        //    if (Companies.Count>0)
        //    {
        //        company = Companies.FirstOrDefault(x => x.DbName.ToLowerInvariant() == dbName.ToLowerInvariant());
        //        return company;
        //    }

        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        company = context.TblCompanies.FirstOrDefault(x => x.DbName.ToLowerInvariant() == dbName.ToLowerInvariant());
        //        Companies = context.TblCompanies.ToList();
        //    }

        //    return company;
        //}

        private static TblCompany GetCompany(string dbName)
        {

            TblCompany company;

            if (Companies.Count == 0)
            {
                using (var context = new WorkFlowManagerDBEntities())
                {

                    Companies = context.TblCompanies.ToList();
                }
            }


            //if (Companies.Count > 0)
            //{
            company = Companies.FirstOrDefault(x => x.DbName.ToLowerInvariant() == dbName.ToLowerInvariant());
            return company;
            //}



            //return company;
        }


        private IEnumerable<string> GenericUpdate<T>(T oldValues, T newValues, ObjectContext entities)
        {
            var orgRow = entities.ObjectStateManager.GetObjectStateEntry(oldValues);
            orgRow.ApplyCurrentValues(newValues);

            entities.SaveChanges();
            var modifiedProperties = orgRow.GetModifiedProperties();
            return modifiedProperties;
        }

        //[OperationContract]
        //public void ViewReport(string reportname, List<string> reportPara, string company)
        //{
        //    PublicReportName = reportname;
        //    PublicReportPara = reportPara;
        //    var reportserver = "";
        //    var Con = new TblConnection();
        //    using (var context = new HREntities(GetSqlConnectionString(company, out Con)))
        //    {
        //        if (context.TblAuthPermissions.Any(x => x.Code == reportname))
        //        {
        //            reportserver = context.TblAuthPermissions.FirstOrDefault(x => x.Code == reportname).ReportServer;
        //        }
        //        if (string.IsNullOrEmpty(reportserver))
        //        {
        //            reportserver = context.tblChainSetups.SingleOrDefault(x => x.sGlobalSettingCode == "ReportServer").sSetupValue;
        //        }
        //    }
        //    PublicReportPara = reportPara;

        //    if (PublicReportPara == null)
        //    {
        //        PublicReportPara = new List<string>();

        //    }
        //    PublicReportPara.Add(Con.ServerIp);
        //    PublicReportPara.Add(Con.Ip);
        //    PublicReportPara.Add(Con.DatabaseName);
        //    ReportServer = reportserver;
        //}

        static public string PublicReportName { get; set; }

        static public string ReportServer { get; set; }

        static public List<string> PublicReportPara { get; set; }
    }
}