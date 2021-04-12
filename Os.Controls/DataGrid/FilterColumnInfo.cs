using System;
using System.Globalization;
using System.Windows.Data;

namespace Os.Controls.DataGrid
{
    /// <summary>
    /// The FilterColumnInfo class has the information about the column binding information.  It is used to configure
    /// the ColumnFilterHEader and the ColumnOptionControl
    /// </summary>
    public class FilterColumnInfo
    {
        public string PropertyPath { get; set; }

        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public CultureInfo ConverterCultureInfo { get; set; }

        public Type PropertyType { get; set; }
    }
}