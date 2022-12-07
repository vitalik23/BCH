using System;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;
using BlazorComponentHeap.Components.RadioButton.RadioButtonsContainer;

namespace BlazorComponentHeap.Components.RadioButton.RadioButton;

public partial class BCHRadioButton
{
    [CascadingParameter(Name = "BCHRadioButtonContainer")] public BCHRadioButtonsContainer OwnerContainer { get; set; } = null!;
    [Parameter] public string Text { get; set; } = string.Empty;

    [Parameter] public RenderFragment<bool> ContentTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<bool> CircleTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<bool> DataTemplate { get; set; } = null!;

    [Parameter] public bool Disabled { get; set; }
    
    protected override void OnInitialized()
    {
        OwnerContainer.AddRadioButton(this);
    }

    private void OnClick()
    {
        OwnerContainer.SelectButton(this);
    }

    public void Update() => StateHasChanged();
}

