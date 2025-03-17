// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.App;

public partial class Index
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }
    private Guid _userId;
    private string _searchText;
    private bool isSearchEnd = false;
    private List<UserRoleDto>? roles = default;
    private UserModel? user = default;
    private TraceResponseDto? firstTrace;
    private PhoneModelDto? phoneModel;
    private Dictionary<string, object> claims = new();
    private DateTime end, start;
    private QuickRangeKey quickRangeKey = QuickRangeKey.Last5Minutes;
    private StringNumber index;
    private string serviceName;
    private List<ValueTuple<string, string>> services;
    private static Dictionary<string, List<EnvironmentAppDto>> _teamServices = new();
    private bool claimShow = false;
    private List<TraceResponseDto> traceLines;
    private IJSObjectReference? module = null;

    private string? spanId;
    private DateTime? logTime;
    private NameValueCollection? values;
    private bool dataLoading = false;
    private bool isNeedSetPosition = false;
    private static readonly Regex webviewReg = new(@"Chrome/(\d+\.?)+", default, TimeSpan.FromSeconds(1));
    protected override bool IsPage => true;

    public static EnvironmentAppDto? GetService(string service)
    {
        foreach (var item in _teamServices)
        {
            var app = item.Value.Find(item => item.AppId == service);
            if (app != null) return app;
        }
        return default;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/_content/Masa.Tsc.Web.Admin.Rcl/Pages/App/Index.razor.js");
        }
        await base.OnAfterRenderAsync(firstRender);
        if (isNeedSetPosition && data.Count > 0)
        {
            await SetRecord();
            await OnTabIndexChange(index);
            isNeedSetPosition = false;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        services = new();
        traceLines = new();
        var uri = NavigationManager.ToAbsoluteUri(CurrentUrl.ToNomalBlazorUrl());
        values = HttpUtility.ParseQueryString(uri.Query);
        Search.Service = string.Empty;
        Search.Endpoint = string.Empty;
        Search.Environment = string.Empty;
        Search.TraceId = string.Empty;
        string
            start = values.Get("start"), end = values.Get("end"),
            userId = values.Get("userId"),
            logTime = values.Get("logTime"),
            tabIndex = values.Get("tab"),
            treeLineIndex = values.Get("treeIndex");
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
        if (treeLineIndex != null && int.TryParse(treeLineIndex, out num) && num > 0)
        {
            treeIndex = num;
            isNeedSetPosition = true;
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
        if (!string.IsNullOrEmpty(serviceName) && !services.Exists(item => item.Item1 == serviceName))
            serviceName = default!;
        isLoadService = false;
        if (values != null && values.Count > 0)
        {
            await LoadUserClaimsAsync();
            await LoadTrace(clearIndex: false);
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
        if (firstTrace == null) return default!;
        return firstTrace.Resource.TryGetValue("device.platform", out var platform) && firstTrace.Resource.TryGetValue("device.version", out var version) ? $"{platform} {version}" : default!;
    }

    private string WebviewVersion()
    {
        if (firstTrace == null) return default!;
        if (!firstTrace.Attributes.TryGetValue("client.user_agent", out var value)) return default!;
        var userAgent = value.ToString();
        if (string.IsNullOrEmpty(userAgent)) return default!;
        if (!webviewReg.IsMatch(userAgent)) return default!;
        return webviewReg.Match(userAgent).Value.Split('/')[1];
    }

    //auth获取
    private string RolesNames()
    {
        if (roles == null || !roles.Any() || user == null || user.Roles == null || user.Roles.Count == 0)
            return default;
        return string.Join(", ", roles.Where(role => user.Roles.Exists(r => r.Id == role.Id)).Select(role => role.Name));
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
        if (_userId == userId)
            return;
        ClearData(true);
        _userId = userId;
        user = await ApiCaller.UserService.GetUserDetailAsync(userId);
        if (roles == null || roles.Count == 0)
            roles = await ApiCaller.UserService.GetUserRolesAsync(userId);
        await LoadUserClaimsAsync();
        await LoadTrace();
    }

    private async Task LoadUserClaimsAsync()
    {
        this.claims.Clear();
        if (_userId == Guid.Empty)
            return;
        var claims = await ApiCaller.UserService.GetUserClaimAsync(_userId);
        var claimTypes = await ApiCaller.UserService.GetClaimsAsync();
        if (claims != null && claims.Count > 0 && claimTypes != null && claimTypes.Count > 0)
        {
            var keys = claims.Keys.ToList();

            foreach (var key in keys)
            {
                var declare = claimTypes.Find(item => item.Name == key);
                if (declare == null || string.IsNullOrEmpty(declare.Description) || declare.Description == key)
                    continue;
                var value = claims[key];
                claims.Remove(key);
                claims.Add($"{key}({declare.Description})", value);
            }

            foreach (var key in claims.Keys)
            {
                this.claims.Add(key, claims[key]);
            }
        }
    }

    private async Task ServiceChange(string service)
    {
        serviceName = service;
        ClearData();
        await LoadTrace();
    }

    private async Task OnTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) time)
    {
        if (Search.Start == DateTime.MinValue)
        {
            await LoadService();
        }
        (start, end) = (time.start!.Value.UtcDateTime, time.end!.Value.UtcDateTime);
        data.Clear();
        await LoadTrace();
    }

    private async Task OnAutoTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) time)
    {
        (start, end) = (time.start!.Value.UtcDateTime, time.end!.Value.UtcDateTime);
        data.Clear();
        await InvokeAsync(async () =>
        {
            await LoadTrace();
        });
    }

    private async Task LoadTrace(bool isPre = false, bool isNext = false, int seconds = 0, bool clearIndex = true)
    {
        if (clearIndex)
        {
            treeIndex = 1;
            currentTrace = default;
            currentLog = default;
        }
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
                //app特有查询
                new FieldConditionDto{
                    Name="MAUI"
                },
                new FieldConditionDto{
                    Name =StorageConst.Current.Trace.UserId,
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

        if (isPre)
            query.End = query.Start.AddMinutes(seconds);
        else if (isNext)
            query.Start = query.End.AddMinutes(-seconds);

        var traces = await ApiCaller.ApmService.GetTraceListAsync(query);
        await SetDeviceModel(traces.Result?.LastOrDefault(item => item.Attributes.ContainsKey("client.type")));
        SetTraceData(traces.Result!);
        await LoadLog(traces.Result?.Select(item => item.TraceId).Distinct().ToList()!);
        if (currentTrace == null && data.Count > 0)
            currentTrace = data[treeIndex - 1];
        else if (data.Count == 0)
        {
            currentTrace = default;
            currentLog = default;
        }
        UpdateSearch();
        dataLoading = false;
        StateHasChanged();
    }

    private bool isLoadService = true;
    private async Task LoadService()
    {
        if (services != null && services.Count > 0 || start == DateTime.MinValue)
            return;

        isLoadService = true;
        var data = await ApiCaller.TraceService.GetAttrValuesAsync(new SimpleAggregateRequestDto
        {
            Start = DateTime.UtcNow.AddDays(-1),
            Conditions = new List<FieldConditionDto>
            {
                new FieldConditionDto{
                    Name="ScopeName",
                    Type= ConditionTypes.In,
                    Value=new string[]{ "MAUI","web" }
                }
            },
            Name = StorageConst.Current.ServiceName,
            Type = AggregateTypes.GroupBy
        });
        services = new();
        if (data == null || !data.Any())
        {
            services = new();
            return;
        }
        _teamServices = await ApiCaller.ApmService.GetEnvironmentServiceAsync(GlobalConfig.CurrentTeamId, end.AddDays(-30), end, ignoreTeam: true);
        if (_teamServices != null && _teamServices.Count > 0)
        {
            foreach (var service in data)
            {
                bool find = false;
                foreach (var item in _teamServices)
                {
                    var app = item.Value.Find(app => app.AppId == service);
                    if (app != null)
                    {
                        services.Add(ValueTuple.Create(service, app.AppDescription));
                        find = true;
                        return;
                    }
                }
                if (!find)
                    services.Add(ValueTuple.Create(service, default(string)!));
            }
        }
        //services = data?.ToList() ?? new();
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

    private void ClearData(bool clearUser = false)
    {
        if (clearUser)
        {
            user = null;
            claims.Clear();
        }
        currentLog = null;
        currentTrace = null;
        traceLines.Clear();
        firstTrace = null;
        phoneModel = null;
        data.Clear();
        StateHasChanged();
    }

    private void SetTraceData(IEnumerable<TraceResponseDto> traces)
    {
        if (traces == null || !traces.Any()) return;
        foreach (var trace in traces)
        {
            if (data.Exists(item => item.Data.SpanId == trace.SpanId))
                continue;
            var @new = new OperationLineTraceModel(trace);
            (bool isAppend, int index) = GetPosition(@new.Time);
            if (isAppend)
                data.Add(@new);
            else
                data.Insert(index, @new);
        }
    }

    private (bool append, int index) GetPosition(DateTime time)
    {
        if (data.Count == 0)
            return (true, 0);
        int index = 0, max = data.Count - 1;
        do
        {
            if (data[index].Time > time)
            {
                return (false, index);
            }
            index++;
        }
        while (max - index >= 0);
        return (true, 0);
    }

    private async Task LoadLog(List<string> traceIds)
    {
        if (traceIds == null || !traceIds.Any())
            return;
        var query = new BaseRequestDto
        {
            Start = start,
            End = end,
            Page = 1,
            PageSize = 800,
            Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                    Name=StorageConst.Current.TraceId,
                    Type = ConditionTypes.In,
                    Value = traceIds.Distinct()
                }
            },
            Sort = new FieldOrderDto
            {
                Name = StorageConst.Current.Timestimap,
                IsDesc = false
            }
        };
        var logs = await ApiCaller.ApmService.GetLogListAsync(Guid.Empty, query, ignoreTeam: true);

        if (logs == null || logs.Result == null || logs.Result.Count == 0)
            return;

        SetLogData(logs.Result);
    }

    private void SetLogData(List<LogResponseDto> logs)
    {
        foreach (var trace in data)
        {
            var childLogs = logs.Where(log => log.TraceId == trace.Data.TraceId).ToList();
            SetChild(trace, childLogs);
        }
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
        var str = $"apm/app?userId={_userId}&service={serviceName}&start={start}&end={end}&search={_searchText}&spanId={currentTrace?.Data?.SpanId}&logTime={currentLog?.Time}&tab={index}&treeIndex={treeIndex}".ToSafeBlazorUrl();
        await JSRuntime.InvokeVoidAsync(JsInteropConstants.Copy, $"{NavigationManager.BaseUri}{str}");
        await PopupService.EnqueueSnackbarAsync("分享连接复制成功", AlertTypes.Success, true);
    }

    bool showNormalClient = false;
    private void ShowHideHttpTrace()
    {
        showNormalClient = !showNormalClient;
        StateHasChanged();
    }

    private async Task Top()
    {
        await ChangeCurrent(data[0]);
        await module!.InvokeVoidAsync("setPosition", 1, data.Count);
    }

    private async Task SetRecord()
    {
        await ChangeCurrent(data[treeIndex - 1]);
        await module!.InvokeVoidAsync("setPosition", treeIndex, data.Count);
    }

    private async Task Bottom()
    {
        await ChangeCurrent(data[data.Count - 1]);
        await module!.InvokeVoidAsync("setPosition", data.Count, data.Count);
    }

    private async Task Pre()
    {
        start = start.AddMinutes(-1);
        await LoadTrace(isPre: true, seconds: 60);
    }

    private async Task Next()
    {
        end = end.AddMinutes(1);
        await LoadTrace(isNext: true, seconds: 60);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (module != null)
            await module.DisposeAsync();
        await base.DisposeAsyncCore();
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
                var traces = await ApiCaller.TraceService.GetAsync(currentTrace.Data.TraceId, currentTrace.Data.Timestamp.AddHours(-6), currentTrace.Data.EndTimestamp.AddHours(6));
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
        Search.Service = default;
        Search.Environment = default!;
        Search.Endpoint = default!;
        Search.TraceId = currentTrace?.Data.TraceId!;
    }

    private static string GetVersion(Dictionary<string, object>? dic)
    {
        if (dic == null || dic.Count == 0)
            return string.Empty;
        if (dic.TryGetValue("service.version", out var version) && version != null)
            return version.ToString()!;
        return string.Empty;
    }
}
