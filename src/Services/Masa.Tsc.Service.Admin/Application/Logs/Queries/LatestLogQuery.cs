// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Logs;

public record LatestLogQuery(DateTime Start, DateTime End, string Query, bool IsDesc = true) : Query<LogDto>
{
    public override LogDto Result { get; set; }
}
