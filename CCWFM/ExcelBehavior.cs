using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Browser;

namespace CCWFM.ViewModel
{
    public static class ExcelBehavior
    {
        private static string GetCellData(IEnumerable items, DataGrid dataGrid)
        {
            if (dataGrid.ItemsSource == null) return null;

            //build row-newline delim, column tab delim string
            var sb = new StringBuilder("");

            //add headers first
            var rowData = new List<string>();
            var columnData = new List<string>();
            foreach (var column in dataGrid.Columns)
            {
                var cellData = column.Header;
                if (cellData != null)
                    columnData.Add(cellData.ToString());
            }
            rowData.Add(String.Join("\t", columnData.ToArray()));

            //for each selected item get all column data
            foreach (var item in items)
            {
                columnData = new List<string>();
                foreach (var column in dataGrid.Columns)
                {
                    //cells that are scrolled out of view aren't created yet and won't give us their content
                    //therefore we engage in some STSOOI behavior to get er' done
                    dataGrid.ScrollIntoView(item, column);

                    columnData.Add(GetCellText(item, column));
                }
                rowData.Add(String.Join("\t", columnData.ToArray()));
            }
            sb.Append(String.Join(Environment.NewLine, rowData.ToArray()));
            string textData = sb.ToString();
            return textData;
        }

        private static string GetCellText(object item, DataGridColumn column)
        {
            var cellData = GetCellTextBlock(item, column);

            if (cellData != null)
                return cellData.Text;
            else
                return null;
        }

        private static TextBlock GetCellTextBlock(object item, DataGridColumn column)
        {
            return GetCellItem<TextBlock>(item, column);
        }

        private static T GetCellItem<T>(object item, DataGridColumn column) where T : class
        {
            if (item == null) return null;

            var cellData = (column.GetCellContent(item) as T);
            if (cellData == null)
            {
                //for our custom columns
                var gridData = (column.GetCellContent(item) as Panel);
                if (gridData != null)
                {
                    cellData = (gridData.Children.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T);
                }
            }
            return cellData;
        }

        private static void SetCellData(object startObject, DataGrid dataGrid, string textData)
        {
            if (dataGrid.ItemsSource == null) return;
            if (String.IsNullOrEmpty(textData)) return;

            //first split for rows
            string[] rowData = Regex.Split(textData, Environment.NewLine);
            if (rowData.Length == 0) return;

            //enumerate datasource just once
            var itemsSource = new List<object>();
            foreach (var item in dataGrid.ItemsSource) itemsSource.Add(item);

            //don't support more rows than we have for now
            if (rowData.Length > itemsSource.Count)
            {
                MessageBox.Show("Can't paste " + rowData.Length + " rows, only " + itemsSource.Count + " rows in grid");
                return;
            }

            //get writeable column count in grid
            int writeableColumnCount = dataGrid.Columns.Where(x => !x.IsReadOnly).Count();
            int totalColumnCount = dataGrid.Columns.Count;

            //allow them to either paste the same number as in grid or exact number of writeable columns
            int pastedColumnCount = rowData[0].Split('\t').Length;
            var columnsToPasteTo = new List<DataGridColumn>();
            if (pastedColumnCount == totalColumnCount)
                columnsToPasteTo.AddRange(dataGrid.Columns);
            else if (pastedColumnCount < totalColumnCount)
                columnsToPasteTo.AddRange(dataGrid.Columns.Where(x => !x.IsReadOnly));

            //find where we will start in the grid if not the first row
            int gridRow = 0;
            if (startObject != null)
            {
                foreach (var item in itemsSource)
                {
                    if (startObject == item)
                        break;

                    gridRow++;
                }
            }

            //send in the data
            for (int i = 0; i < rowData.Length; i++)
            {
                if (gridRow >= itemsSource.Count) break;

                var item = itemsSource[gridRow++];

                //skip blank last line if any
                if (String.IsNullOrEmpty(rowData[i])) continue;

                //get column data from row
                var columnsOfData = rowData[i].Split('\t');

                //these should be the same number 
                if (columnsOfData.Length > columnsToPasteTo.Count)
                {
                    MessageBox.Show("More columns in data than grid, aborting");
                    return;
                }

                for (int j = 0; j < columnsOfData.Length; j++)
                {
                    var columnData = columnsOfData[j];
                    var column = columnsToPasteTo[j];
                    var cellData = GetCellTextBlock(item, column);

                    //skip read only columns
                    if (column.IsReadOnly) continue;

                    //cells that are scrolled out of view aren't created yet and won't give us their content
                    //therefore we engage in some STSOOI behavior to get er' done
                    dataGrid.ScrollIntoView(item, column);

                    if (cellData != null)
                        cellData.Text = columnData;
                    //else throw error or ignore if cant set one of the cells? 
                }
            }
        }

