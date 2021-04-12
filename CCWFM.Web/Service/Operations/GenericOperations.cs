using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<GenericTable> GetGeneric(string tablEname, string code, string Ename, string Aname, string sort, string direction)
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

            var temp = GetGenericFiltered(sqlParam.ToArray()).ToList();
            return temp;
        }

        private IEnumerable<GenericTable> GetGenericFiltered(SqlParameter[] param)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.ExecuteStoreQuery<GenericTable>("exec sp_GetGenericTableData @Table_Name, @Code, @Ename, @Aname,@Sort,@Direction", param).ToList<GenericTable>();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public int DeleteGeneric(string tablEname, string iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                context.sp_GenericDelete(tablEname, iserial);
                context.SaveChanges();
            }

            return Convert.ToInt32(iserial);
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public GenericTable GenericUpdateOrInsert(string tablEname, GenericTable item, int index, out int outindex)
        {
            outindex = index;

            using (var context = new WorkFlowManagerDBEntities())
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

                return GetGeneric(tablEname, item.Code, item.Ename, item.Aname, "Iserial", "Asc").FirstOrDefault();
            }
        }
    }

    [DataContract]
    public class GenericTable
    {
        [DataMember]
        public int Iserial { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Aname { get; set; }

        [DataMember]
        public string Ename { get; set; }

        public GenericTable()
        {
        }

        public GenericTable(int iserial, string code, string aname, string ename)
        {
            Iserial = iserial;
            Code = code;
            Aname = aname;
            Ename = ename;
        }
    }

    [DataContract]
    public class RecInvDataTable
    {
        [DataMember]
        public int Iserial { get; set; }
        [DataMember]
        public string Style { get; set; }

        [DataMember]
        public string ColorCode { get; set; }

        [DataMember]
        public string ColorName { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public decimal Quantity { get; set; }

        [DataMember]
        public string SizeCode { get; set; }

        [DataMember]
        public int Currency { get; set; }

        [DataMember]
        public  int TblColor { get; set; }
        [DataMember]
        public string BatchNo { get; set; }
    }
}