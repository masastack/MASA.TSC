// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus.Model;

public class LableValueQueryRequest: MetaDataQueryRequest
{
    public string Lable { get; set; } = "__name__";
}
