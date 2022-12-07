using BlazorComponentHeap.Components.Cropper;

namespace BlazorComponentHeap.TestApp.Pages.Cropper;

public partial class CropperPage
{
    private BCHCropper _bchCropper = null!;
    
    private async Task OnGetResultAsync()
    {
        var result = await _bchCropper.GetBase64ResultAsync();
        Console.WriteLine(result);
    }
}