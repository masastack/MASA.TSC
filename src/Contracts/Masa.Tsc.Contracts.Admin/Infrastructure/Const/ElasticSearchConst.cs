// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Infrastructure.Const;

public sealed class ElasticSearchConst
{
    private ElasticSearchConst() { }

    public const string LogLevelText = "SeverityText";

    public const string LogErrorText = "Error";

    public const string LogWarningText = "Warning";

    public const string ServiceName = "Resource.service.name";

    public const string URL = "Attributes.http.target";

    public const string HttpPort = "Attributes.http.status_code";

    public static string TraceId { get; private set; } = "masa-stack-traces-0.6.1";

    public static string LogIndex { get; private set; } = "masa-stack-logs-0.6.1";

    public static string TraceIndex { get; private set; } = "masa-stack-traces-0.6.1";

    public static string Environment => "Resource.service.namespace";

    public const string ExceptionMessage = "Attributes.exception.message";

    public static void SetLogIndex(string index)
    {
        LogIndex = index;
    }

    public static void SetTraceIndex(string index)
    {
        TraceIndex = index;
    }
}