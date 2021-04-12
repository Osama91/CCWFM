using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public void ChangePassword(string username, string newPassword, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var row = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);
                row.UserPassword = newPassword;
                row.UserName = username;
                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthUser> GetAllUsers(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblAuthUser> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblAuthUsers.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAuthUsers.Include("TblAuthJob").Include("TblCompany1").Include("TblCompany2").Include("TblPosition1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAuthUsers.Count();
                    query = context.TblAuthUsers.Include("TblAuthJob").Include("TblCompany1").Include("TblCompany2").Include("TblPosition1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        //public string GetSqlConnectionStringForUser(string dbName)
        //{
        //    var sqlBuilder = new SqlConnectionStringBuilder();
        //    TblCompany company;
        //    using (var context = new WorkFlowManagerDBEntities())
        //    {
        //        company = context.TblCompanies.FirstOrDefault(x => x.DbName == dbName);
        //    }
        //    // Set the properties for the data source.
        //    sqlBuilder.DataSource = company.Ip + company.Port;
        //    sqlBuilder.InitialCatalog = dbName;
        //    sqlBuilder.UserID = "Pts";
        //    sqlBuilder.Password = "2583094";
        //    sqlBuilder.IntegratedSecurity = false;

        //    // Build the SqlConnection connection string.
        //    var providerString = sqlBuilder.ToString();
        //    // Initialize the EntityConnectionStringBuilder.
        //    var entityBuilder = new EntityConnectionStringBuilder();

        //    //Set the provider name.
        //    entityBuilder.Provider = "System.Data.SqlClient";

        //    // Set the provider-specific connection string.
        //    entityBuilder.ProviderConnectionString = providerString;

        //    // Set the Metadata location.
        //    entityBuilder.Metadata = @"res://*/Model.CCNewEntities.csdl|res://*/Model.CCNewEntities.ssdl|res://*/Model.CCNewEntities.msl";

        //    return entityBuilder.ToString();
        //}

        [OperationContract]
        public List<EmployeesView> GetEmpTable(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new TimeAttEntities())
            {
                IQueryable<EmployeesView> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.EmployeesViews.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.EmployeesViews.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.EmployeesViews.Count();
                    query = context.EmployeesViews.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public TblAuthUser SaveUser(TblAuthUser newRow, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (newRow.Iserial != 0)
                {
                    var oldRow = (from s in context.TblAuthUsers
                                  where s.Iserial == newRow.Iserial
                                  select s).SingleOrDefault();
                    if (oldRow != null) GenericUpdate(oldRow, newRow, context);
                }

                if (newRow.Iserial == 0)
                {
                    context.TblAuthUsers.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int DeleteUser(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var user = context.TblAuthUsers.SingleOrDefault(x => x.Iserial == Iserial);

                if (user != null) context.DeleteObject(user);
                context.SaveChanges();
            }
            return Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblAuthJob> GetAllJobs()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAuthJobs.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<User> GetAxUser()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.Users.ToList();
            }
        }
    }
}