using BlazorComponentHeap.Core.Models;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.Modal.Content;

public partial class BCHModalContent : IDisposable
{
    [Inject] private IPopupService PopupService { get; set; } = null!;

    [Parameter] public ModalModel ModalModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ModalModel.OnUpdate += StateHasChanged;
    }
    
    public void Dispose()
    {
        ModalModel.OnUpdate -= StateHasChanged;
    }
    
    private void OnOverlayClicked()
    {
        PopupService.FireOverlayClicked(ModalModel);
    }
    
    private bool IsCenter()
    {
        return string.IsNullOrWhiteSpace(ModalModel.X) || string.IsNullOrWhiteSpace(ModalModel.Y);
    }
}