using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblAccount> GetTblAccount(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, bool ChildOnly, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var accountQuery = new List<int>();
                if (ChildOnly)
                {
                    accountQuery = entity.TblAccountIntervals.Select(x => x.TblAccount).ToList();
                }
                IQueryable<TblAccount> query;

                if (filter != null)
                {
                    if (ChildOnly)
                    {
                        foreach (var variable in accountQuery)
                        {
                            filter = filter + " and it.Iserial !=(@Iserial" + variable + ")";
                            valuesObjects.Add("Iserial" + variable, variable);
                        }
                    }
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblAccounts.Where(filter, parameterCollection.ToArray()).Count();
                    query =
                        entity.TblAccounts.Include("TblAccountType1")
                            .Include("TblCurrency1").Include("TblJournalAccountType1")
                            .Where(filter, parameterCollection.ToArray())
                            .OrderBy(sort)
                            .Skip(skip)
                            .Take(take);
                }
                else
                {
                    if (ChildOnly)
                    {
                        fullCount = entity.TblAccounts.Count(x => (!accountQuery.Contains(x.Iserial) && ChildOnly));
                        query =
                            entity.TblAccounts.Include("TblAccountType1")
                                .Include("TblCurrency1").Include("TblJournalAccountType1")
                                .OrderBy(sort)
                                .Where(x => (!accountQuery.Contains(x.Iserial) && ChildOnly))
                                .Skip(skip)
                                .Take(take);
                    }
                    else
                    {
                        fullCount = entity.TblAccounts.Count();
                        query =
                            entity.TblAccounts.Include("TblAccountType1")
                                .Include("TblCurrency1").Include("TblJournalAccountType1")
                                .OrderBy(sort)
                                .Skip(skip)
                                .Take(take);
                    }
                }

                List<int?> intList = query.Select(x => x.EntityAccount).ToList();

                
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                
                entityList = entity.Entities.Where(x => x.scope == 0 && intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
        
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAccount GetTblAccountbyCode(string code, string company, bool ChildOnly)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var accountQuery = new List<int>();
                if (ChildOnly)
                {
                    accountQuery = entity.TblAccountIntervals.Select(x => x.TblAccount).ToList();
                }
                var query = entity.TblAccounts.Include("TblAccountType1").Include("TblCurrency1").FirstOrDefault(x => (!accountQuery.Contains(x.Iserial) && ChildOnly) && x.Code == code);

                return query;
            }
        }

        [OperationContract]
        private TblAccount UpdateOrInsertTblAccounts(TblAccount newRow, bool save, int index, out int outindex, string company)
        {
         
                outindex = index;
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    
                    if (save)
                    {
                        if (newRow.TblAccountType == 9)
                        {
                            newRow.TblAccountIntervals = new EntityCollection<TblAccountInterval>
                            {
                                new TblAccountInterval
                                {
                                    FromRange = newRow.Code,
                                    ToRange = newRow.Code + "Z"
                                }
                            };
                        }
                        entity.TblAccounts.AddObject(newRow);
                    }
                    else
                    {
                        var oldRow = (from e in entity.TblAccounts
                            where e.Iserial == newRow.Iserial
                            select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            GenericUpdate(oldRow, newRow, entity);
                        }
                    }
                    entity.SaveChanges();
             
                }
                
                
                return newRow;
           
        }

        [OperationContract]
        private int DeleteTblAccount(TblAccount row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblAccounts
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private TblCostDimHeader GetTblCostDimSetupHeaderForAccount(int tblJournalAccountType, int? iserial, string company, out TblCostDimSetupHeader HeaderRow, out int tblJournalAccType)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                tblJournalAccType = tblJournalAccountType;
                if (iserial == null)
                {
                    HeaderRow = entity.TblCostDimSetupHeaders.Include("TblCostDimSetupDetails.TblCostCenterOption1").Include("TblCostDimSetupDetails.TblCostCenterType1").FirstOrDefault(x => x.TblJournalAccountType == tblJournalAccountType);
                    return null;
                }
                HeaderRow = null;
                return entity.TblCostDimHeaders.Include("TblCostDimDetails.TblCostCenter1").FirstOrDefault(x => x.Iserial == iserial);
            }
        }

        [OperationContract]
        private int SaveTblCostDimHeader(int tblJournalAccountType, List<TblCostDimDetail> list, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var setupHeader = entity.TblCostDimSetupHeaders.FirstOrDefault(x => x.TblJournalAccountType == tblJournalAccountType);
                var header = entity.TblCostDimHeaders.FirstOrDefault(x => x.TblCostDimSetupHeader == setupHeader.Iserial);

                if (header == null)
                {
                    var newheader = new TblCostDimHeader();
                    newheader.TblCostDimSetupHeader = setupHeader.Iserial;

                    foreach (var row in list)
                    {
                        newheader.TblCostDimDetails.Add(row);
                    }
                    entity.TblCostDimHeaders.AddObject(newheader);
                    entity.SaveChanges();
                    return newheader.Iserial;
                }

                else
                {
                    var newheader = new TblCostDimHeader();
                    bool exsist = false;
                    var listCostDimDetail = new List<TblCostDimDetail>();

                    foreach (var row in list)
                    {
                        newheader.TblCostDimDetails.Add(row);
                        if (!entity.TblCostDimDetails.Any(x => row.TblCostCenter == x.TblCostCenter))
                        {
                            exsist = true;
                        }
                        else
                        {
                            foreach (var variable in entity.TblCostDimDetails.Where(x => row.TblCostCenter == x.TblCostCenter).ToList())
                            {
                                listCostDimDetail.Add(variable);
                            }
                        }
                    }
                    int prevCostCenter = 0;
                    var costDomList = new List<TblCostDimDetail>();
                    if (exsist == false)
                    {
                        foreach (var row in list)
                        {
                            if (prevCostCenter == 0)
                            {
                                prevCostCenter = row.TblCostCenter;

                                costDomList = listCostDimDetail.Where(x => x.TblCostCenter == row.TblCostCenter).ToList();
                            }
                            else
                            {
                                var temp = listCostDimDetail.Where(x => x.TblCostCenter == row.TblCostCenter).Select(x => x.TblCostDimHeader);

                                costDomList = costDomList.Where(x => temp.Contains(x.TblCostDimHeader)).ToList();
                            }
                        }
                    }

                    if (exsist)
                    {
                        newheader.TblCostDimSetupHeader = setupHeader.Iserial;

                        foreach (var row in list)
                        {
                            newheader.TblCostDimDetails.Add(row);
                        }
                        entity.TblCostDimHeaders.AddObject(newheader);
                        entity.SaveChanges();
                        return newheader.Iserial;
                    }

                    if (costDomList.Any())
                    {
                        return costDomList.FirstOrDefault().TblCostDimHeader;
                    }

                    return 0;
                }
            }
        }
    }
}