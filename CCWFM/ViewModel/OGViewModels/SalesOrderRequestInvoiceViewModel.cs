using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.WarehouseService;
using CCWFM.Views.OGView.SearchChildWindows;
using CCWFM.ViewModel.GenericViewModels;
using System.Collections.Specialized;

namespace CCWFM.ViewModel.OGViewModels
{
	#region ViewModels

	public class TblSalesOrderRequestInvoiceHeaderModel : CRUDManagerService.PropertiesViewModelBase
    {
        private GlService.GenericTable _currencyPerRow;

        public GlService.GenericTable CurrencyPerRow
        {
            get { return _currencyPerRow; }
            set
            {
                _currencyPerRow = value; RaisePropertyChanged("CurrencyPerRow");
                if (_currencyPerRow != null) TblCurrency = CurrencyPerRow.Iserial;
            }
        }

        private int _TblCurrency;

        public int TblCurrency
        {
            get { return _TblCurrency; }
            set { _TblCurrency = value;RaisePropertyChanged("TblCurrency"); }
        }

        private string _SupplierInv;

        public string SupplierInv
        {
            get { return _SupplierInv; }
            set { _SupplierInv = value;RaisePropertyChanged("SupplierInv"); }
        }

        private GlService.GenericTable _JournalAccountTypePerRow;

        public GlService.GenericTable JournalAccountTypePerRow
        {
            get { return _JournalAccountTypePerRow; }
            set
            {
                _JournalAccountTypePerRow = value; RaisePropertyChanged("JournalAccountTypePerRow");
                TblJournalAccountType = _JournalAccountTypePerRow.Iserial;
            }
        }

        int TblJournalAccountTypeField;
        public int TblJournalAccountType
        {
            get
            {
                return this.TblJournalAccountTypeField;
            }
            set
            {
                if ((this.TblJournalAccountTypeField.Equals(value) != true))
                {
                    this.TblJournalAccountTypeField = value;
                    RaisePropertyChanged("TblJournalAccountType");
                }
            }
        }

        private GlService.Entity _EntityPerRow;

        public GlService.Entity EntityPerRow
        {
            get { return _EntityPerRow; }
            set
            {
                _EntityPerRow = value; RaisePropertyChanged("EntityPerRow");
                EntityAccount = _EntityPerRow.Iserial;
            }
        }

        int EntityAccountField;
        public int EntityAccount
        {
            get
            {
                return this.EntityAccountField;
            }
            set
            {
                if ((this.EntityAccountField.Equals(value) != true))
                {
                    this.EntityAccountField = value;
                    RaisePropertyChanged("EntityAccount");
                }
            }
        }


        private DateTime? _From;

        public DateTime? FromDate
        {
            get { return _From; }
            set { _From = value; RaisePropertyChanged("FromDate"); }
        }

        private DateTime? _to;

        public DateTime? ToDate
        {
            get { return _to; }
            set { _to = value; RaisePropertyChanged("ToDate"); }
        }
        private bool _visPosted;

		public bool VisPosted
		{
			get { return _visPosted; }
			set { _visPosted = value; RaisePropertyChanged("VisPosted"); }
		}
		private SortableCollectionView<TblMarkupTranProdViewModel> _markUpTransList;

		public SortableCollectionView<TblMarkupTranProdViewModel> MarkUpTransList
		{
			get { return _markUpTransList ?? (_markUpTransList = new SortableCollectionView<TblMarkupTranProdViewModel>()); }
			set
			{
				_markUpTransList = value;
				RaisePropertyChanged("MarkUpTransList");
			}
		}

		private int _miscValueType;

		public int MiscValueType
		{
			get { return _miscValueType; }
			set { _miscValueType = value; RaisePropertyChanged("MiscValueType"); }
		}

		private double? _miscValueField;
		public double? MiscValue
		{
			get
			{
				return _miscValueField;
			}
			set
			{
				if ((_miscValueField.Equals(value) != true))
				{
					_miscValueField = value;
					RaisePropertyChanged("MiscValue");
				}
			}
		}
        
