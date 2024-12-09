// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.DccConfigurations;

public class AppSettingConfiguration
{
    public string LogIndex { get; set; }

    public string TraceIndex { get; set; }

    public AppSettingTraceConfiguration Trace { get; set; }

    public ClickhouseConfiguration Clickhouse { get; set; }

    public bool IsClickHouse { get; set; }

    public bool IsElasticsearch { get; set; } = true;
}

public class AppSettingTraceConfiguration
{
    public int[] ErrorStatus { get; set; }
}

public class ClickhouseConfiguration
{
    public string Connection { get; set; }

    public string LogSource { get; set; }

    public string TraceSource { get; set; }

    public string AppLogTable { get; set; }

    public string AppTraceTable { get; set; }

    public string Suffix { get; set; }
}