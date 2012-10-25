﻿using Jukebox.Events;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.Settings.Player
{
    public class PlayerSettingsViewModel : BindableBase,
        IInitializeAfterPropertyInjection,
        IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        private bool _isRandomPlayMode;
        public bool IsRandomPlayMode
        {
            get { return _isRandomPlayMode; }
            set
            {
                if (SetProperty(ref _isRandomPlayMode, value))
                {
                    PresentationBus.Publish(new RandomPlayModeChangedEvent(_isRandomPlayMode));
                }
            }
        }

        public void Initialize()
        {
            var request = new IsRandomPlayModeRequest();
            PresentationBus.Publish(request);
            _isRandomPlayMode = request.IsRandomPlayMode;
        }
    }
}