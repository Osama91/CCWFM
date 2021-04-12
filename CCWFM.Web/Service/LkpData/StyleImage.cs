using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CCWFM.Web.Service.LkpData
{
    public partial class LkpData
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool InsertStyleImageFromFolder(int tblStyle, string styleCode)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var styleImageQuery = context.TblStyleImages.FirstOrDefault(x => x.TblStyle == tblStyle && x.DefaultImage == true);
                if (styleImageQuery == null)
                {
                    try
                    {
                        // string imagePath = string.Format(@"D:\StylePicture\{0}.jpg", styleCode);
                        // string imagePath = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", styleCode);
                        string imgExtension = "";
                        string imagePath = GetImagePath(styleCode,out imgExtension);
                        Image img = Image.FromFile(imagePath);

                        //Resize Image To Get Thumbnail
                        Image newImage = new Bitmap(100, 100);
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
                        if (results != null)
                        {
                            //Insert

                            TblStyleImage tblStyleImage = new TblStyleImage();
                            tblStyleImage.TblStyle = tblStyle;
                            tblStyleImage.IsPrintable = false;
                            tblStyleImage.IsActive = true;
                            tblStyleImage.ImagePath = @"Uploads/Imagessss/StylePicture/"+styleCode+ imgExtension;
                            tblStyleImage.CreationDate = System.DateTime.Now;
                            tblStyleImage.ImagePathThumbnail = results;
                            tblStyleImage.DefaultImage = true;
                            tblStyleImage.OrginalFileName = styleCode + imgExtension;
                            context.TblStyleImages.AddObject(tblStyleImage);
                            int Saved =  context.SaveChanges();
                            
                            if (Saved > 0)
                            {
                                //Copy Image To Local Path
                                // File.Copy(imagePath, @"D:\CCWFM\CCWFM.Web\Uploads\Imagessss\StylePicture\" + styleCode+ imgExtension);
                                File.Copy(imagePath, @"D:\StylePicture\" + styleCode + imgExtension);

                            }

                            return true;
                        }
                        else
                        { return false; }
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                arr = ms.ToArray();
            }
            return arr;
        }
        public int GetMaxIserialOfStyle(out string imagePath)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var serialNo =
               context.TblStyleImages
              .Select(x => x.Iserial).Cast<int?>().Max();

                imagePath = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "ImagesFolderPath").sSetupValue;
                if (serialNo != null)
                { return (int)serialNo + 1; }
                return 1;
            }
        }

        private string GetImagePath(string StyleCode,out string imgExtension)
        {
            //Create SetUpPath for Image Path

            string Path = "";
            imgExtension = "";
            if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpg", StyleCode);
                imgExtension = ".jpg";
            }
            else if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.png", StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.png", StyleCode);
                imgExtension = ".png";
            }
            else if (File.Exists(string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpeg", StyleCode)))
            {
                Path = string.Format(@"\\192.168.1.14\Share\All\D and P\StylePicture\{0}.jpeg", StyleCode);
                imgExtension = ".jpeg";
            }
            return Path;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public byte[] GetStyleImageFromFolder(string styleCode,out double ImgHeiht, out double Imgwidth)
        {
            string imgExtension = "";
            string imagePath = GetImagePath(styleCode, out imgExtension);
            Image img = Image.FromFile(imagePath);
            ImgHeiht = img.Height;
            Imgwidth = img.Width;
            byte[] results;
            using (MemoryStream ms = new MemoryStream())
            {
                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters jpegParms = new EncoderParameters(1);
                jpegParms.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                img.Save(ms, codec, jpegParms);
                results = ms.ToArray();
            }
            return results;
        }
    }
}

