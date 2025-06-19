// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.EndpointDetail;

internal record EndpointDetailErrorResponse<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_DETAIL_ERROR_LIST_VIEW)] TData Data) where TData : class;

internal record EndpointDetailErrorChartItemResponse(
    string SpanId,
    int Cnt
);

internal record ServiceErrorChartResponse(int Cnt, CubejsAggDateTimeValueResponse DateKey);

internal record ServiceErrorListItemTotalResponse(int GrCnt);

internal record ServiceErrorListItemResponse(string ExceptionType, string MsgGroupKey, int Cnt, DateTime MaxT);