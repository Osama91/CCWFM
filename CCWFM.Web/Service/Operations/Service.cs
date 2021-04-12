using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using Microsoft.Dynamics.BusinessConnectorNet;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblService> GetTblService(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblService> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblServices.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblServices.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblServices.Count();
                    query = context.TblServices.OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        public void InsertServiceItem(TblService objToPost, int userIserial)
        {
            var axapta = new Axapta();//Ready To be Dependent from Ax
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var credential = new NetworkCredential("bcproxy", "around1");
                    var user = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == userIserial);

                    axapta.LogonAs(user.User_Win_Login, user.User_Domain, credential, "Ccm", null, null, null);

                    axapta.TTSBegin();
                    var itemId = objToPost.Code;
                    var tableName = "InventTable";

                    var axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemGroupId", objToPost.ServiceGroup);

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ItemName", objToPost.Ename);
                    axaptaRecord.set_Field("ModelGroupID", "Service");
                    axaptaRecord.set_Field("ItemType", 0);
                    axaptaRecord.set_Field("DimGroupId", "Service");
                    //Commit the record to the database.
                    axaptaRecord.Insert();

                    tableName = "InventTableModule";
                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 0);
                    axaptaRecord.set_Field("Price", 0.00);

                    axaptaRecord.set_Field("UnitId", "KG");
                    //axaptaRecord.set_Field("PriceUnit", tempu);

                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 1);
                    axaptaRecord.set_Field("Price", 0.00);
                    axaptaRecord.set_Field("UnitId", "KG");

                    axaptaRecord.Insert();

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("ModuleType", 2);
                    axaptaRecord.set_Field("Price", 0.00);
                    axaptaRecord.set_Field("UnitId", "KG");
                    axaptaRecord.set_Field("UnitId", "KG");

                    axaptaRecord.Insert();

                    tableName = "InventItemLocation";

                    axaptaRecord = axapta.CreateAxaptaRecord(tableName);
                    axaptaRecord.Clear();
                    axaptaRecord.InitValue();

                    axaptaRecord.set_Field("ItemId", itemId);
                    axaptaRecord.set_Field("InventDIMID", "AllBlank");
                    axaptaRecord.Insert();

                    // Commit the record to the database.
                    axapta.TTSCommit();
                }
            }
            catch (Exception ex)
            {
                axapta.TTSAbort();
                throw ex;
            }
        }

        [OperationContract]
        private TblService UpdateOrInsertTblService(TblService newRow, bool save, int index, int user, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblServices.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblServices
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                if (SharedOperation.UseAx())
                {


                    if (save)
                    {
                        InsertServiceItem(newRow, user);
                    }
                }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblService(TblService row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblServices
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }


    }
}