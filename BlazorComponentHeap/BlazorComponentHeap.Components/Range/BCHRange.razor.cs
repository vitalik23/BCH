using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorComponentHeap.Components.Range;

public partial class BCHRange : IDisposable
{
    [Inject] private IJSUtilsService _jSUtilsService { get; set; } = null!;
    [Inject] private IJSRuntime _jSRuntime { get; set; } = null!;

    [Parameter] public int Step { get; set; }
    [Parameter] public int AfterPointCountNumber { get; set; }
    [Parameter] public double Min { get; set; }
    [Parameter] public double Max { get; set; }
    [Parameter] public bool ShowTooltip { get; set; } = true;
    [Parameter] public bool Vertical { get; set; }
    [Parameter] public bool ShowFillColor { get; set; }
    [Parameter] public string TypeValue { get; set; } = string.Empty;
    [Parameter] public EventCallback<double> ValueChanged { get; set; }
    [Parameter]
    public double Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            var points = Max - Min;

            if (Vertical)
            {
                _offsetY = ((_value - Min) * _containerHeight) / points;
            }
            else
            {
                _offsetX = ((_value - Min) * _containerWidth) / points;
            }

            ValueChanged.InvokeAsync(value);
        }
    }

    private string _circleId = $"_id{Guid.NewGuid()}";
    private string _containerId = $"_id{Guid.NewGuid()}";

    private DotNetObjectReference<BCHRange> _dotNetRef = null!;
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private double _offsetX = 0;
    private double _offsetY = 0;
    private bool _mouseDown;
    private BoundingClientRect _thumb = new();
    private BoundingClientRect _container = new();
    private double _thumbHeight;
    private double _thumbWidth;
    private bool _showTooltip;
    private double _value;
    private int _stepInPixels;
    private bool _firstRender;
    private double _widthStep;
    private double _heightStep;
    private double _containerWidth => _container.Width - _thumb.Width;
    private double _containerHeight => _container.Height - _thumb.Height;

    private double _prevPageY;
    private double _prevPageX;

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        IJSUtilsService.OnResize += OnResizeAsync;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _firstRender = firstRender;

        if (firstRender)
        {
            await OnUpdateAsync();

            var points = Max - Min;

            if (Vertical)
            {
                _offsetY = ((_value - Min) * _containerHeight) / points;
            }
            else
            {
                _offsetX = ((_value - Min) * _containerWidth) / points;
            }

        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_firstRender)
        {
            await OnUpdateAsync();
        }
    }

    #region Gorizontal

    private void OnContainerGorizontalMouseDown(MouseEventArgs mouseEvent)
    {
        //_offsetX = GetOffsetX(mouseEvent.ClientX, (_shiftX + _thumbWidth), _container.Left);
        //UpdateValue();
    }

    private void OnContainerGorizontalTouchDown(TouchEventArgs mouseEvent)
    {
        //_offsetX = GetOffsetX(mouseEvent.Touches[0].ClientX, (_shiftX + _thumbWidth), _container.Left);
        //UpdateValue();
    }

    private async Task OnGorizontalMouseDownAsync(MouseEventArgs mouseEvent)
    {
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "mouseup", _dotNetRef, "OnGorizontalMouseUpAsync");
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "mousemove", _dotNetRef, "OnGorizontalMouseMoveAsync");

        _thumb = await _jSUtilsService.GetBoundingClientRectAsync(_circleId);
        _prevPageX = mouseEvent.PageX;

        _showTooltip = true;
        _mouseDown = true;
    }

    private async Task OnGorizontalTouchStartAsync(TouchEventArgs mouseEvent)
    {
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "touchend", _dotNetRef, "OnGorizontalTouchEndAsync");
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "touchmove", _dotNetRef, "OnGorizontalTouchMoveAsync");

        _thumb = await _jSUtilsService.GetBoundingClientRectAsync(_circleId);
        _prevPageX = mouseEvent.Touches[0].PageX;

        _showTooltip = true;
        _mouseDown = true;
    }

    [JSInvokable]
    public async Task OnGorizontalMouseUpAsync()
    {
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "mouseup");
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "mousemove");

        _showTooltip = false;
        _mouseDown = false;

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnGorizontalMouseMoveAsync(MouseEventArgs mouseEvent)
    {
        if (!_mouseDown) return;

        var dX = mouseEvent.PageX - _prevPageX;

        _prevPageX = mouseEvent.PageX;
        _offsetX += dX;
        _offsetX = Math.Clamp(_offsetX, 0, _container.Width - _thumb.Width);

        UpdateValue();
    }

    [JSInvokable]
    public async Task OnGorizontalTouchEndAsync()
    {
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "touchend");
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "touchmove");

        _showTooltip = false;
        _mouseDown = false;

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnGorizontalTouchMoveAsync(TouchEventArgs touchEvent)
    {
        if (!_mouseDown) return;

        if (touchEvent.Touches.Length == 1)
        {
            var dX = touchEvent.Touches[0].PageX - _prevPageX;

            _prevPageX = touchEvent.Touches[0].PageX;
            _offsetX += dX;
            _offsetX = Math.Clamp(_offsetX, 0, _container.Width - _thumb.Width);
            UpdateValue();
            return;
        }

        await OnGorizontalTouchEndAsync();
    }

    private string GetLeftOffset()
    {
        return (((_value - Min) / (Max - Min)) * _containerWidth).ToString(_nF);
    }

    private string GetFillOffsetX()
    {
        return ((((_value - Min) / (Max - Min)) * _containerWidth) + (_thumbWidth / 2)).ToString(_nF);
    }

    #endregion

    #region Vertical

    private void OnContainerVerticalMouseDown(MouseEventArgs mouseEvent)
    {
        //_offsetY = GetOffsetY(mouseEvent.ClientY, (_shiftY + _thumbHeight), _container.Bottom);
        //UpdateValue();
    }

    private void OnContainerVerticalTouchDown(TouchEventArgs mouseEvent)
    {
        //_offsetY = GetOffsetY(mouseEvent.Touches[0].ClientY, (_shiftY + _thumbHeight), _container.Bottom);
        //UpdateValue();
    }

    private async Task OnVerticalMouseDownAsync(MouseEventArgs mouseEvent)
    {
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "mouseup", _dotNetRef, "OnVerticalMouseUpAsync");
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "mousemove", _dotNetRef, "OnVerticalMouseMoveAsync");

        _thumb = await _jSUtilsService.GetBoundingClientRectAsync(_circleId);
        _prevPageY = mouseEvent.PageY;

        _showTooltip = true;
        _mouseDown = true;
    }

    private async Task OnVerticalTouchStartAsync(TouchEventArgs touchEvent)
    {
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "touchend", _dotNetRef, "OnVerticalTouchEndAsync");
        await _jSRuntime.InvokeVoidAsync("addDocumentListener", _containerId, "touchmove", _dotNetRef, "OnVerticalTouchMoveAsync");

        _thumb = await _jSUtilsService.GetBoundingClientRectAsync(_circleId);
        _prevPageY = touchEvent.Touches[0].PageY;

        _showTooltip = true;
        _mouseDown = true;
    }

    [JSInvokable]
    public async Task OnVerticalMouseUpAsync()
    {
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "mouseup");
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "mousemove");

        _showTooltip = false;
        _mouseDown = false;

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnVerticalMouseMoveAsync(MouseEventArgs mouseEvent)
    {
        if (!_mouseDown) return;

        var dY = _prevPageY - mouseEvent.PageY;

        _prevPageY = mouseEvent.PageY;
        _offsetY += dY;
        _offsetY = Math.Clamp(_offsetY, 0, _container.Height - _thumb.Height);
        UpdateValue();
    }

    [JSInvokable]
    public async Task OnVerticalTouchEndAsync()
    {
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "touchend");
        await _jSRuntime.InvokeVoidAsync("removeDocumentListener", _containerId, "touchmove");

        _showTooltip = false;
        _mouseDown = false;

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnVerticalTouchMoveAsync(TouchEventArgs touchEvent)
    {
        if (!_mouseDown) return;

        if (touchEvent.Touches.Length == 1)
        {
            var dY = _prevPageY - touchEvent.Touches[0].PageY;

            _prevPageY = touchEvent.Touches[0].PageY;
            _offsetY += dY;
            _offsetY = Math.Clamp(_offsetY, 0, _container.Height - _thumb.Height);
            UpdateValue();
            return;
        }

        await OnVerticalTouchEndAsync();
    }

    private string GetBottomOffset()
    {
        var bottomOffset = (_containerHeight - (((_value - Min) / (Max - Min)) * _containerHeight)).ToString(_nF);
        return bottomOffset;
    }

    #endregion

    private void UpdateValue()
    {
        if (Step > 0)
        {
            int stepIndex = 0;
            if (Vertical)
            {
                stepIndex = (int)((((_offsetY + _heightStep * 0.5) / _containerHeight) * (Max - Min)) / Step);
            }
            else
            {
                stepIndex = (int)((((_offsetX + _widthStep * 0.5) / _containerWidth) * (Max - Min)) / Step);
            }

            _value = Step * stepIndex;
            ValueChanged.InvokeAsync(_value);
            StateHasChanged();
            return;
        }

        var points = Max - Min;

        _value = Vertical ? ((_offsetY / _containerHeight) * points) + Min : ((_offsetX / _containerWidth) * points) + Min;
        ValueChanged.InvokeAsync(_value);

        StateHasChanged();
    }

    private async Task OnUpdateAsync()
    {
        _container = await _jSUtilsService.GetBoundingClientRectAsync(_containerId);
        _thumb = await _jSUtilsService.GetBoundingClientRectAsync(_circleId);
        _thumbHeight = _thumb.Height;
        _thumbWidth = _thumb.Width;

        if (Step > 0)
        {
            var stepCount = (Max - Min) / Step;
            _widthStep = _containerWidth / stepCount;
            _heightStep = _containerHeight / stepCount;
        }

        StateHasChanged();
    }

    private async Task OnResizeAsync()
    {
        await OnUpdateAsync();
    }

    public void Dispose()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
    }

    public void SetValue(double value)
    {
        _value = value;
        StateHasChanged();
    }
}
