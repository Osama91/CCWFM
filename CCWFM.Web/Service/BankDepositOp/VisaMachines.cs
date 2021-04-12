using CCWFM.Models;
using CCWFM.Models.Gl;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CCWFM.Web.Service.BankDepositOp
{
    [DataContract]
    public class StoreVisaMachine : StoreVisaMachineModel
    {
        TblStore store;
        [DataMember]
        public TblStore Store
        {
            get { return store; }
            set { store = value; RaisePropertyChanged(nameof(Store)); }
        }

        string entityCode;
        [DataMember]
        public string EntityCode
        {
            get { return entityCode; }
            set { entityCode = value; RaisePropertyChanged(nameof(EntityCode)); }
        }
    }
    public partial class BankDepositService
    {
        [OperationContract]
        private List<StoreVisaMachine> GetVisaMachine(int BankIserial, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var defaultQuery = context.TblStoreVisaMachines.Include(
                    nameof(TblStoreVisaMachine.TblVisaMachine1)).Where(v => v.TblVisaMachine1.TblBank == BankIserial);
                List<StoreVisaMachine> result = new List<StoreVisaMachine>();
                if (defaultQuery != null)
                {
                    foreach (var item in defaultQuery)
                    {
                        var entity = context.Entities.FirstOrDefault(e =>
                                 e.Iserial == item.TblVisaMachine1.EntityAccount && e.TblJournalAccountType == 15);
                        var model = new StoreVisaMachine()
                        {
                            VisaMachineIserial = item.TblVisaMachine1.Iserial,
                            Code = item.TblVisaMachine1.Code,
                            MachineId = item.TblVisaMachine1.MachineId,
                            BankIserial = item.TblVisaMachine1.TblBank,
                            DiscountPercent = item.TblVisaMachine1.DiscountPercent,
                            EntityAccount = item.TblVisaMachine1.EntityAccount,
                            EntityCode = entity == null ? "0" : entity.Code,

                            StoreVisaMachineIserial = item.Iserial,
                            StoreIserial = item.TblStore,
                            IsDefault = item.IsDefault,
                            Store = item.TblStore1,
                        };
                        result.Add(model);
                    }
                }
                return result;
            }
        }
        
        [OperationContract]
        private List<StoreVisaMachine> UpdateOrInsertVisaMachine(List<StoreVisaMachine> newRows, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                try
                {
                    foreach (var item in newRows)
                    {
                        var oldVisaMachinesRow = context.TblVisaMachines.FirstOrDefault(th => th.Iserial == item.VisaMachineIserial);
                        var oldStoreVisaMachinesRow = context.TblStoreVisaMachines.FirstOrDefault(th => th.Iserial == item.StoreVisaMachineIserial);
                        bool isVisaMachinesRow = oldVisaMachinesRow != null,
                            isStoreVisaMachinesRow = oldStoreVisaMachinesRow != null;

                        if (oldVisaMachinesRow == null)
                            oldVisaMachinesRow = new TblVisaMachine();
                        oldVisaMachinesRow.Code = item.Code;
                        oldVisaMachinesRow.TblBank = item.BankIserial;
                        oldVisaMachinesRow.MachineId = item.MachineId;
                        oldVisaMachinesRow.DiscountPercent = item.DiscountPercent;
                        oldVisaMachinesRow.EntityAccount = item.EntityAccount;

                        if (oldStoreVisaMachinesRow == null)
                            oldStoreVisaMachinesRow = new TblStoreVisaMachine() { TblVisaMachine1 = oldVisaMachinesRow };
                        oldStoreVisaMachinesRow.TblVisaMachine = item.VisaMachineIserial;
                        oldStoreVisaMachinesRow.TblStore = item.StoreIserial;
                        oldStoreVisaMachinesRow.IsDefault = item.IsDefault;

                        if (!isVisaMachinesRow)// كده ده جديد هضيفه
                        {
                            context.TblVisaMachines.AddObject(oldVisaMachinesRow);
                        }
                        if (!isStoreVisaMachinesRow)// كده ده جديد هضيفه
                        {
                            oldVisaMachinesRow.TblStoreVisaMachines.Add(oldStoreVisaMachinesRow);
                            context.TblStoreVisaMachines.AddObject(oldStoreVisaMachinesRow);
                        }
                        context.SaveChanges();
                        item.StoreVisaMachineIserial = oldStoreVisaMachinesRow.Iserial;
                        item.VisaMachineIserial = oldVisaMachinesRow.Iserial;
                    }
                }
                catch (Exception ex) { throw Helper.GetInnerException(ex); }
                return newRows;
            }
        }

        [OperationContract]
        private int DeleteVisaMachine(StoreVisaMachine row, string company)
        {
            using (var context = new ccnewEntities(service.GetSqlConnectionString(company)))
            {
                var oldStoreRow = (from e in context.TblStoreVisaMachines
                              where e.Iserial == row.StoreVisaMachineIserial
                              select e).SingleOrDefault();
                if (oldStoreRow != null) context.DeleteObject(oldStoreRow);

                var oldRow = context.TblVisaMachines.SingleOrDefault(r => r.Iserial == row.VisaMachineIserial);
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.StoreVisaMachineIserial;
        }
        
    }
}