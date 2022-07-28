// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class DirectoryService : ServiceBase
{
    public DirectoryService(IServiceCollection services) : base(services, "/api/Instrument/directory")
    {
        App.MapPost($"{BaseUri}", AddAsync);
        App.MapPut($"{BaseUri}", UpdateAsync);
        App.MapDelete($"{BaseUri}/tree/{{id}}/{{userId}}", DeleteAsync);
        App.MapGet($"{BaseUri}/tree/{{userId}}", GetTreeAsync);
    }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync([FromServices] IEventBus eventBus, Guid userId)
    {
        var query = new DirectoryTreeQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, AddDirectoryDto param)
    {
        var query = new AddDirectoryCommand(param.Name, param.Sort, param.ParentId, param.UserId);
        await eventBus.PublishAsync(query);
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, UpdateDirectoryDto param)
    {
        var query = new UpdateDirectoryCommand(param.Id, param.Name, param.Sort, param.UserId);
        await eventBus.PublishAsync(query);
    }

    public async Task DeleteAsync([FromServices] IEventBus eventBus, Guid id,Guid userId)
    {
        var query = new RemoveDirectoryCommand(id, userId);
        await eventBus.PublishAsync(query);
    }
}
