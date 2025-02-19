// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class QueryResultCommonResponse: ResultBaseResponse
{
    public QueryResultDataResponse? Data { get; set; }
}
