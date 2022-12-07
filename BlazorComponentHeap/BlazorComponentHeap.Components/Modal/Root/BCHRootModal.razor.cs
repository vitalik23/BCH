using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Modal.Root;

public partial class BCHRootModal : IDisposable
{
    [Inject] private IPopupService PopupService { get; set; } = null!;

    protected override void OnInitialized()
    {
        PopupService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        PopupService.OnUpdate -= StateHasChanged;
    }
}
