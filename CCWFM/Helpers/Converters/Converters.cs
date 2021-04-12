using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CCWFM.CRUDManagerService;
using CCWFM.UserControls;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.PrintPreviews;
using ZXing;

namespace CCWFM.Helpers.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (System.Convert.ToDouble(value) ==0)
            {
                return null;
            }
            var timeDouble = System.Convert.ToDouble((value));
            var t = TimeSpan.FromSeconds(timeDouble);
            var timeConverted = new DateTime().Add(t);
            return timeConverted;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null) return null;
            //if (System.Convert.ToDouble(value) == 0)
            //{
            //    return null;
            //}
            var time = (DateTime)value;
            var t = time.TimeOfDay;
            var timeConverted = System.Convert.ToInt32(t.TotalSeconds);
            return timeConverted;
        }
    }

    public class PriceToBrushConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            bool enabled = System.Convert.ToBoolean(value);
            if (enabled)
            {
                return new SolidColorBrush(Colors.Green);
            }

            return new SolidColorBrush(Colors.Cyan);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter
    }

    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return EnumToIEnumerableConverter.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class EnumToIEnumerableConverter
    {
        private static readonly IDictionary<Type, object[]> Cache = new Dictionary<Type, object[]>();

        public static object[] GetValues(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            object[] values;
            if (!Cache.TryGetValue(type, out values))
            {
                values = type.GetFields()
                    .Where(f => f.IsLiteral)
                    .Select(f => f.GetValue(null))
                    .ToArray();
                Cache[type] = values;
            }
            return values;
        }
    }

    public class SelectedItemToIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value == null;
            if (parameter != null)
                return !result;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class SelectedItemToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value == null ? Visibility.Collapsed : Visibility.Visible;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class NumToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result = int.Parse(value.ToString()) != 100
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Colors.Green);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class TrueToRedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result = (bool)value
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Colors.Transparent);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class TrueToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (bool)value
                ? FontStyles.Italic
                : FontStyles.Normal;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ItemsCountToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ((ItemCollection)value).Count < 6;
            if (parameter != null)
                return ((ItemCollection)value).Count < int.Parse(parameter.ToString());
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ReversedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;
            try
            {
                result = !(bool)value;
            }
            catch (Exception)
            {
                if (value == null)
                {
                    value = 1;
                }

                var count = (int)value;
                result = count == 0;
            }

            if (parameter != null)
                return !result;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class PixelsConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Inch = 96.0;
            var Cm = 37.7952755905512;

            var strValue = value.ToString();

            var values = strValue.Split(" ".ToCharArray());
            var unit = values[1];
            var size = values[0];

            if (unit == "CM")
            {
                return System.Convert.ToDouble(size) * Cm;
            }
            if (unit == "Inch")
            {
                return System.Convert.ToDouble(size) * Inch;
            }
            return System.Convert.ToDouble(size);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class IntegerToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            if (parameter != null)
            {
                result = value.ToString() == "0" ? "" : value.ToString();
            }
            else
            {
                result = value.ToString() == "0" ? "In" : "Out";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class CustomeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ((List<MapperSizeInfo>)value).Sum(x => x.KeyValue);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                if (parameter != null)
                {
                    var visibility = !(bool)value;
                    return visibility ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    var visibility = (bool)value;
                    return visibility ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                if (parameter == null)
                {
                    if (value==null)
                    {

                        return Visibility.Collapsed;
                    }
                    if ((int)value <= 0)
                    {
                        return Visibility.Collapsed;
                    }
                    return Visibility.Visible;
                }
                if ((int)value == 0)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }

    public class AppendSignToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value + "%";
            if (parameter != null)
                return value.ToString() + parameter.ToString();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class GenericItemToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value == null ? "" : ((GenericViewModel)value).Ename;
            return parameter != null ? "" : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class FabricCategoryTypeToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result;
           
            result = value == null
                ? Visibility.Collapsed
                : parameter == null
                    ? Visibility.Collapsed
                    : parameter.ToString().Contains(value.ToString())
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            try
            {
                var valueint = System.Convert.ToInt32(value);
                if (parameter != null)
                {
                    string para = parameter.ToString();
                    if (value != null)
                        if (para == "ALL" && valueint > 0)
                        {
                            return Visibility.Visible;
                        }
                }
            }
            catch (Exception)
            {
                
                
            }
         
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class FabricCategoryTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && (parameter != null && (parameter.ToString().Contains(value.ToString())));
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ByteToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new BitmapImage();
            if (value != null)
            {
                using (var memStream = new MemoryStream(value as byte[]))
                {
                    result.SetSource(memStream);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NumericAdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (double)value;
            if (parameter != null)
                return (double)value + (double)parameter;
            return result + 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DisplayItemPathByLang : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
                return CultureInfo.CurrentCulture.Name.ToLower().Contains("en")
                    ? parameter.ToString().Split(';')[0]
                    : parameter.ToString().Split(';')[1];
            return CultureInfo.CurrentCulture.Name.ToLower().Contains("en") ? "Ename" : "Aname";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DefectsDisplayByLang : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defect = value as tbl_WF_Defects;
            return CultureInfo.CurrentCulture.Name.ToLower().Contains("en") ? defect.Ename : defect.Aname;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class Times10 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = System.Convert.ToDouble(value) * 10;
            return doubleValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class NinetyPercentOfTheValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = System.Convert.ToDouble(value) * .7;
            return doubleValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class BoldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boldValue = bool.Parse(value.ToString());

            return boldValue ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ItalicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var italicValue = bool.Parse(value.ToString());

            return italicValue ? FontStyles.Italic : FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class Barcode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var writer = new BarcodeWriter();
            switch (DisplayingBarcodeToPrintUserControl.BarcodeSettings.PrintingBarcodeFormate)
            {
                case "CODE_39":
                    {
                        writer.Format = BarcodeFormat.CODE_39;
                        break;
                    }

                case "All_1D":
                    {
                        writer.Format = BarcodeFormat.All_1D;
                        break;
                    }
                case "AZTEC":
                    {
                        writer.Format = BarcodeFormat.AZTEC;
                        break;
                    }

                case "CODABAR":
                    {
                        writer.Format = BarcodeFormat.CODABAR;
                        break;
                    }

                case "CODE_128":
                    {
                        writer.Format = BarcodeFormat.CODE_128;
                        break;
                    }
                case "CODE_93":
                    {
                        writer.Format = BarcodeFormat.CODE_39;
                        break;
                    }

                case "DATA_MATRIX":
                    {
                        writer.Format = BarcodeFormat.DATA_MATRIX;
                        break;
                    }

                case "EAN_13":
                    {
                        writer.Format = BarcodeFormat.EAN_13;
                        break;
                    }

                case "EAN_8":
                    {
                        writer.Format = BarcodeFormat.EAN_8;
                        break;
                    }

                case "ITF":
                    {
                        writer.Format = BarcodeFormat.ITF;
                        break;
                    }

                case "MAXICODE":
                    {
                        writer.Format = BarcodeFormat.MAXICODE;
                        break;
                    }

                case "MSI":
                    {
                        writer.Format = BarcodeFormat.MSI;
                        break;
                    }

                case "PDF_417":
                    {
                        writer.Format = BarcodeFormat.PDF_417;
                        break;
                    }
                case "PLESSEY":
                    {
                        writer.Format = BarcodeFormat.PLESSEY;
                        break;
                    }

                case "QR_CODE":
                    {
                        writer.Format = BarcodeFormat.QR_CODE;
                        break;
                    }

                case "RSS_14":
                    {
                        writer.Format = BarcodeFormat.RSS_14;
                        break;
                    }

                case "RSS_EXPANDED":
                    {
                        writer.Format = BarcodeFormat.RSS_EXPANDED;
                        break;
                    }
                case "UPC_A":
                    {
                        writer.Format = BarcodeFormat.UPC_A;
                        break;
                    }

                case "UPC_E":
                    {
                        writer.Format = BarcodeFormat.UPC_E;
                        break;
                    }

                case "UPC_EAN_EXTENSION":
                    {
                        writer.Format = BarcodeFormat.UPC_EAN_EXTENSION;
                        break;
                    }
                default:
                    {
                        writer.Format = BarcodeFormat.CODE_39;
                        break;
                    }
            }

            var writeableBitmap = writer.Write(value.ToString());
            return writeableBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class FontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? new FontFamily(value.ToString()) : new FontFamily("arial");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class UnderLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var underlineValue = bool.Parse(value.ToString());

            if (underlineValue)
            {
                return TextDecorations.Underline;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class FileInfoIntoImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var file = value as FileInfo;

            var image = new BitmapImage();
            try
            {
                image.SetSource(file.OpenRead());
                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class IntToEnabledConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return (int?)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}