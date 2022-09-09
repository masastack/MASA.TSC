// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamSearch
{
    [Parameter]
    public TeamSearchModel Value
    {
        get { return _value; }
        set
        {
            if (value == null)
            {
                _value.Start = default;
                _value.End = default;
                _value.Keyword = default!;
                _value.AppId = default!;
            }
            else
            {
                _value = value;
            }
        }
    }

    [Parameter]
    public EventCallback<TeamSearchModel> OnValueChanged { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<ProjectDto>>> OnLoadProjectsAsync { get; set; }

    private TeamSearchModel _value = new();
    private List<ProjectDto> _projects { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var data = await OnLoadProjectsAsync.Invoke();
            if (data != null && data.Any())
                _projects.AddRange(data);

            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
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
        if(OnValueChanged.HasDelegate)
           await OnValueChanged.InvokeAsync(_value);
    }
}