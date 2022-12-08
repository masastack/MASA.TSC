// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamSearch
{
    [Parameter]
    public EventCallback<TeamSearchModel> OnValueChanged { get; set; }

    private TeamSearchModel _value = new();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnValueChange(DateTime? start = default, DateTime? end = default, string? text = default, string? appid = default)
    {
        if (start.HasValue)
            _value.Start = start.Value;
        if (end.HasValue)
            _value.End = end.Value;
        if (text != null)
            _value.Keyword = text;
        if (appid != null)
            _value.AppId = appid;
        if (OnValueChanged.HasDelegate)
            await OnValueChanged.InvokeAsync(_value);
    }

}