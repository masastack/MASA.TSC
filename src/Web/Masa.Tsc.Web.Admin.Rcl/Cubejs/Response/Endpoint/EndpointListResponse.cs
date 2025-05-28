// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response;


public record EndpointListResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_LIST_VIEW)] EndpointListItemResponse Item);

public record EndpointListItemResponse(string ServiceName, string Target, string Method, double LatencyAgg, long Throughput, double FailedAgg);

public record EndpointTotalResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_LIST_VIEW)] EndpointTotalItemResponse Item);

public record EndpointTotalItemResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_LIST_COUNT)] long Total);