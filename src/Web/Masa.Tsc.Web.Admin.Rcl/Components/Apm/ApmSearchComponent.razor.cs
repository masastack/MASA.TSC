// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

public partial class ApmSearchComponent
{
    [Parameter]
    public bool ShowComparison { get; set; }

    [Parameter]
    public bool IsEndpoint { get; set; }

    [Parameter]
    public bool IsService { get; set; }

    [Parameter]
    public bool IsLog { get; set; }

    [Parameter]
    public SearchData Value { get { return Search; } set { Search = value; } }

    [Parameter]
    public EventCallback<SearchData> ValueChanged { get; set; }

    [Inject]
    public IMultiEnvironmentUserContext UserContext { get; set; }

    [Inject]
    public IMemoryCache MemoryCache { get; set; }

    private static List<(ApmComparisonTypes value, string text)> listComparisons = new()
    {
        new (ApmComparisonTypes.None, "None"),
        new (ApmComparisonTypes.Day, "Day before"),
        new (ApmComparisonTypes.Week, "Week before"),
    };
    private List<ValueTuple<string, string>> services = new();
    private List<string> projects = new();
    private List<string> environments = new();
    private Dictionary<string, List<EnviromentAppDto>> enviromentServices = new();
    private bool isServiceLoading = true, isEnvLoading = true;
    private List<string> types = new() {
        AppTypes.UI.ToString(),
        AppTypes.Service.ToString(),
         AppTypes.Job.ToString()
    };

