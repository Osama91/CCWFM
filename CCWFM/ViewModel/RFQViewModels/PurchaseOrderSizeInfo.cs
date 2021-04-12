using System;
using System.Windows.Media;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class PurchaseOrderSizeInfo : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler ConsumptionChangedHandler;

        public event EventHandler RatioChangedHandler;

        public virtual void OnConsumptionChanged()
        {
            var handler = ConsumptionChangedHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public virtual void OnRatioChanged()
        {
            var handler = RatioChangedHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Private fields ]

        private int? _Iserial;

        private bool _isTextBoxEnabled;

        private string _sizeCode;

        private double _sizeConsumption;

        private string _sizeGroup;

        private decimal _sizeRatio;

        #endregion [ Private fields ]

        #region [ Public properties ]

        private Brush _textBoxBackColor;

        public int? Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        public bool IsTextBoxEnabled
        {
            get { return _isTextBoxEnabled; }
            set
            {
                _isTextBoxEnabled = value;
                RaisePropertyChanged("IsTextBoxEnabled");
            }
        }

        public string SizeCode
        {
            get { return _sizeCode; }
            set
            {
                _sizeCode = value;
                RaisePropertyChanged("SizeCode");
            }
        }

        public double SizeConsumption
        {
            get { return _sizeConsumption; }
            set
            {
                //if (Math.Abs(_sizeConsumption - value) < double.Epsilon) return;
                TextBoxBackColor = value > 0 ? new SolidColorBrush(Color.FromArgb(255, 144, 238, 144)) : new SolidColorBrush(Colors.White);
                _sizeConsumption = value;
                OnConsumptionChanged();
                RaisePropertyChanged("SizeConsumption");
            }
        }

        public string SizeGroup
        {
            get { return _sizeGroup; }
            set
            {
                _sizeGroup = value;
                RaisePropertyChanged("SizeGroup");
            }
        }

        public decimal SizeRatio
        {
            get { return _sizeRatio; }
            set
            {
                if (_sizeRatio == value) return;
                _sizeRatio = value;
                OnRatioChanged();
                RaisePropertyChanged("SizeRatio");
            }
        }

        public Brush TextBoxBackColor
        {
            get { return _textBoxBackColor ?? (_textBoxBackColor = new SolidColorBrush(Colors.White)); }
            set
            {
                _textBoxBackColor = value;
                RaisePropertyChanged("TextBoxBackColor");
            }
        }

        #endregion [ Public properties ]
    }
}