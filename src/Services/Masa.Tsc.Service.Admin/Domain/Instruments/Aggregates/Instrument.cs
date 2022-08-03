// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Instrument : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; set; }

    public string Layer { get; set; }

    public string Entity { get; set; }

    public int Sort { get; set; }

    public bool IsRoot { get; set; }

    public Guid DirectoryId { get; set; }

    public bool IsGlobal { get; set; }

    public List<Pannel> Pannels { get; set; }

    public void AddPannel() { }

    public void RemovePannel() { }
}