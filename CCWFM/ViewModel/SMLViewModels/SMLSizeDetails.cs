using System.ComponentModel.DataAnnotations;
using CCWFM.Helpers.LocalizationHelpers;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class SmlSizeDetails : ViewModelBase
    {
        #region [ Events ]

        #endregion [ Events ]

        #region [ Fields ]

        private int _Iserial;
        private object _parentObject;

        private int _parentSerial;

        private string _sizeCode;
        private decimal _sizeRatio;
        #endregion [ Fields ]

        #region [ Properties ]

        public int Iserial
        {
            get { return _Iserial; }
            set
            {
                _Iserial = value;
                RaisePropertyChanged("Iserial");
            }
        }

        public object ParentObject
        {
            get { return _parentObject; }
            set { _parentObject = value; RaisePropertyChanged("ParentObject"); }
        }

        public int ParentSerial
        {
            get { return _parentSerial; }
            set
            {
                _parentSerial = value;
                RaisePropertyChanged("ParentSerial");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Sizes")]
        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                _sizeCode = value;
                RaisePropertyChanged("SizeCode");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Ratio")]
        public decimal SizeRatio
        {
            get { return _sizeRatio; }
            set
            {
                _sizeRatio = value;
                RaisePropertyChanged("SizeRatio");
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        #endregion [ Constructors ]

        #region [ Commands ]

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        #endregion [ Commands bound methods ]
    }
}