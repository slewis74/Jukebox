﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Pages
{
    public class Navigator : INavigator
    {
        private readonly IPresentationBus _presentationBus;
        private readonly IControllerFactory _controllerFactory;

        public Navigator(
            IPresentationBus presentationBus, 
            IControllerFactory controllerFactory)
        {
            _presentationBus = presentationBus;
            _controllerFactory = controllerFactory;
        }

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController, new()
        {
            var instance = _controllerFactory.Create<TController>();

            var body = (MethodCallExpression)action.Body;
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

            var pageResult = result as IPageActionResult;
            if (pageResult != null)
            {
                var canRequestNavigation = pageResult.Parameter as ICanRequestNavigation;
                if (canRequestNavigation != null)
                {
                    canRequestNavigation.Navigator = this;
                }

                _presentationBus.Publish(new NavigationRequest(new NavigationRequestEventArgs(pageResult.PageType, pageResult.Parameter)));
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

    public class PageActionResult<T> : ActionResult, IPageActionResult
    {
        public PageActionResult(object parameter)
        {
            PageType = typeof(T);
            Parameter = parameter;
        }

        public Type PageType { get; set; }
        public object Parameter { get; set; }
    }

    public class SettingsPageActionResult<T> : PageActionResult<T>
        where T : SettingsView
    {
        public SettingsPageActionResult(object parameter) : base(parameter)
        {}
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