using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    #region ViewModels

    #endregion ViewModels

    public class AssetsTransactionViewModel : ViewModelBase
    {
        public AssetsTransactionViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.AssetsTransaction.ToString());

                Client.GetTblAssetsAsync(0, int.MaxValue, "it.Iserial", null, null, true);
                Client.GetTblAssetsCompleted += (s, sv) =>
                {
                    AssetsList = sv.Result;
                };
                var tblAssetsTypeclient = new CRUD_ManagerServiceClient();
                tblAssetsTypeclient.GetGenericAsync("TblAssetsStatus", "%%", "%%", "%%", "Iserial", "ASC");

                tblAssetsTypeclient.GetGenericCompleted += (s, sv) =>
                {
                    AssetsStatusList = sv.Result;
                };

                Client.UpdateOrInsertTblAssetsTransactionCompleted += (s, x) =>
                {
                    if (x.Result != null)
                    {
                        SelectedMainRow.InjectFrom(x.Result);
                        MessageBox.Show(strings.SavedMessage);
                    }
                    else
                    {
                        MessageBox.Show("This Asset Is Not Returned Yet");
                    }
                };

                Client.DeleteTblAssetsTransactionCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }
                    MessageBox.Show("Deleted Successfully");

                    SelectedMainRow = new TblAssetsTransactionViewModel();
                };
            }
        }

        private ObservableCollection<GenericTable> _assetsStatusList;

        public ObservableCollection<GenericTable> AssetsStatusList
        {
            get { return _assetsStatusList; }
            set { _assetsStatusList = value; RaisePropertyChanged("AssetsStatusList"); }
        }

        private ObservableCollection<TblAsset> _assetsList;

        public ObservableCollection<TblAsset> AssetsList
        {
            get { return _assetsList; }
            set { _assetsList = value; RaisePropertyChanged("AssetsList"); }
        }

        public void SearchAssetsTransaction()
        {
            var childWindow = new SearchAssetsTransaction(this);
            childWindow.Show();
        }

        public void DeleteMainRow()
        {
            var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                if (SelectedMainRow.Iserial != 0)
                {
                    if (AllowDelete != true)
                    {
                        MessageBox.Show(strings.AllowDeleteMsg);
                        return;
                    }
                    Loading = true;
                    Client.DeleteTblAssetsTransactionAsync(
                        (TblAssetsTransaction)new TblAssetsTransaction().InjectFrom(SelectedMainRow), 0);
                }
                else
                {
                    SelectedMainRow = new TblAssetsTransactionViewModel();
                }
            }
        }

        public void SaveMainRow()
        {
            if (SelectedMainRow != null)
            {
                var valiationCollection = new List<ValidationResult>();

                var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);
                if (SelectedMainRow.Empid == null && SelectedMainRow.StoreCode == null || (SelectedMainRow.Status == 0))
                {
                    isvalid = false;
                }
                if (isvalid)
                {
                    var save = SelectedMainRow.Iserial == 0;
                    if (save)
                    {
                        if (AllowAdd != true)
                        {
                            MessageBox.Show(strings.AllowAddMsg);
                            return;
                        }
                    }
                    else
                    {
                        if (AllowUpdate != true)
                        {
                            MessageBox.Show(strings.AllowUpdateMsg);
                            return;
                        }
                    }

                    var saveRow = new TblAssetsTransaction();
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblAssetsTransactionAsync(saveRow, save, 0);
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        #region Prop

        private TblAssetsTransactionViewModel _selectedMainRow;

        public TblAssetsTransactionViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblAssetsTransactionViewModel()); }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        public event EventHandler SubmitSearchAction;

        protected virtual void OnSubmitSearchAction()
        {
            EventHandler handler = SubmitSearchAction;
            if (handler != null) handler(this, EventArgs.Empty);
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

        public void SubmitSearch(TblAssetsTransactionViewModel row)
        {
            SelectedMainRow = row;
            SubmitSearchAction.Invoke(this, null);
        }
    }
}