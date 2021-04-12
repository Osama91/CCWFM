using CCWFM.CRUDManagerService;

namespace CCWFM.Helpers.Utilities
{
    static public class ChaingSetupMethod
    {
        static public void GetSettings(string formName, CRUD_ManagerServiceClient webService)
        {
            webService.GetChainSetupAsync(formName);
        }
    }
}