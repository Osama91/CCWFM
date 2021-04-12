using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.IO;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StyleImagePreview 
    {
        public StyleImagePreview(string StyleCode)
        {
            InitializeComponent();
            LoadImage(StyleCode);
        }

        private void LoadImage(string styleCode)
        {
            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
            _client.GetStyleImageFromFolderCompleted += (s, sv) => 
            {
                MemoryStream stream = new MemoryStream(sv.Result);
                BitmapImage b = new BitmapImage();
                b.SetSource(stream);
                imgStyle.Source = b;
                imgStyle.Height = sv.ImgHeiht;
                imgStyle.Width = sv.Imgwidth;

             
                double Ratio = sv.Imgwidth / sv.ImgHeiht;
                //if (Ratio < 1 && this.Height > sv.ImgHeiht)
                //{
                //    sv.ImgHeiht =  this.Height / sv.ImgHeiht
                //}
                this.Height = sv.ImgHeiht;
                this.Width = sv.Imgwidth;

            };
            _client.GetStyleImageFromFolderAsync(styleCode);

        }
    }
}
