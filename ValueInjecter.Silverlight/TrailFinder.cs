using System;
using System.Collections.Generic;
using System.Reflection;

namespace Omu.ValueInjecter.Silverlight
{
    public static class TrailFinder
    {
        public static IEnumerable<IList<string>> GetTrails(string upn, PropertyInfo[] all, Func<Type, bool> f, StringComparison  comparison)
        {
            foreach (PropertyInfo p in all)
            {
                foreach (var s in GetTrails(upn, p, f, new List<string>(), comparison)) yield return s;
            }
        }

        public static IEnumerable<IList<string>> GetTrails(string upn, PropertyInfo prop, Func<Type, bool> f, IList<string> root, StringComparison comparison)
        {
            if (string.Equals(upn,prop.Name,comparison) && f(prop.PropertyType))
            {
                var l = new List<string> { prop.Name };
                yield return l;
                yield break;
            }

            if (upn.StartsWith(prop.Name, comparison))
            {
                root.Add(prop.Name);
                foreach (var p in prop.GetType().GetProps())
                {
                    foreach (var trail in GetTrails(upn.RemovePrefix(prop.Name, comparison), p, f, root, comparison))
                    {
                        var r = new List<string> { prop.Name };
                        r.AddRange(trail);
                        yield return r;
                    }
                }
            }
        }
    }
}