// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailChartResponse<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_CHART_VIEW)] TData Data) where TData : class;

internal record EndpointDetailChartItemResponse(
    long Latency,
    double Throughput,
    double Failed,
    long PNinetyNine,
    long PNinetyFive,
    CubejsAggDateTimeValueResponse DateKey
    );

internal record EndpointDetailPageP95Response(double PNinetyFive);
