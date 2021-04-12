using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<Entity> GetGlItemGroup(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return entity.GlItemGroup().ToList();
            }
        }

        [OperationContract]
        private List<GlInventoryGroup_Result> GetGlInventoryGroup(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                return entity.GlInventoryGroup().ToList();
            }
        }

        [OperationContract]
        private List<TblInventPosting> GetTblInventPosting(int tblInventAccountType, string company, out List<Entity> entityList, int JournalAccountType)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblInventPosting> query = entity.TblInventPostings.Include("TblAccount1").Where(x => x.TblInventAccountType == tblInventAccountType);
                List<int> intList = query.Select(x => x.SupCustRelation).ToList();

                List<int> intScopeList = query.Select(x => x.SupCustScope).ToList();

                entityList = entity.Entities.Where(x => intScopeList.Contains(x.scope) && x.TblJournalAccountType == JournalAccountType && intList.Contains(x.Iserial)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblInventAccountType> GetTblInventPostingType(string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                IQueryable<TblInventAccountType> query = entity.TblInventAccountTypes;

                return query.ToList();
            }
        }

        [OperationContract]
        private TblInventPosting UpdateOrInsertTblInventPostings(TblInventPosting newRow, bool save, int index, out int outindex, string company)
        {
            outindex = index;
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                if (save)
                {
                    entity.TblInventPostings.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in entity.TblInventPostings
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
        private int DeleteTblInventPosting(TblInventPosting row, int index, string company)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = (from e in entity.TblInventPostings
                             where e.Iserial == row.Iserial
                             select e).SingleOrDefault();
                if (query != null) entity.DeleteObject(query);

                entity.SaveChanges();
            }
            return row.Iserial;
        }
    }
}