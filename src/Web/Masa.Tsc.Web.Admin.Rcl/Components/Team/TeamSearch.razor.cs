// Copyright (c) MASA Stack All rights reserved.
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
            _ = OnSearchChange(value);
        }
    }

    [Parameter]
    public EventCallback<TeamSearchModel> OnValueChanged { get; set; }

    [Parameter]
    public EventCallback<TeamSearchModel> OnTabsChanged { get; set; }

    [Parameter]
    public EventCallback<TeamSearchModel> OnSearchChanged { get; set; }

    [Parameter]
    public EventCallback<QuickRangeKey?> OnQuickRangeChanged { get; set; }

    private TeamSearchModel _value = new() { ProjectType = "all" };
    private List<KeyValuePair<string, string>> _projectTypes = new() { KeyValuePair.Create("all", "All") };

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.ProjectService.GetProjectTypesAsync();
        if (data != null)
            _projectTypes.AddRange(data);
        await base.OnInitializedAsync();
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