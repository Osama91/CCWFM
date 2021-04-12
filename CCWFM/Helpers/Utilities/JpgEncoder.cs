using System.IO;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using FluxJpeg.Core.Encoder;

namespace CCWFM.Helpers.Utilities
{
    public class JpgEncoder
    {
        public static Stream Encode(WriteableBitmap bitmap, int quality)
        {
            //Convert the Image to pass into FJCore
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var bands = 3;

            var raster = new byte[bands][,];

            for (var i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (var row = 0; row < height; row++)
            {
                for (var column = 0; column < width; column++)
                {
                    var pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            var model = new ColorModel { colorspace = ColorSpace.RGB };

            var img = new Image(model, raster);

            //Encode the Image as a JPEG
            var stream = new MemoryStream();
            var encoder = new JpegEncoder(img, quality, stream);

            encoder.Encode();

            //Move back to the start of the stream
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}