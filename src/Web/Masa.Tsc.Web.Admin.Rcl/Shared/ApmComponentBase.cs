// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Shared.Apm;

public partial class ApmComponentBase : MasaComponentBase
{
    [Inject]
    public JsInitVariables JsInitVariables { get; set; }

    [Inject]
    public TscCaller ApiCaller { get; set; }

    [Inject]
    public virtual SearchData Search { get; set; }

    [Inject]
    public GlobalConfig GlobalConfig { get; set; } = default!;

    public static TimeZoneInfo CurrentTimeZone { get; private set; }

    public static ApmSearchComponent ApmSearchComponent { get; set; }

    public static EnvironmentAppDto? GetService(string? service, Func<string, EnvironmentAppDto?>? func = default)
    {
        if (string.IsNullOrEmpty(service)) return default;
        if (func != null)
        {
            var value = func.Invoke(service);
            if (value != null)
                return value;
        }

        if (ApmSearchComponent != null)
            return ApmSearchComponent.GetService(service);
        return null;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && (CurrentTimeZone == null || CurrentTimeZone.BaseUtcOffset != JsInitVariables.TimezoneOffset))
            CurrentTimeZone = TimeZoneInfo.CreateCustomTimeZone("user custom timezone", JsInitVariables.TimezoneOffset, default, default);

        base.OnAfterRender(firstRender);
    }

    protected virtual bool IsPage { get; set; } = false;

    protected virtual bool IsServicePage { get; }
    protected virtual bool IsEndPointPage { get; }
    protected virtual bool IsErrorPage { get; set; }

    public ApmComponentBase()
    {

    }

    private async Task SetStorage()
    {
        if (StorageConst.Current != null) return;
        var setting = await ApiCaller.SettingService.GetStorage();
        if (setting == null)
            throw new InvalidDataException("Storage setting is null");
        if (setting.IsClickhouse)
            StorageConst.Init(new ClickhouseStorageConst());
        else if (setting.IsElasticsearch)
            StorageConst.Init(new ElasticsearchStorageConst());
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (IsPage)
            await SetStorage();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (StorageConst.Current == null) return;
        if (IsPage)
        {
            Search.Loaded = false;
            if (IsServicePage)
            {
                Search.Project = default!;
                Search.Service = default!;
            }
            if (IsServicePage || IsEndPointPage)
            {
                Search.Method = default!;
                Search.Endpoint = default!;
            }
            Search.ExceptionType = nameof(AppTypes.Service);
            Search.ExceptionMsg = default!;
            Search.TraceId = default!;
            Search.TextField = default!;
            Search.TextValue = default!;
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

            Search.TraceId = values.Get("traceId")!;
            Search.Status = values.Get("status")!;
            Search.Method = values.Get("method")!;
            Search.SpanId = values.Get("spanId")!;
            Search.ExceptionType = values.Get("ex_type")!;
            Search.ExceptionMsg = values.Get("ex_msg")!;
        }
    }

    public static string GetUrlParam(string? service = default,
         string? env = default,
         string? endpoint = default,
         DateTime? start = default,
         DateTime? end = default,
         ApmComparisonTypes? comparisonType = default,
         string? exType = default,
         string? exMsg = default,
         string? traceId = default,
         string? spanId = default,
         string? search = default,
         string? method = default,
         string? statusCode = default)
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
        if (!string.IsNullOrEmpty(method))
            text.AppendFormat("&method={0}", method);
        if (!string.IsNullOrEmpty(statusCode))
            text.AppendFormat("&status={0}", statusCode);

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
