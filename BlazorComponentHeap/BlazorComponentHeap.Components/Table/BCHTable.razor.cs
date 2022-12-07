using BlazorComponentHeap.Components.Table.TableColumn;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Table;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Table;

public partial class BCHTable<TRowData> : ComponentBase, IDisposable
    where TRowData : class
{
    [Inject]
    private ISubscriptionService _subscriptionService { get; set; } = null!;

    [Parameter]
    public ICollection<TRowData> Items { get; set; } = new List<TRowData>();

    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public EventCallback<SelectData> OnFilterData { get; set; }

    [Parameter]
    public EventCallback<SortedData> OnClickSorted { get; set; }

    [Parameter]
    public string MinWidth { get; set; } = string.Empty;

    #region Pagination
    [Parameter]
    public bool IsPagination { get; set; }

    [Parameter]
    public int PageSize { get; set; }

    [Parameter]
    public int TotalItems { get; set; }

    [Parameter]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value) return;
            _currentPage = value;
            CurrentPageChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public bool IsScrollPagination { get; set; }

    [Parameter]
    public EventCallback<int> CurrentPageChanged { get; set; }

    [Parameter]
    public EventCallback<int> OnClickPagination { get; set; }

    [Parameter]
    public EventCallback OnScrollBottom { get; set; }

    private int _prevCurrentPage = -1;
    private int _currentPage = 1;
    private bool _prevInBottom = false;
    #endregion

    private readonly List<BCHTableColumn<TRowData>> columns = new List<BCHTableColumn<TRowData>>();

    internal void AddColumn(BCHTableColumn<TRowData> column)
    {
        columns.Add(column);
    }

    protected override void OnInitialized()
    {
        _subscriptionService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    { 
        _subscriptionService.OnUpdate -= StateHasChanged;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!_subscriptionService.IsSubscriptionActivated) return;

        if (firstRender)
        {
            StateHasChanged();
        }
    }

    private async Task OnSelectPageNumberAsync(int number)
    {
        _currentPage = number;
        await OnClickPagination.InvokeAsync(_currentPage);
    }

    private async Task OnPaginationAsync(bool isNextNumber)
    {
        CurrentPage = Math.Clamp(CurrentPage + (Convert.ToInt32(isNextNumber) * 2 - 1), 1, Items.Count);

        if (CurrentPage != _prevCurrentPage)
        {
            _prevCurrentPage = CurrentPage;
            await OnClickPagination.InvokeAsync(CurrentPage);
        }
    }

    private async Task OnScrollAsync(ScrollEventArgs e)
    {
        if (IsPagination && IsScrollPagination)
        {
            bool inBottom = (e.ScrollTop + e.ClientHeight) >= e.ScrollHeight - 1;

            if (inBottom && !_prevInBottom)
            {
                await OnScrollBottom.InvokeAsync();
            }

            _prevInBottom = inBottom;
        }
    }
}
