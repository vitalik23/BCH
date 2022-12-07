using BlazorComponentHeap.Components.Cropper;
using BlazorComponentHeap.Components.Models.Cropper;
using BlazorComponentHeap.Components.Models.Zoom;
using BlazorComponentHeap.Components.Range;
using BlazorComponentHeap.Components.Zoom;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Components.ImageCropper;

public partial class BCHImageCropper : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public string Base64Image { get; set; } = string.Empty;
    [Parameter] public float MinScale { get; set; } = 0.1f;
    [Parameter] public float MaxScale { get; set; } = 8.0f;
    [Parameter] public bool IsMobile { get; set; } = false;

    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private BCHCropper _bchCropper = null!;
    private BCHRange _bchRange = null!;
    private DotNetObjectReference<BCHImageCropper> _dotNetRef = null!;
    private ZoomContext _zoomContext = new();

    private bool _rotateCropper = false;

    protected override async Task OnInitializedAsync()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _zoomContext.OnUpdate += OnUpdateCropper;
        
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "mouseup", _dotNetRef, "OnMouseUp");
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "touchend", _dotNetRef, "OnMouseUp");
    }

    [JSInvokable]
    public void OnMouseUp()
    {
        _rotateCropper = false;
    }

    private async Task OnMouseDownAsync(float angleDelta)
    {
        _rotateCropper = true;
        await RotateAsync(angleDelta);
    }

    private async Task RotateAsync(float angleDelta)
    {
        if (!_rotateCropper) return;
        await Task.Delay(15);
        _bchCropper.Rotate(angleDelta);

        await RotateAsync(angleDelta);
    }

    private void OnRangeMove(double value)
    {
        // Console.WriteLine($"OnRangeMove = {value}");
        _bchCropper.ScaleTo((float) value);
    }

    private void OnUpdateCropper()
    {
        _bchRange.SetValue(_zoomContext.ScaleLinear);
    }
    
    public async ValueTask DisposeAsync()
    {
        _zoomContext.OnUpdate -= OnUpdateCropper;
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "mouseup");
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "touchend");
        _dotNetRef?.Dispose();
    }
}