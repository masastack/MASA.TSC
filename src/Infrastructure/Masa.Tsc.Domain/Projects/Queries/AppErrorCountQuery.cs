// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Queries;

public record AppErrorCountQuery(string AppId, DateTime Start, DateTime End) : Query<long>
{
    public override long Result { get; set; }
}