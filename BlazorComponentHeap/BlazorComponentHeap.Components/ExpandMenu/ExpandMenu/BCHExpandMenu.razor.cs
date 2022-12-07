using BlazorComponentHeap.Components.ExpandMenu.ExpandMenuContainer;
using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;
namespace BlazorComponentHeap.Components.ExpandMenu.ExpandMenu;

public partial class BCHExpandMenu : IDisposable
{

    [Inject] private IJSUtilsService _jSUtilsService { get; set; }

    [CascadingParameter(Name = "BCHExpandMenuContainer")] public BCHExpandMenuContainer OwnerContainer { get; set; } = null!;
    
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;

    private string _descriptionId = $"_id{Guid.NewGuid()}";
    private double _heightDescription = default(double);

    protected override void OnInitialized()
    {
        OwnerContainer.AddExpandMenu(this);
        IJSUtilsService.OnResize += OnResizeAsync;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            
        }
    }

    private async Task OnResizeAsync()
    {
        var boundingClientRect = await _jSUtilsService.GetBoundingClientRectAsync(_descriptionId);
        _heightDescription = boundingClientRect.OffsetHeight;
        StateHasChanged();
    }

    private async Task OnClick()
    {
        await OnResizeAsync();
        OwnerContainer.SelectButton(this);
    }

    public void Update() => StateHasChanged();

    public void Dispose()
    {
        IJSUtilsService.OnResize -= OnResizeAsync;
    }
}
