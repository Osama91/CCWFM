using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Os.Controls
{
    public static class ExcelBehavior
    {
   
       

        //private static void GotFocusHandler(System.Windows.Controls.DataGrid dataGrid)
        //{
        //    if (dataGrid.Tag != null)
        //    {
        //        var box = GetCellItem<TextBox>(dataGrid.SelectedItem, dataGrid.CurrentColumn);
        //        if (box != null)
        //        {
        //            box.Text = dataGrid.Tag.ToString();
        //            box.SelectionStart = 1; //move editing cursor to end of text
        //        }
        //    }
        //}

        //private static void EditEndedHandler(System.Windows.Controls.DataGrid dataGrid)
        //{
        //    dataGrid.Tag = null;
        //}

        //public static void EnableForGrid(System.Windows.Controls.DataGrid dataGrid)
        //{
        //    dataGrid.KeyUp += (s, e) => KeyUpHandler(e, dataGrid);

        //    dataGrid.GotFocus += (s, e) => GotFocusHandler(dataGrid);

        //    dataGrid.CellEditEnded += (s, e) => EditEndedHandler(dataGrid);
        //}
    }
}