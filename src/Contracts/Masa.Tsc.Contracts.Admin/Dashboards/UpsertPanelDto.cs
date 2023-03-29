// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class UpsertPanelDto: GridstackChangeEventArgs
{
    public string Title { get; set; }

    public string Description { get; set; }

    public PanelTypes PanelType { get; set; }

    public List<UpsertPanelDto> ChildPanels { get; set; } = new();

    public virtual List<PanelMetricDto> Metrics { get; set; } = new();

    public Dictionary<ExtensionFieldTypes, object?> ExtensionData { get; set; } = new();

    public object? this[ExtensionFieldTypes field]
    {
        get => ExtensionData.GetValueOrDefault(field);
        set => ExtensionData[field] = value;
    }

    #region UI

    [JsonIgnore]
    public UpsertPanelDto? ParentPanel { get; set; }

    public virtual UpsertPanelDto Clone(UpsertPanelDto panel)
    {
        Id = panel.Id;
        Title = panel.Title;
        Description = panel.Description;
        PanelType = panel.PanelType;
        Width = panel.Width;
        Height = panel.Height;
        X = panel.X;
        Y = panel.Y;
        ChildPanels.Clear();
        ChildPanels.AddRange(panel.ChildPanels);
        Metrics.Clear();
        Metrics.AddRange(panel.Metrics ?? new());
        ExtensionData.Clear();
        if (panel.ExtensionData is not null)
        {
            foreach (var (key, value) in panel.ExtensionData)
            {
                ExtensionData.Add(key, value);
            }
        }

        return this;
    }

    #endregion

    public UpsertPanelDto(int x = 0,int y = 0)
    {
        Width = GlobalPanelConfig.Width; Height = GlobalPanelConfig.Height;
        X = x;
        Y = y;
    }
}
