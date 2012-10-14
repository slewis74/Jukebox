using System;
using Slew.WinRT.PresentationBus;
using Windows.UI.ApplicationSettings;

namespace Slew.WinRT.Requests
{
    public class DisplaySettingsRequest : PresentationRequest<Type>
    {
        public DisplaySettingsRequest(Type args, SettingsPaneCommandsRequest commandsRequest) : base(args)
        {
            CommandsRequest = commandsRequest;
            MustBeHandled = true;
        }

        public SettingsPaneCommandsRequest CommandsRequest { get; set; }
    }
}