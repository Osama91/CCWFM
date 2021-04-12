using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using Lite.ExcelLibrary.SpreadSheet;
using Os.Controls;
using System.Reflection;

namespace CCWFM.Helpers.Utilities
{


    public static class GlobalMethods
    {
        private static bool IsPrimitive(Type type)
        {
            return type == typeof(byte) ||
                type == typeof(byte?) ||
                    type == typeof(sbyte) ||
                    type == typeof(sbyte?) ||
                    type == typeof(short) ||
                    type == typeof(short?) ||
                    type == typeof(ushort) ||
                    type == typeof(ushort?) ||
                    type == typeof(int) ||
                     type == typeof(int?) ||
                    type == typeof(uint) ||
                    type == typeof(long) ||
                    type == typeof(long?) ||
                    type == typeof(ulong) ||
                    type == typeof(ulong?) ||
                    type == typeof(float) ||
                       type == typeof(float?) ||
                    type == typeof(double) ||
                    type == typeof(double?) ||
                    type == typeof(decimal) ||
                    type == typeof(decimal?) ||
                     type == typeof(bool) ||
                     type == typeof(bool?) ||



                    type == typeof(char);
        }
        public static IList<string> GetDifferingProperties(object source, object target)
        {
            var sourceType = source.GetType();
            var sourceProperties = sourceType.GetProperties();
            var targetType = target.GetType();
            var targetProperties = targetType.GetProperties();

            var result = new List<string>();

            foreach (var property in
                (from s in sourceProperties
                 from t in targetProperties
                 where s.Name == t.Name &&
                 s.PropertyType == t.PropertyType &&
                 !Equals(s.GetValue(source, null), t.GetValue(target, null))
                 select new { Source = s, Target = t }))
            {
                // it's up to you to decide how primitive is primitive enough
                if (IsPrimitive(property.Source.PropertyType))
                {
                    result.Add(property.Source.Name);
                }
                else
                {
                    foreach (var subProperty in GetDifferingProperties(
                        property.Source.GetValue(source, null),
                        property.Target.GetValue(target, null)))
                    {
                        result.Add(property.Source.Name + "." + subProperty);
                    }
                }
            }

            return result;
        }


        public static void BrowseImage(out byte[] image)
        {
            image = null;
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                if (dlg.Files.Any(x => x.Length > 1048576))
                {
                    new ErrorWindow("Error Uploading Image"
                        , "I have detected that the uploaded Image exceeds the allowed size which is 1 Megabyte...\nPlease note that any image larger than 1 MB will not be added to the galary!");
                }
                if (dlg.Files.ToList().Count > 0)
                {
                    if (dlg.File.Length <= 1048576)
                    {
                        var reader = dlg.File.OpenRead();
                        var byteArray = new byte[reader.Length];
                        reader.Read(byteArray, 0, Convert.ToInt32(reader.Length));
                        image = byteArray;
                    }
                }
            }
        }

        #region Attached Property

        /// <summary>
        /// Include current column in export report to excel

        public static readonly DependencyProperty IsExportedProperty = DependencyProperty.RegisterAttached("IsExported",
                                                                                        typeof(bool), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(true));

        /// <summary>
        /// Use custom header for report
        /// </summary>
        public static readonly DependencyProperty HeaderForExportProperty = DependencyProperty.RegisterAttached("HeaderForExport",
                                                                                        typeof(string), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(null));

        /// <summary>
        /// Use custom path to get value for report
        /// </summary>
        public static readonly DependencyProperty PathForExportProperty = DependencyProperty.RegisterAttached("PathForExport",
                                                                                        typeof(string), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(null));

        /// <summary>
        /// Use custom path to get value for report
        /// </summary>
        public static readonly DependencyProperty FormatForExportProperty = DependencyProperty.RegisterAttached("FormatForExport",
                                                                                        typeof(string), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(null));


        #endregion Attached Property



        private static string GetHeader(DataGridColumn column)
        {
            var headerForExport = GetHeaderForExport(column);
            if (headerForExport == null && column.Header != null)
                return column.Header.ToString();
            return headerForExport;
        }

        private static object GetValue(IEnumerable<string> path, object obj, string formatForExport)
        {
            foreach (var pathStep in path)
            {
                if (obj == null)
                    return null;

                var type = obj.GetType();
                var property = type.GetProperty(pathStep);

                if (property == null)
                {
                    Debug.WriteLine("Couldn't find property '{0}' in type '{1}'", pathStep, type.Name);
                    return null;
                }

                obj = property.GetValue(obj, null);
            }

            if (!string.IsNullOrEmpty(formatForExport))
                return string.Format("{0:" + formatForExport + "}", obj);

            return obj;
        }

