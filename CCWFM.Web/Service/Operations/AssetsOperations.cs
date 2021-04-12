using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblAsset> GetTblAssets(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, bool Pending, out List<int> PendingAssets)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                PendingAssets = context.TblAssetsTransactions.Where(w => w.ReturnDate == null && w.TblAsset.Disposable == false).Select(x => x.TblAssets).ToList();
                IQueryable<TblAsset> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblAssets.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAssets.Include("TblAssetsType1").Include("TblHardDisk1").Include("TblMemory1").Include("TblProcessor1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAssets.Count();
                    query = context.TblAssets.Include("TblAssetsType1").Include("TblHardDisk1").Include("TblMemory1").Include("TblProcessor1").OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAsset UpdateOrInsertTblAssets(TblAsset newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblAssets.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblAssets
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private string GetMaxAssets()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                string codeNoString = "";
                try
                {
                    int? codeNo =
                        context.TblAssets
                            .Select(x => x.Code).Cast<int?>().Max();

                    if (codeNo != null)
                    {
                        codeNo++;

                        var code = (int)codeNo;
                        codeNoString = code.ToString("0000");
                    }
                    else
                    {
                        codeNoString = "0001";
                    }
                }
                catch (Exception)
                {
                    codeNoString = "0001";
                }
                return codeNoString;
            }
        }

        [OperationContract]
        private void CopyAssets(TblAsset newRow, int times)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                for (int i = 0; i < times; i++)
                {
                    var row = new TblAsset
                    {
                        Aname = newRow.Aname,
                        Ename = newRow.Ename,
                        Notes = newRow.Notes,
                        Code = GetMaxAssets(),
                        PurchasePrice = newRow.PurchasePrice,
                        Disposable = newRow.Disposable,
                        YearOfProduct = newRow.YearOfProduct,
                        TblAssetsType = newRow.TblAssetsType,
                        TblHardDisk = newRow.TblHardDisk,
                        TblMemory = newRow.TblMemory,
                        TblProcessor = newRow.TblProcessor,
                        TechSpec = newRow.TechSpec,
                    };

                    context.TblAssets.AddObject(row);
                    context.SaveChanges();
                }
            }
        }

        [OperationContract]
        private int DeleteTblAssets(TblAsset row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAssets
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblAssetsTransaction> GetTblAssetsTransaction(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, out List<StoreForAllCompany> Stores, out List<Employee> employees)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblAssetsTransaction> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblAssetsTransactions.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblAssetsTransactions.Include("TblAsset").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblAssetsTransactions.Count();
                    query = context.TblAssetsTransactions.Include("TblAsset").OrderBy(sort).Skip(skip).Take(take);
                }

                var listOfEmp = query.Select(x => x.Empid);
                var listOfOrganizations = query.Select(x => x.OrganizationId);
                var listOfStores = query.Select(x => x.StoreCode);
                Stores =
                    context.StoreForAllCompanies.Where(
                        x => listOfOrganizations.Any(l => x.Organization == l && listOfStores.Any(w => x.Code == w))).ToList();

                employees = context.Employees.Where(x => listOfEmp.Any(l => x.EMPLID == l)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblAssetsTransaction UpdateOrInsertTblAssetsTransaction(TblAssetsTransaction newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var temp = (context.TblAssetsTransactions.Count(
                   e => e.TblAssets == newRow.TblAssets && e.ReturnDate == null));
                    if (temp > 0)
                    {
                        return null;
                    }

                    context.TblAssetsTransactions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblAssetsTransactions
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblAssetsTransaction(TblAssetsTransaction row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblAssetsTransactions
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
    }
}