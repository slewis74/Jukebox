using System;
using System.Linq.Expressions;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.Pages
{
    public interface IContollerInvoker
    {
        ActionResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;
    }
}