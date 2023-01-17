﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamSearch
{
    string _search;

    public string Search
    {
        get => _search;
        set
        {
            _search = value;
            OnSearchChange(value);
        }
    }

    [Parameter]
    public EventCallback<TeamSearchModel> OnValueChanged { get; set; }

    [Parameter]
    public EventCallback<TeamSearchModel> OnTabsChanged { get; set; }

    [Parameter]
    public EventCallback<TeamSearchModel> OnSearchChanged { get; set; }

    private TeamSearchModel _value = new() { ProjectType = "all" };
    private List<KeyValuePair<string, string>> _projectTypes = new() { KeyValuePair.Create("all", "All") };

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.ProjectService.GetProjectTypesAsync();
        if (data != null)
            _projectTypes.AddRange(data);
        await base.OnInitializedAsync();
    }

    private async Task OnValueChange(DateTime? start = default, DateTime? end = default, string? projectType = default, string? text = default, string? appid = default)
    {
        if (start.HasValue)
            _value.Start = start.Value;
        else if (_value.Start.HasValue)
            _value.Start = null;

        if (end.HasValue)
            _value.End = end.Value;
        else if (_value.End.HasValue)
            _value.End = null;

        if (text != null)
            _value.Keyword = text;
        else if (!string.IsNullOrEmpty(_value.Keyword))
            _value.Keyword = default!;

        if (appid != null)
            _value.AppId = appid;

        if (!string.IsNullOrEmpty(projectType))
            _value.ProjectType = projectType;
        else if (!string.IsNullOrEmpty(_value.ProjectType))
            _value.ProjectType = string.Empty;

        if (OnValueChanged.HasDelegate)
            await OnValueChanged.InvokeAsync(_value);
    }

    async Task OnTabsChange(string projectType)
    {
        _value.ProjectType = projectType;
        await OnTabsChanged.InvokeAsync(_value);
    }

    async Task OnSearchChange(string search)
    {
        _value.Keyword = search;
        await OnTabsChanged.InvokeAsync(_value);
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        var (startTime, endTime) = times;
        _value.Start = startTime.UtcDateTime;
        _value.End = endTime.UtcDateTime;
        await OnValueChanged.InvokeAsync(_value);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        var (startTime, endTime) = times;
        _value.Start = startTime.UtcDateTime;
        _value.End = endTime.UtcDateTime;
        await base.InvokeAsync(async () => await OnValueChanged.InvokeAsync(_value));
    }
}