// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Extensions;

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddPanelDto
{
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public Guid InstrumentId { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }=string.Empty;

    public int Sort { get; set; }

    public virtual PanelTypes Type { get; set; }

    public List<PanelMetricDto> Metrics { get; set; }
}
