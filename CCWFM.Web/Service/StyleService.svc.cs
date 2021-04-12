using CCWFM.Web.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CCWFM.Web.Service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]

    public class StyleService
    {
        [OperationContract]
        private int RemoveSalesOrderApproval(string salesOrderCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int result = context.RemoveApprovedSalesOrder(salesOrderCode);
                context.SaveChanges();
                return result;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool InsertStyleImageFromFolder(int tblStyle, string styleCode)
        {



            return true;
        }


        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }



    }

}
