using BlazorComponentHeap.Core.Models;
using BlazorComponentHeap.Core.Options;
using BlazorComponentHeap.Core.Providers;
using BlazorComponentHeap.Core.Services.Interfaces;

namespace BlazorComponentHeap.Core.Services;

internal class SubscriptionService : ISubscriptionService
{

    public bool IsSubscriptionActivated { get; private set; } = true;
    public Action? OnUpdate { get; set; }

    public SubscriptionService()
    {

    }

    private async Task CheckSubscriptionAsync()
    {
       
    }
}
