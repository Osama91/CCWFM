using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Views.OGView.SearchChildWindows;
using SilverlightCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CCWFM.ViewModel.GenericViewModels
{
    public class GenericSearchCodeNameViewModel<T> : GenericSearchViewModelBase
    {        
        public GenericSearchCodeNameViewModel()
        {
            ItemsList = new ObservableCollection<T>();
            OkCommand = new RelayCommand((o) =>
            {
                var view = (o as GenericSearchWindow);
                if (view != null)
                {
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
    }
}
