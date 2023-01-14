// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;
using static Nest.JoinField;
using System.Collections.Generic;

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Panel : AggregateRoot<Guid>
{
    public Instrument Instrument { get; set; }

    public Guid InstrumentId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public PanelTypes Type { get; set; }

    public string Height { get; set; } = string.Empty;

    public string Width { get; set; } = string.Empty;

    public string Left { get; set; } = string.Empty;

    public string Top { get; set; } = string.Empty;

    #region tab and items

    public int Index { get; set; }

    public Guid ParentId { get; set; }

    public List<Panel> Panels { get; set; }

    #endregion

    #region echart 、table and top n

    public Dictionary<ExtensionFieldTypes, object?> ExtensionData { get; set; }

    public List<PanelMetric>? Metrics { get; set; }

    #endregion

    public Panel() { }

    public Panel(Guid Id) : base(Id) { }

    public Panel(UpsertPanelDto data, Guid instrumentId, Guid parentId)
    {
        Id = data.Id;
        InstrumentId = instrumentId;
        ParentId = parentId;
        Update(data);
    }

    private Panel(UpsertPanelDto data, Guid parentId, int index, Guid instrumentId)
    {
        Type = data.PanelType;
        ParentId = parentId;
        Index = index;
        Title = data.Title ?? string.Empty;
        Width = data.Width.ToString();
        Height = data.Height.ToString();
        Top = data.Y.ToString();
        Left = data.X.ToString();
        Id = data.Id;
        InstrumentId = instrumentId;
        ExtensionData = data.ExtensionData;

        if (data.ChildPanels != null)
        {
            Panels = new List<Panel>();
            foreach (var item in data.ChildPanels)
            {
                Panels.Add(new Panel(item, instrumentId, Id));
            }
        }
    }

    public void Update(UpsertPanelDto update)
    {
        Type = update.PanelType;
        if (Type == PanelTypes.Tabs)
        {
            UpdateTabItems(update.ChildPanels, Id);
        }
        Title = update.Title ?? update.PanelType.ToString("G");
        Description = update.Description ?? string.Empty;

        Width = update.Width.ToString();
        Height = update.Height.ToString();
        Top = update.Y.ToString();
        Left = update.X.ToString();

        if (update.Metrics != null && update.Metrics.Any())
        {
            var list = new List<PanelMetric>();
            foreach (var item in update.Metrics)
            {
                var metric = Metrics?.FirstOrDefault(m => m.Id == item.Id);
                if (metric == null)
                    metric = new PanelMetric(Id, item);
                else
                    metric.Update(item);
                list.Add(metric);
            }
            Metrics = list;
        }
        else if (Metrics != null && Metrics.Any())
        {
            Metrics.Clear();
        }
        
        ExtensionData = update.ExtensionData;
    }

    /// <summary>
    /// update tab items
    /// </summary>
    /// <param name="children"></param>
    private void UpdateTabItems(List<UpsertPanelDto> children, Guid parentId)
    {
        if (children == null || !children.Any())
        {
            if (Panels != null && Panels.Any())
                Panels.Clear();
        }
        if (Panels == null)
            Panels = new();
        var list = new List<Panel>();

        var itemIndex = 0;
        foreach (var item in children!)
        {
            var panel = Panels.FirstOrDefault(x => x.Id == item.Id);
            if (panel == null)
            {
                panel = new Panel(item, parentId, itemIndex, InstrumentId);
            }
            else
            {
                panel.UpdateTabItem(item, itemIndex);
                Panels.Remove(panel);
            }
            list.Add(panel);
            itemIndex++;
        }
        Panels = list;
    }

    private void UpdateTabItem(UpsertPanelDto update, int index)
    {
        if (Type != PanelTypes.TabItem)
            return;

        Index = index;
        Title = update.Title;

        if (update.ChildPanels == null || !update.ChildPanels.Any())
        {
            if (Panels != null && Panels.Any())
                Panels.Clear();
        }
        else
        {
            var list = new List<Panel>();
            foreach (var item in update.ChildPanels)
            {
                var panel = Panels.FirstOrDefault(x => x.Id == item.Id);
                if (panel == null)
                {
                    panel = new Panel(item, InstrumentId, Id);
                }
                else
                {
                    panel.Update(item);
                    Panels.Remove(panel);
                }
                list.Add(panel);
            }
            Panels = list;
        }
    }
}