    private bool Isloaded = false;
    private QuickRangeKey quickRangeKey = QuickRangeKey.Last15Minutes;
    private List<string> textFileds = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        SetQueryList();
        if (IsEndpoint)
            await LoadAsync();
        if (Search.Start > DateTime.MinValue)
        {
            await InitAsync();
        }
        Isloaded = true;
        await OnValueChanged();
        StateHasChanged();
    }

    private void SetQuickRangeKey(TimeSpan timeSpan)
    {
        var minutes = (int)timeSpan.TotalMinutes;

        if (minutes - 1440 >= 0)
        {
            var days = minutes / 1440;
            switch (days)
            {
                case 1:
                    quickRangeKey = QuickRangeKey.Last24Hours;
                    return;
                case 2:
                    quickRangeKey = QuickRangeKey.Last2Days;
                    return;
                case 7:
                    quickRangeKey = QuickRangeKey.Last7Days;
                    return;
                case 30:
                    quickRangeKey = QuickRangeKey.Last30Days;
                    return;
            }
        }

        switch (minutes)
        {
            case 5:
                quickRangeKey = QuickRangeKey.Last5Minutes;
                return;
            case 15:
                quickRangeKey = QuickRangeKey.Last15Minutes;
                return;
            case 30:
                quickRangeKey = QuickRangeKey.Last30Minutes;
                return;
            case 60:
                quickRangeKey = QuickRangeKey.Last1Hour;
                return;
            case 180:
                quickRangeKey = QuickRangeKey.Last3Hours;
                return;
            case 360:
                quickRangeKey = QuickRangeKey.Last6Hours;
                return;
            case 720:
                quickRangeKey = QuickRangeKey.Last12Hours;
                return;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        GlobalConfig.OnCurrentTeamChanged += TeamChanged;
        SetQueryList();
    }

    public EnviromentAppDto? GetService(string service)
    {
        if (string.IsNullOrEmpty(service))
            return default;
        if (enviromentServices.Count == 0)
            return default;
        foreach (var item in enviromentServices)
        {
            if (string.IsNullOrEmpty(Search.Environment) || item.Key == Search.Environment)
            {
                var findApp = item.Value.Find(app => app.AppId == service);
                if (findApp != null)
                    return findApp;
            }
        }

        return default!;
    }

    private void SetQueryList()
    {
        if (textFileds.Count > 0 || StorageConst.Current == null)
            return;
        textFileds = new List<string> {
                StorageConst.Current.TraceId,
                StorageConst.Current.SpanId
        };
        if (IsEndpoint)
        {
            textFileds.AddRange(new string[] {
                StorageConst.Current.Trace.URLFull,
                StorageConst.Current.Trace.UserId,
                StorageConst.Current.Trace.HttpRequestBody
            });
        }
        else if (!IsService)
        {
            textFileds.Add(StorageConst.Current.ExceptionMessage);
            if (IsLog)
            {
                textFileds.Add(StorageConst.Current.Log.Body);
                textFileds.Add(StorageConst.Current.Log.TaskId);
            }
        }
        if (string.IsNullOrEmpty(Value.TextField) || !textFileds.Contains(Value.TextField))
            Value.TextField = textFileds[0];
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (string.IsNullOrEmpty(uri.Query))
        {
            if (string.IsNullOrEmpty(Search.Environment) && string.IsNullOrEmpty(Search.Service) && !string.IsNullOrEmpty(UserContext.Environment))
                Search.Environment = UserContext.Environment;
            if (Search.ComparisonType == ApmComparisonTypes.None)
                Search.ComparisonType = ApmComparisonTypes.Day;
        }

        if (!string.IsNullOrEmpty(Search.SpanId))
        {
            Search.TextField = StorageConst.Current.SpanId;
            Search.TextValue = Search.SpanId;
        }
        else if (!string.IsNullOrEmpty(Search.TraceId))
        {
            Search.TextField = StorageConst.Current.TraceId;
            Search.TextValue = Search.TraceId;
        }
        if (GlobalConfig.CurrentTeamId == Guid.Empty)
            GlobalConfig.CurrentTeamId = MasaUser.CurrentTeamId;
        if (Search.Start > DateTime.MinValue && Search.End > Search.Start)
        {
            SetQuickRangeKey(Search.End - Search.Start);
        }
    }

    private void TeamChanged(Guid teamId)
    {
        GlobalConfig.CurrentTeamId = teamId;
        if (Isloaded && CheckUrl())
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
            return;
        }
        _ = InvokeAsync(async () =>
        {
            await LoadEnvironmentAsync();
            await LoadProjectAsync();
            await LoadServiceTypeAsync();
            await LoadServiceAsync();
            await OnValueChanged();
            await LoadEndpointAsync();
            StateHasChanged();
        });
    }

    private bool CheckUrl()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var values = HttpUtility.ParseQueryString(uri.Query);
        if (values.Count > 0)
        {
            return true;
        }
        else
            return false;
    }

    private async Task ServiceTypeChanged(string value)
    {
        Search.ServiceType = value;
        await LoadProjectAsync();
        await LoadServiceAsync();
        await OnValueChanged();
    }

    private async Task LoadServiceAsync()
    {
        isServiceLoading = true;
        List<EnviromentAppDto> projects = new();
        if (!string.IsNullOrEmpty(Search.Environment))
        {
            if (!enviromentServices.TryGetValue(Search.Environment!, out projects!))
                projects = new();
        }
        else
        {
            foreach (var item in enviromentServices)
            {
                projects.AddRange(item.Value);
            }
        }
        if (projects.Count > 0)
        {
            if (!string.IsNullOrEmpty(Search.Project))
                projects = projects.Where(app => app.ProjectId == Search.Project).ToList();
            if (!string.IsNullOrEmpty(Search.ServiceType))
                projects = projects.Where(app => app.AppType.ToString() == Search.ServiceType).ToList();
        }

        services = projects.Select(app => ValueTuple.Create(app.AppId, app.AppDescription)).Distinct().ToList();
        if (!string.IsNullOrEmpty(Search.Project) && !projects.Exists(p => p.ProjectId == Search.Project))
            Search.Service = default!;
        if (!string.IsNullOrEmpty(Search.Service) && !services.Exists(item => item.Item1 == Search.Service))
            Search.Service = default!;
        isServiceLoading = false;
        await Task.CompletedTask;
    }

    private async Task LoadEnvironmentAsync()
    {
        isEnvLoading = true;
        var result = await ApiCaller.ApmService.GetEnviromentServiceAsync(GlobalConfig.CurrentTeamId, Search.Start, Search.End, Search.Environment!) ?? new();
        enviromentServices = result;
        environments = result.Keys.ToList();
        if (!string.IsNullOrEmpty(Search.Service) && string.IsNullOrEmpty(Search.Environment))
        {
            var findEnv = enviromentServices.FirstOrDefault(item => item.Value.Exists(app => app.AppId == Search.Service));
            if (string.IsNullOrEmpty(findEnv.Key))
                Search.Service = default!;
            else
                Search.Environment = findEnv.Key;
        }

        if (!string.IsNullOrEmpty(Search.Environment) && !environments.Contains(Search.Environment))
            Search.Environment = default!;
        isEnvLoading = false;
    }

    private async Task LoadProjectAsync()
    {
        List<EnviromentAppDto> projects = new();
        if (!string.IsNullOrEmpty(Search.Environment))
        {
            if (!enviromentServices.TryGetValue(Search.Environment!, out projects!))
                projects = new();
        }
        else
        {
            foreach (var item in enviromentServices)
            {
                projects.AddRange(item.Value);
            }
        }
        if (projects.Count > 0)
        {
            if (!string.IsNullOrEmpty(Search.ServiceType))
                projects = projects.Where(app => app.AppType.ToString() == Search.ServiceType).ToList();
        }

        this.projects = projects.Select(p => p.ProjectId).Distinct().ToList();
        if (string.IsNullOrEmpty(Search.Project) || !string.IsNullOrEmpty(Search.Project) && !this.projects.Contains(Search.Project))
        {
            EnviromentAppDto? project = null;
            if (!string.IsNullOrEmpty(Search.Service))
                project = projects.Find(app => app.AppId == Search.Service);
            if (project != null)
                Search.Project = project.ProjectId;
            else
                Search.Project = default!;
        }

        await Task.CompletedTask;
    }

    private async Task LoadServiceTypeAsync()
    {
        List<EnviromentAppDto> projects = new();
        if (!string.IsNullOrEmpty(Search.Environment))
        {
            if (!enviromentServices.TryGetValue(Search.Environment!, out projects!))
                projects = new();
        }
        else
        {
            foreach (var item in enviromentServices)
            {
                projects.AddRange(item.Value);
            }
        }

        if (projects.Count > 0)
        {
            if (!string.IsNullOrEmpty(Search.Project))
                projects = projects.Where(app => app.ProjectId == Search.Project).ToList();
        }

        types = projects.Select(app => app.AppType.ToString()).Distinct().ToList();
        if (string.IsNullOrEmpty(Search.ServiceType) || !string.IsNullOrEmpty(Search.ServiceType) && !types.Contains(Search.ServiceType))
        {
            EnviromentAppDto? projectApp = null;
            if (!string.IsNullOrEmpty(Search.Service))
                projectApp = projects.Find(app => app.AppId == Search.Service);
            else if (!string.IsNullOrEmpty(Search.Project))
                projectApp = projects.Find(app => app.ProjectId == Search.Project);
            if (projectApp != null)
                Search.ServiceType = projectApp!.AppType.ToString();
            else
                Search.ServiceType = default!;
        }
        await Task.CompletedTask;
    }

    private async Task LoadEndpointAsync()
    {
        if (!string.IsNullOrEmpty(Search.Service))
            endpoints = await ApiCaller.ApmService.GetEndpointsAsync(new BaseRequestDto { Service = Search.Service!, End = Search.End, Start = Search.Start }) ?? new();
        if (!string.IsNullOrEmpty(Search.Endpoint) && endpoints != null && !endpoints.Contains(Search.Endpoint))
            Search.Endpoint = default!;
    }

    private async Task LoadErrorAsync()
    {
        exceptions = await ApiCaller.ApmService.GetExceptionTypesAsync(new BaseRequestDto { Service = Search.Service!, End = Search.End, Start = Search.Start });
        if (exceptions == null || !exceptions.Contains(Search.ExceptionType))
            Search.ExceptionType = default!;
    }

    private async Task OnTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) times)
    {
        if (Isloaded || Search.Start == DateTime.MinValue)
        {
            Search.Start = times.start!.Value.UtcDateTime;
            Search.End = times.end!.Value.UtcDateTime;
        }
        if (Isloaded)
        {
            await InitAsync();
            await OnValueChanged();
            StateHasChanged();
        }
    }

    private async Task InitAsync()
    {
        await LoadEnvironmentAsync();
        await LoadProjectAsync();
        await LoadServiceTypeAsync();
        await LoadServiceAsync();
        await LoadEndpointAsync();
        await LoadErrorAsync();
    }

    private async Task OnEnvironmentChanged(string env)
    {
        Search.Environment = env;
        await LoadProjectAsync();
        await LoadServiceTypeAsync();
        await LoadServiceAsync();
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnProjectChanged(string? project)
    {
        Search.Project = project;
        await LoadServiceTypeAsync();
        await LoadServiceAsync();
        await LoadEndpointAsync();
        await LoadErrorAsync();
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnServiceChanged(string? service)
    {
        Search.Service = service;
        await LoadProjectAsync();
        await LoadServiceTypeAsync();
        await LoadEndpointAsync();
        await LoadErrorAsync();
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnSelectChanged(ApmComparisonTypes comparisonType)
    {
        Search.ComparisonType = comparisonType;
        await OnValueChanged();
        StateHasChanged();
    }

    private List<string> endpoints = new();
    private List<string> statuses = new();
    private List<string> exceptions = new();
    private async Task OnEndpointChange(string value)
    {
        Value.Endpoint = value;
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnStatusCodeChange(string value)
    {
        Value.Status = value;
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnExceptionChange(string value)
    {
        Value.ExceptionType = value;
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnMessageEnter()
    {
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnValueChanged()
    {
        if (!Isloaded || string.IsNullOrEmpty(Value.Environment))
            return;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
        StateHasChanged();
    }

    private async Task LoadAsync()
    {
        var key = "mc_statuscodes";
        statuses = MemoryCache.Get<List<string>>(key) ?? new();
        if (statuses.Any())
            return;
        statuses = (await ApiCaller.ApmService.GetStatusCodesAsync()) ?? new();
        if (statuses.Any())
        {
            statuses = statuses.Where(s => s.Trim().Length > 0).ToList();
            MemoryCache.Set(key, statuses, TimeSpan.FromMinutes(10));
        }
        if (!string.IsNullOrEmpty(Search.Status) && !statuses.Contains(Search.Status))
            Search.Status = default!;
    }

    protected override ValueTask DisposeAsyncCore()
    {
        GlobalConfig.OnCurrentTeamChanged -= TeamChanged;
        return base.DisposeAsyncCore();
    }
}