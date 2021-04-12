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
using System.Net;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        public string StyleSpecDetailFiles = System.Configuration.ConfigurationManager.AppSettings["StyleSpecDetailFiles"];

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleSpecDetail> GetStyleSpecDetails(int tblStyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleSpecDetails.Include("tblStyleSpecDetailAttachments").Where(x => x.TblStyle == tblStyle).ToList();
                return query;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleSpecType> GetTblStyleSpecTypes()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleSpecTypes.ToList();
                return query;
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleSpecDetail> UpdateOrInsertStyleSpecDetails(List<TblStyleSpecDetail> styleSpecDetails)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var row in styleSpecDetails)
                {
                    var oldRow = (from e in context.TblStyleSpecDetails
                                  where e.Iserial == row.Iserial && e.TblStyle == row.TblStyle
                                  select e).SingleOrDefault();
                    if (oldRow != null)
                    {
                        oldRow.Description = row.Description;
                        oldRow.TblStyleSpecTypes = row.TblStyleSpecTypes;
                        foreach (var attchment in row.tblStyleSpecDetailAttachments.ToList())
                        {
                            tblStyleSpecDetailAttachment newAttachment = new tblStyleSpecDetailAttachment();
                            newAttachment.tblStyleSpecDetails = oldRow.Iserial;
                            newAttachment.galaryLint = StyleSpecDetailFiles + newAttachment.galaryLint;
                            newAttachment.FileName = attchment.FileName;

                            SaveFileToPath(StyleSpecDetailFiles, attchment.ImageThumb, attchment.FileName);

                            context.tblStyleSpecDetailAttachments.AddObject(newAttachment);
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        TblStyleSpecDetail newRow = new TblStyleSpecDetail();
                        newRow.TblStyle = row.TblStyle;
                        newRow.Description = row.Description;
                        newRow.TblStyleSpecTypes = row.TblStyleSpecTypes;
                        context.TblStyleSpecDetails.AddObject(newRow);
                        context.SaveChanges();
                        foreach (var attchment in row.tblStyleSpecDetailAttachments.ToList())
                        {
                            tblStyleSpecDetailAttachment newAttachment = new tblStyleSpecDetailAttachment();
                            newAttachment.tblStyleSpecDetails = newRow.Iserial;
                            newAttachment.galaryLint = StyleSpecDetailFiles + newAttachment.galaryLint;
                            newAttachment.FileName = attchment.FileName;
                            context.tblStyleSpecDetailAttachments.AddObject(newAttachment);

                            SaveFileToPath(StyleSpecDetailFiles, attchment.ImageThumb, attchment.FileName);

                            context.SaveChanges();
                        }
                    }
                }

                int tblStyle = styleSpecDetails.ToList().FirstOrDefault().TblStyle;
                var query = context.TblStyleSpecDetails.Include("tblStyleSpecDetailAttachments").Where(x => x.TblStyle == tblStyle);
                return query.ToList();
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


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteStyleSpecDetailRow(int TblStyleSpecDetail, int tblStyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var deletedSpecDetail = context.TblStyleSpecDetails.Where(x => x.TblStyle == tblStyle && x.Iserial == TblStyleSpecDetail);
                if (deletedSpecDetail != null)
                {
                    //Delete Attachment
                    var deletedAttacment = context.tblStyleSpecDetailAttachments.Where(x => x.tblStyleSpecDetails == TblStyleSpecDetail);
                    foreach (var item in deletedAttacment.ToList())
                    {
                        context.tblStyleSpecDetailAttachments.DeleteObject(item);
                        context.SaveChanges();
                    }


                    //Delete Detail
                    TblStyleSpecDetail q = deletedSpecDetail.FirstOrDefault();
                    context.TblStyleSpecDetails.DeleteObject(q);
                    context.SaveChanges();
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tblStyleSpecDetailAttachment> GetTblStyleSpecDetailAttachment(int tblstyle, int tblStyleSpecDetails)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.tblStyleSpecDetailAttachments.Where(x => x.TblStyleSpecDetail.TblStyle == tblstyle && x.tblStyleSpecDetails == tblStyleSpecDetails);
                return query.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteTblStyleSpecDetailAttachmentRow(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var row = context.tblStyleSpecDetailAttachments.Where(x => x.Iserial == Iserial);
                if (row != null)
                {
                    foreach (var item in row.ToList())
                    {
                        context.tblStyleSpecDetailAttachments.DeleteObject(item);
                        context.SaveChanges();
                    }
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteTblStyleTNADetailAttachmentRow(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var row = context.TblStyleTNADetailAttachments.Where(x => x.Iserial == Iserial);
                if (row != null)
                {
                    foreach (var item in row.ToList())
                    {
                        context.TblStyleTNADetailAttachments.DeleteObject(item);
                        context.SaveChanges();
                    }
                }
            }
        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                return System.Drawing.Image.FromStream(ms, true);//Exception occurs here
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UploadStyleImage(int tblStyle, string galaryLink, byte[] styleImage)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var styleImageQuery = context.TblStyleImages.FirstOrDefault(x => x.TblStyle == tblStyle && x.DefaultImage == true);
                if (styleImageQuery == null)
                {
                    try
                    {

                        //Resize Image To Get Thumbnail
                        System.Drawing.Image img = byteArrayToImage(styleImage);

                        //Resize Image To Get Thumbnail
                        System.Drawing.Image newImage = new Bitmap(100, 100);
                        using (Graphics g = Graphics.FromImage(newImage))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(img, 0, 0, 100, 100);
                        }

                        byte[] results;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                            EncoderParameters jpegParms = new EncoderParameters(1);
                            jpegParms.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                            newImage.Save(ms, codec, jpegParms);
                            results = ms.ToArray();
                        }

                        TblStyleImage tblStyleImage = new TblStyleImage();
                        tblStyleImage.TblStyle = tblStyle;
                        tblStyleImage.IsPrintable = false;
                        tblStyleImage.IsActive = true;
                        tblStyleImage.ImagePath = @"Uploads/Imagessss/StylePicture/" + galaryLink;
                        tblStyleImage.CreationDate = System.DateTime.Now;
                        tblStyleImage.ImagePathThumbnail = results;
                        tblStyleImage.DefaultImage = true;
                        tblStyleImage.OrginalFileName = galaryLink;
                        context.TblStyleImages.AddObject(tblStyleImage);
                        int Saved = context.SaveChanges();

                        if (Saved > 0)
                        {
                            File.WriteAllBytes(@"D:\StylePicture\" + galaryLink, styleImage);
                        }

                    }
                    catch { }
                }
            }
        }


        #region StyleTNADetails

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UploadStyleTNADetailFiles(int tblStyleTNADetail, string galaryLink, byte[] styleImage)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                //var styleImageQuery = context.TblStyleImages.FirstOrDefault(x => x.TblStyle == tblStyle && x.DefaultImage == true);
                //if (styleImageQuery == null)
                //{
                //    try
                //    {

                //        //Resize Image To Get Thumbnail
                //        System.Drawing.Image img = byteArrayToImage(styleImage);

                //        //Resize Image To Get Thumbnail
                //        System.Drawing.Image newImage = new Bitmap(100, 100);
                //        using (Graphics g = Graphics.FromImage(newImage))
                //        {
                //            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //            g.DrawImage(img, 0, 0, 100, 100);
                //        }

                //        byte[] results;
                //        using (MemoryStream ms = new MemoryStream())
                //        {
                //            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                //            EncoderParameters jpegParms = new EncoderParameters(1);
                //            jpegParms.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                //            newImage.Save(ms, codec, jpegParms);
                //            results = ms.ToArray();
                //        }

                //        TblStyleImage tblStyleImage = new TblStyleImage();
                //        tblStyleImage.TblStyle = tblStyle;
                //        tblStyleImage.IsPrintable = false;
                //        tblStyleImage.IsActive = true;
                //        tblStyleImage.ImagePath = @"Uploads/Imagessss/StylePicture/" + galaryLink;
                //        tblStyleImage.CreationDate = System.DateTime.Now;
                //        tblStyleImage.ImagePathThumbnail = results;
                //        tblStyleImage.DefaultImage = true;
                //        tblStyleImage.OrginalFileName = galaryLink;
                //        context.TblStyleImages.AddObject(tblStyleImage);
                //        int Saved = context.SaveChanges();

                //        if (Saved > 0)
                //        {
                //            File.WriteAllBytes(@"D:\StylePicture\" + galaryLink, styleImage);
                //        }

                //    }
                //    catch { }
                //}
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyleTNADetailAttachment> GetTblStyleTNADetailAttachment(int tblStyleTNADetail)
        {

            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleTNADetailAttachments.Where(x => x.TblStyleTNADetail == tblStyleTNADetail);
                return query.ToList();
            }
        }


        #endregion
    }
}