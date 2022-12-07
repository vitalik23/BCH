using System;
using BlazorComponentHeap.Components.RadioButton.RadioButton;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Components.RadioButton.RadioButtonsContainer;

public partial class BCHRadioButtonsContainer
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    private List<BCHRadioButton> _radioButtons = new();

    internal BCHRadioButton SelectedRb { get; set; } = null!;

    internal void AddRadioButton(BCHRadioButton radioButton)
    {
        _radioButtons.Add(radioButton);
        SelectedRb = _radioButtons[0];
    }

    internal void SelectButton(BCHRadioButton radioButton)
    {
        SelectedRb = radioButton;

        foreach (var rb in _radioButtons)
        {
            rb.Update();
        }
    }
}

