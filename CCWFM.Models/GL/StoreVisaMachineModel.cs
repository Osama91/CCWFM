using CCWFM.Web.DataLayer;
using System.Runtime.Serialization;

namespace CCWFM.Models.Gl
{
    [DataContract]
    public class StoreVisaMachineModel : PropertiesViewModelBase
    {
        bool isDefault = false;
        string code = "", machineId = "";
        int storeIserial, storeVisaMachineIserial,
            visaMachineIserial, bankIserial, entityAccount;
        decimal discountPercent;

        [DataMember]
        public int BankIserial
        {
            get { return bankIserial; }
            set { bankIserial = value; RaisePropertyChanged(nameof(BankIserial)); }
        }


        [DataMember]
        public int VisaMachineIserial
        {
            get { return visaMachineIserial; }
            set { visaMachineIserial = value; RaisePropertyChanged(nameof(VisaMachineIserial)); }
        }
        [DataMember]
        public int EntityAccount
        {
            get { return entityAccount; }
            set { entityAccount = value; RaisePropertyChanged(nameof(EntityAccount)); }
        }
        [DataMember]
        public decimal DiscountPercent
        {
            get { return discountPercent; }
            set { discountPercent = value; RaisePropertyChanged(nameof(DiscountPercent)); }
        }
        [DataMember]
        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged(nameof(Code)); }
        }
        [DataMember]
        public string MachineId
        {
            get { return machineId; }
            set { machineId = value; RaisePropertyChanged(nameof(MachineId)); }
        }


        [DataMember]
        public int StoreVisaMachineIserial
        {
            get { return storeVisaMachineIserial; }
            set { storeVisaMachineIserial = value; RaisePropertyChanged(nameof(StoreVisaMachineIserial)); }
        }
        [DataMember]
        public int StoreIserial
        {
            get { return storeIserial; }
            set { storeIserial = value; RaisePropertyChanged(nameof(StoreIserial)); }
        }
        [DataMember]
        public bool IsDefault
        {
            get { return isDefault; }
            set { isDefault = value; RaisePropertyChanged(nameof(IsDefault)); }
        }
    }
}
