using CCWFM.Models.Gl;
using CCWFM.Web.Model;
using CCWFM.Web.Service.Operations;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web.Hosting;
using Excel = Microsoft.Office.Interop.Excel;

namespace CCWFM.Web.Service.BankStatOp
{
    public partial class BankStatService
    {
        [OperationContract]
        private List<TblBankStatHeader> GetBankStatHeader(int skip, int take, string sort, string filter, Dictionary<string, object> valuesObjects,
            out int fullCount, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var defaultQuery = context.TblBankStatHeaders.Include(
                    nameof(TblBankStatHeader.TblBank1)).Include(
                    nameof(TblBankStatHeader.TblCurrency1));
                IQueryable<TblBankStatHeader> query;
                if (filter != null)
                {
                    var parameterCollection = SharedOperation.ConvertToParamters(valuesObjects);

                    fullCount = defaultQuery.Where(filter, parameterCollection.ToArray()).Count();
                    query = defaultQuery.Where(filter, parameterCollection.ToArray()).OrderBy(sort);
                }
                else
                {
                    fullCount = defaultQuery.Count();
                    query = defaultQuery.OrderBy(sort);
                }
                return query.Skip(skip).Take(take).ToList();
            }
        }

        [OperationContract]
        private TblBankStatHeader GetBankStatHeaderByIserial(int headerIserial, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                return context.TblBankStatHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
            }
        }
        [OperationContract]
        private bool IsBankStatHeaderHasMatchedRows(int headerIserial, out int Iserial, string company)
        {
            Iserial = headerIserial;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var header = context.TblBankStatHeaders.Include(nameof(TblBankStatHeader.TblBankStatDetails)).FirstOrDefault(ah =>
                       ah.Iserial == headerIserial&&ah.TblBankStatDetails.Any(d =>
                        d.TblBankStatDetailTblLedgerDetails.Count > 0));
                if (header != null)
                    return true;// header.TblBankStatDetails.Any(d =>
                        //d.TblBankStatDetailTblLedgerDetails.Count > 0);
                else return false;
            }
        }
        [OperationContract]
        private TblBankStatHeader UpdateOrInsertBankStatHeader(TblBankStatHeader newRow, int index, int userIserial,
            out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                try
                {
                    newRow.TblBank1 = null;
                    newRow.TblCurrency1 = null;
                    var oldRow = context.TblBankStatHeaders.Include(nameof(TblBankStatHeader.TblBankStatDetails)).FirstOrDefault(th => th.Iserial == newRow.Iserial);
                    if (oldRow != null)// الهيدر موجود قبل كده
                    {
                        newRow.LastChangeUser = userIserial;
                        newRow.LastChangeDate = DateTime.Now;
                        if (!oldRow.MatchApproved && newRow.MatchApproved)// كده لسه معموله ابروف
                        {
                            newRow.MatchApproveDate = DateTime.Now;
                            newRow.MatchApprovedBy = userIserial;
                        }
                        if (!oldRow.Approved && newRow.Approved)// كده لسه معموله ابروف
                        {
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        foreach (var item in newRow.TblBankStatDetails.ToArray())
                        {
                            // هشوف بقى الى اتعدل والجديد
                            int temp, headeriserial;
                            headeriserial = item.TblBankStatHeader;
                            item.TblBankStatHeader1 = null;
                            item.TblBankStatHeader = headeriserial;
                            UpdateOrInsertBankStatDetail(item, 1, out temp, company);
                            item.TblBankStatHeader1 = newRow;
                        }
                        //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                        SharedOperation.GenericUpdate(oldRow, newRow, context);
                    }
                    else// الهيدر ده جديد
                    {
                        if (newRow.MatchApproved)// كده معموله ابروف
                        {
                            newRow.MatchApproveDate = DateTime.Now;
                            newRow.MatchApprovedBy = userIserial;
                        }
                        if (newRow.Approved)// كده معموله ابروف
                        {
                            newRow.ApproveDate = DateTime.Now;
                            newRow.ApprovedBy = userIserial;
                        }
                        newRow.CreatedBy = userIserial;
                        newRow.CreationDate = DateTime.Now;
                        newRow.LastChangeDate = DateTime.Now;
                        newRow.LastChangeUser = userIserial;

                        context.TblBankStatHeaders.AddObject(newRow);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex) { throw ex; }
                return newRow;
            }
        }

        [OperationContract]
        private int DeleteBankStatHeader(TblBankStatHeader row, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankStatHeaders
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row.Iserial;
        }
        
        [OperationContract]
        private List<TblBankStatDetail> GetBankStatDetail(int skip, int take, int headerId, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var query = context.TblBankStatDetails.Include(nameof(TblBankStatDetail.TblBankStatHeader1
                    )).Where(v =>
                    v.TblBankStatHeader == headerId).OrderBy(x => x.Iserial).Skip(skip).Take(take);
                var result = query.ToList();
                return result;
            }
        }

        [OperationContract]
        private TblBankStatDetail UpdateOrInsertBankStatDetail(TblBankStatDetail newRow, int index, out int outindex, string company)
        {
            outindex = index;
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankStatDetails
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //context.Entry(oldRow).CurrentValues.SetValues(newRow);
                    SharedOperation.GenericUpdate(oldRow, newRow, context);
                }
                else
                {
                    context.TblBankStatDetails.AddObject(newRow);
                }

                context.SaveChanges();
                return newRow;
            }
        }

        [OperationContract]
        private TblBankStatDetail DeleteBankStatDetail(TblBankStatDetail row, string company)
        {
            using (var context = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var oldRow = (from e in context.TblBankStatDetails
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null) context.DeleteObject(oldRow);

                context.SaveChanges();
            }
            return row;
        }
        
        #region Import
        [OperationContract]
        private int InsertImportedItems(TblBankStatHeader header,
            List<ImportedBankStatement> importedList, string company)
        {
            List<string> errors = new List<string>();
            using (var entities = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                try
                {
                    header.Approved = false;
                    foreach (var item in importedList)
                    {
                        TblBankStatDetail detail = PrepareDetail(entities, header, item, errors);
                        if (detail != null)
                            header.TblBankStatDetails.Add(detail);
                    }

                    header.TblBank1 = null;
                    header.TblCurrency1 = null;

                    header.CreationDate = DateTime.Now;
                    header.LastChangeDate = DateTime.Now;
                    header.LastChangeUser = header.CreatedBy;

                    entities.TblBankStatHeaders.AddObject(header);
                    entities.SaveChanges();
                    return header.Iserial;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        [OperationContract]
        private List<string> InsertRemainingImportedItems(int headerIserial, List<ImportedBankStatement> importedList, string company)
        {
            using (var entities = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var header = entities.TblBankStatHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);
                List<string> errors = new List<string>();
                try
                {
                    foreach (var item in importedList)
                    {
                        TblBankStatDetail detail = PrepareDetail(entities, header, item, errors);
                        if (detail != null)
                            // Add detail
                            header.TblBankStatDetails.Add(detail);
                    }
                    entities.SaveChanges();
                    return errors;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private static TblBankStatDetail PrepareDetail(ccnewEntities entities, TblBankStatHeader header,
            ImportedBankStatement transaction, List<string> errors)
        {
            string recInfo = string.Format(
                "Doc Date:{0}, Transaction Type:{1}, Amount:{2}, ChequeNo:{3}, DepositNo:{4}, DepositNo:{5}"
                , transaction.DocDate, transaction.TransactionType, transaction.Amount, transaction.ChequeNo, transaction.DepositNo, transaction.Description);

            var BankTransactionType = entities.TblBankTransactionTypes.Where(i => i.Code == transaction.TransactionType);
        
            if (BankTransactionType.Count() != 1)
            {
                if (BankTransactionType.Count() == 0)
                    errors.Add(string.Format("{1} -->> Bank transaction type not Found. More info -->> {0}",
                        recInfo, DateTime.Now));
                else
                    errors.Add(string.Format("{1} -->> found more than one bank transaction type . More info -->> {0}",
                        recInfo, DateTime.Now));
                return null;
            }
            var detail = new TblBankStatDetail()
            {
                TblBankStatHeader = header.Iserial,
                TblBankTransactionType = BankTransactionType.FirstOrDefault().Iserial,
                DocDate = transaction.DocDate,
                Description = transaction.Description,
                Amount = transaction.Amount,
                ChequeNo = transaction.ChequeNo,
                DepositNo = transaction.DepositNo,
            };
            return detail;
        }
        [OperationContract]
        private void DeleteBankStatByIserial(int headerIserial, string company)
        {
            using (var entities = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var temp = entities.TblBankStatHeaders.FirstOrDefault(ah => ah.Iserial == headerIserial);               
                if (temp != null)
                {
                    entities.DeleteObject(temp);
                }
                entities.SaveChanges();
            }
        }
        #endregion

        [OperationContract]
        private int InsertExcelFileDate(TblBankStatHeader header,
           List<Models.Excel.CellModel> detailList, string company)
        {
            List<string> errors = new List<string>();

            using (var entities = new ccnewEntities(SharedOperation.GetSqlConnectionString(company)))
            {
                var template = entities.TblBankStatExcelTemplates.FirstOrDefault(t => t.TblBank == header.TblBank);
                if (template == null) throw new ArgumentNullException("Template", "Cannot find Bank Template");
                string templatePath = string.Format("{0}\\BankStatExcelTemplates\\{1}","..", template.TemplatePath).Replace("\\\\", "\\");
                string workingPath = string.Format("{0}\\BankStatExcelTemplates\\{1}\\{2}\\{3}", "..",
                    DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss"), Guid.NewGuid(),
                    template.TemplatePath).Replace("\\\\", "\\");
                string workingDirectory = Path.GetDirectoryName(workingPath);
                if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);
                File.Copy(templatePath, workingPath);
                var fs = new FileInfo(workingPath);//, FileMode.Create);
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    int baseColIndex = 3;
                    var sheet = package.Workbook.Worksheets.First();
                    //Excel._Worksheet sheet = xlWorkbook.Sheets[1];
                    //sheet.Column(2).Style.Numberformat.Format = "dd-mm-yyyy";
                    foreach (var item in detailList)
                    {
                        //((Excel.Range)sheet.Cells[item.Row + 1, item.Column + 1]).Value = item.Value;
                        if (item.Column + 1 == 2 && item.Value.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                        {
                            sheet.Cells[item.Row + 1, 1].LoadFromText(item.Day);
                            sheet.Cells[item.Row + 1, 2].LoadFromText(item.Mounth);
                            sheet.Cells[item.Row + 1, 3].LoadFromText(item.Year);
                        }
                        else
                            //فيه مشكلة فى التاريخ بيبدل اليوم والشهر فى الاصلى مظبوط وبعد النسخ بتحصل المشكلة
                            sheet.Cells[item.Row + 1, item.Column + baseColIndex + 1].LoadFromText(item.Value);//1-base
                    }
                    package.Save();
                    //xlWorkbook.Save();
                }
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(workingPath);
                List<ImportedBankStatement> importedList = new List<ImportedBankStatement>();
                //using (ExcelPackage package = new ExcelPackage(fs))
                {
                    var sheet = xlWorkbook.Sheets[1]; // package.Workbook.Worksheets.First(); //
                    try
                    {  
                        FillList(importedList, sheet, template.StartRow);                      
                        header.Approved = false;
                        foreach (var item in importedList)
                        {
                            TblBankStatDetail detail = PrepareDetail(entities, header, item, errors);
                            if (detail != null)
                                header.TblBankStatDetails.Add(detail);
                        }

                        header.TblBank1 = null;
                        header.TblCurrency1 = null;

                        header.CreationDate = DateTime.Now;
                        header.LastChangeDate = DateTime.Now;
                        header.LastChangeUser = header.CreatedBy;
                        //header.Approved = true;
                        entities.TblBankStatHeaders.AddObject(header);
                        entities.SaveChanges();
                        return header.Iserial;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        var process = SharedOperation.GetExcelProcess(xlApp);
                        if (process != null)
                        {
                            process.Kill();
                        }
                    }
                }
            }
        }
        private static void FillList(List<ImportedBankStatement> importedList,Excel._Worksheet sheet , int startRow)// ExcelWorksheet sheet
        {
            int docDateIndex = 27;
            int transactionTypeIndex = 28;
            int descriptionIndex = 29;
            int amountIndex = 30;
            int chequeNoIndex = 31;
            int depositNoIndex = 32;
            for (int i = startRow; i < sheet.UsedRange.Rows.Count; i++)//  // sheet.Cells.End.Row
            {
                try
                {
                    var newemp = new ImportedBankStatement();
                    //sheet.Cells[i, docDateIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, docDateIndex]).Text))
                    //if (!string.IsNullOrWhiteSpace(sheet.Cells[i, docDateIndex].Text))
                    {
                        var docDate = ((Excel.Range)sheet.Cells[i, docDateIndex]).Text.ToUpper().Trim();
                        //var docDate = sheet.Cells[i, docDateIndex].Text.ToUpper().Trim();//.Replace("/", "").Replace("-", "");
                        //newemp.DocDate = DateTime.ParseExact(docDate, "dd-mm-yyyy",
                        //                System.Globalization.CultureInfo.InvariantCulture,
                        //                System.Globalization.DateTimeStyles.None);
                        if (((string)docDate).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Length == 3)
                        {
                            var y = ((Excel.Range)sheet.Cells[i, 3]).Text.ToUpper().Trim(); // sheet.Cells[i, 3].Text.ToUpper().Trim();
                            var m =  ((Excel.Range)sheet.Cells[i, 2]).Text.ToUpper().Trim(); // sheet.Cells[i, 2].Text.ToUpper().Trim();
                            var d = ((Excel.Range)sheet.Cells[i, 1]).Text.ToUpper().Trim(); // sheet.Cells[i, 1].Text.ToUpper().Trim();
                            newemp.DocDate = new DateTime(int.Parse(y), int.Parse(m), int.Parse(d));
                        }
                    }

                    //sheet.Cells[i, transactionTypeIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, transactionTypeIndex]).Text))
                    {
                        var transactionType = //sheet.Cells[i, transactionTypeIndex].Text.ToUpper().Trim();
                        ((Excel.Range)sheet.Cells[i, transactionTypeIndex]).Text.ToUpper().Trim();
                        newemp.TransactionType = transactionType;
                    }
                    //sheet.Cells[i, descriptionIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, descriptionIndex]).Text))
                    {
                        var description = //sheet.Cells[i, descriptionIndex].Text.ToUpper().Trim();
                        ((Excel.Range)sheet.Cells[i, descriptionIndex]).Text.ToUpper().Trim();
                        newemp.Description = description;
                    }
                    //sheet.Cells[i, amountIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, amountIndex]).Text))
                    {
                        var amount = //sheet.Cells[i, amountIndex].Text.ToUpper().Trim();
                        ((Excel.Range)sheet.Cells[i, amountIndex]).Text.ToUpper().Trim();
                        newemp.Amount = Convert.ToDecimal(amount);
                    }
                    //sheet.Cells[i, chequeNoIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, chequeNoIndex]).Text ))
                    {
                        var chequeNo = //sheet.Cells[i, chequeNoIndex].Text.ToUpper().Trim();
                        ((Excel.Range)sheet.Cells[i, chequeNoIndex]).Text.ToUpper().Trim();
                        if (chequeNo.Trim().Length > 0)
                            newemp.ChequeNo = Convert.ToInt64(chequeNo);
                    }
                    //sheet.Cells[i, depositNoIndex].Calculate();
                    if (!string.IsNullOrWhiteSpace(((Excel.Range)sheet.Cells[i, depositNoIndex]).Text))
                    {
                        var depositNo = //sheet.Cells[i, depositNoIndex].Text.ToUpper().Trim();
                        ((Excel.Range)sheet.Cells[i, depositNoIndex]).Text.ToUpper().Trim();
                        newemp.DepositNo = depositNo;
                    }
                    importedList.Add(newemp);
                }
                catch (Exception ex) { throw ex; }
            }
        }

    }
}