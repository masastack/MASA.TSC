// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddInstrumentsDto
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Layer { get; set; }

    public string Model { get; set; }

    public int Sort { get; set; }

    public List<AddPannelDto> Pannels { get; set; }
}