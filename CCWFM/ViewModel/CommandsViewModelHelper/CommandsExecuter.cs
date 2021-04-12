using System;
using System.Windows.Input;

namespace CCWFM.ViewModel.CommandsViewModelHelper
{
    public class CommandsExecuter : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _method;
        private Action _ParameterlessMethod;

        public event EventHandler CanExecuteChanged;

        //public CommandsExecuter(Action<object> method) : this(method, null) { }

        public CommandsExecuter(Action<object> method, Predicate<object> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        public CommandsExecuter(Action handler)
        {
            // Assign the method name to the handler
            _ParameterlessMethod = handler;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return IsEnabled;
            }
            return _canExecute(parameter);
        }

        public bool IsEnabled { get; set; }

        public void Execute(object parameter)
        {
            if (_method != null)
                _method.Invoke(parameter);
            if (_ParameterlessMethod != null)
                _ParameterlessMethod.Invoke();
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged != null)
                canExecuteChanged(this, e);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }
    }
}