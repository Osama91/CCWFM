using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void AddAccType(tbl_lkp_AccessoryItemType _ObjectToAdd)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (!context.tbl_lkp_AccessoryItemType.Contains(_ObjectToAdd))
                    {
                        context.tbl_lkp_AccessoryItemType.AddObject(_ObjectToAdd);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteAccType(tbl_lkp_AccessoryItemType _ObjToDel)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    context.tbl_lkp_AccessoryItemType.DeleteObject(_ObjToDel);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_lkp_AccessoryItemType GetAccType(int _Iserial)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    return context.tbl_lkp_AccessoryItemType.SingleOrDefault(x => x.Iserial == _Iserial);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_lkp_AccessoryItemType> GetAllAccTypes()
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    return context.tbl_lkp_AccessoryItemType.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}