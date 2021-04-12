using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;


namespace CCWFM.ViewModel.OGViewModels
{
    public class TblCreateNewUserViewModel : PropertiesViewModelBase
    {


        private string _empid;
        public string Empid
        {
            get
            {
                return _empid;
            }
            set
            {
                if ((ReferenceEquals(_empid, value) != true))
                {
                    _empid = value;
                    RaisePropertyChanged("Empid");
                }
            }
        }
        private EmployeesView _empPerRow;
        public EmployeesView EmpPerRow
        {
            get { return _empPerRow ?? (_empPerRow = new EmployeesView()); }
            set
            {
                _empPerRow = value; RaisePropertyChanged("EmpPerRow");

                Empid = EmpPerRow.Emplid;

            }
        }


        private string _Secondempid;
        public string SecondEmpid
        {
            get
            {
                return _Secondempid;
            }
            set
            {
                if ((ReferenceEquals(_Secondempid, value) != true))
                {
                    _Secondempid = value;
                    RaisePropertyChanged("Empid");
                }
            }
        }

        private EmployeesView _secondEmpPerRow;
        public EmployeesView SecondEmpPerRow
        {
            get { return _secondEmpPerRow ?? (_secondEmpPerRow = new EmployeesView()); }
            set
            {
                _secondEmpPerRow = value; RaisePropertyChanged("SecondEmpPerRow");

                SecondEmpid = SecondEmpPerRow.Emplid;
            }
        }
    }
}