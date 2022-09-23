// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddPanelDto
{ 
    public Guid Id { get; set; }=Guid.NewGuid();

    public Guid ParentId { get; set; }

    public Guid InstrumentId { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Sort { get; set; }

    public InstrumentTypes Type { get; set; }

    public Dictionary<string, object> Attributes { get; set; }

    public List<AddPanelMetricDto> Metrics { get; set; }
}
