using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<GenericTable> GetGeneric(string tablEname, string code, string Ename, string Aname, string sort, string direction, string company)
        {
            var sqlParam = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Table_Name",
                    Value = tablEname,
                    SqlDbType = SqlDbType.NVarChar
                },

                new SqlParameter
                {
                    ParameterName = "Code",
                    Value = code,
                    SqlDbType = SqlDbType.NVarChar
                },
                new SqlParameter
                {
                    ParameterName = "Ename",
                    Value = Ename,
                    SqlDbType = SqlDbType.NVarChar
                },
                new SqlParameter
                {
                    ParameterName = "Aname",
                    Value = Aname,
                    SqlDbType = SqlDbType.NVarChar
                },
                 new SqlParameter
                {
                    ParameterName = "Sort",
                    Value = sort,
                    SqlDbType = SqlDbType.NVarChar
                },
                   new SqlParameter
                {
                    ParameterName = "Direction",
                    Value = direction,
                    SqlDbType = SqlDbType.NVarChar
                }
            };

            var temp = GetGenericFiltered(sqlParam.ToArray(), company).ToList();
            return temp;
        }

        private IEnumerable<GenericTable> GetGenericFiltered(SqlParameter[] param, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return context.ExecuteStoreQuery<GenericTable>("exec sp_GetGenericTableData @Table_Name, @Code, @Ename, @Aname,@Sort,@Direction", param).ToList<GenericTable>();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int DeleteGeneric(string tablEname, string iserial, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                context.sp_GenericDelete(tablEname, iserial);
                context.SaveChanges();
            }

            return Convert.ToInt32(iserial);
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public GenericTable GenericUpdateOrInsert(string tablEname, GenericTable item, int index, out int outindex, string company)
        {
            outindex = index;

            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (item.Iserial == 0)
                {
                    context.sp_GenericInsertIntoTable(tablEname, item.Code, item.Ename, item.Aname.Normalize());
                }
                else
                {
                    context.sp_GenericUpdateTable(tablEname, item.Iserial.ToString(), item.Code, item.Ename, item.Aname.Normalize());
                }
                context.SaveChanges();

                return GetGeneric(tablEname, item.Code, item.Ename, item.Aname, "Iserial", "Asc", company).FirstOrDefault();
            }
        }
    }
}