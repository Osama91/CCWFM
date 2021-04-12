namespace Omu.ValueInjecter.Silverlight
{
    public interface IValueInjection
    {
        object Map(object source, object target);
    }
}