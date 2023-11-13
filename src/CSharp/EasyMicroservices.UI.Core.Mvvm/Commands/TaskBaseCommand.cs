﻿using EasyMicroservices.UI.Core.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyMicroservices.UI.Core.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskBaseCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// 
        /// </summary>
        protected readonly Func<object, Task> _execute = null;
        /// <summary>
        /// 
        /// </summary>
        protected readonly Func<object, bool> _canExecute = null;
        /// <summary>
        /// /
        /// </summary>
        protected readonly IBusyViewModel _busyViewModel = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public TaskBaseCommand(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="busyViewModel"></param>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public TaskBaseCommand(IBusyViewModel busyViewModel, Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _busyViewModel = busyViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter = null)
        {
            try
            {
                return _canExecute == null || _canExecute(parameter);
            }
            catch (Exception ex)
            {
                _busyViewModel?.OnError(ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            if (_busyViewModel != null)
            {
                _busyViewModel.Busy();
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
            _ = InternalExecute(parameter);
        }

        async Task InternalExecute(object parameter)
        {
            try
            {
                await _execute(parameter);
            }
            catch (Exception ex)
            {
                _busyViewModel?.OnError(ex);
            }
            finally
            {
                if (_busyViewModel != null)
                {
                    _busyViewModel.UnBusy();
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}