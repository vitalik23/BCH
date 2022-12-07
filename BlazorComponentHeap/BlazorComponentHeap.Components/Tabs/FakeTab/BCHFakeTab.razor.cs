using BlazorComponentHeap.Components.Models.Tab;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorComponentHeap.Components.Tabs.FakeTab;

public partial class BCHFakeTab : IDisposable
{
    private readonly string _closeIcon = "_content/BlazorComponentHeap.Components/img/tabs/close-tab.svg";
    private readonly string _closeIconSelected = "_content/BlazorComponentHeap.Components/img/tabs/close-tab-selected.svg";

    [Parameter] public TabContextModel TabContext { get; set; } = null!;
    [Parameter] public TabModel TabModel { get; set; } = null!;
    [Parameter] public TabPanelModel PanelModel { get; set; } = null!;
    [Parameter] public string CloseImage { get; set; } = string.Empty;

    private NumberFormatInfo _numberFormatWithDot = new NumberFormatInfo { NumberDecimalSeparator = "." };
    private bool IsSelected() => PanelModel.SelectedTab == TabModel;

    private bool _shouldRender = true;
    protected override bool ShouldRender() => _shouldRender;

    protected override void OnAfterRender(bool firstRender)
    {
        //Console.WriteLine("MFakeTab OnAfterRender");
        _shouldRender = false;
    }

    protected override void OnInitialized()
    {
        TabContextModel.OnDragStart += OnDragStart;
    }

    public void Dispose()
    {
        TabContextModel.OnDragStart -= OnDragStart;
    }

    private void OnDragStart()
    {
        _shouldRender = true;
    }
}
