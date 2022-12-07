using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentHeap.Shared.Models.Events;

public class ExtMouseEventArgs : MouseEventArgs
{
    public List<CoordsHolder> PathCoordinates { get; set; } = new();

    public double ClientWidth { get; set; }
    public double ClientHeight { get; set; }
    public string TargetClassList { get; set; } = string.Empty;
    public string RelatedTargetClassList { get; set; } = string.Empty;
    public bool RelatedTargetIsChildOfTarget { get; set; }
}