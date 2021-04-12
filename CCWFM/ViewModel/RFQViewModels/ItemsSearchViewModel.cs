using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.CommandsViewModelHelper;

namespace CCWFM.ViewModel.RFQViewModels
{
    public class ItemsSearchViewModel : ViewModelBase
    {
        public event EventHandler SearchEnded;

        protected virtual void OnSearchEnded()
        {
            var handler = SearchEnded;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #region [ Private Fields ]

        private PagedCollectionView _genericItemsList;
        private string _searchTerm;
        private string _selectedSource;
        private List<string> _sourcesList;

        #endregion [ Private Fields ]

        #region [ Public Properties ]

        private string _descSearchTerm;

        public string DescSearchTerm
        {
            get { return _descSearchTerm; }
            set
            {
                _descSearchTerm = value;
                RaisePropertyChanged("DescSearchTerm");
            }
        }

        public PagedCollectionView GenericItemsList
        {
            get { return _genericItemsList ?? (_genericItemsList = new PagedCollectionView(new List<ItemsDto>())); }
            set
            {
                _genericItemsList = value;
                RaisePropertyChanged("GenericItemsList");
            }
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { _searchTerm = value; RaisePropertyChanged("SearchTerm"); }
        }

        public ItemsDto SelectedGItem
        {
            get { return _selectedGItem; }
            set
            {
                _selectedGItem = value;
                RaisePropertyChanged("SelectedGItem");
            }
        }

        public string SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                _selectedSource = value;
                RaisePropertyChanged("SelectedSource");
            }
        }

        public List<string> SourcesList
        {
            get { return _sourcesList ?? (_sourcesList = new List<string> { "Fabrics", "Accessories", "Generic" }); }
            set { _sourcesList = value; RaisePropertyChanged("SearchTerm"); }
        }

        #endregion [ Public Properties ]

        #region [ Constructor(s) ]

        public ItemsSearchViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Client = new CRUD_ManagerServiceClient();
                Client.SearchForRfqItemsCompleted += (s, e) =>
                {
                    if (e.Error != null) return;
                    GenericItemsList = new PagedCollectionView(e.Result);
                    OnSearchEnded();
                    RaisePropertyChanged("GenericItemsList");
                };
            }
        }

        #endregion [ Constructor(s) ]

        #region [ Commands ]

        private CommandsExecuter _searchCommand;
        private ItemsDto _selectedGItem;

        public CommandsExecuter SearchCommand
        {
            get
            {
                return _searchCommand
                    ??
                    (_searchCommand = new CommandsExecuter(Search) { IsEnabled = true });
            }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void Search()
        {
            Client.SearchForRfqItemsAsync(SearchTerm, DescSearchTerm, SelectedSource);
        }

        #endregion [ Commands bound methods ]

        #region [ Internal Logic ]

        #endregion [ Internal Logic ]
    }
}