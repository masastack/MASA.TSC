// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Const;

internal class ElasticConst
{
    public static string LogIndex { get; private set; } = "logs";

    public static string TraceIndex { get; private set; } = "trace";

    public static void Configure(IConfiguration configuration)
    {
        var str = configuration.GetSection("elastic:logIndex").Value;
        if (!string.IsNullOrEmpty(str))
            LogIndex = str;

        str = configuration.GetSection("elastic:traceIndex").Value;
        if (!string.IsNullOrEmpty(str))
            LogIndex = str;
    }
}
