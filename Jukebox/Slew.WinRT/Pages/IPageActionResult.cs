using System;

namespace Slew.WinRT.Pages
{
    public interface IPageActionResult
    {
        Type PageType { get; set; }
        object Parameter { get; set; }
    }
}