        private static string[] GetPath(DataGridColumn gridColumn)
        {
            var path = GetPathForExport(gridColumn);

            if (string.IsNullOrEmpty(path))
            {
                if (gridColumn is DataGridBoundColumn)
                {
                    var binding = ((DataGridBoundColumn)gridColumn).Binding;
                    if (binding != null)
                    {
                        path = binding.Path.Path;
                    }
                }
                else
                {
                    path = gridColumn.SortMemberPath;
                }
            }

            return string.IsNullOrEmpty(path) ? null : path.Split('.');
        }


        public static void ExportExcel(this DataGrid grid, string excelName)
        {
            var sDialog = new SaveFileDialog { Filter = "Excel Files(*.xls)|*.xls" };
            if (sDialog.ShowDialog() == true)
            {
                // create an instance of excel workbook
                var workbook = new Workbook();
                // create a worksheet object
                var worksheet = new Worksheet(excelName);
                // write data in worksheet cells
                //  var a = grid.GetRows();
                var list = grid.ItemsSource.Cast<object>().ToList();
                var data = new object[list.Count + 1, grid.Columns.Count];

                var columns = grid.Columns.Where(x => x.SortMemberPath != "Iserial" && GetIsExported(x));

                for (var columnIndex = 0; columnIndex < columns.Count(); columnIndex++)
                {
                    var gridColumn = columns.ElementAt(columnIndex);

                    data[0, columnIndex] = GetHeader(gridColumn);
                    var cell = new Cell(columns.ElementAt(columnIndex).Header);

                    worksheet.Cells[0, columnIndex] = cell;

                    var path = GetPath(gridColumn);

                    var formatForExport = GetFormatForExport(gridColumn);

                    if (path != null)
                    {
                        // Fill data with values
                        for (var rowIndex = 1; rowIndex <= list.Count; rowIndex++)
                        {
                            var source = list[rowIndex - 1];
                            data[rowIndex, columnIndex] = GetValue(path, source, formatForExport);
                            worksheet.Cells[rowIndex, columnIndex] = new Cell(data[rowIndex, columnIndex]);
                        }
                    }
                }

                workbook.Worksheets.Add(worksheet);
                var sFile = sDialog.OpenFile();

                // save method needs a stream object to write an excel file.
                workbook.Save(sFile);
            }
        }

        #region Attached properties helpers methods

        public static void SetIsExported(DataGridColumn element, Boolean value)
        {
            element.SetValue(IsExportedProperty, value);
        }

        public static Boolean GetIsExported(DataGridColumn element)
        {
            return (Boolean)element.GetValue(IsExportedProperty);
        }

        public static void SetPathForExport(DataGridColumn element, string value)
        {
            element.SetValue(PathForExportProperty, value);
        }

        public static string GetPathForExport(DataGridColumn element)
        {
            return (string)element.GetValue(PathForExportProperty);
        }

        public static void SetHeaderForExport(DataGridColumn element, string value)
        {
            element.SetValue(HeaderForExportProperty, value);
        }

        public static string GetHeaderForExport(DataGridColumn element)
        {
            return (string)element.GetValue(HeaderForExportProperty);
        }

        public static void SetFormatForExport(DataGridColumn element, string value)
        {
            element.SetValue(FormatForExportProperty, value);
        }

        public static string GetFormatForExport(DataGridColumn element)
        {
            return (string)element.GetValue(FormatForExportProperty);
        }

        #endregion Attached properties helpers methods

        public static void ExportExcel<T>(this IEnumerable<T> list, Stream File, string sheetName)
        {
            // create an instance of excel workbook
            var workbook = new Workbook();
            // create a worksheet object
            var worksheet = new Worksheet(sheetName);
            // write data in worksheet cells
            //  var a = grid.GetRows();
            var props = typeof(T).GetProperties();
            var data = new object[list.Count() + 1, props.Count()];

            for (var columnIndex = 0; columnIndex < props.Count(); columnIndex++)
            {
                var prop = props.ElementAt(columnIndex);

                data[0, columnIndex] = prop.Name;
                var cell = new Cell(props.ElementAt(columnIndex).Name);

                worksheet.Cells[0, columnIndex] = cell;

                // Fill data with values
                for (var rowIndex = 1; rowIndex <= list.Count(); rowIndex++)
                {
                    var source = list.ElementAt(rowIndex - 1);
                    data[rowIndex, columnIndex] = prop.GetValue(source, null).ToString();
                    worksheet.Cells[rowIndex, columnIndex] = new Cell(data[rowIndex, columnIndex]);
                }

            }

            workbook.Worksheets.Add(worksheet);
            var sFile = File;

            // save method needs a stream object to write an excel file.
            workbook.Save(sFile);
        }

        static void WriteTsv<T>(this IEnumerable<T> data, TextWriter output)
        {
            var props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                output.Write(prop.Name); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyInfo prop in props)
                {
                    output.Write(prop.GetValue(item, null));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }
    }
}