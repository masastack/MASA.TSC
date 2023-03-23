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

    protected override async Task OnParametersSetAsync()
    {
        if (!Visible || string.IsNullOrEmpty(ConfigurationRecord.ProjectId) || ConfigurationRecord.TeamId == Guid.Empty)
            return;

        var key = $"{ConfigurationRecord.ProjectId}{ConfigurationRecord.TeamId}";
        if (_oldKey != key)
        {
            _oldKey = key;
            await InitDataAsync();
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
        var url = $"/dashbord/log/{ConfigurationRecord.Service}/{ConfigurationRecord.StartTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/{ConfigurationRecord.EndTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/Error";
        await JSRuntime.InvokeAsync<object>("open", url, "_blank");
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        (ParamData.Start, ParamData.End) = times;
        ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        OnDateTimeUpdateAsync(times);
        await InvokeAsync(StateHasChanged);
    }

    async Task<int> GetErroCountAsync(string service)
    {
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
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
        Team = null;
        await VisibleChanged.InvokeAsync(false);
    }

    void NavigateToDashboardConfiguration()
    {
        ConfigurationRecord.NavigateToConfiguration();
    }
}
