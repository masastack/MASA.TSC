// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class InstrumentDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid DirecotryId { get; set; }

    public string Layer { get; set; }

    public string Model { get; set; }

    public int Sort { get; set; }

    public bool IsGlobal { get; set; }

    public bool IsRoot { get; set; }

    public List<UpsertPanelDto> Panels { get; set; }
}