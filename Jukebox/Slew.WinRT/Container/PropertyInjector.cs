using System;
using System.Collections.Generic;

namespace Slew.WinRT.Container
{
    public static class PropertyInjector
    {
        private static readonly List<IPropertyInjectorRule> Rules = new List<IPropertyInjectorRule>();

        public static void AddRule(IPropertyInjectorRule rule)
        {
            Rules.Add(rule);
        }

        public static T Inject<T>(Func<T> objectCreationAction)
        {
            var obj = objectCreationAction();

            foreach (var rule in Rules)
            {
                rule.Process(obj);
            }

            return obj;
        }
    }
}