// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared.Apm;

public partial class ApmComponentBase : BDomComponentBase
{
    [Inject]
    public I18n I18n { get; set; }

    [Inject]
    public JsInitVariables JsInitVariables { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public SearchData Search { get; set; }

    public static TimeZoneInfo CurrentTimeZone { get; private set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (CurrentTimeZone == null || CurrentTimeZone.BaseUtcOffset != JsInitVariables.TimezoneOffset)
                CurrentTimeZone = TimeZoneInfo.CreateCustomTimeZone("user custom timezone", JsInitVariables.TimezoneOffset, default, default);
        }
        base.OnAfterRender(firstRender);
    }

    protected virtual bool IsPage { get; set; } = false;

    public ApmComponentBase()
    {
    }

    protected override void OnInitialized()
    {
        if (IsPage)
        {
            Search.Text = default!;
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var values = HttpUtility.ParseQueryString(uri.Query);
            var start = values.Get("start");
            var end = values.Get("end");
            if (DateTime.TryParse(start, out DateTime startTime) && DateTime.TryParse(end, out DateTime endTime) && endTime > startTime)
            {
                Search.Start = startTime.ToDateTimeOffset(default).UtcDateTime;
                Search.End = endTime.ToDateTimeOffset(default).UtcDateTime;
            }
            var service = values.Get("service");
            var env = values.Get("env");
            if (!string.IsNullOrEmpty(env))
                Search.Environment = env;
            if (!string.IsNullOrEmpty(service))
                Search.Service = service;

            var endpoint = values.Get("endpoint");
            if (!string.IsNullOrEmpty(endpoint))
                Search.Endpoint = endpoint;

            var compare = values.Get("comparison");
            if (Enum.TryParse(compare, out ApmComparisonTypes type))
                Search.ComparisonType = type;

            var search = values.Get("search");
            if (!string.IsNullOrEmpty(search))
                Search.Text = search;
            var traceId = values.Get("traceId");
            if (!string.IsNullOrEmpty(traceId))
            {
                var text = $"TraceId='{traceId}'";
                if (string.IsNullOrEmpty(Search.Text) || !Search.Text.Contains(text))
                {
                    Search.Text = $"{Search.Text} and {text}";
                }
            }

            var spanId = values.Get("spanId");
            if (!string.IsNullOrEmpty(spanId))
            {
                var text = $"SpanId='{spanId}'";
                if (string.IsNullOrEmpty(Search.Text) || !Search.Text.Contains(text))
                {
                    Search.Text = $"{Search.Text} and {text}";
                }
            }

            if (!string.IsNullOrEmpty(Search.Text) && Search.Text.Trim().StartsWith("and "))
            {
                Search.Text = Search.Text.Trim()[4..];
            }
        }
        base.OnInitialized();
    }

    public string GetUrlParam(string? service = default,
         string? env = default,
         string? endpoint = default,
         DateTime? start = default,
         DateTime? end = default,
         ApmComparisonTypes? comparisonType = default,
         string? exType = default,
         string? exMsg = default,
         string? traceId = default,
         string? spanId = default,
         string? search = default)
    {
        var text = new StringBuilder();
        if (!string.IsNullOrEmpty(env))
            text.AppendFormat("&env={0}", HttpUtility.UrlEncode(env));
        if (!string.IsNullOrEmpty(service))
            text.AppendFormat("&service={0}", HttpUtility.UrlEncode(service));
        if (!string.IsNullOrEmpty(endpoint))
            text.AppendFormat("&endpoint={0}", HttpUtility.UrlEncode(endpoint));
        if (comparisonType.HasValue)
            text.AppendFormat("&comparison={0}", (int)comparisonType);
        if (start.HasValue && start.Value > DateTime.MinValue)
            text.AppendFormat("&start={0}", HttpUtility.UrlEncode(start.Value.ToString("yyyy-MM-dd HH:mm:ss")));
        if (end.HasValue && end.Value > DateTime.MinValue)
            text.AppendFormat("&end={0}", HttpUtility.UrlEncode(end.Value.ToString("yyyy-MM-dd HH:mm:ss")));
        if (!string.IsNullOrEmpty(exType))
            text.AppendFormat("&ex_type={0}", HttpUtility.UrlEncode(exType));
        if (!string.IsNullOrEmpty(exMsg))
            text.AppendFormat("&ex_msg={0}", HttpUtility.UrlEncode(exMsg).Replace(".", "x2E"));
        if (!string.IsNullOrEmpty(traceId))
            text.AppendFormat("&traceId={0}", HttpUtility.UrlEncode(traceId));
        if (!string.IsNullOrEmpty(spanId))
            text.AppendFormat("&spanId={0}", HttpUtility.UrlEncode(spanId));
        if (!string.IsNullOrEmpty(search))
            text.AppendFormat("&search={0}", HttpUtility.UrlEncode(search));

        if (text.Length > 0)
            text.Remove(0, 1).Insert(0, "?");
        return text.Remove(0, 1).Insert(0, "?").ToString();
    }

    public static string? GetSearchEnv(string? selectEnv, params string[]? envs)
    {
        if (!string.IsNullOrEmpty(selectEnv))
            return selectEnv;
        if (envs != null && envs.Length == 1)
            return envs[0];
        return default;
    }
}
