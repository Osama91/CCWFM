using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblBrandSectionMailSample> GetTblBrandSectionMailSample(string brand, int tblLkpBrandSection, out List<Employee> EmpList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblBrandSectionMailSample> query;

                query = context.TblBrandSectionMailSamples.Include("TblRequestForSampleStatu").Where(x => x.TblBrand == brand && x.TblLkpBrandSection == tblLkpBrandSection);

                var emp = query.Select(x => x.Emp);
                EmpList = context.Employees.Where(x => emp.Contains(x.EMPLID)).ToList();
                return query.ToList();
            }
        }

        [OperationContract]
        private TblBrandSectionMailSample UpdateOrInsertTblBrandSectionMailSample(TblBrandSectionMailSample newRow, bool save, int index, out int outindex)
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (save)
                {
                    context.TblBrandSectionMailSamples.AddObject(newRow);
                }
                else
                {
                    var oldRow = (from e in context.TblBrandSectionMailSamples
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
        private TblBrandSectionMailSample DeleteTblBrandSectionMailSample(TblBrandSectionMailSample row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblBrandSectionMailSamples
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
    }
}