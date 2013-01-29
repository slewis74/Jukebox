using System;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Requests
{
    public class ViewModelNavigationRequestEventArgs : EventArgs
    {
        public ViewModelNavigationRequestEventArgs(ViewModelWithOrientation viewModel, string target = null)
        {
            ViewModel = viewModel;
            Target = target;
        }

        public ViewModelWithOrientation ViewModel { get; set; }
        public string Target { get; set; }
    }
}