﻿using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Markup;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Core.Services;

public class JSUtilsService : IJSUtilsService
{
    private readonly IJSRuntime _jsRuntime;

    public JSUtilsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<BoundingClientRect> GetBoundingClientRectAsync(string id)
    {
        return await _jsRuntime.InvokeAsync<BoundingClientRect>("bchGetBoundingClientRectById", id);
    }

    public async Task ScrollToAsync(string id, string x, string y, string behavior = "smooth")
    {
        await _jsRuntime.InvokeVoidAsync("bchScrollElementTo", id, x, y, behavior);
    }
}