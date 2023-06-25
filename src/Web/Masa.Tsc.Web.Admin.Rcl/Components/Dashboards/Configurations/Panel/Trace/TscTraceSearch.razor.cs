// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch
{
    [Parameter, EditorRequired]
    public EventCallback<(string?, string?, string?, string?)> OnQueryUpdate { get; set; }

    [Parameter, EditorRequired]
    public Func<Task<IEnumerable<string>>> QueryServices { get; set; }

    [Parameter, EditorRequired]
    public Func<string, Task<IEnumerable<string>>> QueryInstances { get; set; }

    [Parameter, EditorRequired]
    public Func<string, string?, Task<IEnumerable<string>>> QueryEndpoints { get; set; }

    [Parameter]
    public EventCallback<(DateTime, DateTime)> OnDateTimeRangeUpdate { get; set; }

    [Parameter]
    public bool PageMode { get; set; }

    [Parameter]
    public DateTime? StartDateTime { get; set; }

    [Parameter]
    public DateTime? EndDateTime { get; set; }

    [Parameter]
    public string Service { get; set; }

    [Parameter]
    public string Keyword { get { return Search; } set { Search = Keyword; } }

    private List<string> _services = new();
    private List<string> _instances = new();
    private List<string> _endpoints = new();

    private string _service = string.Empty;
    private string? _instance;
    private string? _endpoint;
    private string? Search;

    private bool _serviceSearching;
    private bool _instanceSearching;
    private bool _endpointSearching;
    private int width = 268;
    private string _style = "flex:none;width:{0}px !important;";

    protected override async Task OnInitializedAsync()
    {
        Search = Keyword;
        await SearchServices();
        if (PageMode)
            width = 208;
        _style = string.Format(_style, width);
        await base.OnInitializedAsync();
    }

    private bool IsTraceId()
    {
        if (!string.IsNullOrEmpty(Keyword) && Keyword.Length - 32 == 0)
        {
            return Regex.IsMatch("[a-zA-Z0-9]{32}", Keyword);
        }
        return false;
    }

    public async Task SearchServices()
    {
        _serviceSearching = true;
        _services = (await QueryServices.Invoke())?.ToList();
        if (!string.IsNullOrEmpty(Service) && _services != null && _services.Contains(Service))
            _service = Service;
        else
            _service = default;
        _instance = default;
        _endpoint = default;
        _serviceSearching = false;
    }

    private async Task SearchInstances()
    {
        _instanceSearching = true;
        _instances = (await QueryInstances(_service))?.ToList();
        _instanceSearching = false;
    }

    private async Task SearchEndpoints()
    {
        _endpointSearching = true;
        _endpoints = (await QueryEndpoints(_service, _instance))?.ToList();
        _endpointSearching = false;
    }

    private void OnEnter()
    {
        Query();
    }

    private void Query(bool isService = false, bool isInstance = false)
    {
        NextTick(async () =>
        {
            if (isService)
            {
                _instance = default;
                _endpoint = default;
                await SearchInstances();
                await SearchEndpoints();
            }
            else if (isInstance)
            {
                await SearchEndpoints();
            }

            await OnQueryUpdate.InvokeAsync((_service, _instance, _endpoint, Search));
            StateHasChanged();
        });
    }

    private async Task OnDateTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        await InvokeDateTimeUpdate(range);
    }

    private async Task OnDateTimeAutoUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        await InvokeDateTimeUpdate(range);
    }

    private Task InvokeDateTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) range)
    {
        if (range is { start: not null, end: not null })
        {
            var localStart = range.start.Value.UtcDateTime;
            var localEnd = range.end.Value.UtcDateTime;
            return OnDateTimeRangeUpdate.InvokeAsync((localStart, localEnd));
        }
        return Task.CompletedTask;
    }
}
