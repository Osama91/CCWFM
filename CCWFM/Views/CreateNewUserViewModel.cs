using CCWFM.ViewModel;

namespace CCWFM.ViewModel.OGViewModels
{
    public class CreateNewUserViewModel : ViewModelBase
    {
        private TblCreateNewUserViewModel _selectedMainRow;

        public TblCreateNewUserViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblCreateNewUserViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }
    }
}