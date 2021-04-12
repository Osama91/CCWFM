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
        public List<TblRequestForSampleImage> GetTblRequestForSampleImage(int RequestForSample)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    return
                        context.TblRequestForSampleImages.Where(x => x.TblRequestForSample == RequestForSample).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        public int MaxIserialOfRequestForSampleImage(out string imagePath)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNo =
          context.TblRequestForSampleImages
              .Select(x => x.Iserial).Cast<int?>().Max();

                imagePath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "ImagesFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }
                return 1;
            }
        }

        [OperationContract]
        private List<TblRequestForSampleImage> UpdateOrInsertTblRequestForSampleImage(List<TblRequestForSampleImage> imageList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in imageList)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        context.TblRequestForSampleImages.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblRequestForSampleImages
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            var changedItems = GenericUpdate(oldRow, newRow, context);
                            if (changedItems.Count() > 1)
                            {
                                newRow.LastUpdatedDate = DateTime.Now;
                                GenericUpdate(oldRow, newRow, context);
                            }
                        }
                    }
                }

                context.SaveChanges();
                return imageList;
            }
        }

        [OperationContract]
        private int DeleteTblRequestForSampleImage(TblRequestForSampleImage row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblRequestForSampleImages
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}