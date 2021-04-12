using System;
using System.Collections.Generic;
using System.Reflection;

namespace Omu.ValueInjecter.Silverlight
{
    /// <summary>
    /// this is for caching the PropertyInfo[] of each Type
    /// </summary>
    public static class PropertyInfosStorage
    {
        private static readonly Dictionary<Type, PropertyInfo[]> Storage = new Dictionary<Type, PropertyInfo[]>();
        private static readonly object LockObj = new object();

        public static PropertyInfo[] GetProps(Type type)
        {
            if (!Storage.ContainsKey(type))
            {
                lock (LockObj)
                {
                    if (!Storage.ContainsKey(type))
                    {
                        var props = type.GetProperties();
                        Storage.Add(type, props);
                    }
                }
            }

            return Storage[type];
        }

        public static PropertyInfo[] GetProps(this object o)
        {
            if (o!=null)
            {
                return GetProps(o.GetType());
            }
            else
            {
                return null;
            }
           
        }
    }
}