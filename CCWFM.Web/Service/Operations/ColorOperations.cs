using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblColor> GetTblColor(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblColor> query;
                if (filter != null)
                {

                    filter = filter + " and it.TblLkpColorGroup <>(@colorGroup0)";
                    valuesObjects.Add("colorGroup0", 24);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblColors.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblColors.Include("TblLkpColorGroup1").Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblColors.Count(x => x.TblLkpColorGroup1.Code != "Acc");
                    query = context.TblColors.Include("TblLkpColorGroup1").OrderBy(sort).Where(x => x.TblLkpColorGroup1.Code != "Acc").Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblColor> SearchTblColor(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, bool acc)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblColor> query;
                if (filter != null)
                {
                    if (acc)
                    {
                        filter = filter + " and it.TblLkpColorGroup <>(@colorGroup0)";
                        valuesObjects.Add("colorGroup0", 24);
                    }
                    else
                    {
                        filter = filter + " and it.TblLkpColorGroup ==(@colorGroup0)";
                        valuesObjects.Add("colorGroup0", 24);
                    }

                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblColors.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblColors.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblColors.Count(x => (x.TblLkpColorGroup1.Code != "Acc" || acc == false) && (x.TblLkpColorGroup1.Code == "Acc" || acc));
                    query = context.TblColors.OrderBy(sort).Where(x => (x.TblLkpColorGroup1.Code != "Acc" || acc == false) && (x.TblLkpColorGroup1.Code == "Acc" || acc)).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblColor UpdateOrInsertTblColor(TblColor newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                var colorgroup = context.TblLkpColorGroups.FirstOrDefault(w => w.Iserial == newRow.TblLkpColorGroup);
                newRow.ColorItemType = colorgroup.ColorItemType;

                if (save)
                {

                    context.TblColors.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblColors
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
        private int DeleteTblColor(TblColor row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblColors
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        public List<TblColor> SearchForColor(string item)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblColors.Where(x => x.Code.StartsWith(item) && x.TblLkpColorGroup != 24).Take(30).ToList();
            }
        }
    }
}