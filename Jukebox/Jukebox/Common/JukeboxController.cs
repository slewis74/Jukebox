using System;
using Slew.WinRT.Container;
using Slew.WinRT.Pages;

namespace Jukebox.Common
{
    public class JukeboxController : Controller
    {
        protected T Resolve<T>(Func<T> objectCreationAction)
        {
            return PropertyInjector.Resolve(objectCreationAction);
        }
    }
}