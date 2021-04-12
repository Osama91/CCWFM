using System;
using System.Linq;
using System.Reflection;

namespace Omu.ValueInjecter.Silverlight
{
    public static class DescriptorTools
    {
        /// <summary>
        /// Seek for a PropertyInfo within the collection by Name
        /// </summary>
        /// <returns>the search result or null if nothing was found</returns>
        public static PropertyInfo GetByName(this PropertyInfo[] collection, string name)
        {
             return collection.FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Seek for a PropertyInfo within the collection by Name with option to ignore case
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyInfo GetByName(this PropertyInfo[] collection,
                                                   string name, bool ignoreCase)
        {
            return collection.FirstOrDefault(t => string.Compare(t.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Search for a PropertyInfo within the collection that is of a specific type T
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyInfo GetByNameType<T>(this PropertyInfo[] collection, 
            string name)
        {
            var p = collection.GetByName(name);
            if (p != null && p.PropertyType == typeof (T)) return p;
            return null;
        }
    }
}