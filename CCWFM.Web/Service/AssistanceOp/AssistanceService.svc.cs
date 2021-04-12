using NLog;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace CCWFM.Web.Service.AssistanceOp
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public partial class AssistanceService
    {
        private static Logger logger = LogManager.GetLogger(Global.InfoLoggerName);

        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        public void SaveLog(string json, int userIserial)
        {
            logger.Info(string.Format("User Iserial:{0}", userIserial));
            logger.Error(json);
        }
        [WebInvoke(Method = "GET",
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "xml/{id}")]
        [OperationContract]
        private string getData(string id)
        {
            return "success";
        }
    }
}
