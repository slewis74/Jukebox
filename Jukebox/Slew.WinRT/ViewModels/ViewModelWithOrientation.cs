using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Slew.WinRT.Pages.Navigation;
using Windows.UI.ViewManagement;

namespace Slew.WinRT.ViewModels
{
    public abstract class ViewModelWithOrientation : CanRequestNavigationBase
    {
        protected ViewModelWithOrientation(INavigator navigator) : base(navigator)
        {
        }

        protected ViewModelWithOrientation(INavigator navigator, SynchronizationContext synchronizationContext) : base(navigator, synchronizationContext)
        {
        }

        public virtual object LandscapeViewModel
        {
            get { return this; }
        }

        public virtual object SnappedViewModel
        {
            get { return this; }
        }

        public virtual object FilledViewModel
        {
            get { return this; }
        }

        public virtual object PortraitViewModel
        {
            get { return this; }
        }

        public virtual Type LandscapeViewType
        {
            get { return DetermineViewType(LandscapeViewModel.GetType(), ApplicationViewState.FullScreenLandscape); }
        }
        public virtual Type SnappedViewType
        {
            get { return DetermineViewType(SnappedViewModel.GetType(), ApplicationViewState.Snapped); }
        }
        public virtual Type FilledViewType
        {
            get { return DetermineViewType(FilledViewModel.GetType(), ApplicationViewState.Filled); }
        }
        public virtual Type PortraitViewType
        {
            get { return DetermineViewType(PortraitViewModel.GetType(), ApplicationViewState.FullScreenPortrait); }
        }

        private Type DetermineViewType(Type viewModelType, ApplicationViewState applicationViewState)
        {
            var vmTypeName = viewModelType.Name;
            var logicalTypeName = vmTypeName.Substring(0, vmTypeName.IndexOf("ViewModel"));
            string viewTypeName;

            var exportedTypes = viewModelType.GetTypeInfo().Assembly.ExportedTypes.ToArray();

            switch (applicationViewState)
            {
                case ApplicationViewState.Filled:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Filled");
                    break;
                case ApplicationViewState.Snapped:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Snapped");
                    break;
                case ApplicationViewState.FullScreenPortrait:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Portrait");
                    break;
                case ApplicationViewState.FullScreenLandscape:
                default:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Landscape");
                    break;
            }
            viewTypeName += "View";

            var viewTypes = exportedTypes.Where(t => t.Name == viewTypeName).ToArray();

            if (viewTypes.Any() == false)
            {
                if (applicationViewState == ApplicationViewState.Filled)
                {
                    // Can't find a Filled View, so try to fall back to the Lanscape view
                    return DetermineViewType(viewModelType, ApplicationViewState.FullScreenLandscape);
                }
                if (applicationViewState == ApplicationViewState.FullScreenLandscape)
                {
                    // Can't find a Landscape View, so try to fall back to the view without any orientation specifier
                    viewTypes = exportedTypes.Where(t => t.Name == logicalTypeName + "View").ToArray();
                }

                if (viewTypes.Any() == false)
                {
                    throw new InvalidOperationException(string.Format("Unable to locate view for {0}", vmTypeName));
                }
            }
            if (viewTypes.Count() > 1)
            {
                throw new InvalidOperationException(string.Format("Ambiguous view list for {0}", vmTypeName));
            }

            return viewTypes.First();
        }

        private static string ConvertLogicalTypeToViewTypeName(string logicalTypeName, string suffixToTrim)
        {
            return logicalTypeName.EndsWith(suffixToTrim) ? logicalTypeName : logicalTypeName + suffixToTrim;
        }
    }
}