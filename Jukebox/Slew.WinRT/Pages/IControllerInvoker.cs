using System;
using System.Linq.Expressions;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.Pages
{
    public interface IControllerInvoker
    {
        ControllerInvokerResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;

        ControllerInvokerResult Call(string uri);
    }

    public class ControllerInvokerResult
    {
        public string Uri { get; set; }
        public ActionResult Result { get; set; }
    }
}