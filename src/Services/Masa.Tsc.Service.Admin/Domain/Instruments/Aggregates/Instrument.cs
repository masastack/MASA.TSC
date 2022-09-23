// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Instrument : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; set; }

    public string Layer { get; set; }

    public string Model { get; set; }

    public int Sort { get; set; }

    public bool IsRoot { get; set; }

    public Guid DirectoryId { get; set; }

    public bool IsGlobal { get; set; }

    public List<Panel> Panels { get; set; }

    public void Update(UpdateInstrumentDto pannel) {  }

    public void Save() { }

    public void RemovePanel(params Guid[] values) { }
}