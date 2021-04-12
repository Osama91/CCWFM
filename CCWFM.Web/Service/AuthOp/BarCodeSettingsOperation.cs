using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.AuthOp
{
    public partial class AuthService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_lkp_BarcodeOperations> GetBarcodeOperationsLkp()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.tbl_lkp_BarcodeOperations.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<PrintingBarcodeProperty> GetBarcodeProperty()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.PrintingBarcodeProperties.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<BarcodeDisplaySettingsDetail> GetBarcodeDisplaySettingsDetail(int BarcodeDisplaySettingsHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.BarcodeDisplaySettingsDetails.Where(x => x.BarcodeDisplaySettingsHeader == BarcodeDisplaySettingsHeader).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateAndSaveBarcodeDisplaySettingsDetails(int BarcodeDisplaySettingsHeader, double? FontSize, bool? BoldProperty, bool? ItalicProperty, double CanvasTop, double CanvasLeft, string FontFamily, int propertyIserial, string PropertyName, string PropertyNameArabic, string PropertyType)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var newRecord = false;
                var displayDetailsRow = (from h in context.BarcodeDisplaySettingsDetails
                                         where h.BarcodeDisplaySettingsHeader == BarcodeDisplaySettingsHeader && h.printingPropertiesIserial == propertyIserial
                                         && h.PropertyName == PropertyName
                                         select h).SingleOrDefault();
                if (displayDetailsRow == null)
                {
                    displayDetailsRow = new BarcodeDisplaySettingsDetail();
                    newRecord = true;
                }

                displayDetailsRow.BarcodeDisplaySettingsHeader = BarcodeDisplaySettingsHeader;
                displayDetailsRow.PropertyName = PropertyName;
                displayDetailsRow.PropertyNameArabic = PropertyNameArabic;
                displayDetailsRow.PropertyType = PropertyType;
                displayDetailsRow.FontSize = FontSize;
                displayDetailsRow.BoldProperty = BoldProperty;
                displayDetailsRow.ItalicProperty = ItalicProperty;
                displayDetailsRow.FontFamily = FontFamily;
                displayDetailsRow.CanvasLeft = CanvasLeft;
                displayDetailsRow.CanvasTop = CanvasTop;
                displayDetailsRow.printingPropertiesIserial = propertyIserial;

                if (newRecord)
                {
                    context.BarcodeDisplaySettingsDetails.AddObject(displayDetailsRow);
                }
                context.SaveChanges();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<BarcodeDisplaySettingsHeader> GetBarcodeDisplaySettingsHeader()
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                return context.BarcodeDisplaySettingsHeaders.ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public string UpdateAndSaveBarcodeDisplaySettingsHeader(int Iserial, int barcodeOperation, string PrintingBarcodeFormate, double BarcodeWidth, double BarcodeHeight, string code, double PageWidth, double PageHeight, string PageSizeUnit)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var displayHeaderRow = new BarcodeDisplaySettingsHeader();

                if (Iserial != 0)
                {
                    displayHeaderRow = (from h in context.BarcodeDisplaySettingsHeaders
                                        where h.Iserial == Iserial
                                        select h).SingleOrDefault();
                }

                displayHeaderRow.BarcodeOperation = barcodeOperation;
                displayHeaderRow.PrintingBarcodeFormate = PrintingBarcodeFormate;
                displayHeaderRow.BarcodeWidth = BarcodeWidth;
                displayHeaderRow.BarcodeHeight = BarcodeHeight;
                displayHeaderRow.Code = code;
                displayHeaderRow.PageHeight = PageHeight;
                displayHeaderRow.PageWidth = PageWidth;
                displayHeaderRow.PageSizeUnit = PageSizeUnit;
                if (Iserial == 0)
                {
                    context.BarcodeDisplaySettingsHeaders.AddObject(displayHeaderRow);
                }
                context.SaveChanges();
                return code;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteBarcodeDisplaySettingsHeader(int Iserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var barcodeSettingRowQuery = (from h in context.BarcodeDisplaySettingsHeaders
                                              where h.Iserial == Iserial
                                              select h).SingleOrDefault();

                var HeaderIserial = barcodeSettingRowQuery.Iserial;

                var details = (from h in context.BarcodeDisplaySettingsDetails
                               where h.BarcodeDisplaySettingsHeader == HeaderIserial
                               select h).ToList();

                foreach (var item in details)
                {
                    context.DeleteObject(item);
                }

                context.DeleteObject(barcodeSettingRowQuery);
                context.SaveChanges();
            }
        }
    }
}