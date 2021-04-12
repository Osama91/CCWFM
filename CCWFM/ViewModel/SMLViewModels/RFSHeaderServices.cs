using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class RFSHeaderServices : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler DeleteService;

        public virtual void OnServiceDeletion()
        {
            var handler = DeleteService;
            if (handler == null) return;
            var res = MessageBox.Show(strings.DeleteConfirmationMessage, strings.Delete, MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.Cancel) return;
            handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Private Fields ]

        private byte[] _image;
        private int? _Iserial;
        private string _notes;
        private ObjectStatus _objStatus;
        private int? _parentID;
        private ObservableCollection<GenericViewModel> _rfqServices;
        private GenericViewModel _selectedRFQService;
        private string _serviceCode;
        #endregion [ Private Fields ]

        #region [ Public Properties ]

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

        [Display(ResourceType = typeof(strings), Name = "Notes")]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; RaisePropertyChanged("Notes"); }
        }

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set { _objStatus = value; RaisePropertyChanged("ObjStatus"); }
        }

        public int? ParentID
        {
            get { return _parentID; }
            set { _parentID = value; RaisePropertyChanged("ParentID"); }
        }

        public ObservableCollection<GenericViewModel> RFQServices
        {
            get { return _rfqServices; }
            set { _rfqServices = value; RaisePropertyChanged("RFQServices"); }
        }

        public GenericViewModel SelectedRFQService
        {
            get { return _selectedRFQService; }
            set { _selectedRFQService = value; RaisePropertyChanged("SelectedRFQService"); }
        }

        [Required]
        [Display(ResourceType = typeof(strings), Name = "service")]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = value; RaisePropertyChanged("ServiceCode"); }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public RFSHeaderServices(IEnumerable<GenericViewModel> servicesList)
        {
            if (ObjStatus == null)
            {
                ObjStatus = new ObjectStatus { IsNew = true };
            }
            InitiateCommands();
            RFQServices = new ObservableCollection<GenericViewModel>();
            RFQServices.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (GenericViewModel item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (GenericViewModel item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= new PropertyChangedEventHandler((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            foreach (var item in servicesList)
            {
                RFQServices.Add(item);
            }
        }

        #endregion [ Constructor(s) ]

        #region [ Commands ]

        private CommandsExecuter _addImageCommand;
        private CommandsExecuter _deleteServiceCommand;
        private CommandsExecuter _serviceSavingCommand;

        public CommandsExecuter AddImageCommand
        {
            get { return _addImageCommand; }
            set { _addImageCommand = value; RaisePropertyChanged("AddImageCommand"); }
        }

        public CommandsExecuter DeleteServiceCommand
        {
            get { return _deleteServiceCommand ?? (_deleteServiceCommand = new CommandsExecuter(DeleteServices) { IsEnabled = true }); }
        }

        public CommandsExecuter ServiceSavingCommand
        {
            get { return _serviceSavingCommand; }
            set { _serviceSavingCommand = value; RaisePropertyChanged("ServiceSavingCommand"); }
        }

        #endregion [ Commands ]

        #region [ Private Methods: Initiators ]

        private void InitiateCommands()
        {
            AddImageCommand = new CommandsExecuter(AddNewImage) { IsEnabled = true };

            ServiceSavingCommand = new CommandsExecuter(SaveServices) { IsEnabled = true };
        }

        #endregion [ Private Methods: Initiators ]

        #region [ Private Methods: Commands bound Methods ]

        private void AddNewImage()
        {
            byte[] temp;
            GlobalMethods.BrowseImage(out temp);
            Image = temp;
        }

        private void DeleteServices()
        {
            OnServiceDeletion();
        }

        private void SaveServices()
        {
        }

        #endregion [ Private Methods: Commands bound Methods ]
    }
}