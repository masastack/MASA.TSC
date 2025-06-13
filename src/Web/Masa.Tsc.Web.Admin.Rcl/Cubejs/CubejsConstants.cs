// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs;

internal sealed class CubejsConstants
{
    private CubejsConstants() { }

    public const string CUBEJS_IDENTITY = "cube";
    public const string TIMESTAMP_AGG = "datekey";
    public const string TIMESTAMP_AGG_VALUE = "value";
    public const string ENV_AGG = "namespace";
    public const string SERVICENAME = "servicename";
    public const string TARGET = "target";
    public const string METHOD = "method";
    public const string LATENCY = "latency";
    public const string THROUGHPUT = "throughput";
    public const string FAILED = "failed";
    public const string FAILED_AGG = "failedagg";
    public const string LATENCY_AGG = "latencyagg";
    public const string TEAM_ID = "teamid";
    public const string PROJECT = "projectIdentity";
    public const string APPTYPE = "apptype";
    public const string SERVICE_DESCRIPTION = "appdescription";
    public const string STATUS_CODE = "statuscode";
    public const string TRACEID = "traceid";
    public const string SPANID = "spanid";
    public const string USERID = "enduserid";
    public const string EXCEPTION_TYPE = "exceptiontype";
    public const string EXCEPTION_MESSAGE = "exceptionmessage";
    public const string REQUEST_QUERY = "url";
    public const string REQUEST_BODY = "requestcontentbody";
    public const string P99 = "pninetynine";
    public const string P95 = "pninetyfive";

    public const string ENDPOINT_LIST_COUNT = "dcnt";
    public const string ENDPOINT_LIST_BYDETAIL_COUNT = "dcnt";

    public const string ENDPOINT_LIST_VIEW = "metrics";
    public const string ENDPOINT_LIST_CHART_VIEW = "metrics";
    public const string ENDPOINT_LIST_BYDETAIL_VIEW = "listdetail";
    public const string ENDPOINT_DETAIL_CHART_VIEW = "metricspageone";
}