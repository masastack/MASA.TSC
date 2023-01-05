// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Topology;

public partial class TopologyPanel
{
    List<int> _depthItems = new() { 1, 2, 3 };
    int _depth = 3;
    int _oldDepth = 0;

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

    public LinkTrackingTopologyViewModel Data { get; set; }
    public LinkTrackingTopologyViewModel ViewData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetYopologyPanelData();
        await base.OnInitializedAsync();
    }

    LinkTrackingTopologyViewModel GetDepthData()
    {
        if(_oldDepth != _depth)
        {
            _oldDepth = _depth;
            var nodes = Data.Nodes.Where(x => x.Depth <= _depth).ToList();
            var edges = Data.Edges.Where(x => nodes.Any(y => y.Id == x.Source || y.Id == x.Target)).ToList();
            ViewData = new LinkTrackingTopologyViewModel
            {
                Nodes = nodes,
                Edges = edges
            };
        }
        return ViewData;
    }

    async Task GetYopologyPanelData()
    {
        Data = new();
        Data.Edges = new List<LinkTrackingTopologyEdgeViewModel>
        {
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="1",
                Target="0",
                Label="11"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="2",
                Label="12"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="3",
                Label="13"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="4",
                Label="14"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="1",
                Target="2",
                Label="15"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="5",
                Target="1",
                Label="16"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="4",
                Target="6",
                Label="17"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="7",
                Target="5",
                Label="18"
            },
        };
        Data.Nodes = new List<LinkTrackingTopologyNodeViewModel>
        {
            new LinkTrackingTopologyNodeViewModel{
                Id="0",
                Label="EShop-Order",
                Depth=0,
                State=MonitorStatuses.Normal,
                X=0,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="1",
                Label="EShop-ApI",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=-200,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="2",
                Label="EShop-Product",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=200,
                Y=100
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="3",
                Label="EShop-Sql",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=300,
                Y=-150
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="4",
                Label="EShop-Payment",
                Depth=1,
                State=MonitorStatuses.Warn,
                X=200,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="5",
                Label="Api-gateway",
                Depth=2,
                State=MonitorStatuses.Error,
                X=-300,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="6",
                Label="EShop-Yun",
                Depth=2,
                State=MonitorStatuses.Error,
                X=500,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="7",
                Label="EShop-App",
                Depth=3,
                State=MonitorStatuses.Normal,
                X=-500,
                Y=-50
            },
        };


        //await ApiCaller.TopologyService.GetAsync();
        await Task.CompletedTask;
    }

    async Task Refresh()
    {
        _depth = 1;
        await GetYopologyPanelData();
    }
}
