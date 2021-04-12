namespace Omu.ValueInjecter.Silverlight
{
    public abstract class LoopValueInjectionBase : ValueInjection
    {
        protected virtual bool AllowSetValue(object value)
        {
            return true;
        }
    }
}