// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Services.Observability.Elastic;

public static class ElasticConst
{
    public const string ES_HTTP_CLIENT_NAME = "masa.stack.tsc.service.es.client";

    public static string LogIndex { get; private set; } = "logs";
    public static string LogTimestamp { get; private set; } = "@timestamp";

    public static string TraceIndex { get; private set; } = "trace";
    //public static string SpanIndex { get; private set; } = "span";
    public static string TraceTimestamp { get; private set; } = "@timestamp";

    public const string TRACE_ID = "TraceId";
    public const string PARENT_ID = "ParentSpanId";
    public const string SPAN_ID = "SpanId";
    //public const string TRANSACTION_ID = "transaction.id";

    public const string TRACE_SERVICE_NAME = "Resource.service.name";
    public const string TRACE_INSTANCE_NAME = "Resource.service.instance.id";
    public const string TRACE_ENDPOINT_NAME = "Attributes.http.target";

    public static int MAX_DATA_COUNT = 10000;

    public static void ConfigureElasticIndex(this IConfiguration configuration)
    {
        var str = configuration.GetSection("masa:elastic:logIndex").Value;
        if (!string.IsNullOrEmpty(str))
            LogIndex = str;

        str = configuration.GetSection("masa:elastic:traceIndex").Value;
        if (!string.IsNullOrEmpty(str))
            TraceIndex = str;

        //str = configuration.GetSection("masa:elastic:spanIndex").Value;
        //if (!string.IsNullOrEmpty(str))
        //    SpanIndex = str;
    }
}
