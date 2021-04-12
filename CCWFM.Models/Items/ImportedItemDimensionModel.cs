using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCWFM.Models.Items
{
    public class ImportedItemDimensionModel
    {
        string color, size = "", batchNo = "", itemCode;
        decimal qty, cost;
        public string BatchNo
        {
            get { return batchNo; }
            set { batchNo = value; }
        }
        public string Color
        {
            get { return color; }
            set { color = value; }
        }
        public string ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        public decimal Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }

    }
}
