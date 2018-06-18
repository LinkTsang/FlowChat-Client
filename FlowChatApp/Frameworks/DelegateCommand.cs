using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlowChatApp.Frameworks
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        { }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> execute, Func<bool> canExecute) : base(execute, canExecute)
        {
        }
        public DelegateCommand(Action<object> execute) : base(execute)
        {
        }
    }
}
