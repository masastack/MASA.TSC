// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Panel : AggregateRoot<Guid>
{
    public Instrument Instrument { get; set; }

    public Guid InstrumentId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Sort { get; set; }

    public PanelTypes Type { get; set; }

    public string UiType { get; set; } = string.Empty;

    public string Height { get; set; } = string.Empty;

    public string Width { get; set; } = string.Empty;

    //public string XName { get; set; }

    //public string YName { get; set; }

    public string ChartType { get; set; } = string.Empty;

    public Guid ParentId { get; set; }

    public List<PanelMetric> Metrics { get; set; }

    public Panel(Guid Id) : base(Id) { }

    public Panel() { }

    public void Remove()
    {

    }

    public void Update(UpdatePanelDto panel)
    {
        Title = panel.Name;
        Description = panel.Description ?? string.Empty;
        Sort = panel.Sort;
    }

    public void UpdateParentId(Guid parentId)
    {
        ParentId = parentId;
    }

    public void UpdateWidthHeight(string height, string width)
    { 
        Width = width??string.Empty;
        Height = height ?? string.Empty;
    }
}
