// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response.Search;

internal record SearchResponseBase<TData>([property: JsonPropertyName(CubejsConstants.ENDPOINT_LIST_VIEW)] TData Data) where TData : class;

public record SearchEnvServiceResponse(string NameSpace,string ServiceName, string ProjectIdentity,string AppDescription,string AppType);

public record SearchEndpointResponse(string Target);
