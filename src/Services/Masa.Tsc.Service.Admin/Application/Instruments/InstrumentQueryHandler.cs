// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class InstrumentQueryHandler
{
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
    public async Task Query(InstrumentListQuery query)
    {
        var result = await _instrumentRepository.GetPaginatedListAsync(Predicate(query), new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.Size
        });
        query.Result = new PaginatedListBase<InstrumentListDto>();
        if (result != null)
        {
            query.Result.Total = result.Total;
            if (result.Result != null)
                query.Result.Result = result.Result.Select(item => new InstrumentListDto
                {

                }).ToList();
        }
    }

    private static Expression<Func<Instrument, bool>> Predicate(InstrumentListQuery query)
    {
        Expression<Func<Instrument, bool>> condition;

        if (string.IsNullOrEmpty(query.Keyword))
            condition = item => item.IsGlobal || item.Creator == query.UserId;
        else
            condition = item => (item.IsGlobal || item.Creator == query.UserId) && item.Name.Contains(query.Keyword);
        return condition;
    }

    [EventHandler]
    public async Task GetDetailAsync(InstrumentDetailQuery query)
    {
        var dto = await _instrumentRepository.GetDetailAsync(query.Id, query.UserId);

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

    [EventHandler]
    public async Task GetAsync(InstrumentQuery query)
    {
        var instument = await _instrumentRepository.GetAsync(query.Id, query.UserId);
        if (instument == null)
            throw new UserFriendlyException($"instument {query.Id} is not exists");
        query.Result = new()
        {
            Id = instument.Id,
            Folder = instument.DirectoryId,
            Name = instument.Name,
            IsRoot = instument.IsRoot,
            Layer = Enum.Parse<LayerTypes>(instument.Layer),
            Model = Enum.Parse<ModelTypes>(instument.Model),
            Type = Enum.Parse<LabelTypes>(instument.Lable),
            Order = instument.Sort
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
