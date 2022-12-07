using System.Globalization;
using BlazorComponentHeap.Components.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Math;

namespace BlazorComponentHeap.Components.Select;

public partial class BCHSelect<TItem> : ComponentBase, IAsyncDisposable where TItem : class
{
    [Inject] private ISubscriptionService SubscriptionService { get; set; } = null!;
    [Inject] private IJSUtilsService JsUtilsService { get; set; } = null!;

    private class Element
    {
        public string Name { get; set; } = string.Empty;
        public TItem Item { get; set; } = default!;
    }

    private class Group
    {
        public bool Expanded { get; set; } = true;
        public string Name { get; set; } = string.Empty;
        public List<Element> Elements { get; set; } = new();
    }

    [Inject] private IJSRuntime _jsRuntime { get; set; } = null!;

    [Parameter] public EventCallback OnFocusOut { get; set; }
    [Parameter] public bool Grouping { get; set; } = false;
    [Parameter] public bool Filtering { get; set; } = false;
    [Parameter] public bool MultipleSelect { get; set; } = false;
    [Parameter] public bool ScrollToSelected { get; set; } = false;
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string ContentId { get; set; } = $"_id{Guid.NewGuid()}";
    [Parameter] public string NoItemsTest { get; set; } = "No items found";

    [Parameter] public int ItemHeight { get; set; } = 40;
    [Parameter] public int Height { get; set; } = 200;
    [Parameter] public int Width { get; set; } = 200;
    [Parameter] public TItem DefaultValue { get; set; } = null!;
    [Parameter] public string DefaultText { get; set; } = "Please Select";
    [Parameter] public IEnumerable<TItem> Options { get; set; } = new List<TItem>();
    [Parameter] public Func<TItem, string> ElementNamePredicate { get; set; } = null!;
    [Parameter] public Func<TItem, string> FilterByPredicate { get; set; } = null!;
    [Parameter] public Func<TItem, string> GroupNamePredicate { get; set; } = x => string.Empty;
    [Parameter] public Func<TItem, object> GroupPredicate { get; set; } = x => string.Empty;
    [Parameter] public Func<TItem, string> CssItemPredicate { get; set; } = x => string.Empty;
    [Parameter] public EventCallback<KeyboardEventArgs> OnFilterKeyDown { get; set; }
    [Parameter] public EventCallback<TItem> SelectedChanged { get; set; }
    [Parameter] public TItem Selected
    {
        get => _selectedValue;
        set
        {
            if (_selectedValue == value) return;
            _selectedValue = value;

            SelectedChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<string> FilterChanged { get; set; }
    [Parameter] public string Filter
    {
        get => _typedFilterValue;
        set
        {
            if (_typedFilterValue == value) return;
            _typedFilterValue = value;
            OnFilterType();
            FilterChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<bool> IsOpenedChanged { get; set; }
    [Parameter] public bool IsOpened
    {
        get => _isOpened;
        set
        {
            if (_isOpened == value) return;
            _isOpened = value;
            IsOpenedChanged.InvokeAsync(value);
        }
    }

    [Parameter] public IList<TItem> SelectedItems { get; set; } = new List<TItem>();
    [Parameter] public EventCallback<TItem> OnSelectItem { get; set; }
    [Parameter] public EventCallback<TItem> OnDeselectItem { get; set; }
    [Parameter] public RenderFragment<TItem> RowTemplate { get; set; } = null!;

    private readonly string _containerId = $"_id{Guid.NewGuid()}";
    private readonly string _inputId = $"_id{Guid.NewGuid()}";
    private readonly string _scrollerId = $"_id{Guid.NewGuid()}";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";

    private TItem _selectedValue = null!;
    private bool _isOpened = false;
    private ElementReference _inputRef;
    private string _typedFilterValue = string.Empty;
    private string _placeholder = "";

    private List<Group> _groups = new();

    private int _prevCount = -1;
    private DotNetObjectReference<BCHSelect<TItem>> _dotNetRef = null!;
    private bool _scrolled = false;
    private Vec2 _containerPos = new ();
    private float _contentWidth = 0;
    private NumberFormatInfo _nF = new () { NumberDecimalSeparator = "." };

    protected override async Task OnInitializedAsync()
    {
        SubscriptionService.OnUpdate += StateHasChanged;
        if (!SubscriptionService.IsSubscriptionActivated) return;
        
        _dotNetRef = DotNetObjectReference.Create(this);

        if (FilterByPredicate == null!)
        {
            FilterByPredicate = ElementNamePredicate;
        }

        if (!MultipleSelect)
        {
            Selected = DefaultValue == null! ? null! : DefaultValue;
        }

        _placeholder = Selected == null! ? DefaultText : ElementNamePredicate.Invoke(Selected);

        StateHasChanged();
        
        await _jsRuntime.InvokeVoidAsync("addDocumentListener", _subscriptionKey, "mousedown", _dotNetRef, "OnDocumentMouseDownAsync");
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        SubscriptionService.OnUpdate -= StateHasChanged;
        
        await _jsRuntime.InvokeVoidAsync("removeDocumentListener", _subscriptionKey, "mousedown");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!SubscriptionService.IsSubscriptionActivated) return;

        if (IsOpened)
        {
            await _inputRef.FocusAsync();

            if (ScrollToSelected && Selected != null! && !MultipleSelect && !_scrolled)
            {
                _scrolled = true;

                var index = Grouping ? -1 : -2;

                for (var i = 0; i < _groups.Count; i++)
                {
                    foreach (var element in _groups[i].Elements)
                    {
                        if (element.Item == Selected)
                        {
                            i = _groups.Count;
                            break;
                        }

                        index++;
                    }

                    index++;
                }
                
                var offset = index * ItemHeight;
                await JsUtilsService.ScrollToAsync(_scrollerId, "0", $"{offset}", "auto");
            }
        }

        if (_prevCount != Options.Count())
        {
            FilterData();
            StateHasChanged();
        }
    }

    private async Task OnOptionClickedAsync(TItem option)
    {
        if (!MultipleSelect)
        {
            IsOpened = false;
            Selected = option;
            Filter = string.Empty;

            if (Selected != null!)
            {
                _placeholder = ElementNamePredicate.Invoke(Selected);
            }
        }
        else
        {
            if (SelectedItems.Contains(option))
            {
                SelectedItems.Remove(option);
                await OnDeselectItem.InvokeAsync(option);
            }
            else
            {
                SelectedItems.Add(option);
                await OnSelectItem.InvokeAsync(option);
            }
        }

        _scrolled = false;

        StateHasChanged();
    }

    private bool IsSelected(TItem option)
    {
        if (MultipleSelect)
        {
            return SelectedItems.Contains(option);
        }

        return _selectedValue == option;
    }

    private async Task OnSelectClickedAsync()
    {
        if (!IsOpened)
        {
            var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
            _containerPos.Set(containerRect.X, containerRect.Y);
            _contentWidth = (float) containerRect.Width;
        }
        
        IsOpened = !IsOpened;
        
        _scrolled = false;
        Filter = string.Empty;

        FilterData();
        StateHasChanged();
    }

    private void OnFilterType()
    {
        FilterData(_typedFilterValue);

        StateHasChanged();
    }

    private void FilterData(string filter = "")
    {
        _groups = new();

        var groups = Options.GroupBy(GroupPredicate);
        _prevCount = Options.Count();

        foreach (var group in groups)
        {
            if (!group.Any()) continue;

            var gr = new Group
            {
                Name = GroupNamePredicate.Invoke(group.First())
            };

            foreach (var item in group)
            {
                var name = ElementNamePredicate == null ? string.Empty : ElementNamePredicate.Invoke(item);
                var filterElementValue = FilterByPredicate.Invoke(item);

                if (!string.IsNullOrEmpty(filterElementValue) && filterElementValue.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    gr.Elements.Add(new Element
                    {
                        Item = item,
                        Name = name
                    });
                }
            }

            if (gr.Elements.Count == 0) continue;

            _groups.Add(gr);
        }
    }

    private void OnGroupClicked(Group group)
    {
        group.Expanded = !group.Expanded;
        StateHasChanged();
    }

    private async Task OnInputKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
        {
            _typedFilterValue = string.Empty;
            StateHasChanged();
        }
        else
        {
            if (!IsOpened)
            {
                IsOpened = true;
                _scrolled = false;

                FilterData();
                StateHasChanged();
            }
        }

        await OnFilterKeyDown.InvokeAsync(e);
    }

    [JSInvokable]
    public async Task OnDocumentMouseDownAsync(ExtMouseEventArgs e)
    {
        var container = e.PathCoordinates
            .FirstOrDefault(x => x.Id == _containerId || x.Id == ContentId);

        if (container == null) // outside of select
        {
            IsOpened = false;
            _scrolled = false;
            Filter = string.Empty;
            await OnFocusOut.InvokeAsync();
            StateHasChanged();
        }
    }
}
