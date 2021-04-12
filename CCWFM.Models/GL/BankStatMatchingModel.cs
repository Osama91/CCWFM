using CCWFM.Web.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCWFM.Models.Gl
{
    public class BankStatMatchingModel : PropertiesViewModelBase
    {
        bool isChecked = false, isLedger = false;
        string description = "", depositNo = "";
        long? chequeNo;
        int iserial = 0;
        DateTime? docDate;
        decimal amount;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; RaisePropertyChanged(nameof(IsChecked)); }
        }

        public DateTime? DocDate
        {
            get { return docDate; }
            set { docDate = value; RaisePropertyChanged(nameof(DocDate)); }
        }

        public string Description
        {
            get { return description; }
            set { description = value; RaisePropertyChanged(nameof(Description)); }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; RaisePropertyChanged(nameof(Amount)); }
        }

        public long? ChequeNo
        {
            get { return chequeNo; }
            set { chequeNo = value; RaisePropertyChanged(nameof(ChequeNo)); }
        }

        public string DepositNo
        {
            get { return depositNo; }
            set { depositNo = value; RaisePropertyChanged(nameof(DepositNo)); }
        }

        public int Iserial
        {
            get { return iserial; }
            set { iserial = value; RaisePropertyChanged(nameof(Iserial)); }
        }
        
        public bool IsLedger
        {
            get { return isLedger; }
            set { isLedger = value; RaisePropertyChanged(nameof(IsLedger)); }
        }
    }
}
