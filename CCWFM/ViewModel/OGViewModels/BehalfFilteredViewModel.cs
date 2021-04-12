using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CCWFM.Helpers.AuthenticationHelpers;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblBehalfFilteredViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private string _emplidField;

        private string _managerIdField;

        private string _nameField;

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set { _checked = value; RaisePropertyChanged("Checked");}
        }
        
        
        public string Emplid
        {
            get
            {
                return _emplidField;
            }
            set
            {
                if ((ReferenceEquals(_emplidField, value) != true))
                {
                    _emplidField = value;
                    RaisePropertyChanged("Emplid");
                }
            }
        }

        
        public string ManagerId
        {
            get
            {
                return _managerIdField;
            }
            set
            {
                if ((ReferenceEquals(_managerIdField, value) != true))
                {
                    _managerIdField = value;
                    RaisePropertyChanged("ManagerId");
                }
            }
        }

        
        public string Name
        {
            get
            {
                return _nameField;
            }
            set
            {
                if ((ReferenceEquals(_nameField, value) != true))
                {
                    _nameField = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
    }

    public class BehalfFilteredViewModel : ViewModelBase
    {
        public BehalfFilteredViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new ObservableCollection<TblBehalfFilteredViewModel>();
                SelectedMainRow = new TblBehalfFilteredViewModel();
                Client.GetBehalfFilteredsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblBehalfFilteredViewModel();
                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Emplid";

            Client.GetBehalfFilteredsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Code);
        }

        public void SaveMainRow()
        {
       
                foreach (var behalfFiltered in MainRowList.Where(x=>x.Checked))
                {
                    Client.TerminateEmpAsync(behalfFiltered.Emplid);
                }
               
            
        }

        #region Prop

        private ObservableCollection<TblBehalfFilteredViewModel> _mainRowList;

        public ObservableCollection<TblBehalfFilteredViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblBehalfFilteredViewModel> _selectedMainRows;

        public ObservableCollection<TblBehalfFilteredViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblBehalfFilteredViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblBehalfFilteredViewModel _selectedMainRow;

        public TblBehalfFilteredViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}