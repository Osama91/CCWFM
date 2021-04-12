using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.ServiceReference1;
using Microsoft.Dynamics.BusinessConnectorNet;
using Omu.ValueInjecter;
using EmployeesView = CCWFM.Web.Model.EmployeesView;
using TblEmployeeShift = CCWFM.Web.Model.TblEmployeeShift;
using TblEmployeeShiftLookup = CCWFM.Web.Model.TblEmployeeShiftLookup;
using CCWFM.Web.Service.Operations;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        public List<TblEmployeeShiftLookup> GetTblEmployeeShiftLookup()
        {
            using (var db = new TimeAttEntities())
            {
                db.TblEmployeeShiftLookups.MergeOption = MergeOption.NoTracking;

                var u = db.TblEmployeeShiftLookups.ToList();
                return u;
            }
        }

        [OperationContract]
        public List<TblEmployeeShift> GetTblEmployeeShiftByStore(int week, ObservableCollection<string> empList, bool pending)
        {
            using (var db = new TimeAttEntities())
            {
                db.TblEmployeeShifts.MergeOption = MergeOption.NoTracking;
                var u = db.TblEmployeeShifts.Where(z => z.Week == week && empList.Contains(z.EmpId) && (z.Status == 0 || pending == false)).ToList();
                return u;
            }
        }

        [OperationContract]
        public List<string> GetEmpPosition(string code)
        {
            using (var db = new TimeAttEntities())
            {
                if (string.IsNullOrWhiteSpace(code))
                {

                    return db.EmployeesViews.Select(wde => wde.Position).Distinct().ToList();
                }
                var empList = (from e in db.EmployeesViews
                               join eb in db.TblEmployeeBehalves
                                on e.Emplid equals eb.Emplid
                               orderby e.Emplid
                               where eb.AttOperatorId == code
                               || eb.Emplid == code
                               || eb.ManagerId == code
                               select e.Position).Distinct();
                return empList.ToList();
            }
        }

        [OperationContract]
        public List<string> GetEmpTransportationLine(string code)
        {
            using (var db = new TimeAttEntities())
            {
                var empList = (from e in db.EmployeesViews
                               join eb in db.TblEmployeeBehalves
                                on e.Emplid equals eb.Emplid
                               orderby e.Emplid
                               where eb.AttOperatorId == code
                               || eb.Emplid == code
                               || eb.ManagerId == code
                               select e.TransportationLine).Distinct();
                var EMPLOYESS = empList.ToList();
                return empList.ToList();
            }
        }

        [OperationContract]
        public List<EmployeesView> GetEmpByAttOperator(string position, string transportation, string code)
        {
            bool IsMangerOfMangers = false;
            using (var wf = new WorkFlowManagerDBEntities())
            {
                //Added By Hashem on 05-10-2020 To Get Employees Related to the second Level Manger 
                var employeeJob = wf.TblAuthUsers.FirstOrDefault(x => x.Code == code);
                var perm = wf.TblAuthJobPermissions.FirstOrDefault(x => x.Tbljob == employeeJob.TblJob && x.TblAuthPermission.Code == "AttendanceDirector");
                if (perm != null)
                {
                    IsMangerOfMangers = true;
                }

            }
            using (var db = new TimeAttEntities())
            {
                //Added By Hashem on 05-10-2020 To Get Employees Related to the second Level Manger 

                if (IsMangerOfMangers)
                {
                    List<EmployeesView> empList = new List<EmployeesView>();

                    var firstLevelMangers = (from e in db.EmployeesViews
                                             join eb in db.TblEmployeeBehalves
                                              on e.Emplid equals eb.Emplid
                                             orderby e.Emplid
                                             where (eb.AttOperatorId == code
                                             || eb.Emplid == code
                                             || eb.ManagerId == code)
                                             && (e.Position == position || position == null)
                                             && (e.TransportationLine == transportation || transportation == null)
                                             select e);

                    foreach (var mangerCode in firstLevelMangers)
                    {
                        var mangerEmployees = (from e in db.EmployeesViews
                                               join eb in db.TblEmployeeBehalves
                                                on e.Emplid equals eb.Emplid
                                               orderby e.Emplid
                                               where (eb.AttOperatorId == mangerCode.Emplid
                                               || eb.Emplid == mangerCode.Emplid
                                               || eb.ManagerId == mangerCode.Emplid)
                                               && (e.Position == position || position == null)
                                               && (e.TransportationLine == transportation || transportation == null)
                                               select e);

                        empList.AddRange(mangerEmployees);
                    }

                    var ee = empList.GroupBy(cc => cc.Emplid)
                   .Select(grp => grp.First())
                   .ToList();
                    return ee;
                }
                else
                {
                    var empList = (from e in db.EmployeesViews
                                   join eb in db.TblEmployeeBehalves
                                    on e.Emplid equals eb.Emplid
                                   orderby e.Emplid
                                   where (eb.AttOperatorId == code
                                   || eb.Emplid == code
                                   || eb.ManagerId == code)
                                   && (e.Position == position || position == null)
                                   && (e.TransportationLine == transportation || transportation == null)

                                   select e);
                    return empList.ToList();
                }
            }
        }

        [OperationContract]
        private void TerminateEmp(string emp)
        {
            if (SharedOperation.UseAx())
            {
                var Axapta = new Axapta();
                var credential = new NetworkCredential("bcproxy", "around1");
                Axapta.LogonAs("Osama.Gamal", "ccasual.loc", credential, "Ccm", "", "", "");
                var AxaptaRecord = Axapta.CreateAxaptaRecord("EMPLTABLE");
                AxaptaRecord.Clear();
                AxaptaRecord.InitValue();

                AxaptaRecord.ExecuteStmt("select forupdate * from %1 where %1.Emplid == '" + emp + "");
                if (AxaptaRecord.Found)
                {
                    AxaptaRecord.set_Field("CSPPeriodID", "2012");
                    AxaptaRecord.Update();
                    Axapta.TTSCommit();
                }
            }
        }

        [OperationContract]
        private int DeleteTblEmployeeShift(TblEmployeeShift row)
        {
            if (!Att.AttService.EmpCanEdit(row.TransDate, row.EmpId))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }

            using (var context = new TimeAttEntities())
            {
                var oldRow = (from e in context.TblEmployeeShifts.Include("TblEmployeeShiftLookup1")
                              where
                               e.EmpId == row.EmpId
                             && e.TransDate == row.TransDate

                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //if (oldRow.Status == 1)
                    //{
                    //    var srv = new EmployeePayrollServiceClient();
                    //    srv.DeleteEmpCalendarDeviation(row.EmpId, row.TransDate, oldRow.TblEmployeeShiftLookup1.CalendarId);
                    //}

                    context.DeleteObject(oldRow);
                }

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        public List<TblEmployeeShift> UpdateAndInsertTblEmployeeShiftList(List<TblEmployeeShift> newList)
        {


            using (var db = new TimeAttEntities())
            {
                foreach (var row in newList)
                {
                    var oldRow = (from e in db.TblEmployeeShifts
                                  where e.EmpId == row.EmpId
                 && e.TransDate == row.TransDate
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        row.Iserial = oldRow.Iserial;
                        GenericUpdate(oldRow, row, db);
                    }
                    else
                    {
                        db.TblEmployeeShifts.AddObject(row);
                    }

                    db.SaveChanges();
                }
                //var srv = new EmployeePayrollServiceClient();

                //var listOfEmployeeShifts = new List<ServiceReference1.TblEmployeeShift>();

                //foreach (var row in newList.Where(x => x.Status == 1))
                //{
                //    listOfEmployeeShifts.Add(new ServiceReference1.TblEmployeeShift().InjectFrom(row) as ServiceReference1.TblEmployeeShift);
                //}
                //try
                //{
                //    if (listOfEmployeeShifts.Count != 0)
                //    {
                //        srv.InsertEmpCalendarDeviation(listOfEmployeeShifts);
                //    }
                //}
                //catch (Exception)
                //{
                //    throw;
                //}

                return newList;
            }
        }

        [OperationContract]
        public TblEmployeeShift UpdateAndInsertTblEmployeeShift(TblEmployeeShift row, int index, int userIserial, out int outindex)
        {
            if (!Att.AttService.EmpCanEdit(row.TransDate, row.EmpId))
            {
                throw new Exception("Cannot edit a transaction because this period is already closed");
            }
            outindex = index;
            IEnumerable<string> changedList = null;
            bool save = row.Iserial == 0;

            using (var db = new TimeAttEntities())
            {
                var oldRow = (from e in db.TblEmployeeShifts
                              where e.EmpId == row.EmpId
                             && e.TransDate == row.TransDate
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    if (row.CreationDate == DateTime.MinValue)
                    {
                        row.CreatedBy = userIserial;

                        row.CreationDate = DateTime.Now;
                    }
                    row.CreatedBy = userIserial;
                    row.Iserial = oldRow.Iserial;
                    changedList = GenericUpdate(oldRow, row, db);
                }
                else
                {
                    row.CreatedBy = userIserial;
                    row.CreationDate = DateTime.Now;
                    db.TblEmployeeShifts.AddObject(row);
                }

                if (row.Status == 1)
                {
                    //    var srv = new EmployeePayrollServiceClient();

                    //    var listOfEmployeeShifts = new List<ServiceReference1.TblEmployeeShift>
                    //{
                    //    new ServiceReference1.TblEmployeeShift().InjectFrom(row) as ServiceReference1.TblEmployeeShift
                    //};

                    //    try
                    //    {
                    //        if (listOfEmployeeShifts.Count != 0)
                    //        {
                    //            srv.InsertEmpCalendarDeviation(listOfEmployeeShifts);
                    //        }
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                }
                db.SaveChanges();
            }
            return row;
        }

        [OperationContract]
        public List<EmployeesView> GetEmpTablebyStoreAndCompany(string company, string storeCode)
        {
            using (var db = new TimeAttEntities())
            {
                db.EmployeesViews.MergeOption = MergeOption.NoTracking;
                var emp =
                    db.EmployeesViews.Where(x => x.Company == company && x.Store == storeCode).ToList();
                return emp;
            }
        }
    }
}