// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Metric : AggregateRoot<Guid>
{
    public Guid PannelId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Unit { get; set; }

    public string Value { get; set; }
}
