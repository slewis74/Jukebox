using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Slew.WinRT.Pages.Navigation
{
    public interface INavigator
    {
        void Navigate<TController>(Expression<Func<TController, ActionResult>> action) where TController : IController;
        void Navigate<TController>(Expression<Func<TController, Task<ActionResult>>> action) where TController : IController;
        
        DataActionResult<TData> GetData<TController, TData>(Expression<Func<TController, ActionResult>> action) 
            where TController : IController;

        Task<DataActionResult<TData>> GetDataAsync<TController, TData>(
            Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController;

        void Navigate(string uri);

        void SettingsNavigateBack();
    }
}