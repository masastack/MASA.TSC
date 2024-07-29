//// Copyright (c) MASA Stack All rights reserved.
//// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

//namespace Masa.Tsc.Contracts.Admin.Infrastructure.Const;

//public sealed class StorageConst
//{
//    private StorageConst() { }

//    public static string Timestimap(bool isElasticsearch = false, bool isClickhouse = false)
//    {
//        if (isElasticsearch)
//        {
//            return "@timestamp";
//        }
//        else if (isClickhouse)
//        {
//            return "Timestamp";
//        }
//        return default!;
//    }

//    public const string LogLevelText = "SeverityText";

//    public const string LogErrorText = "Error";

//    public const string LogWarningText = "Warning";

//    public const string ServiceName = "Resource.service.name";

//    public const string ServiceInstance = "Resource.service.instance.id";

//    public const string URL = "Attributes.http.target";

//    public const string HttpPort = "Attributes.http.status_code";

//    public static string TraceId { get; private set; } = "TraceId";

//    public static string SpanId { get; private set; } = "SpanId";

//    public const string Environment = "Resource.service.namespace";

//    public const string ExceptionMessage = "Attributes.exception.message";

//    public const string ExceptionType = "Attributes.exception.type";

//    public const string TaskId = "Attributes.TaskId";

//    public const string UserId = "Attributes.enduser.id";
//}