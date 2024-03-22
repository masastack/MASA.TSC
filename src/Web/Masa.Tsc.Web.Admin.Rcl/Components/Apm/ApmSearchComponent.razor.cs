// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System;

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

    private List<(string, string)> _values = new List<(string, string)> { new("All", "All") };
    private static List<(ApmComparisonTypes value, string text)> listComparisons = new()
    {
        new (ApmComparisonTypes.None, "None"),
        new (ApmComparisonTypes.Day, "Day before"),
        new (ApmComparisonTypes.Week, "Week before"),
    };
    private List<string> services = new();
    private List<string> enviroments = new();
    private bool isServiceLoading = true, isEnvLoading = true;

    private bool isCallQuery = false;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (!isCallQuery && Search.Start > DateTime.MinValue)
        {
            await LoadEnviromentAsync();
            await LoadServiceAsync();
            await OnValueChanged();
            isCallQuery = true;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (string.IsNullOrEmpty(uri.Query))
        {
            if (string.IsNullOrEmpty(Search.Enviroment) && string.IsNullOrEmpty(Search.Service) && !string.IsNullOrEmpty(UserContext.Environment))
                Search.Enviroment = UserContext.Environment;
            if (Search.ComparisonType == ApmComparisonTypes.None)
                Search.ComparisonType = ApmComparisonTypes.Day;
        }
    }

    private async Task LoadServiceAsync()
    {
        if (!ShowService) return;
        isServiceLoading = true;
        var query = new SimpleAggregateRequestDto
        {
            Start = Search.Start,
            End = Search.End,
            Name = StorageConst.ServiceName,
            Type = AggregateTypes.GroupBy
        };
        if (!string.IsNullOrEmpty(Search.Enviroment) && Search.Enviroment != "All")
        {
            query.Conditions = new List<FieldConditionDto> {
                new FieldConditionDto{
                    Name=StorageConst.Environment,
                    Type= ConditionTypes.Equal,
                    Value=Search.Enviroment
                }
            };
        }
        var result = await ApiCaller.TraceService.GetAttrValuesAsync(query);
        services = result?.ToList() ?? new List<string>();
        if (!string.IsNullOrEmpty(Search.Service) && !services.Contains(Search.Service))
            Search.Service = default!;
        isServiceLoading = false;
    }

    private async Task LoadEnviromentAsync()
    {
        if (!ShowEnv) return;
        isEnvLoading = true;
        var query = new SimpleAggregateRequestDto
        {
            Start = Search.Start,
            End = Search.End,
            Name = StorageConst.Environment,
            Type = AggregateTypes.GroupBy
        };
        var result = await ApiCaller.TraceService.AggregateAsync<IEnumerable<string>>(query);
        enviroments = result?.ToList() ?? new List<string>();
        if (!string.IsNullOrEmpty(Search.Enviroment) && !enviroments.Contains(Search.Enviroment))
            Search.Enviroment = default!;
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
        await LoadEnviromentAsync();
        await LoadServiceAsync();
        await OnValueChanged();
    }

    private async Task OnEnviromentChanged(string env)
    {
        Search.Enviroment = env;
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
        Search.Timestamp = DateTime.Now.ToUnixTimestamp();
        await ValueChanged.InvokeAsync(Search);
        StateHasChanged();
    }
}