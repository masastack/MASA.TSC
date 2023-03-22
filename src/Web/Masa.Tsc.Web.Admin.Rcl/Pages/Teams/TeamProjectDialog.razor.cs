// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TeamProjectDialog
{
    string lastKey;

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public TeamDialogModel ParamData { get; set; } = new();

    int ErrorCount { get; set; }

    [Inject]
    TeamDetailConfigurationRecord ConfigurationRecord { get; set; }

    [Inject]
    IJSRuntime JSRuntime { get; set; }

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
            ConfigurationRecord.StartTime = ParamData.Start;
            ConfigurationRecord.EndTime= ParamData.End;
            ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
        }
    }

    async Task OpenLogAsync()
    {
        var url = $"/dashbord/log/{ConfigurationRecord.Service}/{ConfigurationRecord.StartTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/{ConfigurationRecord.EndTime.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")}/Error";        
        await JSRuntime.InvokeAsync<object>("open", url, "_blank");
    }

    async Task OnDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
    }

    async Task OnAutoDateTimeUpdateAsync((DateTimeOffset, DateTimeOffset) times)
    {
        (ConfigurationRecord.StartTime, ConfigurationRecord.EndTime) = times;
        ErrorCount = await GetErroCountAsync(ConfigurationRecord.Service!);
        await base.InvokeAsync(base.StateHasChanged);
    }

    void NavigateToDashboardConfiguration()
    {
        ConfigurationRecord.NavigateToConfiguration();
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
