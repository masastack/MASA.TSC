// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.User;
using System.Collections.Specialized;

namespace Masa.Tsc.Web.Admin.Rcl.Pages.App;

public partial class Index
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }
    private Guid _userId;
    private string _searchText;
    private bool isSearchEnd = false;
    private List<UserRoleDto>? roles = default;
    private TraceResponseDto? firstTrace;
    private PhoneModelDto? phoneModel;
    private Dictionary<string, object>? claims = default;
    private DateTime end, start;
    private QuickRangeKey quickRangeKey = QuickRangeKey.Last5Minutes;
    private StringNumber index;
    private string serviceName;
    private List<string> services;
    private bool claimShow = false;
    private List<TraceResponseDto> traceLines;
    private IJSObjectReference? module;

    private string? spanId;
    private DateTime? logTime;
    private bool firstLoaded = false;
    private NameValueCollection? values;
    private bool dataLoading = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/Masa.Tsc.Web.Admin.Rcl/Pages/App/Index.razor.js");
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        services = new();
        traceLines = new();
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri.ToNomalBlazorUrl());
        values = HttpUtility.ParseQueryString(uri.Query);
        string
            start = values.Get("start"), end = values.Get("end"),
            userId = values.Get("userId"),
            logTime = values.Get("logTime"),
            tabIndex = values.Get("index");
        serviceName = values.Get("service");
        if (DateTime.TryParse(start, default, default, out DateTime startTime) && DateTime.TryParse(end, default, default, out DateTime endTime) && endTime > startTime)
        {
            (this.start, this.end) = (startTime.ToDateTimeOffset(default).UtcDateTime, endTime.ToDateTimeOffset(default).UtcDateTime);
        }
        if (userId != null && Guid.TryParse(userId, out var guid))
            _userId = guid;
        spanId = values.Get("spanId");
        _searchText = values.Get("search");
        if (tabIndex != null && int.TryParse(tabIndex, out var num) && num >= 1 && num - 4 <= 0)
        {
            index = num;
        }
        if (DateTime.TryParse(logTime, default, default, out var time))
            this.logTime = time;
        if (values.Count == 0)
        {
            this.end = DateTime.UtcNow;
            this.start = this.end.AddMinutes(-1);
            index = 1;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadService();
        if (!string.IsNullOrEmpty(serviceName) && !services.Contains(serviceName))
            serviceName = default!;
        if (values != null && values.Count > 0)
        {
            await LoadTrace();
            await LoadTraceTimeLines();
            StateHasChanged();
        }
    }

    private string ClientType()
    {
        if (firstTrace == null) return default;
        return firstTrace.Resource.TryGetValue("device.Idiom", out var idiom) ? idiom.ToString() : default;
    }

    private string PhoneModel()
    {
        if (firstTrace == null) return default;
        if (phoneModel != null)
        {
            return $"{(phoneModel.ModeName.Contains(phoneModel.BrandName) ? "" : phoneModel.BrandName)} {phoneModel.ModeName}";
        }

        if (firstTrace.Resource.TryGetValue("device.manufacturer", out var brand) && firstTrace.Resource.TryGetValue("device.model", out var model))
        {
            return $"未知({brand} {model})";
        }
        return "无";
    }

    private string ClientOs()
    {
        if (firstTrace == null) return default;
        return firstTrace.Resource.TryGetValue("device.platform", out var platform) && firstTrace.Resource.TryGetValue("device.version", out var version) ? $"{platform} {version}" : default;
    }

    //auth获取
    private string RolesNames()
    {
        if (roles == null || !roles.Any())
            return default;
        return string.Join(", ", roles.Select(role => role.Name));
    }

    private void SearchValueChange(string value)
    {
        _searchText = value;
        isSearchEnd = false;
    }

    private void OnErrorChanged()
    {
        isSearchEnd = true;
    }

    private bool IsSearchKeyword(object value)
    {
        if (!isSearchEnd || string.IsNullOrEmpty(_searchText))
            return false;
        bool isGuid = Guid.TryParse(_searchText, out var _);
        bool isNum = int.TryParse(_searchText, out var _);

        if (value is OperationLineTraceModel trace)
        {
            return isGuid && (trace.Data.TraceId == _searchText || trace.Data.SpanId == _searchText)
                || isNum && (trace.Data.Attributes.TryGetValue("http.status_code", out var http_status_code) && http_status_code != null && http_status_code.ToString() == _searchText)
                || trace.Data.Attributes["http.url"].ToString()!.Contains(_searchText)
                || trace.Data.Attributes.TryGetValue("http.request.content_body", out var body) && body != null && body.ToString()!.Contains(_searchText);
        }
        else if (value is OperationLineLogModel log)
        {
            return isGuid && (log.Data.TraceId == _searchText || log.Data.SpanId == _searchText)
                || log.Data.Body.ToString()!.Contains(_searchText) || log.IsError && log.Data.Attributes["exception.message"].ToString()!.Contains(_searchText);
        }
        return false;
    }

    private async Task UserChange(Guid userId)
    {
        _userId = userId;
        roles = await ApiCaller.UserService.GetUserRolesAsync(userId);
        var claims = await ApiCaller.UserService.GetUserClaimAsync(userId);
        var claimTypes = await ApiCaller.UserService.GetClaimsAsync();
        if (claims != null && claims.Count > 0 && claimTypes != null && claimTypes.Count > 0)
        {
            var keys = claims.Keys.ToList();

            foreach (var key in keys)
            {
                var declare = claimTypes.Find(item => item.Name == key);
                if (string.IsNullOrEmpty(declare.Description) || declare.Description == key)
                    continue;
                var value = claims[key];
                claims.Remove(key);
                claims.Add($"{key}({declare.Description})", value);
            }
        }
        this.claims = claims;
        data.Clear();
        await LoadTrace();
    }

    private async Task ServiceChange(string service)
    {
        serviceName = service;
        await LoadTrace();
    }

    private async Task OnTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) time)
    {
        (start, end) = (time.start!.Value.UtcDateTime, time.end!.Value.UtcDateTime);
        data.Clear();
        await LoadTrace();
    }

    private async Task OnAutoTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) time)
    {
        (start, end) = (time.start!.Value.UtcDateTime, time.end!.Value.UtcDateTime);
        await LoadTrace();
    }

    private async Task LoadTrace()
    {
        if (_userId == Guid.Empty || string.IsNullOrEmpty(serviceName))
        {
            UpdateSearch();
            currentTrace = default;
            currentLog = default;
            return;
        }

        dataLoading = true;
        StateHasChanged();
        var query = new BaseRequestDto
        {
            Start = start,
            End = end,
            Service = serviceName,
            Page = 1,
            PageSize = 200,
            Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                    Name =StorageConst.UserId,
                     Type= ConditionTypes.Equal,
                      Value =_userId.ToString().ToLower()
                }
            },
            Sort = new FieldOrderDto
            {
                Name = "@timestamp",
                IsDesc = false
            }
        };
        var traces = await ApiCaller.ApmService.GetTraceListAsync(query);
        await SetDeviceModel(traces.Result?.FirstOrDefault());
        SetTraceData(traces.Result!);
        await LoadLog(traces.Result?.Select(item => item.SpanId).Distinct().ToList()!);
        if (currentTrace == null && data.Count > 0)
            currentTrace = data[0];
        else if (data.Count == 0)
        {
            currentTrace = default;
            currentLog = default;
        }
        UpdateSearch();
        dataLoading = false;
        StateHasChanged();
    }

    private async Task LoadService()
    {
        services.Clear();
        var data = await ApiCaller.TraceService.GetAttrValuesAsync(new SimpleAggregateRequestDto
        {
            Start = DateTime.UtcNow.AddDays(-1),
            Conditions = new List<FieldConditionDto>
            {
                new FieldConditionDto{
                    Name="ScopeName",
                    Type= ConditionTypes.Equal,
                    Value="MAUI"
                }
            },
            Name = StorageConst.ServiceName,
            Type = AggregateTypes.GroupBy
        });
        if (data != null)
            services.AddRange(data);
    }

    private async Task SetDeviceModel(TraceResponseDto? trace)
    {
        firstTrace = trace;
        phoneModel = null;
        if (trace != null && trace.Resource.TryGetValue("device.manufacturer", out var brand) && trace.Resource.TryGetValue("device.model", out var model))
        {
            phoneModel = await ApiCaller.ApmService.GetDeviceModelAsync(brand.ToString()!, model.ToString()!);
        }
    }

    private void SetTraceData(IEnumerable<TraceResponseDto> traces)
    {
        if (traces == null || !traces.Any()) return;
        foreach (var trace in traces)
        {
            if (data.Exists(item => item.Data.TraceId == trace.TraceId))
                continue;
            var @new = new OperationLineTraceModel(trace);
            data.Add(@new);
        }
    }

    private async Task LoadLog(List<string> spanIds)
    {
        if (spanIds == null || !spanIds.Any())
            return;
        var query = new BaseRequestDto
        {
            Start = start,
            End = end,
            Service = serviceName,
            Page = 1,
            PageSize = 200,
            Conditions = new List<FieldConditionDto> {
                //new FieldConditionDto{
                //    Name =StorageConst.TraceId,
                //     Type= ConditionTypes.In,
                //      Value =traceIds
                //},
                new FieldConditionDto{
                    Name=StorageConst.SpanId,
                    Type = ConditionTypes.In,
                    Value = spanIds
                }
            },
            Sort = new FieldOrderDto
            {
                Name = "@timestamp",
                IsDesc = false
            }
        };
        var logs = await ApiCaller.ApmService.GetLogListAsync(query);

        if (logs == null || logs.Result == null || logs.Result.Count == 0)
            return;

        SetLogData(logs.Result);
    }

    private void SetLogData(List<LogResponseDto> logs)
    {
        do
        {
            var first = logs[0];
            var trace = data.Find(trace => trace.Data.SpanId == first.SpanId);
            if (trace == null)
            {
                //该页面报错了,先不处理
                logs.Remove(first);
            }
            else
            {
                var childLogs = logs.Where(log => log.SpanId == first.SpanId).ToList();
                SetChild(trace, childLogs);
                logs.RemoveAll(childLogs);
            }
        }
        while (logs.Count > 0);
    }

    private static void SetChild(OperationLineTraceModel trace, List<LogResponseDto> logs)
    {
        if (logs == null || !logs.Any()) return;
        bool hasData = trace.Children != null;
        trace.Logs.AddRange(logs);

        var children = logs.Where(log => log.Attributes.ContainsKey("Time") && (log.Attributes.ContainsKey("Label") || log.Attributes.ContainsKey("Name") || log.Attributes.ContainsKey("EventName")));
        if (children.Any())
        {
            if (!hasData)
                trace.Children = children.Select(child => new OperationLineLogModel(child)).ToList();
            else
            {
                foreach (var child in children)
                {
                    if (trace.Children!.Exists(r => r.Time == child.Timestamp))
                        continue;
                    trace.Children.Add(new OperationLineLogModel(child));
                }
            }
        }
    }

    private async Task Share()
    {
        var str = $"apm/app?userId={_userId}&service={serviceName}&start={end}&end={start}&search={_searchText}&spanId={currentTrace?.Data?.SpanId}&logTime={currentLog?.Time}&tab={index}".ToSafeBlazorUrl();
        await Js.InvokeVoidAsync(JsInteropConstants.Copy, $"{NavigationManager.BaseUri}{str}");
        await Task.Delay(500);
    }

    private async Task Top()
    {
        await module!.InvokeVoidAsync("setPosition", true);
    }

    private async Task Bottom()
    {
        await module!.InvokeVoidAsync("setPosition", false);
    }

    private async Task Pre()
    {
        start = start.AddMinutes(-1);
        end = end.AddMinutes(-1);
        data.Clear();
        await LoadTrace();
    }

    private async Task Next()
    {
        start = start.AddMinutes(1);
        end = end.AddMinutes(1);
        data.Clear();
        await LoadTrace();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        module?.DisposeAsync();
        return base.DisposeAsyncCore();
    }

    private async Task OnTabIndexChange(StringNumber current)
    {
        index = current;
        await LoadTraceTimeLines();
    }

    private async Task LoadTraceTimeLines()
    {
        if (index == 2 && currentTrace != null)
        {
            if (traceLines.Count == 0 || traceLines[0].TraceId != currentTrace.Data.TraceId)
            {
                var traces = await ApiCaller.TraceService.GetAsync(currentTrace.Data.TraceId);
                if (traces != null)
                    traceLines = traces.ToList();
                else
                    traceLines.Clear();
                StateHasChanged();
            }
        }
    }

    private void UpdateSearch()
    {
        Search.Start = start;
        Search.End = end;
        Search.Service = serviceName;
        if (currentTrace != null)
        {
            if (currentTrace.Data.Resource.TryGetValue("service.namespace", out var env))
            {
                Search.Environment = env.ToString();
                Search.Text = $"TraceId='{currentTrace.Data.TraceId}'";
            }
        }
        else
        {
            Search.Environment = default!;
        }
    }
}
