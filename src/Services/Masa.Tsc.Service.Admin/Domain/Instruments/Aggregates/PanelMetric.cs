// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class PanelMetric : AggregateRoot<Guid>
{
    public PanelMetric() : base() { }

    public PanelMetric(Guid id) : base(id) { }

    public PanelMetric(PanelMetricDto data)
    {
        Update(data);
    }

    public Panel Panel { get; set; }

    public Guid PanelId { get; set; }

    public string Name { get; set; }

    public string Caculate { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public int Sort { get; set; }

    public string Icon { get; set; } = string.Empty;

    public void Update(PanelMetricDto data)
    {
        Name = data.Name;
        Caculate = data.Caculate ?? string.Empty;
        Color = data.Color ?? string.Empty;
        Unit = data.Unit ?? string.Empty;
        Sort = data.Sort;
        Icon = data.Icon ?? string.Empty;
    }
}
