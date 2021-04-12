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
        public void AddFabricGalary(List<tbl_FabricImage> _ObjToAdd)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    foreach (var item in _ObjToAdd)
                    {
                        context.tbl_FabricImage.AddObject(item);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error, Galary was not Saved!\nError Details: " + ex.Message);
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateFabricGalary(List<tbl_FabricImage> _ObjToUpdate)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    foreach (var item in _ObjToUpdate)
                    {
                        var temp = (from x in context.tbl_FabricImage
                                    where x.Iserial == (int)item.Iserial
                                    select x).SingleOrDefault();

                        temp.FabImage = item.FabImage;
                        temp.FabricCode = item.FabricCode;
                        temp.ImageDesc = item.ImageDesc;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error, Galary was not Saved!\nError Details: " + ex.Message);
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_FabricImage> GetFabricImagesByFabric(string _FabricCode)
        {
            try
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var temp = context.tbl_FabricImage.Where(x => x.FabricCode == _FabricCode).ToList();
                    return temp;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error trying to retrieve the data!\nError Details: " + ex.Message);
            }
        }
    }
}