using BlazorComponentHeap.Components.Modal.Root;
using BlazorComponentHeap.Core.Extensions;
using BlazorComponentHeap.TestApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<BCHRootModal>("body::after");

var services = builder.Services;

services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddBCHComponents("subscription_key"); // key should be passed here somehow

await builder.Build().RunAsync();