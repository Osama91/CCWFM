using CCWFM.ViewModel;

namespace CCWFM.Helpers.GenericViewModels
{
    public class ErrorInfo : ViewModelBase
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; RaisePropertyChanged("ErrorMessage"); }
        }

        private string _errorCode;

        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; RaisePropertyChanged("ErrorCode"); }
        }
    }
}