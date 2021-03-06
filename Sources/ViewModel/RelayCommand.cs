using System;
using System.Windows.Input;

namespace CMScoutIntrinsic {

    class RelayCommand : ICommand {
        public RelayCommand(Action<Object> execute) : this(execute, null) {
        }

        public RelayCommand(Action<Object> execute, Predicate<Object> canExecute) {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public void Execute(Object parameter) {
            _execute(parameter);
        }

        public Boolean CanExecute(Object parameter) {
            return ( _canExecute == null || _canExecute(parameter) );
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }



        private readonly Action<Object>    _execute;
        private readonly Predicate<Object> _canExecute;
    }

}
