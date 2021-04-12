using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
namespace CCWFM.Web.Service.BankDepositOp
{
    public partial class BankDepositService
    {
        [OperationContract]
        private List<TblSalaryApprovalHeader> GetSalaryApprovalHeader(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects, int userIserial, out int fullCount, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblSalaryApprovalHeader> query;
                if (filter != null)
                {
                    //filter = filter + " and it.Code LIKE(@Code0)";
                    //valuesObjects.Add("Code0", accCode);
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblSalaryApprovalHeaders
                    .Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalaryApprovalHeaders.Include(nameof(TblSalaryApprovalHeader.TblStore1)).Include(nameof(TblSalaryApprovalHeader.TblSalaryType1))
                        .Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalaryApprovalHeaders.Count();
                    query = context.TblSalaryApprovalHeaders.Include(nameof(TblSalaryApprovalHeader.TblStore1)).Include(nameof(TblSalaryApprovalHeader.TblSalaryType1)).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalaryApprovalHeader UpdateOrInsertSalaryApprovalHeader(TblSalaryApprovalHeader newRow, int index, int userIserial, out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                try
                {
                    var oldRow = context.TblSalaryApprovalHeaders.FirstOrDefault(th => th.Iserial == newRow.Iserial);
                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        oldRow.StoreApproved = newRow.StoreApproved;
                        //foreach (var item in newRow.TblSalaryApprovalDetails.ToArray())
                        //{
                        //    // هشوف بقى الى اتعدل والجديد
                        //    int temp;
                        //    //headeriserial = item.TblSalaryApprovalHeader;
                        //    item.TblSalaryApprovalHeader1 = null;
                        //    item.TblSalaryApprovalHeader = newRow.Iserial;
                        //    UpdateOrInsertSalaryApprovalDetail(item, userIserial, 1, out temp,company);
                        //    item.TblSalaryApprovalHeader1Reference = null;
                        //}
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);                   
                    }
                    else// الهيدر ده جديد
                    {

                        context.TblSalaryApprovalHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex) { throw ex; }


                return newRow;
            }
        }

        [OperationContract]
        private List<TblSalaryApprovalDetail> GetSalaryApprovalDetail(int skip, int take, int ledgerHeader, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<TblEmployee> EmpList, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                IQueryable<TblSalaryApprovalDetail> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblSalaryApprovalHeader ==(@Group0)";
                    valuesObjects.Add("Group0", ledgerHeader);

                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);
                    fullCount = context.TblSalaryApprovalDetails.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSalaryApprovalDetails.OrderBy(sort).Where(filter, parameterCollection.ToArray()).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSalaryApprovalDetails.Count(v => v.TblSalaryApprovalHeader == ledgerHeader);
                    query = context.TblSalaryApprovalDetails.OrderBy(sort).Where(v => v.TblSalaryApprovalHeader == ledgerHeader).Skip(skip).Take(take);
                }

                List<int> intList = query.Select(x => x.TblEmployee).ToList();

                EmpList = context.TblEmployees.Where(x => intList.Contains(x.iserial)).ToList();


                return query.ToList();
            }
        }

        [OperationContract]
        private TblSalaryApprovalDetail UpdateOrInsertSalaryApprovalDetail(
            TblSalaryApprovalDetail newRow, int userIserial, int index, out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblSalaryApprovalDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    var result = SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblSalaryApprovalDetails.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteSalaryApprovalDetail(TblSalaryApprovalDetail row, int userIserial, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblSalaryApprovalDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    context.DeleteObject(oldRow);
                    context.SaveChanges();
                }
            }
            return row.Iserial;
        }
        [OperationContract]
        private void ImportSalaryApproval(List<TblSalaryApprovalHeader> List, int userIserial, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                foreach (var item in List)
                {
                    var store = 0;
                    var salarytype = 0;
                    try
                    {
                        store = context.TblStores.FirstOrDefault(w => w.code == item.TblStore1.code).iserial;
                        salarytype = context.TblSalaryTypes.FirstOrDefault(w => w.EName == item.TblSalaryType1.EName).Iserial;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    item.TblStore1 = null;
                    item.TblSalaryType1 = null;
                    var headerRow = context.TblSalaryApprovalHeaders.FirstOrDefault(w => w.Code == item.Code && w.TblStore == store && w.Month == item.Month && w.Year == item.Year && w.TblSalaryType == salarytype);
                    if (headerRow == null)
                    {
                        var newHeader = new TblSalaryApprovalHeader()
                        {
                            Month = item.Month,
                            TblSalaryType = salarytype,
                            TblStore = store,
                            Year = item.Year,
                            TblUser = 0,
                            VotDate = DateTime.Now,
                            CreationDate = DateTime.Now,
                            HQApproved = true,
                            StoreApproved = false,
                            Code = item.Code,
                            DueDate = item.DueDate
                        };
                        newHeader.TblSalaryApprovalDetails = new System.Data.Objects.DataClasses.EntityCollection<TblSalaryApprovalDetail>();
                        headerRow = newHeader;
                    }
                    foreach (var SalaryDetail in item.TblSalaryApprovalDetails)
                    {
                        var empcode = SalaryDetail.TblEmployee.ToString();
                        try
                        {
                            var Employee = context.TblEmployees.FirstOrDefault(e => e.code == empcode).iserial;
                            var newDetail = new TblSalaryApprovalDetail()
                            {
                                Approved = false,
                                CreationDate = DateTime.Now,
                                Salary = SalaryDetail.Salary,
                                TblEmployee = Employee,
                            };
                            headerRow.TblSalaryApprovalDetails.Add(newDetail);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    if (headerRow.Iserial == 0)
                    {
                        context.TblSalaryApprovalHeaders.AddObject(headerRow);
                    }

                }
                context.SaveChanges();
            }
        }
    }
}