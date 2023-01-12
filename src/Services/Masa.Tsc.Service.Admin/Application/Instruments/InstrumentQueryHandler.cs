// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class InstrumentQueryHandler
{
    private readonly IInstrumentRepository _instrumentRepository;

    public InstrumentQueryHandler(IInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
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
        try
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
                Panels = ConvertPanels(dto.Panels)
            };
        }
        catch (Exception ex)
        {


        }
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
            Type =instument.Lable,
            Order = instument.Sort
        };
    }

    private List<UpsertPanelDto> ConvertPanels(List<Panel> panels)
    {
        if (panels == null || !panels.Any())
            return default!;
        var result = new List<UpsertPanelDto>();
        foreach (var panel in panels)
        {
            var dto = new UpsertPanelDto
            {
                Description = panel.Description,
                ExtensionData = panel.ExtensionData,
                Height = int.Parse(panel.Height),
                Width = int.Parse(panel.Width),
                X = int.Parse(panel.Left),
                Y = int.Parse(panel.Top),
                Title = panel.Title,
                Id = panel.Id,
                PanelType = panel.Type,
                Metrics = panel.Metrics?.Select(item => new PanelMetricDto
                {
                    Caculate = item.Caculate,
                    Color = item.Color,
                    Icon = item.Icon,
                    DisplayName= item.DisplayName,
                    Id = item.Id,
                    Name = item.Name,
                    Sort = item.Sort,
                    Unit = item.Unit,
                    Range = item.Name
                })?.ToList()!
            };
            if (panel.Panels != null && panel.Panels.Any())
                dto.ChildPanels = ConvertPanels(panel.Panels);
            result.Add(dto);
        }
        return result;
    }

    [EventHandler]
    public async Task GetPanelLinkAsync(LinkTypeQuery query)
    {
        if (query.Type == MetricValueTypes.Service || query.Type == MetricValueTypes.Instance || query.Type == MetricValueTypes.Endpoint)
        {
            var type = ModelTypes.Service;
            if (query.Type == MetricValueTypes.Service)
                type = ModelTypes.Service;
            else if (query.Type == MetricValueTypes.Instance)
                type = ModelTypes.ServiceInstance;
            else
                type = ModelTypes.Endpoint;
            var instrument = await _instrumentRepository.ToQueryable().Where(item => item.Model == type.ToString("G")).OrderBy(item => item.CreationTime).FirstOrDefaultAsync();
            query.Result = new LinkResultDto { InstrumentId = instrument?.Id };
        }
        else
        {
            query.Result = new LinkResultDto { Href = "/dashbord/trace" };
        }
    }
}