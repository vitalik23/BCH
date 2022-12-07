using BlazorComponentHeap.Shared.Enums.Table;
using BlazorComponentHeap.Shared.Models.Table;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Table.TableColumn;

public partial class BCHTableColumn<TRowData> : ComponentBase where TRowData : class
{
    [CascadingParameter]
    public BCHTable<TRowData> OwnerGrid { get; set; } = null!;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string FilterProperty { get; set; } = string.Empty;

    [Parameter]
    public Func<TRowData, object> Expression { get; set; } = null!;

    [Parameter]
    public RenderFragment<TRowData> TdTemplate { get; set; } = null!;

    [Parameter]
    public RenderFragment ThTemplate { get; set; } = null!;

    [Parameter]
    public string Width { get; set; } = string.Empty;

    [Parameter]
    public ColumnFilterType FilterType { get; set; }

    [Parameter]
    public EventCallback<bool> OnClickSorted { get; set; }

    [Parameter]
    public bool IsSorted { get; set; }

    [Parameter]
    public List<string> SelectData { get; set; } = new();

    [Parameter]
    public BCHTableColumn<TRowData> Column { get; set; } = null!;

    [Parameter]
    public EventCallback<SelectData> OnFilterData { get; set; }

    protected override void OnInitialized()
    {
        OwnerGrid.AddColumn(this);
    }

    //protected override void OnParametersSet()
    //{
    //    if (lastCompiledExpression != Expression)
    //    {
    //        compiledExpression = Expression?.Compile();
    //        lastCompiledExpression = Expression;
    //    }
    //}


    //private static string GetMemberName<T>(Expression<T> expression)
    //{
    //    return expression.Body switch
    //    {
    //        MemberExpression m => m.Member.Name,
    //        UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
    //        _ => throw new NotSupportedException("Expression of type '" + expression.GetType().ToString() + "' is not supported")
    //    };
    //}

}
