// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Extensions;

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class TabsPanelDto : PanelDto
{
    //[JsonConverter(typeof(PanelDtoEnumerableConverter))]
    public List<TabItemPanelDto> Tabs { get; set; }

    public override InstrumentTypes Type => InstrumentTypes.Tabs;
}
