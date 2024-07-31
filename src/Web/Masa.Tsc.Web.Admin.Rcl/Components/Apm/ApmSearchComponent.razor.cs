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

    private static List<(ApmComparisonTypes value, string text)> listComparisons = new()
    {
        new (ApmComparisonTypes.None, "None"),
        new (ApmComparisonTypes.Day, "Day before"),
        new (ApmComparisonTypes.Week, "Week before"),
    };
    private List<string> services = new();
    private List<string> environments = new();
    private Dictionary<string, List<string>> enviromentServices = new();
    private bool isServiceLoading = true, isEnvLoading = true;

    private bool isCallQuery = false;
    private QuickRangeKey quickRangeKey = QuickRangeKey.Last15Minutes;
    private List<string> textFileds;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Search.Start > DateTime.MinValue && Search.End > Search.Start)
        {
            SetQuickRangeKey(Search.End - Search.Start);
        }

        if (!isCallQuery && Search.Start > DateTime.MinValue)
        {
            await LoadEnvironmentAsync();
            await LoadServiceAsync();
            await OnValueChanged();
            await LoadEndpointAsync();
            await LoadErrorAsync();
            isCallQuery = true;
        }
        await LoadAsync();
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
                textFileds.Add(StorageConst.Current.Log.Body);
        }
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
    }

    private async Task LoadServiceAsync()
    {
        isServiceLoading = true;
        if (!string.IsNullOrEmpty(Search.Environment))
        {
            if (!enviromentServices.TryGetValue(Search.Environment!, out services))
                services = new();
        }
        else
        {
            foreach (var item in enviromentServices)
            {
                services.AddRange(item.Value);
            }
            services = services.Distinct().ToList();
        }
        if (!string.IsNullOrEmpty(Search.Service) && !services.Contains(Search.Service))
            Search.Service = default!;
        isServiceLoading = false;
    }

    private async Task LoadEnvironmentAsync()
    {
        isEnvLoading = true;
        var query = new BaseApmRequestDto
        {
            Start = Search.Start,
            End = Search.End
        };
        var result = await ApiCaller.ApmService.GetEnviromentServiceAsync(query);
        enviromentServices = result;
        environments = result.Keys.ToList();
        if (!string.IsNullOrEmpty(Search.Environment) && !environments.Contains(Search.Environment))
            Search.Environment = default!;
        isEnvLoading = false;
    }

    private async Task LoadEndpointAsync()
    {
        if (!string.IsNullOrEmpty(Search.Service))
            endpoints = await ApiCaller.ApmService.GetEndpointsAsync(new BaseRequestDto { Service = Search.Service!, End = Search.End, Start = Search.Start });
        if (!string.IsNullOrEmpty(Search.Endpoint) && endpoints != null && !endpoints.Contains(Search.Endpoint))
            Search.Endpoint = default!;
    }

    private async Task LoadErrorAsync()
    {
        exceptions = await ApiCaller.ApmService.GetExceptionTypesAsync(new BaseRequestDto { Service = Search.Service!, End = Search.End, Start = Search.Start });
    }

    private async Task OnTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) times)
    {
        Search.Start = times.start!.Value.UtcDateTime;
        Search.End = times.end!.Value.UtcDateTime;
        await LoadEnvironmentAsync();
        await LoadServiceAsync();
        await OnValueChanged();
    }

    private async Task OnEnvironmentChanged(string env)
    {
        Search.Environment = env;
        await LoadServiceAsync();
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnServiceChanged(string? service)
    {
        Search.Service = service;
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
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }

    private async Task LoadAsync()
    {
        statuses = await ApiCaller.ApmService.GetStatusCodesAsync();
    }
}