        private static void KeyUpHandler(KeyEventArgs e, DataGrid dataGrid)
        {
            //copy
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.C)
                {
                    var items = dataGrid.SelectedItems;
                    if (items.Count == 0) return;

                    //don't let it bubble up so nothing else happens
                    e.Handled = true;

                    //JS supports clipboard, silverlight doesn't, use it
                    var clipboardData = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");
                    clipboardData.Invoke("setData", "text", ExcelBehavior.GetCellData(items, dataGrid));
                   // MessageBox.Show("Your data is now available for pasting");
                }
                else if (e.Key == Key.V)
                {
                    var items = dataGrid.SelectedItems;
                    object startObject = (items.Count > 0 ? items[0] : null);

                    //don't let it bubble up so nothing else happens
                    e.Handled = true;

                    //JS supports clipboard, silverlight doesn't, use it
                    var clipboardData = (ScriptObject)HtmlPage.Window.GetProperty("clipboardData");
                    string textData = clipboardData.Invoke("getData", "text").ToString();
                    ExcelBehavior.SetCellData(startObject, dataGrid, textData);
                }
            }
            else
            {
                //don't start editing for nav keys so a left arrow, etc. doesn't put text in a box
                if (dataGrid.Tag == null && !IsNavigationKey(e) && !dataGrid.CurrentColumn.IsReadOnly)
                {
                    bool isShifty = ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                    string letter = ((char)e.PlatformKeyCode).ToString();
                    letter = (isShifty ? letter.ToUpper() : letter.ToLower());

                    dataGrid.Tag = letter;

                    //beginedit will fire the focus event
                    //if we try to access the textbox here its text will not be set
                    dataGrid.BeginEdit();
                }
            }
        }

        private static bool IsNavigationKey(KeyEventArgs e)
        {
            return new[] { Key.Escape, Key.Enter, Key.Down, Key.Up, Key.Left, Key.Right, Key.Tab, Key.Shift, Key.Ctrl, Key.Alt }.Contains(e.Key);
        }

        private static void GotFocusHandler(DataGrid dataGrid)
        {
            if (dataGrid.Tag != null)
            {
                var box = ExcelBehavior.GetCellItem<TextBox>(dataGrid.SelectedItem, dataGrid.CurrentColumn);
                if (box != null)
                {
                    box.Text = dataGrid.Tag.ToString();
                    box.SelectionStart = 1; //move editing cursor to end of text
                }
            }
        }

        private static void EditEndedHandler(DataGrid dataGrid)
        {
            dataGrid.Tag = null;
        }

        public static void EnableForGrid(DataGrid dataGrid)
        {
            dataGrid.KeyUp += (s, e) =>
            {
                KeyUpHandler(e, dataGrid);
            };

            dataGrid.GotFocus += (s, e) =>
            {
                GotFocusHandler(dataGrid);
            };

            dataGrid.CellEditEnded += (s, e) =>
            {
                EditEndedHandler(dataGrid);
            };
        }
    }
}
