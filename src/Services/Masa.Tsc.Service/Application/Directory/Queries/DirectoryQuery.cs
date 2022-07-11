// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Queries;

public record DirectoryQuery(Guid Id) : Query<DirectoryDto>
{ 
    public override DirectoryDto Result { get; set; }
}