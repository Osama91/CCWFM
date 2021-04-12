namespace Os.Controls.DataGrid
{
    public class Enums
    {
        public enum FilterOperation
        {
            Unknown,
            Contains,
            Equals,
            StartsWith,
            EndsWith,
            GreaterThanEqual,
            LessThanEqual,
            GreaterThan,
            LessThan
        }

        public enum ColumnOption
        {
            Unknown = 0,
            AddGrouping,
            RemoveGrouping,
            PinColumn,
            UnpinColumn
        }
    }
}