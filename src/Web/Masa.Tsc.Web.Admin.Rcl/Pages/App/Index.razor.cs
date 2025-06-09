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
    private UserModel? user = default;
    private TraceResponseDto? firstTrace;
    private PhoneModelDto? phoneModel;
    private Dictionary<string, object> claims = [];
    private DateTime end, start;
    private QuickRangeKey quickRangeKey = QuickRangeKey.Last5Minutes;
    private StringNumber index;
    private string serviceName;
    private List<ValueTuple<string, string>> services;
    private static Dictionary<string, List<EnvironmentAppDto>> _teamServices = new();
    private bool claimShow = false;
    private List<TraceResponseDto> traceLines;
    private IJSObjectReference? module = null;
    private List<MenuModel>? _menus = default;

    private string? spanId;
    private DateTime? logTime;
    private NameValueCollection? values;
    private bool dataLoading = false;
    private bool isNeedSetPosition = false;
    private const string andriodWebviewVersionReg = @"Chrome/(\d+\.?)+", iosWebviewVersionReg = @"AppleWebKit/(\d+\.?)+";
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
        if (Regex.IsMatch(userAgent, andriodWebviewVersionReg))
        {
            return Regex.Match(userAgent, andriodWebviewVersionReg).Value.Split('/')[1];
        }
        if (Regex.IsMatch(userAgent, iosWebviewVersionReg))
        {
            return Regex.Match(userAgent, iosWebviewVersionReg).Value.Split('/')[1];
        }
        return default!;
    }

    //auth获取
    private string RolesNames()
    {
        if (user == null || user.Roles == null || user.Roles.Count == 0)
            return default;
        return string.Join(", ", user.Roles.Select(role => role.Name));
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
        await LoadUserClaimsAsync();
        await LoadTrace();
    }

    private async Task LoadUserClaimsAsync()
    {
        claims.Clear();
        if (_userId == Guid.Empty)
            return;
        var userClaims = await ApiCaller.UserService.GetUserClaimAsync(_userId);
        var claimTypes = await ApiCaller.UserService.GetClaimsAsync();
        if (userClaims != null && userClaims.Count > 0 && claimTypes != null && claimTypes.Count > 0)
        {
            claims.TryAddRange(
            userClaims.Select(item =>
            {
                var declare = claimTypes.Find(c => c.Name == item.Key);
                if (declare != null && !string.IsNullOrEmpty(declare.Description) && declare.Description != item.Key)
                    return KeyValuePair.Create($"{item.Key}({declare.Description})", (object)item.Value);
                return KeyValuePair.Create(item.Key, (object)item.Value);
            }).GroupBy(item => item.Key).Select(item =>
            {
                if (item.Count() == 1)
                    return item.First();
                return KeyValuePair.Create(item.Key, (object)string.Join(',', item.Select(i => (string)i.Value)));
            }));
        }
        StateHasChanged();
    }

    private async Task ServiceChange(string service)
    {
        serviceName = service;
        ClearData();
        await LoadTrace();
        await LoadAppMenusAsync();
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
        if (traces.Result != null && traces.Result.Count > 0)
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
                        break;
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
        Search.TraceId = default!;
        _legend = string.Empty;
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
    //private void ShowHideHttpTrace()
    //{
    //    showNormalClient = !showNormalClient;
    //    StateHasChanged();
    //}

    private async Task Top()
    {
        if (data.Count == 0) return;
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
        if (data.Count == 0) return;
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
        await LoadAppMenusAsync();
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

    private string? _legend = default;
    private string? lastRoute = default!;
    private void GetUrlLegend(OperationLineTraceModel data)
    {
        if (_menus == null || _menus.Count == 0 || data == null)
            return;

        if (data.Data.Attributes.ContainsKey("client.path") && data.Data.Attributes.ContainsKey("client.path.route"))
        {
            var route = data.Data.Attributes["client.path.route"].ToString();
            if (lastRoute == route)
                return;
            lastRoute = route;
            var item = UrlToName(data.Data.Attributes["client.path"].ToString()!, route);
            _legend = item.Item2;
        }
        else
        {
            _legend = default;
        }
        StateHasChanged();
    }

    private static ValueTuple<string, string> UrlToName(string url, string? route)
    {
        if (_permissions == null || _permissions.Count == 0 || string.IsNullOrEmpty(url))
            return ValueTuple.Create<string, string>(default!, default!);

        url = url.ToLower();
        var length = url.Split('/').Length;

        var matchKeys = _permissions.Keys.Where(key => url.StartsWith(key) && key.Split('/').Length - length == 0).ToList();
        if (matchKeys.Count == 1)
            return _permissions[matchKeys[0]];
        else if (matchKeys.Count > 1)
        {
            var equalKey = matchKeys.Find(key => key == url);
            if (!string.IsNullOrEmpty(equalKey))
                return _permissions[equalKey];
            else
                return _permissions[matchKeys.OrderByDescending(key => key.Length).First()];
        }
        //使用路由匹配
        else if (!string.IsNullOrEmpty(route))
        {
            route = route.ToLower();
            var matchKey = _permissions.Keys.FirstOrDefault(key => route == key);
            if (!string.IsNullOrEmpty(matchKey))
                return _permissions[matchKey];
        }
        return ValueTuple.Create<string, string>(default!, default!);
    }

    private static Dictionary<string, ValueTuple<string, string>>? _permissions = new();

    private static void ConverToDic(List<MenuModel>? menus)
    {
        if (menus == null || menus.Count == 0)
            return;
        foreach (var item in menus)
        {
            if (!string.IsNullOrEmpty(item.Url) && !_permissions!.ContainsKey(item.Url.ToLower()))
            {
                _permissions.Add(item.Url.ToLower(), ValueTuple.Create(item.Name, item.Legend));
            }
            ConverToDic(item.Children);
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

    private string? lastServiceName = default;

    private async Task LoadAppMenusAsync()
    {
        if (!string.IsNullOrEmpty(serviceName) && index == 5 && lastServiceName != serviceName)
        {
            if (serviceName == "lonsid-fusion")
            {
                if (lastServiceName != "lonsid-fusion-app")
                    lastServiceName = "lonsid-fusion-app";
            }
            else if (lastServiceName != serviceName)
            {
                lastServiceName = serviceName;
            }
            else
            {
                return;
            }

            lastRoute = default;
            _legend = default;
            _menus = await AuthClient.PermissionService.GetMenusAsync(lastServiceName);
            _permissions?.Clear();
            ConverToDic(_menus);
            GetUrlLegend(currentTrace!);
        }
    }
}
