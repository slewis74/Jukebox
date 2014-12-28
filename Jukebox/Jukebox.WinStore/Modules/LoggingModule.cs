using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Autofac;
using Serilog;

namespace Jukebox.WinStore.Modules
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Observers(logEvents => logEvents
                    .Do(le => { Debug.WriteLine(le.RenderMessage()); })
                    .Subscribe())
                .CreateLogger();
        }
    }
}