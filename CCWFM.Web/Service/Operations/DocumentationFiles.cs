using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using CCWFM.Web.Model;
using System.IO;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleAttachment> GetDocumentationFiles()
        {
         //   var uri =MapPath("~");
            var uri = HttpContext.Current.Server.MapPath("~/");
            var temp =Path.Combine(uri, SharedOperation.GetChainSetup("DocumentationFolder"));
            
            
            var dInfo = new DirectoryInfo(@temp);//your path
            FileInfo[] FilesList = dInfo.GetFiles();//can filter here with appropriate extentions
            var list = new List<TblStyleAttachment>();
            foreach (var fileInfo in FilesList)
            {
                var newFile = new TblStyleAttachment();
                newFile.AttachmentPath = SharedOperation.GetChainSetup("DocumentationFolder")+"/"+ fileInfo.Name;
                newFile.AttachmentDescription = fileInfo.Name;
                newFile.OrginalFileName = fileInfo.Name;
                list.Add(newFile);
            }
            return list;//you can also return more generic xml
        }
        
    }
}