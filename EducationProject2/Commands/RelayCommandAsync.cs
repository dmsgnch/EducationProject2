using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EducationProject2.Commands
{
    /// <summary>
    /// Universal command with asynchronous execution
    /// </summary>
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Func<bool> _canExecute;
    
        public RelayCommandAsync(Func<object, Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
    
        public event EventHandler CanExecuteChanged;
        
        private bool _isExecuting;
        private bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }
    
        public bool CanExecute(object parameter) => (_canExecute?.Invoke() ?? true) && !IsExecuting;
    
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        } 
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(null)) throw new Exception("Command cant be executed!");
            
            IsExecuting = true;
            
            try
            {
                await _execute.Invoke(parameter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Operation Error: {ex.Message}");
            }
            finally
            {
                IsExecuting = false;
            }
        }
    
        private async void OnCanExecuteChanged()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatcher != null)
            {
                if (dispatcher.HasThreadAccess)
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    });
                }
            }
        }
    
        public void RaiseCanExecuteChanged() => OnCanExecuteChanged();
    }
}