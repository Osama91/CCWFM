using CCWFM.Web.DataLayer;
namespace CCWFM.Models.Inv
{
    public enum AuthWarehouseType
    {
        None = 0,
        TransferTo = 1,
        TransferFrom = 2,
        TransferToFrom = 3,
        Adjustment = 4,
        FirstDegreeWarehouse = 5,
        SecondDegreeWarehouse = 6,
        ThridDegreeWarehouse = 7
    }

    public class AuthWarehouseModel : PropertiesViewModelBase
    {
        bool isGranted = false;
        string warehoseEname = "", warehouseCode = "";
        int warehouseIserial = 0;

        public bool IsGranted
        {
            get { return isGranted; }
            set { isGranted = value; RaisePropertyChanged(nameof(IsGranted)); }
        }
        public string WarehoseEname
        {
            get { return warehoseEname; }
            set { warehoseEname = value; RaisePropertyChanged(nameof(WarehoseEname)); }
        }

        public string WarehouseCode
        {
            get { return warehouseCode; }
            set { warehouseCode = value; RaisePropertyChanged(nameof(WarehouseCode)); }
        }

        public int WarehouseIserial
        {
            get { return warehouseIserial; }
            set { warehouseIserial = value; RaisePropertyChanged(nameof(WarehouseIserial)); }
        }
    }
}
