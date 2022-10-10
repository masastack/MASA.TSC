﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class TabsPanelDto : PanelDto
{
    public List<PanelDto> Tabs { get; set; }

    public override InstrumentTypes Type => InstrumentTypes.Tabs;
}
