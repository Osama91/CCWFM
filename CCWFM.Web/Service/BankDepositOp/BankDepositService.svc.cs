using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CCWFM.Web.Service.BankDepositOp
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public partial class BankDepositService
    {
        [OperationContract]
        public void DoWork()
        {
            // Add your operation implementation here
            return;
        }

        //// Add more operations here and mark them with [OperationContract]
        //[OperationContract]
        //private void GetDailySalesCommision(
        //    int store, DateTime from, DateTime to, int userIserial, string company)
        //{
        //    try
        //    {
        //        new Operations.GlOperations.GlService().GetDailySalesCommision(0, from, to, userIserial, "CCNEW");
        //    }
        //    catch (Exception ex)
        //    {
        //    }
           
        //}
    }
}
