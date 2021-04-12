using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private TblBrandStoreTargetHeader GetTblBrandStoreTargetHeader(int month, int brand, int year, string brandCode, string company, out List<StoresForBrand> Stores)
        {
            using (var model = new WorkFlowManagerDBEntities())
            {
                Stores = model.StoresForBrands.Where(x => x.TblBrandCode == brandCode).ToList();
            }
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return context.TblBrandStoreTargetHeaders.Include("TblBrandStoreTargetDetails.TblStore1").FirstOrDefault(x => x.Month == month && x.TblItemDownLoadDef == brand && x.Year == year);
            }
        }

        [OperationContract]
        private List<TblBrandStoreTargetHeader> GetTblBrandStoreTargetHeaderForManagment(int skip, int take, string sort, string filter,
             Dictionary<string, object> valuesObjects,string company, out int fullCount)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblBrandStoreTargetHeader> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblBrandStoreTargetHeaders.Include("TblItemDownLoadDef1").Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblBrandStoreTargetHeaders.Include("TblItemDownLoadDef1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblBrandStoreTargetHeaders.Include("TblItemDownLoadDef1").Count();
                    query = context.TblBrandStoreTargetHeaders.Include("TblItemDownLoadDef1").OrderBy(sort).Skip(skip).Take(take);
                }

                return query.ToList();
            }
        }

        [OperationContract]
        private List<StoresForBrand> GetStoresForbrand(string brandCode)
        {
            using (var model = new WorkFlowManagerDBEntities())
            {
                return model.StoresForBrands.Where(x => x.TblBrandCode == brandCode).ToList();
            }
        }

        [OperationContract]
        private TblBrandStoreTargetHeader UpdateOrInsertTblBrandStoreTargetHeader(TblBrandStoreTargetHeader newRow, bool save, int index,string company,
            out int outindex)
        {
            outindex = index;

            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    context.TblBrandStoreTargetHeaders.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblBrandStoreTargetHeaders
                                  where e.Year == newRow.Year
                                  && e.TblItemDownLoadDef == newRow.TblItemDownLoadDef
                                  && e.Month == newRow.Month
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
        private TblBrandStoreTargetDetail UpdateOrInsertTblBrandStoreTargetDetail(TblBrandStoreTargetDetail newRow, bool save, int index,string company,
            out int outindex)
        {
            outindex = index;

            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    context.TblBrandStoreTargetDetails.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblBrandStoreTargetDetails
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
        private int DeleteTblBrandStoreTargetDetail(TblBrandStoreTargetDetail row, string company)
        {
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBrandStoreTargetDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private void SendMailBrandStoreTarget(int month, int brand, int year, string brandCode, string company)
        {
            List<StoresForBrand> stores;
            using (var model = new WorkFlowManagerDBEntities())
            {
                stores = model.StoresForBrands.Where(x => x.TblBrandCode == brandCode).ToList();
            }
            using (var context = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var storesIntList = stores.Select(x => x.Iserial).ToList();

                var detail = context.TblBrandStoreTargetDetails.Include("TblStore1")
                    .Where(
                        x =>
                            storesIntList.Contains((int)x.Tblstore) && x.TblBrandStoreTargetHeader.Month == month &&
                            x.TblBrandStoreTargetHeader.Year == year)
                    .GroupBy(w => w.Tblstore)
                    .ToDictionary(x => x.Key, x => x.Sum(w => w.Amount));

                foreach (var row in stores)
                {
                    var detailrow = detail.FirstOrDefault(x => x.Key == row.Iserial);

                    SendEmail(null, "Retail@ccausal.loc", null, "Budget For " + month + "/" + year, detailrow.Value.ToString());
                }
            }
        }
    }
}