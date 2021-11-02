using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using System.Data.Objects;
using System.Net;
using System.IO;
using System.Text;

namespace CCWFM.Web.Service.Production
{
    public partial class ProductionService
    {
        public string StyleTNADetailFiles = System.Configuration.ConfigurationManager.AppSettings["StyleSpecDetailFiles"];

        [OperationContract]
        private List<TblStyleTNA> GetTblStyleLookup(int tblstyle = 0)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (tblstyle == 0)
                {
                    var Data = context.TblStyleTNAs.ToList();
                    return Data;
                }
                else
                {
                    var Data = (from s in context.TblStyleTNAs
                            join sa in context.TblStyleTNARoutes on s.Iserial equals sa.TblStyleTNA
                            where sa.TblStyle == tblstyle
                            select s).ToList();
                    return Data;
                }
              
            }
        }        

        [OperationContract]
        private List<StyleTnaCount_Result> StyleTnaCount(int user)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var Data = context.StyleTnaCount(user).ToList();
                return Data;                     
            }
    
        }
        [OperationContract]
        private int DeleteTblStyleTNAHeader(TblStyleTNAHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStyleTNAHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStyleTNAHeader> GetTblStyleTNAHeader( int tblStyle,int season, out List<TBLsupplier> SupplierList)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblStyleTNAHeader> query;
              
                
                    query = context.TblStyleTNAHeaders.Include("TblStyleTNADetails").Include("TblStyleTNAColorDetails.TblColor1")
                        .Where(x => x.TblStyle == tblStyle && (x.TblLkpSeason== season||season==-1));

                var listOfSuppliers = query.Select(x => x.TblSupplier).Where(x => x > 0).Distinct().ToArray();
                using (var entity = new ccnewEntities())
                {
                    entity.TBLsuppliers.MergeOption = MergeOption.NoTracking;
                    
                    SupplierList = listOfSuppliers.Any() ? entity.TBLsuppliers.Where(x => listOfSuppliers.Any(l => x.Iserial == l)).ToList() : null;                 
                }

                return query.ToList();
            }
        }

        [OperationContract]        
        private DateTime RequestStyleTna(int tblStyle,bool RequestTna)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldrow=context.TblStyles.FirstOrDefault(w => w.Iserial == tblStyle);

                if (RequestTna|| oldrow.TNACreationDate==null)
                {
                    oldrow.TNACreationDate = DateTime.Now;
                }
                oldrow.RequestTna = RequestTna;
                context.SaveChanges();
                return oldrow.TNACreationDate.Value;
                //return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private List<TblStyleTNADetail> GetTblStyleTNADetail( int TblStyleTNAHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {

                IQueryable<TblStyleTNADetail> query;                           
                    query = context.TblStyleTNADetails
                    .Where(x => x.TblStyleTNAHeader == TblStyleTNAHeader);                
                return query.ToList();
            }
        }

        [OperationContract]
        private TblStyleTNAHeader UpdateOrInsertTblStyleTNAHeader(TblStyleTNAHeader newRow, bool save, int index, int user, out int outindex)
        {
            /*
            outindex = index;
            try 
            {
                //Check  Delivery Date Limit with Contract DeliveryDate
                bool res = CheckContractDeliveryDate(newRow);
                if(!res)
                {
                    throw new Exception("Invalid Warehouse Delivery According to Delivery Date in Contract");
                }



                using (var context = new WorkFlowManagerDBEntities())
                {
                    if (save)
                    {
                        var style = context.TblStyles.FirstOrDefault(t => t.Iserial == newRow.TblStyle);
                        //if (style.RequestTna!=true)
                        //{
                        //    throw new Exception("Cannot add TNA to this style because it doesn't require TNA");
                        //}
                        style.RequestTna = false;
                        newRow.CreatedBy = user;
                        newRow.CreationDate = DateTime.Now;
                        foreach (var item in newRow.TblStyleTNADetails)
                        {
                            item.CreatedBy = user;
                            item.CreationDate = DateTime.Now;

                            //
                            foreach (var attchment in item.TblStyleTNADetailAttachments.ToList())
                            {
                                //TblStyleTNADetailAttachment newAttachment = new TblStyleTNADetailAttachment();
                                //newAttachment.TblStyleTNADetail = item.Iserial;
                                //newAttachment.galaryLink = StyleTNADetailFiles + newAttachment.galaryLink;
                                //newAttachment.FileName = attchment.FileName;
                                
                                SaveFileToPath(StyleTNADetailFiles, attchment.ImageThumb, attchment.FileName);
                                attchment.ImageThumb = null;
                            }
                        }
                        context.TblStyleTNAHeaders.AddObject(newRow);
                    }
                    else
                    {
                        bool changePrice = false;
                        if (context.TblStyleTNAColorDetails.Any(wde=>wde.TblStyleTNAHeader==newRow.Iserial))
                        {
                         var CostList=   context.TblStyleTNAColorDetails.Where(wde => wde.TblStyleTNAHeader == newRow.Iserial).Select(w => w.LocalCost).ToList();

                            var minCost = CostList.Min(w=>w.Value);
                            var maxCost = CostList.Max(w => w.Value);

                            if (minCost== maxCost)
                            {
                                changePrice = true;
                            }
                        }
                        var oldRow = (from e in context.TblStyleTNAHeaders
                                      where e.Iserial == newRow.Iserial
                                      select e).SingleOrDefault();
                        if (oldRow != null)
                        {
                            newRow.LastUpdatedDate = DateTime.Now;
                            newRow.LastUpdatedBy = user;
                            SharedOperation.GenericUpdate(oldRow, newRow, context);
                            foreach (var row in newRow.TblStyleTNADetails.ToList())
                            {
                                var oldColorRow = (from e in context.TblStyleTNADetails
                                                   where e.Iserial == row.Iserial && e.TblStyleTNAHeader == row.TblStyleTNAHeader
                                                   select e).SingleOrDefault();
                                if (oldColorRow != null)
                                {
                                    row.CreatedBy = oldColorRow.CreatedBy;
                                    row.CreationDate = oldColorRow.CreationDate;
                                    row.LastUpdatedDate = DateTime.Now;
                                    row.LastUpdatedBy = user;
                                    row.TblStyleTNAHeader = newRow.Iserial;
                                    row.TblStyleTNAHeader1 = null;
                                    
                                    SharedOperation.GenericUpdate(oldColorRow, row, context);

                                    //Update Or Insert DetailAttachment
                                        if (row != null)
                                        {
                                            foreach (var attchment in row.TblStyleTNADetailAttachments.ToList())
                                            {
                                                TblStyleTNADetailAttachment newAttachment = new TblStyleTNADetailAttachment();
                                                newAttachment.TblStyleTNADetail = row.Iserial;
                                                newAttachment.galaryLink = StyleTNADetailFiles + newAttachment.galaryLink;
                                                newAttachment.FileName = attchment.FileName;

                                                SaveFileToPath(StyleTNADetailFiles, attchment.ImageThumb, attchment.FileName);
                                              
                                                context.TblStyleTNADetailAttachments.AddObject(newAttachment);
                                            }
                                            context.SaveChanges();
                                        }
                                }
                                else
                                {
                                    row.CreatedBy = user;
                                    row.CreationDate = DateTime.Now;
                                    row.TblStyleTNAHeader = newRow.Iserial;
                                    oldRow.TblStyleTNADetails.Add(row);
                                    // newRow.TblStyleTNADetails.Add(row);
                                    //context.TblStyleTNADetails.AddObject(row);
                                    context.SaveChanges();

                                    foreach (var attchment in row.TblStyleTNADetailAttachments.ToList())
                                    {
                                        TblStyleTNADetailAttachment newAttachment = new TblStyleTNADetailAttachment();
                                        newAttachment.TblStyleTNADetail = row.Iserial;
                                        newAttachment.galaryLink = StyleTNADetailFiles + newAttachment.galaryLink;
                                        newAttachment.FileName = attchment.FileName;
                                        SaveFileToPath(StyleTNADetailFiles, attchment.ImageThumb, attchment.FileName);
                                        context.TblStyleTNADetailAttachments.AddObject(newAttachment);
                                        context.SaveChanges();
                                    }
                                }
                            }

                            foreach (var row in newRow.TblStyleTNAColorDetails.ToList())
                            {
                                var oldColorRow = (from e in context.TblStyleTNAColorDetails
                                                   where e.Iserial == row.Iserial && e.TblStyleTNAHeader == row.TblStyleTNAHeader
                                                   select e).SingleOrDefault();
                                if (oldColorRow != null)
                                {                                
                                    row.TblStyleTNAHeader = newRow.Iserial;
                                   // row.TblStyleTNAHeader1 = null;
                                    if (changePrice)
                                    {
                                        row.LocalCost = newRow.LocalCost;
                                        row.ExchangeRate = newRow.ExchangeRate;
                                        row.FabricCost = newRow.FabricCost;
                                        row.AccCost = newRow.AccCost;
                                        row.OperationCost = newRow.OperationCost;
                                    }
                                    SharedOperation.GenericUpdate(oldColorRow, row, context);
                                }
                                else
                                {                                    
                                    row.TblStyleTNAHeader = newRow.Iserial;
                                    row.TblStyleTNAHeader1 = null;
                                    context.TblStyleTNAColorDetails.AddObject(row);
                                }
                            }

                        }
                    }
                 
                    context.SaveChanges();
                  
                    return newRow = context.TblStyleTNAHeaders.Include("TblStyleTNADetails").Include("TblStyleTNAColorDetails.TblColor1")
                        .FirstOrDefault(x => x.Iserial == newRow.Iserial) ;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            */
            outindex = index;
            return null; 
        }


        [OperationContract]
        private bool CheckContractDeliveryDate(TblStyleTNAHeader row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int DaysCout = 0;
                var query = row.TblStyleTNADetails.FirstOrDefault(x => x.TblStyleTNA == 9);
                var LimitTime = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "ApproveTNADeliveryDatePeriodLimit").sSetupValue;

                if (LimitTime != null)
                    int.TryParse(LimitTime,out DaysCout);

                if (query != null)
                {
                    var StyleContractDetails = context.TblContractDetails.Where(x => x.TblSalesOrderColor1.TblSalesOrder1.TblStyle == row.TblStyle);
                    if(StyleContractDetails != null)
                    {
                        if(StyleContractDetails.Count() > 0)
                        {
                            var MaxAppliedDeliveryDate = StyleContractDetails.Min(x => x.DeliveryDate).AddDays(-1 * DaysCout);
                            if (DateTime.Now < MaxAppliedDeliveryDate)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        
                    }               
                }
                return true;
            }

        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        private void DownloadAttachFile(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //WebClient wc = new WebClient();
                //wc.DownloadStringCompleted += (s, e3) =>
                //{
                //    if (e3.Error == null)
                //    {
                //        try
                //        {
                //            byte[] fileBytes = Encoding.UTF8.GetBytes(e3.Result);
                //            using (Stream fs = (Stream)mySaveFileDialog.OpenFile())
                //            {
                //                fs.Write(fileBytes, 0, fileBytes.Length);
                //                fs.Close();

                //                MessageBox.Show("File successfully saved!");
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            MessageBox.Show("Error getting result: " + ex.Message);
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show(e3.Error.Message);
                //    };

                //    wc.DownloadStringAsync("myURI", UriKind.RelativeOrAbsolute));
                //}
            }
        }


        private void SaveFileToPath(string galaryLint, byte[] FileData, string FileName)
        {
            try
            {
                File.WriteAllBytes(galaryLint + FileName, FileData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}