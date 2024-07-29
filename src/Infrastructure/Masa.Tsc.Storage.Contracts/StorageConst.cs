// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Contracts;

public class StorageConst
{
    public static StorageConst Current { get; protected set; }

    protected StorageConst()
    {
        if (Current == null)
        {
            Log = StorageLog.GetInstance();
            Trace = StorageTrace.GetInstance();
            Current = this;
        }
    }

    public static void Init(StorageConst current) => Current ??= current;

    public virtual string Timestimap => "Timestamp";

    public virtual string ServiceName => "Resource.service.name";

    public virtual string ServiceInstance => "Resource.service.instance.id";

    public virtual string Environment => "Resource.service.namespace";

    public virtual string TraceId => "TraceId";

    public virtual string SpanId => "SpanId";

    public virtual string ExceptionMessage => "Attributes.exception.message";

    public virtual string ExceptionType => "Attributes.exception.type";

    public virtual StorageLog Log { protected set; get; }

    public virtual StorageTrace Trace { protected set; get; }
}

public class StorageLog
{
    protected StorageLog() { }

    internal static StorageLog GetInstance()
    {
        return new StorageLog();
    }

    public virtual string Body => "Body";

    public virtual string LogLevelText => "SeverityText";

    public virtual string LogErrorText => "Error";

    public virtual string LogWarningText => "Warning";

    public virtual string Url => "RequestPath";

    public virtual string TaskId => "Attributes.TaskId";
}

public class StorageTrace
{
    protected StorageTrace() { }

    internal static StorageTrace GetInstance()
    {
        return new StorageTrace();
    }

    public virtual string URL => "Attributes.http.target";

    public virtual string URLFull => "Attributes.http.url";

    public virtual string HttpStatusCode => "Attributes.http.status_code";

    public virtual string HttpMethod => "Attributes.http.method";

    public virtual string HttpRequestBody => "Attributes.http.request_content_body";

    public virtual string Duration => "Duration";

    public virtual string SpanKind => "SpanKind";

    public virtual string UserId => "Attributes.enduser.id";
}