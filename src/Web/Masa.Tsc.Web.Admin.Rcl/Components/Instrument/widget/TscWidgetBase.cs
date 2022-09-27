// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetBase : TscComponentBase
{
    [Parameter]
    public int Index { get; set; }

    [Parameter]
    public StringNumber Height { get; set; }

    [Parameter]
    public StringNumber Width { get; set; }

    [Parameter]
    public string Style { get; set; }

    [Parameter]
    public string Class { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public virtual AddPanelDto Item { get; set; }
    
    protected Dictionary<string, object> Values { get; set; } = new();

    protected void SetValue(string key, object value)
    {
        if (Values.ContainsKey(key))
            Values[key] = value;
        else
            Values.Add(key, value);
    }

    public virtual AddPanelDto ToPanel()
    {
        return default!;
    }

    protected override async Task OnParametersSetAsync()
    {
        SetParameters();
        await base.OnParametersSetAsync();
    }

    protected virtual void SetParameters()
    {
    }
}
