

namespace CCWFM.Web.DataLayer
{
    using System;
    using System.ComponentModel;
    public class ItemDimensionSearchModel : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            try
            {
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception) { }
        }

        #endregion Implement INotifyPropertyChanged

        public ItemDimensionSearchModel()
        {

        }

        int itemId, colorFromId, colorToId, itemDimFromIserial, itemDimToIserial,
            siteFromIserial, siteToIserial;
        bool isAcc = false;
        string itemCode, itemName, sizeFrom, sizeTo, batchNoFrom, batchNoTo,
            itemType, colorFromCode;
        decimal availableQuantity, pendingQuantity, transferredQuantity,
            availableToQuantity, pendingToQuantity;
        TblColor colorTo, colorFrom;

        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; RaisePropertyChanged(nameof(ItemId)); }
        }
        public string ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; RaisePropertyChanged(nameof(ItemCode)); }
        }
        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; RaisePropertyChanged(nameof(ItemName)); }
        }
        public string ItemType
        {
            get { return itemType; }
            set { itemType = value; RaisePropertyChanged(nameof(ItemType)); }
        }
        public decimal TransferredQuantity
        {
            get { return transferredQuantity; }
            set { transferredQuantity = value; RaisePropertyChanged(nameof(TransferredQuantity)); }
        }
        public bool IsAcc
        {
            get { return isAcc; }
            set { isAcc = value; RaisePropertyChanged(nameof(IsAcc)); }
        }
      
        #region From
        public int ColorFromId
        {
            get { return colorFromId; }
            set { colorFromId = value; RaisePropertyChanged(nameof(ColorFromId)); }
        }
        public string SizeFrom
        {
            get { return sizeFrom; }
            set { sizeFrom = value; RaisePropertyChanged(nameof(SizeFrom)); }
        }
        public string BatchNoFrom
        {
            get { return batchNoFrom; }
            set { batchNoFrom = value; RaisePropertyChanged(nameof(BatchNoFrom)); }
        }
        public string ColorFromCode
        {
            get { return colorFromCode; }
            set { colorFromCode = value; RaisePropertyChanged(nameof(ColorFromCode)); }
        }
        public int ItemDimFromIserial
        {
            get { return itemDimFromIserial; }
            set { itemDimFromIserial = value; RaisePropertyChanged(nameof(ItemDimFromIserial)); }
        }
        public int SiteFromIserial
        {
            get { return siteFromIserial; }
            set { siteFromIserial = value; RaisePropertyChanged(nameof(SiteFromIserial)); }
        }
        public decimal AvailableQuantity
        {
            get { return availableQuantity; }
            set { availableQuantity = value; RaisePropertyChanged(nameof(AvailableQuantity)); }
        }
        public decimal PendingQuantity
        {
            get { return pendingQuantity; }
            set { pendingQuantity = value; RaisePropertyChanged(nameof(PendingQuantity)); }
        }
        public TblColor ColorFrom
        {
            get { return colorFrom ?? (colorFrom = new TblColor()); }
            set { colorFrom = value; RaisePropertyChanged(nameof(ColorFrom)); }
        }
        #endregion

        #region To
        public int ColorToId
        {
            get { return colorToId; }
            set { colorToId = value; RaisePropertyChanged(nameof(ColorToId)); }
        }
        public string SizeTo
        {
            get { return sizeTo; }
            set { sizeTo = value; RaisePropertyChanged(nameof(SizeTo)); }
        }
        public string BatchNoTo
        {
            get { return batchNoTo; }
            set { batchNoTo = value; RaisePropertyChanged(nameof(BatchNoTo)); }
        }
        public TblColor ColorPerRow
        {
            get { return colorTo ?? (colorTo = new TblColor()); }
            set { colorTo = value; RaisePropertyChanged(nameof(ColorPerRow)); }
        }       
        public int ItemDimToIserial
        {
            get { return itemDimToIserial; }
            set { itemDimToIserial = value; RaisePropertyChanged(nameof(ItemDimToIserial)); }
        }
        public int SiteToIserial
        {
            get { return siteToIserial; }
            set { siteToIserial = value; RaisePropertyChanged(nameof(SiteToIserial)); }
        }
        public decimal AvailableToQuantity
        {
            get { return availableToQuantity; }
            set { availableToQuantity = value; RaisePropertyChanged(nameof(AvailableToQuantity)); }
        }
        public decimal PendingToQuantity
        {
            get { return pendingToQuantity; }
            set { pendingToQuantity = value; RaisePropertyChanged(nameof(PendingToQuantity)); }
        }
        #endregion
    }
}
