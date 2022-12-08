// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceSearch : IDisposable
{
    [Parameter]
    public EventCallback<(string?, string?, string?, string?)> OnQueryUpdate { get; set; }

    private List<string> _services = new();
    private List<string> _instances = new();
    private List<string> _endpoints = new();

    private string _service = string.Empty;
    private string? _instance;
    private string? _endpoint;
    private string? _keyword;

    private async Task SearchServices(string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Type = AggregateTypes.GroupBy,
            Service = key
        };

        _services = (await ApiCaller.TraceService.GetAttrValuesAsync(query)).ToList();
    }

    private async Task SearchInstances(string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Type = AggregateTypes.GroupBy,
            Service = _service,
            Instance = key
        };

        _instances = (await ApiCaller.TraceService.GetAttrValuesAsync(query)).ToList();
    }

    private async Task SearchEndpoints(string key)
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Type = AggregateTypes.GroupBy,
            Service = _service,
            Endpoint = key
        };

        _endpoints = (await ApiCaller.TraceService.GetAttrValuesAsync(query)).ToList();
    }

    private async Task KeywordChanged(string? val)
    {
        _keyword = val;
        await Search();
    }

    private async Task Search()
    {
        await OnQueryUpdate.InvokeAsync((_service, _instance, _endpoint, _keyword));
    }
}
