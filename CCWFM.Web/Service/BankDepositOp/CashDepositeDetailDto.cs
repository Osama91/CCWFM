namespace CCWFM.Web.Service.BankDepositOp
{
    internal class CashDepositeDetailDto
    {
        public CashDepositeDetailDto()
        {
        }

        public int EntityAccount { get; set; }
        public string MachineId { get; set; }
        public int TblBank { get; set; }
        public int TblJournalAccountType { get; set; }
    }
}