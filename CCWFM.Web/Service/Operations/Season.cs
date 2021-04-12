using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblLkpSeason> Seasons()
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var seasons = (from s in entities.TblLkpSeasons
                               join c in entities.TblColorLinks
                               on s.Iserial equals c.TblLkpSeason
                               select s).Distinct().ToList();
                return seasons;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblLkpSeason> GetAllSeasonsByUser(int tblUser)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var UserJob = context.TblAuthUsers.FirstOrDefault(x => x.Iserial == tblUser).TblJob;
                var GetAllSeasons = context.TblAuthJobPermissions.FirstOrDefault(x => x.Tbljob == UserJob && x.TblAuthPermission.Code == "ReturnAllSeasons");
                if (GetAllSeasons != null)
                {
                    return context.TblLkpSeasons.Include("TblSeasonTracks.TblSeasonTrackType1").OrderBy(x => x.Iserial).ToList();
                }
                return context.TblLkpSeasons.Include("TblSeasonTracks.TblSeasonTrackType1").Where(x=>x.IsActive == true).OrderBy(x=>x.Iserial).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblLkpSeason> GetAllSeasons()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblLkpSeasons.Include("TblSeasonTracks.TblSeasonTrackType1").ToList();
            }
        }

        [OperationContract]
        private List<TblLkpSeason> GetTblLkpSeason(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblLkpSeason> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblLkpSeasons.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblLkpSeasons.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblLkpSeasons.Count();
                    query = context.TblLkpSeasons.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblLkpSeason UpdateOrInsertTblLkpSeason(TblLkpSeason newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblLkpSeasons.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblLkpSeasons
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
        private int DeleteTblLkpSeason(TblLkpSeason row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblLkpSeasons
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSeasonTrack> GetTblSeasonTrack(int skip, int take, int season, string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblSeasonTrack> query;
                if (filter != null)
                {
                    filter = filter + " and it.TblLkpSeason ==(@Season0)";
                    valuesObjects.Add("Season0", season);
                    var parameterCollection = ConvertToParamters(valuesObjects);
                    fullCount = context.TblSeasonTracks.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSeasonTracks.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSeasonTracks.Count(v => v.TblLkpSeason == season);
                    query = context.TblSeasonTracks.OrderBy(sort).Where(v => v.TblLkpSeason == season).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        private TblSeasonTrack UpdateOrInsertTblSeasonTrack(TblSeasonTrack newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblSeasonTracks.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblSeasonTracks
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
        private int DeleteTblSeasonTrack(TblSeasonTrack row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblSeasonTracks
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<TblSeason> GetTblRetailSeason(int skip, int take, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new ccnewEntities())
            {
                IQueryable<TblSeason> query;
                if (filter != null)
                {
                    var parameterCollection = ConvertToParamters(valuesObjects);

                    fullCount = context.TblSeasons.Where(filter, parameterCollection.ToArray()).Count();
                    query = context.TblSeasons.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = context.TblSeasons.Count();
                    query = context.TblSeasons.OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }
    }
}