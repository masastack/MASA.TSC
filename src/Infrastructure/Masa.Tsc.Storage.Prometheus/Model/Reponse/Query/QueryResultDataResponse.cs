// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class QueryResultDataResponse
{
    public ResultTypes ResultType { get; set; }

    public object[]? Result { get; set; }
}
