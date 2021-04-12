using CCWFM.ViewModel.CommandsViewModelHelper;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class RFQSearchViewModel : ViewModelBase
    {
        private CommandsExecuter _searchCommand;
        private RFQViewModel _searchDataContext;

        public CommandsExecuter SearchCommand
        {
            get { return _searchCommand; }
            set
            {
                _searchCommand = value;
                RaisePropertyChanged("SearchCommand");
            }
        }

        public RFQViewModel SearchDataContext
        {
            get { return _searchDataContext; }
            set { _searchDataContext = value; RaisePropertyChanged("SearchDataContext"); }
        }

        public RFQSearchViewModel()
        {
            SearchDataContext = new RFQViewModel();
        }

        private void Search()
        {
        }
    }
}