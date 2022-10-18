// Copyright (c) MASA Stack All rights reserved.
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
    public async Task Detail(InstrumentDetailQuery query)
    {
        var dto = await _instrumentRepository.GetAsync(query.Id, query.UserId);

        query.Result = new InstrumentDetailDto
        {
            Id = dto.Id,
            Name = dto.Name,
            DirecotryId = dto.DirectoryId,
            IsGlobal = dto.IsGlobal,
            IsRoot = dto.IsRoot,
            Layer = dto.Layer,
            Model = dto.Model,
            Sort = dto.Sort,
            Panels = ConvertPanels(dto.Panels, Guid.Empty)
        };
    }

    private List<PanelDto> ConvertPanels(List<Panel> panels, Guid parentId)
    {
        var children = panels.Where(m => m.ParentId == parentId).ToList();
        panels.RemoveAll(m => m.ParentId == parentId);
        if (!children.Any())
            return default!;

        var result = new List<PanelDto>();
        foreach (var item in children)
        {
            var panel = item.Type.ToModel(item);
            if (item.Type == PanelTypes.Tabs)
            {
                ((TabsPanelDto)(panel)).Tabs = ConvertPanels(panels, panel.Id)?.Select(item => (TabItemPanelDto)item)?.ToList()!;
            }
            else if (item.Type == PanelTypes.TabItem)
            {
                ((TabItemPanelDto)(panel)).Tabs = ConvertPanels(panels, panel.Id);
            }
            result.Add(panel);
        }
        return result;
    }

    [EventHandler]
    public async Task QueryPanels(PanelQuery query)
    {
        var result = await _panelRepository.GetListAsync(t => t.InstrumentId == query.InstrumentId, nameof(Panel.Sort), false);
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
