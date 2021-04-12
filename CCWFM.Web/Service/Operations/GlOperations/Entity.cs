using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<Entity> GetEntity(int skip, int take, int tblJournalAccountType, int scope, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount, string company, int user, bool PreventNullAccount)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {

                var SettingExist = entity.TblAuthUserJournalSettings.Any(w => w.TblAuthUser == user);

                // var JournalList;
                List<GlPosting> JournalList = new List<GlPosting>();
                if (SettingExist)
                {
                    JournalList = entity.TblJournalSettingEntities.Where(w => w.TblJournalAccountType !=null &&
                  w.TblJournalSetting1.TblAuthUserJournalSettings.Any(e => e.TblAuthUser == user)).Select(w => new GlPosting { GroupIserial = w.TblJournalAccountType ?? 0, StoreIserial = w.EntityAccount ?? 0 ,ItemIserial=w.Scope??0}).ToList();
                }
                var AccountIserials = new List<int>();
                foreach (var item in JournalList)
                {
                    AccountIserials.Add(item.StoreIserial);
                }

      
                IQueryable<Entity> query;
                if (filter != null)
                {
                    if (JournalList.Any())
                    {
                        var counter = 0;
                        foreach (var variable in JournalList.Where(w=>w.GroupIserial== tblJournalAccountType).ToList())
                        {
                            counter++;
                            filter = filter + " and it.TblJournalAccountType ==(@TblJournalAccountType" + counter + ")";
                            valuesObjects.Add("TblJournalAccountType" + counter, variable.GroupIserial);
                            if (variable.ItemIserial == 0)
                            {
                                filter = filter + " and it.Iserial ==(@StoreIserial" + counter + ")";
                                valuesObjects.Add("StoreIserial" + counter, variable.StoreIserial);
                            }
                            else
                            {
                                filter = filter + " and it.GroupIserial ==(@StoreIserial" + counter + ")";
                                valuesObjects.Add("StoreIserial" + counter, variable.StoreIserial);
                            }                          
                        }
                    }


                    filter = filter + " and it.TblJournalAccountType ==(@Group0)";
                    valuesObjects.Add("Group0", tblJournalAccountType);

                    filter = filter + " and it.scope ==(@scope0)";
                    valuesObjects.Add("scope0", scope);
                    if (PreventNullAccount)
                    {
                        filter = filter + " and it.AccountIserial !=(@Pre0)";
                        valuesObjects.Add("Pre0", 0);
                    }
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = entity.Entities.Where(filter, parameterCollection.ToArray()).Count();
                    query = entity.Entities.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
              
                    if (JournalList.Any())
                    {
                        fullCount = entity.Entities.Count(x => AccountIserials.Contains(x.GroupIserial??0)&&  x.TblJournalAccountType == tblJournalAccountType && x.scope == scope && (x.AccountIserial != 0 || PreventNullAccount == false));
                        query = entity.Entities
                       .OrderBy(sort).Where(v => AccountIserials.Contains(v.GroupIserial ?? 0) && v.TblJournalAccountType == tblJournalAccountType && v.scope == scope && (v.AccountIserial != 0 || PreventNullAccount == false)).Skip(skip).Take(take);
                        //query = entity.TblJournals.Include("TblCurrency1").Include("TblSequence").Include("TblSequence1").Include("TblJournalType1").Include("TblJournalAccountType1")
                        //    .OrderBy(sort).Where(w => JournalList.Contains(w.Iserial)).Skip(skip).Take(take);
                    }
                    else
                    {
                        fullCount = entity.Entities.Count(x => x.TblJournalAccountType == tblJournalAccountType && x.scope == scope && (x.AccountIserial != 0 || PreventNullAccount == false));
                        query = entity.Entities
                            .OrderBy(sort).Where(v => v.TblJournalAccountType == tblJournalAccountType && v.scope == scope && (v.AccountIserial != 0 || PreventNullAccount == false)).Skip(skip).Take(take);

                    }
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private Entity FindEntityByCode(int tblJournalAccountType, string code, int scope, string company, bool PreventNullAccount)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.Entities
             .FirstOrDefault(v => v.TblJournalAccountType == tblJournalAccountType && v.Code == code && v.scope == scope && (v.AccountIserial != 0 || PreventNullAccount == false));

                return query;
            }
        }

        [OperationContract]
        private Entity FindEntity(int tblJournalAccountType, int iserial, int scope, string company, bool PreventNullAccount)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var query = entity.Entities
             .FirstOrDefault(v => v.TblJournalAccountType == tblJournalAccountType && v.scope == scope && v.Iserial == iserial && (v.AccountIserial != 0 || PreventNullAccount == false));

                return query;
            }
        }

    }
}