		public TblSalesOrderRequestInvoiceHeaderModel()
		{
			DocDate = DateTime.Now;
		}

		private DateTime? _docDateField;		    

		private int _iserialField;

		private bool _enabled;

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if ((_enabled.Equals(value) != true))
				{
					_enabled = value;
					RaisePropertyChanged("Enabled");
				}
			}
		}

		[Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqDate")]
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

		private string _description;

		public string Description
		{
			get { return _description; }
			set { _description = value; RaisePropertyChanged("Description"); }
		}
		private int _status;

		public int Status
		{
			get { return _status; }
			set { _status = value;RaisePropertyChanged("Status"); }
		}

		private int _tblUser;

		public int TblUser
		{
			get { return _tblUser; }
			set { _tblUser = value;RaisePropertyChanged("TblUser"); }
		}

		private string _code;

		public string Code
		{
			get { return _code; }
			set { _code = value;RaisePropertyChanged("Code"); }
		}


    }

    public class TblSalesOrderRequestInvoiceDetailModel : CRUDManagerService.PropertiesViewModelBase
	{

        private CRUDManagerService.ItemsDto _itemPerRow;

        public CRUDManagerService.ItemsDto ItemPerRow
        {
            get { return _itemPerRow; }
            set
            {
                _itemPerRow = value; RaisePropertyChanged("ItemPerRow");
             
            }
        }

        private int IserialField;

        private System.Nullable<decimal> MiscField;

        private System.Nullable<decimal> PriceField;

        private System.Nullable<decimal> QtyField;

        private int TblItemDimField;

        private CCWFM.WarehouseService.TblItemDim TblItemDim1Field;
        

        private System.Nullable<int> TblSalesIssueHeaderField;


        private System.Nullable<int> TblSalesOrderRequestInvoiceHeaderField;


        
        public int Iserial
        {
            get
            {
                return this.IserialField;
            }
            set
            {
                if ((this.IserialField.Equals(value) != true))
                {
                    this.IserialField = value;
                    this.RaisePropertyChanged("Iserial");
                }
            }
        }

        
        public System.Nullable<decimal> Misc
        {
            get
            {
                return this.MiscField;
            }
            set
            {
                if ((this.MiscField.Equals(value) != true))
                {
                    this.MiscField = value;
                    this.RaisePropertyChanged("Misc");
                }
            }
        }

        
        public System.Nullable<decimal> Price
        {
            get
            {
                return this.PriceField;
            }
            set
            {
                if ((this.PriceField.Equals(value) != true))
                {
                    this.PriceField = value;
                    this.RaisePropertyChanged("Price");
                }
            }
        }

        [ReadOnly(true)]
        public System.Nullable<decimal> Qty
        {
            get
            {
                return this.QtyField;
            }
            set
            {
                if ((this.QtyField.Equals(value) != true))
                {
                    this.QtyField = value;
                    this.RaisePropertyChanged("Qty");
                }
            }
        }

        [ReadOnly(true)]
        public int TblItemDim
        {
            get
            {
                return this.TblItemDimField;
            }
            set
            {
                if ((this.TblItemDimField.Equals(value) != true))
                {
                    this.TblItemDimField = value;
                    this.RaisePropertyChanged("TblItemDim");
                }
            }
        }

        [ReadOnly(true)]
        public CCWFM.WarehouseService.TblItemDim TblItemDim1
        {
            get
            {
                return this.TblItemDim1Field;
            }
            set
            {
                if ((object.ReferenceEquals(this.TblItemDim1Field, value) != true))
                {
                    this.TblItemDim1Field = value;
                    this.RaisePropertyChanged("TblItemDim1");
                }
            }
        }
        
        public System.Nullable<int> TblSalesIssueHeader
        {
            get
            {
                return this.TblSalesIssueHeaderField;
            }
            set
            {
                if ((this.TblSalesIssueHeaderField.Equals(value) != true))
                {
                    this.TblSalesIssueHeaderField = value;
                    this.RaisePropertyChanged("TblSalesIssueHeader");
                }
            }
        }


        
        public System.Nullable<int> TblSalesOrderRequestInvoiceHeader
        {
            get
            {
                return this.TblSalesOrderRequestInvoiceHeaderField;
            }
            set
            {
                if ((this.TblSalesOrderRequestInvoiceHeaderField.Equals(value) != true))
                {
                    this.TblSalesOrderRequestInvoiceHeaderField = value;
                    this.RaisePropertyChanged("TblSalesOrderRequestInvoiceHeader");
                }
            }
        }


    }

	#endregion ViewModels

	public class SalesOrderRequestInvoiceViewModel : ViewModelStructuredBase
	{
        #region Prop        
        private ObservableCollection<GlService.GenericTable> _journalAccountTypeList;

        public ObservableCollection<GlService.GenericTable> JournalAccountTypeList
        {
            get { return _journalAccountTypeList; }
            set { _journalAccountTypeList = value; RaisePropertyChanged("JournalAccountTypeList"); }
        }

        public int TblSalesOrderHeaderRequest { get; set; }

		private string _supplierInv;

		public string SupplierInv
		{
			get { return _supplierInv; }
			set { _supplierInv = value; RaisePropertyChanged("SupplierInv");}
		}
		
		private TblMarkupTranProdViewModel _selectedMarkup;

		public TblMarkupTranProdViewModel SelectedMarkupRow
		{
			get { return _selectedMarkup; }
			set
			{
				_selectedMarkup = value;
				RaisePropertyChanged("SelectedMarkupRow");
			}
		}

		private ObservableCollection<CRUDManagerService.GenericTable> _miscValueType;

		public ObservableCollection<CRUDManagerService.GenericTable> MiscValueTypeList
		{
			get { return _miscValueType; }
			set { _miscValueType = value; RaisePropertyChanged("MiscValueTypeList"); }
		}

		private ObservableCollection<CRUDManagerService.GenericTable> _currencyList;

		public ObservableCollection<CRUDManagerService.GenericTable> CurrencyList
		{
			get { return _currencyList; }
			set { _currencyList = value; RaisePropertyChanged("CurrencyList"); }
		}

		private ObservableCollection<CRUDManagerService.TblMarkupProd> _markupList;

		public ObservableCollection<CRUDManagerService.TblMarkupProd> MarkupList
		{
			get { return _markupList; }
			set { _markupList = value; RaisePropertyChanged("MarkupList"); }
		}

		private TblSalesOrderRequestInvoiceHeaderModel _transactionHeader;

		public TblSalesOrderRequestInvoiceHeaderModel TransactionHeader
		{
			get
			{
				return _transactionHeader;
			}
			set
			{
				if ((ReferenceEquals(_transactionHeader, value) != true))
				{
					_transactionHeader = value;
					RaisePropertyChanged("TransactionHeader");
				}
			}
		}

		private ObservableCollection<TblSalesOrderRequestInvoiceHeaderModel> _transactionHeaderList;

		public ObservableCollection<TblSalesOrderRequestInvoiceHeaderModel> TransactionHeaderList
		{
			get
			{
				return _transactionHeaderList ?? (_transactionHeaderList = new ObservableCollection<TblSalesOrderRequestInvoiceHeaderModel>());
			}
			set
			{
				if ((ReferenceEquals(_transactionHeaderList, value) != true))
				{
					_transactionHeaderList = value;
					RaisePropertyChanged("TransactionHeaderList");
				}
			}
		}

		private ObservableCollection<TblSalesOrderRequestInvoiceDetailModel> _transactionDetails;

		public ObservableCollection<TblSalesOrderRequestInvoiceDetailModel> TransactionDetails
		{
			get
			{
				return _transactionDetails ?? (_transactionDetails = new ObservableCollection<TblSalesOrderRequestInvoiceDetailModel>());
			}
			set
			{
				if ((ReferenceEquals(_transactionDetails, value) != true))
				{
					_transactionDetails = value;
					RaisePropertyChanged("TransactionDetails");
				}
			}
		}

        private ObservableCollection<TblSalesIssueHeaderModel> _RecieveHeaderList;

        public ObservableCollection<TblSalesIssueHeaderModel> RecieveHeaderList
        {
            get { return _RecieveHeaderList ?? (_RecieveHeaderList = new ObservableCollection<TblSalesIssueHeaderModel>()); }
            set { _RecieveHeaderList = value; RaisePropertyChanged("RecieveHeaderList"); }
        }

        private ObservableCollection<TblSalesIssueHeaderModel> _RecieveHeaderChoosedList;

        public ObservableCollection<TblSalesIssueHeaderModel> RecieveHeaderChoosedList
        {
            get { return _RecieveHeaderChoosedList ?? (_RecieveHeaderChoosedList = new ObservableCollection<TblSalesIssueHeaderModel>()); }
            set { _RecieveHeaderChoosedList = value; RaisePropertyChanged("RecieveHeaderChoosedList"); }
        }

        #endregion Prop    

		private readonly CRUDManagerService.CRUD_ManagerServiceClient _webService = new CRUDManagerService.CRUD_ManagerServiceClient();
        public PurchasePlanService.PurchasePlanClient PurchasePlanClient = new PurchasePlanService.PurchasePlanClient();


        WarehouseService.WarehouseServiceClient WarehouseClient = new WarehouseService.WarehouseServiceClient();

        public SalesOrderRequestInvoiceViewModel() : base(PermissionItemName.SalesOrderRequestInvoice)
        {
            //         WarehouseClient.PostSalesOrderRequestInvoiceCompleted += (s, sv) =>
            //{
            //	if (sv.Result != null) TransactionHeader.InjectFrom(sv.Result);
            //	TransactionHeader.VisPosted = false;
            //	MessageBox.Show("Posted Completed");
            //};


            GetItemPermissions(PermissionItemName.SalesOrderRequestInvoice.ToString());
            var journalAccountTypeClient = new GlService.GlServiceClient();
            journalAccountTypeClient.GetGenericCompleted += (s, sv) =>
            {
                JournalAccountTypeList = sv.Result;
            };
            journalAccountTypeClient.GetGenericAsync("TblJournalAccountType", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

            MiscValueTypeList = new ObservableCollection<CRUDManagerService.GenericTable>
				{
					new CRUDManagerService.GenericTable {Iserial = 0, Code = "%", Ename = "%", Aname = "%"},
					new CRUDManagerService.GenericTable {Iserial = 1, Code = "Value", Ename = "Value", Aname = "Value"}
				};

		    var currencyClient = new GlService.GlServiceClient();

            currencyClient.GetGenericCompleted += (s, sv) =>
			{
                CurrencyList = new ObservableCollection<CRUDManagerService.GenericTable>();
                foreach (var item in sv.Result)
                {
                    CurrencyList.Add(new CRUDManagerService.GenericTable().InjectFrom(item) as CRUDManagerService.GenericTable);
                }

			};
			currencyClient.GetGenericAsync("TblCurrency", "%%", "%%", "%%", "Iserial", "ASC", LoggedUserInfo.DatabasEname);

            PurchasePlanClient.GetTblMarkupProdAsync(0, int.MaxValue, "it.Iserial", null, null, LoggedUserInfo.DatabasEname);

            PurchasePlanClient.GetTblMarkupProdCompleted += (s, sv) =>
			{
                MarkupList = new ObservableCollection<CRUDManagerService.TblMarkupProd>();
                foreach (var item in sv.Result)
                {
                    MarkupList.Add(new CRUDManagerService.TblMarkupProd().InjectFrom(item) as CRUDManagerService.TblMarkupProd) ;
                }
			
			};


			TransactionHeader = new TblSalesOrderRequestInvoiceHeaderModel { DocDate = DateTime.Now };



            WarehouseClient.UpdateOrInsertTblSalesOrderRequestInvoiceHeaderCompleted += (s, sv) =>
			{

				if (sv.Error != null)
				{
					MessageBox.Show(sv.Error.Message);
				}
				try
				{
					TransactionHeader.InjectFrom(sv.Result);
					
					if (TransactionHeader.Status==0)
					{
						TransactionHeader.VisPosted = true;
					}
					else
					{
						TransactionHeader.VisPosted = false;
					}
				}
				// ReSharper disable once EmptyGeneralCatchClause
				catch (Exception)
				{
				}
				Loading = false;
				Loading = false;
				MessageBox.Show("Saved");
			};

            WarehouseClient.UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProdsCompleted += (s, x) =>
			{
				var markup = new CRUDManagerService.TblMarkupProd();
				try
				{
					var row = TransactionHeader.MarkUpTransList.ElementAt(x.outindex);
					if (row != null)
					{
						markup = row.TblMarkupProd1;
					}
					TransactionHeader.MarkUpTransList.ElementAt(x.outindex).InjectFrom(x.Result);

					if (row != null) row.TblMarkupProd1 = markup;
				}
				// ReSharper disable once EmptyGeneralCatchClause
				catch (Exception)
				{
				}

				Loading = false;
			};
            WarehouseClient.GetTblSalesOrderRequestInvoiceHeaderCompleted += (s, sv) =>
			{
				Loading = false;
				TransactionHeaderList.Clear();
				foreach (var variable in sv.Result.ToList())
				{					
					var newrow = new TblSalesOrderRequestInvoiceHeaderModel();
					newrow.InjectFrom(variable);
					if (newrow.Status == 0)
					{
						newrow.VisPosted = true;
					}
                    newrow.JournalAccountTypePerRow = new GlService.GenericTable();
                    newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial == newrow.TblJournalAccountType);
                    newrow.EntityPerRow = new GlService.Entity().InjectFrom(sv.entityList.FirstOrDefault(w => w.Iserial == variable.EntityAccount && w.TblJournalAccountType == variable.TblJournalAccountType)) as GlService.Entity;
                   newrow.CurrencyPerRow = new GlService.GenericTable().InjectFrom( CurrencyList.FirstOrDefault(w => w.Iserial == newrow.TblCurrency)) as GlService.GenericTable;
                    TransactionHeaderList.Add(newrow);
				}
			};
            WarehouseClient.GetTblSalesIssueHeaderCompleted += (s, sv) =>
            {
                if (sv.Result != null)
                {
                    foreach (var row in sv.Result)
                    {
                        if (!RecieveHeaderList.Select(x => x.DocCode).Contains(row.DocCode))
                        {
                            RecieveHeaderList.Add(new TblSalesIssueHeaderModel().InjectFrom(row) as TblSalesIssueHeaderModel);
                        }
                    }
                }
            };
            WarehouseClient.GetSalesIssueHeaderPendingCompleted += (s, sv) =>
             {
                 if (sv.Result != null)
                 {
                     foreach (var row in sv.Result)
                     {
                         if (!RecieveHeaderChoosedList.Select(x => x.DocCode).Contains(row.DocCode))
                         {
                             RecieveHeaderChoosedList.Add(
                                 new TblSalesIssueHeaderModel().InjectFrom(row) as TblSalesIssueHeaderModel);
                         }
                     }
                 }
             };

            WarehouseClient.GetTblSalesOrderRequestInvoiceDetailCompleted += (s, sv) =>
			{
				foreach (var variable in sv.Result.ToList())
				{
					var newrow = new TblSalesOrderRequestInvoiceDetailModel();
					newrow.InjectFrom(variable);
newrow.ItemPerRow= new CRUDManagerService.ItemsDto().InjectFrom( sv.itemsList.FirstOrDefault(w => w.ItemGroup == variable.TblItemDim1.ItemType && w.Iserial == variable.TblItemDim1.ItemIserial) ) as CRUDManagerService.ItemsDto;
					TransactionDetails.Add(newrow);
				}
			};
            WarehouseClient.SearchSalesOrderRequestInvoiceCompleted += (s, sv) =>
			{
                if (sv.Result != null) TransactionHeader.InjectFrom(sv.Result);
                TransactionHeader.VisPosted = true;
                GetDetailData();
            };
            WarehouseClient.PostTblSalesOrderRequestInvoiceHeaderCompleted += (s, sv) =>
            {
                if (sv.Result != null) TransactionHeader.InjectFrom(sv.Result);
             //   TransactionHeader.VisPosted = true;
               // GetDetailData();
            };

        }

		public void GetMarkUpdata(bool header)
		{
			var client=
            new WarehouseService.WarehouseServiceClient();

            TransactionHeader.MarkUpTransList.Clear();
            client.GetTblSalesOrderRequestInvoiceMarkupTransProdAsync(0, TransactionHeader.Iserial, LoggedUserInfo.DatabasEname);
            client.GetTblSalesOrderRequestInvoiceMarkupTransProdCompleted += (s, sv) =>
			{
				foreach (var row in sv.Result)
				{
					var newrow = new TblMarkupTranProdViewModel();
					newrow.InjectFrom(row);
					newrow.CurrencyPerRow = new CRUDManagerService.GenericTable();                    
					newrow.CurrencyPerRow.InjectFrom(CurrencyList.FirstOrDefault(w => w.Iserial == newrow.TblCurrency));
                    newrow.TblMarkupProd1 = new CRUDManagerService.TblMarkupProd();                    
                    newrow.TblMarkupProd1.InjectFrom(MarkupList.FirstOrDefault(w=>w.Iserial==row.TblMarkupProd));
                    newrow.JournalAccountTypePerRow = JournalAccountTypeList.FirstOrDefault(w => w.Iserial==newrow.TblJournalAccountType);                
                      newrow.EntityPerRow = new GlService.Entity().InjectFrom(sv.entityList.FirstOrDefault(w => w.Iserial == row.EntityAccount && w.TblJournalAccountType == row.TblJournalAccountType)) as GlService.Entity;
                    newrow.TblJournalAccountType = row.TblJournalAccountType;
                newrow.EntityAccount = row.EntityAccount;
                                       
					TransactionHeader.MarkUpTransList.Add(newrow);
				}

				Loading = false;

				if (TransactionHeader.MarkUpTransList.Count == 0)
				{
					AddNewMarkUpRow(false, true);
				}
			};

			Loading = true;
		}

        public void GetRecieveHeaderListData()
        {
           
                WarehouseClient.GetTblSalesIssueHeaderAsync(RecieveHeaderList.Count, PageSize, TblSalesOrderHeaderRequest,TransactionHeader.TblJournalAccountType, TransactionHeader.EntityAccount, "it.Iserial", DetailSubFilter, DetailSubValuesObjects, LoggedUserInfo.DatabasEname);
            
        }
        public void DeleteMarkupRow(bool header)
		{
		}

		public void SaveMarkupRow()
		{
			if (SelectedMarkupRow != null)
			{
				var valiationCollection = new List<ValidationResult>();

				var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
					new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

				if (isvalid)
				{
					var save = SelectedMarkupRow.Iserial == 0;
					if (AllowUpdate != true && !save)
					{
						MessageBox.Show(strings.AllowUpdateMsg);
						return;
					}
					var saveRow = new WarehouseService.TblSalesOrderRequestInvoiceHeaderMarkupTransProd();
					saveRow.InjectFrom(SelectedMarkupRow);
					saveRow.TblSalesOrderRequestInvoiceHeader = TransactionHeader.Iserial;
					    
					if (!Loading)
					{
						Loading = true;

						WarehouseClient.UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProdsAsync(saveRow, save, TransactionHeader.MarkUpTransList.IndexOf(SelectedMarkupRow),
					  LoggedUserInfo.DatabasEname);
					}
				}
			}
		}

		public void SaveMarkupRowOldRow(TblMarkupTranProdViewModel oldRow)
		{
			if (oldRow != null)
			{
				var valiationCollection = new List<ValidationResult>();

				var isvalid = Validator.TryValidateObject(oldRow,
					new ValidationContext(oldRow, null, null), valiationCollection, true);

				if (isvalid)
				{
					var save = oldRow.Iserial == 0;
					if (AllowUpdate != true && !save)
					{
						MessageBox.Show(strings.AllowUpdateMsg);
						return;
					}
					var saveRow = new TblSalesOrderRequestInvoiceHeaderMarkupTransProd();
					saveRow.InjectFrom(oldRow);
					saveRow.TblSalesOrderRequestInvoiceHeader = TransactionHeader.Iserial;					

					if (!Loading)
					{
						Loading = true;

                        WarehouseClient.UpdateOrInsertTblSalesOrderRequestInvoiceMarkupTransProdsAsync(saveRow, save,
								TransactionHeader.MarkUpTransList.IndexOf(oldRow), LoggedUserInfo.DatabasEname);
					}
				}
			}
		}

		public void AddNewMarkUpRow(bool checkLastRow, bool header)
		{
			var currentRowIndex = (TransactionHeader.MarkUpTransList.IndexOf(SelectedMarkupRow));

			if (checkLastRow)
			{
				var valiationCollection = new List<ValidationResult>();

				var isvalid = Validator.TryValidateObject(SelectedMarkupRow,
					new ValidationContext(SelectedMarkupRow, null, null), valiationCollection, true);

				if (!isvalid)
				{
					return;
				}
			}
			if (AllowAdd != true)
			{
				MessageBox.Show(strings.AllowAddMsg);
				return;
			}
			var newrow = new TblMarkupTranProdViewModel
			{
				Type = 0,
				TblSalesOrderRequestInvoiceHeader = TransactionHeader.Iserial,
				MiscValueType = 1,
                JournalAccountTypePerRow= TransactionHeader.JournalAccountTypePerRow,
                EntityPerRow = TransactionHeader.EntityPerRow,
                TblJournalAccountType = TransactionHeader.TblJournalAccountType,
                EntityAccount = TransactionHeader.EntityAccount,
            };

			TransactionHeader.MarkUpTransList.Insert(currentRowIndex + 1, newrow);
			SelectedMarkupRow = newrow;
		}
        public void GetRecFromTo()
        {
              if (TransactionHeader.FromDate != null)
                    if (TransactionHeader.ToDate != null)                        
                            WarehouseClient.GetSalesIssueHeaderPendingAsync(TransactionHeader.TblJournalAccountType, TransactionHeader.EntityAccount, TransactionHeader.FromDate, TransactionHeader.ToDate);
            
        }

        public void GetRecieveDetailData()
        {
            var row = new TblSalesOrderRequestInvoiceHeader();

            row.InjectFrom(TransactionHeader);
            var headers = new ObservableCollection<int>(RecieveHeaderChoosedList.Select(x => x.Iserial));
            WarehouseClient.SearchSalesOrderRequestInvoiceAsync(row,LoggedUserInfo.Iserial,headers);
        }
        public void GetMaindata()
		{
            WarehouseClient.GetTblSalesOrderRequestInvoiceHeaderAsync(TransactionHeaderList.Count, PageSize,TblSalesOrderHeaderRequest, "it.Iserial", DetailFilter, DetailValuesObjects,LoggedUserInfo.DatabasEname);
		}

		public void SaveOrder()
		{
			var valiationCollectionHeader = new List<ValidationResult>();
			var isvalidHeader = Validator.TryValidateObject(TransactionHeader,
				new ValidationContext(TransactionHeader, null, null), valiationCollectionHeader, true);

			var details = new ObservableCollection<TblSalesOrderRequestInvoiceDetail>();
			var isvalid = false;
			foreach (var item in TransactionDetails)
			{
				var valiationCollection = new List<ValidationResult>(); isvalid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), valiationCollection, true);
				if (isvalid == false)
				{
					return;
				}

				details.Add((TblSalesOrderRequestInvoiceDetail)new TblSalesOrderRequestInvoiceDetail().InjectFrom(item));
			}
			var newrow = new  TblSalesOrderRequestInvoiceHeader();
			newrow.InjectFrom(TransactionHeader);

			newrow.TblSalesOrderRequestInvoiceDetails = details;
			if (isvalid && isvalidHeader)
			{
				if (Loading == false)
				{
					Loading = true;
                    WarehouseClient.UpdateOrInsertTblSalesOrderRequestInvoiceHeaderAsync(newrow, 0,LoggedUserInfo.Iserial);
				}
			}
			else
			{
				MessageBox.Show("Data Is NOt Valid");
			}
		}

		public void GetDetailData()
		{
			WarehouseClient.GetTblSalesOrderRequestInvoiceDetailAsync(0, int.MaxValue, TransactionHeader.Iserial, "it.Iserial", null, null,LoggedUserInfo.DatabasEname);
			//SubmitSearchAction.Invoke(this, null);
		}

		internal void FabricInspectionReport()
		{
			var reportName = "SalesOrderRequestInvoice";

			if (LoggedUserInfo.CurrLang == 0)
			{ reportName = "SalesOrderRequestInvoice"; }

			var para = new ObservableCollection<string> { TransactionHeader.Iserial.ToString(CultureInfo.InvariantCulture) };

			var reportViewmodel = new GenericReportViewModel();
			reportViewmodel.GenerateReport(reportName, para);
		}

		public void Post()
		{
			var saveRow = new TblSalesOrderRequestInvoiceHeader();
			saveRow.InjectFrom(TransactionHeader);
		WarehouseClient.PostTblSalesOrderRequestInvoiceHeaderAsync(saveRow, LoggedUserInfo.Iserial, LoggedUserInfo.DatabasEname);
		}


        public override void Search()
        {
            TransactionHeaderList.Clear();
            GetMaindata();
            if (SearchWindow == null)
                SearchWindow = new GenericSearchWindow(GetSearchModel());
            GenericSearchViewModel<TblSalesOrderRequestInvoiceHeaderModel> vm =
                new GenericSearchViewModel<TblSalesOrderRequestInvoiceHeaderModel>() { Title = "Sales Request Invoice Search" };
            vm.FilteredItemsList = TransactionHeaderList;
            vm.ItemsList = TransactionHeaderList;
            vm.ResultItemsList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    TransactionHeader = vm.ResultItemsList[e.NewStartingIndex];
                GetDetailData();
                // RaisePropertyChanged(nameof(IsReadOnly));
            };
            vm.GetDataCommand = new SilverlightCommands.RelayCommand((o) =>
            {
                Filter = vm.Filter;
                ValuesObjects = vm.ValuesObjects;
                GetMaindata();
            },
            (o) =>
            {
                return true;//هنا الصلاحيات
            });
            SearchWindow.DataContext = vm;
            base.Search();
        }
        private static ObservableCollection<SearchColumnModel> GetSearchModel()
        {
            return new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header="Code",
                        PropertyPath=nameof(TblSalesOrderRequestInvoiceHeaderModel.Code),
                    },
                     new SearchColumnModel()
                    {
                        Header=strings.JournalAccountType,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblSalesOrderRequestInvoiceHeaderModel.JournalAccountTypePerRow),nameof(TblWarehouse.Code)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblSalesOrderRequestInvoiceHeader.TblJournalAccountType),nameof(TblWarehouse.Code)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.Entity,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblSalesOrderRequestInvoiceHeaderModel.EntityPerRow),nameof(TblWarehouse.Code)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblSalesOrderRequestInvoiceHeader.EntityAccount),nameof(TblWarehouse.Ename)),
                    },
                       new SearchColumnModel()
                    {
                        Header=strings.Entity,
                        PropertyPath= string.Format("{0}.{1}", nameof(TblSalesOrderRequestInvoiceHeaderModel.EntityPerRow),nameof(TblWarehouse.Ename)),
                        FilterPropertyPath=string.Format("{0}.{1}",nameof(TblSalesOrderRequestInvoiceHeader.EntityAccount),nameof(TblWarehouse.Ename)),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.DocDate,
                        PropertyPath=nameof(TblSalesOrderRequestInvoiceHeaderModel.DocDate),
                    },
                    new SearchColumnModel()
                    {
                        Header=strings.SupplierInvoice,
                        PropertyPath=nameof(TblSalesOrderRequestInvoiceHeaderModel.SupplierInv),
                    },                  
                };
        }
    }
}