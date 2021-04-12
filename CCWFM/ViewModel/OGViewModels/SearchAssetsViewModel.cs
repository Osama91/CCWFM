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

    public class TblAssetsViewModel : PropertiesViewModelBase
    {
        private string _codeField;

        private int _iserialField;

        private string _aname;

        private string _ename;

        public int Iserial
        {
            get { return _iserialField; }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string Code
        {
            get { return _codeField; }
            set
            {
                if ((ReferenceEquals(_codeField, value) != true))
                {
                    if (value != null) _codeField = value.Trim();
                    RaisePropertyChanged("Code");
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAname")]
        public string Aname
        {
            get { return _aname; }
            set
            {
                if ((ReferenceEquals(_aname, value) != true))
                {
                    if (value != null) _aname = value.Trim();
                    RaisePropertyChanged("Aname");
                    if (string.IsNullOrWhiteSpace(Ename))
                    {
                        Ename = Aname;
                    }
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqEname")]
        public string Ename
        {
            get { return _ename; }
            set
            {
                if ((ReferenceEquals(_ename, value) != true))
                {
                    if (value != null) _ename = value.Trim();
                    RaisePropertyChanged("Ename");
                    if (string.IsNullOrWhiteSpace(Aname))
                    {
                        Aname = Ename;
                    }
                }
            }
        }

        private string _techSpec;

        public string TechSpec
        {
            get
            {
                return _techSpec;
            }
            set
            {
                if ((ReferenceEquals(_techSpec, value) != true))
                {
                    // Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "TechSpec" });
                    _techSpec = value;
                    RaisePropertyChanged("TechSpec");
                }
            }
        }

        private int? _yearOfProduct;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqYearOfProduction")]
        public int? YearOfProduct
        {
            get
            {
                return _yearOfProduct;
            }
            set
            {
                if ((_yearOfProduct.Equals(value) != true))
                {
                    //  Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "YearOfProduct" });
                    _yearOfProduct = value;
                    RaisePropertyChanged("YearOfProduct");
                }
            }
        }

        private string _notes;

        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                if ((ReferenceEquals(_notes, value) != true))
                {
                    _notes = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        private double? _purchasePrice;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqPurchasePrice")]
        public double? PurchasePrice
        {
            get
            {
                return _purchasePrice;
            }
            set
            {
                if ((_purchasePrice.Equals(value) != true))
                {
                    //   Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "PurchasePrice" });
                    _purchasePrice = value;
                    RaisePropertyChanged("PurchasePrice");
                }
            }
        }

        private int? _tblAssetsType;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqAssetType")]
        public int? TblAssetsType
        {
            get
            {
                return _tblAssetsType;
            }
            set
            {
                if ((_tblAssetsType.Equals(value) != true))
                {
                    //Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "TblAssetsType" });
                    _tblAssetsType = value;
                    RaisePropertyChanged("TblAssetsType");
                }
            }
        }

        private GenericTable _assetPerRow;

        public GenericTable AssetTypePerRow
        {
            get { return _assetPerRow ?? (_assetPerRow = new GenericTable()); }
            set { _assetPerRow = value; RaisePropertyChanged("AssetTypePerRow"); }
        }

        private bool _disposable;

        public bool Disposable
        {
            get { return _disposable; }
            set { _disposable = value; RaisePropertyChanged("Disposable"); }
        }

        private int _tblProcessor;

        public int TblProcessor
        {
            get
            {
                return _tblProcessor;
            }
            set
            {
                if ((_tblProcessor.Equals(value) != true))
                {
                    _tblProcessor = value;
                    RaisePropertyChanged("TblProcessor");
                }
            }
        }

        private int _statusField;

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

        private GenericTable _processorPerRow;

        public GenericTable ProcessorPerRow
        {
            get { return _processorPerRow ?? (_processorPerRow = new GenericTable()); }
            set { _processorPerRow = value; RaisePropertyChanged("ProcessorPerRow"); }
        }

        private int _tblHardDisk;

        public int TblHardDisk
        {
            get
            {
                return _tblHardDisk;
            }
            set
            {
                if ((_tblHardDisk.Equals(value) != true))
                {
                    _tblHardDisk = value;
                    RaisePropertyChanged("TblHardDisk");
                }
            }
        }

        private GenericTable _hardDiskPerRow;

        public GenericTable HardDiskPerRow
        {
            get { return _hardDiskPerRow ?? (_hardDiskPerRow = new GenericTable()); }
            set { _hardDiskPerRow = value; RaisePropertyChanged("HardDiskPerRow"); }
        }

        private int _tblMemory;

        public int TblMemory
        {
            get
            {
                return _tblMemory;
            }
            set
            {
                if ((_tblMemory.Equals(value) != true))
                {
                    _tblMemory = value;
                    RaisePropertyChanged("TblMemory");
                }
            }
        }

        private GenericTable _memoryPerRow;

        public GenericTable MemoryPerRow
        {
            get { return _memoryPerRow ?? (_memoryPerRow = new GenericTable()); }
            set { _memoryPerRow = value; RaisePropertyChanged("MemoryPerRow"); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        private bool _pending;

        public bool Pending
        {
            get { return _pending; }
            set { _pending = value; RaisePropertyChanged("Pending"); }
        }
    }

    #endregion ViewModels

    public class SearchAssetsViewModel : ViewModelBase
    {
        public SearchAssetsViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<TblAssetsViewModel>();
                SelectedMainRow = new TblAssetsViewModel();
                

                Client.GetTblAssetsCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblAssetsViewModel();
                        newrow.InjectFrom(row);
                        if (row.TblAssetsType1 != null) newrow.AssetTypePerRow.InjectFrom(row.TblAssetsType1);
                        if (row.TblHardDisk1 != null) newrow.HardDiskPerRow.InjectFrom(row.TblHardDisk1);
                        if (row.TblMemory1 != null) newrow.MemoryPerRow.InjectFrom(row.TblMemory1);
                        if (row.TblProcessor1 != null) newrow.ProcessorPerRow.InjectFrom(row.TblProcessor1);
                        newrow.Pending = sv.PendingAssets.Any(x => x == row.Iserial);
                        MainRowList.Add(newrow);
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
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblAssetsAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, false);
        }

        #region Prop

        private SortableCollectionView<TblAssetsViewModel> _mainRowList;

        public SortableCollectionView<TblAssetsViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblAssetsViewModel> _selectedMainRows;

        public ObservableCollection<TblAssetsViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblAssetsViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblAssetsViewModel _selectedMainRow;

        public TblAssetsViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop

        public void Report()
        {
            var reportName = "AssetsReport";

            //if (CultureInfo.CurrentCulture.Name.ToLower().Contains("ar"))
            //{ reportName = "FabricInspectionar"; }

            //var para = new ObservableCollection<string> { TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }
    }
}