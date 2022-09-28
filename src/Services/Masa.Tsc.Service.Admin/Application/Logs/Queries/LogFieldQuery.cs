﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogFieldQuery : Query<IEnumerable<Contracts.Admin.MappingResponse>>
{
    public override IEnumerable<Contracts.Admin.MappingResponse> Result { get; set; }
}