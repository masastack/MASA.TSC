﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

public partial class ApmTraceView
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public object Value { get; set; }

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public bool Dialog { get; set; } = true;

    [Parameter]
    public string LinkUrl { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<bool> LoadingCompelete { get; set; }

    [Parameter]
    public StringNumber? Height { get; set; }

    [Parameter]
    public bool IsRedirectTrace { get; set; } = true;

    private async Task CloseAsync()
    {
        Show = false;
        if (ShowChanged.HasDelegate)
            await ShowChanged.InvokeAsync(Show);
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        if ((!Dialog || Show))
        {
            if (Value == null) Value = new Dictionary<string, object>();
            var newKey = JsonSerializer.Serialize(Value);
            if (!string.Equals(md5Key, newKey))
            {
                if (Value is Dictionary<string, object> dic)
                    _dic = dic;
                else
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

    private async Task OpenLogAsync()
    {
        await JSRuntime.InvokeVoidAsync("open", LinkUrl, "_blank");
    }

    private void ShowJwt(string value)
    {
        showJwt = true;
        jwtValue = value;
        StateHasChanged();
    }


    private bool showJwt = false;
    private string? jwtValue = default;
}