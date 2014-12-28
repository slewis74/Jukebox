﻿using Autofac;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Modules
{
    public class PresentationBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PresentationBus>().As<IPresentationBus>().SingleInstance();
        }
    }
}