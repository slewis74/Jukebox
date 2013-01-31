﻿using Autofac;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Modules
{
    public class PresentationBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PresentationBus>().As<IPresentationBus>().SingleInstance();
        }
    }
}