// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogFieldQuery : Query<IEnumerable<Nest.MappingResponse>>
{
    public override IEnumerable<Nest.MappingResponse> Result { get; set; }
}

