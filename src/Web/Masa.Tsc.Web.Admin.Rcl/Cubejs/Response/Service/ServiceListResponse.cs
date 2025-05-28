// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response;

public record ServiceListResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_LIST_VIEW)] ServiceListItemResponse Item);

public record ServiceListItemResponse(string ServiceName, string Namespace, double LatencyAgg, long Throughput, double FailedAgg);
