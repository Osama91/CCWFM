using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<tbl_lkp_AccessoryGroup> GetTblAccessoriesGroup(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_lkp_AccessoryGroup> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.tbl_lkp_AccessoryGroup.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tbl_lkp_AccessoryGroup.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_lkp_AccessoryGroup.Count();
                    query = context.tbl_lkp_AccessoryGroup.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private tbl_lkp_AccessoryGroup UpdateOrInsertTblAccessoriesGroup(tbl_lkp_AccessoryGroup newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.tbl_lkp_AccessoryGroup.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.tbl_lkp_AccessoryGroup
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
        private int DeleteTblAccessoriesGroup(tbl_lkp_AccessoryGroup row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tbl_lkp_AccessoryGroup
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<tbl_AccessoriesSubGroup> GetTblAccessoriesSubGroup(int skip, int take, int groupId, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<tbl_AccessoriesSubGroup> query;
                if (filter != null)
                {
                    filter = filter + " and it.GroupID ==(@Group0)";
                    valuesObjects.Add("Group0", groupId);

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.tbl_AccessoriesSubGroup.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.tbl_AccessoriesSubGroup.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.tbl_AccessoriesSubGroup.Count(v => v.GroupID == groupId);
                    query = context.tbl_AccessoriesSubGroup.OrderBy(sort).Where(v => v.GroupID == groupId).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private tbl_AccessoriesSubGroup UpdateOrInsertTblAccessoriesSubGroup(tbl_AccessoriesSubGroup newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.tbl_AccessoriesSubGroup.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.tbl_AccessoriesSubGroup
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
        private int DeleteTblAccessoriesSubGroup(tbl_AccessoriesSubGroup row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tbl_AccessoriesSubGroup
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblAccSubGroupSizeLink> GetTblAccSubGroupSizeLink(int subGroupIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblAccSubGroupSizeLinks.Include("TblAccSize1").Where(v => v.tbl_AccessoriesSubGroup == subGroupIserial);

                return query.ToList();
            }
        }

        [OperationContract]
        private void UpdateOrDeletedAccSubGroupSizeLink(TblAccSubGroupSizeLink newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    var oldRow = (from e in context.TblAccSubGroupSizeLinks
                                  where e.TblAccSize == newRow.TblAccSize
                                  && e.tbl_AccessoriesSubGroup == newRow.tbl_AccessoriesSubGroup
                                  select e).SingleOrDefault();
                    if (oldRow == null)
                    {
                        context.TblAccSubGroupSizeLinks.AddObject(newRow);
                    }
                }
                else
                {
                    var oldRow = (from e in context.TblAccSubGroupSizeLinks
                                  where e.TblAccSize == newRow.TblAccSize
                                  && e.tbl_AccessoriesSubGroup == newRow.tbl_AccessoriesSubGroup
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        context.DeleteObject(oldRow);
                    }
                }
                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_AccessoriesSubGroup> GetAccSubGroupByGroup(int groupId)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_AccessoriesSubGroup.Where(x => x.GroupID == groupId).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_lkp_ItemGroupType> GetAccItemGroupTypes()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_lkp_ItemGroupType.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_lkp_AccessoryGroup> GetAllAccGroup()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_lkp_AccessoryGroup.ToList();
            }
        }
    }
}