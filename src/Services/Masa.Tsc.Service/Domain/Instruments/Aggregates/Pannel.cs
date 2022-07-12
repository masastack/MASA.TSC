// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Pannel:AggregateRoot<Guid>
{
    public Guid InstrumentId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int Index { get; set; }

    public string UiType { get; set; }

    public string XName { get; set; }

    public string YName { get; set; }

    public string Color { get; set; }

    public string XDisplayName { get; set; }

    public string YDisplayName { get; set; }

    public Guid ParentId { get; set; }

    public List<Pannel> Pannels { get; set; }

    public List<Metric> Metrics { get; set; }
}
