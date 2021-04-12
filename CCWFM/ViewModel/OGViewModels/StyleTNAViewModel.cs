using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using CCWFM.ProductionService;

namespace CCWFM.ViewModel.OGViewModels
{



    public class StyleTNAHeaderViewModel : ViewModelBase
    {

        public StyleTNAHeaderViewModel(StyleHeaderViewModel selectedStyle)
        {
            if (!DesignerProperties.IsInDesignTool)
            {

                GetItemPermissions(PermissionItemName.StyleTNAForm.ToString());

                SelectedStyleViewModel = selectedStyle;
                MainRowList = new ObservableCollection<TblStyleTNAHeaderViewModel>();
                SelectedMainRow = new TblStyleTNAHeaderViewModel();


            }
        }



        #region Prop

        //        public event EventHandler StyleCompletedCompleted;
        private ObservableCollection<GenericTable> _StyleTNA;

        public ObservableCollection<GenericTable> StyleTNAList
        {
            get { return _StyleTNA ?? (_StyleTNA = new ObservableCollection<GenericTable>()); }
            set { _StyleTNA = value; RaisePropertyChanged("StyleTNAList"); }
        }

        private
        ObservableCollection<TblStyleTNAHeaderViewModel> _mainRowList;

        public ObservableCollection<TblStyleTNAHeaderViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblStyleTNAHeaderViewModel> _selectedMainRows;

        public ObservableCollection<TblStyleTNAHeaderViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStyleTNAHeaderViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private ObservableCollection<TblStoreIntialOrderViewModel> _selectedDetailRows;

        public ObservableCollection<TblStoreIntialOrderViewModel> SelectedDetailRows
        {
            get { return _selectedDetailRows ?? (_selectedDetailRows = new ObservableCollection<TblStoreIntialOrderViewModel>()); }
            set { _selectedDetailRows = value; RaisePropertyChanged("SelectedDetailRows"); }
        }

        private StyleHeaderViewModel _selectedStyleViewModel;

        public StyleHeaderViewModel SelectedStyleViewModel
        {
            get { return _selectedStyleViewModel; }
            set { _selectedStyleViewModel = value; RaisePropertyChanged("SelectedStyleViewModel"); }
        }

        private TblStyleTNAHeaderViewModel _selectedMainRow;

        public TblStyleTNAHeaderViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }


        #endregion Prop

        public void PreviewReport()
        {
            const string reportName = "SeasonMasterList";

            var reportViewmodel = new GenericReportViewModel();
            var para = new ObservableCollection<string>
            {
                LoggedUserInfo.Iserial.ToString(),
                SelectedStyleViewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture)
            };

            reportViewmodel.GenerateReport(reportName, para);
        }


    }
}