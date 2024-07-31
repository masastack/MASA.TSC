// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Apm;

public class LatencyTypeChartData
{
    public MetricTypes MetricType { get; set; }

    public ChartData Avg { get; set; } = new();

    public ChartData P95 { get; set; } = new();

    public ChartData P99 { get; set; } = new();

    public ChartData ChartData
    {
        get
        {
            switch (MetricType)
            {
                case MetricTypes.P95: return P95;
                case MetricTypes.P99: return P99;
                default: return Avg;
            }
        }
    }
}

public enum MetricTypes
{
    Avg,
    P95,
    P99
}
