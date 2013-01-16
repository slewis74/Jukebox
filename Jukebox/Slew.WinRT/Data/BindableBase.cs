﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using Slew.WinRT.Container;

namespace Slew.WinRT.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

        protected BindableBase()
        {
            PropertyInjector.Inject(() => this); 
        }

        protected BindableBase(SynchronizationContext synchronizationContext)
        {
            SynchronizationContext = synchronizationContext;
            PropertyInjector.Inject(() => this);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            NotifyChanged(new[] { propertyName });
            return true;
        }

        protected void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyChanged(new [] { propertyName });
        }

		protected void NotifyChanged<T>(params Expression<Func<T>>[] expressions)
		{
			if (PropertyChanged == null)
				return;

            var propertyNames = new List<string>();

            foreach (var expression in expressions)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null)
                {
                    var lambdaExpression = expression as LambdaExpression;
                    if (lambdaExpression != null)
                    {
                        memberExpression = lambdaExpression.Body as MemberExpression;
                    }
                }
                propertyNames.Add(memberExpression.Member.Name);
            }

            NotifyChanged(propertyNames.ToArray());
        }

        protected void NotifyChanged(params string[] propertyNames)
        {
            if (PropertyChanged == null || propertyNames == null || propertyNames.Any() == false)
                return;

            DispatchCall(
                _ =>
                {
                    foreach (var propertyName in propertyNames)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                });
        }

        protected void DispatchCall(SendOrPostCallback call)
        {
            if (SynchronizationContext.Current != SynchronizationContext)
            {
                SynchronizationContext.Post(call, null);
            }
            else
            {
                call(null);
            }
        }
    }
}
