using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Os.Controls.DataGrid
{
    public class RefreshEventArgs : EventArgs
    {
        public SortDescriptionCollection SortDescriptions { get; set; }
    }

    public static class DataGridExtensions
    {
        /// <summary>
        /// Gets the list of DataGridRow objects.
        /// </summary>
        /// <param name="grid">The grid wirhrows.</param>
        /// <returns>List of rows of the grid.</returns>
        public static ICollection<DataGridRow> GetRows(this System.Windows.Controls.DataGrid grid)
        {
            var rows = new List<DataGridRow>();

            foreach (var rowItem in grid.ItemsSource)
            {
                // Ensures that all rows are loaded.
                grid.ScrollIntoView(rowItem, grid.Columns.Last());

                // Get the content of the cell.
                var el = grid.Columns.Last().GetCellContent(rowItem);

                // Retrieve the row which is parent of given element.
                var row = DataGridRow.GetRowContainingElement(el.Parent as FrameworkElement);

                // Sometimes some rows for some reason can be null.
                if (row != null)
                    if (!rows.Contains(row))
                    {
                        rows.Add(row);
                    }
            }

            return rows;
        }
    }
    public static class MyExtensions
    {
        public static object GetItemAt<T>(this ObservableCollection<T> coll, int index)
        {
            if ((index >= 0) && (index < coll.Count))
            {
                return coll[index];
            }
            return null;
        }
    }

    public class SortableCollectionView<T> : ObservableCollection<T>
    {
        public SortableCollectionView()
        {
            _currentItem = null;
            _currentPosition = -1;
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (0 == index || null == _currentItem)
            {
                _currentItem = item;
                _currentPosition = index;
            }
        }

        public virtual object GetItemAt(int index)
        {
            if ((index >= 0) && (index < Count))
            {
                return this[index];
            }
            return null;
        }

        #region ICollectionView Members

        public bool CanFilter
        {
            get { return false; }
        }

        public bool CanGroup
        {
            get { return true; }
        }

        public bool CanSort
        {
            get { return true; }
        }

        public bool Contains(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }
            return Contains((T)item);
        }

        private bool IsValidType(object item)
        {
            return item is T;
        }

        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                if (!(Equals(_culture, value)))
                {
                    _culture = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Culture"));
                }
            }
        }

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        private object _currentItem;

        public object CurrentItem
        {
            get { return _currentItem; }
        }

        private int _currentPosition;

        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
        }

        public IDisposable DeferRefresh()
        {
            return new DeferRefreshHelper(Refresh);
        }

        private class DeferRefreshHelper : IDisposable
        {
            private readonly Action _callback;

            public DeferRefreshHelper(Action callback)
            {
                _callback = callback;
            }

            public void Dispose()
            {
                _callback();
            }
        }

        public Predicate<object> Filter { get; set; }

        public bool IsCurrentAfterLast
        {
            get
            {
                if (!IsEmpty)
                {
                    return (CurrentPosition >= Count);
                }
                return true;
            }
        }

        public bool IsCurrentBeforeFirst
        {
            get
            {
                if (!IsEmpty)
                {
                    return (CurrentPosition < 0);
                }
                return true;
            }
        }

        protected bool IsCurrentInSync
        {
            get
            {
                if (IsCurrentInView)
                {
                    return (GetItemAt(CurrentPosition) == CurrentItem);
                }
                return (CurrentItem == null);
            }
        }

        private bool IsCurrentInView
        {
            get
            {
                return ((0 <= CurrentPosition) && (CurrentPosition < Count));
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (Count == 0);
            }
        }

        public bool MoveCurrentTo(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }
            if (Equals(CurrentItem, item) && ((item != null) || IsCurrentInView))
            {
                return IsCurrentInView;
            }
            int index = IndexOf((T)item);
            return MoveCurrentToPosition(index);
        }

        public bool MoveCurrentToFirst()
        {
            return MoveCurrentToPosition(0);
        }

        public bool MoveCurrentToLast()
        {
            return MoveCurrentToPosition(Count - 1);
        }

        public bool MoveCurrentToNext()
        {
            return ((CurrentPosition < Count) && MoveCurrentToPosition(CurrentPosition + 1));
        }

        public bool MoveCurrentToPrevious()
        {
            return ((CurrentPosition >= 0) && MoveCurrentToPosition(CurrentPosition - 1));
        }

        public bool MoveCurrentToPosition(int position)
        {
            if ((position < -1) || (position > Count))
            {
                throw new ArgumentOutOfRangeException("position");
            }
            if (((position != CurrentPosition) || !IsCurrentInSync) && OkToChangeCurrent())
            {
                ChangeCurrentToPosition(position);
                OnCurrentChanged();
                OnPropertyChanged("CurrentPosition");
                OnPropertyChanged("CurrentItem");
            }
            return IsCurrentInView;
        }

        private void ChangeCurrentToPosition(int position)
        {
            if (position < 0)
            {
                _currentItem = null;
                _currentPosition = -1;
            }
            else if (position >= Count)
            {
                _currentItem = null;
                _currentPosition = Count;
            }
            else
            {
                _currentItem = this[position];
                _currentPosition = position;
            }
        }

        protected bool OkToChangeCurrent()
        {
            var args = new CurrentChangingEventArgs();
            OnCurrentChanging(args);
            return !args.Cancel;
        }

        protected virtual void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (CurrentChanging != null)
            {
                CurrentChanging(this, args);
            }
        }

        protected void OnCurrentChanging()
        {
            _currentPosition = -1;
            OnCurrentChanging(new CurrentChangingEventArgs(false));
        }

        protected override void ClearItems()
        {
            OnCurrentChanging();
            base.ClearItems();
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<RefreshEventArgs> OnRefresh;

        public void Refresh()
        {
            // sort and refersh
            if (null != OnRefresh)
            {
                OnRefresh(this, new RefreshEventArgs { SortDescriptions = SortDescriptions });
            }
            //this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private CustomSortDescriptionCollection _sort;

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (_sort == null)
                {
                    SetSortDescriptions(new CustomSortDescriptionCollection());
                }
                return _sort;
            }
        }

        private void SetSortDescriptions(CustomSortDescriptionCollection descriptions)
        {
            _sort = descriptions;
        }

        public IEnumerable SourceCollection
        {
            get
            {
                return this;
            }
        }

        #endregion ICollectionView Members
    }

    public class CustomSortDescriptionCollection : SortDescriptionCollection
    {
        public event NotifyCollectionChangedEventHandler MyCollectionChanged
        {
            add
            {
                CollectionChanged += value;
            }
            remove
            {
                CollectionChanged -= value;
            }
        }
    }
}