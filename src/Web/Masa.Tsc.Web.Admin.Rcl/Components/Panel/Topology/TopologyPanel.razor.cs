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

    object G6Option { get; set; } = new
    {
        container = "mountNode",
        fitView = true,
        fitViewPadding = new[] { 20, 40, 50, 20 },
        animate = true,
        modes = new
        {
            Default = new[] { "drag-node" },
        },
        defaultNode = new
        {
            type = "image",
            img = "https://g.alicdn.com/cm-design/arms-trace/1.0.155/styles/armsTrace/images/TAIR.png",
            size = 80,
            labelCfg = new
            {
                position = "bottom"
            }
        },
        defaultEdge = new
        {
            type = "line-dash",
            lineWidth = 10,
            // 边上的标签文本配置
            labelCfg = new
            {
                autoRotate = true, // 边上的标签文本根据边的方向旋转
            },
        },
        layout = new
        {
            // Object，可选，布局的方法及其配置项，默认为 random 布局。
            type = "dagre", // 指定为力导向布局
            preventOverlap = true, // 防止节点重叠
            linkDistance = 1000, // 指定边距离为100
        },
        animateCfg = new
        {
            duration = 100, // Number，一次动画的时长
            easing = "linearEasing", // String，动画函数
        }
    };

    object? G6Data { get; set; }

    [CascadingParameter]
    ConfigurationRecord ConfigurationRecord { get; set; }

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
        var result = await ApiCaller.TopologyService.GetAsync(ConfigurationRecord.AppName, _depth, ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime);
        if (result?.Data is null) return;
        G6Data = new
        {
            nodes = result.Services.Select(item => new 
            {
                id = item.Id,
                label = item.Name
            }),
            edges = result.Relations.Select(item => new
            {
                source = item.CurrentId,
                target = item.DestId,
                //label = item.AvgLatency.ToString()
            }),
        };
    }

    async Task RefreshAsync()
    {
        _depth = 1;
        await GetYopologyPanelData();
    }
}