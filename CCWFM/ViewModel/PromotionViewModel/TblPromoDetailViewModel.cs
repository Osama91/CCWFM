using System;
using System.ComponentModel.DataAnnotations;
using CCWFM.Helpers.LocalizationHelpers;

namespace CCWFM.ViewModel.PromotionViewModel
{
    public class TblPromoDetailViewModel : ViewModelBase
    {
        private int _selecteIndex;

        public int SelecteIndex
        {
            get { return _selecteIndex; }
            set
            {
                _selecteIndex = value;
                RaisePropertyChanged("SelecteIndex");
            }
        }

        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set
            {
                _Iserial = value;
                RaisePropertyChanged("Iserial");
            }
        }

        private string CodeField;

        private int GlserialField;

        private string MobileNoField;

        private string PINField;

        private int? StatusField;

        private int? TblPromoHeaderField;

        private DateTime? UseDateField;

        public new string Code
        {
            get
            {
                return CodeField;
            }
            set
            {
                if ((ReferenceEquals(CodeField, value) != true))
                {
                    CodeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public int Glserial
        {
            get
            {
                return GlserialField;
            }
            set
            {
                if ((GlserialField.Equals(value) != true))
                {
                    GlserialField = value;
                    RaisePropertyChanged("Glserial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqMobileNo")]
        public string MobileNo
        {
            get
            {
                return MobileNoField;
            }
            set
            {
                if ((ReferenceEquals(MobileNoField, value) != true))
                {
                    MobileNoField = value;
                    RaisePropertyChanged("MobileNo");
                }
            }
        }

        public string PIN
        {
            get
            {
                return PINField;
            }
            set
            {
                if ((ReferenceEquals(PINField, value) != true))
                {
                    PINField = value;
                    RaisePropertyChanged("PIN");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStatus")]
        public int? Status
        {
            get
            {
                return StatusField;
            }
            set
            {
                if ((StatusField.Equals(value) != true))
                {
                    StatusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public int? TblPromoHeader
        {
            get
            {
                return TblPromoHeaderField;
            }
            set
            {
                if ((TblPromoHeaderField.Equals(value) != true))
                {
                    TblPromoHeaderField = value;
                    RaisePropertyChanged("TblPromoHeader");
                }
            }
        }

        public DateTime? UseDate
        {
            get
            {
                return UseDateField;
            }
            set
            {
                if ((UseDateField.Equals(value) != true))
                {
                    UseDateField = value;
                    RaisePropertyChanged("UseDate");
                }
            }
        }

        private int? _SalesGlSerial;

        public int? SalesGlSerial
        {
            get { return _SalesGlSerial; }
            set { _SalesGlSerial = value;RaisePropertyChanged("SalesGlSerial"); }
        }

        private int? _SalesTblStore;

        public int? SalesTblStore
        {
            get { return _SalesTblStore; }
            set { _SalesTblStore = value; RaisePropertyChanged("SalesTblStore"); }
        }

    }
}