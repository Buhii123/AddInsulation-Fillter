using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InsulationPipe.Base
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExcute;
        private readonly Action<T> _execute;
        public RelayCommand(Predicate<T> canExcute, Action<T> excute)
        {
            if (excute == null) throw new ArgumentNullException("excute");
            _canExcute = canExcute;
            _execute = excute;
        }


        public bool CanExecute(object parameter)
        {
            return _canExcute == null ? true : _canExcute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
