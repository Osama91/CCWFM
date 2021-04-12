using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace Os.Controls.DataGrid
{
    public class DateFormatConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var c = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                var date = DateTime.Parse(value.ToString());

                if (c != null) return date.ToString(c.DateTimeFormat.ShortDatePattern);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}