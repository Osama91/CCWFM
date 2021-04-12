using System;
using System.Collections.Generic;

namespace Os.Controls.DataGrid.Events
{
    public class FilterEvent : EventArgs
    {
        public IEnumerable<ColumnFilterControl> FiltersPredicate { get; set; }
    }
}