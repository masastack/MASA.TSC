// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
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

    DateTimeOffset? StartTime { get; set; } = DateTimeOffset.UtcNow.AddMinutes(-15);

    DateTimeOffset? EndTime { get; set; } = DateTimeOffset.UtcNow;

    IntervalItem Interval { get; set; } = IntervalItem.Off;

    private bool _firstRender = true;

    private string? _oldKey;

    private int ErrorCount { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (!Visible || string.IsNullOrEmpty(ConfigurationRecord.ProjectId) || ConfigurationRecord.TeamId == Guid.Empty)
        {
            return;
        }

        var key = $"{ConfigurationRecord.Service}_{ConfigurationRecord.ProjectId}_{ConfigurationRecord.TeamId}_{ConfigurationRecord.StartTime}_{ConfigurationRecord.EndTime}";
        if (_oldKey != key)
        {
            _firstRender = true;
            _oldKey = key;
            Interval = ConfigurationRecord.Interval;
            StartTime = ConfigurationRecord.StartTime;
            EndTime = ConfigurationRecord.EndTime;
            DefaultQuickRangeKey = ConfigurationRecord.DefaultQuickRangeKey;
            await InitDataAsync();
            ErrorCount = await GetErrorCountAsync(ConfigurationRecord.Service!);
        }
    }

    async Task OnServiceChanged(string service)
    {
        ConfigurationRecord.Service = service;
        ErrorCount = await GetErrorCountAsync(service);
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
        var url = $"/dashboard/log/{ConfigurationRecord.Service}/{StartTime?.UtcDateTime:yyyy-MM-dd HH:mm:ss}/{EndTime?.UtcDateTime:yyyy-MM-dd HH:mm:ss}/Error";
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset?, DateTimeOffset?) times)
    {
        if (_firstRender)
        {
            _firstRender = false;
            return;
        }
        (StartTime, EndTime) = times;
        ConfigurationRecord.UpdateDateTimesFromTuple(times);
        ConfigurationRecord.DefaultQuickRangeKey = DefaultQuickRangeKey;
        ErrorCount = await GetErrorCountAsync(ConfigurationRecord.Service!);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset?, DateTimeOffset?) times)
    {
        await OnDateTimeUpdateAsync(times);
        await InvokeAsync(StateHasChanged);
    }

    async Task<int> GetErrorCountAsync(string service)
    {
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = StartTime!.Value.UtcDateTime,
            End = EndTime!.Value.UtcDateTime,
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

}