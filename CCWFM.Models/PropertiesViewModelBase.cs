using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CCWFM.Web.DataLayer
{
    [DataContract]
    public class PropertiesViewModelBase : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;
            if (propertyName.StartsWith("StyleC"))
            {
            }
            try
            {
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception)
            {


            }

        }

        #endregion Implement INotifyPropertyChanged
    }
}
