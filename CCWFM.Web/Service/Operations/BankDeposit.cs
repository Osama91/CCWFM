using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public DateTime BankDepositFromDate(string code, out bool Enabled,string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                // int Amout = DB.OutletExpensis( );

                var row =
                    db.TblBankDeposits.OrderByDescending(x => x.ToDate).FirstOrDefault(x => x.TblStore1.code == code);
                if (row != null)
                {
                    var todate = row.ToDate;
                    Enabled = false;
                    return todate.AddDays(1);
                }
                else
                {
                    Enabled = true;

                    return DateTime.Now;
                }
            }
        }

        [OperationContract]
        public double BankDepositAmount(DateTime fdate, DateTime tdate, string storeCode,string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var cashAmount = db.OutletExpensis(fdate, tdate, storeCode).SingleOrDefault();

                if (cashAmount != null) return cashAmount.Net_Cash_Deposit;
                return 0;
            }
        }

        [OperationContract]
        public string Bankdigit(int ActiveStore, bool serialnum,string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                //int _bank = db.TblBankDeposits.FirstOrDefault(x=>x.TblStore== ActiveStore).TblStore;
                var glSerial = db.TblStoreGlserials.FirstOrDefault(x => x.TblStore == ActiveStore);
                var bank = (Convert.ToInt32(glSerial.bank) + 1).ToString();
                var code = db.TblStores.FirstOrDefault(z => z.iserial == ActiveStore).code;
                var digit = db.TblChainSettings.FirstOrDefault();
                var docvotdig = digit.docvotdig;
                var docstoredig = digit.docstoredig;
                // int? instore = docvotdig - code.Length;
                var instore = docstoredig - code.Length;
                var result = new String('0', Convert.ToInt32(instore)) + code;
                var invotebank = docvotdig - bank.Trim().Length;
                var result1 = new string('0', Convert.ToInt32(invotebank)) + bank.Trim();
                var finalresult = result + result1;

                if (serialnum == true)
                {
                    glSerial.bank = (Convert.ToInt32(glSerial.bank) + 1);
                    db.SaveChanges();
                }

                return finalresult;
            }
        }

        [OperationContract]
        public List<TblStore> SearchBysStoreName(List<int> Storeiserial, int user, string storename, string code,string company)
        {
            var permissions = new List<string>();
            using (var db = new WorkFlowManagerDBEntities())
            {
                permissions = (from tblLkpBrandSectionLinks in db.TblLkpBrandSectionLinks
                               join tblUserBrandSections in db.TblUserBrandSections
                                   on
                                   new { BrandCode = tblLkpBrandSectionLinks.TblBrand, tblLkpBrandSectionLinks.TblLkpBrandSection }
                                   equals new { tblUserBrandSections.BrandCode, tblUserBrandSections.TblLkpBrandSection }
                               where
                                   tblUserBrandSections.TblAuthUser == user
                               select tblLkpBrandSectionLinks.TblItemDownLoadDef).ToList();
            }

            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                db.Connection.Open();
                var u = new List<TblStore>();
                if (permissions.Any())
                {
                    u = (from tblBrandStorePriorityActives in db.TblBrandStorePriorityActives
                         join tblStores in db.TblStores on new { TblstoreCode = tblBrandStorePriorityActives.TblstoreCode }
                             equals new { TblstoreCode = tblStores.code }
                         where
                             permissions.Contains(tblBrandStorePriorityActives.TblBrandCode) && tblStores.Type == 4
                         orderby
                             tblBrandStorePriorityActives.Priority descending
                         select tblStores).Distinct().ToList();
                }
                u = db.TblStores.Where(z => (z.ENAME.StartsWith(storename) || storename == null || storename == "") && z.Type == 4 && (z.code.StartsWith(code) || code == null || code == "") &&
                            Storeiserial.Any(l => z.iserial == l)).ToList();
                return u;
            }
        }

        [OperationContract]
        public TblStore SearchByStoreCode(string code, List<int> Storeiserial,string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var u = db.TblStores.FirstOrDefault(z => z.code == code && z.Type == 4 && Storeiserial.Any(l => z.iserial == l));
                return u;
            }
        }

        [OperationContract]
        public List<TblBankDeposit> SearchTblBankDeposit(string iserial, string storeName, DateTime? Date, string Code, List<int> Storeiserial,string company)
        {
            using (var db = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var tbl = db.TblBankDeposits.Include("TblStore1").Where(m => (m.TblStore1.ENAME.StartsWith(storeName) || storeName == null || storeName == "")
                                                                                && (m.Iserial.StartsWith(iserial) || iserial == null)
                                                                                && (m.TblStore1.code.StartsWith(Code) || Code == null || Code == "")
                                                                                && ((EntityFunctions.TruncateTime(m.FromDate) >= Date && EntityFunctions.TruncateTime(m.ToDate) >= Date) || Date == null)
                                                                                && Storeiserial.Any(l => m.TblStore1.iserial == l))
                                                                                .ToList();

                return tbl;
            }
        }

        [OperationContract]
        private TblBankDeposit UpdateOrInsertTblBankDeposit(TblBankDeposit newRow,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankDeposits
                              where e.Glserial == newRow.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //   GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    newRow.Iserial = Bankdigit(newRow.TblStore, true,company);
                    context.TblBankDeposits.AddObject(newRow);
                }

                context.SaveChanges();

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblBankDeposit(TblBankDeposit row,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankDeposits
                              where e.Glserial == row.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
                return row.Glserial;
            }
        }

        [OperationContract]
        private List<TblBankDeposit> GetTblBankDeposit(int skip, int take, int store, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBankDeposit> query;

                fullCount = context.TblBankDeposits.Count(x => x.TblStore == store);
                query = context.TblBankDeposits.Where(x => x.TblStore == store && x.Status != 1).OrderBy(x => x.FromDate);

                return query.ToList();
            }
        }

        [OperationContract]
        private void ApproveBankDeposit(TblBankDeposit newRow, int status,string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankDeposits
                              where e.Glserial == newRow.Glserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    oldRow.Status = status;
                    //   GenericUpdate(oldRow, newRow, context);
                    context.SaveChanges();
                }

            }
        }
    }
}