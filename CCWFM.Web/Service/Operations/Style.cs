using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;
using Omu.ValueInjecter;
using CCWFM.Web.Service.Operations;
using LinqKit;
using System.Linq.Dynamic;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        private List<TblStyle> GetTblStyle(int skip, int take, int salesOrderType, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount, out List<tbl_FabricAttriputes> mainFabricList,
            out Dictionary<int, int> salesOrdersPendingCount, out List<TblGROUP1> group1List
            , out List<TblGroup4> group4List, out List<TblGroup6> group6List, out List<TblGroup7> group7List,
            out List<TblGroup8> group8List, out Dictionary<int, bool> TransactionExist,
            out Dictionary<int, bool> RetialPoTransactionExist,out Dictionary<int?,string> LastTnaStatus,
            int userIserial, string StyleTnaOptions,
            string _brandsFilter,
            List<TblLkpBrandSection> _brandSectionFilter,
            List<TblLkpDirection> _directionFilter,
            List<TblStyleCategory> _styleCategoryFilter,
            List<TblFamily> _familyFilter,
            List<TblSubFamily> _subfamilyFilter
            )
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                IQueryable<TblStyle> query;
                if (filter != null)
                {
                    
                    // filter = SharedOperation.GetBrandPerUser(filter, valuesObjects, userIserial, context);
                    var parameterCollection = ConvertToParamters(valuesObjects);                                      
                    query =
                        context.TblStyles.Include("TblStyleFabricComposition1")
                            .Include("TblStyleCategory1")
                            .Include("TblLkpBrandSection1")
                            .Include("TblLkpSeason1")
                            .Include("TblStyleStatu")
                            .Include("TblSizeGroup1")
                            .Include("tbl_lkp_FabricDesignes1")
                            .Include("TblLkpDirection1")
                            .Include("TblSupplierFabric1")
                            .Include("TblFamily1")
                            .Include("TblStyleTNAHeaders")
                            .Include("TblStyleTNAHeaders.TblStyleTNAColorDetails")
                            .Include("TblSubFamily1")
                            .Include("TblSeasonalMasterLists")
                            .Include("TblStyleTNAHeaders.TblStyleTNAStatu")
                            .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                            .Include("TblSalesOrderColorTheme")
                            .Include("TblGenericFabric1")
                            .Where(filter, parameterCollection.ToArray())
                            .Where(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand && e.TblLkpBrandSection==x.TblLkpBrandSection));
                        //query= query.AsExpandable();

                    fullCount = query.Count();
                }
                else
                {
                    //var tblsalesorder =
                    //    context.TblSalesOrders.Where(
                    //        x => x.TblStyle1.Brand == "fz" && x.Status == 1 && x.SalesOrderType == 2).ToList();

                    //foreach (var VARIABLE in tblsalesorder)
                    //{
                    //    PostStyleToPo(VARIABLE.TblStyle, VARIABLE.Iserial, false, userIserial);
                    //}

                    fullCount =
                        context.TblStyles.Count(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand && e.TblLkpBrandSection == x.TblLkpBrandSection));
                    query = context.TblStyles.Include("TblStyleFabricComposition1")
                        .Include("TblStyleCategory1")
                        .Include("TblLkpBrandSection1")
                        .Include("TblLkpSeason1")
                        .Include("TblStyleStatu")
                        .Include("TblSizeGroup1")
                        .Include("tbl_lkp_FabricDesignes1")
                        .Include("TblLkpDirection1")
                        .Include("TblSupplierFabric1")
                        .Include("TblFamily1")
                        .Include("TblSubFamily1")
                        .Include("TblSeasonalMasterLists")
                        .Include("TblStyleTNAHeaders.TblStyleTNAStatu")
                        .Include("TblStyleTNAHeaders")
                        .Include("TblStyleTNAHeaders.TblStyleTNAColorDetails")
                        .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                        .Include("TblSalesOrderColorTheme")
                        .Include("TblGenericFabric1")
                        .Where(
                            x =>
                                x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                    e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand && e.TblLkpBrandSection == x.TblLkpBrandSection));
                    
                    //try
                    //{
                    //    var XX = query.Where(x=>x.Iserial == 35715).ToList().FirstOrDefault().TblStyleTNAHeaders.ToList().FirstOrDefault().TblStyleTNAColorDetails.ToList();
                    //}
                    //catch (Exception ex ) { throw new Exception(ex.Message); }

                }
                string TblStyleTNAStatus = "";

                if (valuesObjects != null)
                {
                    if (valuesObjects.Any(x => x.Key == "TblStyleTNAStatus0"))
                    {
                        TblStyleTNAStatus = valuesObjects.FirstOrDefault(x => x.Key == "TblStyleTNAStatus0").Value.ToString().ToLower();
                    }
                }
                if (!string.IsNullOrWhiteSpace(TblStyleTNAStatus))
                {
                    query = query.Where(x => x.TblStyleTNAHeaders.Any()&&
                    x.TblStyleTNAHeaders.OrderByDescending(w=>w.Iserial).FirstOrDefault().TblStyleTNAStatu.Ename.ToLower().Contains(TblStyleTNAStatus));
                }


                if (StyleTnaOptions == "RequireTna")
                {

                    query = query.AsExpandable()
                        .Where(e => e.Canceled != true && e.RequestTna == true);//context.TblSalesOrders.Select(s => s.TblStyleTNAHeader).Any(w => e.TblStyleTNAHeaders.Select(w => w.Iserial).Contains(w));
                }

                if (StyleTnaOptions == "TNARoute")
                {

                    query = query.AsExpandable()
                        .Where(e => e.Canceled != true && e.RequestTna == true) ;//context.TblSalesOrders.Select(s => s.TblStyleTNAHeader).Any(w => e.TblStyleTNAHeaders.Select(w => w.Iserial).Contains(w));
                }

                if (StyleTnaOptions == "RequirePo")
                {
                    //query = query.AsExpandable()
                    // .Where(e => e.TblStyleTNAHeaders.Any());
                    query = query.AsExpandable()
                        .Where(e => e.Canceled != true && e.TblStyleTNAHeaders.Any(t => (t.TblStyleTNAStatus == 0) && !t.TblSalesOrders.Any()));//context.TblSalesOrders.Select(s => s.TblStyleTNAHeader).Any(w => e.TblStyleTNAHeaders.Select(w => w.Iserial).Contains(w));
                }

                if (StyleTnaOptions == "RequirePoCost")
                {

                    query = query.AsExpandable()
                        .Where(e => e.Canceled != true && e.TblStyleTNAHeaders.Any(t => t.TblSalesOrders.Any(w => w.SalesOrderType == 1)
                           && t.TblSalesOrders.Where(w => w.SalesOrderType == 1 && w.Status == 0
                           ).Any()));
                }

                if (StyleTnaOptions == "RequireTnaModification")
                {
                    query = query.AsExpandable()
                   .Where(e => e.Canceled != true && e.TblStyleTNAHeaders.Any(t => (t.TblStyleTNAStatus == 2 || t.TblStyleTNAStatus == 3) && !t.TblSalesOrders.Any()));
                }
                if (StyleTnaOptions == "Modified")
                {
                    query = query.AsExpandable()
                   .Where(e => e.Canceled != true && e.TblStyleTNAHeaders.Any(t => (t.TblStyleTNAStatus == 4) && !t.TblSalesOrders.Any()));
                }

                if (StyleTnaOptions == "RequirePoApproval")
                {
                    query = query.AsExpandable()
                           .Where(e => e.Canceled != true && e.TblStyleTNAHeaders.Any(t => t.TblSalesOrders.Any(w => w.SalesOrderType == 1)
                              && t.TblSalesOrders.Where(w => w.SalesOrderType == 1 && w.Status == 5
                              ).Any()));
                }

                if (filter == null)
                {
                    //Brand
                    if (!string.IsNullOrEmpty(_brandsFilter))
                    {
                        query = query.Where(x => x.Brand == _brandsFilter);
                    }
                    //BrandSection
                    if (_brandSectionFilter != null && _brandSectionFilter.Count() > 0)
                    {
                        List<int> brandSectionFilterIserial =
                                                _brandSectionFilter.Select(x=>x.Iserial).ToList();

                         query = query.Where(x => brandSectionFilterIserial.Any(es=> es== x.TblLkpBrandSection));
                    }
                    //Direction
                     if (_directionFilter != null && _directionFilter.Count() > 0)
                    {
                        List<int> directionFilterIserial = _directionFilter.Select(x => x.Iserial).ToList();
                        query = query.Where(x => directionFilterIserial.Any(es => es == x.TblLkpDirection));

                    }
                    //Category
                    if (_styleCategoryFilter != null && _styleCategoryFilter.Count() > 0)
                    {
                        List<int> styleCategoryFilterIserial = _styleCategoryFilter.Select(x => x.Iserial).ToList();
                        query = query.Where(x => styleCategoryFilterIserial.Any(es => es == x.TblStyleCategory));
                    }
                    //Family
                    if (_familyFilter != null && _familyFilter.Count() > 0)
                    {
                        List<int> familyFilterIserial = _familyFilter.Select(x => x.Iserial).ToList();
                        query = query.Where(x => familyFilterIserial.Any(es => es == x.TblFamily));
                    }
                    //SubFamily
                    if (_subfamilyFilter != null && _subfamilyFilter.Count() > 0)
                    {
                        List<int> subfamilyFilterIserial = _subfamilyFilter.Select(x => x.Iserial).ToList();
                        query = query.Where(x => subfamilyFilterIserial.Any(es => es == x.TblSubFamily));
                    }
                }


                if (query != null)
                {
                    var   querylist = query.OrderByDescending(t => t.Iserial)
                                                .Skip(skip)
                                                .Take(take).ToList();

                    var listOfStyles = querylist.Select(x => x.Iserial);
                    salesOrdersPendingCount = context.TblSalesOrders.Where(
                        x =>
                            x.Status == 0 && x.SalesOrderType == salesOrderType &&
                            listOfStyles.Any(l => x.TblStyle == l))
                        .GroupBy(x => x.TblStyle).ToDictionary(t => t.Key, t => t.Count());
                    TransactionExist = context.TblSalesOrders.Where(x => listOfStyles.Any(l => x.TblStyle == l))
                        .GroupBy(x => x.TblStyle).ToDictionary(t => t.Key, t => true);

                    RetialPoTransactionExist = context.TblSalesOrders.Where(
                        x => listOfStyles.Any(l => x.TblStyle == l) && x.SalesOrderType == 1 && x.Status == 1)
                        .GroupBy(x => x.TblStyle).ToDictionary(t => t.Key, t => true);
                    LastTnaStatus =new Dictionary<int?, string>();
                    if (context.TblStyleTNAHeaders.OrderByDescending(w => w.Iserial).Any(w => listOfStyles.Any(l => w.TblStyle == l)))
                    {
                        LastTnaStatus = context.TblStyleTNAHeaders.Include("TblStyleTNAStatu").OrderByDescending(w => w.Iserial).Where(w => listOfStyles.Any(l => w.TblStyle == l)).GroupBy(w=>w.TblStyle).ToDictionary(m=>m.Key,m=>m.FirstOrDefault().TblStyleTNAStatu.Ename);

                    }
                    var listOfGroup1 = querylist.Select(x => x.TblGroup1).Where(x => x > 0).Distinct();

                    var listOfGroup6 = querylist.Select(x => x.TblGroup6).Where(x => x > 0).Distinct();
                    var listOfGroup7 = querylist.Select(x => x.TblGroup7).Where(x => x > 0).Distinct();

                    using (var entity = new ccnewEntities())
                    {
                        entity.TblGROUP1.MergeOption = MergeOption.NoTracking;
                        entity.TblGroup6.MergeOption = MergeOption.NoTracking;
                        entity.TblGroup7.MergeOption = MergeOption.NoTracking;

                        group1List = listOfGroup1.Any()
                            ? entity.TblGROUP1.Where(x => listOfGroup1.Any(l => x.ISERIAL == l)).ToList()
                            : null;
                        group4List = null;

                        group6List = listOfGroup6.Any()
                            ? entity.TblGroup6.Where(x => listOfGroup6.Any(l => x.ISERIAL == l)).ToList()
                            : null;
                        group7List = listOfGroup7.Any()
                            ? entity.TblGroup7.Where(x => listOfGroup7.Any(l => x.ISERIAL == l)).ToList()
                            : null;
                        group8List = null;
                    }
                    var listOfFabrics = querylist.Select(x => x.tbl_FabricAttriputes);
                    mainFabricList =
                        context.tbl_FabricAttriputes.Where(x => listOfFabrics.Any(l => x.Iserial == l)).ToList();

                    if (StyleTnaOptions == "TNARoute")
                    { 
                        var Data = (from p in querylist
                                     where !context.TblStyleTNARoutes.Any(sp => sp.TblStyle == p.Iserial)
                                     select p).ToList();
                        return Data;
                    }

                    return querylist.ToList();
                }
                else
                {
                    group1List = null;
                    group4List = null;
                    group6List = null;
                    group7List = null;
                    group8List = null;
                    mainFabricList = null;
                    salesOrdersPendingCount = null;
                    TransactionExist = null;
                    RetialPoTransactionExist = null;
                    LastTnaStatus = null;
                    return null;
                }
            }
        }

        [OperationContract]
        private List<TblStyleTNAColorDetail> GetTblStyleTNAColorDetail(int TblStyleTNAHeader)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyleTNAColorDetails.Where(x => x.TblStyleTNAHeader == TblStyleTNAHeader);
                return query.ToList();
            }
        }

        [OperationContract]
        private List<TblStyle> GetPendingStyle(int userIserial, int salesOrderType, int status = 0)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = context.TblStyles.Include("TblStyleFabricComposition1")
                    .Include("TblStyleCategory1")
                    .Include("TblLkpBrandSection1")
                    .Include("TblLkpSeason1")
                    .Include("TblStyleStatu")
                    .Include("TblSizeGroup1")
                    .Include("tbl_lkp_FabricDesignes1")
                    .Include("TblLkpDirection1")
                    .Include("TblSupplierFabric1")
                    .Include("TblFamily1")
                    .Include("TblSubFamily1")
                    .Include("TblLkpBrandSection1.TblBrandSectionPermissions.TblAuthPermission1")
                    .Include("TblGenericFabric1")
                    .Where(
                        x => x.TblSalesOrders.Any(w => w.Status == status && w.SalesOrderType == 1) &&
                             x.TblLkpBrandSection1.TblUserBrandSections.Any(
                                 e => e.TblAuthUser == userIserial && e.BrandCode == x.Brand)
                    );

                return query.ToList();
            }
        }

        public List<TblStyle> GetTblStyleFiltered(int skip, int take, string sort)
        {
            return null;
        }

        private string GetMaxStyle(string brand, int season, int family, int brandSection, int subfamily)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                int MasterSeasonIserial = 0;
               if (season > 39)
                {
                    //Hasehm 22-05-2019

                    string MasterSeasonCode = context.TblLkpSeasons.Where(x => x.Iserial == season).FirstOrDefault().SubSeasonCode;
                    MasterSeasonIserial = context.TblLkpSeasons.Where(x => x.SubSeasonCode == MasterSeasonCode && x.Iserial != season).FirstOrDefault().Iserial;
                }
                var checkFAmily = context.TblFamilies.First(x => x.Iserial == family).IncludeSub;
                var CheckSubFamilyLink = context.TblSubFamilies.FirstOrDefault(x => x.Iserial == subfamily).SubFamilyLink;
                var CheckParentSubFamilyLink = context.TblSubFamilies.FirstOrDefault(x => x.SubFamilyLink == subfamily);

                if (checkFAmily && (CheckSubFamilyLink != null || CheckParentSubFamilyLink != null))
                {
                    checkFAmily = true;
                }
                //Added By Hashem Coding By SubFamily Code
                else if (checkFAmily && CheckSubFamilyLink == null)
                {
                    checkFAmily = true;
                }
                else
                {
                    checkFAmily = false;
                }

                //var seasonRow = context.TblLkpSeasons.FirstOrDefault(x => x.Iserial == season).ShortCode;
                string serialNoString = "";
                try
                {
                    int? serialNo = 0;


                    if (checkFAmily)
                    {
                        var tempsub = context.TblStyles.Where(
                            x =>
                                x.Brand == brand && x.TblLkpBrandSection == brandSection && ( x.TblLkpSeason == season || x.TblLkpSeason== MasterSeasonIserial) &&
                                x.TblFamily == family && x.TblSubFamily == subfamily);
                        serialNo =
                           tempsub
                                .Select(x => x.SerialNo).Cast<int?>().Max();
                    }
                    else
                    {
                        var temp = context.TblStyles.Where(
                     x =>
                         x.Brand == brand && x.TblLkpBrandSection == brandSection && (x.TblLkpSeason == season || x.TblLkpSeason == MasterSeasonIserial) &&
                         x.TblFamily == family);
                        serialNo = temp
                                .Select(x => x.SerialNo).Cast<int?>().Max();
                    }

                    if (serialNo != null)
                    {
                        serialNo++;

                        var serial = (int)serialNo;
                        serialNoString = serial.ToString("000");
                    }
                    else
                    {
                        serialNoString = "001";
                    }
                }
                catch (Exception)
                {
                    serialNoString = "001";
                }

                return serialNoString;
            }
        }

        private string GetMaxStyleImported(string brand, int season, int family, int brandSection, int subfamily, List<TblStyle> Styles)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                if (season > 39)
                {
                    //Hasehm 22-05-2019
                    string MasterSeasonCode = context.TblLkpSeasons.Where(x => x.Iserial == season).FirstOrDefault().SubSeasonCode;
                    int MasterSeasonIserial = context.TblLkpSeasons.Where(x => x.Code == MasterSeasonCode).FirstOrDefault().Iserial;
                    season = MasterSeasonIserial;
                }
                var checkFAmily = context.TblFamilies.First(x => x.Iserial == family).IncludeSub;
                var CheckSubFamily = context.TblSubFamilies.FirstOrDefault(x => x.Iserial == subfamily).SubFamilyLink;
                if (CheckSubFamily == null)
                {
                    checkFAmily = false;
                }
                //var seasonRow = context.TblLkpSeasons.FirstOrDefault(x => x.Iserial == season).ShortCode;
                string serialNoString = "";
                try
                {
                    int? serialNo = 0;

                    if (checkFAmily)
                    {
                        var tempSub = Styles.Where(
                            x =>
                                x.Brand == brand && x.TblLkpBrandSection == brandSection && x.TblLkpSeason == season &&
                                x.TblFamily == family && x.TblSubFamily == subfamily).AsQueryable();
                        serialNo =
                            tempSub
                         .Select(x => NullableTryParseInt32FromStr(x.SerialNo)).Max();
                    }
                    else
                    {
                        var temp = Styles.Where(
                   x =>
                       x.Brand == brand && x.TblLkpBrandSection == brandSection && x.TblLkpSeason == season &&
                       x.TblFamily == family).AsQueryable();
                        serialNo =
                            temp.Select(x => NullableTryParseInt32FromStr(x.SerialNo)).Max();

                    }

                    serialNo++;

                    if (serialNo != null)
                    {
                        var serial = (int)serialNo;
                        serialNoString = serial.ToString("000");
                    }
                }
                catch (Exception)
                {
                    serialNoString = "001";
                }

                return serialNoString;
            }
        }

        [OperationContract]
        private byte[] GetStyleMainImage(int tblstyle)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var firstOrDefault = context.TblStyleImages.FirstOrDefault(x => x.TblStyle == tblstyle && x.DefaultImage);
                if (firstOrDefault != null)
                {
                    var image = firstOrDefault.ImagePathThumbnail;
                    return image;
                }
                return null;
            }
        }

        private void SaveStyle(TblStyle newRow, bool save, string UserName, WorkFlowManagerDBEntities context, bool Imported, string RepeatedSerial = "", string StyleTheme = "")
        {

            var chainrow = context.tblChainSetups.FirstOrDefault(x => x.sGlobalSettingCode == "StarWithBrandSection");
            var serial = "";
            if (string.IsNullOrEmpty(RepeatedSerial))
            {
                serial = GetMaxStyle(newRow.Brand, newRow.TblLkpSeason, newRow.TblFamily, newRow.TblLkpBrandSection, newRow.TblSubFamily);
            }
            if (!string.IsNullOrEmpty(serial))
            {
                if (Convert.ToInt32(serial) < 11)
                {
                    serial = "0" + context.TblFamilies.Where(x => x.Iserial == newRow.TblFamily).FirstOrDefault().StartIserial.ToString();
                }
            }
            if (!string.IsNullOrEmpty(RepeatedSerial))
            {
                if (RepeatedSerial.Length < 2)
                { serial = "00" + RepeatedSerial; }
                else { serial = "0" + RepeatedSerial; }
            }
            if (newRow.TblSupplierFabric == null || newRow.TblSupplierFabric == 0)
            {
                var defaultFabric = context.TblSupplierFabrics.FirstOrDefault(x => x.Code == "N/A").Iserial;

                newRow.TblSupplierFabric = defaultFabric;
            }
            if (newRow.tbl_FabricAttriputes == 0)
            {
                var defaultFabric = context.tbl_FabricAttriputes.FirstOrDefault(x => x.FabricID.StartsWith("N/A")).Iserial;

                newRow.tbl_FabricAttriputes = defaultFabric;
            }
            var tblLkpBrandSectionLink = context.TblLkpBrandSectionLinks.Include("TblLkpBrandSection1").FirstOrDefault(
                     x => x.TblLkpBrandSection == newRow.TblLkpBrandSection && newRow.Brand == x.TblBrand);
            var seasonShortCode = context.TblLkpSeasons.FirstOrDefault(x => x.Iserial == newRow.TblLkpSeason);
            var category = context.TblStyleCategories.FirstOrDefault(x => x.Iserial == newRow.TblStyleCategory);
            var familyShortCode = context.TblFamilies.FirstOrDefault(x => x.Iserial == newRow.TblFamily);
            var subFamilyCode = context.TblSubFamilies.FirstOrDefault(x => x.Iserial == newRow.TblSubFamily);
            var tbllkpdirection = context.TblLkpDirections.FirstOrDefault(x => x.Iserial == newRow.TblLkpDirection);


            if (save)
            {
                //Check Theme
                var CheckUseThemeCode = context.TblBrands.FirstOrDefault(x => x.Brand_Code == newRow.Brand && x.EnableTheme == true);
                if (CheckUseThemeCode != null)
                {
                    StyleTheme = context.TblSalesOrderColorThemes.FirstOrDefault(x => x.Iserial == newRow.tblTheme).Code;
                    if (!string.IsNullOrEmpty(StyleTheme))
                    {
                        if (StyleTheme.Length > 3)
                            StyleTheme = StyleTheme.Substring(0, 3);
                    }
                }

                if (Imported)
                {
                    //if (Convert.ToInt32(serial) > Convert.ToInt32(newRow.SerialNo))
                    //{
                    //    newRow.SerialNo =  (Convert.ToInt32(serial) + Convert.ToInt32(newRow.SerialNo)).ToString("000");
                    //}
                    newRow.SerialNo = serial;
                }
                else
                {
                    newRow.SerialNo = serial;
                }

                newRow.CreationDate = DateTime.Now;
                newRow.TNACreationDate = DateTime.Now;

                if (newRow.RefStyleCode == null || string.IsNullOrWhiteSpace(newRow.RefStyleCode))
                {
                    if (tblLkpBrandSectionLink != null && seasonShortCode != null && familyShortCode != null)
                    {
                        //var refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code+ seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code.PadLeft(2, '0') +
                        //                               newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;


                        var refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code.PadLeft(2, '0') +
                                                       newRow.SerialNo + tbllkpdirection.Code;
                        if (CheckUseThemeCode != null)
                        {
                            refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code.PadLeft(2, '0') +
                                                       newRow.SerialNo + StyleTheme;
                        }

                        if (chainrow.sSetupValue == "0")
                        {
                            //Commented ON 08-06-2020
                            //string tempCategoryDirection = "";
                            //if (tblLkpBrandSectionLink.UseCategory)
                            //{
                            //    tempCategoryDirection = category.Code;
                            //}
                            //if (tblLkpBrandSectionLink.UseDirection)
                            //{
                            //    tempCategoryDirection = tempCategoryDirection + tbllkpdirection.Code;
                            //}

                            //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code.PadLeft(2, '0') +
                            //                           newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;

                            refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code.PadLeft(2, '0') +
                                                      newRow.SerialNo + tbllkpdirection.Code;
                            if (CheckUseThemeCode != null)
                            {
                                refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                               seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code.PadLeft(2, '0') +
                                                     newRow.SerialNo + StyleTheme;
                            }
                        }

                        var CheckSubFamilyLink = context.TblSubFamilies.FirstOrDefault(x => x.Iserial == newRow.TblSubFamily).SubFamilyLink;
                        var CheckParentSubFamilyLink = context.TblSubFamilies.FirstOrDefault(x => x.SubFamilyLink == newRow.TblSubFamily);

                        if (familyShortCode.IncludeSub && (CheckSubFamilyLink != null || CheckParentSubFamilyLink != null))
                        {
                            if (subFamilyCode != null)
                            {

                                //&& subFamilyCode.SubFamilyLink != null)

                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code 
                                // + seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;
                                //
                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + 
                                //                      seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tbllkpdirection.Code;


                                refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                     seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                      newRow.SerialNo + tbllkpdirection.Code;
                                if (CheckUseThemeCode != null)
                                {
                                    refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                        seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                         newRow.SerialNo + StyleTheme;
                                }
                            }

                            if (chainrow.sSetupValue == "0")
                            {
                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;


                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                      newRow.SerialNo +  tbllkpdirection.Code;

                                refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                  seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                   newRow.SerialNo + tbllkpdirection.Code;
                                if (CheckUseThemeCode != null)
                                {
                                    refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                    seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                     newRow.SerialNo + StyleTheme;
                                }

                            }
                        }
                        //Added By Hashem To Code By SubFamily code Instead OF Family in ACCESSORIES CASE
                        if (familyShortCode.IncludeSub && CheckSubFamilyLink == null)
                        {
                            if (subFamilyCode != null)
                            {

                                //&& subFamilyCode.SubFamilyLink != null)

                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code 
                                // + seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;
                                //
                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + 
                                //                      seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tbllkpdirection.Code;


                                refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                     seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                      newRow.SerialNo + tbllkpdirection.Code;
                                if (CheckUseThemeCode != null)
                                {
                                    refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                        seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                         newRow.SerialNo + StyleTheme;
                                }
                            }

                            if (chainrow.sSetupValue == "0")
                            {
                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                       newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;


                                //refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.Code.PadLeft(3, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                                //                      newRow.SerialNo +  tbllkpdirection.Code;

                                refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                  seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                   newRow.SerialNo + tbllkpdirection.Code;
                                if (CheckUseThemeCode != null)
                                {
                                    refstylecode = tblLkpBrandSectionLink.TblBrand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code +
                                                    seasonShortCode.Code.PadLeft(3, '0') + subFamilyCode.Code.PadLeft(3, '0') +
                                                     newRow.SerialNo + StyleTheme;
                                }

                            }
                        }


                        newRow.RefStyleCode = refstylecode;
                        newRow.SupplierRef = newRow.Brand + tblLkpBrandSectionLink.TblLkpBrandSection1.Code + seasonShortCode.Code + tbllkpdirection.Code + subFamilyCode.Code.PadLeft(4, '0') + newRow.SerialNo;
                    }
                }

                newRow.StyleCode = newRow.RefStyleCode;
                context.TblStyles.AddObject(newRow);
                //  var subFamiliesBudle = context.TblSubFamilies.Where(w => w.SubFamilyLink == newRow.TblSubFamily).ToList();
                //foreach (var item in subFamiliesBudle)
                //{
                if (subFamilyCode.SubFamilyLink != null)
                {
                    var newstyle = new TblStyle();
                    newstyle = newRow.Clone();
                    newstyle.TblSubFamily = subFamilyCode.SubFamilyLink ?? 0;
                    newstyle.Iserial = 0;
                    newstyle.StyleCode = "";
                    newstyle.RefStyleCode = "";
                    SaveStyle(newstyle, true, UserName, context, false);

                }


            }
            else
            {
                var oldRow = (from e in context.TblStyles
                              where e.Iserial == newRow.Iserial
                              select e).SingleOrDefault();
                if (oldRow != null)
                {
                    //if (newRow.Brand == "DH" && !context.TblSalesOrders.Any(w=>w.TblStyle==newRow.Iserial))
                    //{
                    //    //newRow.RefStyleCode =
                    //    //                            seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code.PadLeft(2, '0') +
                    //    //                               newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;

                    //    if (familyShortCode.IncludeSub)
                    //    {
                    //        if (subFamilyCode != null)

                    //            newRow.RefStyleCode =
                    //                               seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                    //                                   newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;

                    //        if (chainrow.sSetupValue == "0")
                    //        {
                    //            newRow.RefStyleCode =
                    //                                seasonShortCode.SubSeasonCode.PadLeft(2, '0') + familyShortCode.Code + subFamilyCode.Code.PadLeft(2, '0') +
                    //                                   newRow.SerialNo + tblLkpBrandSectionLink.ShortCode + tbllkpdirection.Code;
                    //        }
                    //    }
                    //    //}
                    //    if (oldRow.TblSizeGroup != newRow.TblSizeGroup)
                    //    {
                    //        foreach (var seasonalMasterList in oldRow.TblSeasonalMasterLists)
                    //        {
                    //            foreach (var detail in seasonalMasterList.TblSeasonalMasterListDetails.ToList())
                    //            {
                    //                context.DeleteObject(detail);
                    //            }
                    //        }
                    //    }

                    //    if (oldRow.TblLkpSeason != newRow.TblLkpSeason)
                    //    {
                    //        newRow.SerialNo = serial;
                    //    }
                    //    else
                    //    {
                    //        newRow.SerialNo = oldRow.SerialNo;
                    //    }
                    //}
                    //if (newRow.tbl_FabricAttriputes == 0)
                    //{
                    //    var defaultFabric = context.tbl_FabricAttriputes.FirstOrDefault(x => x.FabricID == "N/A").Iserial;

                    //    newRow.tbl_FabricAttriputes = defaultFabric;
                    //}
                    //var changeditems = GenericUpdate(oldRow, newRow, context);

                    if (oldRow != null)
                    {
                        //if (oldRow.TblSizeGroup != newRow.TblSizeGroup)
                        //{
                        //    foreach (var seasonalMasterList in oldRow.TblSeasonalMasterLists)
                        //    {
                        //        foreach (var detail in seasonalMasterList.TblSeasonalMasterListDetails.ToList())
                        //        {
                        //            context.DeleteObject(detail);
                        //        }
                        //    }
                        //}

                        //if (newRow.Status == 100)
                        //{
                        //    var code = newRow.StyleCode;
                        //    if (newRow.StyleCode.Length == 11)
                        //    {
                        //        newRow.StyleCode = code + serial;
                        //        newRow.SerialNo = serial;
                        //    }
                        //}
                        oldRow.CCTargetCostPrice = newRow.CCTargetCostPrice;
                        newRow.CreationDate = oldRow.CreationDate;
                        oldRow.LastUpdatedDate = DateTime.Now;
                        oldRow.LastUpdatedBy = UserName;

                        oldRow.Status = 2;
                        context.SaveChanges();
                        //   newRow.StyleCode = newRow.RefStyleCode.Trim();
                        // GenericUpdate(oldRow, newRow, context);

                        //if (context.TblSalesOrders.Any(x => x.TblStyle == newRow.Iserial && x.SalesOrderType == 1 && x.Status == 0))
                        //{
                        //    var salesorders =
                        //        context.TblSalesOrders.Where(x => x.TblStyle == newRow.Iserial && x.SalesOrderType == 1 && x.Status == 0);

                        //    foreach (var row in salesorders)
                        //    {
                        //        var salesordersColors =
                        //         context.TblSalesOrderColors.Where(x => x.TblSalesOrder == row.Iserial).ToList();

                        //        //foreach (var colorrow in salesordersColors)
                        //        //{
                        //        //    colorrow.Cost = (decimal?)newRow.TargetCostPrice;
                        //        //    colorrow.LocalCost = (decimal?)newRow.TargetCostPrice;
                        //        //}
                        //    }
                        //}
                    }
                }
            }
        }

      

        [OperationContract]
        private TblStyle UpdateOrInsertTblStyle(TblStyle newRow, bool save, int index, out int outindex, string UserName,string RepeatedSerial = "",string StyleTheme = "")
        {
            outindex = index;
            using (var context = new WorkFlowManagerDBEntities())
            {
                if(!CheckCanAddStylesOnPDM(UserName))
                {
                    throw new Exception("Can't Save Style On PDM, Save On Stitch");
                }
              
                SaveStyle(newRow, save, UserName, context, false, RepeatedSerial, StyleTheme);

                context.SaveChanges();

                //var style =
                //    context.TblStyles
                //        .Include("TblLkpSeason1")
                //        .FirstOrDefault(x => x.Iserial == newRow.Iserial);
                //using (var entity = new ccnewEntities())
                //{
                //    var group4 = FindOrCreate("TblGroup4",
                //   new GenericTable
                //   {
                //       Iserial = 0,
                //       Aname = style.TblLkpSeason1.Aname,
                //       Code = style.TblLkpSeason1.Code,
                //       Ename = style.TblLkpSeason1.Ename
                //   });
                //    var query =
                //        "UPDATE  tblitem set AName='" + newRow.Description + "',EName='" + newRow.Description + "',SName='" + newRow.Description + "' ,TblGroup4='" + group4 + "' where style= '" + newRow.StyleCode + "'";
                //    entity.ExecuteStoreCommand(query);
                //}

                return newRow;
            }
        }

        private bool CheckCanAddStylesOnPDM(string userName)
        {
            using (WorkFlowManagerDBEntities db = new WorkFlowManagerDBEntities())
            {
                var user = db.TblAuthUsers.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
                var perm = db.TblAuthJobPermissions.FirstOrDefault(x => x.Tbljob == user.TblJob && x.TblAuthPermission.Code == "DisableStyleCodingOnPDM");
                if (perm != null)
                {
                    return false;
                } return true;

            }
        }

        [OperationContract]
        private string DeleteTblStyle(TblStyle row)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var oldRow = (from e in context.TblStyles
                              where e.Iserial == row.Iserial
                              select e).SingleOrDefault();

                if (context.TblSalesOrders.Any(x => x.TblStyle == oldRow.Iserial) || context.TblRequestForSamples.Any(x => x.TblStyle == oldRow.Iserial))
                {
                    throw new FaultException("Transaction Exists");
                }
                //Delete  tblStyleSpecDetails
                var oldRowStyleSpec = 
                              (from e in context.TblStyleSpecDetails
                              where e.TblStyle == row.Iserial
                              select e).SingleOrDefault();

                if (oldRowStyleSpec != null)
                {
                    var oldRowStyleSpecAttachment =
                              (from e in context.tblStyleSpecDetailAttachments
                               where e.tblStyleSpecDetails == oldRowStyleSpec.Iserial
                               select e);
                    foreach (var item in oldRowStyleSpecAttachment)
                    {
                        context.DeleteObject(item);
                    }
                    context.DeleteObject(oldRowStyleSpec);
                    context.SaveChanges();
                }

                if (oldRow != null) context.DeleteObject(oldRow);
                 context.SaveChanges();
            }
            return row.StyleCode;
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblStyle> SearchStyle(string style, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var query = (from x in context.TblStyles.Include("TblSizeGroup1").Include("TblFamily1").Include("TblSubFamily1").Include("TblLkpBrandSection1").Include("TblLkpSeason1.TblSeasonTracks.TblSeasonTrackType1")
                             where (x.StyleCode.StartsWith(style) || x.Description.StartsWith(style))
                          && x.TblLkpBrandSection1.TblUserBrandSections.Any(e => e.TblAuthUser == userIserial)
                             select x).Take(20).ToList();
                return query;
            }
        }

        [OperationContract]
        public TblChainSetting GetRetailTblChainSetting()
        {
            using (var context = new ccnewEntities())
            {
                return context.TblChainSettings.FirstOrDefault();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<TblGROUP1> GetRetailGroups(int skip, int take, string TableName, string sort, string filter,
            Dictionary<string, object> valuesObjects, out int fullCount)
        {
            using (var context = new ccnewEntities())
            {
                List<TblGROUP1> query = null;
                fullCount = 0;
                switch (TableName)
                {
                    case "TblGroup1":

                        if (filter != null)
                        {
                            var parameterCollection = ConvertToParamters(valuesObjects);

                            fullCount = context.TblGROUP1.Where(filter, parameterCollection.ToArray()).Count();
                            query = context.TblGROUP1.Where(filter, parameterCollection.ToArray()).OrderBy(sort).Skip(skip).Take(take).ToList();
                        }
                        else
                        {
                            fullCount = context.TblGROUP1.Count();
                            query = context.TblGROUP1.OrderBy(sort).Skip(skip).Take(take).ToList();
                        }
                        break;

                    case "TblGroup6":
                        if (filter != null)
                        {
                            var parameterCollection = ConvertToParamters(valuesObjects);

                            fullCount = context.TblGroup6.Where(filter, parameterCollection.ToArray()).Count();

                            var temp = context.TblGroup6.Where(filter, parameterCollection.ToArray())
                                .OrderBy(sort)
                                .Skip(skip)
                                .Take(take).ToList();

                            query =

                                     temp.Select(
                                         x =>
                                             new TblGROUP1
                                             {
                                                 ANAME = x.ANAME,
                                                 CODE = x.CODE,
                                                 ENAME = x.ENAME,
                                                 ISERIAL = x.ISERIAL
                                             }).ToList();
                        }
                        else
                        {
                            fullCount = context.TblGroup6.Count();

                            var temp = context.TblGroup6.OrderBy(sort).Skip(skip).Take(take).ToList();
                            query =

                                 temp.Select(
                                     x =>
                                         new TblGROUP1
                                         {
                                             ANAME = x.ANAME,
                                             CODE = x.CODE,
                                             ENAME = x.ENAME,
                                             ISERIAL = x.ISERIAL
                                         }).ToList();
                        }
                        break;

                    case "TblGroup7":
                        if (filter != null)
                        {
                            var parameterCollection = ConvertToParamters(valuesObjects);

                            fullCount = context.TblGroup7.Where(filter, parameterCollection.ToArray()).Count();

                            var temp = context.TblGroup7.Where(filter, parameterCollection.ToArray())
                                .OrderBy(sort)
                                .Skip(skip)
                                .Take(take).ToList();
                            query =

                                 temp.Select(
                                     x =>
                                         new TblGROUP1
                                         {
                                             ANAME = x.ANAME,
                                             CODE = x.CODE,
                                             ENAME = x.ENAME,
                                             ISERIAL = x.ISERIAL
                                         }).ToList();
                        }
                        else
                        {
                            fullCount = context.TblGroup7.Count();

                            var temp = context.TblGroup7.OrderBy(sort)
                                .Skip(skip)
                               .Take(take).ToList();

                            query =

                                 temp.Select(
                                     x =>
                                         new TblGROUP1
                                         {
                                             ANAME = x.ANAME,
                                             CODE = x.CODE,
                                             ENAME = x.ENAME,
                                             ISERIAL = x.ISERIAL
                                         }).ToList();
                        }
                        break;

                    case "TblGroup8":
                        if (filter != null)
                        {
                            var parameterCollection = ConvertToParamters(valuesObjects);

                            fullCount = context.TblGroup8.Where(filter, parameterCollection.ToArray()).Count();

                            var temp = context.TblGroup8.Where(filter, parameterCollection.ToArray())
                                .OrderBy(sort)
                                .Skip(skip)
                                .Take(take).ToList();

                            query =

                                 temp.Select(
                                     x =>
                                         new TblGROUP1
                                         {
                                             ANAME = x.ANAME,
                                             CODE = x.CODE,
                                             ENAME = x.ENAME,
                                             ISERIAL = x.ISERIAL
                                         }).ToList();
                        }
                        else
                        {
                            fullCount = context.TblGroup8.Count();
                            var temp = context.TblGroup8
                              .OrderBy(sort)
                              .Skip(skip)
                              .Take(take).ToList();

                            query =

                                 temp.Select(
                                     x =>
                                         new TblGROUP1
                                         {
                                             ANAME = x.ANAME,
                                             CODE = x.CODE,
                                             ENAME = x.ENAME,
                                             ISERIAL = x.ISERIAL
                                         }).ToList();
                        }
                        break;
                }
                return query;
            }
        }

        [OperationContract]
        public List<TblGROUP1> Group1Search(string code)
        {
            using (var context = new ccnewEntities())
            {
                return
                    context.TblGROUP1.Where(
                        x => x.CODE.StartsWith(code) || x.ANAME.StartsWith(code) || x.ENAME.StartsWith(code)).Take(30)
                        .ToList();
            }
        }

        [OperationContract]
        public List<TblGroup4> Group4Search(string code)
        {
            using (var context = new ccnewEntities())
            {
                return
                    context.TblGroup4.Where(
                        x => x.CODE.StartsWith(code) || x.ANAME.StartsWith(code) || x.ENAME.StartsWith(code)).Take(30)
                        .ToList();
            }
        }

        [OperationContract]
        public List<TblGroup6> Group6Search(string code)
        {
            using (var context = new ccnewEntities())
            {
                return
                    context.TblGroup6.Where(
                        x => x.CODE.StartsWith(code) || x.ANAME.StartsWith(code) || x.ENAME.StartsWith(code)).Take(30)
                        .ToList();
            }
        }

        [OperationContract]
        public List<TblGroup7> Group7Search(string code)
        {
            using (var context = new ccnewEntities())
            {
                return
                    context.TblGroup7.Where(
                        x => x.CODE.StartsWith(code) || x.ANAME.StartsWith(code) || x.ENAME.StartsWith(code)).Take(30)
                        .ToList();
            }
        }

        [OperationContract]
        public List<TblGroup8> Group8Search(string code)
        {
            using (var context = new ccnewEntities())
            {
                return
                    context.TblGroup8.Where(
                        x => x.CODE.StartsWith(code) || x.ANAME.StartsWith(code) || x.ENAME.StartsWith(code)).Take(30)
                        .ToList();
            }
        }

        [OperationContract]
        public void DeleteRetailStyle(string code)
        {
            using (var context = new ccnewEntities())
            {
                context.DeleteStyle(code);
            }
        }

        public void generateSalesOrderColorFromMasterList(WorkFlowManagerDBEntities context, TblStyleTNAHeader StyleTna)
        {
            var brand = context.Brands.FirstOrDefault(w => w.Brand_Code == StyleTna.TblStyle1.Brand);
            var user = context.TblAuthUsers.FirstOrDefault(w => w.Iserial == StyleTna.CreatedBy).UserName;
            var style = context.TblStyles.FirstOrDefault(w => w.Iserial == StyleTna.TblStyle);

            var ListOfDeliveries = StyleTna.TblStyleTNAColorDetails.GroupBy(w => new { w.DeliveryDate, w.AccCost, w.FabricCost, w.OperationCost }).Select(w => w.Key);
            int index = 0;

            if (ListOfDeliveries.Any())
            {

                foreach (var item in ListOfDeliveries)
                {
                    var salesorderRow = new TblSalesOrder()
                    {
                        TblStyle = StyleTna.TblStyle ?? 0,
                        AccCost = item.AccCost,
                        FabricCost = item.FabricCost,
                        OperationCost = item.OperationCost,
                        TblRetailOrderProductionType = StyleTna.TblRetailOrderProductionType,
                        Customer = brand.Brand_Code,
                        SalesOrderType = 1,
                        TblSupplier = StyleTna.TblSupplier,
                        DeliveryDate = item.DeliveryDate,
                        CreatedBy = user,
                        TblStyleTNAHeader = StyleTna.Iserial,
                        Status = 5,
                        IsPlannedOrder = true,
                        Iserial=0,
                    };
                  
                    salesorderRow.TblApprovals = new EntityCollection<TblApproval>();
                    salesorderRow.TblApprovals.Add(new TblApproval()
                    {
                        TblAuthUser = StyleTna.CreatedBy,
                        ApprovalDate = DateTime.Now,
                        ApprovalType = 1,
                        ApprovedStatus = 5
                    });
                    foreach (var row in StyleTna.TblStyleTNAColorDetails.Where(w => w.DeliveryDate == item.DeliveryDate && w.AccCost == item.AccCost && w.FabricCost == item.FabricCost && w.OperationCost == item.OperationCost))
                    {
                        var seasonalmasterlist = context.TblSeasonalMasterLists.Include(nameof(TblSeasonalMasterList.TblSeasonalMasterListDetails)).FirstOrDefault(w => w.TblStyle== StyleTna.TblStyle && w.TblColor == row.TblColor);

                        decimal cost = (decimal)row.Cost;
                        if (row.TblColor != null)
                        {
                            var newrow = new TblSalesOrderColor
                            {
                                TblColor = (int)row.TblColor,
                                TblSalesOrderColorTheme = seasonalmasterlist.TblSalesOrderColorTheme,
                                ManualCalculation = seasonalmasterlist.ManualCalculation,
                                
                            };
                            newrow.TblSalesOrderSizeRatios = new EntityCollection<TblSalesOrderSizeRatio>();
                            foreach (var VARIABLE in seasonalmasterlist.TblSeasonalMasterListDetails)
                            {
                                var newsalesordersizeratio = new TblSalesOrderSizeRatio();
                                newsalesordersizeratio.InjectFrom(VARIABLE);
                                newsalesordersizeratio.Iserial = 0;
                                newsalesordersizeratio.EntityKey = null;
                                newsalesordersizeratio.TblSalesOrderColor1 = null;
                                newrow.TblSalesOrderSizeRatios.Add(newsalesordersizeratio);
                            }
                            newrow.Cost = cost;
                            if (style.TargetCostPrice<1)
                            {
                                style.TargetCostPrice = (double)cost;
                            }
                          
                            newrow.ExchangeRate = row.ExchangeRate;
                            newrow.AdditionalCost = row.AdditionalCost;
                            newrow.LocalCost = row.LocalCost;
                            newrow.TblCurrency = row.TblCurrency;
                            newrow.Total = seasonalmasterlist.Qty;
                            newrow.TblSalesOrderColorTheme = seasonalmasterlist.TblSalesOrderColorTheme;
                            newrow.DeliveryDate = seasonalmasterlist.DelivaryDate;
                            salesorderRow.TblSalesOrderColors.Add(newrow);
                       
                        }
                    }
                    UpdateOrInsertTblSalesOrder(salesorderRow, true, 0, out index);
                }
            }
            else
            {
                var list = GetSeasonalMasterListNotLinkedToSalesorderByStyle(StyleTna.TblStyle ?? 0, 1);
                var salesorderRow = new TblSalesOrder()
                {
                    TblStyle = StyleTna.TblStyle ?? 0,
                    AccCost = StyleTna.AccCost,
                    FabricCost = StyleTna.FabricCost,
                    OperationCost = StyleTna.OperationCost,
                    TblRetailOrderProductionType = StyleTna.TblRetailOrderProductionType,
                    Customer = brand.Brand_Code,
                    SalesOrderType = 1,
                    TblSupplier = StyleTna.TblSupplier,
                    DeliveryDate = StyleTna.DeliveryDate,
                    CreatedBy = user,
                    TblStyleTNAHeader = StyleTna.Iserial,
                    Status = 5,
                    IsPlannedOrder = true,
                };
               
                generateSalesOrderColorFromMasterList(list, context, salesorderRow, style);
            }


        }
        public void generateSalesOrderColorFromMasterList(List<TblSeasonalMasterList> list, WorkFlowManagerDBEntities context, TblSalesOrder SalesOrder, TblStyle Style)
        {
            int index = 0;
            decimal cost = (decimal)Style.TargetCostPrice;
            var currency = context.TblCurrencies.First(x => x.LocalCurrency == true).Iserial;
          

            var TblStyleTNAHeader = new TblStyleTNAHeader();
            if (SalesOrder.TblStyleTNAHeader!=null)
            {
                TblStyleTNAHeader = context.TblStyleTNAHeaders.Include("TblStyleTNAColorDetails").FirstOrDefault(w => w.Iserial == SalesOrder.TblStyleTNAHeader);
            }

            if (SalesOrder.TblStyleTNAHeader != null)
            {
                cost = (decimal)TblStyleTNAHeader.TargetCostPrice;
                currency = TblStyleTNAHeader.TblCurrency??0;
            }

            if (Style.TargetCostPrice < 1)
            {
                Style.TargetCostPrice = (double)cost;
            }

            //Style.TargetCostPrice = (double)cost;
            foreach (var row in list)
            {

                if (row.TblColor != null)
                {
                    if (SalesOrder.TblStyleTNAHeader != null)
                    {

                        if (TblStyleTNAHeader.TblStyleTNAColorDetails.Any(w => w.TblColor == row.TblColor))
                        {
                            cost = (decimal)TblStyleTNAHeader.TblStyleTNAColorDetails.FirstOrDefault(w => w.TblColor == row.TblColor).Cost;
                        }
                    }
                    var newrow = new TblSalesOrderColor
                    {
                        TblColor = (int)row.TblColor,
                        TblSalesOrderColorTheme = row.TblSalesOrderColorTheme,
                        ManualCalculation = row.ManualCalculation,
                    };
                    newrow.TblSalesOrderSizeRatios = new EntityCollection<TblSalesOrderSizeRatio>();
                    foreach (var VARIABLE in row.TblSeasonalMasterListDetails)
                    {
                        var newsalesordersizeratio = new TblSalesOrderSizeRatio();
                        newsalesordersizeratio.InjectFrom(VARIABLE);
                        newsalesordersizeratio.Iserial = 0;
                        newsalesordersizeratio.EntityKey = null;
                        newsalesordersizeratio.TblSalesOrderColor1 = null;
                        newrow.TblSalesOrderSizeRatios.Add(newsalesordersizeratio);
                    }
                    newrow.Cost = cost;
                    newrow.ExchangeRate = 1;
                    newrow.AdditionalCost = 0;
                    newrow.LocalCost = cost;
                    newrow.TblCurrency = currency;
                    newrow.Total = row.Qty;
                    newrow.TblSalesOrderColorTheme = row.TblSalesOrderColorTheme;
                    newrow.DeliveryDate = row.DelivaryDate;
                    SalesOrder.TblSalesOrderColors.Add(newrow);
                  
                }
            }
            if (SalesOrder.TblStyleTNAHeader != null)
            {
                UpdateOrInsertTblSalesOrder(SalesOrder, true, 0, out index);
            }
           
        }

        [OperationContract]
        public void InsertImportedSalesOrder(List<TblSalesOrder> SalesOrder, string username)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                foreach (var variable in SalesOrder)
                {
                    var selectedMainRow = context.TblStyles.FirstOrDefault(x => x.StyleCode == variable.TblStyle1.StyleCode);
                    var list = GetSeasonalMasterListNotLinkedToSalesorderByStyle(selectedMainRow.Iserial, variable.SalesOrderType);
                    generateSalesOrderColorFromMasterList(list, context, variable, selectedMainRow);
                    using (var entity = new ccnewEntities())
                    {
                        var supplier = entity.TBLsuppliers.FirstOrDefault(x => x.Code == variable.SalesOrderCode).Iserial;
                        variable.TblSupplier = supplier;
                    }
                    variable.SalesOrderCode = selectedMainRow.StyleCode + "-001";

                    variable.TblStyle1 = null;
                    variable.TblStyle = selectedMainRow.Iserial;
                    context.TblSalesOrders.AddObject(variable);
                }
                context.SaveChanges();
            }
        }

        [OperationContract]
        public void InsertImportedStyles(List<TblStyle> styleList, List<TblSeasonalMasterList> seasonalMasterList, string username)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var listofsizegroups = new List<int>();
                var listofSizes = new List<TblSize>();
                CCWFM.Web.Service.LkpData.LkpData lookupService = new LkpData.LkpData();
                
                //var families = lookupService.GetTblFamilyLink(styleList.FirstOrDefault().Brand, styleList.FirstOrDefault().TblLkpBrandSection);
                var families = lookupService.GetTblFamilyCategoryLink(styleList.FirstOrDefault().Brand, styleList.FirstOrDefault().TblLkpBrandSection);
                foreach (var variable in styleList)
                {
                    if (!listofsizegroups.Contains(variable.TblSizeGroup))
                    {
                        listofSizes.AddRange(GetSizesBySizeGroup(0, 50, variable.TblSizeGroup));

                    }


                    try
                    {
                        variable.TblStyleCategory =
                          context.TblStyleCategories.FirstOrDefault(x => x.Ename == variable.TblStyleCategory1.Ename).Iserial;

                    }
                    catch (Exception)
                    {
                        throw new FaultException(variable.TblStyleCategory1.Ename + " Category Doesn't Exist");
                    }

                    try
                    {
                        var group1 = Group1Search(variable.TblStyleCategory1.Aname).FirstOrDefault().ISERIAL;

                        variable.TblGroup1 = group1;
                    }
                    catch (Exception)
                    {
                        throw new FaultException(variable.TblStyleCategory1.Aname + " Source Doesn't Exist");
                    }

                   
                  
                    variable.TblStyleCategory1 = null;

                    variable.TblLkpDirection =
                     context.TblLkpDirections.FirstOrDefault(x => x.Ename == variable.TblLkpDirection1.Ename).Iserial;
                    variable.TblLkpDirection1 = null;
                    try
                    {
                        variable.TblFamily =
                                         families.FirstOrDefault(x => x.TblFamily1.Ename.ToUpper().Trim() == variable.TblFamily1.Ename.ToUpper()).TblFamily;
                        variable.TblFamily1 = null;
                    }
                    catch (Exception)
                    {
                        throw new FaultException(variable.TblFamily1.Ename + " Family Doesn't Exist");
                    }

                    try
                    {
                        
                        //var subfamilies = lookupService.GetTblSubFamilyLink(variable.TblFamily, variable.Brand,
                        //    variable.TblLkpBrandSection);

                        var subfamilies = lookupService.GetTblSubFamilyCategoryLink(variable.TblFamily, variable.Brand,
                            variable.TblLkpBrandSection);
                        variable.TblSubFamily =
                            subfamilies.FirstOrDefault(
                                x => x.TblSubFamily1.Ename.ToUpper().Trim() == variable.TblSubFamily1.Ename.ToUpper())
                                .TblSubFamily;
                        variable.TblSubFamily1 = null;
                    }
                    catch (Exception)
                    {
                        throw new FaultException(variable.TblSubFamily1.Ename + " Sub Family Doesn't Exist Or Not Linked To The Family");
                    }

                    foreach (var salesOrder in variable.TblSalesOrders)
                    {
                        using (var entity = new ccnewEntities())
                        {

                            salesOrder.TblSupplier = entity.TBLsuppliers.FirstOrDefault(x => x.Code == salesOrder.AdditionalNotes).Iserial;
                            salesOrder.AdditionalNotes = "";
                        }

                    }
                    foreach (var seasonalmasterlist in variable.TblSeasonalMasterLists.ToList())
                    {
                        var entitiycollections = new EntityCollection<TblSeasonalMasterListDetail>();

                        var ratios = seasonalmasterlist.TblSalesOrderColorTheme1.Aname.Split(',');
                        var ratiosDouble = new List<double>();

                        foreach (var ratiodouble in ratios)
                        {
                            ratiosDouble.Add(Convert.ToDouble(ratiodouble));
                        }
                        var temp = listofSizes.Where(x => x.TblSizeGroup == variable.TblSizeGroup).OrderBy(x => x.Id).ToList();
                        var sum = ratiosDouble.Sum();
                        foreach (var size in temp.OrderBy(x => x.Id))
                        {
                            double ratio = 0;
                            int productionPerSize = 0;
                            try
                            {
                                ratio = Convert.ToDouble(ratios.ElementAt(temp.IndexOf(size)));
                                productionPerSize = Convert.ToInt32(seasonalmasterlist.Qty * (ratio / sum));
                            }
                            catch (Exception)
                            {
                                ratio = 0;
                                productionPerSize = 0;
                            }
                            var seasonalmasterlistdetailrow = new TblSeasonalMasterListDetail();
                            seasonalmasterlistdetailrow.Size = size.SizeCode;
                            seasonalmasterlistdetailrow.Ratio = ratio;
                            seasonalmasterlistdetailrow.ProductionPerSize = productionPerSize;

                            entitiycollections.Add(seasonalmasterlistdetailrow);
                        }
                        seasonalmasterlist.TblSeasonalMasterListDetails = entitiycollections;

                        var theme = context.TblSalesOrderColorThemes.FirstOrDefault(
                                x =>
                                    x.TblLkpSeason == variable.TblLkpSeason && x.TblBrand == variable.Brand &&
                                    x.TblLkpBrandSection == variable.TblLkpBrandSection &&
                                    x.Ename == seasonalmasterlist.TblSalesOrderColorTheme1.Ename);
                        if (theme != null)
                        {
                            seasonalmasterlist.TblSalesOrderColorTheme = theme.Iserial;

                            seasonalmasterlist.DelivaryDate = theme.DeliveryDate;
                        }
                        else
                        {
                            throw new FaultException(seasonalmasterlist.TblSalesOrderColorTheme1.Ename + " Theme Doesn't Exist");
                        }

                        listofsizegroups.Add(variable.TblSizeGroup);

                        seasonalmasterlist.TblSalesOrderColorTheme1 = null;
                        try
                        {
                            seasonalmasterlist.TblColor =
                                                   context.TblColors.FirstOrDefault(x => x.Code == seasonalmasterlist.TblColor1.Code && x.TblLkpColorGroup != 24).Iserial;
                        }
                        catch (Exception)
                        {
                            throw new FaultException(seasonalmasterlist.TblColor1.Code + " Color Doesn't Exist");
                        }
                        seasonalmasterlist.TblColor1 = null;
                    }
                    variable.SerialNo = "0";
                    variable.SerialNo = GetMaxStyleImported(variable.Brand, variable.TblLkpSeason, variable.TblFamily, variable.TblLkpBrandSection, variable.TblSubFamily, styleList);
                    if(string.IsNullOrEmpty(variable.SerialNo))
                        variable.SerialNo = "0";
                    {
                        try
                        {
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw new FaultException("Saved Until Style "+variable.StyleCode + "Error : " + ex.Message);
                        }

                    }
                }
               //context.SaveChanges();
            }
        }
    }
}