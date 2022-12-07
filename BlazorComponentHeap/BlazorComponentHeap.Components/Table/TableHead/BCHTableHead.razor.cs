using BlazorComponentHeap.Components.Table.TableColumn;
using BlazorComponentHeap.Shared.Models.Datepicker;
using BlazorComponentHeap.Shared.Models.Table;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Table.TableHead;


public partial class BCHTableHead<TRowData> : ComponentBase where TRowData : class
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string Width { get; set; } = string.Empty;

    [Parameter]
    public BCHTableColumn<TRowData> Column { get; set; } = null!;

    [Parameter]
    public EventCallback<SelectData> OnFilterData { get; set; }

    [Parameter]
    public EventCallback<SortedData> OnClickSorted { get; set; }

    private List<string> _selectedItems = new();
    private List<DateTime> _selectedDates = new();
    private bool _sorted;

    private async Task OnFilterAsync(ChangeEventArgs changeEvent, string columnName, bool isMultiple = false)
    {
        var listData = new List<string>();

        if (isMultiple)
        {
            var list = changeEvent.Value as string[];

            if (list == null)
            {
                return;
            }

            list.ToList().ForEach(item => listData.Add(item.ToString()));
        }
        else
        {
            listData.Add(changeEvent!.Value!.ToString()!);
        }

        var selectedData = new SelectData
        {
            PropertyName = columnName,
            Data = listData
        };

        await OnFilterData.InvokeAsync(selectedData);
    }

    private async Task OnSelectedAsync(string columnName)
    {
        await OnFilterData.InvokeAsync(new SelectData
        {
            PropertyName = columnName,
            Data = _selectedItems.ToList()
        });
    }

    private async Task OnSelectedAsync(string selected, string columnName)
    {
        await OnFilterData.InvokeAsync(new SelectData
        {
            PropertyName = columnName,
            Data = new List<string> { selected }
        });
    }

    private async Task OnSortedAsync(string columnName)
    {
        _sorted = !_sorted;
        var sortedData = new SortedData
        {
            PropertyName = columnName,
            Value = _sorted
        };

        await OnClickSorted.InvokeAsync(sortedData);
    }

    private async Task OnSelectDateAsync(DateTime date, string columnName)
    {
        await OnFilterData.InvokeAsync(new SelectData
        {
            PropertyName = columnName,
            Date = date
        });
    }

    private async Task OnSelectDateAsync(DateRange date, string columnName)
    {
        await OnFilterData.InvokeAsync(new SelectData
        {
            PropertyName = columnName,
            DateRange = date
        });
    }
}