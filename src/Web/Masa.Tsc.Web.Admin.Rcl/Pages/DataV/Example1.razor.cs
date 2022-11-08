// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules;
using Masa.Tsc.Web.Admin.Rcl.Pages.DataV.Modules.LinkTrackingTopologys;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.DataV;

public partial class Example1 : BDomComponentBase
{

    public LinkTrackingTopologyViewModel Data { get; set; } = new();

    private LinkTrackingTopologyViewModel AllData { get; set; } = new();

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
        AllData.Edges = new List<LinkTrackingTopologyEdgeViewModel>
        {
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="1",
                Target="0",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="2",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="3",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="0",
                Target="4",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="1",
                Target="2",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="5",
                Target="1",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="4",
                Target="6",
                Label="123.54"
            },
            new LinkTrackingTopologyEdgeViewModel
            {
                Source="7",
                Target="5",
                Label="123.54"
            },
        };

        AllData.Nodes = new List<LinkTrackingTopologyNodeViewModel>
        {
            new LinkTrackingTopologyNodeViewModel{
                Id="0",
                Label="EShop-Order",
                Depth=0,
                State=LinkTrackingTopologyStatuses.Normal,
                X=0,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="1",
                Label="EShop-ApI",
                Depth=1,
                State=LinkTrackingTopologyStatuses.Normal,
                X=-200,
                Y=0
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="2",
                Label="EShop-Product",
                Depth=1,
                State=LinkTrackingTopologyStatuses.Normal,
                X=200,
                Y=100
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="3",
                Label="EShop-Sql",
                Depth=1,
                State=LinkTrackingTopologyStatuses.Normal,
                X=300,
                Y=-150
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="4",
                Label="EShop-Payment",
                Depth=1,
                State=LinkTrackingTopologyStatuses.Alarm,
                X=200,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="5",
                Label="Api-gateway",
                Depth=2,
                State=LinkTrackingTopologyStatuses.Error,
                X=-300,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="6",
                Label="EShop-Yun",
                Depth=2,
                State=LinkTrackingTopologyStatuses.Error,
                X=500,
                Y=50
            },
            new LinkTrackingTopologyNodeViewModel{
                Id="7",
                Label="EShop-App",
                Depth=3,
                State=LinkTrackingTopologyStatuses.Normal,
                X=-500,
                Y=-50
            },
        };
    }

    private void DepthChange(int depth)
    {
        var nodes = AllData.Nodes.Where(x => x.Depth <= depth).ToList();
        var edges = AllData.Edges.Where(x => nodes.Any(y => y.Id == x.Source || y.Id == x.Target)).ToList();
        Data = new LinkTrackingTopologyViewModel
        {
            Nodes = nodes,
            Edges = edges
        };
    }

    private void Refresh()
    {

    }
}
