// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailPageResponse([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_TRACE_PAGE_VIEW)] EndpointDetailPageItemResponse Data);

internal record EndpointDetailPageItemResponse(string TraceId, string Duration, CubejsAggDateTimeValueResponse DateKey);