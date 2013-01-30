using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Pages.Settings
{
    public interface ISettingsManager : IHandlePresentationEvents
    {
        void Add<TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController, new();

        void Add<TView, TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController, new();

        IEnumerable<SettingsViewConfig> GetGlobalSettings();
        IEnumerable<SettingsViewConfig> GetViewSettings<TView>();
    }
}