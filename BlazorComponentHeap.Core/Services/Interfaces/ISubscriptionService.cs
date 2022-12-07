namespace BlazorComponentHeap.Core.Services.Interfaces;

public interface ISubscriptionService
{
    bool IsSubscriptionActivated { get; }
    Action? OnUpdate { get; set; }
}
