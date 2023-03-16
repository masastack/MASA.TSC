namespace Masa.Stack.Components;

public partial class ColorGroup
{
    [Parameter]
    public List<string> Colors { get; set; } = new List<string> { "#4318FF", "#05CD99", "#FFB547", "#37A7FF", "#FF5252" };

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public int Size { get; set; } = 24;

    [Parameter]
    public int Elevation { get; set; }

    private async Task OnClickHandler(string color)
    {
        await ValueChanged.InvokeAsync(color == Value ? null : color);
    }
}
