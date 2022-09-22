// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddPanelDto
{
    public Guid Id { get; set; }=Guid.NewGuid();

    public int Width { get; set; }

    public int Height { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string UiType { get; set; }

    public string XName { get; set; }

    public string YName { get; set; }

    public InstrumentTypes Type { get; set; }

    public Dictionary<string, object> Attributes { get; set; }

    public List<AddPanelDto> Panels { get; set; }

    public List<AddMetricDto> Metrics { get; set; }
}
