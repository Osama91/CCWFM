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
        public List<tblChainSetup> GetChainSetup(string formName, out DateTime Today)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    Today = DateTime.Now;
                    return context.tblChainSetups.Where(x => x.sGridHeaderEName == formName || x.sGridHeaderEName == "Common").ToList();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }


        public string GetChainSetupBycode(string sGlobalSettingCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                
                    return context.tblChainSetups.Where(x => x.sGlobalSettingCode == sGlobalSettingCode).FirstOrDefault().sSetupValue;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }


    }
}