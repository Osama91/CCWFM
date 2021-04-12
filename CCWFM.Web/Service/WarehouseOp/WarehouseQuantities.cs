using CCWFM.Web.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CCWFM.Web.Service
{
    public static class WarehouseQuantities
    {
        /// <summary>
        /// هيجيب الكمية المتاحة من اول صنف فى المخزن هلاقيه او هرجع صفر
        /// </summary>
        /// <param name="warehouseCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="colorCode"></param>
        /// <param name="size"></param>
        /// <param name="rollBatch"></param>
        /// <returns></returns>
        public static decimal GetAvilableQuantity(string warehouseCode,
            string itemCode, string colorCode, string size, string rollBatch)
        {
            var temp = GetInspectionsRoutes(warehouseCode, itemCode, colorCode, rollBatch);
            if (temp.Count() <= 0)// مفيش حاجة لاقاها
                return 0;
            return Convert.ToDecimal(temp.FirstOrDefault().RemainingMarkerRollQty);
        }
    
        public static List<InspectionsRoute> GetInspectionsRoutes(string warehouseCode, string itemCode, string colorCode, string rollBatch)
        {
            using (var entities = new WorkFlowManagerDBEntities())
            {
                var predicate = PredicateBuilder.True<InspectionsRoute>();

                predicate = predicate.And(i => i.FinishedWarehouse == warehouseCode);
                predicate = predicate.And(i => i.Fabric_Code == itemCode);
                if (!string.IsNullOrWhiteSpace(colorCode))
                    predicate = predicate.And(i => i.ColorCode == colorCode);
                //if (!string.IsNullOrWhiteSpace(size))
                //    predicate = predicate.And(i => i.size == colorCode);
                if (!string.IsNullOrWhiteSpace(rollBatch))
                    predicate = predicate.And(i => i.RollBatch == rollBatch);

                IQueryable<InspectionsRoute> query = entities.InspectionsRoutes.AsExpandable().Where(predicate);
                return query.ToList();
            }
        }

        /// <summary>
        /// Pages the specified query.
        /// </summary>
        /// <typeparam name="T">Generic Type Object</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The Object query where paging needs to be applied.</param>
        /// <param name="pageNum">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="orderByProperty">The order by property.</param>
        /// <param name="isAscendingOrder">if set to <c>true</c> [is ascending order].</param>
        /// <param name="rowsCount">The total rows count.</param>
        /// <returns></returns>
        private static IQueryable<T> PagedResult<T, TResult>(IQueryable<T> query, int pageNum, int pageSize,
                        Expression<Func<T, TResult>> orderByProperty, bool isAscendingOrder, out int rowsCount)
        {
            if (pageSize <= 0) pageSize = 20;

            //Total result count
            rowsCount = query.Count();

            //If page number should be > 0 else set to first page
            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * pageSize;

            query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            return query.Skip(excludedRows).Take(pageSize);
        }

        private static IQueryable<T> GetNextPage<T, TResult>(IQueryable<T> query, int currentCount, int pageSize,
                      Expression<Func<T, TResult>> orderByProperty, bool isAscendingOrder, out int rowsCount)
        {
            if (pageSize <= 0) pageSize = 20;
            
            //Total result count
            rowsCount = query.Count();
            
            query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            return query.Skip(currentCount).Take(pageSize);
        }

        public static List<T> GetLookupItems<T>(string query, Guid requestId, out Guid responseId)
        {
            responseId = requestId;
            var entity = new WorkFlowManagerDBEntities();

            return entity.CreateQuery<T>(query).ToList();
        }
    }
}