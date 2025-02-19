﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse;

public class ClickhouseStorageConst : StorageConst
{
    private readonly ClickhouseStorageLog log = new();
    private readonly ClickhouseStorageTrace trace = new();

    public ClickhouseStorageConst() : base()
    {
        Current = this;
    }

    public override string Timestimap => "Timestamp";

    public override string ServiceName => "ServiceName";

    public override string ServiceInstance => "Resource.service.instance.id";

    public override string Environment => "Resource.service.namespace";

    public override string TraceId => "TraceId";

    public override string SpanId => "SpanId";

    public override string ExceptionMessage => "Attributes.exception.message";

    public override string ExceptionType => "Attributes.exception.type";

    public override StorageLog Log => log;

    public override StorageTrace Trace => trace;
}

public class ClickhouseStorageLog : StorageLog
{
    internal ClickhouseStorageLog() { }

    public override string Body => "Body";

    public override string LogLevelText => "SeverityText";

    public override string LogErrorText => "Error";

    public override string LogWarningText => "Warning";

    public override string Url => "Attributes.http.target";

    public override string TaskId => "Attributes.TaskId";
}

public class ClickhouseStorageTrace : StorageTrace
{
    internal ClickhouseStorageTrace() { }

    public override string URL => "Attributes.http.target";

    public override string URLFull => "Attributes.http.url";

    public override string HttpStatusCode => "Attributes.http.status_code";

    public override string HttpMethod => "Attributes.http.method";

    public override string HttpRequestBody => "Attributes.http.request_content_body";

    public override string UserId => "Attributes.enduser.id";
}