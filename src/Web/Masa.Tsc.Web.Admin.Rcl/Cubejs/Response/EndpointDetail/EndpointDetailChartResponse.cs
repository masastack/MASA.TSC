// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailChartResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_CHART_VIEW)] EndpointDetailChartItemResponse Data);

internal record EndpointDetailChartItemResponse(
    long Latency,
    double Throughput,
    double Failed,
    long PNinetyNine,
    long PNinetyFive,
    CubejsAggDateTimeValueResponse DateKey
    );
