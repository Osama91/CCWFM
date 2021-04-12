using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace CCWFM.Web.Service.AssistanceOp
{
    public partial class AssistanceService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        public List<TblAttachment> GetAttachment(string TableName,int MasterId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblAttachments.Where(a =>
                a.TableName == TableName && a.RecordId == MasterId).ToList();
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        private int DeleteAttachment(TblAttachment row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var temp = context.TblAttachments.FirstOrDefault(ah => ah.Iserial == row.Iserial);
                if (temp != null)
                {
                    context.DeleteObject(temp);
                }
                if (context.SaveChanges() > 0)
                {
                    if (File.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath+temp.Path))
                        File.Delete(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + temp.Path);
                }
            }
            return row.Iserial;
        }


        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        private List<TblAttachment> UpdateOrInsertAttachment(List<TblAttachment> attachments,
            out List<Tuple<int,string>> attachmentPath,out string FolderPath)
        {
            attachmentPath = new List<Tuple<int, string>>();
            using (var context = new WorkFlowManagerDBEntities())
            {
                var relativePath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "AttachmentsDefaultFolderPath").sSetupValue;
                FolderPath = relativePath;
                var v = GetPath(context);
                foreach (var newRow in attachments)
                {
                    if (newRow.Iserial == 0)
                    {
                        newRow.CreationDate = DateTime.Now;
                        newRow.FileName = Guid.NewGuid() + Path.GetExtension(newRow.OrginalFileName);
                        newRow.Path = "/Uploads/"+ relativePath +"/"+ newRow.FileName;
                        attachmentPath.Add(new Tuple<int, string>(v, newRow.Path));
                        context.TblAttachments.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in context.TblAttachments
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            var changedItems = SharedOperation.GenericUpdate(oldRow, newRow, context);
                            if (changedItems.Count() > 1)
                            {
                                newRow.LastUpdatedDate = DateTime.Now;
                                attachmentPath.Add(new Tuple<int, string>(oldRow.Iserial, relativePath + oldRow.FileName));
                                SharedOperation.GenericUpdate(oldRow, newRow, context);
                            }
                        }
                    }
                }
                context.SaveChanges();
                return attachments;
            }
        }
        
        public int GetPath(WorkFlowManagerDBEntities context)
        {
            var serialNo = context.TblAttachments.Select(x => x.Iserial).Cast<int?>().Max();
                //attachmentPath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "AttachmentsDefaultFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }// this will be assigned with identity
                return 1;          
        }
    }
}