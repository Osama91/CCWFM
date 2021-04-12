using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using Omu.ValueInjecter;
using CSPATTENDANCEFILE = CCWFM.Web.Model.CSPATTENDANCEFILE;
using EmployeeInfo = CCWFM.Web.Model.EmployeeInfo;
using TblAttendanceFile = CCWFM.Web.Model.TblAttendanceFile;
using TblEmployeeShift = CCWFM.Web.Model.TblEmployeeShift;
using TblExcuse = CCWFM.Web.Model.TblExcuse;
using TblMission = CCWFM.Web.Model.TblMission;
using TblVacation = CCWFM.Web.Model.TblVacation;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace CCWFM.Web.Service.Att
{
    [ServiceContract]
    public class Transaction
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string SalaryTerm { get; set; }

    }
    public partial class AttService
    {
        [OperationContract]
        public void FixAtt()
        {
            //var srv = new EmployeePayrollServiceClient();
            //srv.InsertEmpAttNoExist();
            //srv.InsertEmpCalendarDeviationNoExist();
            //srv.InsertEmpExcuseNoExist();
            //srv.InsertEmpMissionNoExist();
            //srv.InsertEmpVacationNoExist();
        }
        [DataMember]
        public int count;
        [OperationContract]
        public int SaveFixAtt(ObservableCollection<Transaction> MyList, DateTime NewDate, out int SavedCount)
        {
            List<Transaction> ResultList = new List<Transaction>();
            try
            {
                using (var context = new TimeAttEntities())
                {
                    for (int x = 0; x < MyList.Count; x++)
                    {

                        Model.TblVariableTermManual term = new Model.TblVariableTermManual();
                        string Code = MyList[x].Code;
                        string SalaryTerm = MyList[x].SalaryTerm;
                        term.Emplid = new PayrollEntities().TblEmployeePayrolls.FirstOrDefault(i => i.EmpId == Code).EmpId;
                        var termDetail = context.TblSalaryTerms.Where(t => t.Ename == SalaryTerm);
                        term.TermId = termDetail.FirstOrDefault().Iserial;
                        term.TransDate = NewDate;
                        term.Days = 0;
                        term.Hours = MyList[x].Amount;
                        term.status = 1;
                        context.TblVariableTermManuals.AddObject(term);
                    }
                    return count = SavedCount = context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [OperationContract]
        public List<EmployeeInfo> GetTblEmloyeeInfo(string code)
        {
            using (var db = new TimeAttEntities())
            {
                var query = (from e in db.TblEmployeeBehalves
                             join eb in db.EmployeeInfoes
                                 on e.Emplid equals eb.EnrollNumber

                             orderby eb.EnrollNumber, eb.Adate descending
                             where e.Emplid == code
                                   || e.ManagerId == code
                                   || e.AttOperatorId == code

                             select eb);

                return query.ToList();
            }
        }

        #region Attendance

        [OperationContract]
        public List<TblAttendanceFile> GetTblAttendanceFileForEmp(List<string> empId, List<DateTime> dayes, out List<CSPATTENDANCEFILE> axAttendanceFileList, out List<PayrollTblAttendanceFile> PayrollAttendanceFile, bool pending)
        {
         

            using (var db = new PayrollEntities())
            {
                     PayrollAttendanceFile = db.PayrollTblAttendanceFiles.Include("TblEmployee1").Where(x => empId.Contains(x.TblEmployee1.EmpId) && dayes.Contains(x.TransDate)).ToList();
            }
            List<TblAttendanceFile> AttList = new List<TblAttendanceFile>();

            foreach (var item in PayrollAttendanceFile)
            {
                var newRecord = new TblAttendanceFile() {
                    Emplid = item.TblEmployee1.EmpId,
                    TransDate = item.TransDate,
                    ToTime = item.ToTime,
                    OrginalFromTime = item.OrginalFromTime,

                    OrginalInTime = item.OrginalToTime,
                    Status = item.Status ?? 0,

                    FromTime = item.FromTime,
                    Description = item.Description,
                    ApprovedBy = item.ApprovedBy,
                    CreatedBy = item.CreatedBy,
                    CreationDate = item.CreationDate,
                    ApprovedDate = item.ApprovedDate,
                    Iserial=item.Iserial
                    
                };
                AttList.Add(newRecord);
            }
            axAttendanceFileList = null;
            PayrollAttendanceFile = null;
            return AttList;
        }

        [OperationContract]
        public WORKCALENDARDATELINE GetCalendarTime(string empId, DateTime day)
        {
            TblEmployeeShift empcalendar;
            using (var db = new TimeAttEntities())
            {
                empcalendar = db.TblEmployeeShifts.Include("TblEmployeeShiftLookup1").FirstOrDefault(x => x.EmpId == empId && x.TransDate == day);
            }


            using (var db = new PayrollEntities())
            {
                if (empcalendar != null)
                {
                    //            var workCalendar = db.TblWorkCalendarDateLines.FirstOrDefault(x => x.TransDate == day && x.TblCalendar1 == empcalendar.TblEmployeeShiftLookup1.CalendarId);
                    //return db.WORKCALENDARDATELINEs.FirstOrDefault(x => x.CALENDARID == workCalendar.CALENDARID && x.TRANSDATE == day);

                    //var workCalendar = db.WORKCALENDARDATEs.FirstOrDefault(x => x.TRANSDATE == day && x.CALENDARID == empcalendar.TblEmployeeShiftLookup1.CalendarId);
               var workcalendarDateLine=     db.TblWorkCalendarDateLines.FirstOrDefault(x => x.TblCalendar1.Code == empcalendar.TblEmployeeShiftLookup1.CalendarId && x.TransDate == day);
                    if (workcalendarDateLine!=null)
                    {
                        var record = new WORKCALENDARDATELINE() {

                            CALENDARID = empcalendar.TblEmployeeShiftLookup1.CalendarId,
                            FROMTIME= workcalendarDateLine.FromTime,
                            TOTIME=workcalendarDateLine.ToTime,
                            TRANSDATE=workcalendarDateLine.TransDate,
                        };
                        return record;
                    }
                    else
                    {
                        return null;
                    }
                }
                string calendar = null;
                var workCalendardev = db.TblEmployeeCalendarsDeviations.FirstOrDefault(x => x.FromDate <= day && x.ToDate >= day && x.TblEmployee1.EmpId == empId);
                if (workCalendardev != null)
                {
                    var calendartoSelect = db.TblCalendars.FirstOrDefault(w => w.Iserial == workCalendardev.TblCalendarNew);
                    if (calendartoSelect != null)
                    {
                        calendar = calendartoSelect.Code;
                    }
                  
                }
                if (calendar == null || string.IsNullOrEmpty(calendar))
                {
                    var workcalendar = db.TblEmployeeCalendars.FirstOrDefault(x => x.FromDate <= day && x.ToDate >= day && x.TblEmployee1.EmpId == empId);
                    if (workcalendar != null)
                    {

                        var calendartoSelect = db.TblCalendars.FirstOrDefault(w => w.Iserial == workcalendar.TblCalendar);
                        if (calendartoSelect != null)
                        {
                            calendar = calendartoSelect.Code;
                        }

                        // calendar = workcalendar.CALENDARID;
                        if (calendar!=null)
                        {
                            var workcalendarDateLine = db.TblWorkCalendarDateLines.FirstOrDefault(x => x.TblCalendar1.Code == calendar && x.TransDate == day);

                            if (workcalendarDateLine != null)
                            {
                                var record = new WORKCALENDARDATELINE()
                                {

                                    CALENDARID = calendar,
                                    FROMTIME = workcalendarDateLine.FromTime,
                                    TOTIME = workcalendarDateLine.ToTime,
                                    TRANSDATE = workcalendarDateLine.TransDate,
                                };
                                return record;
                            }
                            else
                            {
                                return null;
                            }

                        }
              

                    }
                }
                return null;
      
              //  return db.WORKCALENDARDATELINEs.FirstOrDefault(x => x.CALENDARID == calendar && x.TRANSDATE == day.Date);
            }
            //using (var db = new configurationEntities())
            //{
            //    if (empcalendar != null)
            //    {
            //        var workCalendar = db.WORKCALENDARDATEs.FirstOrDefault(x => x.TRANSDATE == day && x.CALENDARID == empcalendar.TblEmployeeShiftLookup1.CalendarId);
            //        return db.WORKCALENDARDATELINEs.FirstOrDefault(x => x.CALENDARID == workCalendar.CALENDARID && x.TRANSDATE == day);
            //    }
            //    string calendar = null;
            //    var workCalendardev = db.CSPEMPLOYEECALENDARSDEVIATIONS.FirstOrDefault(x => x.FROMDATE <= day && x.TODATE >= day && x.EMPLID == empId);
            //    if (workCalendardev != null)
            //    {
            //        calendar = workCalendardev.CALENDARIDNEW;
            //    }
            //    if (calendar == null || string.IsNullOrEmpty(calendar))
            //    {
            //        var workcalendar = db.CSPEMPLOYEECALENDARS.FirstOrDefault(x => x.FROMDATE <= day && x.TODATE >= day && x.EMPLID == empId);
            //        if (workcalendar != null)
            //        {
            //            calendar = workcalendar.CALENDARID;
            //        }
            //    }
            //    return db.WORKCALENDARDATELINEs.FirstOrDefault(x => x.CALENDARID == calendar && x.TRANSDATE == day.Date);
            //}
        }
        
        [OperationContract]
        public TblAttendanceFile UpdateAndInsertTblAttendanceFile(TblAttendanceFile row, bool save, int index, out int outindex, int userIserial)
        {
            outindex = index;

         
           

            if (!EmpCanEdit(row.TransDate, row.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }

            //using (var db = new TimeAttEntities())
            //{



            //    var oldRow = (from e in db.TblAttendanceFiles
            //                  where e.Emplid == row.Emplid
            //                  && e.TransDate == row.TransDate
            //                  select e).SingleOrDefault();

            //    if (oldRow != null)
            //    {
            //        if (row.CreationDate == DateTime.MinValue)
            //        {
            //            row.CreatedBy = userIserial;
            //            row.CreationDate = DateTime.Now;
            //        }
            //        row.Iserial = oldRow.Iserial;
            //        Operations.SharedOperation.GenericUpdate(oldRow, row, db);
            //    }
            //    else
            //    {
            //        int limit = 0;
            //        var count = AttendanceCount(row.Emplid, row.TransDate, out limit) + 1;
            //        if (count > limit && limit != 0)
            //        {
            //            row.Status = 5;
            //            return row;
            //        }
            //        row.CreatedBy = userIserial;
            //        row.CreationDate = DateTime.Now;

            //        db.TblAttendanceFiles.AddObject(row);
            //    }

            //    db.SaveChanges();
            //}

            //if (row.Status == 1)
            //{
            int limit = 0;
            var count = AttendanceCount(row.Emplid, row.TransDate, out limit) + 1;
            if (count > limit && limit != 0)
            {
                row.Status = 5;
                return row;
            }
            var srv = new EmployeePayrollServiceClient();

                var listOfAttFile = new List<ServiceReference1.TblAttendanceFile>
                {
                    new ServiceReference1.TblAttendanceFile().InjectFrom(row) as ServiceReference1.TblAttendanceFile
                };

                if (listOfAttFile.Any())
                {
                    //srv.InsertAttFile(listOfAttFile);
                }

                //Insert Attendance File on Payroll 
                if (listOfAttFile.Any())
                {
                    using (PayrollEntities db = new PayrollEntities())
                    {
                        foreach (var attfile in listOfAttFile)
                        {
                            var tblemployee = GetEmployeeIserialByCode(db, attfile.Emplid);
                            var oldattfile = db.PayrollTblAttendanceFiles.FirstOrDefault(w => w.TblEmployee == tblemployee&&w.TransDate== row.TransDate);
                            if (oldattfile!=null)
                            {
                                oldattfile.FromTime = row.FromTime??0;
                                oldattfile.ToTime = row.ToTime??0;
                                oldattfile.Manual = true;
                            oldattfile.Status = row.Status;
                            }
                            else
                            {
                                PayrollTblAttendanceFile _file = new PayrollTblAttendanceFile();
                                _file.TblEmployee = tblemployee;
                                _file.FromTime = row.FromTime??0 ;
                                _file.ToTime = row.ToTime??0;
                                _file.TransDate = attfile.TransDate;
                                _file.Manual = true;
                                _file.Status = row.Status;
                                
                                db.AddToPayrollTblAttendanceFiles(_file);
                            }                         
                        }
                        db.SaveChanges();
                    }
                //}
            }

            return row;
        }

        private int GetEmployeeIserialByCode(PayrollEntities db, string EmpID)
        {
            return db.TblEmployeePayrolls.FirstOrDefault(x => x.EmpId == EmpID).Iserial;
        }


        [OperationContract]
        private int DeleteTblAttendanceFile(TblAttendanceFile row)
        {
            if (!EmpCanEdit(row.TransDate, row.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            //using (var db = new TimeAttEntities())
            //{
            //    var oldRow = (from e in db.TblAttendanceFiles
            //                  where e.Emplid == row.Emplid
            //                     && e.TransDate == row.TransDate
            //                  select e).SingleOrDefault();
            //    if (oldRow != null)
            //    {
            //        var srv = new EmployeePayrollServiceClient();

            //        using (PayrollEntities payrolldb = new PayrollEntities())
            //        {
            //            deletePayrollAt(payrolldb, oldRow);
            //            payrolldb.SaveChanges();
            //        }

            //      //  srv.DeleteAttFile(row.Emplid, row.TransDate);

            //        db.DeleteObject(oldRow);
            //    }

            //    db.SaveChanges();
            //}

            using (PayrollEntities payrolldb = new PayrollEntities())
            {
                deletePayrollAt(payrolldb, row);
                payrolldb.SaveChanges();
            }


            return row.Iserial;
        }

        [OperationContract]
        public int AttendanceCount(string emp, DateTime date, out int limit)
        {
            using (var db = new PayrollEntities())
            {

            var period=    db.TblEmployeePayrolls.Include("TblOrganizationUnit1").FirstOrDefault(w => w.EmpId == emp).TblOrganizationUnit1.TblPeriod;
                    var periodRecord = db.TblPeriods.FirstOrDefault(w => w.Iserial == period);
                    limit = periodRecord.AttModificationLimit??0;
                
                    return 0;
            }
        }

        #endregion Attendance

        #region Excuse

        
        [OperationContract]
        public List<TblExcuse> GetTblExcuses(int skip, int take, string empId, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var db = new TimeAttEntities())
            {
                IQueryable<TblExcuse> query;
                if (filter != null)
                {
                    filter = filter + " and it.Emplid ==(@Emplid0)";
                    valuesObjects.Add("Emplid0", empId);

                    var parameterCollection = Operations.SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = db.TblExcuses.Where(filter, parameterCollection.ToArray()).Count();
                    query = db.TblExcuses.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = db.TblExcuses.Count(z => z.Emplid == empId);
                    query = db.TblExcuses.Where(z => z.Emplid == empId).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblExcuse> GetTblExcusesForStores(List<string> empId, List<DateTime> dayes, bool pending)
        {
            using (var db = new TimeAttEntities())
            {
                var query = db.TblExcuses.Where(x => empId.Contains(x.Emplid) && dayes.Contains(x.TransDate) && (x.Status == 0 || pending == false));
                return query.ToList();
            }
        }

        [OperationContract]
        public List<ExcuseType> ExcuseType()
        {
            using (var db = new TimeAttEntities())
            {
                var q = db.ExcuseTypes.ToList();

                return q;
            }
        }

        [OperationContract]
        public Dictionary<decimal, int> ExcuseCount(string emp, string period, decimal periodLine,DateTime date)
        {

            var periodline = new TblPeriodLine();
            using (var db = new PayrollEntities())
            {
                     periodline = db.TblPeriodLines.FirstOrDefault(x => x.FromDate <= date && x.ToDate >= date && x.TblPeriod1.Code == period);
    
            }

            using (var db = new TimeAttEntities())
            {
                //Check Excuse Limit 

               
                    var ExcuseCount = db.TblExcuses.Count(x =>x.Emplid==emp && x.TransDate>= periodline.FromDate&& x.TransDate<= periodline.ToDate&& x.CSPEXCUSEID != "ChristianFeast" && x.CSPEXCUSEID != "Deduction");
                var test = new Dictionary<decimal, int> { { periodLine, ExcuseCount } };

                return test;
            }
          }

        [OperationContract]
        public TblExcuse UpdateAndInsertTblExcuse(TblExcuse newRow, bool save, int index, out int outindex, string period, decimal periodLine, int userIserial)
        {
            outindex = index;
            if (!EmpCanEdit(newRow.TransDate, newRow.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            var createdbyy = 0;
            if (userIserial == -1)
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    var firstOrDefault = context.TblAuthUsers.FirstOrDefault(w => w.Code == newRow.Emplid);
                    if (firstOrDefault != null)
                        createdbyy = firstOrDefault.Iserial;
                }
            }

            using (var db = new TimeAttEntities())
            {
                //Check Excuse Limit 

                //var ExcuseTimeLimit = db.TblPeriodExcuseLimits.FirstOrDefault(x => x.TblPeriod == period);
                //if (ExcuseTimeLimit != null)
                //{

                //    decimal MaxExcuseLimit = ExcuseTimeLimit.MaxExcuseMinLimit.Value * 60;
                //    if ((newRow.FromTime + MaxExcuseLimit) > newRow.ToTime)
                //    {
                //        newRow.Status = 6;
                //        return newRow;
                //    }
                //}




                var oldExcuseList = new List<ServiceReference1.TblExcuse>();


                var oldRow = (from e in db.TblExcuses
                              where e.Iserial == newRow.Iserial

                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.Iserial = oldRow.Iserial;
                    if (newRow.CSPEXCUSEID != "Deduction" && newRow.CSPEXCUSEID != "ChristianFeast")
                    {
                        var test = 0;
                        var rules = GetTblExcuseRule(0, int.MaxValue, "it.Iserial", null, null, out test);
                        var excuseCounts = ExcuseCount(newRow.Emplid, period, periodLine, newRow.TransDate);

                        if (excuseCounts.FirstOrDefault().Value >
                            rules.FirstOrDefault(x => x.PeriodId == period).CounterPerPeriodLine)
                        {
                            newRow.Status = 5;
                            return newRow;
                        }
                    }
                    
                    if (newRow.Status == 1)
                    {
                        oldExcuseList.Add((ServiceReference1.TblExcuse)new ServiceReference1.TblExcuse().InjectFrom(oldRow));
                    }

                    if (newRow.CreationDate == DateTime.MinValue)
                    {
                        newRow.CreatedBy = userIserial;
                        if (userIserial == -1)
                        {
                            newRow.CreatedBy = createdbyy;
                        }
                        newRow.CreationDate = DateTime.Now;
                    }
                    Operations.SharedOperation.GenericUpdate(oldRow, newRow, db);
                }
                else
                {
                    var test = 0;
                    var rules = GetTblExcuseRule(0, int.MaxValue, "it.Iserial", null, null, out test);
                    //int pendingExcuses = db.TblExcuses.Count(
                    //x =>
                    //    x.Emplid == newRow.Emplid && x.TransDate.Month == newRow.TransDate.Month &&
                    //    x.TransDate.Year == newRow.TransDate.Year && x.Status != 1 && x.CSPEXCUSEID != "Deduction" && x.CSPEXCUSEID != "ChristianFeast");

                    if (newRow.CSPEXCUSEID != "Deduction" && newRow.CSPEXCUSEID != "ChristianFeast")
                    {
                        // till the new period
                        var excuseCounts = ExcuseCount(newRow.Emplid, period, periodLine, newRow.TransDate);

                        if ((excuseCounts.FirstOrDefault().Value ) >= rules.FirstOrDefault(x => x.PeriodId == period).CounterPerPeriodLine)
                        {
                            //if ((pendingExcuses) >= rules.FirstOrDefault(x => x.PeriodId == period).CounterPerPeriodLine)
                            //{
                            newRow.Status = 5;
                            return newRow;
                        }
                    }
                    newRow.CreatedBy = userIserial;
                    if (userIserial == -1)
                    {
                        newRow.CreatedBy = createdbyy;
                    }
                    newRow.CreationDate = DateTime.Now;
                    db.TblExcuses.AddObject(newRow);
                }

                if (newRow.Status == 1)
                {
                    var srv = new EmployeePayrollServiceClient();

                    var listOfExcuse = new List<ServiceReference1.TblExcuse>
                {
                    new ServiceReference1.TblExcuse().InjectFrom(newRow) as ServiceReference1.TblExcuse
                };

                    if (listOfExcuse.Any() || oldExcuseList.Any())
                    {
                        try
                        {
                       //     srv.InsertEmpExcuse(listOfExcuse, oldExcuseList);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    //New Excuse In Payroll
                    if (listOfExcuse.Any() || oldExcuseList.Any())
                    {
                     
                            using (PayrollEntities payrolldb = new PayrollEntities())
                            {
                            deletePayrollEx(payrolldb, oldExcuseList);

                                foreach (var NewEx in listOfExcuse)
                                {
                                    var emp = payrolldb.TblEmployeePayrolls.FirstOrDefault(w => w.EmpId == NewEx.Emplid);
                                    var exType = payrolldb.PayrollTblExcuses.FirstOrDefault(w => w.Ename == NewEx.CSPEXCUSEID);
                                    if (emp != null && exType != null)
                                    {
                                        var ExToInsert = new TblExcuseTran()
                                        {
                                            TblEmployee = emp.Iserial,
                                            Counter = 1,
                                            Description = NewEx.Description,
                                            ToTime = NewEx.ToTime,
                                            TransDate = NewEx.TransDate,
                                            FromTime = NewEx.FromTime,
                                            Status = 1,
                                            TblExcuse = exType.Iserial,

                                        };
                                        payrolldb.TblExcuseTrans.AddObject(ExToInsert);


                                    }
                                }
                                payrolldb.SaveChanges();

                            }
                      
              
                    }
                }
                else if (oldRow != null && (newRow.Status != 1))
                {
                    using (PayrollEntities payrolldb = new PayrollEntities())
                    {
                        deletePayrollEx(payrolldb, oldExcuseList);
                        payrolldb.SaveChanges();
                    }
                    //    var srv = new EmployeePayrollServiceClient();
                    //srv.DeleteEmpExcuse(oldRow.Emplid, oldRow.TransDate, oldRow.FromTime, oldRow.ToTime);
                }
                db.SaveChanges();
                return newRow;
            }
        }

        private void deletePayrollEx(PayrollEntities payrolldb , List<ServiceReference1.TblExcuse> oldExcuseList ) {
            foreach (var oldEx in oldExcuseList)
            {
                var exOldList = payrolldb.TblExcuseTrans.Where(w => w.TransDate == oldEx.TransDate && w.TblEmployee1.EmpId == oldEx.Emplid && w.FromTime == oldEx.FromTime).ToList();

                if (exOldList.Any())
                {
                    foreach (var item in exOldList)
                    {
                        payrolldb.DeleteObject(item);

                    }
                }
            }
        }

        private void deletePayrollVc(PayrollEntities payrolldb, TblVacation Oldrc)
        {
            if (Oldrc == null)
            {
                return;
            }
                var exOldList = payrolldb.TblVacationTrans.Where(w => w.FromDate == Oldrc.FromDate && w.TblEmployee1.EmpId == Oldrc.Emplid).ToList();

            if (exOldList.Any())
            {
                foreach (var item in exOldList)
                {
                    payrolldb.DeleteObject(item);

                }
            }                        
        }
        private void deletePayrollAt(PayrollEntities payrolldb, TblAttendanceFile Oldrc)
        {
            if (Oldrc == null)
            {
                return;
            }
            var exOldList = payrolldb.PayrollTblAttendanceFiles.Where(w => w.TransDate == Oldrc.TransDate && w.TblEmployee1.EmpId == Oldrc.Emplid);

            if (exOldList.Any())
            {
                foreach (var item in exOldList.ToList())
                {
                    payrolldb.DeleteObject(item);

                }
            }
        }

        private void deletePayrollMS(PayrollEntities payrolldb, TblMission Oldrc)
        {

            if (Oldrc == null)
            {
                return;
            }
            var exOldList = payrolldb.TblMissionTrans.Where(w => w.FromDate == Oldrc.FromDate && w.TblEmployee1.EmpId == Oldrc.Emplid).ToList();

            if (exOldList.Any())
            {
                foreach (var item in exOldList)
                {
                    payrolldb.DeleteObject(item);

                }
            }

        }
        [OperationContract]
        private int DeleteTblExcuse(TblExcuse row)
        {
            if (!EmpCanEdit(row.TransDate, row.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            using (var db = new TimeAttEntities())
            {
                var oldRow = (from e in db.TblExcuses
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    if (oldRow.Status == 1)
                    {
                        using (PayrollEntities payrolldb = new PayrollEntities())
                        {
                            var exList = new List<ServiceReference1.TblExcuse>();
                         exList.Add((ServiceReference1.TblExcuse)new ServiceReference1.TblExcuse().InjectFrom(oldRow));
                            
                            deletePayrollEx(payrolldb, exList);
                            payrolldb.SaveChanges();
                        }
                        //var srv = new EmployeePayrollServiceClient();
                        //srv.DeleteEmpExcuse(row.Emplid, row.TransDate, row.FromTime, row.ToTime);
                    }

                    db.DeleteObject(oldRow);
                }

                db.SaveChanges();
            }
            return row.Iserial;
        }


        [OperationContract]
        private decimal GetExcuseMaxTimeLimit(string Emplid)
        {
            using (var db = new TimeAttEntities())
            {
               var period = db.EmployeesViews.FirstOrDefault(x => x.Emplid == Emplid).Period;
               if(!string.IsNullOrEmpty(period))
               {
                    var periodLimit = db.TblPeriodExcuseLimits.FirstOrDefault(x => x.TblPeriod == period);
                    if (periodLimit != null)
                    {
                        return periodLimit.MaxExcuseMinLimit.Value;
                    }
               }
            }
            return 0;
        }

        #endregion Excuse

        #region Mission

        [OperationContract]
        public List<TblMission> GetTblMissionForStores(List<string> empId, List<DateTime> dayes, bool pending)
        {
            using (var db = new TimeAttEntities())
            {
                var query = db.TblMissions.Where(x => empId.Contains(x.Emplid) && dayes.Contains(x.FromDate) && dayes.Contains(x.ToDate) && (x.Status == 0 || pending == false));
                return query.ToList();
            }
        }

        [OperationContract]
        public List<TblMission> GetTblMission(int skip, int take, string empId, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var db = new TimeAttEntities())
            {
                IQueryable<TblMission> query;
                if (filter != null)
                {
                    filter = filter + " and it.Emplid ==(@Emplid0)";
                    valuesObjects.Add("Emplid0", empId);
                    var parameterCollection = Operations.SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = db.TblMissions.Where(filter, parameterCollection.ToArray()).Count();
                    query = db.TblMissions.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
                }
                else
                {
                    fullCount = db.TblMissions.Count(z => z.Emplid == empId);
                    query = db.TblMissions.Where(z => z.Emplid == empId).OrderBy(sort).Skip(skip).Take(take);
                }
                return query.ToList();
            }
        }

        [OperationContract]
        public List<MissionType> MissionType()
        {
            using (var db = new TimeAttEntities())
            {
                var q = db.MissionTypes.ToList();
            
                return q;
            }
        }

        [OperationContract]
        public TblMission UpdateAndInsertTblMission(TblMission newRow, bool save, int index, int userIserial, out int outindex)
        {
            if (!EmpCanEdit(newRow.FromDate, newRow.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            var createdbyy = 0;
            if (userIserial == -1)
            {
                using (var context = new WorkFlowManagerDBEntities())
                {
                    createdbyy = context.TblAuthUsers.FirstOrDefault(w => w.Code == newRow.Emplid).Iserial;
                }
            }
            using (var db = new TimeAttEntities())
            {
                var oldMissionList = new List<ServiceReference1.TblMission>();
                outindex = index;

                var oldRow = (from e in db.TblMissions
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.Iserial = oldRow.Iserial;
                    if (oldRow.Status == 1)
                    {
                        oldMissionList.Add((ServiceReference1.TblMission)new ServiceReference1.TblMission().InjectFrom(oldRow));
                    }

                    if (newRow.CreationDate == DateTime.MinValue)
                    {
                        newRow.CreatedBy = userIserial;
                        if (userIserial == -1)
                        {
                            newRow.CreatedBy = createdbyy;
                        }

                        newRow.CreationDate = DateTime.Now;
                    }
                    Operations.SharedOperation.GenericUpdate(oldRow, newRow, db);
                }
                else
                {
                    newRow.CreatedBy = userIserial;
                    if (userIserial == -1)
                    {
                        newRow.CreatedBy = createdbyy;
                    }
                    newRow.CreationDate = DateTime.Now;
                    db.TblMissions.AddObject(newRow);
                }
                db.SaveChanges();
                if (newRow.Status == 1)
                {
                    var srv = new EmployeePayrollServiceClient();

                    var missions = db.TblMissions.Where(w => w.Emplid == newRow.Emplid && w.FromDate == newRow.FromDate).ToList();
                    var listOfMission = new List<ServiceReference1.TblMission>();
                    if (missions.Count() > 1)
                    {
                        var NewModifiedMission = new TblMission()
                        {
                            Emplid = newRow.Emplid,
                            CSPMISSIONID = newRow.CSPMISSIONID,
                            //CreatedBy=newRow.CreatedBy,
                            //ApprovedBy=newRow.ApprovedBy,
                            //ApprovedDate=newRow.ApprovedDate,
                            //CreationDate=newRow.CreationDate,
                            Description = newRow.Description,
                            FromTime = missions.Min(w => w.FromTime),
                            ToTime = missions.Max(w => w.ToTime),
                            ToDate = newRow.ToDate,
                            FromDate = newRow.ToDate
                        };
                        listOfMission.Add(new ServiceReference1.TblMission().InjectFrom(NewModifiedMission) as ServiceReference1.TblMission);
                        
                    }
                    else
                    {
                        listOfMission.Add(new ServiceReference1.TblMission().InjectFrom(missions.FirstOrDefault()) as ServiceReference1.TblMission);

                    }

                    try
                    {
                        if (listOfMission.Any() || oldMissionList.Any())
                        {
                           // srv.InsertEmpMission(listOfMission, oldMissionList);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (listOfMission.Any())
                    {

                        using (PayrollEntities payrolldb = new PayrollEntities())
                        {
                            if (oldRow != null)
                            {
                                deletePayrollMS(payrolldb, oldRow);
                            }

                            foreach (var NewEx in listOfMission)
                            {
                                var emp = payrolldb.TblEmployeePayrolls.FirstOrDefault(w => w.EmpId == NewEx.Emplid);
                                var exType = payrolldb.PayrollTblMissions.FirstOrDefault(w => w.Ename == NewEx.CSPMISSIONID);
                                if (emp != null && exType != null)
                                {
                                    var ExToInsert = new TblMissionTran()
                                    {
                                        TblEmployee = emp.Iserial,
                                        Description = NewEx.Description,
                                        FromDate = NewEx.FromDate,
                                        ToDate = NewEx.ToDate,
                                        Status = 1,
                                        TblMission = exType.Iserial,
                                        FromTime=NewEx.FromTime,
                                        ToTime=NewEx.ToTime,                                        
                                    };
                                    payrolldb.TblMissionTrans.AddObject(ExToInsert);
                                }
                            }
                            payrolldb.SaveChanges();
                        }
                    }
                }
                else if (oldRow != null && (newRow.Status != 1))
                {
                    using (PayrollEntities payrolldb = new PayrollEntities())
                    {
                        deletePayrollMS(payrolldb, oldRow);
                        payrolldb.SaveChanges();
                    }
                    //var srv = new EmployeePayrollServiceClient();
                    //srv.DeleteEmpMission(oldRow.Emplid, oldRow.FromDate, oldRow.FromTime, oldRow.ToTime);
                }
                db.SaveChanges();
                return newRow;
            }
        }


        [OperationContract]
        private int DeleteTblMission(TblMission row)
        {
            if (!EmpCanEdit(row.FromDate, row.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            using (var db = new TimeAttEntities())
            {
                var oldRow = (from e in db.TblMissions
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    if (oldRow.Status == 1)
                    {
                        using (PayrollEntities payrolldb = new PayrollEntities())
                        {
                            deletePayrollMS(payrolldb, oldRow);
                            payrolldb.SaveChanges();
                        }

                        //var srv = new EmployeePayrollServiceClient();
                        //srv.DeleteEmpMission(row.Emplid, row.FromDate, row.FromTime, row.ToTime);
                    }

                    db.DeleteObject(oldRow);
                }

                db.SaveChanges();
            }
            return row.Iserial;
        }

        #endregion Mission

        #region Vacation

        [OperationContract]
        public List<TblVacation> GetTblVacationForStores(List<string> empId, List<DateTime> dayes, bool pending)
        {
            using (var db = new TimeAttEntities())
            {
                var query = db.TblVacations.Where(x => empId.Contains(x.Emplid) && dayes.Contains(x.FromDate) && dayes.Contains(x.ToDate) && (x.Status == 0 || pending == false));
                return query.ToList();
            }
        }

        //[OperationContract]
        //public List<TblVacation> GetTblVacation(int skip, int take, string empId, string sort, string filter,
        //    Dictionary<string, object> valuesObjects, out int fullCount)
        //{
        //    using (var db = new TimeAttEntities())
        //    {
        //        IQueryable<TblVacation> query;
        //        if (filter != null)
        //        {
        //            filter = filter + " and it.Emplid ==(@Emplid0)";
        //            valuesObjects.Add("Emplid0", empId);
        //            var parameterCollection = Operations.SharedOperation.ConvertToParamters(valuesObjects);
        //            fullCount = db.TblVacations.Where(filter, parameterCollection.ToArray()).Count();
        //            query = db.TblVacations.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take);
        //        }
        //        else
        //        {
        //            fullCount = db.TblVacations.Count(z => z.Emplid == empId);
        //            query = db.TblVacations.Where(z => z.Emplid == empId).OrderBy(sort).Skip(skip).Take(take);
        //        }
        //        return query.ToList();
        //    }
        //}

        [OperationContract]
        public List<TblVacationDetail> GetEmpVacation(string emp)
        {
            #region oldRegion Vacation From Axapta
            /*
            var tblVacations = new List<TblVacation>();
            using (var db = new TimeAttEntities())
            {
                tblVacations = db.TblVacations.Where(x => x.Emplid == emp && x.Status == 0).ToList();
            }
            using (var db = new configurationEntities())
            {
                var query = db.CSPEMPLOYEEVACATIONS.Where(x => x.EMPLID == emp).ToList();

                foreach (var variable in query)
                {
                    variable.RECID = 0;
                    variable.RECID = tblVacations.Count(x => x.CSPVACATIONID.ToLower() == variable.CSPVACATIONID.ToLower());
                }
                return query.ToList();
            }
            */
            #endregion

            //var tblVacations = new List<TblVacation>();
            //using (var db = new TimeAttEntities())
            //{
            //    tblVacations = db.TblVacations.Where(x => x.Emplid == emp && x.Status == 0).ToList();

            //    var query = db.TblVacationDetails.Where(x => x.EMPLID == emp).ToList();

            //    foreach (var variable in query)
            //    {
            //        variable.RECID = 0;
            //        variable.RECID = tblVacations.Count(x => x.CSPVACATIONID.ToLower() == variable.CSPVACATIONID.ToLower());
            //    }
            //    return query.ToList();
            //}


            var tblVacations = new List<TblVacationTran>();
            using (var db = new PayrollEntities())
            {

                tblVacations = db.TblVacationTrans.Where(x => x.TblEmployee1.EmpId == emp && x.Status == 0).ToList();
                var query = db.TblEmployeeVacations.Include("TblEmployee1").Include("TblVacation1").Where(x => x.TblEmployee1.EmpId == emp).ToList();

                List<TblVacationDetail> empVacDetails = new List<TblVacationDetail>();


                foreach (var variable in query)
                {
                    TblVacationDetail d = new TblVacationDetail();
                    d.EMPLID = emp;
                    d.EMPLNAME = variable.TblEmployee1.Ename;
                    d.CSPVACATIONID = variable.TblVacation1.Code;
                    d.REMAINING = variable.Remaining;
                    d.CONSUMED = variable.Consumed;
                    d.VACATIONDAYS = variable.VacationDays;
                    d.RECID = 0;
                    d.RECID = tblVacations.Count(x => x.TblVacation == variable.TblVacation);
                    empVacDetails.Add(d);
                }
                return empVacDetails.ToList();
            }

        }

        [OperationContract]
        public List<VacationType> VacationType(string period, int year = 2015, string emp = "")
        {

            using (var db = new TimeAttEntities())
            {
                
                var periodee = db.TblExtraPeriods.FirstOrDefault(x => x.Year == year && x.Emp == emp);
                List<VacationType> VacType;
                if (periodee != null)
                {
                    VacType = db.VacationTypes.Where(x => x.CSPPERIODID == period || x.CSPPERIODID == periodee.Period).ToList();
                }
                else
                {


                    VacType = db.VacationTypes.Where(x => x.CSPPERIODID == period).ToList();
                }

                return VacType;            
            }
        }

        static public bool EmpCanEdit(DateTime fromDate, string emp)
        {
            string period = "";
            using (var context = new TimeAttEntities())
            {
                try
                {
                    period = context.EmployeesViews.FirstOrDefault(x => x.Emplid == emp).Period;

                    using (var db = new PayrollEntities())
                    {
                        //var periodline = db.CSPPERIODLINES.FirstOrDefault(x => x.FROMDATE <= fromDate && x.TODATE >= fromDate && x.CSPPERIODID == period);

                        var periodline = db.TblPeriodLines.FirstOrDefault(x => x.FromDate <= fromDate && x.ToDate >= fromDate && x.TblPeriod1.Code == period);
                        string periodlineStr = Convert.ToString(Convert.ToInt32(periodline.LineNumber));

                        var periodlock = context.TblPeriodLocks.FirstOrDefault(w => w.CspPeriodLineID == periodlineStr && w.CspPeriodID == period);

                        if (DateTime.Now.Date >= periodlock.ClosingDate)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return true;
                }
                return true;
            }
        }
       
        [OperationContract]
        public TblVacation UpdateAndInsertTblVacation(TblVacation newRow, bool save, int index, int userIserial, out int outindex)
        {
            if (!EmpCanEdit(newRow.FromDate, newRow.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            var createdbyy = 0;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (newRow.Status == 1)
                {
                    if (newRow.CSPVACATIONID.StartsWith("SICK LEAVE"))
                    {
                        var user = context.TblAuthUsers.FirstOrDefault(w => w.Iserial == userIserial);
                        if (user != null)
                        {

                            var job = user.TblJob;

                            var permission = context.TblAuthJobPermissions.Any(w => w.Tbljob == job && w.TblAuthPermission.Code == "SickLeaveApproval");

                            if (!permission)
                            {
                                throw new Exception("You Don't have permission to approve Sick Leave");
                            }
                        }
                    }
                }

                if (userIserial == -1)
                {
                    createdbyy = context.TblAuthUsers.FirstOrDefault(w => w.Code == newRow.Emplid).Iserial;
                }
            }

            using (var db = new TimeAttEntities())
            {
                outindex = index;

                var oldRow = (from e in db.TblVacations
                              where e.FromDate == newRow.FromDate
                                  && e.Emplid == newRow.Emplid
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    newRow.Iserial = oldRow.Iserial;

                    var vac = GetEmpVacation(newRow.Emplid);
                    var rem = vac.FirstOrDefault(x => x.CSPVACATIONID.ToLower() == newRow.CSPVACATIONID.ToLower());
                    if (rem != null && (rem.REMAINING - rem.RECID) > -1)
                    {
                        if (newRow.CreationDate == DateTime.MinValue)
                        {
                            newRow.CreatedBy = userIserial;
                            if (userIserial == -1)
                            {
                                newRow.CreatedBy = createdbyy;
                            }
                            newRow.CreationDate = DateTime.Now;
                        }
                        Operations.SharedOperation.GenericUpdate(oldRow, newRow, db);
                    }
                    else
                    {
                        newRow.Status = 5;
                        return newRow;
                    }
                }
                else
                {
                    var vac = GetEmpVacation(newRow.Emplid);
                    var rem = vac.FirstOrDefault(x => x.CSPVACATIONID.ToLower() == newRow.CSPVACATIONID.ToLower());
                    if (rem != null && (rem.REMAINING - rem.RECID) > 0)
                    {
                        newRow.CreatedBy = userIserial;
                        if (userIserial == -1)
                        {
                            newRow.CreatedBy = createdbyy;
                        }

                        newRow.CreationDate = DateTime.Now;
                        db.TblVacations.AddObject(newRow);
                    }
                    else
                    {
                        newRow.Status = 5;
                        return newRow;
                    }
                }

                //if (newRow.Status == 1)
                //{
                    var srv = new EmployeePayrollServiceClient();

                    var listOfVacation = new List<ServiceReference1.TblVacation>
                    {
                    new ServiceReference1.TblVacation().InjectFrom(newRow) as ServiceReference1.TblVacation
                    };

              

                    if (listOfVacation.Any())
                    {

                        using (PayrollEntities payrolldb = new PayrollEntities())
                        {
                            if (oldRow!=null)
                            {
                                deletePayrollVc(payrolldb, oldRow);
                            }
                         

                            foreach (var NewEx in listOfVacation)
                            {
                                var emp = payrolldb.TblEmployeePayrolls.FirstOrDefault(w => w.EmpId == NewEx.Emplid);
                                var exType = payrolldb.PayrollVacations.FirstOrDefault(w => w.Ename == NewEx.CSPVACATIONID);
                                if (emp != null && exType != null)
                                {
                                    var ExToInsert = new TblVacationTran()
                                    {
                                        TblEmployee = emp.Iserial,
                                        Description = NewEx.Description,
                                        FromDate = NewEx.FromDate,
                                        ToDate = NewEx.ToDate,
                                        Status = newRow.Status,
                                        TblVacation = exType.Iserial,
                                      
                                        Days=NewEx.DAYS,                                       
                                    };
                                    payrolldb.TblVacationTrans.AddObject(ExToInsert);                                    
                                }
                            }
                            payrolldb.SaveChanges();

                        }
                    //}
                }
                //else if (oldRow != null && (newRow.Status != 1))
                //{
                //    using (PayrollEntities payrolldb = new PayrollEntities())
                //    {
                //        deletePayrollVc(payrolldb, oldRow);
                //        payrolldb.SaveChanges();
                //    }
                //    //var srv = new EmployeePayrollServiceClient();
                //    //srv.DeleteEmpVacation(oldRow.Emplid, oldRow.FromDate);
                //}
                db.SaveChanges();
                CalculateEmployeeVactionBalance(newRow);
                return newRow;
            }
        }


        [OperationContract]
        private int DeleteTblVacation(TblVacation row)
        {
            if (!EmpCanEdit(row.FromDate, row.Emplid))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            using (var db = new TimeAttEntities())
            {
                var oldRow = (from e in db.TblVacations
                              where e.FromDate == row.FromDate
                                   && e.Emplid == row.Emplid
                              select e).SingleOrDefault();
                if (oldRow != null)
                {

                    using (PayrollEntities payrolldb = new PayrollEntities())
                    {
                        deletePayrollVc(payrolldb, oldRow);
                        payrolldb.SaveChanges();
                    }

                   
                    db.DeleteObject(oldRow);
                }

                db.SaveChanges();
                CalculateEmployeeVactionBalance(row);
            }
            return row.Iserial;
        }

        public void CalculateEmployeeVactionBalance(TblVacation vactionRow)
        {
            //using (TimeAttEntities db = new TimeAttEntities())
            //{

            //    var VacationsCount = db.TblVacations.Count(x => x.Emplid == vactionRow.Emplid && x.CSPVACATIONID == vactionRow.CSPVACATIONID && x.Status == 1);
            //    var VactionDetail = db.TblVacationDetails.FirstOrDefault(x => x.EMPLID == vactionRow.Emplid && x.CSPVACATIONID == vactionRow.CSPVACATIONID);

            //    VactionDetail.CONSUMED = VacationsCount;
            //    VactionDetail.REMAINING = VactionDetail.VACATIONDAYS - VacationsCount;
            //    db.SaveChanges();
            //}

            using (PayrollEntities db = new PayrollEntities())
            {

                var VacationsCount = db.TblVacationTrans.Count(x => x.TblEmployee1.EmpId == vactionRow.Emplid && x.TblVacation1.Code == vactionRow.CSPVACATIONID && x.Status == 1);
                var VactionDetail = db.TblEmployeeVacations.FirstOrDefault(x => x.TblEmployee1.EmpId == vactionRow.Emplid && x.TblVacation1.Code == vactionRow.CSPVACATIONID);

                VactionDetail.Consumed = VacationsCount;
                VactionDetail.Remaining = VactionDetail.VacationDays - VacationsCount;
                db.SaveChanges();
            }
        }

        #endregion Vacation
    }
}