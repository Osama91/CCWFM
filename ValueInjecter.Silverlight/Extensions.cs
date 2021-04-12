using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Omu.ValueInjecter.Silverlight
{
    public static class Extensions
    {
        public static IEnumerable<string> SplitToWords(this string value)
        {
            return from object match
                       in Regex.Matches(value, "[A-Z][a-z0-9]+")
                   select match.ToString();
        }

        public static string RemovePrefix(this string o, string prefix)
        {
            return o.RemovePrefix(prefix, StringComparison.Ordinal);
        }

        public static string RemovePrefix(this string o, string prefix, StringComparison comparison)
        {
            if (prefix == null) return o;
            return !o.StartsWith(prefix, comparison) ? o : o.Remove(0, prefix.Length);
        }

        public static void SetValue(this PropertyInfo propertyInfo, object where, object value)
        {
            try
            {
                propertyInfo.SetValue(where, value, null);
            }
            catch (Exception)
            {

                
            }
        
        }

        public static object GetValue(this PropertyInfo propertyInfo, object from)
        {
            return propertyInfo.GetValue(from, null);
        }
    }
}