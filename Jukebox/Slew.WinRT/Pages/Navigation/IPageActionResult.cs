using System;

namespace Slew.WinRT.Pages.Navigation
{
    public interface IPageActionResult
    {
        Type PageType { get; set; }
        object Parameter { get; set; }
    }
}