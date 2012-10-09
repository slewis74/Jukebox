using System;
using System.Linq.Expressions;

namespace Slew.WinRT.Pages.Navigation
{
    public interface INavigator
    {
        void Navigate<TController>(Expression<Func<TController, ActionResult>> action) where TController : IController, new();
    }
}