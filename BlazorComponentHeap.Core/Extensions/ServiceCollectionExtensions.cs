using BlazorComponentHeap.Core.Options;
using BlazorComponentHeap.Core.Providers;
using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorComponentHeap.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBCHComponents(this IServiceCollection services, string subscriptionKey)
    {
        services.AddScoped<IJSUtilsService, JSUtilsService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IHttpService, HttpService>();
        services.AddScoped<IPopupService, PopupService>();
        services.AddScoped(x => new SubscriptionKeyHolder { SubscriptionKey = subscriptionKey });
        services.AddScoped<EncryptProvider>();
        services.AddScoped<SecurityProvider>();

        return services;
    }
}
