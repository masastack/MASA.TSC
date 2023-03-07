// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class UpsertPanelDto
{
    PanelTypes _panelType;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; }

    public string Description { get; set; }

    public PanelTypes PanelType
    {
        get => _panelType;
        set
        {
            _panelType = value;
            if (Width == 0 && Height == 0)
            {
                switch (value)
                {
                    case PanelTypes.Tabs:
                        Width = 12;
                        Height = 6;
                        break;
                    case PanelTypes.Chart:
                        Width = 12;
                        Height = 5;
                        break;
                    case PanelTypes.Log:
                        Width = 12;
                        Height = 10;
                        break;
                    case PanelTypes.Trace:
                        Width = 12;
                        Height = 9;
                        break;
                    case PanelTypes.Topology:
                        Width = 12;
                        Height = 6;
                        break;
                    default: break;
                }
            }
        }
    }

    public int Width { get; set; }

    public int Height { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

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

    bool _isRemove;
    public bool IsRemove
    {
        get => _isRemove;
        set
        {
            _isRemove = value;
            foreach (var item in ChildPanels)
            {
                item.IsRemove = value;
            }
        }
    }

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

    protected void RemoveChildPanel(UpsertPanelDto panel)
    {
        panel.IsRemove = true;
        ChildPanels.Remove(panel);
    }

    #endregion

    public UpsertPanelDto()
    {
        Width = GlobalPanelConfig.Width; Height = GlobalPanelConfig.Height;
    }
}
