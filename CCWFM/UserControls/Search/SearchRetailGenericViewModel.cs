using System;
using System.ComponentModel;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.ViewModel;
using Os.Controls.DataGrid;
using GenericTable = CCWFM.GlService.GenericTable;

namespace CCWFM.UserControls.Search
{
    public class SearchRetailGenericViewModel : ViewModelBase
    {
        public SearchRetailGenericViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();

                MainRowList = new SortableCollectionView<GenericTable>();
                SelectedMainRow = new GenericTable();



                Glclient.GetGenericCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        MainRowList.Add(row);
                    }

                    Loading = false;
                    
                };

            }
        }

        private string _direction;
        public string Direction
        {
            get { return _direction ?? "ASC"; }
            set { _direction = value; }
        }

        private string _code;
        public new string Code
        {
            get { return _code ?? "%%"; }
            set { _code = value; }
        }
        string _ename;
        public string Ename
        {
            get { return _ename ?? "%%"; }
            set { _ename = value; }
        }

        private string _aname;
        public string Aname
        {
            get { return _aname ?? "%%"; }
            set { _aname = value; }
        }

        private string _tablEname;
        public string TablEname
        {
            get { return _tablEname ?? "%%"; }
            set { _tablEname = value; }
        }
        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Glclient.GetGenericAsync(TablEname, Code, Ename, Aname, SortBy, Direction,LoggedUserInfo.DatabasEname);
        }

        #region Prop

        private SortableCollectionView<GenericTable> _mainRowList;

        public SortableCollectionView<GenericTable> MainRowList
        {
            get { return _mainRowList; }
            set
            {
                _mainRowList = value;
                RaisePropertyChanged("MainRowList");
            }
        }

        private GenericTable _selectedMainRow;

        public GenericTable SelectedMainRow
        {
            get { return _selectedMainRow; }
            set
            {
                _selectedMainRow = value;
                RaisePropertyChanged("SelectedMainRow");
            }
        }

        #endregion Prop
    }
}