using System;

namespace Omu.ValueInjecter.Silverlight
{
    internal class SameNameType : ValueInjection
    {
        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            for (var i = 0; i < sourceProps.Length; i++)
            {
                var s = sourceProps[i];

                var t = target.GetProps().GetByName(s.Name);
                if (t == null) continue;

                if (t.PropertyType==s.PropertyType ||  (Nullable.GetUnderlyingType(t.PropertyType) != null) || (Nullable.GetUnderlyingType(s.PropertyType) != null))
                {
                    t.SetValue(target, s.GetValue(source));
                }
    
            }
        }
    }
}