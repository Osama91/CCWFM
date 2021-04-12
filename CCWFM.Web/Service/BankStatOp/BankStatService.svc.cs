using CCWFM.Web.DataLayer;
using CCWFM.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CCWFM.Web.Service.BankStatOp
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public partial class BankStatService
    {
        [OperationContract]
        public void DoWork()
        {
            // Add your operation implementation here
            return;
        }

        [OperationContract]
        public List<Model.Employee> GetEmployees()
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                return db.Employees.ToList();
            }
        }


        [OperationContract]
        public List<Model.TblCompany> GetCompanies()
        {
            using (var db = new WorkFlowManagerDBEntities())
            {
                return db.TblCompanies.ToList();
            }
        }
        // Add more operations here and mark them with [OperationContract]
    }
}
