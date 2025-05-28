// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response;

/// <summary>
/// base cube request result
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <param name="Data"></param>
public record CubejsBaseResponse<TData>([property: JsonPropertyName(CubejsConstants.CUBEJS_IDENTITY)] List<TData> Data) where TData : class;


/// <summary>
/// group query timestamp
/// </summary>
/// <param name="DateKey"></param>
public record CubejsAggDateTimeResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_AGG)] CubejsAggDateTimeValueResponse DateKey);

/// <summary>
/// group query timestamp value
/// </summary>
/// <param name="Value"></param>
public record CubejsAggDateTimeValueResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_AGG_VALUE)] DateTime Value);