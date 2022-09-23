// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services.Instruments;

public class InstrumentService : ServiceBase
{
    public InstrumentService() : base("/api/Instrument")
    {
        App.MapPost($"{BaseUri}", AddAsync);
        App.MapPut($"{BaseUri}", UpdateAsync);
        App.MapDelete($"{BaseUri}", DeleteAsync);
        App.MapGet($"{BaseUri}/{{userId}}/{{id}}", GetAsync);
        App.MapGet($"{BaseUri}/list/{{userId}}/{{page}}/{{size}}/{{keyword}}", ListAsync);
    }

    private async Task AddAsync([FromServices] IEventBus eventBus, [FromBody] AddInstrumentDto model)
    {
        await eventBus.PublishAsync(new AddInstrumentCommand(model));
    }

    private async Task UpdateAsync([FromServices] IEventBus eventBus, [FromBody] UpdateInstrumentDto model)
    {
        await eventBus.PublishAsync(new UpdateInstrumentCommand(model));
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus, [FromBody] CommonRemoveDto<Guid> model)
    {
        await eventBus.PublishAsync(new RemoveInstrumentCommand(model.UserId, model.Ids.ToArray()));
    }

    public async Task<PaginationDto<InstrumentListDto>> ListAsync([FromServices] IEventBus eventBus, Guid userId, int page, int size, string keyword)
    {
        var query = new InstrumentQuery(userId, keyword, page, size);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<InstrumentDetailDto> GetAsync([FromServices] IEventBus eventBus, Guid userId, Guid id)
    {
        var query = new InstrumentDetailQuery(userId, id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
