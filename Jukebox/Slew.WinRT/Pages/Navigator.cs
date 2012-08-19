using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Pages
{
    public class Navigator : INavigator
    {
        private readonly IControllerFactory _controllerFactory;
        private readonly ICanHandleNavigation _canHandleNavigation;

        public Navigator(IControllerFactory controllerFactory, ICanHandleNavigation canHandleNavigation)
        {
            _controllerFactory = controllerFactory;
            _canHandleNavigation = canHandleNavigation;
        }

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController, new()
        {
            var instance = _controllerFactory.Create<TController>();

            var body = action.Body as MethodCallExpression;
            var parameterValues = new List<object>();
            var parameters = body.Method.GetParameters();
            var arguments = body.Arguments;
            for (var i = 0; i < parameters.Length && i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var lambda = Expression.Lambda<Func<TController, object>>(Expression.Convert(argument, typeof(object)), action.Parameters.ToList());
                var compiled = lambda.Compile();
                var value = compiled(default(TController));
                
                parameterValues.Add(value);
            }

            var result = (ActionResult)body.Method.Invoke(instance, parameterValues.ToArray());

            var pageResult = result as PageActionResult;
            if (pageResult != null)
            {
                if (pageResult.Parameter is ICanRequestNavigation)
                {
                    ((ICanRequestNavigation) pageResult.Parameter).Navigator = this;
                }

                _canHandleNavigation.Navigate(pageResult.PageType, pageResult.Parameter);
            }
        }
    }

    public interface INavigator
    {
        void Navigate<TController>(Expression<Func<TController, ActionResult>> action) where TController : IController, new();
    }

    public interface IController
    {
        INavigator Navigator { get; set; }
    }

    public class Controller : IController
    {
        public INavigator Navigator { get; set; }
    }

    public abstract class ActionResult
    {}

    public class PageActionResult : ActionResult
    {
        public PageActionResult(Type pageType, object parameter)
        {
            PageType = pageType;
            Parameter = parameter;
        }

        public Type PageType { get; set; }
        public object Parameter { get; set; }
    }

    public interface IControllerFactory
    {
        TController Create<TController>() where TController : IController, new();
    }

    public class ControllerFactory : IControllerFactory
    {
        public virtual TController Create<TController>() where TController : IController, new()
        {
            var controller = new TController();
            return controller;
        }
    }
}