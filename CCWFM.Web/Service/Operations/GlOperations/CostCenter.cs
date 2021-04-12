using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.Linq.Dynamic;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblCostCenter> GetTblCostCenter(int skip, int take, int? tblCostCenterType, string sort,
            string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, int user,
            int entityAccount, int journalType)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblCostCenter> query;
                var listofIserial = new List<int>();

                if (user != 0 && entityAccount != 0)
                {
                    int job = 0;
                    using (var context = new WorkFlowManagerDBEntities())
                    {
                        job = (int)context.TblAuthUsers.FirstOrDefault(x => x.Iserial == user).TblJob;
                    }

                    var query1 =
                        (from e in
                             entity.TblGlRuleJobs.Include(
                                 "TblGlRuleHeader1.TblGlRuleMainDetails.TblGlRuleDetails.TblCostCenter1")
                         where e.TblJob == job
                         select e).SingleOrDefault();
                    if (query1 != null)
                    {
                        foreach (
                            var tblCostCenter in
                                query1.TblGlRuleHeader1.TblGlRuleMainDetails.Where(
                                    x => x.EntityAccount == entityAccount && x.TblJournalAccountType == journalType)
                                    .ToList())
                        {
                            listofIserial.AddRange(tblCostCenter.TblGlRuleDetails.Select(w => w.TblCostCenter));
                        }
                    }
                    else
                    {
                        var newquery =
                            (from e in
                                 entity.TblGlRuleJobs.Include(
                                     "TblGlRuleHeader1.TblGlRuleMainDetails.TblGlRuleDetails.TblCostCenter1")
                             where e.TblJob == null
                             select e).SingleOrDefault();

                        if (newquery != null)
                        {
                            foreach (
                                var tblCostCenter in
                                    newquery.TblGlRuleHeader1.TblGlRuleMainDetails.Where(
                                        x => x.EntityAccount == entityAccount && x.TblJournalAccountType == journalType)
                                        .ToList())
                            {
                                listofIserial.AddRange(tblCostCenter.TblGlRuleDetails.Select(w => w.TblCostCenter));
                            }
                        }
                    }
                }

                if (filter != null)
                {
                    if (tblCostCenterType != null)
                    {
                        filter = filter + " and it.TblCostCenterType ==(@Group0)";
                        valuesObjects.Add("Group0", tblCostCenterType);
                    }

                    var paratmer = "@s";
                    foreach (var variable in listofIserial)
                    {
                        paratmer = paratmer + "s";
                        filter = filter + " and it.Iserial ==(" + paratmer + ")";
                        valuesObjects.Add(paratmer, variable);
                    }

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblCostCenters.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        entity.TblCostCenters.Include("TblCostCenter1")
                            .Include("TblCostCenter2")
                            .Include("TblCostCenterType1")
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }

                else
                {
                    if (tblCostCenterType != null)
                    {
                        fullCount = entity.TblCostCenters.Count(x => x.TblCostCenterType == tblCostCenterType);
                        query =
                            entity.TblCostCenters.Include("TblCostCenter1")
                                .Include("TblCostCenter2")
                                .Include("TblCostCenterType1")
                                .OrderBy(sort)
                                .Where(x => x.TblCostCenterType == tblCostCenterType && (listofIserial.Contains(x.Iserial) || !listofIserial.Any()))
                                .Skip(skip)
                                .Take(take);
                    }
                    else
                    {
                        fullCount = entity.TblCostCenters.Count();
                        query =
                            entity.TblCostCenters.Include("TblCostCenter1")
                                .Include("TblCostCenter2")
                                .Include("TblCostCenterType1")
                                .OrderBy(sort)
                                .Where(x => listofIserial.Contains(x.Iserial) || !listofIserial.Any())
                                .Skip(skip)
                                .Take(take);
                    }
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblCostCenter GetTblCostCenterByCode(string code, int? tblCostCenterType, string company, int user)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                TblCostCenter query;
                query =
                    entity.TblCostCenters.Include("TblCostCenter1")
                        .Include("TblCostCenter2")
                        .Include("TblCostCenterType1").FirstOrDefault(x => x.Code == code && x.TblCostCenterType == tblCostCenterType);

                return query;
            }
        }

        [OperationContract]
        private TblCostCenter UpdateOrInsertTblCostCenters(TblCostCenter newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblCostCenters.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblCostCenters
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblCostCenter(TblCostCenter row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblCostCenters
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblBankChequeCostCenter> GetTblBankChequeCostCenter(int skip, int take, int BankCheque, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBankChequeCostCenter> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblBankCheque ==(@Group0)";
                    valuesObjects.Add("Group0", BankCheque);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblBankChequeCostCenters.Where(filter, parameterCollection).Count();
                    query = entity.TblBankChequeCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblBankChequeCostCenters.Count(v => v.TblBankCheque == BankCheque);
                    query = entity.TblBankChequeCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(v => v.TblBankCheque == BankCheque).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblBankChequeCostCenter UpdateOrInsertTblBankChequeCostCenters(TblBankChequeCostCenter newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblBankChequeCostCenters.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblBankChequeCostCenters
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, entity);
                    }
                }
                entity.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblBankChequeCostCenter(TblBankChequeCostCenter row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblBankChequeCostCenters
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}