// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Endpoints;

public partial class TimeLine
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public List<TraceResponseDto> Data { get; set; }

    [Parameter]
    public List<ChartPointDto> Errors { get; set; }

    [Parameter]
    public double Percentile { get; set; }

    [Parameter]
    public int Page { get; set; } = 1;

    [Parameter]
    public int Total { get; set; } = 1;

    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnSpanIdChanged { get; set; }

    private string? lastKey = default;
    private bool loading = true;
    private int totalDuration = 0;
    private int stepDuration = 0;
    private int lastDuration;
    private List<TreeLineDto> timeLines = new();
    DateTime start, end;
    bool showTimeLine = true;
    int[] errorStatus = Array.Empty<int>();

    bool showTraceDetail = false;
    TreeLineDto? currentTimeLine = null;
    TreeLineDto defaultTimeLine = null;
    List<string> services = new();
    string? traceLinkUrl = default, spanLinkUrl = default;
    private string urlService, UrlEndpoint, urlMethod;

    private static List<string> colors = new() {
        "rgb(84, 179, 153)",
        "#5b9bd5","#ed7d31","#70ad47","#ffc000","#4472c4","#91d024","#b235e6","#02ae75",
        "#a565ef","#628cee","#eb9358","#bb60b2","#433e7c","#f47a75","#009db2","#024b51","#0780cf","#765005"
    };

    string ShowTimeLineIcon
    {
        get
        {
            return $"fa:fas fa-angles-{(showTimeLine ? "down" : "up")}";
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var str = $"{JsonSerializer.Serialize(Data)}";
        var key = MD5Utils.Encrypt(str);
        if (lastKey != key)
        {
            loading = true;
            lastKey = key;
            await CaculateTimelines(Data?.ToList());
            loading = false;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var values = HttpUtility.ParseQueryString(uri.Query);
        urlService = values.Get("service")!;
        UrlEndpoint = values.Get("endpoint")!;
        urlMethod = values.Get("method")!;
    }

    protected override async Task OnInitializedAsync()
    {
        errorStatus = await ApiCaller.TraceService.GetErrorStatusAsync();
        await base.OnInitializedAsync();
    }

    private async Task CaculateTimelines(List<TraceResponseDto>? traces)
    {
        traceLinkUrl = default;
        spanLinkUrl = default;
        timeLines.Clear();
        services.Clear();
        if (traces == null || !traces.Any())
            return;

        var spanIds = new List<string>();
        var index = 0;
        do
        {
            if (!spanIds.Contains(traces[index].SpanId))
            {
                spanIds.Add(traces[index].SpanId);
                index++;
            }
            else
                traces.RemoveAt(index);
        }
        while (traces.Count - index >= 1);
        //长连接，只取前100个
        int limit = traces.Count;
        if (limit - 100 >= 0)
        {
            limit = 100;
        }

        start = traces.Min(item => item.Timestamp);
        end = traces.Max(item => item.EndTimestamp);
        CaculateTotalTime(start, end);

        do
        {
            SetRootTimeLine(traces);
        }
        while (traces.Count > 0);
        defaultTimeLine = GetDefaultLine(timeLines)!;
        if (OnSpanIdChanged.HasDelegate)
            await OnSpanIdChanged.InvokeAsync(defaultTimeLine?.Trace?.SpanId);
        timeLines = timeLines.OrderBy(item => item.Trace.Timestamp).ToList();
        services = services.Distinct().ToList();
        traceLinkUrl = GetUrl(defaultTimeLine);
    }

    private TreeLineDto? GetDefaultLine(List<TreeLineDto>? data)
    {
        if (data == null || !data.Any())
            return default;
        foreach (var item in data)
        {
            if (item.Trace.Resource["service.name"].ToString() == urlService
                && item.Trace.Kind == "SPAN_KIND_SERVER"
                && item.Trace.Attributes.ContainsKey("http.target") && item.Trace.Attributes.ContainsKey("http.method")
                && item.Trace.Attributes["http.target"].ToString() == UrlEndpoint && item.Trace.Attributes["http.method"].ToString() == urlMethod)
            {
                return item;
            }
            var find = GetDefaultLine(item.Children);
            if (find != null) return find;
        }
        return default;
    }

    private void SetRootTimeLine(List<TraceResponseDto> traces)
    {
        var roots = GetEmptyParentNodes(traces!);
        if (!roots.Any())
        {
            roots = GetInteruptRootNodes(traces!);
            if (!roots.Any())
                roots = traces.OrderBy(t => t.Timestamp).ToList();
        }
        services.AddRange(traces.Select(item => item.Resource["service.name"].ToString())!);

        foreach (var item in roots)
        {
            traces.Remove(item);
            var timeLine = new TreeLineDto
            {
                ParentSpanId = item.ParentSpanId,
                SpanId = item.SpanId,
                Trace = item,
                Children = GetChildren(item.SpanId, traces)
            };
            timeLine.SetValue(item, start, end, totalDuration, errorStatus);
            var spanError = Errors?.FirstOrDefault(t => t.X == item.SpanId);
            if (spanError != null)
            {
                timeLine.ErrorCount = Convert.ToInt32(spanError.Y);
            }
            timeLines.Add(timeLine);
        }
    }

    private List<TreeLineDto> GetChildren(string spanId, List<TraceResponseDto> traces)
    {
        var children = traces.Where(item => !string.IsNullOrEmpty(item.ParentSpanId) && item.ParentSpanId == spanId).ToList();
        if (!children.Any()) return default!;
        var result = new List<TreeLineDto>();
        foreach (var item in children)
        {
            traces.Remove(item);
            var timeLine = new TreeLineDto
            {
                ParentSpanId = item.ParentSpanId,
                SpanId = item.SpanId,
                Trace = item,
                Children = GetChildren(item.SpanId, traces)
            };
            timeLine.SetValue(item, start, end, totalDuration, errorStatus);
            result.Add(timeLine);
        }
        return result;
    }

    private async Task LoadPageAsync(int page)
    {
        Page = page;
        loading = true;
        if (PageChanged.HasDelegate)
            await PageChanged.InvokeAsync(Page);
    }

    /// <summary>
    /// 完整的parentId为空的roots
    /// </summary>
    /// <param name="traces"></param>
    /// <returns></returns>
    private List<TraceResponseDto> GetEmptyParentNodes(List<TraceResponseDto> traces)
    {
        return traces.Where(item => string.IsNullOrEmpty(item.ParentSpanId)).OrderBy(t => t.Timestamp).ToList();
    }

    private void ShowOrHideLine(TreeLineDto item)
    {
        showTimeLine = true;
        item.Show = !item.Show;
    }

    private void ShowOrHideAll()
    {
        showTimeLine = !showTimeLine;
        //StateHasChanged();
    }

    private void ShowTraceDetail(TreeLineDto current)
    {
        spanLinkUrl = GetUrl(current, true);
        currentTimeLine = current;
        showTraceDetail = true;
    }

    /// <summary>
    /// 截断的trace,取最外面的作为roots
    /// </summary>
    /// <param name="traces"></param>
    /// <returns></returns>
    private List<TraceResponseDto> GetInteruptRootNodes(List<TraceResponseDto> traces)
    {
        //var parentIds = traces.Select(item => item.ParentSpanId).Distinct().ToList();
        var allSpanIds = traces.Select(item => item.SpanId).Distinct().ToList();
        var roots = traces.Where(item => !allSpanIds.Contains(item.ParentSpanId));
        return roots.OrderBy(t => t.Timestamp).ToList();
    }

    private void CaculateTotalTime(DateTime start, DateTime end)
    {
        totalDuration = (int)Math.Floor((end - start).TotalMilliseconds);
        stepDuration = totalDuration / 8;
        lastDuration = totalDuration;
    }

    private string GetUrl(TreeLineDto current, bool isSpan = false, string baseUrl = "/apm/logs")
    {
        if (current == null)
            return string.Empty;
        string spanId = current.Trace.SpanId;
        return $"{baseUrl}{GetUrlParam(service: current.Trace.Resource["service.name"].ToString(), env: current.Trace.Resource["service.namespace"].ToString(), start: start.AddSeconds(-1), end: end.AddSeconds(1), traceId: current.Trace.TraceId, spanId: isSpan ? spanId : default)}";
    }

    private async Task OpenLogAsync(TreeLineDto item)
    {
        var url = GetUrl(item, true, "/apm/errors");
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    private async Task OpenTraceLogAsync()
    {
        await JSRuntime.InvokeVoidAsync("open", traceLinkUrl, "_blank");
    }
}

public class TreeLineDto
{
    public string SpanId { get; set; }

    public string ParentSpanId { get; set; }

    public List<TreeLineDto> Children { get; set; }

    public bool IsClient { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public string Label
    {
        get
        {
            return $"{Type} {Name} {Latency}";
        }
    }

    public string NameClass { get; set; }

    public bool Faild { get; set; } = false;

    public bool IsError
    {
        get
        {
            return ErrorCount > 0 || !string.IsNullOrEmpty(ErrorMessage);
        }
    }

    /// <summary>
    /// 多条error
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// 单条error
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 单位毫秒
    /// </summary>
    public string Latency => ((double)Trace.Duration).FormatTime();

    public string Icon { get; set; }

    public TraceResponseDto Trace { get; set; }

    public int Left { get; set; }

    public int Right { get; set; }

    public int Process { get; set; }

    public bool Show { get; set; } = true;

    public string ShowIcon
    {
        get
        {
            return $"fa:fas fa-chevron-{(Show ? "down" : "up")}";
        }
    }

    public void SetValue(TraceResponseDto trace, DateTime start, DateTime end, int total, int[] errorStatus)
    {
        if (trace.TryParseDatabase(out var database))
        {
            IsClient = true;
            Name = $"{database.Name}";
            Icon = "fa:fas fa-database";

            var sqlKey = "db.statement";
            if (trace.Attributes.ContainsKey(sqlKey))
            {
                var regAction = @"(?<=\s*)(select|update|insert|delete)(?=\s+)";
                var sql = trace.Attributes[sqlKey].ToString();
                var action = Regex.Match(sql!, regAction, RegexOptions.IgnoreCase).Value;
                string table = "unkown";
                if (!string.IsNullOrEmpty(action))
                {
                    bool isSelect = action.Equals("select", StringComparison.CurrentCultureIgnoreCase);
                    var regTable = @$"(?<={(isSelect ? "from" : action)}\s+[\[`])\S+(?=[`\]]\s*)";
                    var regTable2 = @$"(?<={(isSelect ? "from" : action)}\s+)\S+(?=\s*)";
                    var matches = Regex.Matches(sql, regTable, RegexOptions.IgnoreCase);
                    if (matches.Count == 0) matches = Regex.Matches(sql, regTable2, RegexOptions.IgnoreCase);

                    if (matches.Count > 0)
                        table = matches[0].Value;
                }
                else
                {
                    table = database.System;
                }

                Type = $"{action} {table}";
            }

        }
        else if (trace.TryParseHttp(out var http))
        {
            IsClient = trace.Kind == "SPAN_KIND_CLIENT";
            bool isMaui = trace.Attributes.ContainsKey("client.type") && trace.Attributes["client.type"].ToString() == "maui-blazor";
            bool isDapr = http.Url.StartsWith("http://127.0.0.1:3500/");
            if (isMaui)
            {
                if (trace.Attributes.ContainsKey("client.title") && trace.Attributes["client.title"].ToString()!.Length > 0)
                    Name = trace.Attributes["client.title"].ToString()!;
                else if (trace.Attributes.ContainsKey("http.target"))
                    Name = trace.Attributes["http.target"].ToString()!;
                else
                    Name = http.Url;
                Icon = "fas fa-mobile";
                Type = "MAUI Client";
            }
            else
            {
                Name = $"{http.Method} {http.Url} ";
                if (isDapr && IsClient)
                {
                    Icon = "fas fa-grip";
                    Type = $"Dapr Client {http.StatusCode}";
                }
                else
                {
                    Icon = "md:http";
                    Type = $"Http {(IsClient ? "Client " : "")} {http.StatusCode}";
                }
            }

            NameClass = "font-weight-black";
            Faild = errorStatus.Contains(http.StatusCode);
        }
        else
        {
            Name = trace.Name;
        }
        if (trace.TryParseException(out var exception))
        {
            Faild = true;
        }
        if (total == 0)
        {
            Process = 100;
        }
        else
        {
            Left = (int)Math.Floor(Math.Floor((trace.Timestamp - start).TotalMilliseconds) * 100 / total);
            Process = (int)Math.Floor(Math.Floor((trace.EndTimestamp - trace.Timestamp).TotalMilliseconds) * 100 / total);
            if (Process - 1 < 0) Process = 1;
            if (Process + Left - 100 > 0)
            {
                Left = 100 - Process;
            }
            else
            {
                Right = 100 - Left - Process;
                if (Right < 0)
                {
                    Right = 0;
                    Left = 100 - Process;
                }
            }
        }
    }
}
