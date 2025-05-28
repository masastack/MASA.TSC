// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Cubejs.Apm.Response;

internal record DateTimeValue([property: JsonPropertyName("value")] DateTime Value);

internal record TotalReponse(long Dcnt);

internal record EndpointListTotalResponse([property: JsonPropertyName(CubejsConstants.ServiceAndEndpointListModelViewName)] TotalReponse Total);

internal record EndpointListResponse([property: JsonPropertyName(CubejsConstants.ServiceAndEndpointListModelViewName)] EndpointListItemResponse Data);

internal record EndpointListItemResponse(string ServiceName, string Namespace, string Target, string Method, double Latency, long Throughput, long Failed, double FailedAgg, double LatencyAgg, [property: JsonPropertyName(CubejsConstants.Timestamp)] DateTimeValue Time);

internal record ServiceChartItemListResponse([property: JsonPropertyName(CubejsConstants.ServiceChartModelViewName)] ChartItemResponse Data);

internal record EndpointChartItemListResponse([property: JsonPropertyName(CubejsConstants.EndpointChartModelViewName)] ChartItemResponse Data);

internal record ChartItemResponse(string ServiceName, string Namespace, string Target, string Method, double Latency, long Throughput, double Failed, [property: JsonPropertyName(CubejsConstants.Timestamp)] DateTimeValue Time, [property: JsonPropertyName(CubejsConstants.P99)] double P99, [property: JsonPropertyName(CubejsConstants.P95)] double P95);