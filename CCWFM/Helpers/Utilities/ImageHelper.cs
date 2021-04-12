using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CCWFM.Helpers.Utilities
{
    public class ImageHelper
    {
        public static WriteableBitmap GetImageSource(Stream stream, int maxWidth, int maxHeight)
        {
            var bmp = new BitmapImage();
            bmp.SetSource(stream);

            var img = new Image { Source = bmp };

            double scaleX;
            double scaleY;
            int newWidth;
            int newHeight;

            if (bmp.PixelHeight > maxHeight)
            {
                scaleY = Convert.ToDouble(maxHeight) / bmp.PixelHeight;
                newHeight = maxHeight;
            }
            else
            {
                scaleY = 1;
                newHeight = bmp.PixelHeight;
            }

            if (bmp.PixelWidth > maxWidth)
            {
                scaleX = Convert.ToDouble(maxWidth) / bmp.PixelWidth;
                newWidth = maxWidth;
            }
            else
            {
                scaleX = 1;
                newWidth = bmp.PixelWidth;
            }

            double scale = 1;
            if (scaleX < scaleY)
            {
                scale = scaleX;
                newHeight = Convert.ToInt32(bmp.PixelHeight * scale);
            }
            else if (scaleY != 1)
            {
                scale = scaleY;
                newWidth = Convert.ToInt32(bmp.PixelWidth * scale);
            }

            // calculate new image size
            var result = new WriteableBitmap(newWidth, newHeight);
            result.Render(img, new ScaleTransform() { ScaleX = scale, ScaleY = scale });
            result.Invalidate();
            return result;
        }
    }
}