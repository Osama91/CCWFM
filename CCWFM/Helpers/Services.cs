using CCWFM.AssistanceService;
using CCWFM.AuthService;
//using CCWFM.AuthService;
using CCWFM.BankDepositService;
using CCWFM.BankStatService;
using CCWFM.ContractService;
using CCWFM.CRUDManagerService;
using CCWFM.LkpData;
using CCWFM.ProductionService;
using CCWFM.WarehouseService;
using System;
using System.Net;
using System.Windows;

namespace CCWFM.Helpers
{
    public class Services
    {
        private Services() { CookieContainer = new CookieContainer(); }
        static Services instance;
        public static Services Instance
        {
            get { return instance ?? (instance = new Services()); }
        }        
        public CookieContainer CookieContainer
        {
            get;
            set;
        }
        public void Authenticate()
        {
            //bool httpResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);

            //CookieContainer = new CookieContainer();

            //AuthSvc.AuthenticationServiceClient authClient = new AuthSvc.AuthenticationServiceClient();
            //using (new OperationContextScope(authClient.InnerChannel))
            //{
            //    HttpRequestMessageProperty property = new HttpRequestMessageProperty();
            //    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = property;
            //    authClient.CookieContainer = CookieContainer;
            //    authClient.LoginCompleted += (object sender2, AuthSvc.LoginCompletedEventArgs e2) =>
            //    {
            //        if (e2.Result)
            //            MessageBox.Show("Authentication Success!");
            //        else
            //            MessageBox.Show("Authentication Failure!");
            //    };

            //    authClient.LoginAsync("tom", "chicago12", string.Empty, true);
            //}
        }
        public AssistanceServiceClient GetAssistanceServiceClient()
        {
            return new AssistanceServiceClient();
        }
        public ProductionServiceClient GetProductionServiceClient()
        {
            return new ProductionServiceClient();
        }
        public AuthServiceClient GetAuthServiceClient()
        {
            return new AuthServiceClient();
        }
        public BankDepositServiceClient GetBankDepositServiceClient()
        {
    

            var client = new BankDepositServiceClient();
            try
            {
                //client.CookieContainer = CookieContainer;
            }
            catch (Exception) { }

            return client;
        }
        public BankStatServiceClient GetBankStatServiceClient()
        {
            return new BankStatServiceClient();
        }
        public ContractServiceClient GetContractServiceClient()
        {
            return new ContractServiceClient();
        }
        public CRUD_ManagerServiceClient GetCRUD_ManagerServiceClient()
        {
            return new CRUD_ManagerServiceClient();
        }
        public WarehouseServiceClient GetWarehouseServiceClient()
        {
            return new WarehouseServiceClient();
        }
        public LkpDataClient GetLkpDataClient()
        {
            return new LkpDataClient();
        }
    }
}
