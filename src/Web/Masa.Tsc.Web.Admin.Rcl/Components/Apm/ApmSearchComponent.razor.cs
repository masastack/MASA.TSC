// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

public partial class ApmSearchComponent
{
    [Parameter]
    public bool ShowComparison { get; set; }

    [Parameter]
    public bool ShowEnv { get; set; }

    [Parameter]
    public bool ShowText { get; set; }

    [Parameter]
    public bool ShowTime { get; set; }

    [Parameter]
    public bool ShowService { get; set; }

    [Parameter]
    public bool ShowButton { get; set; }

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
            isCallQuery = true;
        }
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
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (string.IsNullOrEmpty(uri.Query))
        {
            if (string.IsNullOrEmpty(Search.Environment) && string.IsNullOrEmpty(Search.Service) && !string.IsNullOrEmpty(UserContext.Environment))
                Search.Environment = UserContext.Environment;
            if (Search.ComparisonType == ApmComparisonTypes.None)
                Search.ComparisonType = ApmComparisonTypes.Day;
        }
    }

    private async Task LoadServiceAsync()
    {
        if (!ShowService) return;
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
        if (!ShowEnv) return;
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

    private async Task OnTextChanged(string text)
    {
        Search.Text = text;
        await OnValueChanged();
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
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnSelectChanged(ApmComparisonTypes comparisonType)
    {
        Search.ComparisonType = comparisonType;
        await OnValueChanged();
        StateHasChanged();
    }

    private async Task OnValueChanged()
    {
        isCallQuery = true;
        //Search.Timestamp = DateTime.Now.ToUnixTimestamp();
        await ValueChanged.InvokeAsync(Search);
        StateHasChanged();
    }
}