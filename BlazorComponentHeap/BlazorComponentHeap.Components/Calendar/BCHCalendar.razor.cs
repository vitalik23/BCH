using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Math;

namespace BlazorComponentHeap.Components.Calendar;

public partial class BCHCalendar : IAsyncDisposable
{
    [Inject] private ISubscriptionService SubscriptionService { get; set; } = null!;

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string Format { get; set; } = string.Empty;
    [Parameter] public bool DateRange { get; set; }
    [Parameter] public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    [Parameter] public EventCallback<DateTime> ValueChanged { get; set; }
    [Parameter] public DateTime Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<List<DateTime>> ValuesChanged { get; set; }
    [Parameter] public List<DateTime> Values
    {
        get => _values;
        set
        {
            if (_values == value) return;
            _values = value;

            ValuesChanged.InvokeAsync(value);
        }
    }

    private DateTime _value = DateTime.Now;
    private List<DateTime> _values = new();
    private bool _showDate = false;
    private bool _showMonth = false;
    private DotNetObjectReference<BCHCalendar> _dotNetRef = null!;
    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarDaysId = $"_id_{Guid.NewGuid()}";
    private readonly string _calendarMonthsId = $"_id_{Guid.NewGuid()}";
    private readonly string _yearsSelectContentId = $"_id_{Guid.NewGuid()}";
    private readonly string _inputId = $"_id_{Guid.NewGuid()}";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private ElementReference _inputRef;
    private CultureInfo _culture = null!;
    private Vec2 _containerPos = new ();
    private NumberFormatInfo _nF = new () { NumberDecimalSeparator = "." };

    private int _selectedYear;
    private int _selectedMonth;

    protected override async Task OnInitializedAsync()
    {
        SubscriptionService.OnUpdate += StateHasChanged;

        if (!SubscriptionService.IsSubscriptionActivated) return;

        _culture = new CultureInfo(Culture);
        Format = string.IsNullOrWhiteSpace(Format) ? _culture.DateTimeFormat.ShortDatePattern : Format;
        
        _dotNetRef = DotNetObjectReference.Create(this);
        await JsRuntime.InvokeVoidAsync("addDocumentListener", _subscriptionKey, "mousedown", _dotNetRef, "OnDocumentMouseDownAsync");
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        SubscriptionService.OnUpdate -= StateHasChanged;
        
        await JsRuntime.InvokeVoidAsync("removeDocumentListener", _subscriptionKey, "mousedown");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!SubscriptionService.IsSubscriptionActivated) return;
        if (_showDate || _showMonth) await _inputRef.FocusAsync();
    }

    [JSInvokable] public Task OnDocumentMouseDownAsync(ExtMouseEventArgs e)
    {
        var container = e.PathCoordinates
            .FirstOrDefault(x => 
                x.Id == _containerId || x.Id == _calendarDaysId || 
                x.Id == _calendarMonthsId || x.Id == _yearsSelectContentId);

        if (container != null) return Task.CompletedTask; // inside calendar

        if (_showMonth)
        {
            _showMonth = false;
            _showDate = true;
            StateHasChanged();
            return Task.CompletedTask;
        }
        
        _showDate = false;
        _showMonth = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task OnCalendarClickedAsync()
    {
        var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
        _containerPos.Set(containerRect.X, containerRect.Y);
        
        if (Value.Year != _selectedYear) _selectedYear = Value.Year;
        if (Value.Month != _selectedMonth) _selectedMonth = Value.Month;
        
        _showDate = !_showMonth && !_showDate;
        _showMonth = false;
        
        StateHasChanged();
    }

    private async Task OnShowModeChangedAsync()
    {
        if (_showDate || _showMonth)
        {
            var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
            _containerPos.Set(containerRect.X, containerRect.Y);
        }
        
        StateHasChanged();
    }
}