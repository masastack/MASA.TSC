// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailTraceDetailResponse<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_TRACE_DETAIL_VIEW)] TData Data);

internal record EndpointDetailTraceDetailItemResponse(
    string TraceId,
    string SpanId,
    string ParentSpanId,
    string SpanKind,
    string Resources,
    string Spans,
    string Duration,
    CubejsAggDateTimeValueResponse DateKey
   )
{
    public TraceResponseDto ToTraceResponse()
    {
        var result = new TraceResponseDto
        {
            TraceId = TraceId,
            SpanId = SpanId,
            ParentSpanId = ParentSpanId,
            Kind = SpanKind,
            Timestamp = DateKey.Value!.Value,
            EndTimestamp = DateKey.Value!.Value.AddMilliseconds(double.Parse(Duration) / 1e6),
            Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(Resources)!,
            Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(Spans)!
        };
        return result;
    }
}