// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Apm;

internal class ListChartData
{
    public string Name { get; set; }

    public string Service { get; set; }

    public string Endpoint { get; set; }

    public string Envs { get; set; }

    public long Latency { get; set; }

    public double Throughput { get; set; }

    public double Failed { get; set; }

    public ChartData LatencyChartData { get; set; }

    public ChartData ThroughputChartData { get; set; }

    public ChartData FailedChartData { get; set; }

    public double P95 { get; set; }

    public double P99 { get; set; }

    public ChartData P95ChartData { get; set; }

    public ChartData P99ChartData { get; set; }        
}

public class ChartData {

    public bool HasChart { get; set; } = true;

    public bool ChartLoading { get; set; } = true;

    public bool EmptyChart { get; set; }

    public object Data { get; set; }
}
