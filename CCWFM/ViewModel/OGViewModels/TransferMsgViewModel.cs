using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblTransferMsgViewModel : PropertiesViewModelBase
    {
        public TblTransferMsgViewModel()
        {
            TransDate = DateTime.Now;
        }

        private string _colorField;
        private string _companyField;
        private string _descriptionField;
        private int _iserialField;
        private string _modelField;
        private string _sizeField;
        private string _storeField;

        public string Color
        {
            get
            {
                return _colorField;
            }
            set
            {
                if ((ReferenceEquals(_colorField, value) != true))
                {
                    _colorField = value;
                    RaisePropertyChanged("Color");
                }
            }
        }

        public string Company
        {
            get
            {
                return _companyField;
            }
            set
            {
                if ((ReferenceEquals(_companyField, value) != true))
                {
                    _companyField = value;
                    RaisePropertyChanged("Company");
                }
            }
        }

        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                if ((ReferenceEquals(_descriptionField, value) != true))
                {
                    _descriptionField = value;
                    RaisePropertyChanged("Description");
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

        public string Model
        {
            get
            {
                return _modelField;
            }
            set
            {
                if ((ReferenceEquals(_modelField, value) != true))
                {
                    _modelField = value;
                    RaisePropertyChanged("Model");
                }
            }
        }

        public string Size
        {
            get
            {
                return _sizeField;
            }
            set
            {
                if ((ReferenceEquals(_sizeField, value) != true))
                {
                    _sizeField = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        public string Store
        {
            get
            {
                return _storeField;
            }
            set
            {
                if ((ReferenceEquals(_storeField, value) != true))
                {
                    _storeField = value;
                    RaisePropertyChanged("Store");
                }
            }
        }

        private DateTime _transDateField;

        public DateTime TransDate
        {
            get
            {
                return _transDateField;
            }
            set
            {
                if ((_transDateField.Equals(value) != true))
                {
                    _transDateField = value;
                    RaisePropertyChanged("TransDate");
                }
            }
        }
    }

    public class TransferMsgViewModel : ViewModelBase
    {
        public TransferMsgViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.TransferMsg.ToString());
                MainRowList = new SortableCollectionView<TblTransferMsgViewModel>();
                SelectedMainRow = new TblTransferMsgViewModel();

          
                Client.GetTblTransferMsgCompleted += (s, sv) =>
                {
                    foreach (var row in sv.Result)
                    {
                        var newrow = new TblTransferMsgViewModel();

                        newrow.InjectFrom(row);
                        MainRowList.Add(newrow);
                    }
                    Loading = false;
                    FullCount = sv.fullCount;
                    if (FullCount == 0 && MainRowList.Count == 0)
                    {
                        AddNewMainRow(false);
                    }
                };

                Client.UpdateOrInsertTblTransferMsgCompleted += (s, x) =>
                {
                    var savedRow = (TblTransferMsgViewModel)MainRowList.GetItemAt(x.outindex);

                    if (savedRow != null) savedRow.InjectFrom(x.Result);
                };
                Client.DeleteTblTransferMsgCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                    if (oldrow != null) MainRowList.Remove(oldrow);
                };

                GetMaindata();
            }
        }
    
        public void GetMaindata()
        {
            if (SortBy == null)
                SortBy = "it.Iserial";
            Loading = true;
            Client.GetTblTransferMsgAsync(MainRowList.Count, PageSize, SortBy, Filter, ValuesObjects, LoggedUserInfo.Code);
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
                        if (row.Iserial != 0)
                        {
                            Client.DeleteTblTransferMsgAsync(
                                (TblTransferMsg)new TblTransferMsg().InjectFrom(row), MainRowList.IndexOf(row));
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

                MainRowList.Insert(currentRowIndex + 1, new TblTransferMsgViewModel());
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
                    var save = SelectedMainRow.Iserial == 0;
                    var saveRow = new TblTransferMsg();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblTransferMsgAsync(saveRow, save, MainRowList.IndexOf(SelectedMainRow), LoggedUserInfo.Code);
                }
            }
        }

        #region Prop

        private SortableCollectionView<TblTransferMsgViewModel> _mainRowList;

        public SortableCollectionView<TblTransferMsgViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblTransferMsgViewModel> _selectedMainRows;

        public ObservableCollection<TblTransferMsgViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblTransferMsgViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblTransferMsgViewModel _selectedMainRow;

        public TblTransferMsgViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        #endregion Prop
    }
}