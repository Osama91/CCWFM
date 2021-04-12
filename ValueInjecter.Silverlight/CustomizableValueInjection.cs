using System;

namespace Omu.ValueInjecter.Silverlight
{
    public abstract class CustomizableValueInjection : PrefixedValueInjection
    {
        protected virtual bool TypesMatch(Type sourceType, Type targetType)
        {
            return targetType == sourceType;
        }

        protected virtual object SetValue(object sourcePropertyValue)
        {
            return sourcePropertyValue;
        }
    }
}