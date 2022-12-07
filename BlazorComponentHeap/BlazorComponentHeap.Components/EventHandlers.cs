using BlazorComponentHeap.Shared.Models.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Components;

// https://asp.net-hacker.rocks/2021/05/04/aspnetcore6-07-custom-event-arguments.html
// https://stackoverflow.com/questions/63265941/blazor-3-1-nested-onmouseover-events

[EventHandler("onextdragenter", typeof(MouseEventArgs), true, true)]
[EventHandler("onextdragstart", typeof(MouseStartEventArgs), true, true)]
[EventHandler("onextdragend", typeof(MouseEventArgs), true, true)]
[EventHandler("onextscroll", typeof(ScrollEventArgs), true, true)]
[EventHandler("onextmousewheel", typeof(ExtWheelEventArgs), true, true)]
[EventHandler("onmouseleave", typeof(ExtMouseEventArgs), true, true)]
public static class EventHandlers
{
}
