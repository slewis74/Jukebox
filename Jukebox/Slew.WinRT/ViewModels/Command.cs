using System;
using System.Windows.Input;
using Slew.WinRT.Container;

namespace Slew.WinRT.ViewModels
{
	public abstract class Command : ICommand
	{
		public event EventHandler CanExecuteChanged;

        protected Command()
        {
            PropertyInjector.Inject(() => this);
        }

		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		public abstract void Execute(object parameter);

		protected void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged == null)
				return;
			CanExecuteChanged(this, EventArgs.Empty);
		}
	}

	public abstract class Command<T> : Command
	{
	    public virtual bool CanExecute(T parameter)
		{
			return true;
		}

		public override bool CanExecute(object parameter)
		{
			return CanExecute((T)parameter);
		}

		public override void Execute(object parameter)
		{
			Execute((T)parameter);
		}

		public abstract void Execute(T parameter);
	}
}