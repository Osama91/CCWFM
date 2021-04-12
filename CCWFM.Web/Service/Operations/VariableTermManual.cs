using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using Omu.ValueInjecter;

// ReSharper disable once CheckNamespace
namespace CCWFM.Web.Service
{
// ReSharper disable once InconsistentNaming
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<Model.TblVariableTermManual> GetTblVariableTermManual(List<string> empCodeList)
        {
            using (var context = new TimeAttEntities())
            {
                var query = context.TblVariableTermManuals.Where(x => empCodeList.Contains(x.Emplid));

                return query.ToList();
            }
        }

        private TblPeriod getPeriodByOrganizationUnit(int unit, PayrollEntities HREntities)
        {
            return HREntities.TblPeriods.FirstOrDefault(wde => wde.TblOrganizationGroups.Any(e => e.Iserial == unit));

        }

        private void deletePayrollVr(PayrollEntities payrolldb, Model.TblVariableTermManual Oldrc,string SelectedTerm)
        {
            if (Oldrc == null)
            {
                return;
            }
            var exOldList = payrolldb.PayrollTblVariableTermManuals.Where(w => w.TransDate == Oldrc.TransDate && w.TblEmployee1.EmpId == Oldrc.Emplid && w.TblSalaryTerm1.Ename== SelectedTerm).ToList();

            if (exOldList.Any())
            {
                foreach (var item in exOldList)
                {
                    payrolldb.DeleteObject(item);

                }
            }
        }

        [OperationContract]
        private Model.TblVariableTermManual UpdateOrInsertTblVariableTermManual(Model.TblVariableTermManual newRow, bool save, int index, out int outindex)
        {
            var oldRow = new Model.TblVariableTermManual();
            outindex = index;
            using (var context = new TimeAttEntities())
            {
                if (save)
                {
                    context.TblVariableTermManuals.AddObject(newRow);
                }
                else
                {
                         oldRow = (from e in context.TblVariableTermManuals
                                  where e.Iserial == newRow.Iserial
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        GenericUpdate(oldRow, newRow, context);
                    }
                }
                context.SaveChanges();

                var srv = new EmployeePayrollServiceClient();
                //newRow
                var row = new ServiceReference1.TblVariableTermManual();
                row.InjectFrom(newRow);
                srv.InsertVariableTerm(row, row);
              var selectedTerm=  context.TblSalaryTerms.FirstOrDefault(w => w.Iserial == newRow.TermId).Ename;
               



                    using (PayrollEntities payrolldb = new PayrollEntities())
                    {
                        if (oldRow != null)
                        {
                            deletePayrollVr(payrolldb, oldRow, selectedTerm);
                        }

                            var emp = payrolldb.TblEmployeePayrolls.FirstOrDefault(w => w.EmpId == newRow.Emplid);
                    var period = getPeriodByOrganizationUnit(emp.TblOrganizationUnit ?? 0, payrolldb).Iserial;
                    var periodline = payrolldb.TblPeriodLines.FirstOrDefault(x => x.FromDate <= newRow.TransDate && x.ToDate >= newRow.TransDate && x.TblPeriod == period);

                    var exType = payrolldb.PayrollTblSalaryTerms.FirstOrDefault(w => w.Ename == selectedTerm).Iserial;
                            if (emp != null && exType != null)
                            {
                                var ExToInsert = new PayrollTblVariableTermManual()
                                {
                                    TblEmployee = emp.Iserial,
                                    TblSalaryTerm= exType,
                                    Hours=newRow.Hours,
                                    AmountCUR=0,
                                    AttHours=0,
                                    AttType="0",
                                    ExcuseTransRECId=null,
                                    OldAmount=0,
                                    TblAttendaceFile=null,
                                    TblCalculated=0,
                                    TblCurrency=null,
                                    TblPenalty=null,
                                    TransDate=newRow.TransDate,
                                    FromAtt=false,
                                    TblPeriodLine= periodline.Iserial,
                                    

                                };
                                payrolldb.PayrollTblVariableTermManuals.AddObject(ExToInsert);
                            }
                    payrolldb.SaveChanges();
                }
                 
                    
               

                return newRow;
            }
        }

        [OperationContract]
        private int DeleteTblVariableTermManual(Model.TblVariableTermManual row)
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblVariableTermManuals
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        private List<Model.TblSalaryTerm> GetTblSalaryTerm()
        {
            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblSalaryTerms
                              select e).ToList();
                return oldRow;
            }
        }
    }
}