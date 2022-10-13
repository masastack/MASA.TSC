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
    public virtual PanelDto Item { get; set; }

    protected Dictionary<string, object> Values { get; set; } = new();

    protected void SetValue(string key, object value)
    {
        if (Values.ContainsKey(key))
            Values[key] = value;
        else
            Values.Add(key, value);
    }

    protected static T CreateDefault<T>() where T : PanelDto, new()
    {
        var result = new T()
        {
            Title = string.Empty,
            Description = string.Empty,
            Height = string.Empty,
            Width = string.Empty
        };
        return result;
    }
}
