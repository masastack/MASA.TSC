//// Copyright (c) MASA Stack All rights reserved.
//// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

//namespace Masa.Tsc.Service.Admin.Services.Instruments;

//public class PanelService : ServiceBase
//{
//    public PanelService() : base("/api/Instrument/panel/{{instrumentId}}") { }

//    public async Task AddAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid instrumentId, [FromBody] PanelDto model)
//    {
//        await eventBus.PublishAsync(new AddPanelCommand(model, instrumentId, userContext.GetUserId<Guid>()));
//    }

//    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid instrumentId, [FromBody] PanelDto model)
//    {
//        await eventBus.PublishAsync(new UpdatePanelCommand(model, instrumentId, userContext.GetUserId<Guid>()));
//    }

//    public async Task DeleteAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid instrumentId, [FromQuery] Guid id)
//    {
//        await eventBus.PublishAsync(new RemovePanelCommand(userContext.GetUserId<Guid>(), instrumentId, id));
//    }

//    public async Task<List<PanelDto>> GetListAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid instrumentId)
//    {
//        var query = new PanelQuery(instrumentId, userContext.GetUserId<Guid>());
//        await eventBus.PublishAsync(query);
//        return query.Result;
//    }
//}