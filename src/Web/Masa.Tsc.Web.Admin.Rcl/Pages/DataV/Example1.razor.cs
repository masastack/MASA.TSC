﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.DataV;

public partial class Example1 : BDomComponentBase
{

    public Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys.LinkTrackingTopologyViewModel2 Data { get; set; } = new();

    private Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys.LinkTrackingTopologyViewModel2 AllData { get; set; } = new();

    private int _depth;
    private List<int> _depthItems = new List<int> { 1, 2, 3 };

    protected override async Task OnInitializedAsync()
    {
        FillData();
        DepthChange(3);
        await base.OnInitializedAsync();
    }

    public void FillData()
    {
        AllData.Edges = new List<LinkTrackingTopologyEdgeViewModel2>
        {
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="1",
                Target="0",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="0",
                Target="2",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="0",
                Target="3",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="0",
                Target="4",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="1",
                Target="2",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="5",
                Target="1",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="4",
                Target="6",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel2
            {
                Source="7",
                Target="5",
                Label="123.54"
            },
        };

        AllData.Nodes = new List<LinkTrackingTopologyNodeViewModel2>
        {
            new LinkTrackingTopologyNodeViewModel2{
                Id="0",
                Label="EShop-Order",
                Depth=0,
                State=MonitorStatuses.Normal,
                X=0,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="1",
                Label="EShop-ApI",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=-200,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="2",
                Label="EShop-Product",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=200,
                Y=100
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="3",
                Label="EShop-Sql",
                Depth=1,
                State=MonitorStatuses.Normal,
                X=300,
                Y=-150
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="4",
                Label="EShop-Payment",
                Depth=1,
                State=MonitorStatuses.Warn,
                X=200,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="5",
                Label="Api-gateway",
                Depth=2,
                State=MonitorStatuses.Error,
                X=-300,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="6",
                Label="EShop-Yun",
                Depth=2,
                State=MonitorStatuses.Error,
                X=500,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel2{
                Id="7",
                Label="EShop-App",
                Depth=3,
                State=MonitorStatuses.Normal,
                X=-500,
                Y=-50
            },
        };
    }

    private void DepthChange(int depth)
    {
        var nodes = AllData.Nodes.Where(x => x.Depth <= depth).ToList();
        var edges = AllData.Edges.Where(x => nodes.Any(y => y.Id == x.Source || y.Id == x.Target)).ToList();
        Data = new LinkTrackingTopologyViewModel2
        {
            Nodes = nodes,
            Edges = edges
        };
    }

    private void Refresh()
    {

    }
}
