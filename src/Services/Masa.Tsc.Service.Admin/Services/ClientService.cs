// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class ClientService : ServiceBase
{
    public ClientService() : base("/api/client")
    { }

    public string GetIp([FromServices] IHttpContextAccessor httpContext)
    {
        return GetIp(httpContext.HttpContext!.Request.Headers, httpContext.HttpContext.Connection.RemoteIpAddress);
    }

    private static string GetIp(IHeaderDictionary headers, IPAddress? deafultIp)
    {
        if (headers.TryGetValue("X-Forwarded-For", out StringValues value))
        {
            var ip = value.ToString().Split(',')[0].Trim();
            if (ip.Length > 0) return ip;
        }
        if (headers.TryGetValue("X-Real-IP", out value))
        {
            var ip = value.ToString();
            if (ip.Length > 0) return ip;
        }

        return deafultIp?.ToString() ?? string.Empty;
    }
}
