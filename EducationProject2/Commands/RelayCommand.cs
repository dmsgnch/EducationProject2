using System;
using System.Windows.Input;

namespace EducationProject2.Commands
{
    /// <summary>
    /// Universal command with synchronous execution
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        } 

        private void OnCanExecuteChanged()
        {var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatcher != null)
            {
                if (dispatcher.HasThreadAccess)
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    }).AsTask().Wait();
                }
            }
        }

        public void RaiseCanExecuteChanged() => OnCanExecuteChanged();
    }
}