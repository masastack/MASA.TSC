// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LogFieldQuery : Query<IEnumerable<MappingResponse>>
{
    public override IEnumerable<MappingResponse> Result { get; set; }
}

