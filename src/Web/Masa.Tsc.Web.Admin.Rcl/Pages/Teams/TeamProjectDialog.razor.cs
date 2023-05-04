// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
    string? _oldKey;

    int ErrorCount { get; set; }

    [Inject]
    IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    ServiceAutoComplete ServiceAutoComplete { get; set; }

    List<AppDetailModel> Apps { get; set; } = new();

    TeamDto? Team { get; set; }

    QuickRangeKey? DefaultQuickRangeKey { get; set; } = QuickRangeKey.Last15Minutes;

    DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;

    IntervalItem Interval { get; set; } = IntervalItem.Off;

    protected override async Task OnParametersSetAsync()
    {
        if (!Visible || string.IsNullOrEmpty(ConfigurationRecord.ProjectId) || ConfigurationRecord.TeamId == Guid.Empty)
        {
            return;
        }

        var key = $"{ConfigurationRecord.Service}_{ConfigurationRecord.ProjectId}_{ConfigurationRecord.TeamId}_{ConfigurationRecord.StartTime}_{ConfigurationRecord.EndTime}";
        if (_oldKey != key)
        {
            firstRender = true;
            _oldKey = key;
            Interval = ConfigurationRecord.Interval;
            StartTime = ConfigurationRecord.StartTime;
            EndTime = ConfigurationRecord.EndTime;
            DefaultQuickRangeKey = ConfigurationRecord.DefaultQuickRangeKey;
            await InitDataAsync();
            ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
        }
    }

    async Task OnServiceChanged(string service)
    {
        ConfigurationRecord.Service = service;
        ErrorCount = await GetErroCountAsync(service);
    }

    async Task InitDataAsync()
    {
        Team = await ApiCaller.TeamService.GetTeamAsync(ConfigurationRecord.TeamId, ConfigurationRecord.ProjectId);
        Apps = Team.CurrentProject.Apps.Select(app => new AppDetailModel
        {
            Name = app.Name,
            Identity = app.Identity,
            Type = app.AppType,
            ServiceType = app.ServiceType
        }).ToList();
        Team.ProjectTotal = ConfigurationRecord.TeamProjectCount;
        Team.AppTotal = ConfigurationRecord.TeamServiceCount;
    }

    async Task OpenLogAsync()
    {
        var url = $"/dashbord/log/{ConfigurationRecord.Service}/{StartTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/{EndTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/Error";
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        if (firstRender)
        {
            firstRender = false;
            return;
        }
        (StartTime, EndTime) = times;
        _oldKey = $"{ConfigurationRecord.Service}_{ConfigurationRecord.ProjectId}_{ConfigurationRecord.TeamId}_{StartTime}_{EndTime}";
        ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        await OnDateTimeUpdateAsync(times);
        await InvokeAsync(StateHasChanged);
    }

    async Task<int> GetErroCountAsync(string service)
    {
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = StartTime.UtcDateTime,
            End = EndTime.UtcDateTime,
            Service = service,
            Name = ElasticSearchConst.ServiceName,
            Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                Name=ElasticSearchConst.LogLevelText,
                Type= ConditionTypes.Equal,
                Value=ElasticSearchConst.LogErrorText
                }
            }
        };
        return await ApiCaller.LogService.AggregateAsync<int>(query);
    }

    async Task DialogVisibleChanged()
    {
        await VisibleChanged.InvokeAsync(false);
    }

    void NavigateToDashboardConfiguration()
    {
        ConfigurationRecord.TeamProjectDialogVisible = false;
        ConfigurationRecord.NavigateToConfiguration();
    }

    bool firstRender = true;
}