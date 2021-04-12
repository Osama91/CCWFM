using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class FingerPrintTransactionModel : PropertiesViewModelBase
    {
        private string _userId;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqCode")]
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                if ((ReferenceEquals(_userId, value) != true))
                {
                    _userId = value;
                    RaisePropertyChanged("UserId");
                }
            }
        }

        private int _glSerialField;

        public int GlSerial
        {
            get
            {
                return _glSerialField;
            }
            set
            {
                if ((_glSerialField.Equals(value) != true))
                {
                    _glSerialField = value;
                    RaisePropertyChanged("GlSerial");
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
                    StoreCode = StorePerRow.Code;
                    Company = StorePerRow.Organization;
                }
            }
        }

        private string _storeCode;

        [Required(ErrorMessageResourceType = typeof(strings), ErrorMessageResourceName = "ReqStore")]
        public string StoreCode
        {
            get
            {
                return _storeCode;
            }
            set
            {
                if ((ReferenceEquals(_storeCode, value) != true))
                {
                    _storeCode = value;
                    RaisePropertyChanged("StoreCode");
                }
            }
        }

        private DateTime _transDate;

        [ReadOnly(true)]
        public DateTime TransDate
        {
            get { return _transDate; }
            set { _transDate = value; RaisePropertyChanged("TransDate"); }
        }

        private int _sender;

        [ReadOnly(true)]
        public int Sender
        {
            get { return _sender; }
            set { _sender = value; RaisePropertyChanged("Sender"); }
        }

        private string _company;

        [ReadOnly(true)]
        public string Company
        {
            get { return _company; }
            set { _company = value; RaisePropertyChanged("Company"); }
        }

        private EmployeesView _empPerRow;

        public EmployeesView EmpPerRow
        {
            get { return _empPerRow; }
            set
            {
                _empPerRow = value; RaisePropertyChanged("EmpPerRow");
                if (EmpPerRow != null) UserId = EmpPerRow.Emplid;
            }
        }
    }

    public class FingerPrintTransactionViewModel : ViewModelBase
    {
        public FingerPrintTransactionViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                MainRowList = new SortableCollectionView<FingerPrintTransactionModel>();
                SelectedMainRow = new FingerPrintTransactionModel();
                GetItemPermissions(PermissionItemName.FingerPrintTransaction.ToString());
                

                Client.GetFingerPrintTransactionCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new FingerPrintTransactionModel();
                        newrow.InjectFrom(row);
                        newrow.StorePerRow =
                            sv.storesAllCompanies.FirstOrDefault(
                                x => x.Code == row.StoreCode && x.Organization == row.Company);
                        newrow.EmpPerRow = sv.empList.FirstOrDefault(x => x.Emplid == row.UserId);

                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertFingerPrintTransactionCompleted += (s, x) =>
                {
                    var savedRow = (FingerPrintTransactionModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteFingerPrintTransactionCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    var oldrow = MainRowList.FirstOrDefault(x => x.GlSerial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }

        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.GlSerial";

            Client.GetFingerPrintTransactionAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects);
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        if (row.GlSerial != 0)
                        {
                            Client.DeleteFingerPrintTransactionAsync(
                                (FingerPrintTransaction)new FingerPrintTransaction().InjectFrom(row), MainRowList.IndexOf(row));
                        }
                        else
                        {
                            MainRowList.Remove(row);
                        }
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new FingerPrintTransactionModel
                {
                    TransDate = DateTime.Now
                });
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                if (isvalid)
                {
                    var save = SelectedMainRow.GlSerial == 0;
                    var saveRow = new FingerPrintTransaction();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertFingerPrintTransactionAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow));
                }
            }
        }

        #region Prop

        private SortableCollectionView<FingerPrintTransactionModel> _mainRowList;

        public SortableCollectionView<FingerPrintTransactionModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<FingerPrintTransactionModel> _selectedMainRows;

        public ObservableCollection<FingerPrintTransactionModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<FingerPrintTransactionModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private FingerPrintTransactionModel _selectedMainRow;

        public FingerPrintTransactionModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}