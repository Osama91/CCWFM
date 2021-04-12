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
        private List<TblSequenceProduction> GetTblSequenceProduction(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSequenceProduction> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.TblSequenceProductions.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.TblSequenceProductions.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = entity.TblSequenceProductions.Count();
                    query = entity.TblSequenceProductions.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSequenceProduction UpdateOrInsertTblSequenceProductions(TblSequenceProduction newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    entity.TblSequenceProductions.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblSequenceProductions
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
        private int DeleteTblSequenceProduction(TblSequenceProduction row, int index, string company)
        {
            using (var entity = new WorkFlowManagerDBEntities())
            {
                var query = (from e in entity.TblSequenceProductions
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
    
}