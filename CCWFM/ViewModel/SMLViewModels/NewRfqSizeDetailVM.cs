namespace CCWFM.ViewModel.SMLViewModels
{
    public class NewRFQSizeDetailVM : ViewModelBase
    {
        #region [ Events ]

        #endregion [ Events ]

        #region [ Constructor ]

        #endregion [ Constructor ]

        #region [ Commands ]

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        #endregion [ Commands bound methods ]

        #region [ Fields ]

        private int IserialField;

        private int? ParentSerialField;

        private int? QtyField;

        private string SizeCodeField;

        private double? SizeRatioField;

        private NewRFQDetailsViewModel tblNewRFQDetailField;

        #endregion [ Fields ]

        #region [ Properties ]

        public int Iserial
        {
            get
            {
                return IserialField;
            }
            set
            {
                if ((IserialField.Equals(value) != true))
                {
                    IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public int? ParentSerial
        {
            get
            {
                return ParentSerialField;
            }
            set
            {
                if ((ParentSerialField.Equals(value) != true))
                {
                    ParentSerialField = value;
                    RaisePropertyChanged("ParentSerial");
                }
            }
        }

        public int? Qty
        {
            get
            {
                return QtyField;
            }
            set
            {
                if ((QtyField.Equals(value) != true))
                {
                    QtyField = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        public string SizeCode
        {
            get
            {
                return SizeCodeField;
            }
            set
            {
                if ((ReferenceEquals(SizeCodeField, value) != true))
                {
                    SizeCodeField = value;
                    RaisePropertyChanged("SizeCode");
                }
            }
        }

        public double? SizeRatio
        {
            get
            {
                return SizeRatioField;
            }
            set
            {
                if ((SizeRatioField.Equals(value) != true))
                {
                    SizeRatioField = value;
                    RaisePropertyChanged("SizeRatio");
                }
            }
        }

        public NewRFQDetailsViewModel tblNewRFQDetail
        {
            get
            {
                return tblNewRFQDetailField;
            }
            set
            {
                if ((ReferenceEquals(tblNewRFQDetailField, value) != true))
                {
                    tblNewRFQDetailField = value;
                    RaisePropertyChanged("tblNewRFQDetail");
                }
            }
        }

        #endregion [ Properties ]
    }
}