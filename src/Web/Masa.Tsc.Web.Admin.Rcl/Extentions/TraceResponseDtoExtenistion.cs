// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

internal static class TraceResponseDtoExtenistion
{
    public static int? StatusCode(this TraceResponseDto trace)
    {
        if (!trace.IsHttp()) return default;
        if (trace.Attributes.TryGetValue("http.status_code", out var statusCode))
            return int.TryParse(statusCode.ToString(), out var num) ? num : default;
        else if (trace.Attributes.TryGetValue("http.response.status_code", out statusCode))
            return int.TryParse(statusCode.ToString(), out var num) ? num : default;
        return default;
    }

    public static string? Method(this TraceResponseDto trace)
    {
        if (!trace.IsHttp()) return default;
        if (trace.Attributes.TryGetValue("http.method", out var method))
            return method?.ToString();
        else if (trace.Attributes.TryGetValue("http.request.method", out method))
            return method?.ToString();
        return default;
    }

    public static string? UserAgent(this TraceResponseDto trace) => trace.Attributes.TryGetValue("user_agent.original", out var agent) || trace.Attributes.TryGetValue("http.user_agent", out agent) || trace.Attributes.TryGetValue("client.user_agent", out agent) ? agent.ToString() : default;

    public static bool TryGetUserAgent(this TraceResponseDto trace, out string? userAgent)
    {
        userAgent = trace.UserAgent();
        return string.IsNullOrEmpty(userAgent);
    }

    public static string? Target(this TraceResponseDto trace)
    {
        if (!trace.IsHttp()) return default;
        var sdkVersion = trace.SdkVersion();

        if (trace.Attributes.TryGetValue("http.url", out var url))
            return url?.ToString();
        else if (trace.Attributes.TryGetValue("url.path", out url))
            return url?.ToString();
        else if (trace.Attributes.TryGetValue("http.target", out url))
            return url?.ToString();
        return default;
    }

    private static bool IsHttp(this TraceResponseDto trace) => trace.Attributes.ContainsKey("http.scheme") || trace.Attributes.ContainsKey("http.url");

    private static string? SdkVersion(this TraceResponseDto trace) => trace.Resource.TryGetValue("telemetry.sdk.version", out var version) ? version.ToString() : default;
}
