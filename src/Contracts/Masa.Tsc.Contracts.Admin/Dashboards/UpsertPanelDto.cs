// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class UpsertPanelDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; }

    public string Description { get; set; }

    public PanelTypes PanelType { get; set; }

    public int Width { get; set; } = 5;

    public int Height { get; set; } = 3;

    public int X { get; set; }

    public int Y { get; set; }

    public List<UpsertPanelDto> ChildPanels { get; set; } = new();

    public List<PanelMetricDto> PanelMetrics { get; set; } = new();

    #region UI

    UpsertPanelDto? _currentTabItem;

    [JsonIgnore]
    public UpsertPanelDto? ParentPanel { get; set; }

    [JsonIgnore]
    public UpsertPanelDto? CurrentTabItem
    {
        get => _currentTabItem ?? ChildPanels.FirstOrDefault();
        set => _currentTabItem = value;
    }

    #endregion
}
