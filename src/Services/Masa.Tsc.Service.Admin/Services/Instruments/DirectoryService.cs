// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class DirectoryService : ServiceBase
{
    public DirectoryService() : base("/api/Instrument/directory")
    {
        App.MapGet($"{BaseUri}/tree/{{isContainsInstrument}}", GetTreeAsync);
    }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, bool isContainsInstrument)
    {
        var query = new DirectoryTreeQuery(userContext.GetUserId<Guid>(), isContainsInstrument);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<DirectoryDto> GetAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, Guid id)
    {
        var query = new DirectoryQuery(id, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] AddDirectoryDto param)
    {
        var command = new AddDirectoryCommand(param.Name, param.Sort, param.ParentId, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] UpdateDirectoryDto param)
    {
        var command = new UpdateDirectoryCommand(param.Id, param.Name, param.Sort, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromQuery] Guid id)
    {
        var command = new RemoveDirectoryCommand(id, userContext.GetUserId<Guid>());
        await eventBus.PublishAsync(command);
    }
}
