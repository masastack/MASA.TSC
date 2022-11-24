// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services.Instruments;

public class PanelService : ServiceBase
{
    public PanelService() : base("/api/Instrument/panel")
    {
        App.MapDelete($"{BaseUri}/{{instrumentId}}/{{id}}/{{userId}}", DeleteAsync);
        App.MapGet($"{BaseUri}/{{userId}}/{{instrumentId}}/{{id}}", ListAsync);
        App.MapPut($"{BaseUri}/{{userId}}/{{id}}/{{parentId}}", UpdateParentIdAsync);
        App.MapPut($"{BaseUri}/{{userId}}/{{id}}/{{width}}/{{height}}", UpdateWidthHeightAsync);
        App.MapPut($"{BaseUri}/{{userId}}", UpdateSortAsync);
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromBody] PanelDto model)
    {
        await eventBus.PublishAsync(new AddPanelCommand(model));
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromBody] PanelDto model)
    {
        await eventBus.PublishAsync(new UpdatePanelCommand(model));
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus, Guid instrumentId, Guid id, Guid userId)
    {
        await eventBus.PublishAsync(new RemovePanelCommand(userId, instrumentId, id));
    }

    public async Task<List<PanelDto>> ListAsync([FromServices] IEventBus eventBus, Guid userId, Guid instrumentId, Guid id)
    {
        var query = new PanelQuery(instrumentId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task UpdateParentIdAsync([FromServices] IEventBus eventBus, Guid userId, Guid id, Guid parentId)
    {
        await eventBus.PublishAsync(new UpdatePanelParentCommond(id, parentId));
    }

    public async Task UpdateWidthHeightAsync([FromServices] IEventBus eventBus, Guid userId, Guid id, string width, string height)
    {
        await eventBus.PublishAsync(new UpdatePanelWidthHeightCommond(id, height, width));
    }

    public async Task UpdateSortAsync([FromServices] IEventBus eventBus, Guid userId, [FromBody] UpdatePanelsSortDto model)
    {
        await eventBus.PublishAsync(new UpdatePanelsSortCommand(model.InstrumentId, model.ParentId, model.PanelIds));
    }
}
