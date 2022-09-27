// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services.Instruments;

public class PanelService : ServiceBase
{
    public PanelService() : base("/api/Instrument/panel")
    {
        App.MapDelete($"{BaseUri}/{{instrumentId}}/{{id}}/{{userId}}", DeleteAsync);
        App.MapGet($"{BaseUri}/{{userId}}/{{instrumentId}}/{{id}}", ListAsync);
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromBody] AddPanelDto model)
    {
        await eventBus.PublishAsync(new AddPanelCommand(model));
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromBody] UpdatePanelDto model)
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
}
