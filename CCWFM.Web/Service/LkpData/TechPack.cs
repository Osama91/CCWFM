using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.IO;
using System.Collections.Generic;
using CCWFM.Web.Service.Operations;
using System;
using System.Web.UI.WebControls;
using System.Data.Objects;
using System.Drawing;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        public string TechPackImages = System.Configuration.ConfigurationManager.AppSettings["TechPackImages"];
        public string TechPackImagesWrite = System.Configuration.ConfigurationManager.AppSettings["TechPackImagesWrite"];

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TBLTechPackStatu> GetTBLTechPackStatus()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TBLTechPackStatus.ToList();
                return query;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tblTechPackPart> GetTBLTechPackParts()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tblTechPackParts.ToList();
                return query;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tblTechPackDetail> GetTBLTechPackDetails(int TBLStyle,out tblTechPackHeader tblTechPackHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tblTechPackDetails.Where(x=>x.tblTechPackHeader1.tblStyle == TBLStyle) .ToList();
                if (query.Count > 0)
                {
                    tblTechPackHeader = context.tblTechPackHeaders.FirstOrDefault(X => X.tblStyle == TBLStyle);
                }
                tblTechPackHeader = context.tblTechPackHeaders.Where(x => x.tblStyle == TBLStyle).FirstOrDefault();
                return query;
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tblTechPackHeader InsertOrUpdateTBLTechPackHeader(tblTechPackHeader newRow)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tblTechPackHeaders
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.tblTechPackHeaders.AddObject(newRow);
                }
                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public  void InsertOrUpdateTBLTechPackDetail(tblTechPackHeader _techPackHeader ,List<tblTechPackDetail> newTechPackDetailRows)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //CheackHeader
                var oldtechPackHeaderRow = (from e in context.tblTechPackHeaders
                                            where e.Iserial == _techPackHeader.Iserial
                                            select e).SingleOrDefault();

                if (oldtechPackHeaderRow != null)
                {
                    SharedOperation.GenericUpdate(oldtechPackHeaderRow, _techPackHeader, context);
                    context.SaveChanges();
                    SaveTechPackDetailRows(oldtechPackHeaderRow, newTechPackDetailRows);
                }
                else
                {
                    context.tblTechPackHeaders.AddObject(_techPackHeader);
                    context.SaveChanges();
                    SaveTechPackDetailRows(_techPackHeader, newTechPackDetailRows);
                }
            }
        }

        private void SaveTechPackDetailRows(tblTechPackHeader HeaderRow, List<tblTechPackDetail> DetailsRow)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    foreach (var newRow in DetailsRow)
                    {
                        newRow.tblTechPackHeader = HeaderRow.Iserial;
                        newRow.LastChangeDate = DateTime.Now;
                        var oldRow = (from e in context.tblTechPackDetails
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        
                        if (!string.IsNullOrEmpty(newRow.galaryLink))
                        {
                            SaveImageToPath(newRow.galaryLink, newRow.ImageThumb);
                            //newRow.galaryLink = "D:\\TechPackImages\\" + newRow.galaryLink;
                            newRow.galaryLink = TechPackImagesWrite + newRow.galaryLink;

                        }
                        else
                        {
                            newRow.galaryLink = oldRow.galaryLink;
                        }

                        //Update
                        if (oldRow != null)
                        {
                            if (string.IsNullOrEmpty(newRow.galaryLink))
                            {
                                newRow.galaryLink = oldRow.galaryLink;
                                newRow.ImageName = oldRow.ImageName;
                            }
                            oldRow.ImageThumb = null;
                            newRow.ImageThumb = null;
                            oldRow.LastChangeDate = DateTime.Now;
                            if (oldRow.Description == newRow.Description
                                && oldRow.galaryLink == newRow.galaryLink
                                && oldRow.ImageName == newRow.ImageName
                                && oldRow.tblTechPackPart == newRow.tblTechPackPart)
                            {
                                //Don't Log
                            }
                            else { SaveTBLTechPackLog(newRow); }
                            SharedOperation.GenericUpdate(oldRow, newRow, context);
                            context.SaveChanges();
                        }
                        else
                        {
                            context.tblTechPackDetails.AddObject(newRow);
                            newRow.ImageThumb = null;
                            context.SaveChanges();
                            SaveTBLTechPackLog(newRow);
                        }
                    }
                }
                catch { }
            }
        }

        private void SaveTBLTechPackLog(tblTechPackDetail _detailRow)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                tblTechPackLog _logRow = new tblTechPackLog();
                _logRow.tblTechPackDetail = _detailRow.Iserial;
                _logRow.tblTechPackPart = _detailRow.tblTechPackPart;
                _logRow.galaryLink = _detailRow.galaryLink;
                _logRow.description = _detailRow.Description;
                _logRow.tblUser  = _detailRow.tblUser;

                context.tblTechPackLogs.AddObject(_logRow);
                context.SaveChanges();
            }
        }

        public  void SaveImageToPath(string ImageName, byte[] image)
        {
            int ThumbW = 0;
            int ThumbH = 0;
            GetNewDimenision(image, out ThumbW, out ThumbH);
            byte[] ImageThum = MakeThumbnail(image, ThumbW, ThumbH);

            //Delete File IF Exist
            //if (File.Exists(@"D:\TechPackImages\"+ImageName))
            //{
            //    File.Delete(@"D:\TechPackImages\" + ImageName);
            //}
            //File.WriteAllBytes("D:\\TechPackImages\\" + "Rez-" + ImageName, ImageThum);
            //File.WriteAllBytes("D:\\TechPackImages\\"+ImageName, image);


            if (File.Exists(TechPackImages + ImageName))
            {
                File.Delete(TechPackImages + ImageName);
            }
            File.WriteAllBytes(TechPackImagesWrite + "Rez-" + ImageName, ImageThum);
            File.WriteAllBytes(TechPackImagesWrite + ImageName, image);
        }

        public byte[] MakeThumbnail(byte[] myImage, int thumbWidth, int thumbHeight)
        {

            using (MemoryStream ms = new MemoryStream())
            using (System.Drawing.Image thumbnail = System.Drawing.Image.FromStream(new MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }


        void GetNewDimenision(byte[] byteArrayIn, out int newWidth, out int newHeight)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            double scaleX;
            double scaleY;
            //int newWidth;
            //int newHeight;

            if (returnImage.Height > 2500)
            {
                scaleY = Convert.ToDouble(2500) / returnImage.Height;
                newHeight = 2500;
            }
            else
            {
                scaleY = 1;
                newHeight = returnImage.Height;
            }

            if (returnImage.Width > 2500)
            {
                scaleX = Convert.ToDouble(2500) / returnImage.Width;
                newWidth = 2500;
            }
            else
            {
                scaleX = 1;
                newWidth = returnImage.Width;
            }

            double scale = 1;
            if (scaleX < scaleY)
            {
                scale = scaleX;
                newHeight = Convert.ToInt32(returnImage.Height * scale);
            }
            else if (scaleY != 1)
            {
                scale = scaleY;
                newWidth = Convert.ToInt32(returnImage.Width * scale);
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool CanChangeDeliveryDate(int tblStyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleTNAHeaders.Where(x => x.TblStyle == tblStyle).ToList();
                if (query.Count > 0)
                {
                    if( query.FirstOrDefault().DeliveryDate != null)
                    return false;
                }
                return true;
            }  
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tblTechPackBOMComment GetTBLTechPackBOMComment(int tblstyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tblTechPackBOMComments.Where(x => x.tblStyle == tblstyle).FirstOrDefault();
                return query;
            }
        }
        
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tblTechPackBOMComment UpdateOrInsertTechPackBOMComment(tblTechPackBOMComment _tblTechPackBOMComment)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if(_tblTechPackBOMComment.Iserial == 0)
                {
                    context.tblTechPackBOMComments.AddObject(_tblTechPackBOMComment);
                    context.SaveChanges();
                    return _tblTechPackBOMComment;
                }
                else
                {
                    var oldRow = context.tblTechPackBOMComments.Where(x => x.tblStyle == _tblTechPackBOMComment.tblStyle).FirstOrDefault();
                    if (oldRow != null)
                    {
                        SharedOperation.GenericUpdate(oldRow, _tblTechPackBOMComment, context);
                        context.SaveChanges();
                    }
                    return oldRow;
                }
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteTechPackDesignDetails(int _tblTechPackDetail)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.tblTechPackDetails
                              where e.Iserial == _tblTechPackDetail
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //Delete Log
                    var oldLogRow = (from e in context.tblTechPackLogs
                                     where e.tblTechPackDetail == oldRow.Iserial
                                     select e).ToList();
                    foreach (var item in oldLogRow)
                    {
                        context.DeleteObject(item);
                    }
                    context.DeleteObject(oldRow);
                }
                context.SaveChanges();
            }
        }

        private static List<ObjectParameter> ConvertToParamters(Dictionary<string, object> valuesObjects)
        {
            return valuesObjects.Select(valuesObject => new ObjectParameter(valuesObject.Key, valuesObject.Value)).ToList();
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyle> GetCurrentTechPackBOMStyles(int skip, int take,string sort, string filter, Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblStyle> query;
                if (filter != null)
                {
                   var parameterCollection = ConvertToParamters(valuesObjects);
                   fullCount = (from _tblStyle in context.TblStyles.Where(filter, parameterCollection.ToArray())
                                 join _techpackBom in context.TechPackBOMs
                                 on _tblStyle.Iserial equals _techpackBom.TblStyle
                                 select _tblStyle).ToList().Distinct().Count();

                    query = (from _tblStyle in context.TblStyles.Where(filter, parameterCollection.ToArray())
                             join _techpackBom in context.TechPackBOMs
                              on _tblStyle.Iserial equals _techpackBom.TblStyle
                             select _tblStyle).Distinct();
                }
                else
                {
                    fullCount  = ( from _tblStyle in context.TblStyles
                                   join _techpackBom in context.TechPackBOMs
                                   on _tblStyle.Iserial equals _techpackBom.TblStyle
                                   select _tblStyle).Distinct().Count();

                    query = (from _tblStyle in context.TblStyles
                             join _techpackBom in context.TechPackBOMs
                              on _tblStyle.Iserial equals _techpackBom.TblStyle
                             select _tblStyle).Distinct();
                }
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblSalesOrderColor> GetTblSalesOrderColor(int TblStyle, int SalesOrderType)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int [] salesorder = context.TblSalesOrders.Where(x => x.TblStyle == TblStyle && x.SalesOrderType == SalesOrderType).Select(x => x.Iserial).ToArray();
                var query = context.TblSalesOrderColors.Include("TblColor1").Where(x => salesorder.Any(c => c == x.TblSalesOrder)).OrderBy(x => x.TblColor);
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private int UpdateSalesOrderColorCancelRequest(int tblSalesOrderColor, bool RequestForCancel)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var color = context.TblSalesOrderColors.Where(x => x.Iserial == tblSalesOrderColor).FirstOrDefault();
                if (RequestForCancel)
                { color.RequestForCancel = 1; }
                else
                { color.RequestForCancel = 0; }
                return  context.SaveChanges();
            }      
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private int ApproveSalesOrderColorCancelRequest(int tblSalesOrderColor)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var color = context.TblSalesOrderColors.Where(x => x.Iserial == tblSalesOrderColor).FirstOrDefault();
                color.RequestForCancel = 2;
                return context.SaveChanges();
            }
        }
        
         [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private int CheckApprovedSalesOrderColorCancelRequest(int tblSalesOrderColor)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var color = context.TblSalesOrderColors.Where(x => x.Iserial == tblSalesOrderColor && x.RequestForCancel == 2);
                if(color != null)
                {
                    context.DeleteObject(color.FirstOrDefault());
                    return context.SaveChanges();
                }

                return 0;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStyleTNARouteStatu> GetTblStyleTNARouteStatus()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.TblStyleTNARouteStatus.ToList();
            }
        }

        // [OperationContract]
        // private List<TblSalesOrder> GetSalesOrderForCopyBom(int skip, int take, int salesOrderType, string sort, string filter,
        //Dictionary<string, object> valuesObjects, out int fullCount, out List<TBLsupplier> SupplierList, out List<ItemsDto> itemsList)
        // {
        //     using (var context = new WorkFlowManagerDBEntities())
        //     {
        //         IQueryable<TblSalesOrder> query;
        //         if (filter != null)
        //         {
        //             var parameterCollection = ConvertToParamters(valuesObjects);

        //             fullCount = context.TblSalesOrders.Where(filter, parameterCollection.ToArray()).Count();
        //             query =
        //                 context.TblSalesOrders.Include("TblSalesOrderOperations.TblRouteGroup").Include("BOMs.BOM_CalcMethod").Where(filter, parameterCollection.ToArray())
        //                     .OrderBy(sort)
        //                     .Skip(skip)
        //                     .Take(take);
        //         }
        //         else
        //         {
        //             fullCount = context.TblSalesOrders.Count(x => x.SalesOrderType == salesOrderType && x.Status == 1);
        //             query =
        //                 context.TblSalesOrders.Include("TblSalesOrderOperations.TblRouteGroup").Include("BOMs.BOM_CalcMethod").OrderBy(sort)

        //                     .Skip(skip)
        //                     .Take(take);
        //         }
        //         var listOfSuppliers = query.Select(x => x.TblSupplier).Where(x => x > 0).Distinct().ToArray();

        //         using (var entity = new ccnewEntities())
        //         {
        //             entity.TBLsuppliers.MergeOption = MergeOption.NoTracking;
        //             SupplierList = listOfSuppliers.Any()
        //                 ? entity.TBLsuppliers.Where(x => listOfSuppliers.Any(l => x.Iserial == l)).ToList()
        //                 : null;
        //         }
        //         var itemsquery = new List<ItemsDto>();
        //         foreach (var bomQuery in query.ToList())
        //         {
        //             var fabricsIserial = bomQuery.BOMs.Where(x => x.BOM_FabricType != "Accessories").Select(x => x.BOM_Fabric);

        //             var accIserial = bomQuery.BOMs.Where(x => x.BOM_FabricType == "Accessories").Select(x => x.BOM_Fabric);
        //             //fabricInventList = temp.Where(x => lineNumbers.All(l => x.LINENUM != l));
        //             itemsquery.AddRange((from x in context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").Include("tbl_lkp_FabricMaterials")
        //                                  where (fabricsIserial.Any(l => x.Iserial == l))

        //                                  select new ItemsDto
        //                                  {
        //                                      Iserial = x.Iserial,
        //                                      Code = x.FabricID,
        //                                      Name = x.FabricDescription,
        //                                      ItemGroup = x.tbl_lkp_FabricMaterials.Ename,
        //                                      Unit = x.tbl_lkp_UoM.Ename
        //                                  }).Take(20).ToList());

        //             itemsquery.AddRange((from x in context.tbl_AccessoryAttributesHeader

        //                                  where (accIserial.Any(l => x.Iserial == l))
        //                                  select new ItemsDto
        //                                  {
        //                                      Iserial = x.Iserial,
        //                                      Code = x.Code,
        //                                      Name = x.Descreption,
        //                                      ItemGroup = "Accessories"
        //                                      ,
        //                                      IsSizeIncluded = x.IsSizeIncludedInHeader,

        //                                      Unit = context.tbl_lkp_UoM.FirstOrDefault(s => s.Iserial == x.UoMID).Ename,
        //                                  }).Take(20).ToList());
        //         }

        //         itemsList = itemsquery;
        //         return query.ToList();
        //     }
        // }
    }
}