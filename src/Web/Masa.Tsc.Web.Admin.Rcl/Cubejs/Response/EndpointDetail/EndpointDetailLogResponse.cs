// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailLogResponse<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_LOG_DETAIL_VIEW)] TData Data) where TData : class;

internal record EndpointDetailLogChartItemResponse(
    CubejsAggDateTimeValueResponse DateKey,
    int Cnt
);

internal record EndpointDetailLogListTotalResponse(int Cnt);

internal record EndpointDetailLogListItemResponse(
    string SpanId,
    string TraceId,
    string SeverityText,
    string SeverityNumber,
    string Body,
    string Resources,
    string Logs,
    CubejsAggDateTimeValueResponse DateKey
);
