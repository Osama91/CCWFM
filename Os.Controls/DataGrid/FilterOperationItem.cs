namespace Os.Controls.DataGrid
{
    /// <summary>
    /// This class defines a filter operation for the ColumnFilterHeader.  i.e.  Equals, Contains, StartsWith....
    /// </summary>
    public class FilterOperationItem
    {
        public Enums.FilterOperation FilterOption { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }

        public string LinqUse { get; set; }

        public FilterOperationItem(Enums.FilterOperation operation, string description, string linqUse, string imagePath)
        {
            FilterOption = operation;
            Description = description;
            ImagePath = imagePath;
            LinqUse = linqUse;
        }
    }
}