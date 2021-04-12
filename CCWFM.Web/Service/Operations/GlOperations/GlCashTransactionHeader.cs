using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CCWFM.Web.Model;
using System.Linq.Dynamic;
namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private TblGlCashTypeSetting GetTblCashTypeSettings(string code, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.TblGlCashTypeSettings.Include(nameof(TblGlCashTypeSetting.TblCurrency)).Include(nameof(TblGlCashTypeSetting.TblJournalAccountType)).FirstOrDefault(x => x.Code == code);
                var intList = new List<int?>();


                intList.Add(query.DefaultHeaderEntity1);


                var intTypeList = new List<int?>();
                intTypeList.Add(query.EntityHeader1TblJournalAccountType);


                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType)).ToList();
                return query;
            }
        }

        [OperationContract]
        private List<TblGlCashTransactionHeader> GetTblGlCashTransactionHeader(int skip, int take, int typeSetting, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlCashTransactionHeader> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlCashTypeSetting ==(@Group0)";
                    valuesObjects.Add("Group0", typeSetting);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlCashTransactionHeaders.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblGlCashTransactionHeaders.Include("TblJournalAccountType1").Include("TblGlCashTypeSetting1").Include("TblCurrency1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlCashTransactionHeaders.Count(x => x.TblGlCashTypeSetting == typeSetting);
                    query = entity.TblGlCashTransactionHeaders.Include("TblJournalAccountType1").Include("TblGlCashTypeSetting1").Include("TblCurrency1").OrderBy(sort).Where(v => v.TblGlCashTypeSetting == typeSetting).Skip(skip).Take(take);
                }

                List<int?> intList = query.Select(x => x.EntityAccount).ToList();
                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();
                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType) && x.scope == 0).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlCashTransactionHeader UpdateOrInsertTblGlCashTransactionHeader(
            TblGlCashTransactionHeader newRow, bool save, int index, int user, bool approve, out int outindex,
            string company)
        {
            using (var scope = new TransactionScope())
            {
                #region Code
                //bool ww = DateTime.Now > DateTime.Now.AddDays(1);

                outindex = index;
                using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
                {
                    var GlCashsetting = entity.TblGlCashTypeSettings.Include(nameof(TblGlCashTypeSetting.TblJournal1)).FirstOrDefault(x => x.Iserial == newRow.TblGlCashTypeSetting);
                    if (save)
                    {
                        try
                        {
                            var firstrow = newRow.TblGlCashTransactionDetails.FirstOrDefault();
                            var entitySelected =
                                entity.Entities.FirstOrDefault(
                                    x => x.TblJournalAccountType == firstrow.TblJournalAccountType &&
                                         x.Iserial == firstrow.EntityAccount);
                           // newRow.Description = entitySelected.Ename;
                        }
                        catch (Exception)
                        {
                        }

                        newRow.CreationDate = DateTime.Now;
                        newRow.CreatedBy = user;
                        //var setting =
                        //    entity.TblGlCashTypeSettings.FirstOrDefault(
                        //        x => x.Iserial == newRow.TblGlCashTypeSetting).TblSequence;
                        var journal = entity.TblSequences.FirstOrDefault(x => x.Iserial == GlCashsetting.TblSequence);
                        // var journal = entity.TblSequences.FirstOrDefault(x => x.Iserial == GlCashsetting.TblSequence);
                        int temp = 0;
                        newRow.Code = SharedOperation.HandelSequence(newRow.Code, journal, "TblLedgerHeader", company, 0,
                            newRow.DocDate.Value.Month, newRow.DocDate.Value.Year, out temp);
                        newRow.CreationDate = DateTime.Now;
                        entity.TblGlCashTransactionHeaders.AddObject(newRow);
                    }
                    else
                    {

                        var oldRow = (from e in entity.TblGlCashTransactionHeaders
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            foreach (var newDetailRow in newRow.TblGlCashTransactionDetails.ToList())
                            {
                                if (newDetailRow.Iserial == 0)
                                {
                                    newDetailRow.TblGlCashTransactionHeader1 = null;
                                    newDetailRow.TblGlCashTransactionHeader = oldRow.Iserial;
                                    entity.TblGlCashTransactionDetails.AddObject(newDetailRow);
                                }
                                else
                                {
                                    var oldRowDetail = (from e in entity.TblGlCashTransactionDetails
                                                        where e.Iserial == newDetailRow.Iserial
                                                        select e).SingleOrDefault();
                                    if (oldRowDetail != null)
                                    {
                                        foreach (var item in newDetailRow.TblGlCashTransactionDetailCostCenters.ToList())
                                        {
                                            if (item.Iserial == 0)
                                            {
                                                item.TblGlCashTransactionDetail1 = null;
                                                item.TblGlCashTransactionDetail = newDetailRow.Iserial;
                                                entity.TblGlCashTransactionDetailCostCenters.AddObject(item);
                                            }
                                        }

                                        GenericUpdate(oldRowDetail, newDetailRow, entity);
                                    }
                                }
                            }
                            GenericUpdate(oldRow, newRow, entity);
                        }
                    }
                    entity.SaveChanges();
                    if (approve)
                    {
                        var ledgerheader = entity.TblLedgerHeaders.Any(
                            x => x.TblJournalLink == newRow.Iserial && x.TblTransactionType == 11);
                        if (!ledgerheader)
                        {
                            try
                            {
                                PostTblGlCashTransactionHeader(newRow.Iserial, user, company, newRow.Code);
                            }
                            catch (Exception)
                            {
                                var ledgerToDelete = entity.TblLedgerHeaders.Where(
                                    x => x.TblJournalLink == newRow.Iserial && x.TblTransactionType == 11).ToList();
                                foreach (var ledgerRow in ledgerToDelete)
                                {
                                    entity.TblLedgerHeaders.DeleteObject(ledgerRow);
                                }
                                throw;
                            }
                            newRow.Approved = true;
                        }
                    }

                    try
                    {
                        entity.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        if (ExceptionContainsErrorCode(ex, 2627))
                        {
                            entity.Detach(newRow);
                            UpdateOrInsertTblGlCashTransactionHeader(newRow, save, index, user, approve, out outindex, company);
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }

                #endregion Code
                scope.Complete();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblGlCashTransactionHeader(TblGlCashTransactionHeader row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlCashTransactionHeaders
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblGlCashTransactionDetail> GetTblGlCashTransactionDetail(int header, string company, out List<Entity> entityList)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlCashTransactionDetail> query;

                query = entity.TblGlCashTransactionDetails.Include("TblJournalAccountType1").Where(x => x.TblGlCashTransactionHeader == header);

                List<int?> intList = query.Select(x => x.EntityAccount).ToList();



                List<int?> intTypeList = query.Select(x => x.TblJournalAccountType).ToList();


                entityList = entity.Entities.Where(x => intList.Contains(x.Iserial) && intTypeList.Contains(x.TblJournalAccountType) && x.scope == 0).ToList();

                return query.ToList();
            }
        }

        [OperationContract]
        private int DeleteTblGlCashTransactionDetail(TblGlCashTransactionDetail row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlCashTransactionDetails
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void PostTblGlCashTransactionHeader(int iserial, int user, string company, string code)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                entity.CommandTimeout = 0;
                var CashTransactionHeader =
                    entity.TblGlCashTransactionHeaders.Include("TblGlCashTypeSetting1.TblJournal1").Include("TblGlCashTransactionDetails.TblGlCashTransactionDetailCostCenters")
                        .FirstOrDefault(x => x.Iserial == iserial);
                var journalint =
                    entity.TblJournals.FirstOrDefault(
                        x => x.Iserial == CashTransactionHeader.TblGlCashTypeSetting1.TblJournal);
                try
                {
                    var newLedgerHeaderRowss = new TblLedgerHeader
                    {
                        CreatedBy = CashTransactionHeader.CreatedBy,
                        CreationDate = DateTime.Now,
                        Description = CashTransactionHeader.Description,
                        DocDate = CashTransactionHeader.DocDate,
                        TblJournal = journalint.Iserial,
                        TblTransactionType = 11,
                        TblJournalLink = CashTransactionHeader.Iserial
                    };
                } catch(Exception ex ) {
                    string c = ex.Message;
                }

                var newLedgerHeaderRow = new TblLedgerHeader
                {
                    CreatedBy = CashTransactionHeader.CreatedBy,
                    CreationDate = DateTime.Now,
                    Description = CashTransactionHeader.Description,
                    DocDate = CashTransactionHeader.DocDate,
                    TblJournal = journalint.Iserial,
                    TblTransactionType = 11,
                    TblJournalLink = CashTransactionHeader.Iserial
                };
                var tempheader = 0;
                UpdateOrInsertTblLedgerHeaders(newLedgerHeaderRow, true, 000, out tempheader, (int)CashTransactionHeader.CreatedBy,
                             company);

                var drorcr = false;
                if (CashTransactionHeader.TblGlCashTypeSetting1.Code == "3")
                {
                    drorcr = true;
                }
                foreach (var row in CashTransactionHeader.TblGlCashTransactionDetails)
                {
                    var accountDetail1 =
                            entity.Entities.FirstOrDefault(
                                x =>
                                    x.TblJournalAccountType == row.TblJournalAccountType &&
                                    x.Iserial == row.EntityAccount).AccountIserial;

                    var newledgerDetailrowh1 = new TblLedgerMainDetail
                    {
                        Amount = (decimal?)row.Amount,
                        Description = row.Description,
                        ExchangeRate = CashTransactionHeader.ExchangeRate,
                        TblCurrency = CashTransactionHeader.TblCurrency,
                        TransDate = CashTransactionHeader.DocDate,
                        TblJournalAccountType = row.TblJournalAccountType,
                        EntityAccount = row.EntityAccount,
                        GlAccount = accountDetail1,
                        TblLedgerHeader = newLedgerHeaderRow.Iserial,
                        DrOrCr = drorcr,
                        PaymentRef= CashTransactionHeader.Code
                    };
                    newledgerDetailrowh1.TblLedgerDetailCostCenters = new System.Data.Objects.DataClasses.EntityCollection<TblLedgerDetailCostCenter>();
                    foreach (var item in row.TblGlCashTransactionDetailCostCenters)
                    {

                        var newCostCenter = new TblLedgerDetailCostCenter()
                        {
                            Amount = item.Amount,
                            Calculated = false,
                            Ratio = 0,
                            TblCostCenter = item.TblCostCenter,
                            TblCostCenterType = item.TblCostCenterType,

                        };

                        newledgerDetailrowh1.TblLedgerDetailCostCenters.Add(newCostCenter);

                    }

                    UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowh1, true, 000, out tempheader, company, (int)CashTransactionHeader.CreatedBy);
                }
                var accountDetail = entity.Entities.FirstOrDefault(
                                 x => x.TblJournalAccountType == CashTransactionHeader.TblJournalAccountType &&
                                     x.Iserial == CashTransactionHeader.EntityAccount).AccountIserial;

                var newledgerDetailrowhCash = new TblLedgerMainDetail
                {
                    Amount = (decimal?)CashTransactionHeader.TblGlCashTransactionDetails.Sum(w => w.Amount),
                    Description = CashTransactionHeader.Description,
                    ExchangeRate = CashTransactionHeader.ExchangeRate,
                    TblCurrency = CashTransactionHeader.TblCurrency,
                    TransDate = CashTransactionHeader.DocDate,
                    TblJournalAccountType = CashTransactionHeader.TblJournalAccountType,
                    EntityAccount = CashTransactionHeader.EntityAccount,
                    GlAccount = accountDetail,
                    TblLedgerHeader = newLedgerHeaderRow.Iserial,
                    DrOrCr = !drorcr,
                    PaymentRef = CashTransactionHeader.Code
                };
                UpdateOrInsertTblLedgerMainDetails(newledgerDetailrowhCash, true, 000, out tempheader, company,
        (int)CashTransactionHeader.CreatedBy);


                CashTransactionHeader.ApproveDate = DateTime.Now;
                CashTransactionHeader.ApprovedBy = user;
                CashTransactionHeader.Approved = true;
                entity.SaveChanges();
            }
        }

        [OperationContract]
        private List<TblGlCashTransactionDetailCostCenter> GetTblGlCashTransactionDetailCostCenter(int skip, int take, int TblGlCashTransactionDetail, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblGlCashTransactionDetailCostCenter> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblGlCashTransactionDetail ==(@Group0)";
                    valuesObjects.Add("Group0", TblGlCashTransactionDetail);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblGlCashTransactionDetailCostCenters.Where(filter, parameterCollection).Count();
                    query = entity.TblGlCashTransactionDetailCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(filter, parameterCollection).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblGlCashTransactionDetailCostCenters.Count(v => v.TblGlCashTransactionDetail == TblGlCashTransactionDetail);
                    query = entity.TblGlCashTransactionDetailCostCenters.Include("TblCostCenterType1").Include("TblCostCenter1").OrderBy(sort).Where(v => v.TblGlCashTransactionDetail == TblGlCashTransactionDetail).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblGlCashTransactionDetailCostCenter UpdateOrInsertTblGlCashTransactionDetailCostCenters(TblGlCashTransactionDetailCostCenter newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblGlCashTransactionDetailCostCenters.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblGlCashTransactionDetailCostCenters
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
        private int DeleteTblGlCashTransactionDetailCostCenter(TblGlCashTransactionDetailCostCenter row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblGlCashTransactionDetailCostCenters
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}