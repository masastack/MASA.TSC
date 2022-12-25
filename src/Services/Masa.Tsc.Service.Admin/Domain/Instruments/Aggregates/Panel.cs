// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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

    public Panel(UpsertPanelDto data)
    {
        Id = data.Id;
        Update(data);
    }

    private Panel(UpsertPanelDto data, Guid parentId, int index)
    {
        Type = data.PanelType;
        ParentId = parentId;
        Index = index;
        Description = data.Description;
        Width = data.Width.ToString();
        Height = data.Height.ToString();
        Top = data.Y.ToString();
        Left = data.X.ToString();
        InstrumentId = data.Id;
        ExtensionData = data.ExtensionData;
    }

    public void Update(UpsertPanelDto update)
    {
        if (Type == PanelTypes.Tabs)
        {
            UpdateTabItems(update.ChildPanels,Id);
        }

        Title = update.Title;
        Description = update.Description;

        if (update.Metrics != null && update.Metrics.Any())
        {
            var list = new List<PanelMetric>();
            foreach (var item in update.Metrics)
            {
                var metric = Metrics?.FirstOrDefault(m => m.Id == item.Id);
                if (metric == null)
                    metric = new PanelMetric(item);
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

        if (update.ExtensionData != null && update.ExtensionData.Any())
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
                panel = new Panel(item, parentId, itemIndex);
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
        if (Type == PanelTypes.TabItem)
            return;

        Index = index;
        Title = update.Title;

        //update pannels
    }

    //public static Panel ConvertTo(PanelDto model)
    //{
    //    var panel = new Panel(model.Id)
    //    {
    //        Type = model.Type,
    //        Title = model.Title ?? string.Empty,
    //        Description = model.Description ?? string.Empty,
    //        Index = model.Sort,
    //        InstrumentId = model.InstrumentId,
    //        ParentId = model.ParentId
    //    };
    //    if (model.Type == PanelTypes.Chart)
    //    {
    //        if (model is EChartPanelDto chartDto && chartDto.Metrics != null && chartDto.Metrics.Any())
    //        {
    //            var list = chartDto.Metrics.Select(x => new PanelMetric(x.Id)
    //            {
    //                Name = x.Name,
    //                Caculate = x.Caculate,
    //                PanelId = model.Id,
    //                Sort = x.Sort,
    //                Unit = x.Unit
    //            });
    //            panel.Metrics = list.ToList();
    //        }
    //    }
    //    return panel;
    //}
}
