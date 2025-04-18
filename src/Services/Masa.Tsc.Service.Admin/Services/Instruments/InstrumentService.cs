﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services.Instruments;

internal class InstrumentService : ServiceBase
{
    public InstrumentService() : base("/api/Instrument")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        App.MapPost($"{BaseUri}/set-root/{{id}}/{{isRoot}}", SetRootAsync).RequireAuthorization();
        App.MapPost($"{BaseUri}/upsert/{{instrumentId}}", UpsertAsync).RequireAuthorization();
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] AddDashboardDto model)
    {
        await eventBus.PublishAsync(new AddInstrumentCommand(model, userContext.GetUserId<Guid>()));
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] UpdateDashboardDto model)
    {
        await eventBus.PublishAsync(new UpdateInstrumentCommand(model, userContext.GetUserId<Guid>()));
    }

    public async Task DeleteAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] CommonRemoveDto<Guid> model)
    {
        await eventBus.PublishAsync(new RemoveInstrumentCommand(userContext.GetUserId<Guid>(), model.Ids.ToArray()));
    }

    private async Task UpsertAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid instrumentId, [FromBody] UpsertPanelDto[] model)
    {
        await eventBus.PublishAsync(new UpInsertCommand(model, instrumentId, userContext.GetUserId<Guid>()));
    }

    public async Task<PaginatedListBase<InstrumentListDto>> GetListAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, int page, int size, string keyword)
    {
        var query = new InstrumentListQuery(userContext.GetUserId<Guid>(), keyword, page, size);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<InstrumentDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid id)
    {
        var query = new InstrumentDetailQuery(userContext.GetUserId<Guid>(), id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<UpdateDashboardDto> GetAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid id)
    {
        var query = new InstrumentQuery(id, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task SetRootAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid id, bool isRoot = true)
    {
        var command = new SetRootCommand(id, isRoot, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(command);
    }

    public async Task<LinkResultDto> GetLinkAsync([FromServices] IEventBus eventBus, string? layer, MetricValueTypes type)
    {
        var query = new LinkTypeQuery(layer ?? MetricConstants.DEFAULT_LAYER, type);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task UpdateTeamAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] UpsertPanelDto[] model)
    {
        await eventBus.PublishAsync(new UpInsertCommand(model, InstrumentConstants.TEAM_INSTRUMENT_ID, userContext.GetUserId<Guid>()));
    }

    public async Task<InstrumentDetailDto> GetTeamAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext)
    {
        var query = new InstrumentDetailQuery(userContext.GetUserId<Guid>(), InstrumentConstants.TEAM_INSTRUMENT_ID);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}