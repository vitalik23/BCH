using System.Globalization;
using BlazorComponentHeap.Components.Models.Cropper;
using BlazorComponentHeap.Components.Models.Zoom;
using BlazorComponentHeap.Components.Zoom;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Math;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Components.Cropper;

public partial class BCHCropper : IAsyncDisposable
{
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter] public string Base64Image { get; set; } = string.Empty;
    [Parameter] public CropperType CropperType { get; set; } = CropperType.MovableRectangle;
    [Parameter] public ViewMode ViewMode { get; set; } = ViewMode.Default;
    [Parameter] public string ResultFormat { get; set; } = "image/jpeg";
    [Parameter] public float Quality { get; set; } = 1.0f;
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";
    [Parameter] public float CroppedWidth { get; set; } = -1;
    [Parameter] public float Ratio { get; set; } = -1;
    [Parameter] public float MinScale { get; set; } = 2.0f;
    [Parameter] public float MaxScale { get; set; } = 6.0f;
    [Parameter] public int MinRectangleWidth { get; set; } = 50;
    [Parameter] public int MinRectangleHeight { get; set; } = 50;
    [Parameter] public bool StretchCropArea { get; set; } = false;
    [Parameter] public float ZoomFactor { get; set; } = 0.009f;
    [Parameter] public ZoomContext ZoomContext { get; set; } = new();

    private BCHZoom _bchZoom = null!;
    private bool _processingData = false;

    private readonly string _cropperId = $"_id_{Guid.NewGuid()}";
    private readonly string _imageId = $"_id_{Guid.NewGuid()}";
    private readonly string _canvasId = $"_id_{Guid.NewGuid()}";
    private readonly string _canvasHolderId = $"_id_{Guid.NewGuid()}";
    private readonly string _rectId = $"_id_{Guid.NewGuid()}";
    private readonly string _key = $"_id_{Guid.NewGuid()}";
    private int _circleSize = 0;
    private int _cropperWidth = 0;
    private int _cropperHeight = 0;

    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };
    private DotNetObjectReference<BCHCropper> _dotNetRef = null!;
    private bool _rectDragged = false;
    private bool _rectHandleDragged = false;
    private readonly Vec2 _rectPos = new();
    private readonly Vec2 _lastMousePosition = new();
    private readonly Vec2 _change = new();
    private readonly Vec2 _rectSize = new() { X = 100, Y = 100 };
    private readonly MouseEventArgs _eventObj = new();
    
    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);

        IJSUtilsService.OnResize += OnResizeAsync;
        ZoomContext.OnUpdate += StateHasChanged;
    }

    public async ValueTask DisposeAsync()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
        _dotNetRef?.Dispose();
        ZoomContext.OnUpdate -= StateHasChanged;
    }
    
    protected override void OnParametersSet()
    {
        Quality = Math.Clamp(Quality, 0.05f, 1.0f);
        
        if (MinRectangleWidth < 50) MinRectangleWidth = 50;
        if (MinRectangleHeight < 50) MinRectangleHeight = 50;
        
        if (CropperType == CropperType.Circle)
        {
            _rectSize.X = _circleSize;
            _rectSize.Y = _circleSize;

            _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
            _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnResizeAsync();

            if (StretchCropArea)
            {
                _rectSize.X = _circleSize;
                _rectSize.Y = _circleSize;

                _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
                _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
                
                StateHasChanged();
            }
        }
    }

    private async Task OnResizeAsync()
    {
        var rect = await JsUtilsService.GetBoundingClientRectAsync(_cropperId);
        _cropperWidth = (int) rect.Width;
        _cropperHeight = (int) rect.Height;
        _circleSize = Math.Min(_cropperWidth, _cropperHeight);

        if (CropperType == CropperType.Circle)
        {
            _rectSize.X = _circleSize;
            _rectSize.Y = _circleSize;

            _rectPos.X = (_cropperWidth - _circleSize) * 0.5f;
            _rectPos.Y = (_cropperHeight - _circleSize) * 0.5f;
        }
        
        if (CropperType == CropperType.FixedRectangle)
        {
            var ratio = Ratio > 0 ? Ratio : 1.0f;

            var h2 = 1.0f;
            var w2 = ratio;

            var vertScale = _cropperHeight / h2;
            var horScale = _cropperWidth / w2;
            
            var scale = Math.Min(vertScale, horScale);

            var nH2 = scale * h2;
            var nW2 = scale * w2;
            
            _rectSize.X = nW2;
            _rectSize.Y = nH2;

            _rectPos.X = (_cropperWidth - nW2) * 0.5f;
            _rectPos.Y = (_cropperHeight - nH2) * 0.5f;
        }

        StateHasChanged();
    }

    private Task OnTouchStartAsync(TouchEventArgs args, bool rectDragged)
    {
        return OnMouseDownAsync(new MouseEventArgs
        {
            PageX = args.Touches[0].PageX,
            PageY = args.Touches[0].PageY
        }, rectDragged);
    }
    
    private async Task OnMouseDownAsync(MouseEventArgs e, bool rectDragged)
    {
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "mouseup", _dotNetRef, "OnMouseLeaveUpAsync");
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "touchend", _dotNetRef, "OnMouseLeaveUpAsync");
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "mousemove", _dotNetRef, "OnMouseMove");
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _key, "touchmove", _dotNetRef, "OnTouchMove");
        
        _lastMousePosition.Set(e.PageX, e.PageY);
        _rectDragged = rectDragged;
        _rectHandleDragged = !rectDragged;
        
        StateHasChanged();
    }
    
    [JSInvokable]
    public async Task OnMouseLeaveUpAsync()
    {
        if (!_rectDragged && !_rectHandleDragged) return;
        
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "mouseup");
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "touchend");
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "mousemove");
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _key, "touchmove");
        
        _rectDragged = false;
        _rectHandleDragged = false;
        
        if (_rectSize.X < MinRectangleWidth) _rectSize.X = MinRectangleWidth;
        if (_rectSize.Y < MinRectangleHeight) _rectSize.Y = MinRectangleHeight;
        
        StateHasChanged();
    }

    [JSInvokable]
    public void OnTouchMove(TouchEventArgs args)
    {
        _eventObj.PageX = args.Touches[0].PageX;
        _eventObj.PageY = args.Touches[0].PageY;
        
        OnMouseMove(_eventObj);
    }
    
    [JSInvokable]
    public void OnMouseMove(MouseEventArgs e)
    {
        if (!_rectDragged && !_rectHandleDragged) return;
        
        _change.Set(
            e.PageX - _lastMousePosition.X,
            e.PageY - _lastMousePosition.Y
        );

        _lastMousePosition.Set(e.PageX, e.PageY);

        if (_change.X == 0 && _change.Y == 0) return;

        var valueToChange = _rectDragged ? _rectPos : _rectSize;
        valueToChange.Add(_change);

        StateHasChanged();
    }

    public async Task<string> GetBase64ResultAsync()
    {
        _processingData = true;
        StateHasChanged();
        
        var imageRect = await JsUtilsService.GetBoundingClientRectAsync(_imageId);
        var imgBounds = new Vec2 { X = (float)imageRect.OffsetWidth,Y = (float)imageRect.OffsetHeight };
            
        var base64Result = await JsRuntime.InvokeAsync<string>("bchOnCropImage", _canvasId, 
            _canvasHolderId, _imageId, ZoomContext.TopLeftPos, imgBounds, ZoomContext.AngleInRadians, 
            ZoomContext.Scale, ResultFormat, Quality, BackgroundColor, CroppedWidth, _rectPos, _rectSize);

        _processingData = false;
        StateHasChanged();
        
        return base64Result;
    }

    public void Rotate(float angleDelta)
    {
        _bchZoom.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, 0, angleDelta);
    }

    public void ScaleTo(float scale)
    {
        var scaleDelta = (scale - ZoomContext.ScaleLinear) / ZoomFactor;
        _bchZoom.Transform(_rectPos.X + _rectSize.X * 0.5f, _rectPos.Y + _rectSize.Y * 0.5f, scaleDelta, 0);
    }
}