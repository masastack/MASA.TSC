// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
    string lastKey;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public TeamDialogModel ParamData { get; set; } = new();

    int ErrorCount { get; set; }

    ConfigurationRecord ConfigurationRecord { get; set; } = new();

    ServiceAutoComplete ServiceAutoComplete { get; set; }

    List<AppDetailModel> Apps { get; set; } = new();

    TeamDto Team { get; set; }

    async Task OnAppChanged(string appid)
    {
        ParamData.ServiceId = appid;
        ConfigurationRecord.Service = appid;
        ErrorCount = await GetErroCountAsync(appid);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!Visible || string.IsNullOrEmpty(ParamData.ProjectId) || ParamData.TeamId == Guid.Empty)
            return;

        var key = $"{ParamData.TeamId}_{ParamData.ProjectId}_{ParamData.Start}_{ParamData.End}_{ParamData.ServiceId}";

        if (lastKey != key)
        {
            lastKey = key;
            Team = await ApiCaller.TeamService.GetTeamAsync(ParamData.TeamId, ParamData.ProjectId);
            Apps = Team.CurrentProject.Apps.Select(app => new AppDetailModel
            {
                Name = app.Name,
                Identity = app.Identity,
                Type = app.AppType,
                ServiceType = app.ServiceType
            }).ToList();
            Team.ProjectTotal = ParamData.TeamProjectCount;
            Team.AppTotal = ParamData.TeamServiceCount;
            ConfigurationRecord.Service = ParamData.ServiceId;
            ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
        }
    }

    void OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        await base.InvokeAsync(base.StateHasChanged);
    }

    async Task NavigateToDashboardConfiguration()
    {
        //add layer
        var data = await base.ApiCaller.InstrumentService.GetLinkAsync(MetricConstants.DEFAULT_LAYER, MetricValueTypes.Service);
        if (data?.InstrumentId is not null)
        {
            NavigationManager.NavigateToDashboardConfiguration(data.InstrumentId.ToString()!, ConfigurationRecord.Service);
        }
    }

    async Task<int> GetErroCountAsync(string appid)
    {
        var query = new SimpleAggregateRequestDto
        {
            Type = AggregateTypes.Count,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
            Service = appid,
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
}
