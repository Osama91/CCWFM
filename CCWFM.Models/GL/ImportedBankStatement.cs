using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCWFM.Models.Gl
{
    public class ImportedBankStatement
    {
        DateTime docDate;
        string transactionType, description, depositNo;
        decimal amount;
        long? chequeNo;
        public DateTime DocDate
        {
            get { return docDate; }
            set { docDate = value; }
        }

        public string TransactionType
        {
            get { return transactionType; }
            set { transactionType = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public long? ChequeNo
        {
            get { return chequeNo; }
            set { chequeNo = value; }
        }

        public string DepositNo
        {
            get { return depositNo; }
            set { depositNo = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
    }
}
