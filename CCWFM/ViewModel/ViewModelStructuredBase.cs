using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.OGViewModels.ControlsOverride;
using CCWFM.Views.OGView.SearchChildWindows;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;

namespace CCWFM.ViewModel
{
    public class ViewModelStructuredBase : ViewModelBase
    {
        public ViewModelStructuredBase(PermissionItemName screen)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                GetItemPermissions(screen.ToString());
                InitializeCommands();
            }
        }
        public override void AfterItemPermissionsCompleted()
        {
            base.AfterItemPermissionsCompleted();
            NewCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            SearchCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            PrintCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
            ExportCommand.RaiseCanExecuteChanged();
        }
        public FormMode _FormMode { get; set; }
        public GenericSearchWindow SearchWindow { get; set; }

        #region Properties
        bool isNew, hasNew, hasEdit, hasDelete;
        public bool HasNew
        {
            get { return hasNew; }
            set { hasNew = value; RaisePropertyChanged(nameof(HasNew)); }
        }
        /// <summary>
        /// متنساش الـ RaisePropertyChanged(nameof(IsNew));
        /// او ممكن تستخدم الـ base.IsNew=value;
        /// </summary>
        public virtual bool IsNew
        {
            get { return isNew; }
            set { isNew = value;
               // IsNewChanged();
            }
        }
        public void IsNewChanged()
        {
            RaisePropertyChanged(nameof(IsNew));
            SaveCommand.RaiseCanExecuteChanged();
        }
        public virtual bool HasEdit
        {
            get { return hasEdit; }
            set { hasEdit = value; RaisePropertyChanged(nameof(HasEdit)); }
        }
        
        public virtual bool HasDelete
        {
            get { return hasDelete; }
            set { hasDelete = value; RaisePropertyChanged(nameof(HasDelete)); }
        }

        #endregion

        #region Methods
        
        #endregion

        #region For Constructor
        void InitializeCommands()
        {
            NewCommand = new RelayCommand(
                () => { NewRecord(); },
                () => { return CanAddRecord(); }
                );
            EditCommand = new RelayCommand(
                () => { EditRecord(); }
                );
            SaveCommand = new RelayCommand(
                () => { if (ValidData()) SaveRecord(); },
                () => { return CanSaveRecord(); }
                );
            SearchCommand = new RelayCommand(() => {
                Search();
            });
            DeleteCommand = new RelayCommand(
                () => { DeleteRecord(); },
                () => { return CanDeleteRecord(); }
                );
            PrintCommand = new RelayCommand(() => {
                Print();
            });
            CancelCommand = new RelayCommand(() => {
                Cancel();
            });

            ExportCommand = new RelayCommand(() => {
                ExportMethod();
            });
        }

        #endregion

        #region Commands
        RelayCommand newCommand;
        public RelayCommand NewCommand
        {
            get { return newCommand; }
            private set { newCommand = value; RaisePropertyChanged(nameof(NewCommand)); }
        }
        /// <summary>
        /// Used For NewCommand just set FormMode
        /// </summary>
        public virtual void NewRecord()
        {          
            _FormMode = FormMode.Add;
        }
        public virtual bool CanAddRecord() { return AllowAdd; }

        RelayCommand editCommand;
        public RelayCommand EditCommand
        {
            get { return editCommand; }
            private set { editCommand = value; RaisePropertyChanged(nameof(EditCommand)); }
        }
        /// <summary>
        /// Used For EditCommand just set FormMode
        /// </summary>
        public virtual void EditRecord()
        {
            _FormMode = FormMode.Update;
        }

        RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get { return saveCommand; }
            private set { saveCommand = value; RaisePropertyChanged(nameof(SaveCommand)); }
        }
        /// <summary>
        /// Used For EditCommand just set FormMode
        /// </summary>
        public virtual void SaveRecord()
        {
            _FormMode = FormMode.Standby;
            //IsSaving = true;
        }
        /// <summary>
        /// For validating data rturns true override to change that
        /// </summary>
        /// <returns> true</returns>
        public virtual bool ValidData()
        {
            return true;
        }
        public virtual bool CanSaveRecord() { return (IsNew && AllowAdd) || (!IsNew && AllowUpdate); }

        RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { return searchCommand; }
            private set { searchCommand = value; RaisePropertyChanged(nameof(SearchCommand)); }
        }
        /// <summary>
        /// Used For SearchCommand just open the window and set FormMode
        /// </summary>
        public virtual void Search()
        {
            if (SearchWindow!=null)      
            SearchWindow.Show();
            _FormMode = FormMode.Search;
        }
       
        RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get { return deleteCommand; }
            private set { deleteCommand = value; RaisePropertyChanged(nameof(DeleteCommand)); }
        }
        /// <summary>
        /// Used For DeleteCommand set FormMode to Add
        /// </summary>
        public virtual void DeleteRecord()
        {
            _FormMode = FormMode.Add;
        }
        public virtual bool CanDeleteRecord() { return AllowDelete; }

        RelayCommand printCommand;
        public RelayCommand PrintCommand
        {
            get { return printCommand; }
            private set { printCommand = value; RaisePropertyChanged(nameof(PrintCommand)); }
        }

        RelayCommand _ExportCommand;
        public RelayCommand ExportCommand
        {
            get { return _ExportCommand; }
            private set { _ExportCommand = value; RaisePropertyChanged(nameof(ExportCommand)); }
        }
        public virtual void ExportMethod() { }



        /// <summary>
        /// Used For PrintCommand
        /// </summary>
        public virtual void Print() { }

        RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get { return cancelCommand; }
            private set { cancelCommand = value; RaisePropertyChanged(nameof(CancelCommand)); }
        }

        /// <summary>
        /// Used For CancelCommand set FormMode to Add
        /// </summary>
        public virtual void Cancel()
        {
            _FormMode = FormMode.Add;
        }

        #endregion
        
    }
}
