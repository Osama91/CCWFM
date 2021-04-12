using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleImage> GetTblStyleImage(int style)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    return
                        context.TblStyleImages.Include("TblColor1").Include("TblStyle1").Where(x => x.TblStyle == style).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        public int MaxIserialOfStyle(out string imagePath)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNo =
               context.TblStyleImages
              .Select(x => x.Iserial).Cast<int?>().Max();

                imagePath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "ImagesFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }
                return 1;
            }
        }

        [OperationContract]
        private List<TblStyleImage> UpdateOrInsertTblStyleImage(List<TblStyleImage> imageList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var newRow in imageList)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        context.TblStyleImages.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblStyleImages
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
        private int DeleteTblStyleImage(TblStyleImage row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStyleImages
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        public byte[] DownloadChunk(String docUrl, Int64 offset, Int32 bufferSize)
        {
            var filePath = HttpContext.Current.Server.MapPath(docUrl);
            if (!File.Exists(filePath))
                return null;
            var fileSize = new FileInfo(filePath).Length;
            //// if the requested Offset is larger than the file, quit.
            if (offset > fileSize)
            {
                //SecurityService.logger.Fatal("Invalid Download Offset - " + String.Format("The file size is {0}, received request for offset {1}", FileSize, Offset));
                return null;
            }
            // open the file to return the requested chunk as a byte[]
            try
            {
                int bytesRead;
                byte[] tmpBuffer;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);	// this is relevent during a retry. otherwise, it just seeks to the start
                    tmpBuffer = new byte[bufferSize];
                    bytesRead = fs.Read(tmpBuffer, 0, bufferSize);	// read the first chunk in the buffer (which is re-used for every chunk)
                }
                if (bytesRead != bufferSize)
                {
                    // the last chunk will almost certainly not fill the buffer, so it must be trimmed before returning
                    var trimmedBuffer = new byte[bytesRead];
                    Array.Copy(tmpBuffer, trimmedBuffer, bytesRead);
                    return trimmedBuffer;
                }
                else
                    return tmpBuffer;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}