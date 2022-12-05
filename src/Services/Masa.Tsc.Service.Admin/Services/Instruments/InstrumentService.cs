// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services.Instruments;

public class InstrumentService : ServiceBase
{
    public InstrumentService() : base("/api/Instrument")
    {
        App.MapGet($"{BaseUri}/list/{{page}}/{{size}}/{{keyword}}", ListAsync);
        App.MapPost($"{BaseUri}/set-root/{{id}}", SetRootAsync);
        App.MapPost($"{BaseUri}/set-panels-show-setting/", UpdatePanelsShowAsync);
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] AddInstrumentDto model)
    {
        await eventBus.PublishAsync(new AddInstrumentCommand(model, userContext.GetUserId<Guid>()));
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] UpdateInstrumentDto model)
    {
        await eventBus.PublishAsync(new UpdateInstrumentCommand(model, userContext.GetUserId<Guid>()));
    }

    public async Task DeleteAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] CommonRemoveDto<Guid> model)
    {
        await eventBus.PublishAsync(new RemoveInstrumentCommand(userContext.GetUserId<Guid>(), model.Ids.ToArray()));
    }

    public async Task<PaginatedListBase<InstrumentListDto>> ListAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, int page, int size, string keyword)
    {
        var query = new InstrumentQuery(userContext.GetUserId<Guid>(), keyword, page, size);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<InstrumentDetailDto> GetAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromQuery] Guid id)
    {
        var query = new InstrumentDetailQuery(userContext.GetUserId<Guid>(), id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task SetRootAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromQuery] Guid id)
    {
        var command = new SetRootCommand(userContext.GetUserId<Guid>(), id);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdatePanelsShowAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] UpdateInstrumentPanelsShowDto model)
    {
        var command = new UpdatePanelsShowCommand(model, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(command);
    }
}