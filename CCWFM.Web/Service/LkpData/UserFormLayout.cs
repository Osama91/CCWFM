using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
    
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tblFormLayout> GettblFormLayoutDefault(string FormID)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var formLayoutQuery = context.tblFormLayouts.Where(x => x.ID == FormID) ;
                return formLayoutQuery.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tblFormLayoutUser> GettblFormLayoutByUser(string FormID, int tblUser)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tblFormLayoutUsers.Include("tblFormLayout1").Where(x => x.tblUser == tblUser && x.tblFormLayout1.ID == FormID).OrderBy(x=>x.ColOrder);
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private bool UpdateOrDeletetblFormLayoutUsers(int User,string FormCode, List<tblFormLayoutUser> UserLayouts)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var Query = context.tblFormLayoutUsers.Where(x => x.tblUser == User && x.tblFormLayout1.ID == FormCode);
                //Delete 
                foreach (var item in Query)
                {
                    context.tblFormLayoutUsers.DeleteObject(item);
                }
                //Save
                foreach (var newItem in UserLayouts)
                {
                    context.tblFormLayoutUsers.AddObject(newItem);
                }
                context.SaveChanges();
            }
            return false;
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TBLsupplier> GetTBLsupplier(string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = context.TBLsuppliers.ToList();
                return query;
            }
        }

        public string GetSqlConnectionString(string dbName)
        {
            var sqlBuilder = new SqlConnectionStringBuilder();

            TblCompany company;
            using (var context = new WorkFlowManagerDBEntities())
            {
                company = context.TblCompanies.FirstOrDefault(x => x.DbName == dbName);
            }
            // Set the properties for the data source.
            if (company != null) sqlBuilder.DataSource = company.Ip + company.Port;
            sqlBuilder.InitialCatalog = dbName;
            sqlBuilder.UserID = "Pts";
            sqlBuilder.Password = "2583094";
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.MultipleActiveResultSets = true;
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
    }
}