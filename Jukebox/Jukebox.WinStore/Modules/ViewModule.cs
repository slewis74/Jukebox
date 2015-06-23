using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Autofac;
using Jukebox.WinStore.Features.MainPage;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Modules
{
    public class ViewModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(ViewModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("View"))
                .AsSelf()
                .InstancePerDependency()
                .OnActivated(c =>
                {
                    var bus = c.Context.Resolve<IPresentationBus>();
                    InjectProperties(c.Context, bus, (FrameworkElement) c.Instance);
                });
        }

        private void InjectProperties(IComponentContext context, IPresentationBus bus, UIElement uiElement)
        {
            if (uiElement == null)
                return;

            context.InjectUnsetProperties(uiElement);

            var subscriber = uiElement as IHandlePresentationEvents;
            if (subscriber != null)
            {
                bus.Subscribe(subscriber);
            }

            InjectPropertiesForChildViews(context, bus, uiElement);
        }

        private void InjectPropertiesForChildViews(IComponentContext context, IPresentationBus bus, UIElement uiElement)
        {
            var userControl = uiElement as UserControl;
            if (userControl != null)
            {
                InjectProperties(context, bus, userControl.Content);
                return;
            }

            var panel = uiElement as Panel;
            if (panel != null)
            {
                foreach (var child in panel.Children)
                {
                    InjectProperties(context, bus, child);
                }
                return;
            }

            var contentControl = uiElement as ContentControl;
            if (contentControl != null)
            {
                InjectProperties(context, bus, contentControl.Content as UIElement);
                return;
            }
        }
    }
}