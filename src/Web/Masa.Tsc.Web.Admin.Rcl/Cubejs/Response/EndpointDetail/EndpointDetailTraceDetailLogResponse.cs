// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailTraceDetailLogResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_LOG_DETAIL_VIEW)] EndpointDetailTraceDetailLogItemResponse Data);

internal record EndpointDetailTraceDetailLogItemResponse(string TraceId, string SpanId, string ServerityText, int ServerityNumber, string Body, string Resources, string Logs, CubejsAggDateTimeValueResponse DateKey)
{
    public LogResponseDto ToLogResponse()
    {
        var result = new LogResponseDto
        {
            TraceId = TraceId,
            SpanId = SpanId,
            Body = Body,
            SeverityNumber = ServerityNumber,
            SeverityText = ServerityText,
            Timestamp = DateKey.Value!.Value,
            Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(Resources)!,
            Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(Logs)!
        };
        return result;
    }
}