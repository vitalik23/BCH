using BlazorComponentHeap.Components.Models.Tab;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.TestApp.Pages.Tabs.TestContent;

public partial class TestContentComponent
{
    [Parameter] public string Text { get; set; } = "";

    private TabContextModel _context = new TabContextModel(4) { Orderable = true };

    protected override void OnAfterRender(bool firstRender)
    {
        Console.WriteLine("TestContentComponent OnAfterRender");
    }

    protected override void OnInitialized()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _context.TabPanels[i].TabModels.Add(new TabModel
                {
                    Type = $"T {i} {j}",
                    Name = $"T {i} {j}",
                    Width = 118,
                    Height = 35,
                    Closable = true,
                    IconImage = "_content/BlazorComponentHeap.Components/img/tabs/default-icon/default-tab.svg",
                    SelectedIconImage = "_content/BlazorComponentHeap.Components/img/tabs/default-icon/default-tab-selected.svg"
                });
            }
        }
    }
}
