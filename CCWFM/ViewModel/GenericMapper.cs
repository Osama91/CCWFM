using System.Collections.Generic;
using System.Collections.ObjectModel;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel
{
    public static class GenericMapper
    {
        public static void InjectFromObCollection<TFrom, TTo>(ObservableCollection<TTo> to, params IEnumerable<TFrom>[] sources) where TTo : new()
        {
            foreach (var from in sources)
            {
                foreach (var source in from)
                {
                    var target = new TTo();
                    target.InjectFrom(source);
                    to.Add(target);
                }
            }
        }
    }
}