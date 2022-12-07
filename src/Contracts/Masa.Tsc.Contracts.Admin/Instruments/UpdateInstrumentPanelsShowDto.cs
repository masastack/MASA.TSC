﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class UpdateInstrumentPanelsShowDto
{
    public Guid Id { get; set; }

    public UpdatePanelShowDto[] Panels { get; set; }
}
