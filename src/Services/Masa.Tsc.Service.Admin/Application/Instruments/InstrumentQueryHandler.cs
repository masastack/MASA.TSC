﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class InstrumentQueryHandler
{
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IPanelRepository _panelRepository;
    private readonly IMetricReposity _metricReposity;

    public InstrumentQueryHandler(IInstrumentRepository instrumentRepository,
        IPanelRepository panelRepository,
        IMetricReposity metricReposity)
    {
        _instrumentRepository = instrumentRepository;
        _panelRepository = panelRepository;
        _metricReposity = metricReposity;
    }

    [EventHandler]
    public async Task Query(InstrumentQuery query)
    {
        var skip = (query.Page - 1) * query.Size;
        var result = await _instrumentRepository.GetPaginatedListAsync(Predicate(query), new PaginatedOptions
        {

        });
        query.Result = new PaginationDto<InstrumentListDto>(result?.Total ?? 0,
            result?.Result?.Select(item => new InstrumentListDto
            {

            })?.ToList() ?? new());
    }

    private Expression<Func<Instrument, bool>> Predicate(InstrumentQuery query)
    {
        Expression<Func<Instrument, bool>> condition = entity => true;

        if (string.IsNullOrEmpty(query.Keyword))
            condition = item => item.IsGlobal || item.Creator == query.UserId;
        else
            condition = item => (item.IsGlobal || item.Creator == query.UserId) && item.Name.Contains(query.Keyword);
        return condition;
    }

    [EventHandler]
    public async Task QueryPanels(PanelQuery query)
    {
        var result = await _panelRepository.GetListAsync(t => t.InstrumentId == query.InstrumentId, nameof(Panel.Index), false);
        if (result != null && result.Any())
            query.Result = result.Select(item => new PanelDto { }).ToList();
        else
            query.Result = new();
    }

    [EventHandler]
    public async Task QueryMetrics(PanelMetricQuery query)
    {
        var result = await _metricReposity.GetListAsync(t => t.PanelId == query.PanelId, nameof(PanelMetric.Sort), false);
        if (result != null && result.Any())
            query.Result = result.Select(item => new PanelMetricDto { }).ToList();
        else
            query.Result = new();
    }
}
