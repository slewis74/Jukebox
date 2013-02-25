using System.Threading;
using Slew.WinRT.Data;

namespace Slew.WinRT.ViewModels
{
    public abstract class HasPageTitleBase : BindableBase
    {
        protected HasPageTitleBase()
        {}

        protected HasPageTitleBase(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {}

        public abstract string PageTitle { get; }
    }
}