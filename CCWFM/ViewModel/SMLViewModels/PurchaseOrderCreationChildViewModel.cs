using System;
using System.Collections;
using System.Collections.ObjectModel;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.RFQViewModels;
using CCWFM.Views.StylePages;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class NewRFQ_PurchaseOrderCreationChildViewModel : ViewModelBase
    {
        private NewRFQCreatePurchaseOrderChild _child;

        private ObservableCollection<SMLDTO> _rfqHeaderList;

        private ObservableCollection<SMLDTO> _selectedStyles;

        private CommandsExecuter _selectionChanged;

        private CommandsExecuter _selectStyles;

        public NewRFQCreatePurchaseOrderChild PurchChild
        {
            get { return _child; }
            set
            {
                _child = value;
                RaisePropertyChanged("PurchChild");
            }
        }

        public ObservableCollection<SMLDTO> RFQHeaderList
        {
            get { return _rfqHeaderList; }
            set { _rfqHeaderList = value; RaisePropertyChanged("RFQHeaderList"); }
        }

        public ObservableCollection<SMLDTO> SelectedStyles
        {
            get { return _selectedStyles; }
            set
            {
                _selectedStyles = value;
                RaisePropertyChanged("SelectedStyle");
            }
        }

        public CommandsExecuter SelectionChanged
        {
            get { return _selectionChanged; }
            private set
            {
                _selectionChanged = value;
                RaisePropertyChanged("SelectionChanged");
            }
        }

        public CommandsExecuter SelectStylesCommand
        {
            get { return _selectStyles; }
            private set
            {
                _selectStyles = value;
                RaisePropertyChanged("SelectStylesCommand");
            }
        }

        public NewRFQ_PurchaseOrderCreationChildViewModel()
        {
            InitializeCollections();
            InitializeCommands();
        }

        public event EventHandler SubmitSelectedStyles;

        public void InitiateSearch()
        {
            PurchChild = new NewRFQCreatePurchaseOrderChild(this);
            PurchChild.Show();
        }

        private void InitializeCollections()
        {
            RFQHeaderList = new ObservableCollection<SMLDTO>();
            RFQHeaderList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFQSubHeader item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (RFQSubHeader item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };

            SelectedStyles = new ObservableCollection<SMLDTO>();
            SelectedStyles.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (RFQSubHeader item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (RFQSubHeader item in e.OldItems)
                    {
                        item.PropertyChanged
                            -= ((s1, e1) => RaisePropertyChanged(e1.PropertyName));
                    }
            };
        }

        private void InitializeCommands()
        {
            SelectionChanged = new CommandsExecuter(OnSelectionChanged, SelectionChangedCanExecute) { IsEnabled = true };
            SelectStylesCommand = new CommandsExecuter(SelectStylesFunc) { IsEnabled = true };
        }

        private void OnSelectionChanged(object parameter)
        {
            if (SelectedStyles != null)
                SelectedStyles.Clear();
            else
                SelectedStyles = new ObservableCollection<SMLDTO>();

            var selectedRecords = (parameter as IList);

            if (selectedRecords == null) return;
            foreach (SMLDTO selectedRecord in selectedRecords)
            {
                SelectedStyles.Add(selectedRecord);
            }
        }

        private bool SelectionChangedCanExecute(object parameter)
        {
            return true;
        }

        private void SelectStylesFunc()
        {
            if (SubmitSelectedStyles == null) return;
            SubmitSelectedStyles(this, new EventArgs());
            PurchChild.DialogResult = true;
        }
    }
}