// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Elasticsearch;

public class ElasticsearchStorageConst : StorageConst
{
    private readonly ElasticsearchStorageLog log = new();
    private readonly ElasticsearchStorageTrace trace = new();

    public ElasticsearchStorageConst() : base()
    {
        Current = this;
    }

    public override string Timestimap => "@timestamp";

    public override string ServiceName => "Resource.service.name";

    public override string ServiceInstance => "Resource.service.instance.id";

    public override string Environment => "Resource.service.namespace";

    public override string TraceId => "TraceId";

    public override string SpanId => "SpanId";

    public override string ExceptionMessage => "Attributes.exception.message";

    public override string ExceptionType => "Attributes.exception.type";

    public override StorageLog Log => log;

    public override StorageTrace Trace => trace;
}

public class ElasticsearchStorageLog : StorageLog
{
    internal ElasticsearchStorageLog() { }

    public override string Body => "Body";

    public override string LogLevelText => "SeverityText";

    public override string LogErrorText => "Error";

    public override string LogWarningText => "Warning";

    public override string Url => "Attributes.http.target";

    public override string TaskId => "Attributes.TaskId";
}

public class ElasticsearchStorageTrace : StorageTrace
{
    internal ElasticsearchStorageTrace() { }

    public override string URL => "Attributes.http.target";

    public override string URLFull => "Attributes.http.url";

    public override string HttpStatusCode => "Attributes.http.status_code";

    public override string HttpMethod => "Attributes.http.method";

    public override string HttpRequestBody => "Attributes.http.request_content_body";

    public override string UserId => "Attributes.enduser.id";
}