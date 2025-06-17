// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailLatencyResponse<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_LATENCY_CHART_VIEW)] TData Data);

internal record EndpointDetailLatencyChartItemResponse(
    long Duration,
    int Total
);

internal record EndpointDetailLatencyTotalResponse(
    int Total
);