using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Models.LocalizationHelpers;
using CCWFM.Views.OGView.SearchChildWindows;
using SilverlightCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CCWFM.ViewModel.GenericViewModels
{
    public class SearchColumnModel
    {
        string propertyPath, header, filterPropertyPath, stringFormat;

        public string FilterPropertyPath { get { return filterPropertyPath; } set { filterPropertyPath = value; } }
        public string Header { get { return header; } set { header = value; } }
        public string PropertyPath { get { return propertyPath; } set { propertyPath = value; } }
        public string StringFormat { get { return stringFormat; } set { stringFormat = value; } }       
    }

    public class GenericSearchViewModelBase : ViewModelBase
    {
        string title;
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(nameof(Title)); }
        }

        public virtual void OnFilter(Os.Controls.DataGrid.Events.FilterEvent e)
        {
           
        }
        
        RelayCommand getDataCommand;
        public RelayCommand GetDataCommand
        {
            get { return getDataCommand; }
            set { getDataCommand = value; RaisePropertyChanged(nameof(GetDataCommand)); }
        }
        int itemsListCount;
        public virtual int ItemsListCount { get { return itemsListCount; } internal set { itemsListCount = value; } }
    }

    public class GenericSearchViewModel<T> : GenericSearchViewModelBase
    {
        public GenericSearchViewModel()
        {
            ItemsList = new ObservableCollection<T>();
            OkCommand = new RelayCommand((o) =>
            {
                var view = (o as GenericSearchWindow);
                if (view != null)
                {
                    //ResultItemsList.Clear();
                    ResultItemsList.Add(SelectedRow);
                    view.DialogResult = true;
                    view.Close();
                }
            });
        }
        public override int ItemsListCount
        {
            get { return (base.ItemsListCount = itemsList.Count); }
        }
        ObservableCollection<T> filteredItemsList;
        public ObservableCollection<T> FilteredItemsList
        {
            get { return filteredItemsList ?? (filteredItemsList = new ObservableCollection<T>()); }
            set { filteredItemsList = value; RaisePropertyChanged(nameof(FilteredItemsList)); }
        }

        ObservableCollection<T> itemsList;
        public ObservableCollection<T> ItemsList
        {
            get { return itemsList ?? (itemsList = new ObservableCollection<T>()); }
            set { itemsList = value; RaisePropertyChanged(nameof(ItemsList)); }
        }
        T selectedRow;
        public T SelectedRow
        {
            get { return selectedRow; }
            set
            {
                if (value != null)
                { selectedRow = value; RaisePropertyChanged(nameof(SelectedRow)); }
            }
        }

        RelayCommand okCommand;
        public RelayCommand OkCommand
        {
            get { return okCommand; }
            set { okCommand = value; RaisePropertyChanged(nameof(OkCommand)); }
        }

        object getDataParameter;
        public object GetDataParameter
        {
            get { return getDataParameter; }
            set { getDataParameter = value; RaisePropertyChanged(nameof(GetDataParameter)); }
        }

        public override void OnFilter(Os.Controls.DataGrid.Events.FilterEvent e)
        {
            base.OnFilter(e);
            ItemsList.Clear();// هنفضى الريكوردات
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            Filter = filter;//الفيلتر موجود فى البيز
            ValuesObjects = valueObjecttemp;// ده كمان فى البيز
            if (GetDataCommand.CanExecute(GetDataParameter))
                GetDataCommand.Execute(GetDataParameter);
        }

        ObservableCollection<T> resultItemsList;
        public ObservableCollection<T> ResultItemsList
        {
            get { return resultItemsList ?? (resultItemsList = new ObservableCollection<T>()); }
            set { resultItemsList = value; RaisePropertyChanged(nameof(ResultItemsList)); }
        }
        
        public void SearchLookup(ObservableCollection<T> searchlist, T selectedRecord,
         RelayCommand getData, string title, Models.LookupItemModel Columns,
         RelayCommand getSelected = null)
        {
            if (Columns == null)
                throw new ArgumentException(nameof(Columns));
            GenericSearchWindow SearchWindow = new GenericSearchWindow(new ObservableCollection<SearchColumnModel>()
                {
                    new SearchColumnModel()
                    {
                        Header = strings.Code,
                        PropertyPath = Columns.CodePath,
                    },new SearchColumnModel()
                    {
                        Header = strings.EnglishName,
                        PropertyPath =Columns.NameEnPath,
                    },
                });
            this.Title = title;
            this.FilteredItemsList = searchlist;
            this.ItemsList = searchlist;
            this.ResultItemsList.CollectionChanged += (s, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    selectedRecord = this.ResultItemsList[e.NewStartingIndex];
                if (getSelected != null)
                    if (getSelected.CanExecute(null)) getSelected.Execute(null);
            };
            this.GetDataCommand = getData;
            SearchWindow.DataContext = this;
            SearchWindow.Show();
        }
    }
}
