// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Panel : AggregateRoot<Guid>
{
    public Instrument Instrument { get; set; }

    public Guid InstrumentId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Index { get; set; }

    public PanelTypes Type { get; set; }

    public string UiType { get; set; } = string.Empty;

    public string Height { get; set; } = string.Empty;

    public string Width { get; set; } = string.Empty;

    public string Left { get; set; } = string.Empty;

    public string Top { get; set; } = string.Empty;

    //public string XName { get; set; }

    //public string YName { get; set; }

    public string ChartType { get; set; } = string.Empty;

    public Guid ParentId { get; set; }

    public List<PanelMetric>? Metrics { get; set; }

    public Panel(Guid Id) : base(Id) { }

    public Panel() { }

    public void Update(PanelDto panel)
    {
        if (panel.Type == PanelTypes.Chart)
        {
            ChartType = ((EChartPanelDto)panel).ChartType;
        }
        Title = panel.Title;
        Description = panel.Description ?? string.Empty;
        Index = panel.Sort;
    }

    public void UpdateShow(UpdatePanelShowDto model)
    {
        Index = model.Index;
        Height = model.Height;
        Width = model.Width;
        Top = model.Top;
        Left = model.Left;
    }

    public void UpdateParentId(Guid parentId)
    {
        ParentId = parentId;
    }

    public void UpdateWidthHeight(string height, string width)
    {
        Width = width ?? string.Empty;
        Height = height ?? string.Empty;
    }

    public static Panel ConvertTo(PanelDto model)
    {
        var panel = new Panel(model.Id)
        {
            Type = model.Type,
            Title = model.Title ?? string.Empty,
            Description = model.Description ?? string.Empty,
            Index = model.Sort,
            InstrumentId = model.InstrumentId,
            ParentId = model.ParentId
        };
        if (model.Type == PanelTypes.Chart)
        {
            panel.ChartType = ((EChartPanelDto)model).ChartType;

            if (model is EChartPanelDto chartDto && chartDto.Metrics != null && chartDto.Metrics.Any())
            {
                var list = chartDto.Metrics.Select(x => new PanelMetric(x.Id)
                {
                    Name = x.Name,
                    Caculate = x.Caculate,
                    PanelId = model.Id,
                    Sort = x.Sort,
                    Unit = x.Unit
                });
                panel.Metrics = list.ToList();
            }
        }
        return panel;
    }
}
