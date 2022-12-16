﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetBase : TscComponentBase
{
    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public virtual UpsertPanelDto Value { get; set; }

    [Parameter]
    public EventCallback<UpsertPanelDto> ValueChanged { get; set; }

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
