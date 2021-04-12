namespace CCWFM.Models.Excel
{
    public  class CellModel
    {
        int column, row;
        string value, year, mounth, day;

        public int Column
        {
            get { return column; }
            set { column = value; }
        }
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public string Day
        {
            get { return day; }
            set { this.day = value; }
        }
        public string Mounth
        {
            get { return mounth; }
            set { this.mounth = value; }
        }
        public string Year
        {
            get { return year; }
            set { this.year = value; }
        }
    }
}
