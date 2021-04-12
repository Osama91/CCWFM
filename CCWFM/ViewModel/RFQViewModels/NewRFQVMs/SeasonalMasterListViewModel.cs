using System;

namespace CCWFM.ViewModel.RFQViewModels.NewRFQVMs
{
    public class SeasonalMasterListViewModel : ViewModelBase
    {
        #region [ Events ]

        #endregion [ Events ]

        #region [ Fields ]

        private string _brandCode;
        private string _description;
        private string _seasonCode;
        private string _styleCode;
        private string _color;
        private DateTime _deliveryDate;
        private int _qty;
        #endregion [ Fields ]

        #region [ Properties ]

        public string BrandCode
        {
            get { return _brandCode; }
            set { _brandCode = value; RaisePropertyChanged("BrandCode"); }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChanged("Color"); }
        }

        public DateTime DeliveryDate
        {
            get { return _deliveryDate; }
            set { _deliveryDate = value; RaisePropertyChanged("DeliveryDate"); }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                RaisePropertyChanged("Qty");
            }
        }

        public string SeasonCode
        {
            get { return _seasonCode; }
            set { _seasonCode = value; RaisePropertyChanged("SeasonCode"); }
        }

        public string StyleCode
        {
            get { return _styleCode; }
            set
            {
                _styleCode = value;
                RaisePropertyChanged("StyleCode");
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