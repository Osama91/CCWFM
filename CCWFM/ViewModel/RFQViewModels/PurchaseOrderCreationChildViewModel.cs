using System;
using System.Collections;
using System.Collections.ObjectModel;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.Views.RequestForQutation;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class PurchaseOrderCreationChildViewModel : ViewModelBase
    {
        private RFQCreatePurchaseOrderChild _child;

        private ObservableCollection<RFQSubHeader> _rfqHeaderList;

        private ObservableCollection<RFQSubHeader> _selectedStyles;

        private CommandsExecuter _selectionChanged;

        private CommandsExecuter _selectStyles;

        public RFQCreatePurchaseOrderChild PurchChild
        {
            get { return _child; }
            set
            {
                _child = value;
                RaisePropertyChanged("PurchChild");
            }
        }

        public ObservableCollection<RFQSubHeader> RFQHeaderList
        {
            get { return _rfqHeaderList; }
            set { _rfqHeaderList = value; RaisePropertyChanged("RFQHeaderList"); }
        }

        public ObservableCollection<RFQSubHeader> SelectedStyles
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

        public PurchaseOrderCreationChildViewModel()
        {
            InitializeCollections();
            InitializeCommands();
        }

        public event EventHandler SubmitSelectedStyles;

        public void InitiateSearch()
        {
            PurchChild = new RFQCreatePurchaseOrderChild(this);
            PurchChild.Show();
        }

        private void InitializeCollections()
        {
            RFQHeaderList = new ObservableCollection<RFQSubHeader>();
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

            SelectedStyles = new ObservableCollection<RFQSubHeader>();
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
                SelectedStyles = new ObservableCollection<RFQSubHeader>();

            var selectedRecords = (parameter as IList);

            if (selectedRecords == null) return;
            foreach (RFQSubHeader selectedRecord in selectedRecords)
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