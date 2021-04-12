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
using CCWFM.Views.OGView.SearchChildWindows;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.ViewModel.OGViewModels
{
    public class AssetsViewModel : ViewModelBase
    {
        public AssetsViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(PermissionItemName.AssetsForm.ToString());

                var tblAssetsTypeclient = new CRUD_ManagerServiceClient();
                tblAssetsTypeclient.GetGenericAsync("TblAssetsType", "%%", "%%", "%%", "Iserial", "ASC");

                tblAssetsTypeclient.GetGenericCompleted += (s, sv) =>
                {
                    AssetsTypeList = sv.Result;
                };

                var tblAssetsStatusclient = new CRUD_ManagerServiceClient();
                tblAssetsStatusclient.GetGenericAsync("TblAssetsStatus", "%%", "%%", "%%", "Iserial", "ASC");

                tblAssetsStatusclient.GetGenericCompleted += (s, sv) =>
               {
                   AssetsStatusList = sv.Result;
               };

                var tblProcessorclient = new CRUD_ManagerServiceClient();
                tblProcessorclient.GetGenericAsync("TblProcessor", "%%", "%%", "%%", "Iserial", "ASC");

                tblProcessorclient.GetGenericCompleted += (s, sv) =>
                {
                    ProcessorList = sv.Result;
                };

                var tblHardDiskclient = new CRUD_ManagerServiceClient();
                tblHardDiskclient.GetGenericAsync("TblHardDisk", "%%", "%%", "%%", "Iserial", "ASC");

                tblHardDiskclient.GetGenericCompleted += (s, sv) =>
                {
                    HardDiskList = sv.Result;
                };

                var tblMemoryclient = new CRUD_ManagerServiceClient();
                tblMemoryclient.GetGenericAsync("TblMemory", "%%", "%%", "%%", "Iserial", "ASC");

                tblMemoryclient.GetGenericCompleted += (s, sv) =>
                {
                    MemoryList = sv.Result;
                };

                Client.UpdateOrInsertTblAssetsCompleted += (s, x) => SelectedMainRow.InjectFrom(x.Result);

                Client.DeleteTblAssetsCompleted += (s, ev) =>
                {
                    if (ev.Error != null)
                    {
                        throw ev.Error;
                    }

                    SelectedMainRow = new TblAssetsViewModel();
                };
                Client.GetMaxAssetsCompleted += (s, sv) =>
                {
                    SelectedMainRow.Code = sv.Result;
                };
            }
        }

        private ObservableCollection<GenericTable> _hardDiskList;

        public ObservableCollection<GenericTable> HardDiskList
        {
            get { return _hardDiskList; }
            set { _hardDiskList = value; RaisePropertyChanged("HardDiskList"); }
        }

        private ObservableCollection<GenericTable> _memoryList;

        public ObservableCollection<GenericTable> MemoryList
        {
            get { return _memoryList; }
            set { _memoryList = value; RaisePropertyChanged("MemoryList"); }
        }

        private ObservableCollection<GenericTable> _processorList;

        public ObservableCollection<GenericTable> ProcessorList
        {
            get { return _processorList; }
            set { _processorList = value; RaisePropertyChanged("ProcessorList"); }
        }

        private ObservableCollection<GenericTable> _assetsTypeList;

        public ObservableCollection<GenericTable> AssetsTypeList
        {
            get { return _assetsTypeList; }
            set { _assetsTypeList = value; RaisePropertyChanged("AssetsTypeList"); }
        }

        private ObservableCollection<GenericTable> _assetsStatusList;

        public ObservableCollection<GenericTable> AssetsStatusList
        {
            get { return _assetsStatusList; }
            set { _assetsStatusList = value; RaisePropertyChanged("AssetsStatusList"); }
        }

        public void SearchAssets()
        {
            var childWindow = new SearchAssets(this);
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
                    Client.DeleteTblAssetsAsync(
                        (TblAsset)new TblAsset().InjectFrom(SelectedMainRow), 0);
                }
                else
                {
                    SelectedMainRow = new TblAssetsViewModel();
                }
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
                    var saveRow = new TblAsset();
                    if (AssetsTypeList != null && SelectedMainRow.TblAssetsType == null)
                    {
                        SelectedMainRow.AssetTypePerRow = AssetsTypeList.FirstOrDefault(x => x.Code == "N/A");
                        if (SelectedMainRow.AssetTypePerRow != null)
                            SelectedMainRow.TblAssetsType = SelectedMainRow.AssetTypePerRow.Iserial;
                    }
                    if (HardDiskList != null && SelectedMainRow.TblHardDisk == 0)
                    {
                        SelectedMainRow.HardDiskPerRow = HardDiskList.FirstOrDefault(x => x.Code == "N/A");
                        if (SelectedMainRow.HardDiskPerRow != null)
                            SelectedMainRow.TblHardDisk = SelectedMainRow.HardDiskPerRow.Iserial;
                    }
                    if (MemoryList != null && SelectedMainRow.TblMemory == 0)
                    {
                        SelectedMainRow.MemoryPerRow = MemoryList.FirstOrDefault(x => x.Code == "N/A");
                        if (SelectedMainRow.MemoryPerRow != null)
                            SelectedMainRow.TblMemory = SelectedMainRow.MemoryPerRow.Iserial;
                    }
                    if (ProcessorList != null && SelectedMainRow.TblProcessor == 0)
                    {
                        SelectedMainRow.ProcessorPerRow = ProcessorList.FirstOrDefault(x => x.Code == "N/A");
                        if (SelectedMainRow.ProcessorPerRow != null)
                            SelectedMainRow.TblProcessor = SelectedMainRow.ProcessorPerRow.Iserial;
                    }
                    saveRow.InjectFrom(SelectedMainRow);
                    Client.UpdateOrInsertTblAssetsAsync(saveRow, save, 0);
                }
                else
                {
                    MessageBox.Show("Data Was Not Saved");
                }
            }
        }

        #region Prop

        private int _copyTimes;

        public int CopyTimes
        {
            get { return _copyTimes; }
            set { _copyTimes = value; RaisePropertyChanged("CopyTimes"); }
        }

        private TblAssetsViewModel _selectedMainRow;

        public TblAssetsViewModel SelectedMainRow
        {
            get { return _selectedMainRow ?? (_selectedMainRow = new TblAssetsViewModel()); }
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
            const string reportName = "AssetsReport";

            var reportViewmodel = new GenericReportViewModel();
            reportViewmodel.GenerateReport(reportName, null);
        }

        public void SubmitSearch(TblAssetsViewModel row)
        {
            SelectedMainRow = row;
            SubmitSearchAction.Invoke(this, null);
        }

        public void CopyAsset()
        {
            var saveRow = new TblAsset();

            saveRow.InjectFrom(SelectedMainRow);
            saveRow.Iserial = 0;
            saveRow.Code = "";

            Client.CopyAssetsAsync(saveRow, CopyTimes);
        }

        public void GetMaxAsset()
        {
            Client.GetMaxAssetsAsync();
            if (AssetsTypeList != null)
            {
                SelectedMainRow.AssetTypePerRow = AssetsTypeList.FirstOrDefault(x => x.Code == "N/A");
                if (SelectedMainRow.AssetTypePerRow != null)
                    SelectedMainRow.TblAssetsType = SelectedMainRow.AssetTypePerRow.Iserial;
            }
            if (HardDiskList != null)
            {
                SelectedMainRow.HardDiskPerRow = HardDiskList.FirstOrDefault(x => x.Code == "N/A");
                if (SelectedMainRow.HardDiskPerRow != null)
                    SelectedMainRow.TblHardDisk = SelectedMainRow.HardDiskPerRow.Iserial;
            }
            if (MemoryList != null)
            {
                SelectedMainRow.MemoryPerRow = MemoryList.FirstOrDefault(x => x.Code == "N/A");
                if (SelectedMainRow.MemoryPerRow != null)
                    SelectedMainRow.TblMemory = SelectedMainRow.MemoryPerRow.Iserial;
            }
            if (ProcessorList != null)
            {
                SelectedMainRow.ProcessorPerRow = ProcessorList.FirstOrDefault(x => x.Code == "N/A");
                if (SelectedMainRow.ProcessorPerRow != null)
                    SelectedMainRow.TblProcessor = SelectedMainRow.ProcessorPerRow.Iserial;
            }
        }
    }
}