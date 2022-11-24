// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class PanelMetric : AggregateRoot<Guid>
{
    public PanelMetric():base() { }

    public PanelMetric(Guid id):base(id) { }

    public Panel Panel { get; set; }

    public Guid PanelId { get; set; }

    public string Name { get; set; }

    public string Caculate { get; set; }

    public string Color { get; set; }=string.Empty;

    public string Unit { get; set; }    

    public int Sort { get; set; }
}
