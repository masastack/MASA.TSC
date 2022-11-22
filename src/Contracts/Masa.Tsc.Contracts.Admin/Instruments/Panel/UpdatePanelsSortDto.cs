﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class UpdatePanelsSortDto
{
    public Guid InstrumentId { get; set; }

    public Guid ParentId { get; set; }

    public List<Guid> PanelIds { get; set; }
}