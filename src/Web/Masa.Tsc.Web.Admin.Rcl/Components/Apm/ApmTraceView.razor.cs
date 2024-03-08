// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

public partial class ApmTraceView
{
    [Parameter]
    public object Value { get; set; }

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public string LinkUrl { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<bool> LoadingCompelete { get; set; }

    private string className = "slide";

    private async Task CloseAsync()
    {
        Show = false;
        if (ShowChanged.HasDelegate)
            await ShowChanged.InvokeAsync(Show);
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        if (Show)
        {
            var newKey = JsonSerializer.Serialize(Value);
            if (!string.Equals(md5Key, newKey))
            {
                _dic = Value.ToDictionary();
                md5Key = newKey;
            }
        }
        base.OnParametersSet();
    }
    private IDictionary<string, object>? _dic = null;
    private string md5Key;
    private string search = string.Empty;

    private void OnSeach(string value)
    {
        search = value;
    }
}