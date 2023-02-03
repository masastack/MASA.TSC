// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Topology;

public partial class TopologyPanel
{
    List<int> _depthItems = new() { 1, 2, 3 };
    int _depth = 1;
    string? oldConfigurationRecordKey;

    int Depth
    {
        get => _depth;
        set
        {
            _depth = value;
            GetYopologyPanelData().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

    LinkTrackingTopologyViewModel Data { get; set; }

    protected override async Task OnInitializedAsync()
    {
        oldConfigurationRecordKey = ConfigurationRecord.Key;
        await GetYopologyPanelData();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (oldConfigurationRecordKey != ConfigurationRecord.Key)
        {
            oldConfigurationRecordKey = ConfigurationRecord.Key;
            await GetYopologyPanelData();
        }
    }

    async Task GetYopologyPanelData()
    {
        Data = new();

        //var result =await ApiCaller.TopologyService.GetAsync("masa-tsc-service-admin", _depth, DateTime.Now.AddMonths(-3), DateTime.Now);
        var result = await ApiCaller.TopologyService.GetAsync(ConfigurationRecord.AppName, _depth, ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime);
        if (result?.Data is null) return;
        Data.Edges = result.Data.Select(item => new LinkTrackingTopologyEdgeViewModel
        {
            Source = item.CurrentId,
            Target = item.DestId,
            Label = item.AvgLatency.ToString()
        }).ToList();
        Data.Nodes = result.Services.Select(item => new LinkTrackingTopologyNodeViewModel
        {
            Id = item.Id,
            Label = item.Name,
            State = MonitorStatuses.Error,
            X = Random.Shared.Next(-500, 500),
            Y = Random.Shared.Next(-500, 500),
        }).ToList();
    }

    async Task RefreshAsync()
    {
        _depth = 1;
        await GetYopologyPanelData();
    }
}

//Data.Edges = new List<LinkTrackingTopologyEdgeViewModel>
//{
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="1",
//        Target="0",
//        Label="11"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="0",
//        Target="2",
//        Label="12"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="0",
//        Target="3",
//        Label="13"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="0",
//        Target="4",
//        Label="14"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="1",
//        Target="2",
//        Label="15"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="5",
//        Target="1",
//        Label="16"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="4",
//        Target="6",
//        Label="17"
//    },
//    new LinkTrackingTopologyEdgeViewModel
//    {
//        Source="7",
//        Target="5",
//        Label="18"
//    },
//};
//Data.Nodes = new List<LinkTrackingTopologyNodeViewModel>
//{
//    new LinkTrackingTopologyNodeViewModel{
//        Id="0",
//        Label="EShop-Order",
//        Depth=0,
//        State=MonitorStatuses.Normal,
//        X=0,
//        Y=0
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="1",
//        Label="EShop-ApI",
//        Depth=0,
//        State=MonitorStatuses.Normal,
//        X=-200,
//        Y=0
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="2",
//        Label="EShop-Product",
//        Depth=0,
//        State=MonitorStatuses.Normal,
//        X=200,
//        Y=100
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="3",
//        Label="EShop-Sql",
//        Depth=0,
//        State=MonitorStatuses.Normal,
//        X=300,
//        Y=-150
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="4",
//        Label="EShop-Payment",
//        Depth=0,
//        State=MonitorStatuses.Warn,
//        X=200,
//        Y=50
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="5",
//        Label="Api-gateway",
//        Depth=0,
//        State=MonitorStatuses.Error,
//        X=-300,
//        Y=50
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="6",
//        Label="EShop-Yun",
//        Depth=0,
//        State=MonitorStatuses.Error,
//        X=500,
//        Y=50
//    },
//    new LinkTrackingTopologyNodeViewModel{
//        Id="7",
//        Label="EShop-App",
//        Depth=0,
//        State=MonitorStatuses.Normal,
//        X=-500,
//        Y=-50
//    },
//};