namespace CCWFM.Web.Service.Dto_s
{
    public class Enums
    {
        public enum ApprovalStatus
        {
            PendingRequest = 0,
            Approved = 1,
            Rejected = 2,
            ProceedNext = 3,
            Canceled = 4,
            FinalCost = 5,
        }

        public enum SalesOrderType
        {
            RetailPo = 1,
            SalesOrderPo = 2,
            RFQ = 3,
            AdvancedSampleRequest = 4
        }
    }
}