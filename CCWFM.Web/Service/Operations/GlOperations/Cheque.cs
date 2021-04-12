using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {

        [OperationContract]
        private bankchequePrint_Result PrintCheck(int iserial, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return entity.bankchequePrint(iserial).FirstOrDefault();
            }

        }

        [OperationContract]
        private List<TblBankCheque> GetTblBankCheque(int skip, int take, int bank, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList, int chequeStatus)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBankCheque> query;
                if (filter != null)
                {
                    if (bank!=0)
                    {
                        filter = filter + " and it.TblBank ==(@Group0)";
                        valuesObjects.Add("Group0", bank);   
                    }

                    if (chequeStatus!=0)
                    {
                        filter = filter + " and it.TblGlChequeStatus ==(@chequeStatus)";
                        valuesObjects.Add("chequeStatus", chequeStatus);   
                    }
                    else
                    {
                        filter = filter + " and it.TblGlChequeStatus !=(@chequeStatus)";
                        valuesObjects.Add("chequeStatus", 3);  
                    }
                   
                    
                   
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblBankCheques.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblBankCheques.Include("TblGlChequeStatu").Include("TblBank1").Include("TblJournalAccountType1").Include("TblCurrency1")
                         .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblBankCheques.Count(w => (w.TblBank == bank || bank == 0) && w.TblGlChequeStatus == chequeStatus || (chequeStatus == 0 && w.TblGlChequeStatus != 3));
                    query = entity.TblBankCheques.Include("TblGlChequeStatu").Include("TblBank1").Include("TblJournalAccountType1").Include("TblCurrency1")
                        .OrderBy(sort).Where(v => (v.TblBank == bank || bank == 0)  && v.TblGlChequeStatus == chequeStatus || (chequeStatus == 0 && v.TblGlChequeStatus != 3)).Skip(skip).Take(take);
                }
                List<int?> intList = query.Select(x => x.EntityAccount).ToList();

                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();

                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private long GetMaxCheque(int tblbank, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                long max = 0;
                if (entity.TblBankCheques.Any(x => x.TblBank == tblbank))
                {
                    max = entity.TblBankCheques.Max(x => x.Cheque);
                }
                return max + 1;
            }
        }

        [OperationContract]
        private List<TblBankCheque> CreateCheque(int tblBank, long from, int to, string company)
        {


            var ints = new List<TblBankCheque>();
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var bank = entity.TblBanks.FirstOrDefault(w => w.Iserial == tblBank);
                long difference = (from + to) - from;
                for (int i = 0; i < difference; i++)
                {
                    var newCheque = new TblBankCheque
                    {
                        TblCurrency= bank.TblCurrency,
                        Cheque = i + from,
                        Amount = 0,
                        TblGlChequeStatus = 4,
                        TblBank = tblBank,
                        TransDate = DateTime.Now,
                    };
                    entity.TblBankCheques.AddObject(newCheque);
                    entity.SaveChanges();
                    ints.Add(newCheque);
                }
            }
            return ints;
        }

        [OperationContract]
        private List<int> DeleteTblBankCheque(int tblBank, int fromNo, int to, string company)
        {
            List<int> ints;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblBankCheques
                             where e.TblBank == tblBank && e.Cheque >= fromNo && e.Cheque <= to
                             select e).ToList();
                ints = query.Select(x => x.Iserial).ToList();
                foreach (var row in query)
                {
                    entity.DeleteObject(row);
                }

                entity.SaveChanges();
            }
            return ints;
        }
    }
}