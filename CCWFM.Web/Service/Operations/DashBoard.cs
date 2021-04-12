using System;
using System.Collections.Generic;
using System.Linq;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        private void GetBrandSales()
        {
            using (var entity = new ccnewEntities())
            {
                entity.CommandTimeout = 0;

                try
                {
                    var brandcomparisonQuery = new List<string>();

                    try
                    {
                        if (entity.ExecuteStoreQuery<string>("select TOP 100 PERCENT(brand+ ' '+cast([%soldqty]as varchar)+ ' '+cast(salesactualtocostofgoodsoldratio as varchar)) as data from smsbrandcomparisoncurrentseason").ToList<string>().Any())
                        {
                            brandcomparisonQuery = entity.ExecuteStoreQuery<string>("select TOP 100 PERCENT(brand+ ' '+cast([%soldqty]as varchar)+ ' '+cast(salesactualtocostofgoodsoldratio as varchar)) as data from smsbrandcomparisoncurrentseason ").ToList<string>();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    var query = entity.ExecuteStoreQuery<string>("Select  * from smsdn").ToList<string>();

                    BrandSales = query;
                    Brandcomparison = brandcomparisonQuery;

                    NetSalesList = new List<GlPosting>();
                    NetSalesList = entity.ExecuteStoreQuery<GlPosting>("SELECT grname Brand,SUM(netsales) NetSales FROM [GlDashBoardNetSales] group by groupiserial,grcode,grname  having SUM(netsales)>0").ToList<GlPosting>();

                    CostOfGoodSoldList = new List<GlPosting>();
                    CostOfGoodSoldList = entity.ExecuteStoreQuery<GlPosting>("SELECT grname Brand,SUM(netsales) NetSales FROM [GlDashBoardNetSalesCost] group by groupiserial,grcode,grname  having SUM(netsales)>0").ToList<GlPosting>();
                }
                catch (Exception)
                {
                  //  GetBrandSales();
                }
            }
        }
    }
}