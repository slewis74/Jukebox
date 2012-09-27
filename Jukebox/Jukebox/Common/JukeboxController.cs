using System;
using Slew.WinRT.Container;
using Slew.WinRT.Pages;

namespace Jukebox.Common
{
    public class JukeboxController : Controller
    {
        protected T Inject<T>(Func<T> objectCreationAction)
        {
            return PropertyInjector.Inject(objectCreationAction);
        }
    }
}