using BlazorComponentHeap.Shared.Models.Datepicker;

namespace BlazorComponentHeap.Shared.Models.Table;

public class SelectData
{
    public List<string> Data { get; set; } = null!;
    public string PropertyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateRange DateRange { get; set; } = null!;
}
