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
    #region ViewModels

    public class TblAssetsTransactionViewModel : PropertiesViewModelBase
    {
        public TblAssetsTransactionViewModel()
        {
            DocDate = DateTime.Now;
            ReciveDate = DateTime.Now;
            Quantity = 1;
        }

        private int _quantity;

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; RaisePropertyChanged("Quantity"); }
        }

        private DateTime? _docDateField;

        private int _iserialField;

        private string _notesField;

        private DateTime? _reciveDateField;

        private DateTime? _returnDateField;

        private int _statusField;

        private int? _tblAssetsField;

        public DateTime? DocDate
        {
            get
            {
                return _docDateField;
            }
            set
            {
                if ((_docDateField.Equals(value) != true))
                {
                    _docDateField = value;
                    RaisePropertyChanged("DocDate");
                }
            }
        }

        private string _organizationId;

        public string OrganizationId
        {
            get { return _organizationId; }
            set
            {
                if ((ReferenceEquals(_organizationId, value) != true))
                {
                    _organizationId = value; RaisePropertyChanged("OrganizationId");
                }
            }
        }

        private string _storeCode;

        public string StoreCode
        {
            get { return _storeCode; }
            set
            {
                if ((ReferenceEquals(_storeCode, value) != true))
                {
                    _storeCode = value; RaisePropertyChanged("StoreCode");
                }
            }
        }

        private StoreForAllCompany _storePerRow;

        public StoreForAllCompany StorePerRow
        {
            get { return _storePerRow; }
            set
            {
                _storePerRow = value; RaisePropertyChanged("StorePerRow");

                if (StorePerRow != null)
                {
                    StoreCode = _storePerRow.Code;
                    OrganizationId = StorePerRow.Organization;
                    EmpPerRow = null;
                    Empid = null;
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public string Notes
        {
            get
            {
                return _notesField;
            }
            set
            {
                if ((ReferenceEquals(_notesField, value) != true))
                {
                    _notesField = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

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
                if (EmpPerRow.Organization != null)
                {
                    OrganizationId = EmpPerRow.Organization;
                    StorePerRow = null;
                    StoreCode = null;
                }
            }
        }

        private GenericTable _assetsStatusPerRow;

        public GenericTable AssetsStatusPerRow
        {
            get { return _assetsStatusPerRow ?? (_assetsStatusPerRow = new GenericTable()); }
            set
            {
                _assetsStatusPerRow = value; RaisePropertyChanged("AssetsStatusPerRow");
                if (AssetsStatusPerRow != null) Status = AssetsStatusPerRow.Iserial;
            }
        }

        public DateTime? ReciveDate
        {
            get
            {
                return _reciveDateField;
            }
            set
            {
                if ((_reciveDateField.Equals(value) != true))
                {
                    _reciveDateField = value;
                    RaisePropertyChanged("ReciveDate");
                }
            }
        }

        public DateTime? ReturnDate
        {
            get
            {
                return _returnDateField;
            }
            set
            {
                if ((_returnDateField.Equals(value) != true))
                {
                    _returnDateField = value;
                    RaisePropertyChanged("ReturnDate");
                }
            }
        }

        public int Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                if ((_statusField.Equals(value) != true))
                {
                    _statusField = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAssets")]
        public int? TblAssets
        {
            get
            {
                return _tblAssetsField;
            }
            set
            {
                if ((_tblAssetsField.Equals(value) != true))
                {
                    _tblAssetsField = value;
                    RaisePropertyChanged("TblAssets");
                }
            }
        }

        private TblAssetsViewModel _assetPerRow;

        public TblAssetsViewModel AssetPerRow
        {
            get { return _assetPerRow ?? (_assetPerRow = new TblAssetsViewModel()); }
            set
            {
                _assetPerRow = value; RaisePropertyChanged("AssetPerRow");
                if (AssetPerRow != null) TblAssets = AssetPerRow.Iserial;
            }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }
    }

    #endregion ViewModels

    public class SearchAssetsTransactionViewModel : ViewModelBase
    {
        public SearchAssetsTransactionViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblAssetsTransactionViewModel>();
                SelectedMainRow = new TblAssetsTransactionViewModel();
                

                Client.GetTblAssetsTransactionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        if (sv.Stores != null)
                        {
                            var newrow = new TblAssetsTransactionViewModel
                            {
                                StorePerRow = sv.Stores.FirstOrDefault(
                                    x => x.Code == row.StoreCode && x.Organization == row.OrganizationId)
                            };

                            var tempEmp = sv.employees.FirstOrDefault(x => x.EMPLID == row.Empid);

                            if (tempEmp != null)
                            {
                                newrow.EmpPerRow = new EmployeesView
                                    {
                                        Emplid = row.Empid,
                                        Name = tempEmp.name
                                    };
                            }
                            newrow.AssetPerRow.InjectFrom(row.TblAsset);
                            newrow.InjectFrom(row);

                            MainRowList.Add(newrow);
                        }
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (MainRowList.Any() && (SelectedMainRow == null || SelectedMainRow.Iserial == 0))
                    {
                        SelectedMainRow = MainRowList.FirstOrDefault();
                    }

                    if (Export)
                    {
                        Export = false;

                        //var handler = ExportCompleted;
                        //if (handler != null) handler(this, EventArgs.Empty);
                        //ExportGrid.ExportExcel("Style");
                    }
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial desc ";
            Loading = true;
            Client.GetTblAssetsTransactionAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        #region Prop

        private SortableCollectionView<TblAssetsTransactionViewModel> _mainRowList;

        public SortableCollectionView<TblAssetsTransactionViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<GenericTable> _statusList;

        public ObservableCollection<GenericTable> StatusList
        {
            get { return _statusList; }
            set { _statusList = value; RaisePropertyChanged("StatusList"); }
        }

        private TblAssetsTransactionViewModel _selectedMainRow;

        public TblAssetsTransactionViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop

        public void Report()
        {
            var reportName = "AssetsTransactionReport";

            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }

            //var para = new ObservableCollection<string> { TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }
    }
}