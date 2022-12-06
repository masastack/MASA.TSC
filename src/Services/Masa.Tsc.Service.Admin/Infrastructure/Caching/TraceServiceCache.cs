// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Caching;

internal class TraceServiceCache
{
    public string Id { get; set; }

    public string Service { get; set; }

    public string Instance { get; set; }

    public TraceNodeTypes Type { get; set; }
}
