﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Queries;

public record InstrumentDetailQuery(Guid UserId, Guid Id) : Query<InstrumentDetailDto>
{
    public override InstrumentDetailDto Result { get; set; }
}