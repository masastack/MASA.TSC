﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Shared.Entities;

public class Directory : AggregateRoot<Guid>
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public Guid ParentId { get; set; } = Guid.Empty;

    public List<Instrument>? Instruments { get; set; }

    public void Update(string name)
    {
        if (!string.IsNullOrEmpty(name) && !string.Equals(Name, name))
            Name = name;
    }
}
