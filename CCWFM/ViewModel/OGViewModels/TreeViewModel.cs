using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace CCWFM.ViewModel.OGViewModels
{
    public class NodeViewModel<T> : INotifyPropertyChanged
    {
        private readonly TreeViewModel<T> _parentViewModel;
        private readonly T _model;
        private readonly int _indentation;
        private readonly Func<T, IEnumerable<T>> _getChildrenFunction;
        private bool _isExpanded;

        public NodeViewModel(TreeViewModel<T> parentModel, T model, int indentation, Func<T, IEnumerable<T>> getChildrenFunction)
        {
            _parentViewModel = parentModel;
            _model = model;
            _indentation = indentation;
            _getChildrenFunction = getChildrenFunction;
        }

        public int Indentation { get { return _indentation; } }

        public double IndentationDistance { get { return _indentation * 15; } }

        public T Model { get { return _model; } }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                    RaisePropertyChanged("ArrowAngle");

                    _parentViewModel.ToggleExpanded(this);
                }
            }
        }

        public double ArrowAngle
        {
            get
            {
                if (_isExpanded)
                {
                    return 225;
                }
                return 135;
            }
        }

        public Visibility ExpanderVisibility
        {
            get
            {
                if (Children != null && Children.Any())
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public IEnumerable<T> Children
        {
            get { return _getChildrenFunction(_model); }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TreeViewModel<T>
    {
        private readonly Func<T, IEnumerable<T>> _getChildFunction;

        public TreeViewModel(T root, Func<T, IEnumerable<T>> getChildFunction)
        {
            _getChildFunction = getChildFunction;
            Nodes = new ObservableCollection<NodeViewModel<T>> { new NodeViewModel<T>(this, root, 0, getChildFunction) };
        }

        public void ToggleExpanded(NodeViewModel<T> nodeViewModel)
        {
            var index = Nodes.IndexOf(nodeViewModel);
            if (nodeViewModel.IsExpanded && nodeViewModel.Children != null)
            {
                foreach (var child in nodeViewModel.Children)
                {
                    index++;
                    Nodes.Insert(index, new NodeViewModel<T>(this, child, nodeViewModel.Indentation + 1, _getChildFunction));
                }
            }

            if (!nodeViewModel.IsExpanded)
            {
                while (Nodes.Count > index && nodeViewModel.Indentation < Nodes[index + 1].Indentation)
                {
                    Nodes.RemoveAt(index + 1);
                }
            }
        }

        public ObservableCollection<NodeViewModel<T>> Nodes
        {
            get;
            private set;
        }
    }
}