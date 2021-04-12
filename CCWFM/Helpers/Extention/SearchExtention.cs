using System;
using System.Collections.Generic;
using System.Linq;

namespace CCWFM.Helpers.Extention
{
    public static class SearchExtention
    {
        public static IEnumerable<SearchColumnModel> Search<TSource, SearchColumnModel>(this IEnumerable<TSource> source,
           Func<TSource, SearchColumnModel> selector)
        {
            return source.Select(selector).ToList();
        }
    }
}
