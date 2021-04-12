//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel;
//using CCWFM.Web.Model;

//namespace CCWFM.Web.Service
//{
//    public partial class CRUD_ManagerService
//    {
//        [OperationContract]
//        public List<tbl_WF_StyleMeasurementHeader> GetMeasurementHeader(string _StyleHeader)
//        {
//            using (WorkFlowManagerDBEntities Entities = new WorkFlowManagerDBEntities())
//            {
//                List<tbl_WF_StyleMeasurementHeader> MeasurementHeader = (from m in Entities.tbl_WF_StyleMeasurementHeader
//                                                                         where m.StyleHeader == _StyleHeader
//                                                                         select m).ToList();
//                return MeasurementHeader;
//            }
//        }

//        [OperationContract]
//        public List<tbl_WF_StyleMeasurement> GetMeasurement(int _HeaderIserial)
//        {
//            using (WorkFlowManagerDBEntities Entities = new WorkFlowManagerDBEntities())
//            {
//                List<tbl_WF_StyleMeasurement> MeasurementQuery = (from m in Entities.tbl_WF_StyleMeasurement
//                                                                  where m.MeasurementHeaderIserial == _HeaderIserial
//                                                                  select m).ToList();

//                return MeasurementQuery;
//            }
//        }

//        [OperationContract]
//        public List<tbl_WF_StyleMeasurementDetails> GetMeasurementDetails(int MeasurementIserial)
//        {
//            using (WorkFlowManagerDBEntities Entities = new WorkFlowManagerDBEntities())
//            {
//                List<tbl_WF_StyleMeasurementDetails> MeasurementDetails = (from m in Entities.tbl_WF_StyleMeasurementDetails
//                                                                           where m.MeasurementIserial == MeasurementIserial
//                                                                           select m).ToList();
//                return MeasurementDetails;
//            }
//        }

//        [OperationContract]
//        public List<tbl_PartsOfMeasurement> Pom()
//        {
//            using (WorkFlowManagerDBEntities Entities = new WorkFlowManagerDBEntities())
//            {
//                List<tbl_PartsOfMeasurement> Datapoms = Entities.tbl_PartsOfMeasurement.ToList();

//                return Datapoms;
//            }
//        }

//        [OperationContract]
//        public int SaveMeasurmentHeader(string _StyleHeader, byte[] Image)
//        {
//            using (WorkFlowManagerDBEntities entities = new WorkFlowManagerDBEntities())
//            {
//                tbl_WF_StyleMeasurementHeader tbl_WF_StyleMeasurementHeader = new tbl_WF_StyleMeasurementHeader();

//                int MeasurmentHeaderList = (from s in entities.tbl_WF_StyleMeasurementHeader
//                                            where s.StyleHeader == _StyleHeader
//                                            select s.Iserial).SingleOrDefault();

//                int iseral = 0;
//                if (MeasurmentHeaderList == 0)
//                {
//                    tbl_WF_StyleMeasurementHeader.StyleHeader = _StyleHeader;
//                    tbl_WF_StyleMeasurementHeader.Image = Image;
//                    entities.AddTotbl_WF_StyleMeasurementHeader(tbl_WF_StyleMeasurementHeader);
//                    entities.SaveChanges();
//                    iseral = tbl_WF_StyleMeasurementHeader.Iserial;
//                }
//                else
//                {
//                    iseral = MeasurmentHeaderList;
//                }
//                return iseral;
//            }
//        }

//        [OperationContract]
//        public void SaveMeasurmentList(int MeasurementHeaderIserial, string Pom, string MeasurementOrder, double? Tolerance, string SizeCode, double? SizeValue)
//        {
//            using (WorkFlowManagerDBEntities entities = new WorkFlowManagerDBEntities())
//            {
//                var MeasurmentList = (from s in entities.tbl_WF_StyleMeasurement
//                                      where s.Pom == Pom && s.MeasurementOrder == MeasurementOrder && s.MeasurementHeaderIserial == MeasurementHeaderIserial
//                                      select s).SingleOrDefault();

//                tbl_WF_StyleMeasurementDetails MeasurmentDetailsTable = new tbl_WF_StyleMeasurementDetails();
//                tbl_WF_StyleMeasurement MeasurmentTable = new tbl_WF_StyleMeasurement();

//                if (MeasurmentList == null)
//                {
//                    MeasurmentTable.MeasurementHeaderIserial = MeasurementHeaderIserial;
//                    MeasurmentTable.Pom = Pom;
//                    MeasurmentTable.MeasurementOrder = MeasurementOrder;
//                    MeasurmentTable.Tolerance = Tolerance;
//                    entities.AddTotbl_WF_StyleMeasurement(MeasurmentTable);
//                    entities.SaveChanges();
//                }
//                else
//                {
//                    MeasurmentTable = MeasurmentList;
//                    MeasurmentTable.Tolerance = Tolerance;
//                }

//                var MeasurmentDetailsList = (from d in entities.tbl_WF_StyleMeasurementDetails
//                                             where d.SizeCode == SizeCode
//                                             && d.MeasurementIserial == MeasurmentTable.Iserial
//                                             select d);

//                if (MeasurmentDetailsList.Count() == 0)
//                {
//                    MeasurmentDetailsTable.MeasurementIserial = MeasurmentTable.Iserial;

//                    MeasurmentDetailsTable.SizeCode = SizeCode;
//                    MeasurmentDetailsTable.SizeValue = SizeValue;
//                    entities.AddTotbl_WF_StyleMeasurementDetails(MeasurmentDetailsTable);
//                    entities.SaveChanges();
//                }
//                else
//                {
//                    MeasurmentDetailsTable.SizeCode = SizeCode;
//                    MeasurmentDetailsTable.SizeValue = SizeValue;
//                }
//                entities.SaveChanges();
//            }
//        }
//    }
//}