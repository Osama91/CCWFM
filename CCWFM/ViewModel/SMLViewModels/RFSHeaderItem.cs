using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;
using CCWFM.Views.RequestForQutation;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class RFSHeaderItem : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler DeleteDetailItem;

        public virtual void OnDeleteDetailtemHeader()
        {
            var handler = DeleteDetailItem;
            if (handler == null) return;
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.Cancel) return;
            handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Private Fields ]

        private string _description;
        private byte[] _image;
        private int? _Iserial;
        private string _itemCode;

        #endregion [ Private Fields ]

        #region [ Public Properties ]

        private string _batch;

        private string _config;

        private string _itemGroup;

        private string _name;

        private ItemsDto _selectedRfqItem;

        private string _size;

        public string Batch
        {
            get { return _batch; }
            set { _batch = value; RaisePropertyChanged("Batch"); }
        }

        public string Config
        {
            get { return _config; }
            set { _config = value; RaisePropertyChanged("Config"); }
        }

        //[Display(ResourceType = typeof(strings), Name = "Notes", Prompt = "...")]
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        [Display(ResourceType = typeof(strings), Name = "img")]
        public byte[] Image
        {
            get { return _image; }
            set { _image = value; RaisePropertyChanged("Image"); }
        }

        public int? Iserial
        {
            get { return _Iserial; }
            set { _Iserial = value; RaisePropertyChanged("Iserial"); }
        }

        [Required]
        //[Display(ResourceType = typeof(strings), Name = "service")]
        public string ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; RaisePropertyChanged("ServiceCode"); }
        }

        public string ItemGroup
        {
            get { return _itemGroup; }
            set { _itemGroup = value; RaisePropertyChanged("ItemGroup"); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set { _objStatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        public int? ParentID
        {
            get { return _parentId; }
            set { _parentId = value; RaisePropertyChanged("ParentID"); }
        }

        public ItemsDto SelectedRFQItem
        {
            get { return _selectedRfqItem; }
            set { _selectedRfqItem = value; RaisePropertyChanged("SelectedRFQItem"); }
        }

        public string Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged("Size"); }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public RFSHeaderItem(IEnumerable<GenericViewModel> itemsList)
        {
            InitiateCommands();
            ObjStatus = new ObjectStatus { IsNew = true };
        }

        #endregion [ Constructor(s) ]

        #region [ Commands ]

        private CommandsExecuter _addImageCommand;

        private CommandsExecuter _deleteItemCommand;

        private CommandsExecuter _getItemCommand;

        private CommandsExecuter _itemSavingCommand;

        private ObjectStatus _objStatus;

        private int? _parentId;

        public CommandsExecuter AddImageCommand
        {
            get { return _addImageCommand; }
            set { _addImageCommand = value; RaisePropertyChanged("AddImageCommand"); }
        }

        public CommandsExecuter DeleteItemCommand
        {
            get { return _deleteItemCommand ?? (_deleteItemCommand = new CommandsExecuter(OnDeleteDetailtemHeader) { IsEnabled = true }); }
        }

        public CommandsExecuter GetItemCommand
        {
            get { return _getItemCommand ?? (_getItemCommand = new CommandsExecuter(GetItem) { IsEnabled = true }); }
        }

        public CommandsExecuter ItemSavingCommand
        {
            get { return _itemSavingCommand; }
            set { _itemSavingCommand = value; RaisePropertyChanged("ItemSavingCommand"); }
        }

        #endregion [ Commands ]

        #region [ Private Methods: Initiators ]

        private void InitiateCommands()
        {
            AddImageCommand = new CommandsExecuter(AddNewImage) { IsEnabled = true };

            ItemSavingCommand = new CommandsExecuter(SaveItem) { IsEnabled = true };
        }

        #endregion [ Private Methods: Initiators ]

        #region [ Private Methods: Commands bound Methods ]

        private void AddNewImage()
        {
            byte[] temp;
            GlobalMethods.BrowseImage(out temp);
            Image = temp;
        }

        private void GetItem()
        {
            var child = new ItemsGetterChild();
            child.ItemChosen += (s, e) =>
            {
                ItemCode = e.Itm.Code;
                Description = e.Itm.Desc;
                Image = e.Itm.Image;
                SelectedRFQItem = e.Itm;
            };
            child.Show();
        }

        private void SaveItem()
        {
        }

        #endregion [ Private Methods: Commands bound Methods ]
    }
}