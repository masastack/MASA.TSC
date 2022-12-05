﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class UpdateInstrumentDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }
}
