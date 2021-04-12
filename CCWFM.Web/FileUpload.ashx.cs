using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CCWFM.Web
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    public class FileUpload : IHttpHandler
    {

            private HttpContext _httpContext;
            private string _fileName;

            private bool _lastChunk;
            private bool _firstChunk;

            public string Folder { get; set; }

            public void ProcessRequest(HttpContext context)
            {
                _httpContext = context;

                if (context.Request.InputStream.Length == 0)
                    throw new ArgumentException("No file input");

                GetQueryStringParameters();

                var uploadFolder = Folder;

                if (_firstChunk)
                {
                    if (!Directory.Exists(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder))
                        Directory.CreateDirectory(@HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder);
                                
                }

                using (var fs = File.Open(@HostingEnvironment.ApplicationPhysicalPath +
                                             "/" + uploadFolder + "/" + _fileName, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                }

                //if (_lastChunk)
                //{
                //    File.Move(HostingEnvironment.ApplicationPhysicalPath + "/" + uploadFolder +
                //              "/" + _fileName, HostingEnvironment.ApplicationPhysicalPath + "/" +
                //                                  uploadFolder + "/" + _fileName);
                //}
            }

            private void GetQueryStringParameters()
            {
                _fileName = _httpContext.Request.QueryString["file"];
                Folder = _httpContext.Request.QueryString["Folder"];
                _lastChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["last"]) ||
                             bool.Parse(_httpContext.Request.QueryString["last"]);

                _firstChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["first"]) ||
                              bool.Parse(_httpContext.Request.QueryString["first"]);
            }

            private void SaveFile(Stream stream, FileStream fs)
            {
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                }
            }

            //public PictureFile Download(string pictureName)
            //{
            //    FileStream fileStream = null;
            //    BinaryReader reader = null;
            //    string imagePath;
            //    byte[] imageBytes;

            //    try
            //    {
            //        imagePath = HttpContext.Current.Server.MapPath(".") +
                        
            //        pictureName + ".jpg";

            //        if (File.Exists(imagePath))
            //        {
            //            fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            //            reader = new BinaryReader(fileStream);

            //            imageBytes = reader.ReadBytes((int)fileStream.Length);

            //            return new PictureFile() { PictureName = pictureName, PictureStream = imageBytes };
            //        }
            //        return null;
            //    }
            //    catch (Exception)
            //    {
            //        return null;
            //    }
            //}

            public bool IsReusable
            {
                get { return false; }
            }
        }
    }
